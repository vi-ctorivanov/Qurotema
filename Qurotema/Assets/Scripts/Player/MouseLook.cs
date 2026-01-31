/*

Camera controls and animation.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

	[Header("Dynamics")]
	public float mouseSensitivity = 130f;
	public float clampAngle = 80f;
	public float easeSpeed = 2f;
	public float shakeSpeed = 1f;
	public float shakeQuantity = 1.4f;
	public float followSpeed = 8f;
	public float heightOffset = 0.5f;
	public LayerMask mask;
 
	[Header("States")]
	private float mouseX = 0f;
	private float mouseY = 0f;
	private float rotY = 0f;
	private float rotX = 0f;
	private float currentX = 0f;
	private float currentY = 0f;
	private float playerSpeed;
	private float perlinX;
	private float perlinY;
	private float perlinZ;
	private bool ready = false;

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
	}

	void Update() {
		if (!ready) {
			if (Nox.Instance.introductionFinished) ready = true;
		}

		handleInput();
		rotate();
		follow();
		shake();
	}

	void handleInput() {
		//get input
		mouseX = Input.GetAxis("Mouse X");
		mouseY = -Input.GetAxis("Mouse Y");

		if (!ready) {
			mouseX = 0;
			mouseY = 0;
		}

		//rotation manipulation (no need to scale by deltaTime as mouse axis are already frame deltas)
		rotY += mouseX * mouseSensitivity;
		rotX += mouseY * mouseSensitivity;
		rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
	}

	void rotate() {
		//ease rotation
		currentX = Mathf.Lerp(currentX, rotX, easeSpeed * Time.deltaTime);
		currentY = Mathf.Lerp(currentY, rotY, easeSpeed * Time.deltaTime);

		//apply rotation
		Vector3 rotation = new Vector3(currentX, currentY, 0.0f);
		Quaternion localRotation = Quaternion.Euler(rotation);
		transform.rotation = localRotation;
	}

	void follow() {
		if (Nox.Instance.player) {
			transform.position = Vector3.Lerp(transform.position, new Vector3(Nox.Instance.player.transform.position.x, Nox.Instance.player.transform.position.y + heightOffset, Nox.Instance.player.transform.position.z), followSpeed * Time.deltaTime);

			//move up if clipping
			RaycastHit hit;
			if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z), -Vector3.up, out hit, 50f, mask)) {
				if (hit.point.y > transform.position.y - 2f) {
					transform.position = new Vector3(transform.position.x, hit.point.y + 2f, transform.position.z);
				}
			}
		}
	}

	void shake() {
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
}