using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    [Header("TimeScale Bölümü")]
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private TextMeshProUGUI timeScaleText;
    private float timeScale;

    [Header("PheromoneTrail Bölümü")]
    [SerializeField] private Toggle pheromoneTrailToggle;

    private float windowAnimationSpeed = .6f;
    private Tween panelTween;

    private bool isPanelOpened;

    private List<LineRenderer> allPheromoneTrails = new List<LineRenderer>();

    private void Start() {
        GetInitialSettings();

        List<Road> allRoads = GraphManager.Instance.GetAllRoads();
        foreach (Road road in allRoads) {
            LineRenderer lineRenderer = road.GetComponentInChildren<LineRenderer>();
            allPheromoneTrails.Add(lineRenderer);
        }
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

        //timeScale
        timeScale = VehicleManager.Instance.GetTimeScale();
        timeScaleSlider.SetValueWithoutNotify(timeScale / 0.25f);
        timeScaleText.text = timeScale.ToString() + "x";

        //pheromoneTrailToggle
        pheromoneTrailToggle.isOn = true;
    }

    public void HandleWindowButton() {
        if (isPanelOpened)
            HideSettingsPanel();
        else
            ShowSettingsPanel();
    }

    public void HandlePheromoneTrailToggle() {
        if (pheromoneTrailToggle.isOn)
            ShowPheromoneTrails();
        else
            HidePheromoneTrails();
    }

    private void ShowSettingsPanel() {
        //settingsButton.enabled = false;

        panelTween?.Kill(); //tween null deðilse, yani hala oynuyorsa zorla kesecek

        settingsPanel.gameObject.SetActive(true);
        isPanelOpened = true;

        panelTween = transform.DOMoveY(activePositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
                //settingsButton.enabled = true;
            });
    }

    private void HideSettingsPanel() {
        //settingsButton.enabled = false;

        panelTween?.Kill();

        isPanelOpened = false;

        panelTween = transform.DOMoveY(disabledPositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
                settingsPanel.gameObject.SetActive(false);
                //settingsButton.enabled = true;
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

        VehicleManager.Instance.SetTimeScale(timeScale);
    }
    public void ShowPheromoneTrails() {
        foreach (var lineRenderer in allPheromoneTrails) {
            lineRenderer.gameObject.SetActive(true);
        }
    }

    public void HidePheromoneTrails() {
        foreach (var lineRenderer in allPheromoneTrails) {
            lineRenderer.gameObject.SetActive(false);
        }
    }

}
