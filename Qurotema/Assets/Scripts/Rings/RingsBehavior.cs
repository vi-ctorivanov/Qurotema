/*

Manages rings instrument.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class RingsBehavior: MonoBehaviour {

	[Header("References")]
	public Transform cursor;
	private MouseLook look;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;

	[Header("Dynamics")]
	public LayerMask mask;
	private float scalingFactor = 2.5f; //related to some scaling stuff on the rings, don't worry it just works
	private float playSpeedMinimum = 2f;
	private float playSpeedMaximum = 4f;

	//state
	private bool resizing = false;
	private RaycastHit hit = new RaycastHit();

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
		
		look = Camera.main.GetComponent<MouseLook>();
	}

	void Update() {
		if (cursorAction.IsPressed()) {

			if (resizing) {
				//find the cursor's percieved location on the ring's xz plane,
				//and its distance from the ring's root to determine its rescale
				Plane p = new Plane(Vector3.up, hit.collider.gameObject.transform.position);
				Ray r = new Ray(transform.position, (cursor.position - transform.position).normalized);
				float enter;
				if (p.Raycast(r, out enter)) {
					float distance = Vector3.Distance(hit.collider.gameObject.transform.position, r.GetPoint(enter));
					hit.collider.gameObject.GetComponent<RingsInstrument>().resize(distance * scalingFactor);
				}
			}

			if (!interactAction.IsPressed()) resizing = false;

			if (!resizing) {
				if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {
					if (hit.collider.tag == "Ring") {
						//resize toggle
						if (interactAction.IsPressed()) resizing = true;

						//resonate
						float playSpeed = Mathf.Abs(look.mouseX) + Mathf.Abs(look.mouseY);

						if (playSpeed >= playSpeedMinimum && playSpeed <= playSpeedMaximum) {
							hit.collider.gameObject.GetComponent<RingsInstrument>().resonate();
							Nox.Instance.ringPlayed();
						}
					}
				}
			}
		}
	}
}