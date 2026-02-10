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
	public float followSpeed = 0.5f;

	[Header("Colors")]
	public Color red = new Color(100f, 0f, 0f);
	public Color purple = new Color(5f, 5f, 100f);

	void Start () {
		transform.position = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromCamera);
		mat = GetComponent<MeshRenderer>().material;
		trail = GetComponent<TrailRenderer>().material;
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
		markerAction = InputSystem.actions.FindAction("Marker");
	}

	void Update () {
		if (Nox.Instance.introductionFinished) {
			if (!cursorAction.IsPressed()) {
				GetComponent<MeshRenderer>().enabled = false;
				GetComponent<TrailRenderer>().enabled = false;
				makePassive();
			} else {
				GetComponent<MeshRenderer>().enabled = true;
				GetComponent<TrailRenderer>().enabled = true;

				if (interactAction.WasPressedThisFrame()) makeActive();
				if (interactAction.WasReleasedThisFrame()) makePassive();
			}
		} else {
			GetComponent<MeshRenderer>().enabled = false;
			GetComponent<TrailRenderer>().enabled = false;
		}

		if (Nox.Instance.player) {
			//override in movement and flight modes
			if (markerAction.IsPressed() || Nox.Instance.player.GetComponent<PlayerMove>().flying) {
				GetComponent<MeshRenderer>().enabled = false;
				GetComponent<TrailRenderer>().enabled = false;
			}

			//audio
			if (cursorAction.IsPressed() && !Nox.Instance.player.GetComponent<PlayerMove>().flying && !markerAction.IsPressed()) {
				Sound.Instance.addEnergy(1f);
			}

			if (cursorAction.WasPressedThisFrame() && !Nox.Instance.player.GetComponent<PlayerMove>().flying && !markerAction.IsPressed()) {
				Sound.Instance.dynamicToggle("rhythms", true);
			}

			if (cursorAction.WasReleasedThisFrame()) {
				Sound.Instance.dynamicToggle("rhythms", false);
			}
		}

		Vector3 targetPosition = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromCamera);
		//no Time.deltaTime to keep it feeling a bit smoother, framerate independence is less important here
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed); 
	}

	void makeActive() {
		mat.SetColor("_EmissiveColor", red);
		trail.SetColor("_EmissiveColor", red);
	}

	void makePassive() {
		mat.SetColor("_EmissiveColor", purple);
		trail.SetColor("_EmissiveColor", purple);
	}
}