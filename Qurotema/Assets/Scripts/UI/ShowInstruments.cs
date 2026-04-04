using UnityEngine;
using UnityEngine.UI;

public class ShowInstruments : MonoBehaviour {

	private Image[] instrumentsUI;
	public Sprite undiscoveredInstrument;
	public Sprite discoveredInstrument;

	void Start() {
		instrumentsUI = GetComponentsInChildren<Image>();
	}

	private void OnEnable() {
		Nox.OnMasterInstrument += updateInstrumentUI;
	}

	private void OnDisable() {
		Nox.OnMasterInstrument -= updateInstrumentUI;
	}

	void updateInstrumentUI(int instrumentsMastered) {
		for (int i = 0; i < instrumentsUI.Length; i++) {
			if (i < instrumentsMastered) {
				instrumentsUI[i].sprite = discoveredInstrument;
				instrumentsUI[i].color = new Color(1f, 1f, 1f, 1f);
			} else {
				instrumentsUI[i].sprite = undiscoveredInstrument;
				instrumentsUI[i].color = new Color(1f, 1f, 1f, 0.3f);
			}
		}
	}
}