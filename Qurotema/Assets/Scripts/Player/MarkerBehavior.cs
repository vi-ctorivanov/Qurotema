/*

Creates marker on mouse look location when trigger button is held.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerBehavior : MonoBehaviour {

	[Header("References")]
	public GameObject marker;

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("States")]
	private bool playing = false;

	void Update() {
		if (Input.GetMouseButton(2) && !Input.GetMouseButton(1) && !Nox.Instance.player.GetComponent<PlayerMove>().flying) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
				Instantiate(marker, hit.point, Quaternion.identity);

				if (!playing) {
					playing = true;
					Sound.Instance.dynamicToggle("droplets", true, 5f);
				}

				if (Input.GetMouseButtonDown(0)) {
					Nox.Instance.player.GetComponent<PlayerMove>().targetFOV = 20f;
					Nox.Instance.player.GetComponent<PlayerMove>().verticalForce = 0f;
					Nox.Instance.player.GetComponent<PlayerMove>().targetDirection = Vector2.zero;
					Nox.Instance.player.transform.position = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
					Sound.Instance.addEnergy(3f);
					Sound.Instance.shootSound("whips");
				}
			}
		}

		if (Input.GetMouseButtonUp(2)) {
			playing = false;
			Sound.Instance.dynamicToggle("droplets", false, 5f);
		}
	}
}