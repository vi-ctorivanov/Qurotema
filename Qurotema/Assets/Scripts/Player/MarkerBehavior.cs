/*

Creates marker on mouse look location when trigger button is held.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerBehavior : MonoBehaviour {

	[Header("References")]
	public Sound soundSystem;
	public GameObject marker;

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("States")]
	private bool playing = false;

	void Update() {
		if (Input.GetMouseButton(2) && !Input.GetMouseButton(1) && !Nox.getPlayer().GetComponent<PlayerMove>().flying) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
				Instantiate(marker, hit.point, Quaternion.identity);

				if (!playing) {
					playing = true;
					soundSystem.dynamicToggle("droplets", true, 5f);
				}

				if (Input.GetMouseButtonDown(0)) {
					Nox.getPlayer().GetComponent<PlayerMove>().targetFOV = 20f;
					Nox.getPlayer().GetComponent<PlayerMove>().verticalForce = 0f;
					Nox.getPlayer().GetComponent<PlayerMove>().targetDirection = Vector2.zero;
					Nox.getPlayer().transform.position = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
					soundSystem.addEnergy(3f);
					soundSystem.shootSound("whips");
				}
			}
		}

		if (Input.GetMouseButtonUp(2)) {
			playing = false;
			soundSystem.dynamicToggle("droplets", false, 5f);
		}
	}
}