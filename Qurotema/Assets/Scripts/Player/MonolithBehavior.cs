/*

Manages monolith interaction.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class MonolithBehavior : MonoBehaviour {

	[Header("References")]
	public Transform cursor;
	public Texture2D tex;

	[Header("Dynamics")]
	public LayerMask mask;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
	}

	void Update() {
		if (cursorAction.IsPressed() && interactAction.IsPressed()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {
				//activate
				if (hit.collider.tag == "MonolithEye") {
					if (!hit.collider.gameObject.GetComponent<MonolithActivate>().active) {
						hit.collider.gameObject.GetComponent<MonolithActivate>().makeActive();
					}
				}

				//play
				if (hit.collider.tag == "Monolith") {
					//read albedo through mesh collider's raycast hit texturecoord
					Color c = tex.GetPixel((int)(hit.textureCoord.x * tex.width), (int)(hit.textureCoord.y * tex.height));
					float value = Mathf.GammaToLinearSpace(c.r);
					hit.collider.gameObject.GetComponent<MonolithInstrument>().play(value);
				}
			}
		}
	}
}