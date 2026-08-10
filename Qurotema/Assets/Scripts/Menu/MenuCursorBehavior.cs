/*

Manages mouse 'cursor' in menu.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCursorBehavior : MonoBehaviour {

	[Header("References")]
	public Material cursor;

	//input
	private InputAction interactAction;

	[Header("Dynamics")]
	public float followSpeed = 50f;
	public float alpha = 0f;

	[Header("Colors")]
	public Color red = new Color(100f, 0f, 0f);
	public Color purple = new Color(5f, 5f, 100f);

	void Start () {
		Cursor.visible = false;

		Vector2 screenPos = Mouse.current.position.ReadValue();
		Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5f));
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime); 

		interactAction = InputSystem.actions.FindAction("Interact");

		cursor.SetFloat("_Alpha", 0f);
	}

	void Update () {
		cursor.SetFloat("_Alpha", alpha);

		if (interactAction.WasPressedThisFrame()) makeActive();
		if (interactAction.WasReleasedThisFrame()) makePassive();

		Vector2 screenPos = Mouse.current.position.ReadValue();
		Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5f));
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime); 
	}

	private void makeActive() {
		cursor.SetColor("_Color", red);
	}

	private void makePassive() {
		cursor.SetColor("_Color", purple);
	}
}