/*

Manages rings instrument.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingsBehavior : MonoBehaviour {

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("States")]
	public bool inArea = false;

	void Update() {
		if (inArea && Input.GetMouseButton(1) && Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {

				Sound.Instance.addEnergy(0.5f);
				Sound.Instance.shootSound("rings", int.Parse(hit.collider.tag) - 1);
				Nox.Instance.ringPlayed();
			}
		}
	}
}