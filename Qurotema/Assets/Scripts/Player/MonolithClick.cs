/*

Manages monolith interaction.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonolithClick : MonoBehaviour {

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("Input")]
	private InputAction cursorAction;
	private InputAction interactAction;

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
	}

    void Update() {
        if (cursorAction.IsPressed()) {
			RaycastHit hit;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "MonolithEye") {
					if (!hit.collider.gameObject.GetComponent<MonolithBehavior>().active) {
						hit.collider.gameObject.GetComponent<MonolithBehavior>().makeActive();
					}
				}
			}
		}
    }
}