/*

Manages mouse 'cursor'.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBehavior : MonoBehaviour {

	[Header("References")]
	private Material mat;
	private Material trail;

	[Header("Dynamics")]
	public float distanceFromCamera = 5f;
	public float followSpeed = 15f;

	[Header("Colors")]
	private Color red = new Color(100f, 0f, 0f);
	private Color purple = new Color(5f, 5f, 100f);

	void Start () {
		transform.position = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromCamera);
		mat = GetComponent<MeshRenderer>().material;
		trail = GetComponent<TrailRenderer>().material;
	}

	void Update () {
		if (Nox.Instance.introductionFinished) {
			if (!Input.GetMouseButton(1)) {
				GetComponent<MeshRenderer>().enabled = false;
				GetComponent<TrailRenderer>().enabled = false;
				makePassive();
			} else {
				GetComponent<MeshRenderer>().enabled = true;
				GetComponent<TrailRenderer>().enabled = true;

				if (Input.GetMouseButtonDown(0)) makeActive();
				if (Input.GetMouseButtonUp(0)) makePassive();
			}
		} else {
			GetComponent<MeshRenderer>().enabled = false;
			GetComponent<TrailRenderer>().enabled = false;
		}

		if (Nox.Instance.player) {
			//override in movement and flight modes
			if (Input.GetMouseButton(2) || Nox.Instance.player.GetComponent<PlayerMove>().flying) {
				GetComponent<MeshRenderer>().enabled = false;
				GetComponent<TrailRenderer>().enabled = false;
			}

			//audio
			if (Input.GetMouseButton(1) && !Nox.Instance.player.GetComponent<PlayerMove>().flying && !Input.GetMouseButton(2)) {
				Sound.Instance.addEnergy(1f);
			}

			if (Input.GetMouseButtonDown(1) && !Nox.Instance.player.GetComponent<PlayerMove>().flying && !Input.GetMouseButton(2)) {
				Sound.Instance.dynamicToggle("rhythms", true);
			}

			if (Input.GetMouseButtonUp(1)) {
				Sound.Instance.dynamicToggle("rhythms", false);
			}
		}

		Vector3 targetPosition = Camera.main.transform.position + (Camera.main.transform.forward * distanceFromCamera);
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
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