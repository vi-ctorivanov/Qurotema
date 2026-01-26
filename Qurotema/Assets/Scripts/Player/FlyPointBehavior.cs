/*

Manages flying behavior.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyPointBehavior : MonoBehaviour {

	[Header("References")]
	public Sound soundSystem;
	public GameObject flyPoint;

	[Header("Dynamics")]
	public LayerMask mask;
	private Vector3 targetPoint;

	void Update() {
		if (Nox.getPlayer().GetComponent<PlayerMove>().flying && Input.GetMouseButton(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) targetPoint = hit.point;

			soundSystem.addEnergy(1f);
		}

		if (Mathf.Abs(targetPoint.x - flyPoint.transform.position.x) > 1f && Mathf.Abs(targetPoint.z - flyPoint.transform.position.z) > 1f) {
			flyPoint.transform.position = Vector3.Lerp(flyPoint.transform.position, targetPoint, 1f * Time.deltaTime);
		}

		//audio
		if (Nox.getPlayer().GetComponent<PlayerMove>().flying && Input.GetMouseButtonDown(0)) {
			soundSystem.dynamicToggle("pads", true);
		}

		if (Nox.getPlayer().GetComponent<PlayerMove>().flying && Input.GetMouseButtonUp(0)) {
			soundSystem.dynamicToggle("pads", false);
		}
	}
}