/*

Manages rings instrument.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingsBehavior : MonoBehaviour {

	private Sound soundSystem;
	public bool inArea = false;
	public LayerMask mask;
	private Story s;

	void Start() {
		soundSystem = GameObject.Find("Nox").GetComponent<Sound>();
		s = GameObject.Find("Nox").GetComponent<Story>();
	}

	void Update() {
		if (inArea && Input.GetMouseButton(1) && Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {

				soundSystem.addEnergy(0.5f);
				soundSystem.shootSound("rings", int.Parse(hit.collider.tag) - 1);
				s.ringPlayed();
			}
		}
	}
}