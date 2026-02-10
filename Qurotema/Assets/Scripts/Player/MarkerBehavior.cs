/*

Creates marker on mouse look location when trigger button is held.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarkerBehavior : MonoBehaviour {

	[Header("References")]
	public GameObject marker;

	[Header("Input")]
	private InputAction cursorAction;
	private InputAction interactAction;
	private InputAction markerAction;

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("States")]
	private bool playing = false;

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
		markerAction = InputSystem.actions.FindAction("Marker");
	}

	void Update() {
		if (markerAction.IsPressed() && !cursorAction.IsPressed() && !Nox.Instance.player.GetComponent<PlayerMove>().flying) {
			RaycastHit hit;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
				Instantiate(marker, hit.point, Quaternion.identity);

				if (!playing) {
					playing = true;
					Sound.Instance.dynamicToggle("droplets", true, 5f);
				}

				if (interactAction.WasPressedThisFrame()) {
					Nox.Instance.player.GetComponent<PlayerMove>().targetFOV = 20f;
					Nox.Instance.player.GetComponent<PlayerMove>().verticalForce = 0f;
					Nox.Instance.player.GetComponent<PlayerMove>().targetDirection = Vector2.zero;
					Nox.Instance.player.transform.position = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
					Sound.Instance.addEnergy(3f);
					Sound.Instance.shootSound("whips");
				}
			}
		}

		if (markerAction.WasReleasedThisFrame()) {
			playing = false;
			Sound.Instance.dynamicToggle("droplets", false, 5f);
		}
	}
}