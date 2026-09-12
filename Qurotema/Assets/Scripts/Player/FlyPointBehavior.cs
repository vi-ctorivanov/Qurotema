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
		//move flypoint upwards for fading effect
		if (flyPoint.transform.position.y < 1000f) flyPoint.transform.Translate(Vector3.up * 50f * Time.deltaTime);

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
			Sound.Instance.flyState.setParameterByName("FlyX", Nox.Instance.remap(Mathf.Abs(flyPoint.transform.position.x), 0f, 3000f, 1f, 0f));
			Sound.Instance.flyState.setParameterByName("FlyZ", Nox.Instance.remap(Mathf.Abs(flyPoint.transform.position.z), 0f, 3000f, 1f, 0f));
			Sound.Instance.flyState.setParameterByName("FlyHeight", Nox.Instance.remap(Mathf.Abs(flyPoint.transform.position.y - targetPoint.y), 0f, 1000f, 0f, 1f));
			Sound.Instance.flyState.setParameterByName("FlyDistance", Nox.Instance.remap(Vector3.Distance(flyPoint.transform.position, Nox.Instance.player.transform.position), 0f, 250f, 0f, 1f));
		}
	}
}