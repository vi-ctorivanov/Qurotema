/*

Plays various sounds depending on the object being looked at.
Only one object can be looked at at a time (naturally).

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnLook : MonoBehaviour {

	[Header("Trackers")]
	private bool lookingAtMonolith = false;
	private bool lookingAtGates = false;
	private bool lookingAtSun = false;
	private bool lookingAtRock = false;
	private bool firstLookMonolith = false;

	void Update() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {

			//on conditions
			if (hit.collider.tag == "Sun" && !lookingAtSun) {
				disableAllFlags();
				lookingAtSun = true;

				Sound.Instance.ambienceToggle("sun", true);
			}

			if (hit.collider.tag == "Monolith" && !lookingAtMonolith) {
				disableAllFlags();
				lookingAtMonolith = true;
				if (!firstLookMonolith) {
					firstLookMonolith = true;
					Nox.Instance.monolithDiscovered();
				}

				Sound.Instance.ambienceToggle("whispers", true);
			}

			if (hit.collider.tag == "Gates" && !lookingAtGates) {
				disableAllFlags();
				lookingAtGates = true;

				Sound.Instance.ambienceToggle("vocals", true);
				Sound.Instance.ambienceToggle("whispers", true);
			}

			if (hit.collider.tag == "Rock" && !lookingAtRock) {
				disableAllFlags();
				lookingAtRock = true;

				Sound.Instance.ambienceToggle("whispers", true);
			}

			//off conditions
			if (hit.collider.tag != "Gates" && lookingAtGates) {
				lookingAtGates = false;
				Sound.Instance.ambienceToggle("vocals", false);
				if (hit.collider.tag != "Monolith" && hit.collider.tag != "Rock") Sound.Instance.ambienceToggle("whispers", false);
			}

			if (hit.collider.tag != "Monolith" && lookingAtMonolith) {
				lookingAtMonolith = false;
				if (hit.collider.tag != "Rock") Sound.Instance.ambienceToggle("whispers", false);
			}

			if (hit.collider.tag != "Sun" && lookingAtSun) {
				lookingAtSun = false;
				Sound.Instance.ambienceToggle("sun", false);
			}

			if (hit.collider.tag != "Rock" && lookingAtRock) {
				lookingAtRock = false;
				if (hit.collider.tag != "Monolith") Sound.Instance.ambienceToggle("whispers", false);
			}
		}
	}

	void disableAllFlags() {
		lookingAtGates = false;
		lookingAtMonolith = false;
		lookingAtSun = false;
		lookingAtRock = false;
	}
}
