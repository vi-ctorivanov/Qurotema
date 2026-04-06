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
	private float proximDistanceToGround = -10000f;
	private float proximSizeMultiplier = 10f;
	private float transitionAnimationSpeedMultiplier = 2f;
	private float followEase = 8f;
	private float sunDistance = 9000f;

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
	}

	void Update() {
		//state management
		if (cursorAction.IsPressed()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {

				//cursor follow
				if (!interactAction.IsPressed()) {
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

		//follow animation
		if (following) {
			Vector3 targetPosition = transform.position + (cursor.position - transform.position).normalized * sunDistance;
			sunSphere.position = Vector3.Lerp(sunSphere.position, targetPosition, followEase * Time.deltaTime);
		}
		
		// //proxim animation
		// if (proxim) {
		// 	sunSphere.localPosition = new Vector3(sunSphere.localPosition.x, sunSphere.localPosition.y, Mathf.Lerp(sunSphere.localPosition.z, proximDistanceToGround, transitionAnimationSpeedMultiplier * 0.1f * Time.deltaTime));
		// 	sunSphere.localScale = Vector3.Lerp(sunSphere.localScale, startSize * proximSizeMultiplier, transitionAnimationSpeedMultiplier * 0.1f * Time.deltaTime);
		// } else {
		// 	sunSphere.localPosition = new Vector3(sunSphere.localPosition.x, sunSphere.localPosition.y, Mathf.Lerp(sunSphere.localPosition.z, -sunDistance, transitionAnimationSpeedMultiplier * Time.deltaTime));
		// 	if (transform.localEulerAngles.y > 20 && transform.localEulerAngles.y < 160) transform.Rotate(0.0f, orbitSpeed * underHorizonSpeedMultiplier * Time.deltaTime, 0.0f, Space.Self);
		// 	else transform.Rotate(0.0f, orbitSpeed * Time.deltaTime, 0.0f, Space.Self);
		// }

		// //gates animation
		// if (gates) {
		// 	Vector3 targetDirection = transform.position - Nox.Instance.gatesSphere.transform.position;
		// 	Vector3 newDirection = Vector3.Lerp(transform.forward, targetDirection, transitionAnimationSpeedMultiplier * Time.deltaTime);
		// 	transform.rotation = Quaternion.LookRotation(newDirection);
		// 	transform.position = Vector3.Lerp(transform.position, Nox.Instance.player.transform.position, transitionAnimationSpeedMultiplier * Time.deltaTime);
		// }

		// //return from proxim
		// if (!proxim) sunSphere.localScale = Vector3.Lerp(sunSphere.localScale, startSize, transitionAnimationSpeedMultiplier * Time.deltaTime);
	}

	private void gatesAppeared() {
		gates = true;
	}
}