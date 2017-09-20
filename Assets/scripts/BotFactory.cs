using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotFactory : MonoBehaviour {

	public GameObject[] botPrefabs;
	int botIndex = 0;

	public float botSpeedMax;
	public float botSpeedMin;

	float[] lines = {-2.15f, -0.75f, 0.75f, 2.15f};
	int[] zOrderLines = {1075, 1025, 975, 925};

    public void initBots(List<GameObject> bots, int count) {
        for (int i = 0; i < count; i++)
            addBot(bots);
    }

	public GameObject getNewBot() {
		GameObject bot = botPrefabs[Random.Range(0, botPrefabs.Length)];
		bot.GetComponent<BotCore>().id = botIndex++;
		bot.GetComponent<BotCore>().speed = Random.Range(botSpeedMin, botSpeedMax);
		return bot;
	}

	public void addBot(List<GameObject> bots) {
		GameObject newBot = Instantiate(getNewBot(), getBotPosition(bots), Quaternion.identity) as GameObject;
		newBot.transform.SetParent(this.transform);
		for (int i = 0; i < zOrderLines.Length; i++) {
			if (newBot.transform.position.y == lines[i]) {
				newBot.GetComponent<Renderer>().sortingOrder = zOrderLines[i];
				break;
			}
		}
		bots.Add(newBot);
	}

	Vector3 getBotPosition(List<GameObject> bots) {
		const float MIN_DISTANCE = 5f;
		const float START_X_MIN = 20f;
		const float START_X_MAX = 40f;
		const int MAX_COUNT = 100;

		Vector3 position;

		float distance;
		int iteration = 0;
		do {
			position = new Vector3(Random.Range(START_X_MIN, START_X_MAX), lines[Random.Range(0, lines.Length)], 0f);
			distance = 1000f;
			for (int i = 0; i < bots.Count; i++) {
				distance = Math.Min(distance, Vector3.Distance(position, bots[i].transform.position));
			}
			if (iteration > MAX_COUNT) break; else iteration ++;
		} while (distance < MIN_DISTANCE);		 
		return position;
	}

	public void updateBots(List<GameObject> bots, float playerSpeed, float dt) {
		for (int i = 0; i < bots.Count; i++) {
			float botSpeed = playerSpeed - bots [i].GetComponent<BotCore>().speed;
			bots[i].transform.position = new Vector3(bots[i].transform.position.x - dt * botSpeed, bots[i].transform.position.y, 0f);
			bots[i].GetComponent<BotCore>().botSafety (bots);

			if (!bots[i].GetComponent<BotCore>().cloned && bots [i].transform.position.x < 8.5f) {
				bots[i].GetComponent<BotCore>().cloned = true;
				addBot(bots);
            } else if (bots[i].transform.position.x < -15f) {
                Destroy(bots[i]);
                bots.Remove(bots[i]);
                break;
            }
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
