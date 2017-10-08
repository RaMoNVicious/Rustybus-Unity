﻿using System.Collections;
using System;
using System.Collections.Generic;
//using UnityEditor.Advertisements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class GameCore : MonoBehaviour
{

	private RewardBasedVideoAd rewardBasedVideoAd;
	private BannerView bannerAd;

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
	public GameObject dialogGuide;
	public GameObject gameDashboard;
	public GameObject dashboardDurability;
	public GameObject dashboardScore;
	public GameObject dashboardDistance;

	private GameObject muteOff;
	private GameObject muteOn;

	private float speed = 1f;
	private float distance;
	private float score;
	private float scoreSpeed;

	private float endTimer = 0f;
	private const float END_GAME_ANIMATION_TIME = 3f;

	private Boolean extrHealth = false;

	private float guideTime;
	private float guidTimeToShow = 4f;

	private float[] speedLevels = {0f, 2f, 4f, 8f, 16f, 32f, 64f, 128f};
	private float[] botLevels = {0f, 3f, 6f, 12f, 24f, 48f, 96f};
	private int lastBotLevelIndex = 0;

	// Use this for initialization
	void Start() {
	}
	
	// ----------------------------------
	// music
	// ----------------------------------
	public void musicSwitch() {
		GetComponent<AudioSource>().mute = !GetComponent<AudioSource>().mute;
		PlayerPrefs.SetInt("muted", GetComponent<AudioSource>().mute ? 1 : 0);
		
		
		if (muteOn == null) muteOn = GameObject.Find("buttonMute").gameObject.transform.Find("muteOn").gameObject;
		if (muteOff == null) muteOff = GameObject.Find("buttonMute").gameObject.transform.Find("muteOff").gameObject;
		
		muteOn.SetActive(GetComponent<AudioSource>().mute);
		muteOff.SetActive(!GetComponent<AudioSource>().mute);
	}
	// ----------------------------------

	public void showRewardAds() {
		//showUnityRewardAd();
		showAdMobRewardAd();
	}
	
	// ----------------------------------
	// Unity ads
	// ----------------------------------
	private void showUnityRewardAd() {
		ShowOptions options = new ShowOptions();
		options.resultCallback = HandleShowResult;

		Advertisement.Show("rewardedVideo", options);
	}
	
	void HandleShowResult (ShowResult result)
	{
		if(result == ShowResult.Finished) {
			//Debug.Log("Video completed - Offer a reward to the player");
			GameObject buttonAd = dialogGameEnd.transform.Find("buttonAd").gameObject;
			buttonAd.SetActive(false);
			extrHealth = true;
		} else if(result == ShowResult.Skipped) {
			//Debug.LogWarning("Video was skipped - Do NOT reward the player");

		} else if(result == ShowResult.Failed) {
			//Debug.LogError("Video failed to show");
		}
	}
	// ----------------------------------

	
	// ----------------------------------
	// AdMob ads
	// ----------------------------------
	private void loadRewardBasedAd() {
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-6337813370020696/8122059181";
		#elif UNITY_IPHONE
		string adUnitId = "unused";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
	}

	private void loadBannerAd()
	{
		AdRequest banneRequest = new AdRequest.Builder().Build();
		
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-6337813370020696/9135757953";
		#elif UNITY_IPHONE
		string adUnitId = "unused";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		bannerAd = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
		bannerAd.LoadAd(banneRequest);
	}

	private void showAdMobRewardAd() {
		if (endTimer > 0 && endTimer < 3) return;
		
		if (rewardBasedVideoAd.IsLoaded()) {
			bannerAd.Hide();
			rewardBasedVideoAd.Show();
		} else {
			print("Ad NOT loaded.");
		}
	}
	
	public event EventHandler<Reward> OnAdRewarded;

	public void HandleOnAdRewarded(object sender, Reward args) {
		print("Ad Reward.");
		GameObject buttonAd = dialogGameEnd.transform.Find("buttonAd").gameObject;
		buttonAd.SetActive(false);
		bannerAd.Show();
		extrHealth = true;
	}
	// ----------------------------------
	
	public void initGame() {
		
		Advertisement.Initialize("1562601");
		rewardBasedVideoAd = RewardBasedVideoAd.Instance;
		rewardBasedVideoAd.OnAdRewarded += HandleOnAdRewarded;
		
		player = GameObject.FindGameObjectWithTag("Player");
		speed = player.GetComponent<PlayerCore>().Speed;
		
		roadFactory = GetComponent<RoadFactory>();
		botFactory = GetComponent<BotFactory>();
        
		roadFactory.initBgs(bgs);
		roadFactory.initRoads(roads);
		roadFactory.initBorders(borders);
		
		dialogGameStart = GameObject.FindGameObjectWithTag("DialogStart");
		dialogGameEnd = GameObject.FindGameObjectWithTag("DialogEnd");
		dialogGuide = GameObject.FindGameObjectWithTag("DialogGuide");
		gameDashboard = GameObject.FindGameObjectWithTag("DialogDashboard");
		dashboardDurability = GameObject.Find("labelHealth");
		dashboardScore = GameObject.Find("labelScore");
		dashboardDistance = GameObject.Find("labelDistance");
		
		if (muteOn == null) muteOn = GameObject.Find("buttonMute").gameObject.transform.Find("muteOn").gameObject;
		if (muteOff == null) muteOff = GameObject.Find("buttonMute").gameObject.transform.Find("muteOff").gameObject;

		Boolean muted = PlayerPrefs.GetInt("muted") != 0;
		GetComponent<AudioSource>().mute = muted;
		muteOn.SetActive(muted);
		muteOff.SetActive(!muted);
		
		idleGame();
	}

	public void idleGame() {
		if (endTimer > 0 && endTimer < 4) return; // TODO: make nice buttons appear
		
		gameState = GAME_IDLE;
		dialogGameStart.SetActive(true);
		dialogGuide.SetActive(false);
		dialogGameEnd.SetActive(false);
		gameDashboard.SetActive(false);

		distance = 0;
		scoreSpeed = 1f;
		score = 0f;

		guideTime = 0f;
		
		player.GetComponent<PlayerCore>().init(extrHealth);
		
		botFactory.clearBots(bots);
		items.Clear();
		
		loadBannerAd();
		bannerAd.Destroy();
		loadRewardBasedAd();
	}

    public void startGame() {
	    gameState = GAME_LIVE;
	    
        dialogGameStart.SetActive(false);
	    dialogGuide.SetActive(true);
	    gameDashboard.SetActive(true);

	    player.GetComponent<PlayerCore>().getFlat();

	    lastBotLevelIndex = 0;
        botFactory = GetComponent<BotFactory>();
        botFactory.initBots(bots, 4);

	    endTimer = 0f;
    }

	private void endGame() {
		dialogGameEnd.SetActive(true);
		gameDashboard.SetActive(false);

		extrHealth = false;
		GameObject buttonAd = dialogGameEnd.transform.Find("buttonAd").gameObject;
		buttonAd.SetActive(true);
		
		player.GetComponent<PlayerCore>().animationStop();
		botFactory.animationStop(bots);
		
		bannerAd.Show();
	}
		
	// Update is called once per frame
	void Update() {
		
		float dt = Time.deltaTime;
		float speedLevel = speed;
		float scoreX = 1;
		for (int i = speedLevels.Length - 1; i >= 0; i --)
			if (distance / 100f > speedLevels[i]) {
				speedLevel = speed + i / 2f;
				scoreX += i;
				break;
			}
		
		botFactory = GetComponent<BotFactory> ();
		roadFactory = GetComponent<RoadFactory>();

		if (gameState == GAME_IDLE) {
			roadFactory.updateBorders(borders, speedLevel, dt);
			roadFactory.updateRoads(roads, speedLevel, dt);
			roadFactory.updateBgs(bgs, speedLevel, dt);
			
			if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
			
		} else if (gameState == GAME_LIVE) {
			guideTime += dt;
			guideTime = Math.Min(guideTime, guidTimeToShow);
			dialogGuide.SetActive(Math.Round(guideTime * 10f, 0) % 10 > 0f && Math.Round(guideTime * 10f, 0) % 10 < 7f ? true : false);

			speedLevel += player.GetComponent<PlayerCore>().isBordered ? -1f : 0f;
			player.GetComponent<PlayerCore>().updateDistance(distance);

			/*for (int i = botLevels.Length - 1; i >= 0; i--) {
				if (distance > botLevels[i] * 100f) {
					if (lastBotLevelIndex != i) {
						lastBotLevelIndex = i;
						botFactory.addBot(bots);
					}
					break;
				}
			}

			print("bots count = " + bots.Count);*/
			
			roadFactory.updateBorders(borders, speedLevel, dt);
			roadFactory.updateRoads(roads, speedLevel, dt);
			roadFactory.updateBgs(bgs, speedLevel, dt);
			
			if (botFactory.updateBots(player, speedLevel, bots, dt) || player.GetComponent<PlayerCore>().updatePlayer(dt)) {
				gameState = GAME_OVER;
				endGame();
				return;
			}

			distance += speedLevel * dt;
			scoreSpeed += dt;
			if (!player.GetComponent<PlayerCore>().isBordered) score += scoreX * scoreSpeed * dt;

			dashboardDurability.GetComponent<Text>().text = "Health: " + player.GetComponent<PlayerCore>().getDurability();
			dashboardScore.GetComponent<Text>().text = "Score: " + Math.Round(score, 0);
			dashboardDistance.GetComponent<Text>().text = Math.Round(distance / 100, 2) + "km";
		} else if (gameState == GAME_OVER)
		{
			print("endTimer = " + endTimer);
			GameObject gameResults = GameObject.Find("valuesStatistic");
			if (endTimer < 1) {
				endTimer += dt;
				gameResults.GetComponent<Text>().text = "-\n-km\n-";
			} else  if (endTimer < 2) {
				endTimer += dt;
				gameResults.GetComponent<Text>().text = Math.Round(score, 0) + "\n-km\n-";
			} else if (endTimer < 3) {
				endTimer += dt;
				gameResults.GetComponent<Text>().text = Math.Round(score, 0)
				                                        + "\n" + Math.Round(distance / 100, 2)
				                                        + "km\n-";
			} else {
				gameResults.GetComponent<Text>().text = Math.Round(score, 0)
														+ "\n" + Math.Round(distance / 100, 2)
														+ "km\n" + botFactory.overtakes;
			}
		}
	}
}
