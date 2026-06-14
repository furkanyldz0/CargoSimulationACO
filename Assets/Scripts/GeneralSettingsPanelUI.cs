using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettingsPanelUI : MonoBehaviour
{
    [Header("Panel Ayarlarý")]
    [SerializeField] private Transform settingsPanel;
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

    [Header("TimeScale Bölümü")]
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private TextMeshProUGUI timeScaleText;
    private float timeScale;

    [Header("PheromoneTrail Bölümü")]
    [SerializeField] private Toggle pheromoneTrailToggle;

    private float windowAnimationSpeed = .6f;
    private Tween panelTween;

    private bool isPanelOpened;

    private void Start() {
        GetInitialSettings();
        VisualManager.Instance.OnEnteredSelectionMode += Instance_OnEnteredSelectionMode;
        VisualManager.Instance.OnExitedSelectionMode += Instance_OnExitedSelectionMode;
    }

    private void OnDestroy() {
        VisualManager.Instance.OnEnteredSelectionMode -= Instance_OnEnteredSelectionMode;
        VisualManager.Instance.OnExitedSelectionMode -= Instance_OnExitedSelectionMode;
    }

    private void Instance_OnEnteredSelectionMode(object sender, EventArgs e) {
        ShowSettingsPanel();
    }

    private void Instance_OnExitedSelectionMode(object sender, EventArgs e) {
        HideSettingsPanel();
    }

    //VehicleManager ve LevelManager'da tanýmlý ilk deðerleri alýr
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

        //timeScale
        timeScale = LevelManager.TimeScale;
        timeScaleSlider.SetValueWithoutNotify(timeScale / 0.25f);
        timeScaleText.text = timeScale.ToString("F2") + "x";

        //pheromoneTrailToggle
        pheromoneTrailToggle.isOn = true;
    }

    public void HandleWindowButton() {
        if (isPanelOpened) {
            HideSettingsPanel();
            AudioManager.Instance.PlayButtonBackClickSound();
        }
        else {
            ShowSettingsPanel();
            AudioManager.Instance.PlayButtonClickSound();
        }
            
    }

    public void HandlePheromoneTrailToggle() {
        VisualManager.Instance.HandlePheromoneTrailsVisibility(pheromoneTrailToggle.isOn);
    }

    private void ShowSettingsPanel() {
        panelTween?.Kill(); //tween null deðilse, yani hala oynuyorsa zorla kesecek

        settingsPanel.gameObject.SetActive(true);
        isPanelOpened = true;

        panelTween = transform.DOMoveY(activePositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
            });
    }

    private void HideSettingsPanel() {
        panelTween?.Kill();

        isPanelOpened = false;

        panelTween = transform.DOMoveY(disabledPositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
                settingsPanel.gameObject.SetActive(false);
            });
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

    public void UpdateTimeScale() {
        timeScale = timeScaleSlider.value * 0.25f;
        timeScaleText.text = timeScale.ToString("F2") + "x";

        LevelManager.SetTimeScale(timeScale);
    }

}
