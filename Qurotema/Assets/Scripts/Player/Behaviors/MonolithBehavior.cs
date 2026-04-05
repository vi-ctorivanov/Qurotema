/*

Manages monolith interaction.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class MonolithBehavior : MonoBehaviour {

	[Header("References")]
	public Transform cursor;

	[Header("Dynamics")]
	public LayerMask mask;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
	}

	void Update() {
		if (cursorAction.IsPressed() && interactAction.IsPressed()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "MonolithEye") {
					if (!hit.collider.gameObject.GetComponent<MonolithActivate>().active) {
						hit.collider.gameObject.GetComponent<MonolithActivate>().makeActive();
					}
				}
			}
		}
	}
}