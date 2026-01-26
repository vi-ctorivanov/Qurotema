/*

Creates marker on mouse look location when trigger button is held.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerBehavior : MonoBehaviour {

	public GameObject marker;
	public LayerMask mask;

	//audio
	private Sound soundSystem;
	private bool playing = false;

	void Start() {
		soundSystem = GameObject.Find("Nox").GetComponent<Sound>();
	}

	void Update() {
		if (Input.GetMouseButton(2) && !Input.GetMouseButton(1) && !Nox.player.GetComponent<PlayerMove>().flying) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
				Instantiate(marker, hit.point, Quaternion.identity);

				if (!playing) {
					playing = true;
					soundSystem.dynamicToggle("droplets", true, 5f);
				}

				if (Input.GetMouseButtonDown(0)) {
					Nox.player.GetComponent<PlayerMove>().targetFOV = 20f;
					Nox.player.GetComponent<PlayerMove>().verticalForce = 0f;
					Nox.player.GetComponent<PlayerMove>().targetDirection = Vector2.zero;
					Nox.player.transform.position = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
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