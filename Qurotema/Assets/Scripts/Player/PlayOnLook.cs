/*

Plays various sounds depending on the object being looked at.
Only one object can be looked at at a time (naturally).

*/

using UnityEngine;

public class PlayOnLook : MonoBehaviour {

	private bool firstLookMonolith = false;

	void Update() {
		RaycastHit hit;
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

		if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
			switch (hit.collider.tag) {
				case "Sun":
					Sound.Instance.lookState.setParameterByName("Look", 1);
					break;

				case "Monolith":
					Sound.Instance.lookState.setParameterByName("Look", 2);
					if (!firstLookMonolith) {
						firstLookMonolith = true;
						Nox.Instance.monolithDiscovered();
					}
					break;

				case "Gates":
					Sound.Instance.lookState.setParameterByName("Look", 3);
					break;
			}
		}
	}
}
