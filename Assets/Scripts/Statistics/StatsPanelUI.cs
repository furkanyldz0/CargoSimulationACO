using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour {
    [Header("Panel Yapısı")]
    [SerializeField] private GameObject statsButton;
    [SerializeField] private Transform statsPanel;
    [SerializeField] private Transform activePositionTransform;
    [SerializeField] private Transform disabledPositionTransform;

    [Header("Metrik Text'leri")]
    [SerializeField] private TextMeshProUGUI deliveryCountText;
    [SerializeField] private TextMeshProUGUI dijkstraInfoText;
    [SerializeField] private TextMeshProUGUI bestAcoInfoText;
    [SerializeField] private TextMeshProUGUI dominantInfoText;
    [SerializeField] private TextMeshProUGUI lastRatioText;
    [SerializeField] private TextMeshProUGUI averageRatioText;
    [SerializeField] private TextMeshProUGUI bestRatioText;
    [SerializeField] private TextMeshProUGUI parametersText;

    private float windowAnimationSpeed = 0.6f;
    private Tween panelTween;
    private bool isPanelOpened;

    private void Start() {
        // Başlangıçta her şey gizli, panel offscreen
        statsButton.SetActive(false);
        statsPanel.gameObject.SetActive(false);

        LevelManager.Instance.OnSimulationStarted += LevelManager_OnSimulationStarted;
        LevelManager.Instance.OnSimulationStopped += LevelManager_OnSimulationStopped;
        StatsManager.Instance.OnStatsUpdated += StatsManager_OnStatsUpdated;
    }

    private void LevelManager_OnSimulationStarted(object sender, System.EventArgs e) {
        statsButton.SetActive(true);
        RefreshLabels(); // başlangıç verileri (Dijkstra hazır, henüz teslimat yok)
    }

    private void LevelManager_OnSimulationStopped(object sender, System.EventArgs e) {
        if (isPanelOpened) HideStatsPanel();
        statsButton.SetActive(false);
    }

    private void StatsManager_OnStatsUpdated(object sender, System.EventArgs e) {
        RefreshLabels();
    }

    public void HandleWindowButton() {
        if (isPanelOpened)
            HideStatsPanel();
        else
            ShowStatsPanel();
    }

    public void ShowStatsPanel() {
        panelTween?.Kill();
        statsPanel.gameObject.SetActive(true);
        isPanelOpened = true;

        panelTween = transform.DOMoveY(activePositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart);
    }

    public void HideStatsPanel() {
        panelTween?.Kill();
        isPanelOpened = false;

        panelTween = transform.DOMoveY(disabledPositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
                statsPanel.gameObject.SetActive(false);
            });
    }

    private void RefreshLabels() {
        if (!StatsManager.Instance.IsRunActive()) return;

        // Toplam teslimat
        deliveryCountText.text = StatsManager.Instance.GetDeliveryCount().ToString();

        // Dijkstra rotası
        var dijkstraPath = StatsManager.Instance.GetDijkstraPath();
        float dijkstraLength = StatsManager.Instance.GetDijkstraLength();
        dijkstraInfoText.text = $"{PathToString(dijkstraPath)}\nUzunluk: {dijkstraLength:F2} km";

        // ACO en iyi
        var bestAcoRoute = StatsManager.Instance.GetBestAcoRoute();
        if (bestAcoRoute != null) {
            float bestAcoLen = StatsManager.Instance.GetBestAcoLength();
            int count = StatsManager.Instance.GetBestAcoRouteCount();
            bestAcoInfoText.text = $"{PathToString(bestAcoRoute)}\nUzunluk: {bestAcoLen:F2} km  ({count} araç)";
        } else {
            bestAcoInfoText.text = "-";
        }

        // ACO yakınsadığı (pheromone-dominant)
        var dominantPath = StatsManager.Instance.ComputeDominantPath();
        if (dominantPath != null && dominantPath.Count > 1 && dominantPath[dominantPath.Count - 1] == GraphManager.Instance.TargetCitySO) {
            float dominantLen = Djikstra.CalculatePathLength(dominantPath);
            dominantInfoText.text = $"{PathToString(dominantPath)}\nUzunluk: {dominantLen:F2} km";
        } else {
            dominantInfoText.text = "-"; //henüz yakınsama yok
        }

        // Oranlar
        float last = StatsManager.Instance.GetLastRatio();
        float avg = StatsManager.Instance.GetAverageRatio();
        float best = StatsManager.Instance.GetBestRatio();
        lastRatioText.text = last > 0 ? $"{last:F2}x" : "-";
        averageRatioText.text = avg > 0 ? $"{avg:F2}x (son 50)" : "-";
        bestRatioText.text = best < float.MaxValue ? $"{best:F2}x" : "-";

        // Parametreler
        float alpha = ACOManager.Instance.GetAlpha();
        float beta = ACOManager.Instance.GetBeta();
        float evap = ACOManager.Instance.GetEvaporationRate();
        int q = ACOManager.Instance.GetQ();
        parametersText.text = $"α={alpha:F2}  β={beta:F2}  p={evap:F2}  Q={q}";
    }

    private string PathToString(List<CitySO> path) {
        if (path == null || path.Count == 0) return "-";
        return string.Join("→", path.ConvertAll(c => c.cityName));
    }
}