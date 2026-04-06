/*

Manages flying behavior.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class FlyPointBehavior : MonoBehaviour {

	[Header("References")]
	public GameObject flyPoint;

	//input
	private InputAction interactAction;

	[Header("Dynamics")]
	public LayerMask mask;
	private Vector3 targetPoint;

	void Start() {
		interactAction = InputSystem.actions.FindAction("Interact");
	}

	void Update() {
		if (Nox.Instance.player) {
			if (Nox.Instance.player.GetComponent<PlayerMove>().flying && interactAction.IsPressed()) {
				RaycastHit hit;
				Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) targetPoint = hit.point;
			}

			if (Mathf.Abs(targetPoint.x - flyPoint.transform.position.x) > 1f && Mathf.Abs(targetPoint.z - flyPoint.transform.position.z) > 1f) {
				flyPoint.transform.position = Vector3.Lerp(flyPoint.transform.position, targetPoint, 1f * Time.deltaTime);
			}

			//audio
			if (Nox.Instance.player.GetComponent<PlayerMove>().flying && interactAction.WasPressedThisFrame()) {
				Sound.Instance.padState.setParameterByName("Volume", 1);
			}

			if (Nox.Instance.player.GetComponent<PlayerMove>().flying && interactAction.WasReleasedThisFrame()) {
				Sound.Instance.padState.setParameterByName("Volume", 0);
			}
		}
	}
}