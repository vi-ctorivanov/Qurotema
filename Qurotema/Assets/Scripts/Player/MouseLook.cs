/*

Camera controls, animation, and FOV dynamics.

*/

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour {

	[Header("Dynamics")]
	public float mouseSensitivity = 130f;
	public float shakeSpeed = 1f;
	public float shakeQuantity = 1.4f;
	public AnimationCurve flashFOVCurve;
	public LayerMask mask;
	private float clampAngle = 80f;
	private float easeSpeed = 10f;
	private float followSpeed = 8f;
	private float heightOffset = 0.5f;

	[Header("FOV")]
	public float minFOV = 65f;
	public float maxFOV = 140f;
	private float easeFOV = 3f;
	private float boostBoost = 25f;

	//input
	private InputAction lookAction;
	private InputAction sprintAction;
 
	[Header("States")]
	public float mouseX = 0f;
	public float mouseY = 0f;
	public float rotY = 0f;
	public float rotX = 0f;
	public float currentX = 0f;
	public float currentY = 0f;
	public float targetFOV = 0f;
	private Vector3 previousCameraLocation = Vector3.zero;
	private float playerSpeed;
	private float perlinX;
	private float perlinY;
	private float perlinZ;
	private bool ready = false;

	private void OnEnable() {
		Nox.OnFlashFeedback += fovFeedback;
		Nox.OnIntroductionFinished += getReady;
	}

	private void OnDisable() {
		Nox.OnFlashFeedback -= fovFeedback;
		Nox.OnIntroductionFinished -= getReady;
	}

	void Start() {
		//lock cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		//get rotation
		Vector3 rot = transform.localRotation.eulerAngles;
		rotY = rot.y;
		rotX = rot.x;
		currentY = rotY;
		currentX = rotX;

		//get perlin noise seed
		perlinX = Random.Range(0f, 1000f);
		perlinY = Random.Range(0f, 1000f);
		perlinZ = Random.Range(0f, 1000f);

		lookAction = InputSystem.actions.FindAction("Look");
		sprintAction = InputSystem.actions.FindAction("Sprint");

		targetFOV = minFOV;
	}

	void Update() {
		handleInput();
		rotate();
		follow();
		shake();
		fov();
	}

	private void getReady() {
		ready = true;
	}

	private void handleInput() {
		//get input
		mouseX = lookAction.ReadValue<Vector2>().x;
		mouseY = -lookAction.ReadValue<Vector2>().y;

		if (!ready) {
			mouseX = 0;
			mouseY = 0;
		}

		//rotation manipulation (no need to scale by deltaTime as mouse axis are already frame deltas)
		rotY += mouseX * mouseSensitivity;
		rotX += mouseY * mouseSensitivity;
		rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
	}

	private void rotate() {
		//ease rotation
		currentX = Mathf.Lerp(currentX, rotX, easeSpeed * Time.deltaTime);
		currentY = Mathf.Lerp(currentY, rotY, easeSpeed * Time.deltaTime);

		//apply rotation
		Vector3 rotation = new Vector3(currentX, currentY, 0.0f);
		Quaternion localRotation = Quaternion.Euler(rotation);
		transform.rotation = localRotation;
	}

	private void follow() {
		if (Nox.Instance.player) {
			Vector3 target = Nox.Instance.player.transform.position;
			target.y += heightOffset;
			transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);

			//move up if clipping
			RaycastHit hit;
			if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z), -Vector3.up, out hit, 50f, mask)) {
				if (hit.point.y > transform.position.y - 2f) {
					transform.position = new Vector3(transform.position.x, hit.point.y + 2f, transform.position.z);
				}
			}
		}
	}

	private void shake() {
		if (Nox.Instance.player) {
			//increment perlin 'cursor'
			perlinX += shakeSpeed * Time.deltaTime;
			perlinY += shakeSpeed * Time.deltaTime;
			perlinZ += shakeSpeed * Time.deltaTime;

			//remap to -1 to 1 and amplify according to shake quantity
			float x = Nox.Instance.remap(Mathf.PerlinNoise(perlinX, 0), 0f, 1f, -1f, 1f) * shakeQuantity;
			float y = Nox.Instance.remap(Mathf.PerlinNoise(perlinY, 0), 0f, 1f, -1f, 1f) * shakeQuantity;
			float z = Nox.Instance.remap(Mathf.PerlinNoise(perlinZ, 0), 0f, 1f, -1f, 1f) * shakeQuantity;

			//use player speed as subtraction to shake speed modifier
			//the faster the player moves, the less camera shake there is
			playerSpeed = Nox.Instance.player.GetComponent<PlayerMove>().getSpeed();

			float shakeSpeedModifier = 1f - (playerSpeed * 0.005f);
			if (shakeSpeedModifier < 0) shakeSpeedModifier = 0;

			//apply perlin noise as rotation
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + (x * shakeSpeedModifier), transform.localEulerAngles.y + (y * shakeSpeedModifier), transform.localEulerAngles.z + (z * shakeSpeedModifier));
		}
	}

	private void fov() {
		float velocity = Vector3.Distance(transform.position, previousCameraLocation) / Time.deltaTime;
		previousCameraLocation = transform.position;

		float extraFOV = 0f;
		if (Nox.Instance.player.GetComponent<PlayerMove>().flying) extraFOV += 10f;
		//boost FOV by pushing up targetFOV spectrum - boosting the FOV itself is too jarring
		//the boostBoost value shouldn't be too high though, or else the FOV will once again undesireably jump abruptly
		if (sprintAction.WasPressedThisFrame() && !Nox.Instance.player.GetComponent<PlayerMove>().jumping) extraFOV += boostBoost;

		targetFOV = Mathf.Lerp(targetFOV, Nox.Instance.remap(velocity, 0f, 300f, minFOV + extraFOV, maxFOV + extraFOV), easeFOV * Time.deltaTime);

		GetComponent<Camera>().fieldOfView = targetFOV;
	}

	private void fovFeedback(float intensity) {
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