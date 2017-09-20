using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class GameCore : MonoBehaviour {

	public BotFactory botFactory;
    public RoadFactory roadFactory;

	public GameObject player;
	
	List<GameObject> roads = new List<GameObject>();
	public GameObject[] deserts;
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

		initDesert();
		initRoads();

        roadFactory = GetComponent<RoadFactory>();
        roadFactory.initBorders(borders);
		roadFactory.initRoads(roads);
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

	void initRoads() {
		if (roads.Length < 0) return;
		float width = roads[0].GetComponent<Renderer>().bounds.size.x;

		roads[0].transform.position = new Vector3 (0f - width/2f, 0f, 0f);
		for (int i = 1; i < roads.Length; i++) {
			roads[i].transform.position = new Vector3 (i * width - width/2f, 0f, 0f);
		}
	}

	void initDesert() {
		if (deserts.Length < 0) return;
		float width = deserts[0].GetComponent<Renderer>().bounds.size.x;

		deserts[0].transform.position = new Vector3 (0f - width/2f, 0f, 0f);
		for (int i = 1; i < deserts.Length; i++) {
			deserts[i].transform.position = new Vector3 (i * width - width/2f, 0f, 0f);
		}
	}
		
	// Update is called once per frame
	void Update() {
		float dt = Time.deltaTime;

		updateDeserts(dt);

		botFactory = GetComponent<BotFactory> ();
		botFactory.updateBots(bots, speed, dt);

        roadFactory = GetComponent<RoadFactory>();
        roadFactory.updateBorders(borders, speed, dt);
	}

	void updateDeserts(float dt) {
		for (int i = 0; i < deserts.Length; i++) {
			deserts[i].transform.position = new Vector3(deserts[i].transform.position.x - dt * speed, 0f, 0f);

			float width = deserts[i].GetComponent<Renderer>().bounds.size.x;
			if (deserts[i].transform.position.x + width < 0f) {
				deserts[i].transform.position = new Vector3(deserts[i].transform.position.x + width * 2f, 0f, 0f);
			}				
		}
	}
}
