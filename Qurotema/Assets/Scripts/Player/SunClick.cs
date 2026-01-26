/*

Manages sun click behavior, playing an animation and
altering post processing and audio.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SunClick : MonoBehaviour {

	[Header("References")]
	public AudioMixer mix;
	public GameObject pp;
	public Sound soundSystem;
	public Camera camComponent;

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("States")]
	private float FOV;
	private bool routineEnded = false;
	private bool negative = false;

	[Header("Coroutine")]
	public Coroutine transitioning;

	void Update() {
		if (routineEnded && transitioning != null) {
			StopCoroutine(transitioning);
			transitioning = null;
			routineEnded = false;
		}

		if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "Sun") {
					soundSystem.addEnergy(5f);
					if (transitioning != null) StopCoroutine(transitioning);
					transitioning = StartCoroutine(SwitchWorlds());
				}
			}
		}
	}

	IEnumerator SwitchWorlds() {
		negative = !negative;

		float cut;
		mix.GetFloat("LP_Freq", out cut);
		FOV = camComponent.fieldOfView;

		while (FOV > 10f) {
			yield return new WaitForSeconds(0.01f);
			FOV = Mathf.Lerp(FOV, 9.9f, 10f * Time.deltaTime);
			cut = Mathf.Lerp(cut, 3000f, 0.1f * Time.deltaTime);
			mix.SetFloat("LP_Freq", cut);
			camComponent.fieldOfView = FOV;
		}

		//filter cutoff
		if (negative) mix.SetFloat("LP_Freq", 1500f);
		else mix.SetFloat("LP_Freq", 22000f);

		//post-processing effects
		if (negative) pp.SetActive(true);
		else pp.SetActive(false);

		while (FOV < 65f) {
			yield return new WaitForSeconds(0.01f);
			FOV = Mathf.Lerp(FOV, 66f, 5f * Time.deltaTime);
			camComponent.fieldOfView = FOV;
		}

		routineEnded = true;
	}
}