using UnityEngine;

public class PadsSetup : MonoBehaviour {

	[Header("References")]
	public GameObject pad;

	[Header("Definition")]
	public int rows = 3;
	public int columns = 16;
	public float distanceBetweenPads = 6.0f;

	void Start() {
		for (int row = 0; row < rows; row++) {
			string tone = "";
			switch (row) {
				case 0: tone = "kick"; break;
				case 1: tone = "snare"; break;
				case 2: tone = "hat"; break;
			}

			for (int column = 0; column < columns; column++) {
				//use local transform to maintain parent's offset
				GameObject p = Instantiate(pad, this.transform, false);
				p.transform.localPosition = new Vector3(row * distanceBetweenPads, 0, -column * distanceBetweenPads);
				p.transform.localRotation = Quaternion.identity;
				p.GetComponent<PadsInstrument>().tone = tone;
				p.GetComponent<PadsInstrument>().count = column;
			}
		}
	}
}