using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCore : MonoBehaviour {

	public float Speed;
	
	public float yMax;
	public float yMin;

	private Boolean steering;
	private float fallSpeed;
	
	public float fallAcc;
	public float fallSpeedMax;
	public float steerAcc;
	public float steerSpeedMax;

	public Boolean isBordered;
	
	private float durability;
	public float durabilitySpeed;
	
	private float[] lines = {-2.9f, -2.15f, -1.45f, -0.75f, 0f, 0.75f, 1.45f, 2.15f, 2.9f};
	private int[] zOrderLines = {1100, 1075, 1050, 1025, 1000, 975, 950, 925, 900};
	

	public void init(Boolean isExtraHealth) {
		isBordered = false;
		steering = false;
		durability = isExtraHealth ? 200 : 100;
		transform.position = new Vector3(-5f, 0f, 0f);
		animationStart();
	}

	public Boolean updatePlayer(float dt) {
		
		if (Input.GetKey(KeyCode.Space) || Input.touchCount != 0) {
			steering = true;
		} else if (Input.GetKeyUp(KeyCode.Space) || Input.touchCount == 0) {
			steering = false;
		}

		if (steering) {
			fallSpeed += steerAcc * dt;		// "-" - go up
			fallSpeed = Math.Min(fallSpeed, steerSpeedMax);
		} else {
			if (fallSpeed > 0) fallSpeed = 0;

			fallSpeed -= fallAcc * dt;		// "+" - go down
			fallSpeed = Math.Max(fallSpeed, -fallSpeedMax);
		}

		float y = transform.position.y + fallSpeed * dt;
		if (y < yMin) fallSpeed = 0;
		
		y = Math.Min(y, yMax);
		y = Math.Max(y, yMin);

		if (y == yMax || y == yMin) {
			isBordered = true;
		} else {
			isBordered = false;
		}
		
		transform.position = new Vector3(transform.position.x, y, 0f);
		updateZ(transform.position.y);
		
		return checkBordering();
	}

	private Boolean checkBordering() {
		if (isBordered) durability -= durabilitySpeed;
		return durability <= 0;
	}

	private void updateZ(float y) {
		for (int i = 0; i < lines.Length; i++) {
			if (y < lines[i]) {
				GetComponent<Renderer>().sortingOrder = zOrderLines[i] + 15;
				break;
			}
		}
	}

	public String getDurability() {
		return "" + Math.Round(durability, 0) + "%";
	}

	public void animationStop() {
		GetComponent<Animator>().enabled = false;
	}
	
	public void animationStart() {
		GetComponent<Animator>().enabled = true;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
