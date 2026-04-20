/*

Manages sun and gates sphere click behavior.
Makes sun interact with mouse cursor and hide behind gates sphere.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class SunBehavior : MonoBehaviour {

	[Header("References")]
	public GameObject pp;
	public Transform sunSphere;
	public Transform cursor;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;

	[Header("Dynamics")]
	public LayerMask mask;
	private bool gates = false;
	private Vector3 startSize;
	private float proximSizeMultiplier = 10f;
	private float transitionAnimationSpeedMultiplier = 2.5f;
	private float followEase = 8f;
	private float sunDistance = 9000f;
	private Vector3 targetPosition = new Vector3();

	//states
	private bool following = false;
	private bool negative = false;
	private bool proxim = false;

	void OnEnable() {
		Nox.OnGatesAppear += gatesAppeared;
	}

	void OnDisable() {
		Nox.OnGatesAppear -= gatesAppeared;
	}

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");

		startSize = sunSphere.localScale;
		targetPosition = sunSphere.transform.position;
	}

	void Update() {
		//state management
		if (cursorAction.IsPressed()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {

				//cursor follow
				if (!interactAction.IsPressed() && !Nox.Instance.player.GetComponent<PlayerMove>().flying) {
					if (hit.collider.tag == "Sun") following = true;
				}

				//proxim and negative
				if (interactAction.WasPressedThisFrame()) {
					if (hit.collider.tag == "Sun") proxim = !proxim;

					if (hit.collider.tag == "GatesSphere" && gates) {
						negative = !negative;
						if (negative) pp.SetActive(true);
						else pp.SetActive(false);
					}
				}
			}
		} else following = false;
		if (proxim || gates) following = false;
		if (gates) proxim = false;

		if (following) {
			targetPosition = transform.position + (cursor.position - transform.position).normalized * sunDistance;
			if (targetPosition.y < 0f) targetPosition.y = 0f; //prevent going below horizon
		}
		
		if (proxim) sunSphere.localScale = Vector3.Lerp(sunSphere.localScale, startSize * proximSizeMultiplier, transitionAnimationSpeedMultiplier * 0.1f * Time.deltaTime);
		else sunSphere.localScale = Vector3.Lerp(sunSphere.localScale, startSize, transitionAnimationSpeedMultiplier * Time.deltaTime);

		if (gates) {
			targetPosition = transform.position + (Nox.Instance.gatesSphere.transform.position - transform.position) + (Nox.Instance.gatesSphere.transform.position - transform.position).normalized * sunDistance;
			sunSphere.position = Vector3.Lerp(sunSphere.position, targetPosition, transitionAnimationSpeedMultiplier * Time.deltaTime);
		} else sunSphere.position = Vector3.Lerp(sunSphere.position, targetPosition, followEase * Time.deltaTime);
	}

	private void gatesAppeared() {
		gates = true;
	}
}