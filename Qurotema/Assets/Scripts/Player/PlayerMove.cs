/*

Manages player movement:
- Moving
- Sprinting
- Jumping
- Flying
- Acceleration and deceleration
- Collision
- FOV dynamics

All done using a custom physics system, since we want a very
particular feel to the character movement, where they slide
across the surface smoothly and have large floaty jumps when
combined with existing momentum.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerMove : MonoBehaviour {

	[Header("References")]
	public GameObject cam;
	public Transform colliders;
	public AudioMixer mix;
	public Material ribbonsBottom;

	[Header("Dynamics")]
	public LayerMask mask;
	public float collisionPushback = 0.1f;
	public AnimationCurve flashFOVCurve;

	[Header("Speed")]
	public float walkSpeed = 20f;
	public float sprintSpeed = 80f;

	[Header("Verticality")]
	public float jumpSpeed = 20f;
	public float terminalVelocity = -20f;
	public float flyHeight = 200f;
	public float gravityEase = 1f;
	public float groundedHeight = 1f;
	public float floatDistance = 10f;
	public float graceSpace = 0.1f;
	public float jumpDelayTime = 0.5f;

	[Header("Acceleration")]
	public float speedChangeWalk = 2f;
	public float speedChangeSprint = 2f;
	public float speedChangeStop = 2f;
	public float directionChangeSpeed = 3f;
	public float airDampening = 0.2f;
	public float flyEase = 4f;
	public float flightSpeedMultiplier = 2f;
	public float flightControlMultiplier = 3f;

	[Header("FOV")]
	public float defaultFOV = 65f;
	public float fastFOV = 90f;
	public float flyingFOV = 100f;
	public float easeFOV = 2f;

	[Header("States")]
	public bool flying = false;
	public bool jumpTrigger = false;
	public bool jumping = false;
	public bool sprinting = false;
	public float targetSpeed = 0f;
	public float targetFOV = 0f;
	public float verticalForce = 0f;
	public float bottomDistanceFromCenter = 1f;
	public Vector2 targetDirection = new Vector2(0f, 0f);
	private bool ready = false;
	
	void Start() {
		targetFOV = defaultFOV;
	}

	void Update() {
		if (!ready) {
			if (Nox.Instance.introductionFinished) ready = true;
		} else {
			handleKeys();
			move();
		}

		setFOV();
		handleSound();

		//set ribbon visibility
		if (flying) ribbonsBottom.SetFloat("_Alpha", Mathf.Lerp(ribbonsBottom.GetFloat("_Alpha"), 1f, 1f * Time.deltaTime));
		else ribbonsBottom.SetFloat("_Alpha", Mathf.Lerp(ribbonsBottom.GetFloat("_Alpha"), 0f, 1f * Time.deltaTime));
	}

	void handleKeys() {
		if (Input.GetKeyUp("escape")) StartCoroutine(quit());

		//switch to flying mode only if no mouse buttons are pressed
		if (Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)) {
			flying = !flying;
			//change speeds and dampening parameters when flying for quicker and snappier movement in-air
			if (flying) {
				walkSpeed *= flightSpeedMultiplier;
				sprintSpeed *= flightSpeedMultiplier;

				speedChangeWalk *= flightControlMultiplier;
				speedChangeSprint *= flightControlMultiplier;
				speedChangeStop *= flightControlMultiplier;
				directionChangeSpeed *= flightControlMultiplier;

				Sound.Instance.dynamicToggle("harmonies", true);
			} else {
				walkSpeed /= flightSpeedMultiplier;
				sprintSpeed /= flightSpeedMultiplier;

				speedChangeWalk /= flightControlMultiplier;
				speedChangeSprint /= flightControlMultiplier;
				speedChangeStop /= flightControlMultiplier;
				directionChangeSpeed /= flightControlMultiplier;

				Sound.Instance.dynamicToggle("harmonies", false);
				Sound.Instance.dynamicToggle("pads", false);
			}
		}
	}

	void handleSound() {
		if (flying && sprinting) Sound.Instance.addEnergy(2.4f);
		else if (flying && !sprinting) Sound.Instance.addEnergy(1.8f);
		else if (sprinting) Sound.Instance.addEnergy(1.4f);
		else if (getSpeed() > 1f) Sound.Instance.addEnergy(0.4f);

		//need listener specifically for a single event
		//repeated calls to dynamicToggle result in loss of functionality
		if (Input.GetKeyDown(KeyCode.LeftShift)) Sound.Instance.dynamicToggle("percussion", true);
		if (Input.GetKeyUp(KeyCode.LeftShift)) Sound.Instance.dynamicToggle("percussion", false);

		if (jumping) {
			float cut;
			mix.GetFloat("Frequency_Cutoff", out cut);
			mix.SetFloat("Frequency_Cutoff", Mathf.Lerp(cut, 1100f, 1f * Time.deltaTime));
		} else {
			float cut;
			mix.GetFloat("Frequency_Cutoff", out cut);
			mix.SetFloat("Frequency_Cutoff", Mathf.Lerp(cut, 10f, 5f * Time.deltaTime));
		}
	}

	void setFOV() {
		if (Camera.main) {
			if (Camera.main.GetComponent<SunClick>().transitioning == null) {
				if (!flying) targetFOV = Mathf.Lerp(targetFOV, Nox.Instance.remap(targetSpeed, walkSpeed, sprintSpeed, defaultFOV, fastFOV), easeFOV * Time.deltaTime);
				Camera.main.GetComponent<Camera>().fieldOfView = targetFOV;
			}
		}
	}

	void move() {
		//get input
		float horizontal = 0f;
		float vertical = 0f;
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f) horizontal = Input.GetAxis("Horizontal");
		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f) vertical = Input.GetAxis("Vertical");
		Vector2 direction = getInput(horizontal, vertical);
		Vector3 newLoc = new Vector3(transform.position.x + direction.x * Time.deltaTime, transform.position.y, transform.position.z + direction.y * Time.deltaTime);

		/*
		An important component of the movement is how, unless the player jumps, they are glued to the ground.
		This is done to create a feeling of 'skating' or 'gliding' across the landscape.
		*/

		//glue to ground, or add gravity while airborne
		if (!flying) {
			if (!jumping) newLoc = groundPlayer(newLoc);
			else {
				if (jumpTrigger) {
					verticalForce = jumpSpeed;
					jumpTrigger = false;
					StartCoroutine(jumpDelay());
				}

				if (verticalForce > 2f) verticalForce = Mathf.Lerp(verticalForce, 0f, gravityEase * Time.deltaTime);
				else verticalForce = Mathf.Lerp(verticalForce, terminalVelocity, gravityEase * Time.deltaTime);

				newLoc.y += verticalForce * Time.deltaTime;
				newLoc.y = preventClip(newLoc);
			}
		} else {
			//fly
			targetFOV = Mathf.Lerp(targetFOV, flyingFOV, easeFOV * Time.deltaTime);

			float floor = 0f;
			RaycastHit hit;
			if (Physics.Raycast(transform.position, -Vector3.up, out hit, 300f, mask)) {
				floor = hit.point.y;
			}

			newLoc = new Vector3(newLoc.x, Mathf.Lerp(transform.position.y, floor + flyHeight, flyEase * Time.deltaTime), newLoc.z);
		}

		//apply movement
		GameObject collision = isColliding(newLoc);
		if (collision != null) {
			//prevent collisions with select objects by moving player away from them
			//it's very rudamentary, but our collisions are simple
			transform.position = new Vector3(transform.position.x, newLoc.y, transform.position.z);
			//Vector3 nearest = collision.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
			Vector3 colliderToPlayer = Vector3.Normalize(collision.transform.position - transform.position);
			transform.position = new Vector3(transform.position.x - colliderToPlayer.x * collisionPushback, newLoc.y, transform.position.z - colliderToPlayer.z * collisionPushback);

		} else transform.position = newLoc;

		//limit player to bounds
		if (transform.position.z > 2900f) transform.position = new Vector3(transform.position.x, transform.position.y, 2899f);
		if (transform.position.z < -2900f) transform.position = new Vector3(transform.position.x, transform.position.y, -2899f);
		if (transform.position.x > 2900f) transform.position = new Vector3(2899f, transform.position.y, transform.position.z);
		if (transform.position.x < -2900f) transform.position = new Vector3(-2899f, transform.position.y, transform.position.z);

		//jump
		if (Input.GetKeyDown(KeyCode.Space) && isGrounded() && !jumping && !flying) {
			jumping = true;
			jumpTrigger = true;
		}

		//no movement - stop all forces (excluding vertical force for jumping)
		if (horizontal == 0f && vertical == 0f && isGrounded()) {
			targetSpeed = Mathf.Lerp(targetSpeed, 0f, speedChangeStop * Time.deltaTime);

		//sprint
		} else if (Input.GetKey(KeyCode.LeftShift)) {
			sprinting = true;
			targetSpeed = Mathf.Lerp(targetSpeed, sprintSpeed, speedChangeSprint * Time.deltaTime);
			
		//walk
		} else {
			sprinting = false;
			targetSpeed = Mathf.Lerp(targetSpeed, walkSpeed, speedChangeWalk * Time.deltaTime);
		}
	}

	Vector2 getInput(float horizontal, float vertical) {
		//calculating direction vector
		Vector3 direction = new Vector3(horizontal, 0.0f, vertical);

		//create rotated transform that is locked to avoid up/down camera angle affecting direction magnitude
		Quaternion cameraRotation = cam.transform.rotation;
		cam.transform.Rotate(Vector3.left, cam.transform.localRotation.eulerAngles.x);

		direction = cam.transform.TransformDirection(direction);
		direction.y = 0.0f;

		//revert camera's rotation
		cam.transform.rotation = cameraRotation;

		//limit input magnitude (to avoid high-magnitude input when moving diagonally)
		direction = Vector3.Normalize(direction);

		//ease direction for smoother movement (dampen direction change if in air)
		float changer = directionChangeSpeed;
		if (jumping) changer *= airDampening;

		targetDirection.x = Mathf.Lerp(targetDirection.x, direction.x, changer * Time.deltaTime);
		targetDirection.y = Mathf.Lerp(targetDirection.y, direction.z, changer * Time.deltaTime);

		//amplify normalized vector to desired speed
		return new Vector2(targetDirection.x, targetDirection.y) * targetSpeed;
	}

	Vector3 groundPlayer(Vector3 location) {
		
		//add small correction (offset upwards) so that a collider on a steep hill doesn't clip through
		RaycastHit hit;
		if (Physics.Raycast(new Vector3(location.x, location.y + graceSpace, location.z), -Vector3.up, out hit, floatDistance, mask)) {
			location = new Vector3(location.x, hit.point.y + bottomDistanceFromCenter + graceSpace, location.z);

		//if distance is big enough (float), make player fall with gravity instead of forcing them to the ground
		} else if (!jumping) {
			jumping = true;
			StartCoroutine(jumpDelay());
		}

		return location;
	}

	float preventClip(Vector3 location) {
		RaycastHit hit;
		if (Physics.Raycast(new Vector3(location.x, location.y + 20f, location.z), -Vector3.up, out hit, 50f, mask)) {
			if (hit.point.y-3f > location.y - bottomDistanceFromCenter) {
				return hit.point.y + bottomDistanceFromCenter + graceSpace;
			}
		}
		return location.y;
	}

	GameObject isColliding(Vector3 location) {
		foreach (Transform child in colliders) {
			if (child.gameObject.GetComponent<Collider>().bounds.Contains(location)) return child.gameObject;
		}
		return null;
	}

	bool isGrounded() {
		return Physics.Raycast(transform.position - new Vector3(0f, bottomDistanceFromCenter, 0f), -Vector3.up, groundedHeight);
	}

	IEnumerator jumpDelay() {
		yield return new WaitForSeconds(jumpDelayTime);

		while(jumping) {
			yield return new WaitForSeconds(0.01f);
			if (isGrounded()) jumping = false;
		}
	}

	public float getSpeed() {
		return targetSpeed / sprintSpeed;
	}

	IEnumerator quit() {
		Nox.Instance.playText("unique_2");

		//5 seconds
		for (int i = 0; i < 500; i++) {
			yield return new WaitForSeconds(0.01f);
			if (Input.GetKeyDown("escape")) Application.Quit();
		}
	}

	public void flashFeedback() {
		StartCoroutine(flashFOV());
	}

	IEnumerator flashFOV() {
		float current = targetFOV;
		for (float i = 0f; i < 1f; i+=0.005f) {
			yield return new WaitForSeconds(0.01f);
			targetFOV = current + flashFOVCurve.Evaluate(i) * 5f;
		}
	}
}