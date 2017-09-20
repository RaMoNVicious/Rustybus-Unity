using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadFactory : MonoBehaviour {

    public GameObject[] roadPrefabs;
    public GameObject[] borderTopPrefabs;
    public GameObject[] borderBottomPrefabs;

    const int BORDER_TOP = 0;
    const int BORDER_BOTTOM = 1;

    public void initBorders(List<GameObject> borders) {
        addBorder(borders, BORDER_TOP);
        addBorder(borders, BORDER_BOTTOM);
        addBorder(borders, BORDER_TOP);
        addBorder(borders, BORDER_BOTTOM);
    }

    public void initRoads(List<GameObject> roads) {
        
    }

    public GameObject getNewBorder(int side) {
        GameObject border = side == BORDER_TOP ? borderTopPrefabs[Random.Range(0, borderTopPrefabs.Length)] : borderBottomPrefabs[Random.Range(0, borderBottomPrefabs.Length)];
        border.GetComponent<RoadBorder>().side = side;
        return border;
    }

    void addBorder(List<GameObject> borders, int side) {
        GameObject newBoroder = Instantiate(getNewBorder(side), new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        newBoroder.transform.SetParent(this.transform);
        setBorderPosition(newBoroder, borders, side);
        newBoroder.GetComponent<Renderer>().sortingOrder = side == BORDER_TOP ? 100 : 1900;
        borders.Add(newBoroder);
        print("borders.count = " + borders.Count);
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
