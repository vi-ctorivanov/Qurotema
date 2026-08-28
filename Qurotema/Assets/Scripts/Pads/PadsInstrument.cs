using System.Collections;
using UnityEngine;

public class PadsInstrument : MonoBehaviour {

	[Header("References")]
	public GameObject lightObject;
	public Transform platform;
	private MeshRenderer lightMat;

	[Header("Definition")]
	public string tone;
	public int count;

	//dynamics
	private MaterialPropertyBlock mpb;

	//states
	private bool active;
	private bool ready = true;
	private float minAlpha = 0.1f;
	private float maxAlpha = 1f;

	//coroutines
	private Coroutine refreshRoutine;
	private Coroutine glowRoutine;

	private void OnEnable() {
		Sound.OnEighth += playBeat;
	}

	private void OnDisable() {
		Sound.OnEighth -= playBeat;
	}

	void Start() {
		lightMat = lightObject.GetComponent<MeshRenderer>();
		mpb = new MaterialPropertyBlock();
		mpb.SetFloat("_Alpha", 0f);
		lightMat.SetPropertyBlock(mpb);

		//move platform down and align to normal so that it matches terrain topology
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
			transform.position = hit.point + new Vector3(0f, -0.1f, 0f);
			transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
		}

		//rotate platform randomly to vary up texture
		platform.localEulerAngles = new Vector3(platform.localEulerAngles.x, Random.Range(0, 3) * 90f, platform.localEulerAngles.z);
	}

	void playBeat(int beat) {
		if (active && beat == count) {
			if (glowRoutine != null) StopCoroutine(glowRoutine);
			glowRoutine = StartCoroutine(Glow());
			playSound();
		}
	}

	private void playSound() {
		int toneInt = 0;
		switch (tone) {
			case "kick": toneInt = 0; break;
			case "snare": toneInt = 1; break;
			case "hat": toneInt = 2; break;
			default: toneInt = 0; break;
		}
		Sound.Instance.playOneShotWithParameters(Sound.Instance.padsEvent, ("PercussionNote", toneInt));
		Nox.Instance.terrain.addFeedback(1.0f);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && ready) {
			ready = false;
			active = !active;

			if (active) {
				if (glowRoutine != null) StopCoroutine(glowRoutine);
				mpb.SetFloat("_Alpha", minAlpha);
				lightMat.SetPropertyBlock(mpb);
				Nox.Instance.padPlayed();
			} else {
				if (glowRoutine != null) StopCoroutine(glowRoutine);
				mpb.SetFloat("_Alpha", 0f);
				lightMat.SetPropertyBlock(mpb);
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
		mpb.SetFloat("_Alpha", alpha);
		lightMat.SetPropertyBlock(mpb);

		while (alpha > minAlpha) {
			yield return new WaitForSeconds(0.01f);
			alpha = Mathf.Lerp(alpha, minAlpha, 2f * Time.deltaTime);
			mpb.SetFloat("_Alpha", alpha);
			lightMat.SetPropertyBlock(mpb);
		}
	}

	IEnumerator Refresh() {
		yield return new WaitForSeconds(0.5f);
		ready = true;
	}
}