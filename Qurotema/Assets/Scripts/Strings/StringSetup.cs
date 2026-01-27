using UnityEngine;

public class StringSetup : MonoBehaviour {

	[Header("References")]
	public GameObject node;

	[Header("Definition")]
	public Vector3 nodesStart = new Vector3(-10f, 10f, -10f);
	public float distanceBetweenNodes = 10f;
	public int rows = 3;
	public int columns = 3;
	public int depths = 3;
	public float nodePositionRandomness = 3f;

	void Start() {
		for (int row = 0; row < rows; row++) {
			for (int column = 0; column < columns; column++) {
				for (int depth = 0; depth < depths; depth++) {
						float x = this.transform.position.x + Random.Range(-nodePositionRandomness, nodePositionRandomness) + nodesStart.x + row * distanceBetweenNodes;
						float y = this.transform.position.y + Random.Range(-nodePositionRandomness, nodePositionRandomness) + nodesStart.y + column * distanceBetweenNodes;
						float z = this.transform.position.z + Random.Range(-nodePositionRandomness, nodePositionRandomness) + nodesStart.z + depth * distanceBetweenNodes;
						GameObject n = Instantiate(node, new Vector3(x, y, z), Quaternion.identity, this.transform);
				}
			}
		}
	}
}