using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotFactory : MonoBehaviour {

	public GameObject[] botPrefabs;
	private int botIndex = 0;

	public float botSpeedMax;
	public float botSpeedMin;

	private float[] lines = {-2.15f, -0.75f, 0.75f, 2.15f};
	private int[] zOrderLines = {1075, 1025, 975, 925};

	private List<int> overtackesIds = new List<int>();
	public int overtakes;

    public void initBots(List<GameObject> bots, int count) {
	    overtakes = 0;
	    overtackesIds.Clear();
	    botIndex = 0;
	    
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

	public void clearBots(List<GameObject> bots) {
		foreach (GameObject bot in bots) {
			Destroy(bot);
		}
		bots.Clear();
	}

	Vector3 getBotPosition(List<GameObject> bots) {
		const float MIN_DISTANCE = 5f;
		const float START_X_MIN = 20f;
		const float START_X_MAX = 50f;
		const int MAX_COUNT = 800;

		Vector3 position;
		int roadLine;// = Random.Range(0, lines.Length);

		float distance;
		int iteration = 0;
		do {
			roadLine = Random.Range(0, lines.Length);
			int initLine = roadLine;
			int linesPassed = 0;
			
			do {
				if (roadLine >= lines.Length) roadLine = 0;
				position = new Vector3(Random.Range(START_X_MIN, START_X_MAX), lines[roadLine], 0f);
				distance = 1000f;
				
				for (int i = 0; i < bots.Count; i++) {
					if (bots[i].transform.position.y == position.y)
						distance = Math.Min(distance, Vector3.Distance(position, bots[i].transform.position));
				}
				linesPassed++;
				roadLine++;
				if (roadLine == initLine) {
					linesPassed++;
					roadLine++;
				}
				if (linesPassed >= lines.Length) break;
				iteration ++;
			} while (distance < MIN_DISTANCE);
				
			if (iteration > MAX_COUNT) break;
			iteration ++;
		} while (distance < MIN_DISTANCE);
		
		if (iteration > MAX_COUNT) print("iterations to getPosition = " + iteration);
		return position;
	}

	public Boolean updateBots(GameObject player, float playerSpeed, List<GameObject> bots, float dt) {
		for (int i = 0; i < bots.Count; i++) {
			float botSpeed = playerSpeed - bots[i].GetComponent<BotCore>().speed;
			bots[i].transform.position = new Vector3(bots[i].transform.position.x - dt * botSpeed, bots[i].transform.position.y, 0f);
			bots[i].GetComponent<BotCore>().botSafety (bots);

			if (bots[i].transform.position.x <= player.transform.position.x &&
			    !overtackesIds.Contains(bots[i].GetComponent<BotCore>().id)) {
				overtackesIds.Add(bots[i].GetComponent<BotCore>().id);
				overtakes++;
			}

			if (!bots[i].GetComponent<BotCore>().cloned && bots [i].transform.position.x < 8.5f) {
				bots[i].GetComponent<BotCore>().cloned = true;
				addBot(bots);
            } else if (bots[i].transform.position.x < -15f) {
                Destroy(bots[i]);
				overtackesIds.Remove(bots[i].GetComponent<BotCore>().id);
                bots.Remove(bots[i]);
                break;
            }

			if (isCollide(player, bots[i]))
				return true;
		}

		return false;
	}
	
	public Boolean isCollide(GameObject player, GameObject bot) {
		Rect p = new Rect(player.transform.position, new Vector2(player.GetComponent<Collider2D>().bounds.size.x, player.GetComponent<Collider2D>().bounds.size.y));
		Rect b = new  Rect(bot.transform.position, new Vector2(bot.GetComponent<Collider2D>().bounds.size.x, bot.GetComponent<Collider2D>().bounds.size.y));
		
		if (p.x + p.width / 2 > b.x - b.width / 2 &&
		    p.x - p.width / 2 < b.x + b.width / 2 &&
		    p.y - p.height / 2 < b.y + b.height / 2 &&
		    p.y + p.height / 2 > b.y - b.height / 2) {
			return true;
		}
		
		return false;
	}

	public void animationStop(List<GameObject> bots) {
		for (int i = 0; i < bots.Count; i++)
			bots[i].GetComponent<BotCore>().animationStop();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
