/*

Manages monolith interaction.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonolithClick : MonoBehaviour {

	[Header("Dynamics")]
	public LayerMask mask;

    void Update() {
        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "MonolithEye") {
					if (!hit.collider.gameObject.GetComponent<MonolithBehavior>().active) {
						Sound.Instance.addEnergy(5f);
						hit.collider.gameObject.GetComponent<MonolithBehavior>().makeActive();
					}
				}
			}
		}
    }
}