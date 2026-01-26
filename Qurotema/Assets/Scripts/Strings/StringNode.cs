using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringNode : MonoBehaviour {

	private float r = 3f;

	void Start() {
		transform.position += new Vector3(Random.Range(-r, r), Random.Range(-r, r), Random.Range(-r, r));
	}
}