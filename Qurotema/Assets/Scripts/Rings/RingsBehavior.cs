/*

Manages rings instrument.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class RingsBehavior: MonoBehaviour {

	[Header("References")]
	public Transform cursor;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;

	[Header("Dynamics")]
	public LayerMask mask;

	//state
	private bool resizing = false;
	private float playSpeed = 0f;

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
	}

	void Update() {
		if (cursorAction.IsPressed()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "Ring") {
					//resize
					if (interactAction.WasPressedThisFrame()) resizing = true;
					if (interactAction.WasReleasedThisFrame()) resizing = false;
					if (resizing) {
						//find the mouse's... idk?
					}

					//resonate
					//Sound.Instance.queueShot("ring", Sound.Instance.ringsEvent, ("ChromaticNote", parsedInt - 1));
					Nox.Instance.ringPlayed();
				}
			}
		}
	}
}