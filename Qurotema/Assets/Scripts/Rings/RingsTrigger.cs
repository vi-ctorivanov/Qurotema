using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingsTrigger : MonoBehaviour {

	private RingsBehavior r;

	void Start() {
		r = Camera.main.gameObject.GetComponent<RingsBehavior>();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			r.inArea = true;
		}
    }

    private void OnTriggerExit(Collider other) {
    	if (other.tag == "Player") {
			r.inArea = false;
		}
    }
}