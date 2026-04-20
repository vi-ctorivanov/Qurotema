/*

Manages string instrument.

*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StringsBehavior : MonoBehaviour {
	
	[Header("References")]
	public LineRenderer liner;
	public GameObject strings;
	public GameObject stringObject;
	public Transform cursor;

	//input
	private InputAction cursorAction;
	private InputAction interactAction;

	[Header("Dynamics")]
	public LayerMask mask;

	//states
	private Vector3 start = new Vector3(0,0,0);
	private Vector3 end = new Vector3(0,0,0);
	private bool stringing = false;
	private List<StringsInstrument> stringSet = new List<StringsInstrument>();

	void Start() {
		cursorAction = InputSystem.actions.FindAction("Cursor");
		interactAction = InputSystem.actions.FindAction("Interact");
	}

	void Update() {
		if (!stringing) passive();
		else {
			drawPendingString();
			release();
		}
	}

	private void passive() {
		//play string
		if (cursorAction.IsPressed() && !interactAction.IsPressed()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "String") {
					hit.collider.gameObject.GetComponent<StringsInstrument>().playSound();
				}
			}
		}

		//start create string
		if (interactAction.WasPressedThisFrame() && cursorAction.IsPressed()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "StringsNode") {
					start = hit.collider.gameObject.transform.position;
					startString(start);
				}
			}
		}
	}

	private void release() {
		//end create string
		if (interactAction.WasReleasedThisFrame() || cursorAction.WasReleasedThisFrame()) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (cursor.position - transform.position).normalized, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "StringsNode") {
					end = hit.collider.gameObject.transform.position;
					if (end == start || stringExists(start, end)) cancelString();
					else endString(end);
				} else cancelString();
			} else cancelString();
		}
	}

	private void drawPendingString() {
		liner.SetPosition(0, start);
		liner.SetPosition(1, transform.position + (transform.forward * 10));
	}

	private void deletePendingString() {
		liner.SetPosition(0, new Vector3(0,0,0));
		liner.SetPosition(1, new Vector3(0,0,0));
	}

	private void startString(Vector3 s) {
		stringing = true;
	}

	private void endString(Vector3 e) {
		stringing = false;
		deletePendingString();
		createString(start, end);
	}

	private void cancelString() {
		stringing = false;
		deletePendingString();
	}

	private void createString(Vector3 s, Vector3 e) {
		Vector3 pos = Vector3.Lerp(s, e, 0.5f);

		GameObject stringInstance = Instantiate(stringObject, pos, Quaternion.identity);
		stringInstance.transform.LookAt(e);
		stringInstance.transform.localScale += new Vector3(0, Vector3.Distance(s, e) * 50, 0);
		stringInstance.transform.Rotate(-90, 0, 0);

		StringsInstrument component = stringInstance.GetComponent<StringsInstrument>();
		component.init(s, e);

		addString(stringInstance);
	}

	private void addString(GameObject o) {
		stringSet.Add(o.GetComponent<StringsInstrument>());
	}

	private bool stringExists(Vector3 s, Vector3 e) {
		foreach (StringsInstrument set in stringSet) {
			StringsInstrument str = set.GetComponent<StringsInstrument>();
			if (str.start == s && str.end == e) return true;
			if (str.start == e && str.end == s) return true;
		}
		return false;
	}
}