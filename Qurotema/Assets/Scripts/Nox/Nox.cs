/*

Holds some global variables and functions.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nox : MonoBehaviour {

	[Header("References")]
	public static GameObject player;
	public static Vector3 playerPosition;
	public AnimateTerrain terrain;

	void Start() {
		player = GameObject.Find("Player");
	}

	void Update() {
		playerPosition = player.transform.position;
	}

	public static GameObject getPlayer() {
		return player;
	}

	public static Vector3 getPlayerPosition() {
		return playerPosition;
	}

	public static float remap(float val, float min1, float max1, float min2, float max2) {
		if (val < min1) val = min1;
		if (val > max1) val = max1;

		return (val - min1) / (max1 - min1) * (max2 - min2) + min2;
	}
}