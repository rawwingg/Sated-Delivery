using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject missionPrefab;

    public List<Mission> activeMissions;

    [Header("Debug Objects")]
    public List<GameObject> gameObjects;
    public GameObject driver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    //Debug
        if (Input.GetKeyDown(KeyCode.N))
        {
            AddDummyMission();
        }
    }
    public void AddActiveMission(Transform startLocation, Transform endLocation, GameObject driver,int id) 
    {
        GameObject missionObject = Instantiate(missionPrefab,this.transform);
        Mission mission = missionObject.GetComponent<Mission>();
        mission.StartPoint = startLocation;
        mission.EndPoint = endLocation;
        mission.id = id;

        mission.ShowRoute();
        activeMissions.Add(mission);

        RouteFollower routeFollower = Instantiate(driver,missionObject.transform).GetComponent<RouteFollower>();
        routeFollower.Visualizer = mission;
    }
    public void RemoveActiveMission(int id)
    {
        for (int i = 0; i<activeMissions.Count;i++) {
            
            if (activeMissions[i].id == id) 
            {
                Destroy(activeMissions[i]);
                activeMissions.RemoveAt(i);
                break;
            }
        }
    }
    public void AddDummyMission() 
    {
        Mission newMission = new Mission();

        AddActiveMission(gameObjects[Random.Range(0, gameObjects.Count)].transform, gameObjects[Random.Range(0, gameObjects.Count)].transform, driver,10);
    }
    
}
