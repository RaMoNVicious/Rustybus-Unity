using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotCore : MonoBehaviour {

	public int id;
	public float speed;
	public bool cloned = false;

	public GameObject lightsTail;
	float lightsTailTimer = 0;
	const float LIGHTS_TAIL_DELAY = -1f;

	float acceleration = 0f;
	const float ACCELERATION_SPEED = -0.025f;
	const float ACCELERATION_MAX = -0.2f;

	public void botSafety(List<GameObject> bots) {
		for (int i = 0; i < bots.Count; i++) {
			if (id != bots [i].GetComponent<BotCore> ().id &&
				transform.position.y == bots [i].GetComponent<BotCore> ().transform.position.y) {

				float distance = (bots[i].transform.position.x - bots[i].GetComponent<BoxCollider2D> ().size.x / 2)
							 	 - (transform.position.x + GetComponent<BoxCollider2D> ().size.x / 2);

				if (distance > 0 &&
				    speed > bots [i].GetComponent<BotCore> ().speed &&
				    distance < this.GetComponent<BoxCollider2D> ().size.x &&
				    acceleration > ACCELERATION_MAX) {

					acceleration += ACCELERATION_SPEED;
					speed += acceleration;
					lightsTailTimer = LIGHTS_TAIL_DELAY;
				} else if (acceleration < 0) {
					acceleration -= ACCELERATION_SPEED / 2f;
					speed += acceleration;
					lightsTailTimer = LIGHTS_TAIL_DELAY;
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		lightsTail.GetComponent<Renderer> ().sortingOrder = this.GetComponent<Renderer> ().sortingOrder + 1;

		if (lightsTailTimer > 0) {
			lightsTail.SetActive (false);
		} else {
			lightsTail.SetActive (true);
			lightsTailTimer += Time.deltaTime;
		}
	}
}
