using System;
using UnityEngine;

[Serializable]
public class MissionData
{
    public int id;
    public string title;
    public string destinationName;
    public int trashAmount;
    public Transform pickupPoint;
    public Transform deliveryPoint;
    public bool isAssigned;
    public VehicleDefinition assignedVehicle;

    public string GetSummary()
    {
        return $"{destinationName} - {trashAmount} trash";
    }
}
