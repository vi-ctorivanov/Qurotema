using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIFollow : MonoBehaviour {

	[Header("References")]
	public PlayerMove playerScript;
	public MouseLook look;

	[Header("Input")]
	private InputAction cursorAction;
	private InputAction markerAction;

	[Header("Dynamics")]
	public float distanceFromCamera = 1.2f;
	public float followSpeed = 0.5f;
	public float fadeDelay = 0f;
	public Vector3 cameraAngularContribution = Vector3.zero;
	public Vector3 cameraTranslationalContribution = Vector3.zero;
	public float overallContribution = 1f;
	public string triggerLayer;
	private float targetOpacity = 0.9f;
	private float fadeSpeed = 10f;
	private float distanceFromCameraDifference = 0.3f;
	private Vector3 lastCameraPosition = Vector3.zero;
	private Vector3 cameraAngularVelocity = Vector3.zero;
	private Vector3 cameraTranslationalVelocity = Vector3.zero;
	
	[Header("States")]
	private float opacity = 0f;
	private float minDistanceFromCamera = 0f;

	[Header("Coroutines")]
	private Coroutine fader;
	
	void Start () {
		minDistanceFromCamera = distanceFromCamera - distanceFromCameraDifference;

		cursorAction = InputSystem.actions.FindAction("Cursor");
		markerAction = InputSystem.actions.FindAction("Marker");
	}

	void Update() {
		//modes
		if (Nox.Instance.player) {
			switch (triggerLayer) {
				case "flight":
					if (playerScript.flying) {
						if (fader != null) StopCoroutine(fader);
						if (opacity != targetOpacity) opacity = Mathf.Lerp(opacity, targetOpacity, (fadeSpeed / 2f) * fadeDelay * Time.deltaTime);
					} else {
						if (fader != null) StopCoroutine(fader);
						if (opacity != 0f) opacity = Mathf.Lerp(opacity, 0f, fadeSpeed / 2f * Time.deltaTime);
					}
					break;

				case "control":
					if (cursorAction.WasPressedThisFrame()) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(targetOpacity));
					}

					if (cursorAction.WasReleasedThisFrame()) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(0f));
					}

					if (markerAction.WasPressedThisFrame()) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(0f));
					}

					if (playerScript.flying) {
						if (fader != null) StopCoroutine(fader);
						if (opacity != 0f) opacity = Mathf.Lerp(opacity, 0f, fadeSpeed / 2f * Time.deltaTime);
					}
					break;

				case "movement":
					if (markerAction.WasPressedThisFrame()) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(targetOpacity));
					}

					if (markerAction.WasReleasedThisFrame()) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(0f));
					}

					if (cursorAction.WasPressedThisFrame()) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(0f));
					}

					if (playerScript.flying) {
						if (fader != null) StopCoroutine(fader);
						if (opacity != 0f) opacity = Mathf.Lerp(opacity, 0f, fadeSpeed / 2f * Time.deltaTime);
					}
					break;
			}
		}

		GetComponent<CanvasGroup>().alpha = opacity;

		follow();
	}

	void follow() {
		if (Camera.main) {
			float targetDistance = Nox.Instance.remap(playerScript.targetFOV, playerScript.defaultFOV, playerScript.fastFOV, distanceFromCamera, minDistanceFromCamera);
			Vector3 targetPosition = new Vector3(0f, 0f, targetDistance);

			/*
			Calculate camera-relative camera velocity and angular velocity to offset UI in opposite direction to create sense of weighty sway.
			The actual velocity calculations are more like representations of velocity, since they're easier to calculate.

			Other solutions like a lerping follow did not work very nicely because small changes to time.deltaTime resulted in
			major jittering.
			*/

			/*
			The rotation is just looking at mouse input, since the camera never turns without user input.
			Of course this means that we can never allow input through while restricting the camera.
			*/
			Vector3 tempAngular = new Vector3(-Camera.main.GetComponent<MouseLook>().mouseX, Camera.main.GetComponent<MouseLook>().mouseY, 0f);
			cameraAngularVelocity = Vector3.Lerp(cameraAngularVelocity, tempAngular, followSpeed * Time.deltaTime);

			/*
			The translation is the distance between the camera's current and previous location.
			We do need to transform this vector so it's relative to the camera's forward direction to know where the UI should offset towards.
			*/
			Vector3 tempTranslational = lastCameraPosition - Camera.main.transform.position;
			cameraTranslationalVelocity = Vector3.Lerp(cameraTranslationalVelocity, Camera.main.transform.InverseTransformDirection(tempTranslational), followSpeed * Time.deltaTime);

			//apply offset
			float x = (cameraAngularVelocity.x * cameraAngularContribution.x) + (cameraTranslationalVelocity.x * cameraTranslationalContribution.x);
			float y = (cameraAngularVelocity.y * cameraAngularContribution.y) + (cameraTranslationalVelocity.y * cameraTranslationalContribution.y);
			float z = cameraTranslationalVelocity.z * cameraTranslationalContribution.z;
			Vector3 offset = new Vector3(x, y, z) * overallContribution;
			targetPosition += offset;

			transform.localPosition = targetPosition;
			transform.rotation = Camera.main.transform.rotation;

			lastCameraPosition = Camera.main.transform.position;
		}
	}

	IEnumerator Fade(float target) {
		yield return new WaitForSeconds(fadeDelay);

		while (Mathf.Abs(opacity - target) > 0.01f) {
			yield return new WaitForSeconds(0.01f);
			opacity = Mathf.Lerp(opacity, target, fadeSpeed * Time.deltaTime);
		}

		opacity = target;
	}
}