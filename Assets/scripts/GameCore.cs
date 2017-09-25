using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;

public class GameCore : MonoBehaviour {

	BotFactory botFactory;
    RoadFactory roadFactory;


	private const int GAME_IDLE = 0;
	private const int GAME_LIVE = 1;
	private const int GAME_PAUSE = 2;
	private const int GAME_OVER = 9;
	private int gameState = GAME_IDLE;

	public GameObject player;
	
	List<GameObject> bgs = new List<GameObject>();
	List<GameObject> roads = new List<GameObject>();
    List<GameObject> borders = new List<GameObject>();
	List<GameObject> bots = new List<GameObject>();
	public List<GameObject> items = new List<GameObject>();

    public GameObject cameraMain;

    public GameObject dialogGameStart;
    public GameObject dialogGameEnd;
	public GameObject gameDashboard;
	public GameObject dashboardDurability;
	public GameObject dashboardScore;
	public GameObject dashboardDistance;

	float speed = 1f;
	private float distance;
	private float score;
	private float scoreSpeed;

	// Use this for initialization
	void Start() {
	}

	public void initGame() {
		player = GameObject.FindGameObjectWithTag("Player");
		speed = player.GetComponent<PlayerCore>().Speed;
		
		roadFactory = GetComponent<RoadFactory>();
		botFactory = GetComponent<BotFactory>();
        
		roadFactory.initBgs(bgs);
		roadFactory.initRoads(roads);
		roadFactory.initBorders(borders);
		
		dialogGameStart = GameObject.FindGameObjectWithTag("DialogStart");
		dialogGameEnd = GameObject.FindGameObjectWithTag("DialogEnd");
		gameDashboard = GameObject.FindGameObjectWithTag("DialogDashboard");
		dashboardDurability = GameObject.Find("labelHealth");
		dashboardScore = GameObject.Find("labelScore");
		dashboardDistance = GameObject.Find("labelDistance");
		
		idleGame();
	}

	public void idleGame() {
		gameState = GAME_IDLE;
		dialogGameStart.SetActive(true);
		dialogGameEnd.SetActive(false);
		gameDashboard.SetActive(false);

		distance = 0;
		scoreSpeed = 1f;
		score = 0f;
		
		player.GetComponent<PlayerCore>().init();
		
		botFactory.clearBots(bots);
		items.Clear();
	}

    public void startGame() {
	    gameState = GAME_LIVE;
	    
        dialogGameStart.SetActive(false);
	    gameDashboard.SetActive(true);

        //cameraMain = GameObject.FindGameObjectWithTag("MainCamera");

        botFactory = GetComponent<BotFactory>();
        botFactory.initBots(bots, 4);
    }

	private void endGame() {
		gameState = GAME_OVER;
		dialogGameEnd.SetActive(true);
		gameDashboard.SetActive(false);
		
		GameObject gameResults = GameObject.Find("valuesStatistic");
		gameResults.GetComponent<Text>().text = Math.Round(score, 0)
		                                        + "\n" + Math.Round(distance / 100, 2)
		                                        + "km\n" + botFactory.overtakes;
	}
		
	// Update is called once per frame
	void Update() {
		float dt = Time.deltaTime;
		botFactory = GetComponent<BotFactory> ();
		roadFactory = GetComponent<RoadFactory>();

		if (gameState == GAME_IDLE) {
			roadFactory.updateBorders(borders, speed, dt);
			roadFactory.updateRoads(roads, speed, dt);
			roadFactory.updateBgs(bgs, speed, dt);
		} else if (gameState == GAME_LIVE) {
			roadFactory.updateBorders(borders, speed, dt);
			roadFactory.updateRoads(roads, speed, dt);
			roadFactory.updateBgs(bgs, speed, dt);
			
			if (botFactory.updateBots(player, bots, dt) || player.GetComponent<PlayerCore>().updatePlayer(dt)) {
				endGame();
			}

			distance += speed * dt;
			scoreSpeed += dt;
			if (!player.GetComponent<PlayerCore>().isBordered) score += scoreSpeed * dt;

			dashboardDurability.GetComponent<Text>().text = "Health: " + player.GetComponent<PlayerCore>().getDurability();
			dashboardScore.GetComponent<Text>().text = "Score: " + Math.Round(score, 0);
			dashboardDistance.GetComponent<Text>().text = Math.Round(distance / 100, 2) + "km";
		}
	}
}
