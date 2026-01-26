using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFollow : MonoBehaviour {

	[Header("References")]
	public PlayerMove playerScript;

	[Header("Dynamics")]
	public float distanceFromCamera = 1.2f;
	public float followSpeed = 45f;
	public float fadeDelay = 0f;
	public string triggerLayer;
	private float targetOpacity = 0.9f;
	private float fadeSpeed = 10f;
	private float distanceFromCameraDifference = 0.3f;
	
	[Header("States")]
	private float opacity = 0f;
	private float minDistanceFromCamera = 0f;
	private float targetDistance = 0f;

	[Header("Coroutines")]
	private Coroutine fader;
	
	void Start () {
		transform.position = Camera.main.transform.position + (Camera.main.transform.forward * targetDistance);
		transform.rotation = Camera.main.transform.rotation;

		targetDistance = distanceFromCamera;

		minDistanceFromCamera = distanceFromCamera - distanceFromCameraDifference;
	}

	void Update() {
		
		//modes
		switch (triggerLayer) {
			case "flight":
				if (Nox.getPlayer().GetComponent<PlayerMove>().flying) {
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

				if (Nox.getPlayer().GetComponent<PlayerMove>().flying) {
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

				if (Nox.getPlayer().GetComponent<PlayerMove>().flying) {
					if (fader != null) StopCoroutine(fader);
					if (opacity != 0f) opacity = Mathf.Lerp(opacity, 0f, fadeSpeed / 2f * Time.deltaTime);
				}
				break;
		}

		GetComponent<CanvasGroup>().alpha = opacity;

		follow();
	}

	void follow() {
		targetDistance = Nox.remap(playerScript.targetFOV, playerScript.defaultFOV, playerScript.fastFOV, distanceFromCamera, minDistanceFromCamera);
		Vector3 targetPosition = Camera.main.transform.position + (Camera.main.transform.forward * targetDistance);

		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
		transform.rotation = Camera.main.transform.rotation;
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