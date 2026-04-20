using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour {

	private Image[] directionsUI;
	public Transform[] POIs;

	void Start() {
		directionsUI = GetComponentsInChildren<Image>();
	}

	void Update() {
		//find nearest POI
		Transform nearest = POIs[0];
		float smallestDistance = 9999f;
		foreach(Transform POITransform in POIs) {
			float distance = Vector2.Distance(
				new Vector2(POITransform.position.x, POITransform.position.z),
				new Vector2(transform.position.x, transform.position.z)
			);
			if (distance < smallestDistance) {
				smallestDistance = distance;
				nearest = POITransform;
			}
		}

		//if near, show here
		if (smallestDistance < 20f) directionsUI[4].color = new Color(1f, 1f, 1f, 1f);
		else {
			//compute direction
			Vector2 playerToPOI = new Vector2((nearest.position - transform.position).normalized.x, (nearest.position - transform.position).normalized.z);
			Vector3 flatForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
    		Vector2 camForward2D = new Vector2(flatForward.x, flatForward.z);
			int direction = Nox.Instance.cardinalDirection(camForward2D, playerToPOI);

			//point to nearest POI
			directionsUI[direction].color = new Color(1f, 1f, 1f, 1f);
		}
		
		//fade out other directions
		foreach (Image ui in directionsUI) {
			if (ui.color.a > 0f) ui.color = new Color(1f, 1f, 1f, ui.color.a - 2f * Time.deltaTime);
		}
	}
}
