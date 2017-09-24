using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadFactory : MonoBehaviour {

    public GameObject[] bgPrefabs;
    public GameObject[] roadPrefabs;
    public GameObject[] borderTopPrefabs;
    public GameObject[] borderBottomPrefabs;

    const int BORDER_TOP = 0;
    const int BORDER_BOTTOM = 1;
    Boolean bgEven = false;

    public void initBorders(List<GameObject> borders) {
        addBorder(borders, BORDER_TOP);
        addBorder(borders, BORDER_BOTTOM);
        addBorder(borders, BORDER_TOP);
        addBorder(borders, BORDER_BOTTOM);
    }

    public void initRoads(List<GameObject> roads) {
        addRoad(roads);
        addRoad(roads);
    }
    
    public void initBgs(List<GameObject> bgs) {
        addBg(bgs);
        addBg(bgs);
    }

    GameObject getNewBorder(int side)
    {
        int itemIndex = side == BORDER_TOP ? Random.Range(0, borderTopPrefabs.Length) : Random.Range(0, borderBottomPrefabs.Length);
        GameObject border = side == BORDER_TOP ? borderTopPrefabs[itemIndex] : borderBottomPrefabs[itemIndex];
        border.GetComponent<RoadBorder>().side = side;
        return border;
    }
    
    GameObject getNewRoad() {
        return roadPrefabs[Random.Range(0, roadPrefabs.Length)];
    }
    
    GameObject getNewBgs() {
        bgEven = !bgEven;
        return bgPrefabs[bgEven ? 0 : 1];
    }

    void addBorder(List<GameObject> borders, int side) {
        GameObject newBoroder = Instantiate(getNewBorder(side), new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        newBoroder.transform.SetParent(transform);
        setBorderPosition(newBoroder, borders, side);
        newBoroder.GetComponent<Renderer>().sortingOrder = side == BORDER_TOP ? 500 : 1500;
        borders.Add(newBoroder);
    }

    void addRoad(List<GameObject> roads) {
        float x = 0f;
        float width = 0f;
        for (int i = 0; i < roads.Count; i++) {
            x = roads[i].transform.position.x;
            width = roads[i].GetComponent<Renderer>().bounds.size.x;
        }
        GameObject newRoad = Instantiate(getNewRoad(), new Vector3(x + width, 0f, 0f), Quaternion.identity) as GameObject;
        newRoad.GetComponent<Renderer>().sortingOrder = 200;
        roads.Add(newRoad);
    }
    
    void addBg(List<GameObject> bgs) {
        float x = 0f;
        float width = 0f;
        for (int i = 0; i < bgs.Count; i++) {
            x = bgs[i].transform.position.x;
            width = bgs[i].GetComponent<Renderer>().bounds.size.x;
        }
        GameObject newBg = Instantiate(getNewBgs(), new Vector3(x + width, 0f, 0f), Quaternion.identity) as GameObject;
        newBg.GetComponent<Renderer>().sortingOrder = 100;
        bgs.Add(newBg);
    }

    void setBorderPosition(GameObject newBorder, List<GameObject> borders, int side) {
        int lastSideId = -1;
        for (int i = 0; i < borders.Count; i++) {
            if (borders[i].GetComponent<RoadBorder>().side == side) {
                lastSideId = i;
            }
        }
        if (lastSideId != -1)
        {
            newBorder.transform.position = new Vector3(borders[lastSideId].transform.position.x
                                                       + borders[lastSideId].GetComponent<Renderer>().bounds.size.x / 2
                                                       + newBorder.GetComponent<Renderer>().bounds.size.x / 2, 0f, 0f);
        }
    }
    
    public void updateBgs(List<GameObject> bgs, float playerSpeed, float dt) {
        for (int i = 0; i < bgs.Count; i++) {
            bgs[i].transform.position = new Vector3(bgs[i].transform.position.x - dt * playerSpeed, bgs[i].transform.position.y, 0f);

            if (bgs[i].transform.position.x < -1 * bgs[i].GetComponent<Renderer>().bounds.size.x) {
                Destroy(bgs[i]);
                bgs.Remove(bgs[i]);
                addBg(bgs);
                break;
            }
        }
    }

    public void updateRoads(List<GameObject> roads, float playerSpeed, float dt) {
        for (int i = 0; i < roads.Count; i++) {
            roads[i].transform.position = new Vector3(roads[i].transform.position.x - dt * playerSpeed, roads[i].transform.position.y, 0f);

            if (roads[i].transform.position.x < -1 * roads[i].GetComponent<Renderer>().bounds.size.x) {
                Destroy(roads[i]);
                roads.Remove(roads[i]);
                addRoad(roads);
                break;
            }
        }
    }
    
    public void updateBorders(List<GameObject> borders, float playerSpeed, float dt) {
        for (int i = 0; i < borders.Count; i++) {
            borders[i].transform.position = new Vector3(borders[i].transform.position.x - dt * playerSpeed, borders[i].transform.position.y, 0f);

            if (borders[i].transform.position.x < -1 * borders[i].GetComponent<Renderer>().bounds.size.x) {
                int side = borders[i].GetComponent<RoadBorder>().side;
                Destroy(borders[i]);
                borders.Remove(borders[i]);
                addBorder(borders, side);
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
