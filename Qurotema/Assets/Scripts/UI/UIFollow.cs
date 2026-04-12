using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIFollow : MonoBehaviour {

	//references
	private PlayerMove playerScript;
	private MouseLook look;
	private CanvasGroup canvas;

	//input
	private InputAction cursorAction;
	private InputAction markerAction;
	private InputAction flightAction;

	[Header("Dynamics")]
	public float distanceFromCamera = 1.2f;
	public float fadeDelay = 0f;
	public Vector3 cameraAngularContribution = Vector3.zero;
	public Vector3 cameraTranslationalContribution = Vector3.zero;
	public float overallContribution = 1f;
	public string triggerLayer;
	private float targetOpacity = 0.9f;
	private float fadeSpeed = 10f;
	private float distanceFromCameraDifference = 0.3f;
	private Vector3 lastCameraPosition = Vector3.zero;
	private Vector3 cameraAngularVelocity = Vector3.zero;
	private Vector3 cameraTranslationalVelocity = Vector3.zero;
	
	//states
	private float opacity = 0f;
	private float minDistanceFromCamera = 0f;

	//coroutines
	private Coroutine fader;
	
	void Start () {
		minDistanceFromCamera = distanceFromCamera - distanceFromCameraDifference;

		playerScript = Nox.Instance.player.GetComponent<PlayerMove>();
		look = Camera.main.GetComponent<MouseLook>();
		canvas = GetComponent<CanvasGroup>();

		cursorAction = InputSystem.actions.FindAction("Cursor");
		markerAction = InputSystem.actions.FindAction("Marker");
		flightAction = InputSystem.actions.FindAction("Flight");

		for(int i = 0; i < 100; i++) {
			follow();
		}
	}

	void Update() {
		//modes
		if (Nox.Instance.player) {
			switch (triggerLayer) {
				case "flight":
					//checking if player is flying gives consistent results because UIFollow always runs after PlayerMove
					if (flightAction.WasPressedThisFrame() && playerScript.flying) doFade(true);
					if (flightAction.WasReleasedThisFrame() && !playerScript.flying) doFade(false);
					break;

				case "control":
					//flight overrides control
					if (cursorAction.WasPressedThisFrame() && !playerScript.flying) doFade(true);
					if (cursorAction.WasReleasedThisFrame()) doFade(false);
					break;

				case "movement":
					//control and flight override movement
					if (markerAction.WasPressedThisFrame() && !cursorAction.IsPressed() && !playerScript.flying) doFade(true);

					if (markerAction.WasReleasedThisFrame()) doFade(false);
					if (cursorAction.WasPressedThisFrame()) doFade(false);
					break;
			}
		}

		canvas.alpha = opacity;

		follow();
	}

	void doFade(bool on) {
		if (fader != null) StopCoroutine(fader);
		if (on) fader = StartCoroutine(Fade(targetOpacity));
		else fader = StartCoroutine(Fade(0f));
	}

	void follow() {
		if (Camera.main) {
			float targetDistance = 0f;
			targetDistance = Nox.Instance.remap(look.targetFOV, look.minFOV, look.maxFOV, distanceFromCamera, minDistanceFromCamera);
			Vector3 targetPosition = new Vector3(0f, 0f, targetDistance);

			/*
			Use camera-relative camera velocity and angular velocity to offset UI in opposite direction to create sense of weighty sway.
			The actual velocity calculations are more like representations of velocity, since they're easier to calculate.

			Other solutions like a lerping follow did not work very nicely because small changes to time.deltaTime resulted in major jittering.
			*/

			/*
			Rotation is just looking at mouse input, since the camera never turns without user input.
			This means that we can never allow look input while also restricting the camera, as the UI will move without the camera.
			*/
			Vector3 tempAngular = new Vector3(-look.mouseX, look.mouseY, 0f);
			cameraAngularVelocity = Vector3.Lerp(cameraAngularVelocity, tempAngular, Time.deltaTime);

			/*
			Translation is the distance between the camera's current and previous location.
			We do need to transform this vector so it's relative to the camera's forward direction to know where the UI should offset towards.
			*/
			Vector3 tempTranslational = lastCameraPosition - Camera.main.transform.position;
			cameraTranslationalVelocity = Vector3.Lerp(cameraTranslationalVelocity, Camera.main.transform.InverseTransformDirection(tempTranslational), Time.deltaTime);

			//apply offset
			float x = (cameraAngularVelocity.x * cameraAngularContribution.x) + (cameraTranslationalVelocity.x * cameraTranslationalContribution.x);
			float y = (cameraAngularVelocity.y * cameraAngularContribution.y) + (cameraTranslationalVelocity.y * cameraTranslationalContribution.y);
			float z = cameraTranslationalVelocity.z * cameraTranslationalContribution.z;
			Vector3 offset = new Vector3(x, y, z) * overallContribution;
			targetPosition += offset;

			transform.localPosition = targetPosition;
			transform.rotation = Camera.main.transform.rotation;

			lastCameraPosition = Camera.main.transform.position;
		}
	}

	IEnumerator Fade(float target) {
		yield return new WaitForSeconds(fadeDelay);

		while (Mathf.Abs(opacity - target) > 0.01f) {
			yield return new WaitForSeconds(0.01f);
			opacity = Mathf.Lerp(opacity, target, fadeSpeed * Time.deltaTime);
		}

		opacity = target;
	}
}