using UnityEngine;
using UnityEngine.UI;

public class ShowMonoliths : MonoBehaviour {
	private Image[] monolithsUI;
	public Sprite undiscoveredMonolith;
	public Sprite discoveredMonolith;

	void Start() {
		monolithsUI = GetComponentsInChildren<Image>();
	}

	private void OnEnable() {
		Nox.OnActivateMonolith += updateMonolithUI;
	}

	private void OnDisable() {
		Nox.OnActivateMonolith -= updateMonolithUI;
	}

	void updateMonolithUI(int monolithsActivated) {
		for (int i = 0; i < monolithsUI.Length; i++) {
			if (i < monolithsActivated) {
				monolithsUI[i].sprite = discoveredMonolith;
				monolithsUI[i].color = new Color(1f, 1f, 1f, 1f);
			} else {
				monolithsUI[i].sprite = undiscoveredMonolith;
				monolithsUI[i].color = new Color(1f, 1f, 1f, 0.3f);
			}
		}
	}
}
