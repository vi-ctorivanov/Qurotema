/*

Manages sun and gates sphere click behavior, playing an animation and
altering post processing and audio.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class SunBehavior : MonoBehaviour {

	[Header("References")]
	public GameObject pp;
	public OrbitingSun sun;

	[Header("Input")]
	private InputAction cursorAction;
	private InputAction interactAction;

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("States")]
	private float FOV;
	private bool routineEnded = false;
	private bool negative = false;
	private bool proxim = false;

	[Header("Coroutine")]
	public Coroutine transitioning;

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
	}

	void Update() {
		if (routineEnded && transitioning != null) {
			StopCoroutine(transitioning);
			transitioning = null;
			routineEnded = false;
		}

		if (cursorAction.IsPressed() && interactAction.WasPressedThisFrame()) {
			RaycastHit hit;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "Sun") toggleProxim();

				if (hit.collider.tag == "GatesSphere" && sun.gates) {
					if (transitioning != null) StopCoroutine(transitioning);
					transitioning = StartCoroutine(SwitchWorlds());
				}
			}
		}
	}

	private void toggleProxim() {
		proxim = !proxim;
		sun.proxim = proxim;

		//todo some audio stuff
	}

	IEnumerator SwitchWorlds() {
		negative = !negative;

		//todo
		//float cut;
		//mix.GetFloat("LP_Freq", out cut);

		//cut = Mathf.Lerp(cut, 3000f, 0.1f * Time.deltaTime);
		//mix.SetFloat("LP_Freq", cut);

		//filter cutoff...

		yield return new WaitForSeconds(0.1f);

		//post-processing effects
		if (negative) pp.SetActive(true);
		else pp.SetActive(false);

		routineEnded = true;
	}
}