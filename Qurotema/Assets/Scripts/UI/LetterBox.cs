using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LetterBox : MonoBehaviour {

	[Header("References")]
	public RectTransform canvasRect;
	public RectTransform lowPanel;
	public RectTransform highPanel;
	public Camera cam;

	//input
	private InputAction cursorAction;

	//dynamics
	private float aspectRatio = 2f;
	private float aspecRatioChangeSpeed = 4f;

	//states
	private float currentAspect;
	private bool ready = false;

	//corountines
	private Coroutine letterBox;

	void OnEnable() {
		Nox.OnIntroductionFinished += getReady;
		Nox.OnMovementStop += unReady;
	}

	void OnDisable() {
		Nox.OnIntroductionFinished -= getReady;
		Nox.OnMovementStop -= unReady;
	}

	void Start() {
		currentAspect = cam.aspect;
		forceAspectRatio(cam.aspect);

		cursorAction = InputSystem.actions.FindAction("Cursor");
	}

	void Update() {
		if (!ready) return;

		if (cursorAction.WasPressedThisFrame()) {
			if (letterBox != null) StopCoroutine(letterBox);
			letterBox = StartCoroutine(changeAspectRatio(aspectRatio));
		}

		if (cursorAction.WasReleasedThisFrame()) {
			if (letterBox != null) StopCoroutine(letterBox);
			letterBox = StartCoroutine(changeAspectRatio(cam.aspect));
		}

		//override when flying
		if (Nox.Instance.player) {
			if (Nox.Instance.player.GetComponent<PlayerMove>().flying) {
				if (letterBox != null) StopCoroutine(letterBox);
				if (currentAspect != cam.aspect) currentAspect = Mathf.Lerp(currentAspect, cam.aspect, aspecRatioChangeSpeed / 4f * Time.deltaTime);
				forceAspectRatio(currentAspect);
			}
		}
		
	}

	private void getReady() {
		ready = true;
	}

	private void unReady() {
		ready = false;
	}

	//force aspect ratio with letterboxing
	private void forceAspectRatio(float ratio) {
		float barHeight = (canvasRect.rect.height - (canvasRect.rect.width / ratio)) / 2f;
		Vector2 resize = new Vector2(canvasRect.rect.width + 10f, barHeight);

		lowPanel.sizeDelta = resize;
		lowPanel.anchoredPosition = new Vector2(0f, (lowPanel.rect.height / 2f) - 1f);

		highPanel.sizeDelta = resize;
		highPanel.anchoredPosition = new Vector2(0f, (-highPanel.rect.height / 2f) + 1f);
	}

	IEnumerator changeAspectRatio(float desiredRatio) {
		while (currentAspect != desiredRatio) {
			yield return new WaitForSeconds(0.01f);
			if (Mathf.Abs(currentAspect - desiredRatio) < 0.01f) currentAspect = desiredRatio;
			else {
				currentAspect = Mathf.Lerp(currentAspect, desiredRatio, aspecRatioChangeSpeed * Time.deltaTime);
				forceAspectRatio(currentAspect);
			}
		}
	}
}