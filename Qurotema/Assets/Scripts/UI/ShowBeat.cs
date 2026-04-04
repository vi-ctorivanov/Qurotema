using UnityEngine;
using UnityEngine.UI;

public class ShowBeat : MonoBehaviour {

	private Image[] beatsUI;
	private float baseLight = 0.2f;

	void Start() {
		beatsUI = GetComponentsInChildren<Image>();
	}

	private void OnEnable() {
		Sound.OnBeat += updateBeatUI;
	}

	private void OnDisable() {
		Sound.OnBeat -= updateBeatUI;
	}

	void Update() {
		for (int i = 0; i < beatsUI.Length; i++) {
			if (beatsUI[i].color.a > baseLight) beatsUI[i].color = new Color(1f, 1f, 1f, beatsUI[i].color.a - 2f * Time.deltaTime);
		}
	}

	void updateBeatUI(int beat) {
		beatsUI[beat].color = new Color(1f, 1f, 1f, 1f);
	}
}