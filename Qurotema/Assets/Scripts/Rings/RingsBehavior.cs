/*

Manages rings instrument.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RingsBehavior : MonoBehaviour {

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

				Sound.Instance.addEnergy(0.5f);
				Sound.Instance.shootSound("rings", int.Parse(hit.collider.tag) - 1);
				Nox.Instance.ringPlayed();
			}
		}
	}
}