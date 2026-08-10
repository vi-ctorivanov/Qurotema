/*

Camera controls, animation, and FOV dynamics.

*/

using UnityEngine;

public class MenuCamera : MonoBehaviour {

	[Header("Dynamics")]
	public float shakeSpeed = 1f;
	public float shakeQuantity = 1.4f;

	[Header("States")]
	private float perlinX;
	private float perlinY;
	private float perlinZ;

	void Start() {
		//get perlin noise seed
		perlinX = Random.Range(0f, 1000f);
		perlinY = Random.Range(0f, 1000f);
		perlinZ = Random.Range(0f, 1000f);
	}

	void Update() {
		shake();
	}

	private void shake() {
		//increment perlin 'cursor'
		perlinX += shakeSpeed * Time.deltaTime;
		perlinY += shakeSpeed * Time.deltaTime;
		perlinZ += shakeSpeed * Time.deltaTime;

		//remap to -1 to 1 and amplify according to shake quantity
		float x = Nox.Instance.remap(Mathf.PerlinNoise(perlinX, 0), 0f, 1f, -1f, 1f) * shakeQuantity;
		float y = Nox.Instance.remap(Mathf.PerlinNoise(perlinY, 0), 0f, 1f, -1f, 1f) * shakeQuantity;
		float z = Nox.Instance.remap(Mathf.PerlinNoise(perlinZ, 0), 0f, 1f, -1f, 1f) * shakeQuantity;

		//apply perlin noise as rotation
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + x, transform.localEulerAngles.y + y, transform.localEulerAngles.z + z);
	}
}