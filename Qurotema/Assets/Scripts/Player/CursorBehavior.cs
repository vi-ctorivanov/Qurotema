/*

Manages mouse 'cursor'.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorBehavior : MonoBehaviour {

	[Header("References")]
	private Material mat;
	private Material trail;

	[Header("Input")]
	private InputAction cursorAction;
	private InputAction interactAction;
	private InputAction markerAction;

	[Header("Dynamics")]
	public float distanceFromCamera = 5f;
	public float followSpeed = 50f;
	public float alphaSpeed = 0.03f;

	[Header("States")]
	public bool on = false;

	[Header("Colors")]
	public Color red = new Color(100f, 0f, 0f);
	public Color purple = new Color(5f, 5f, 100f);

	[Header("Coroutines")]
	private Coroutine routine;

	void Start () {
		transform.position = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromCamera);
		mat = GetComponent<MeshRenderer>().material;
		trail = GetComponent<TrailRenderer>().material;

		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
		markerAction = InputSystem.actions.FindAction("Marker");

		mat.SetFloat("_Alpha", 0f);
		trail.SetFloat("_Alpha", 0f);
	}

	void Update () {
		if (Nox.Instance.introductionFinished) {
			if (!cursorAction.IsPressed()) makePassive();
			else {
				if (interactAction.WasPressedThisFrame()) makeActive();
				if (interactAction.WasReleasedThisFrame()) makePassive();
			}
		}

		if (Nox.Instance.player) {
			//override in movement and flight modes
			if ((markerAction.IsPressed() || Nox.Instance.player.GetComponent<PlayerMove>().flying) && on) toggleCursor(false);

			if (cursorAction.WasPressedThisFrame() && !Nox.Instance.player.GetComponent<PlayerMove>().flying && !markerAction.IsPressed()) toggleCursor(true);

			if (cursorAction.WasReleasedThisFrame()) toggleCursor(false);
		}

		Vector3 targetPosition = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromCamera);
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime); 
	}

	void makeActive() {
		mat.SetColor("_Color", red);
		trail.SetColor("_Color", red);
	}

	void makePassive() {
		mat.SetColor("_Color", purple);
		trail.SetColor("_Color", purple);
	}

	void toggleCursor(bool on) {
		if (routine != null) StopCoroutine(routine);
		routine = StartCoroutine(toggle(on));
	}

	IEnumerator toggle(bool t) {
		int boolInt = t ? 1 : 0;
		Sound.Instance.rhythmState.setParameterByName("Volume", boolInt);

		on = t;

		float alpha = 0f;
		if (!t) alpha = 1f;

		while (true) {
			yield return new WaitForSeconds(0.01f);

			mat.SetFloat("_Alpha", alpha);
			trail.SetFloat("_Alpha", alpha);

			if (t) {
				alpha += alphaSpeed;
				if (alpha > 1f) yield break;
			} else {
				alpha -= alphaSpeed;
				if (alpha < 0f) yield break;
			}
		}
	}
}