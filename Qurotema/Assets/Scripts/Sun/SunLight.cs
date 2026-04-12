using UnityEngine;

public class SunLight : MonoBehaviour {
	
	public Transform sun;

	void Update() {
		Vector3 lookDirection = (transform.position - sun.position).normalized;
		transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
	}
}