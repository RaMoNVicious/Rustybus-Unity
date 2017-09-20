using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class GameCore : MonoBehaviour {

	public BotFactory botFactory;
    public RoadFactory roadFactory;

	public GameObject player;
	
	List<GameObject> bgs = new List<GameObject>();
	List<GameObject> roads = new List<GameObject>();
    List<GameObject> borders = new List<GameObject>();
	List<GameObject> bots = new List<GameObject>();
	public GameObject[] items;

    public GameObject cameraMain;

    public GameObject dialogGameStart;
    public GameObject dialogGameEnd;

	float speed = 1f;

	// Use this for initialization
	void Start() {
	}

	public void initGame() {
        player = GameObject.FindGameObjectWithTag("Player");

		speed = player.GetComponent<PlayerCore>().Speed;

		
		// Enveronment initialization
		roadFactory = GetComponent<RoadFactory>();
        
		roadFactory.initBgs(bgs);
		roadFactory.initRoads(roads);
		roadFactory.initBorders(borders);
	}

    public void startGame() {
        print("---------- { Game started } ----------");
        dialogGameStart = GameObject.FindGameObjectWithTag("DialogStart");
        dialogGameStart.SetActive(false);

        cameraMain = GameObject.FindGameObjectWithTag("MainCamera");
        player.GetComponent<PlayerCore>().transform.position = new Vector3(0f, 0f, 0f);

        bots.Clear ();

        botFactory = GetComponent<BotFactory>();
        botFactory.initBots(bots, 4);
    }

		
	// Update is called once per frame
	void Update() {
		float dt = Time.deltaTime;

		botFactory = GetComponent<BotFactory> ();
		botFactory.updateBots(bots, speed, dt);

        roadFactory = GetComponent<RoadFactory>();
        roadFactory.updateBorders(borders, speed, dt);
		roadFactory.updateRoads(roads, speed, dt);
		roadFactory.updateBgs(bgs, speed, dt);
	}
}
