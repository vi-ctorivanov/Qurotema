using UnityEngine;
using TMPro;

public class LocationData : MonoBehaviour {

	public TMP_Text data;
	
	void Update () {
		if (Nox.Instance.player) {
			Vector3 position = Nox.Instance.player.transform.position;
			var text = (position.x * 20).ToString("F2") + "\n" + (position.y * 20).ToString("F2") + "\n" + (position.z * 20).ToString("F2");
			data.text = text;
		}
	}
}