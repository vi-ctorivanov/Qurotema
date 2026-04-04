using UnityEngine;
using UnityEngine.UI;

public class ShowBeat : MonoBehaviour {
    [Header("References")]
    public Image[] beatsUI;

    [Header("Dynamics")]
    public float baseLight = 0.3f;

    void Update() {
        //ShowBeat needs to load after Sound to read beatChange
        if (Sound.Instance.beatChange) beatsUI[Sound.Instance.currentBeatInBar].color = new Color(1f, 1f, 1f, 1f);
		for (int i = 0; i < beatsUI.Length; i++) {
			if (i != Sound.Instance.currentBeatInBar && beatsUI[i].color.a > baseLight) beatsUI[i].color = new Color(1f, 1f, 1f, beatsUI[i].color.a - 2f * Time.deltaTime);
		}
    }
}