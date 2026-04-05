/*

Manages player movement:
- Moving
- Sprinting
- Jumping
- Flying
- Acceleration and deceleration
- Collision

All done using a custom physics system, since we want a very
particular feel to the character movement, where they slide
across the surface smoothly and have large floaty jumps when
combined with existing momentum.

*/

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour {

	[Header("References")]
	public GameObject cam;
	public Transform colliders;
	public Material ribbonsBottom;

	//input
	private InputAction quitAction;
	private InputAction sprintAction;
	private InputAction jumpAction;
	private InputAction moveAction;
	private InputAction flightAction;
	private InputAction cursorAction;
	private InputAction interactAction;
	private InputAction markerAction;

	[Header("Dynamics")]
	public LayerMask mask;
	private float collisionPushback = 0.1f;

	[Header("Speed")]
	public float walkSpeed = 20f;
	public float sprintSpeed = 80f;

	//verticality
	private float jumpSpeed = 20f;
	private float terminalVelocity = -15f;
	private float flyHeight = 100f;
	private float gravityEase = 1f;
	private float groundedHeight = 1f;
	private float floatDistance = 10f;
	private float graceSpace = 0.1f;
	private float jumpDelayTime = 0.5f;

	//acceleration
	private float speedChangeWalk = 2f;
	private float speedChangeSprint = 2f;
	private float boostBoost = 2.5f;
	private float speedChangeStop = 2f;
	private float directionChangeSpeed = 3f;
	private float airDampening = 0.2f;
	private float flyEase = 1f;
	private float flightSpeedMultiplier = 2f;
	private float flightControlMultiplier = 3f;

	[Header("States")]
	public bool flying = false;
	private bool jumpTrigger = false;
	public bool jumping = false;
	private bool sprinting = false;
	private float targetSpeed = 0f;
	public float verticalForce = 0f;
	private float bottomDistanceFromCenter = 1f;
	public Vector2 targetDirection = new Vector2(0f, 0f);
	private bool ready = false;

	private void OnEnable() {
		Nox.OnIntroductionFinished += getReady;
	}

	private void OnDisable() {
		Nox.OnIntroductionFinished -= getReady;
	}
	
	void Start() {
		quitAction = InputSystem.actions.FindAction("Quit");
		sprintAction = InputSystem.actions.FindAction("Sprint");
		jumpAction = InputSystem.actions.FindAction("Jump");
		moveAction = InputSystem.actions.FindAction("Move");
		flightAction = InputSystem.actions.FindAction("Flight");
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
		markerAction = InputSystem.actions.FindAction("Marker");
	}

	void Update() {
		if (ready) {
			handleKeys();
			move();
		}

		handleSound();

		//set ribbon visibility
		if (flying) ribbonsBottom.SetFloat("_Alpha", Mathf.Lerp(ribbonsBottom.GetFloat("_Alpha"), 1f, 1f * Time.deltaTime));
		else ribbonsBottom.SetFloat("_Alpha", Mathf.Lerp(ribbonsBottom.GetFloat("_Alpha"), 0f, 1f * Time.deltaTime));
	}

	void getReady() {
		ready = true;
	}

	void handleKeys() {
		if (quitAction.WasReleasedThisFrame()) StartCoroutine(quit());

		//switch to flying mode only if no mouse buttons are pressed
		if (flightAction.WasPressedThisFrame() && !cursorAction.IsPressed() && !interactAction.IsPressed() && !markerAction.IsPressed()) {
			flying = !flying;
			//change speeds and dampening parameters when flying for quicker and snappier movement in-air
			if (flying) {
				walkSpeed *= flightSpeedMultiplier;
				sprintSpeed *= flightSpeedMultiplier;

				speedChangeWalk *= flightControlMultiplier;
				speedChangeSprint *= flightControlMultiplier;
				speedChangeStop *= flightControlMultiplier;
				directionChangeSpeed *= flightControlMultiplier;

				Sound.Instance.flyPointState.setParameterByName("Volume", 1);
			} else {
				walkSpeed /= flightSpeedMultiplier;
				sprintSpeed /= flightSpeedMultiplier;

				speedChangeWalk /= flightControlMultiplier;
				speedChangeSprint /= flightControlMultiplier;
				speedChangeStop /= flightControlMultiplier;
				directionChangeSpeed /= flightControlMultiplier;

				Sound.Instance.flyPointState.setParameterByName("Volume", 0);
				Sound.Instance.padState.setParameterByName("Volume", 0);
			}
		}
	}

	void handleSound() {
		if (sprintAction.WasPressedThisFrame()) Sound.Instance.percussionState.setParameterByName("Volume", 1);
		if (sprintAction.WasReleasedThisFrame()) Sound.Instance.percussionState.setParameterByName("Volume", 0);

		if (jumping) {
			//todo
		} else {
			//todo
		}
	}

	void move() {
		//get input
		float horizontal = 0f;
		float vertical = 0f;

		horizontal = moveAction.ReadValue<Vector2>().x;
		vertical = moveAction.ReadValue<Vector2>().y;
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
			Vector3 colliderToPlayer = Vector3.Normalize(collision.transform.position - transform.position);
			transform.position = new Vector3(transform.position.x - colliderToPlayer.x * collisionPushback, newLoc.y, transform.position.z - colliderToPlayer.z * collisionPushback);

		} else transform.position = newLoc;

		//limit player to bounds
		if (transform.position.z > 2900f) transform.position = new Vector3(transform.position.x, transform.position.y, 2899f);
		if (transform.position.z < -2900f) transform.position = new Vector3(transform.position.x, transform.position.y, -2899f);
		if (transform.position.x > 2900f) transform.position = new Vector3(2899f, transform.position.y, transform.position.z);
		if (transform.position.x < -2900f) transform.position = new Vector3(-2899f, transform.position.y, transform.position.z);

		//jump
		if (jumpAction.WasPressedThisFrame() && isGrounded() && !jumping && !flying) {
			jumping = true;
			jumpTrigger = true;
		}

		//no movement - stop all forces (excluding vertical force for jumping)
		if (horizontal == 0f && vertical == 0f && isGrounded()) {
			targetSpeed = Mathf.Lerp(targetSpeed, 0f, speedChangeStop * Time.deltaTime);

		//sprint
		} else if (sprintAction.IsPressed()) {
			sprinting = true;
			targetSpeed = Mathf.Lerp(targetSpeed, sprintSpeed, speedChangeSprint * Time.deltaTime);
			if (sprintAction.WasPressedThisFrame() && !jumping) targetSpeed = sprintSpeed * boostBoost;
			
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
			if (quitAction.WasPressedThisFrame()) Application.Quit();
		}
	}
}