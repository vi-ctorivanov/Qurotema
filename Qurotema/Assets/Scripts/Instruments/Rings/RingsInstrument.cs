/*

Manages rings instrument.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RingsInstrument : MonoBehaviour {

	[Header("Input")]
	private InputAction cursorAction;
	private InputAction interactAction;

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("States")]
	public bool inArea = false;

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
	}

	void Update() {
		if (inArea && cursorAction.IsPressed() && interactAction.WasPressedThisFrame()) {
			RaycastHit hit;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {
				int parsedInt = -1;
				if (int.TryParse(hit.collider.tag, out parsedInt)) {
					Sound.Instance.playOneShotWithParameters(Sound.Instance.ringsEvent, ("ChromaticNote", parsedInt - 1));
					Nox.Instance.ringPlayed();
				}
			}
		}
	}
}