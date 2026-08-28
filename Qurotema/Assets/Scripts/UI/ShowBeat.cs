using UnityEngine;
using UnityEngine.UI;

public class ShowBeat : MonoBehaviour {

	private Image[] beatsUI;
	private float baseLight = 0.2f;

	void Start() {
		beatsUI = GetComponentsInChildren<Image>();
	}

	private void OnEnable() {
		Sound.OnQuarter += updateBeatUI;
	}

	private void OnDisable() {
		Sound.OnQuarter -= updateBeatUI;
	}

	void Update() {
		foreach (Image ui in beatsUI) {
			if (ui.color.a > baseLight) ui.color = new Color(1f, 1f, 1f, ui.color.a - 2f * Time.deltaTime);
		}
	}

	void updateBeatUI(int beat) {
		beatsUI[beat].color = new Color(1f, 1f, 1f, 1f);
	}
}