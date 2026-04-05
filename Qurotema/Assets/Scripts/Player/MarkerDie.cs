using System.Collections;
using UnityEngine;

public class MarkerDie : MonoBehaviour {

	//dynamics
	private float dieSpeed = 0.4f;

	void Start() {
		StartCoroutine(SlowDie());
	}

	IEnumerator SlowDie() {
		while (true) {
			yield return new WaitForSeconds(0.01f);
			transform.localScale -= new Vector3(0, dieSpeed, 0);
			if (transform.localScale.y < 0.05f) Destroy(gameObject);
		}
	}
}