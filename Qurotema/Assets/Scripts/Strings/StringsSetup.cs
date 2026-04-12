using UnityEngine;

public class StringsSetup : MonoBehaviour {

	[Header("References")]
	public GameObject node;

	//definition
	private Vector3 nodesStart = new Vector3(-10f, 10f, -10f);
	private float distanceBetweenNodes = 10f;
	private int rows = 3;
	private int columns = 3;
	private int depths = 3;
	private float nodePositionRandomness = 3f;

	void Start() {
		for (int row = 0; row < rows; row++) {
			for (int column = 0; column < columns; column++) {
				for (int depth = 0; depth < depths; depth++) {
						float x = transform.position.x + Random.Range(-nodePositionRandomness, nodePositionRandomness) + nodesStart.x + row * distanceBetweenNodes;
						float y = transform.position.y + Random.Range(-nodePositionRandomness, nodePositionRandomness) + nodesStart.y + column * distanceBetweenNodes;
						float z = transform.position.z + Random.Range(-nodePositionRandomness, nodePositionRandomness) + nodesStart.z + depth * distanceBetweenNodes;
						Instantiate(node, new Vector3(x, y, z), Quaternion.identity, transform);
				}
			}
		}
	}
}