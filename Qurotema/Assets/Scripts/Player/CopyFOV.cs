/*

Copies main camera FOV.

*/

using UnityEngine;

public class CopyFOV : MonoBehaviour {

	private Camera c;
	private Camera localC;

	void Start() {
		c = Camera.main.GetComponent<Camera>();
		localC = GetComponent<Camera>();
	}

	void Update() {
		localC.fieldOfView = c.fieldOfView;
	}
}