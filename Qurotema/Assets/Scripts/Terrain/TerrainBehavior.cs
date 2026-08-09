using UnityEngine;
using UnityEngine.InputSystem;

public class TerrainBehavior : MonoBehaviour {

	[Header("References")]
	public Transform cursor;

	[Header("Dynamics")]
	public LayerMask mask;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;

	//state
	private bool ready = false;

	private void OnEnable() {
		Nox.OnIntroductionFinished += getReady;
	}

	private void OnDisable() {
		Nox.OnIntroductionFinished -= getReady;
	}

	private void getReady() {
		ready = true;
	}

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
	}

	void Update() {
		if (!ready) return;

		if (cursorAction.IsPressed() && interactAction.WasPressedThisFrame()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "Terrain") {
					float angle = Vector3.Dot(hit.normal, (transform.position - hit.point).normalized);
					hit.collider.gameObject.GetComponent<TerrainInstrument>().play(angle);
				}
			}
		}
	}
}