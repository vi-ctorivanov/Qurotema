/*

Creates marker on mouse look location when trigger button is held.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class MarkerBehavior : MonoBehaviour {

	[Header("References")]
	public GameObject marker;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;
	private InputAction markerAction;

	[Header("Dynamics")]
	public float minTeleportFOV = -10f;
	public float maxTeleportFOV = 10f;
	public Vector3 teleportFOVDistances; //0, 100, 500
	public LayerMask mask;

	//states
	private bool ready = false;
	private bool playing = false;

	private void OnEnable() {
		Nox.OnIntroductionFinished += getReady;
		Nox.OnMovementStop += unReady;
	}

	private void OnDisable() {
		Nox.OnIntroductionFinished -= getReady;
		Nox.OnMovementStop -= unReady;
	}

	private void getReady() {
		ready = true;
	}

	private void unReady() {
		ready = false;
	}

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
		markerAction = InputSystem.actions.FindAction("Marker");
	}

	void Update() {
		if (!ready) return;

		if (markerAction.IsPressed() && !cursorAction.IsPressed() && !Nox.Instance.player.GetComponent<PlayerMove>().flying) {
			RaycastHit hit;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
				Instantiate(marker, hit.point, Quaternion.identity);

				if (!playing) {
					playing = true;
					Sound.Instance.markerState.setParameterByName("SoundOn", 1);
				}

				if (interactAction.WasPressedThisFrame()) {
					//use distance to determine FOV warping
					float d = Vector3.Distance(hit.point, transform.position);
					if (d < teleportFOVDistances.y) d = Nox.Instance.remap(d, teleportFOVDistances.y, teleportFOVDistances.x, 0f, minTeleportFOV);
					else d = Nox.Instance.remap(d, teleportFOVDistances.y, teleportFOVDistances.z, 0f, maxTeleportFOV);
					Nox.Instance.cam.GetComponent<MouseLook>().targetFOV += d;

					Sound.Instance.queueShot("teleport", Sound.Instance.teleportEvent, ("TeleportDistance", Nox.Instance.remap(Vector3.Distance(Nox.Instance.player.transform.position, hit.point), 0f, 500f, 0f, 1f)));

					Nox.Instance.player.GetComponent<PlayerMove>().verticalForce = 0f;
					Nox.Instance.player.GetComponent<PlayerMove>().targetDirection = Vector2.zero;
					Nox.Instance.player.transform.position = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
				}
			}
		}

		//override with control state
		if (playing && (markerAction.WasReleasedThisFrame() || cursorAction.IsPressed() || Nox.Instance.player.GetComponent<PlayerMove>().flying)) {
			playing = false;
			Sound.Instance.markerState.setParameterByName("SoundOn", 0);
		}
	}
}