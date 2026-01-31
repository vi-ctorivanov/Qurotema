using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour {

	[Header("References")]
	public GameObject lightObject;
	public Transform platform;
	private Material lightMat;

	[Header("Pad Definition")]
	public string tone;
	public int count;

	[Header("States")]
	private bool active;
	private bool ready = true;
	private float minAlpha = 0.1f;
	private float maxAlpha = 1f;

	[Header("Coroutines")]
	private Coroutine refreshRoutine;
	private Coroutine glowRoutine;

	void Start() {
		lightMat = lightObject.GetComponent<Renderer>().material;
		lightMat.SetFloat("_Alpha", 0f);

		//move platform down and align to normal so that it matches terrain topology
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
			transform.position = hit.point + new Vector3(0f, -0.1f, 0f);

			//in order to avoid any grid pattern breaking on y axis, the pads parent needs to be aligned to the y axis (0, 180, -180, etc.)
			transform.up = hit.normal;
		}

		//rotate platform randomly to vary up texture
		platform.localEulerAngles = new Vector3(platform.localEulerAngles.x, Random.Range(0, 3) * 90f, platform.localEulerAngles.z);
	}

	void Update() {
		if (Sound.Instance.beat == count && active && Sound.Instance.beatChange) {
			if (glowRoutine != null) StopCoroutine(glowRoutine);
			glowRoutine = StartCoroutine(Glow());
			Sound.Instance.addEnergy(0.1f);
			Sound.Instance.shootSound(tone);
			Nox.Instance.terrain.addFeedback(1.0f);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && ready) {
			ready = false;
			active = !active;

			if (active) {
				if (glowRoutine != null) StopCoroutine(glowRoutine);
				lightMat.SetFloat("_Alpha", minAlpha);
				Nox.Instance.padPlayed();
			} else {
				if (glowRoutine != null) StopCoroutine(glowRoutine);
				lightMat.SetFloat("_Alpha", 0f);
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			if (refreshRoutine != null) StopCoroutine(refreshRoutine);
			refreshRoutine = StartCoroutine(Refresh());
		}
	}

	IEnumerator Glow() {
		float alpha = maxAlpha;
		lightMat.SetFloat("_Alpha", alpha);

		while (alpha > minAlpha) {
			yield return new WaitForSeconds(0.01f);
			alpha = Mathf.Lerp(alpha, minAlpha, 2f * Time.deltaTime);
			lightMat.SetFloat("_Alpha", alpha);
		}
	}

	IEnumerator Refresh() {
		yield return new WaitForSeconds(0.5f);
		ready = true;
	}
}