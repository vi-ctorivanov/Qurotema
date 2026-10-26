using UnityEngine;

public class InstrumentMastered : MonoBehaviour {

    public string instrument = "";
    public bool mastered = false;

    private void OnEnable() {
		Nox.OnMasterInstrument += setMastery;
	}

	private void OnDisable() {
		Nox.OnMasterInstrument -= setMastery;
	}

    private void setMastery(int instrumentsMastered, string instrument) {
        if (instrument == this.instrument) mastered = true;
    }
}
