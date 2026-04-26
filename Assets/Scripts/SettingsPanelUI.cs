using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Panel Ayarlarý")]
    [SerializeField] private Transform settingsPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Transform activePositionTransform;
    [SerializeField] private Transform disabledPositionTransform;

    [Header("SpawnTime Bölümü")]
    [SerializeField] private Slider vehicleSpawnTimeSlider;
    [SerializeField] private TextMeshProUGUI vehicleSpawnTimeText;
    private float vehicleSpawnTime;

    [Header("VehicleSpeed Bölümü")]
    [SerializeField] private Slider vehicleSpeedSlider;
    [SerializeField] private TextMeshProUGUI vehicleSpeedText;
    private int vehicleSpeed;

    [Header("VehicleCount Bölümü")]
    [SerializeField] private Slider vehicleSpawnCountSlider;
    [SerializeField] private TextMeshProUGUI vehicleSpawnCountText;
    private int vehicleSpawnCount;

    private float windowAnimationSpeed = .6f;

    private bool isPanelOpened;

    private void Start() {
        GetInitialSettings();

        vehicleSpawnCountSlider.interactable = false;
    }

    private void GetInitialSettings() {
        //vehicleSpawnTime
        vehicleSpawnTime = VehicleManager.Instance.GetSpawnTime();
        vehicleSpawnTimeSlider.SetValueWithoutNotify(vehicleSpawnTime / 0.05f); //slider.value þeklinde atama yaparsak event tetikleriz
        vehicleSpawnTimeText.text = vehicleSpawnTime.ToString("F2");
        
        //vehicleSpeed
        vehicleSpeed = VehicleManager.Instance.GetVehicleSpeed();
        vehicleSpeedSlider.SetValueWithoutNotify(vehicleSpeed);
        vehicleSpeedText.text = vehicleSpeed.ToString();

        //vehicleCount
        vehicleSpawnCount = VehicleManager.Instance.GetVehicleSpawnCount();
        vehicleSpawnCountSlider.SetValueWithoutNotify(vehicleSpawnCount);
        vehicleSpawnCountText.text = vehicleSpawnCount.ToString();

    }

    public void HandleWindowButton() {
        if (isPanelOpened)
            HideSettingsPanel();
        else
            ShowSettingsPanel();
    }

    public void ShowSettingsPanel() {
        settingsButton.enabled = false;

        transform.DOMoveY(activePositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
                settingsButton.enabled = true;
            });

        settingsPanel.gameObject.SetActive(true);
        isPanelOpened = true;
    }

    public void HideSettingsPanel() {
        settingsButton.enabled = false;

        transform.DOMoveY(disabledPositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
                settingsPanel.gameObject.SetActive(false);
                settingsButton.enabled = true;
            });

        isPanelOpened = false;
    }

    public void UpdateVehicleSpawnTime() {
        vehicleSpawnTime = vehicleSpawnTimeSlider.value * 0.05f; 
        vehicleSpawnTimeText.text = vehicleSpawnTime.ToString("F2");

        VehicleManager.Instance.SetSpawnTime(vehicleSpawnTime);
    }

    public void UpdateVehicleSpeed() {
        vehicleSpeed = (int) vehicleSpeedSlider.value;
        vehicleSpeedText.text = vehicleSpeed.ToString();

        VehicleManager.Instance.SetVehicleSpeed(vehicleSpeed);
    }

    public void UpdateVehicleSpawnCount() {
        vehicleSpawnCount = (int) vehicleSpawnCountSlider.value;
        vehicleSpawnCountText.text = vehicleSpawnCount.ToString();

        VehicleManager.Instance.SetVehicleSpawnCount(vehicleSpawnCount);
    }
    
}
