/*

Manages mouse 'cursor'.

*/

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorBehavior : MonoBehaviour {

	[Header("References")]
	public Material cursorTransparent;
	public Material cursorOpaque;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;

	[Header("Dynamics")]
	public float distanceFromCamera = 5f;
	public float followSpeed = 50f;
	public float alphaSpeed = 0.03f;

	[Header("States")]
	public bool on = false;
	private bool ready = false;

	[Header("Colors")]
	public Color red = new Color(100f, 0f, 0f);
	public Color purple = new Color(5f, 5f, 100f);

	//coroutines
	private Coroutine routine;

	private void OnEnable() {
		Nox.OnIntroductionFinished += getReady;
		Nox.OnMovementStop += unReady;
	}

	private void OnDisable() {
		Nox.OnIntroductionFinished -= getReady;
		Nox.OnMovementStop -= unReady;
	}

	void Start () {
		transform.position = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromCamera);

		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");

		cursorTransparent.SetFloat("_Alpha", 0f);
		cursorOpaque.SetFloat("_Alpha", 0f);
	}

	void Update () {
		if (!ready) return;

		if (!cursorAction.IsPressed()) makePassive();
		else {
			if (interactAction.WasPressedThisFrame()) makeActive();
			if (interactAction.WasReleasedThisFrame()) makePassive();
		}

		if (Nox.Instance.player) {
			//override in flight mode (control overrides movement mode)
			if (Nox.Instance.player.GetComponent<PlayerMove>().flying && on) toggleCursor(false);
			if (cursorAction.WasPressedThisFrame() && !Nox.Instance.player.GetComponent<PlayerMove>().flying) toggleCursor(true);
			
			if (cursorAction.WasReleasedThisFrame() && on) toggleCursor(false);
		}

		Vector3 targetPosition = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromCamera);
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime); 
	}

	private void getReady() {
		ready = true;
	}

	private void unReady() {
		ready = false;
	}

	private void makeActive() {
		cursorTransparent.SetColor("_Color", red);
		cursorOpaque.SetColor("_Color", red);
	}

	private void makePassive() {
		cursorTransparent.SetColor("_Color", purple);
		cursorOpaque.SetColor("_Color", purple);
	}

	private void toggleCursor(bool on) {
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

			cursorTransparent.SetFloat("_Alpha", alpha);
			cursorOpaque.SetFloat("_Alpha", alpha);

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