using UnityEngine;

public class PadSetup : MonoBehaviour {

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
				case 0:
					tone = "kick";
					break;

				case 1:
					tone = "snare";
					break;

				case 2:
					tone = "hat";
					break;
			}

			for (int column = 0; column < columns; column++) {
				GameObject p = Instantiate(pad, new Vector3(this.transform.position.x + row * distanceBetweenPads, this.transform.position.y + 0, this.transform.position.z - column * distanceBetweenPads), Quaternion.identity, this.transform);
				p.GetComponent<Pad>().tone = tone;
				p.GetComponent<Pad>().count = column + 1;
			}
		}
	}
}