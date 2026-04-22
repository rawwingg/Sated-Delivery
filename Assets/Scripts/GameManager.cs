using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Mission Runtime")]
    public GameObject missionPrefab;
    public List<Mission> activeMissions = new();

    [Header("Single Vehicle")]
    public VehicleDefinition selectedVehicle;
    public GameObject fallbackDriverPrefab;

    [Header("Available Missions")]
    public List<MissionData> availableMissions = new();

    [Header("Mission Points")]
    public Transform playerHQ;
    public List<Transform> missionEndpoints = new();

    private int _nextMissionId = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        for (int i = 0; i < availableMissions.Count; i++)
        {
            if (availableMissions[i].id <= 0)
                availableMissions[i].id = _nextMissionId++;
            else
                _nextMissionId = Mathf.Max(_nextMissionId, availableMissions[i].id + 1);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            AddDummyMission();
    }

    public List<MissionData> GetUnassignedMissions()
    {
        List<MissionData> list = new();

        foreach (MissionData mission in availableMissions)
        {
            if (!mission.isAssigned)
                list.Add(mission);
        }

        return list;
    }

    public bool DeployToMission(MissionData mission)
    {
        if (mission == null)
        {
            Debug.LogWarning("[GameManager] Deploy failed because mission is null.");
            return false;
        }

        if (mission.isAssigned)
        {
            Debug.LogWarning($"[GameManager] Mission {mission.id} is already assigned.");
            return false;
        }

        if (mission.pickupPoint == null || mission.deliveryPoint == null)
        {
            Debug.LogWarning($"[GameManager] Mission {mission.id} is missing pickup/delivery points.");
            return false;
        }

        GameObject prefabToSpawn = null;

        if (selectedVehicle != null && selectedVehicle.vehiclePrefab != null)
            prefabToSpawn = selectedVehicle.vehiclePrefab;
        else
            prefabToSpawn = fallbackDriverPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogError("[GameManager] No vehicle prefab available to spawn.");
            return false;
        }

        mission.isAssigned = true;
        mission.assignedVehicle = selectedVehicle;

        AddActiveMission(mission.pickupPoint, mission.deliveryPoint, prefabToSpawn, mission.id, mission);

        Debug.Log($"[GameManager] Deployed {(selectedVehicle != null ? selectedVehicle.vehicleName : prefabToSpawn.name)} to {mission.destinationName}.");
        return true;
    }

    public void AddActiveMission(Transform startLocation, Transform endLocation, GameObject driverPrefab, int id, MissionData missionData = null)
    {
        GameObject missionObject = Instantiate(missionPrefab, transform);
        Mission mission = missionObject.GetComponent<Mission>();

        if (mission == null)
        {
            Debug.LogError("[GameManager] missionPrefab is missing Mission component.");
            return;
        }

        mission.StartPoint = startLocation;
        mission.EndPoint = endLocation;
        mission.id = id;
        mission.missionData = missionData;
        mission.ShowRoute();

        activeMissions.Add(mission);

        RouteFollower routeFollower = Instantiate(driverPrefab, missionObject.transform).GetComponent<RouteFollower>();
        if (routeFollower == null)
        {
            Debug.LogError("[GameManager] Spawned vehicle prefab is missing RouteFollower.");
            return;
        }

        routeFollower.Visualizer = mission;
        routeFollower.StartMoving();
    }

    public void RemoveActiveMission(int id)
    {
        for (int i = 0; i < activeMissions.Count; i++)
        {
            if (activeMissions[i].id != id)
                continue;

            Mission mission = activeMissions[i];

            if (mission != null && mission.missionData != null)
            {
                mission.missionData.isAssigned = false;
                mission.missionData.assignedVehicle = null;
            }

            Destroy(mission.gameObject);
            activeMissions.RemoveAt(i);
            break;
        }
    }

    public void AddDummyMission()
    {
        if (playerHQ == null)
        {
            Debug.LogWarning("[GameManager] Player HQ is not assigned.");
            return;
        }

        if (missionEndpoints == null || missionEndpoints.Count == 0)
        {
            Debug.LogWarning("[GameManager] Need at least 1 mission endpoint.");
            return;
        }

        Transform chosenEndPoint = missionEndpoints[Random.Range(0, missionEndpoints.Count)];

        MissionData mission = new MissionData
        {
            id = _nextMissionId++,
            title = "Waste Pickup",
            destinationName = chosenEndPoint.name,
            trashAmount = Random.Range(2, 12),
            pickupPoint = playerHQ,
            deliveryPoint = chosenEndPoint,
            isAssigned = false
        };

        availableMissions.Add(mission);

        Debug.Log($"[GameManager] Added mission from HQ to {chosenEndPoint.name}.");
    }
}