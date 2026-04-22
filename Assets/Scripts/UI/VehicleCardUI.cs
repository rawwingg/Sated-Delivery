using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleCardUI : MonoBehaviour
{
    [Header("Refs")]
    public Image vehicleIcon;
    public TMP_Text vehicleNameText;
    public TMP_Text descriptionText;

    [Header("Stats")]
    public TMP_Text speedText;
    public TMP_Text capacityText;
    public TMP_Text ecoText;
    public TMP_Text safetyText;

    public void Bind(VehicleDefinition vehicle)
    {
        if (vehicle == null)
            return;

        if (vehicleIcon != null) vehicleIcon.sprite = vehicle.icon;
        if (vehicleNameText != null) vehicleNameText.text = vehicle.vehicleName;
        if (descriptionText != null) descriptionText.text = vehicle.description;

        if (speedText != null) speedText.text = $"Speed: {vehicle.speed}";
        if (capacityText != null) capacityText.text = $"Capacity: {vehicle.capacity}";
        if (ecoText != null) ecoText.text = $"Eco: {vehicle.ecoScore}";
        if (safetyText != null) safetyText.text = $"Safety: {vehicle.safety}";
    }
}