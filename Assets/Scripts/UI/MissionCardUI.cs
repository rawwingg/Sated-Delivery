using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionCardUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text destinationText;
    public TMP_Text trashAmountText;
    public Button selectButton;

    private MissionData _mission;
    private MainGameUI _mainGameUI;

    public void Bind(MissionData mission, MainGameUI mainGameUI)
    {
        _mission = mission;
        _mainGameUI = mainGameUI;

        // Show BOTH name + destination
        if (destinationText != null)
            destinationText.text = $"{mission.title} - {mission.destinationName}";

        if (trashAmountText != null)
            trashAmountText.text = $"Trash: {mission.trashAmount}";

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (_mainGameUI != null && _mission != null)
            _mainGameUI.SelectMission(_mission);
    }
}