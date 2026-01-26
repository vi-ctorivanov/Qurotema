/*

Manages string instrument.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stringer : MonoBehaviour {
	
	[Header("References")]
	public LineRenderer liner;
	public GameObject strings;
	public GameObject stringObject;
	public CursorBehavior cursor;
	public Sound soundSystem;

	[Header("Dynamics")]
	public LayerMask mask;

	[Header("States")]
	private Vector3 start = new Vector3(0,0,0);
	private Vector3 end = new Vector3(0,0,0);
	private bool stringing = false;
	private List<StringCord> stringSet = new List<StringCord>();

	void Update() {
		if (!stringing) leftClick();
		else {
			drawPendingString();
			leftRelease();
		}
	}

	private void leftClick() {
		//play string
		if (Input.GetMouseButton(1) && !Input.GetMouseButton(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "String") {
					hit.collider.gameObject.GetComponent<StringCord>().playSound();
				}
			}
		}

		//start create string
		if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {
				if (hit.collider.tag == "StringsNode") {
					start = hit.collider.gameObject.transform.position;
					startString(start);
				}
			}
		}
	}

	private void leftRelease() {
		//end create string
		if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~mask)) {
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
		liner.SetPosition(1, Camera.main.transform.position + (Camera.main.transform.forward * 10));
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

		StringCord component = stringInstance.GetComponent<StringCord>();
		component.init(s, e);

		addString(stringInstance);
	}

	private void addString(GameObject o) {
		stringSet.Add(o.GetComponent<StringCord>());
	}

	private bool stringExists(Vector3 s, Vector3 e) {
		for (int i = 0; i < stringSet.Count; i++) {
			StringCord str = stringSet[i].GetComponent<StringCord>();
			if (str.start == s && str.end == e) return true;
			if (str.start == e && str.end == s) return true;
		}
		return false;
	}
}