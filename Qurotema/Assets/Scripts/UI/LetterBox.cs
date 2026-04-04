using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LetterBox : MonoBehaviour {

	[Header("References")]
	public RectTransform lowPanel;
	public RectTransform highPanel;
	public Camera cam;

	//input
	private InputAction cursorAction;

	//dynamics
	private float aspectRatio = 1.67f;
	private float aspecRatioChangeSpeed = 4f;

	//states
	private float currentAspect;

	//corountines
	private Coroutine letterBox;

	void Start() {
		currentAspect = cam.aspect;
		forceAspectRatio(cam.aspect);

		cursorAction = InputSystem.actions.FindAction("Cursor");
	}

	void Update() {
		if (Nox.Instance.introductionFinished) {
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
	}

	//force aspect ratio with letterboxing
	void forceAspectRatio(float ratio) {
		if (cam.aspect <= 1.1f) return;

		float variance = (ratio / cam.aspect) - 1f;

		Vector2 resize = new Vector2(Screen.width + 10f, (variance * Screen.height) / 2f);

		lowPanel.sizeDelta = resize;
		//lowPanel.anchoredPosition = new Vector2(0f, (lowPanel.rect.height / 2f) - 1f);

		highPanel.sizeDelta = resize;
		//highPanel.anchoredPosition = new Vector2(0f, (-highPanel.rect.height / 2f) + 1f);
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