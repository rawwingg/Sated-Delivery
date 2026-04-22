using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainGameUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("Managers")]
    public GameManager gameManager;

    [Header("Vehicle Panel")]
    public VehicleCardUI vehicleCardUI;
    public GameObject vehicleStatsRoot;
    public TMP_Text selectedVehicleLabel;

    [Header("Mission Panel")]
    public Transform missionListRoot;
    public MissionCardUI missionCardPrefab;
    public TMP_Text selectedMissionLabel;
    public GameObject missionTabRoot;

    [Header("Actions")]
    public GameObject deployButtonHighlight;
    public TMP_Text feedbackText;

    [Header("Swipe")]
    public float minSwipeDistance = 60f;

    private readonly List<MissionCardUI> _spawnedCards = new();
    private MissionData _selectedMission;
    private Vector2 _dragStart;

    private void Start()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;

        RefreshAll();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            RefreshMissionList();
    }

    public void RefreshAll()
    {
        RefreshVehicleCard();
        RefreshMissionList();
        UpdateSelectionText();
    }

    // Kept only so existing arrow/swipe buttons don't break.
    // In the single-vehicle version these do nothing.
    public void NextVehicle()
    {
        SetFeedback("Only one vehicle is configured.");
    }

    public void PreviousVehicle()
    {
        SetFeedback("Only one vehicle is configured.");
    }

    public void ToggleVehicleStats()
    {
        if (vehicleStatsRoot != null)
            vehicleStatsRoot.SetActive(!vehicleStatsRoot.activeSelf);
    }

    public void ToggleMissionTab()
    {
        if (missionTabRoot == null)
            return;

        bool newState = !missionTabRoot.activeSelf;
        missionTabRoot.SetActive(newState);

        if (newState)
            RefreshMissionList();
    }

    public void SelectMission(MissionData mission)
    {
        if (_selectedMission == mission)
        {
            _selectedMission = null;
            SetFeedback("Mission deselected.");
        }
        else
        {
            _selectedMission = mission;
            SetFeedback($"Selected mission: {mission.title} - {mission.destinationName} ({mission.trashAmount} trash)");
        }

        UpdateSelectionText();
    }

    public void DeploySelectedVehicle()
    {
        if (gameManager == null)
        {
            SetFeedback("No GameManager found.");
            return;
        }

        if (_selectedMission == null)
        {
            SetFeedback("Select a mission first.");
            return;
        }

        bool deployed = gameManager.DeployToMission(_selectedMission);
        if (!deployed)
        {
            SetFeedback("Could not deploy vehicle.");
            return;
        }

        string vehicleName = gameManager.selectedVehicle != null
            ? gameManager.selectedVehicle.vehicleName
            : "vehicle";

        SetFeedback($"Deployed {vehicleName} to {_selectedMission.destinationName}");
        _selectedMission = null;
        RefreshMissionList();
        UpdateSelectionText();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStart = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - _dragStart;
        if (Mathf.Abs(delta.x) < minSwipeDistance || Mathf.Abs(delta.x) < Mathf.Abs(delta.y))
            return;

        // Single vehicle version: swipes do nothing.
    }

    private void RefreshVehicleCard()
    {
        if (gameManager == null)
        {
            SetFeedback("No GameManager found.");
            return;
        }

        if (gameManager.selectedVehicle == null)
        {
            SetFeedback("Assign a VehicleDefinition to GameManager.selectedVehicle.");
            return;
        }

        if (vehicleCardUI != null)
            vehicleCardUI.Bind(gameManager.selectedVehicle);

        UpdateSelectionText();
    }

    private void RefreshMissionList()
    {
        foreach (MissionCardUI card in _spawnedCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        _spawnedCards.Clear();

        if (gameManager == null || missionCardPrefab == null || missionListRoot == null)
            return;

        List<MissionData> missions = gameManager.GetUnassignedMissions();
        bool selectedMissionStillExists = false;

        foreach (MissionData mission in missions)
        {
            if (mission == _selectedMission)
                selectedMissionStillExists = true;

            MissionCardUI card = Instantiate(missionCardPrefab, missionListRoot);
            card.Bind(mission, this);
            _spawnedCards.Add(card);
        }

        if (!selectedMissionStillExists)
            _selectedMission = null;

        UpdateSelectionText();
    }

    private void UpdateSelectionText()
    {
        if (selectedVehicleLabel != null)
        {
            selectedVehicleLabel.text = gameManager != null && gameManager.selectedVehicle != null
                ? $"{gameManager.selectedVehicle.vehicleName}"
                : "Vehicle: None";
        }

        if (selectedMissionLabel != null)
        {
            selectedMissionLabel.text = _selectedMission == null
                ? "Mission: None"
                : $"{_selectedMission.title} | {_selectedMission.destinationName} | Trash: {_selectedMission.trashAmount}";
        }

        if (deployButtonHighlight != null)
            deployButtonHighlight.SetActive(_selectedMission != null);
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;

        Debug.Log($"[MainGameUI] {message}");
    }
}