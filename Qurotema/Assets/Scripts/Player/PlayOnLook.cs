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

		/*
		0 = none
		1 = sun
		2 = monolith
		3 = pads
		4 = strings
		5 = rings
		6 = gates
		7 = beam
		*/

		if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
			switch (hit.collider.tag) {
				case "Sun":
					Sound.Instance.lookState.setParameterByName("Look", 1);
					break;

				case "Monolith":
					Sound.Instance.lookState.setParameterByName("Look", 2);

					if (!firstLookMonolith && Vector3.Distance(Nox.Instance.player.transform.position, hit.point) < 150f) {
						firstLookMonolith = true;
						Nox.Instance.monolithDiscovered();
					}
					break;
					
				case "Pads":
					Sound.Instance.lookState.setParameterByName("Look", 3);
					break;

				case "StringsNode":
				case "StringsPlatform":
					Sound.Instance.lookState.setParameterByName("Look", 4);
					break;

				case "RingsPlatform":
					Sound.Instance.lookState.setParameterByName("Look", 5);
					break;

				case "Gates":
					Sound.Instance.lookState.setParameterByName("Look", 6);
					break;

				case "Beam":
					if (hit.transform.gameObject.TryGetComponent<MonolithBeam>(out MonolithBeam beam)) {
						if (beam.alpha > 0.5f) Sound.Instance.lookState.setParameterByName("Look", 7);
					} else Sound.Instance.lookState.setParameterByName("Look", 7);
					break;

				default:
					Sound.Instance.lookState.setParameterByName("Look", 0);
					break;
			}
		}
	}
}
