using UnityEngine;

[CreateAssetMenu(fileName = "VehicleDefinition", menuName = "Sated Delivery/Vehicle Definition")]
public class VehicleDefinition : ScriptableObject
{
    public string vehicleName;
    [TextArea] public string description;
    public Sprite icon;
    public GameObject vehiclePrefab;

    [Header("Gameplay Stats")]
    [Range(1, 10)] public int speed = 5;
    [Range(1, 10)] public int capacity = 5;
    [Range(1, 10)] public int ecoScore = 5;
    [Range(1, 10)] public int safety = 5;
}
