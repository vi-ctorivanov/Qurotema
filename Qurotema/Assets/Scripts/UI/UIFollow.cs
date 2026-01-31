using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFollow : MonoBehaviour {

	[Header("References")]
	public PlayerMove playerScript;
	public MouseLook look;

	[Header("Dynamics")]
	public float distanceFromCamera = 1.2f;
	public float followSpeed = 45f;
	public float fadeDelay = 0f;
	public string triggerLayer;
	private float targetOpacity = 0.9f;
	private float fadeSpeed = 10f;
	private float distanceFromCameraDifference = 0.3f;
	public LayerMask mask;
	
	[Header("States")]
	private float opacity = 0f;
	private float minDistanceFromCamera = 0f;
	private float targetDistance = 0f;

	[Header("Coroutines")]
	private Coroutine fader;

	private Vector3 lastCameraPos = Vector3.zero;
	
	void Start () {
		transform.position = Camera.main.transform.position + (Camera.main.transform.forward * targetDistance);
		transform.rotation = Camera.main.transform.rotation;

		targetDistance = distanceFromCamera;

		minDistanceFromCamera = distanceFromCamera - distanceFromCameraDifference;
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
					if (Input.GetMouseButtonDown(1)) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(targetOpacity));
					}

					if (Input.GetMouseButtonUp(1)) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(0f));
					}

					if (Input.GetMouseButtonDown(2)) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(0f));
					}

					if (playerScript.flying) {
						if (fader != null) StopCoroutine(fader);
						if (opacity != 0f) opacity = Mathf.Lerp(opacity, 0f, fadeSpeed / 2f * Time.deltaTime);
					}
					break;

				case "movement":
					if (Input.GetMouseButtonDown(2)) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(targetOpacity));
					}

					if (Input.GetMouseButtonUp(2)) {
						if (fader != null) StopCoroutine(fader);
						fader = StartCoroutine(Fade(0f));
					}

					if (Input.GetMouseButtonDown(1)) {
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
			/*
			the core issue is that we want a subtle lerp that is really close to its final position, but without overshooting OR clamping,
			because the second it is somehow limited, its motion has become irregular as the next frame will see it oscillating back to a non overshot / clamped position
			*/
			targetDistance = Nox.Instance.remap(playerScript.targetFOV, playerScript.defaultFOV, playerScript.fastFOV, distanceFromCamera, minDistanceFromCamera);
			Vector3 targetPosition = Camera.main.transform.position + (Camera.main.transform.forward * targetDistance);
			transform.position = Vector3.Lerp(transform.position, targetPosition, Mathf.Clamp(followSpeed * Time.deltaTime, 0f, 0.99f));
			transform.rotation = Camera.main.transform.rotation;
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