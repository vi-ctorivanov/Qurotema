using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerDie : MonoBehaviour {

    void Start() {
        StartCoroutine(SlowDie());
    }

    IEnumerator SlowDie() {
    	while (true) {
    		yield return new WaitForSeconds(0.01f);
    		transform.localScale -= new Vector3(0, 0.5f, 0);
    		if (transform.localScale.y < 0.1f) Destroy(gameObject);
    	}
    }
}