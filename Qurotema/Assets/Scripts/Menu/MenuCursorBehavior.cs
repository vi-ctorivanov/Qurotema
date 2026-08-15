/*

Manages mouse 'cursor' in menu.

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCursorBehavior : MonoBehaviour {

	[Header("References")]
	public Material cursorTransparent;
	public Material cursorOpaque;

	//input
	private InputAction interactAction;

	[Header("Dynamics")]
	public float distanceFromCamera = 5f;
	public float followSpeed = 50f;
	public float alpha = 0f;

	[Header("Colors")]
	public Color red = new Color(100f, 0f, 0f);
	public Color purple = new Color(5f, 5f, 100f);

	void Start () {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;

		Vector2 screenPos = Mouse.current.position.ReadValue();
		Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5f));
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime); 

		interactAction = InputSystem.actions.FindAction("Interact");

		cursorTransparent.SetFloat("_Alpha", 0f);
		cursorOpaque.SetFloat("_Alpha", 0f);
	}

	void Update () {
		cursorTransparent.SetFloat("_Alpha", alpha);
		cursorOpaque.SetFloat("_Alpha", alpha);

		if (interactAction.WasPressedThisFrame()) makeActive();
		if (interactAction.WasReleasedThisFrame()) makePassive();

		Vector2 screenPos = Mouse.current.position.ReadValue();
		Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceFromCamera));
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime); 
		Debug.Log(screenPos);
	}

	private void makeActive() {
		cursorTransparent.SetColor("_Color", red);
		cursorOpaque.SetColor("_Color", red);
	}

	private void makePassive() {
		cursorTransparent.SetColor("_Color", purple);
		cursorOpaque.SetColor("_Color", purple);
	}
}