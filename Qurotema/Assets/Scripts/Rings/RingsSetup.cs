using UnityEngine;

public class RingsSetup: MonoBehaviour {

    [Header("References")]
	public GameObject ring;

    //definition
	private int count = 5;
	private float distanceBetweenRings = 1.2f;

	void Start() {
		for (int i = 0; i < count; i++) {
			GameObject p = Instantiate(ring, transform, false);
			p.transform.localPosition = new Vector3((i - count / 2f) * distanceBetweenRings, 2f, 0f);
		}
	}
}