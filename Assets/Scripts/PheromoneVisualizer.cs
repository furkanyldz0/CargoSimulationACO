using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Splines.ExtrusionShapes;

public class PheromoneVisualizer : MonoBehaviour {
    [Header("Bileþenler")]
    public LineRenderer lineRenderer;
    public SplineContainer splineContainer;

    [Header("Feromon Görsel Ayarlarý")]
    public float heightOffset = 0f;
    public int resolution = 20;

    [Header("Kalýnlýk Sýnýrlarý")]
    public float maxThickness = 8.0f; // Maksimum kalýnlýk
    public float maxExpectedPheromone = 100f; // Ulaþýlmasýný beklediðimiz maksimum feromon

    private Road road;

    void Start() {
        road = GetComponent<Road>();
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (splineContainer == null) splineContainer = GetComponentInChildren<SplineContainer>();

        DrawPheromonePath();

        // BAÞLANGIÇTA TAMAMEN GÖRÜNMEZ (WIDTH = 0)
        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;
    }

    private void Update() {
        UpdatePheromoneVisual(road.pheromoneLevel);
    }

    private void DrawPheromonePath() {
        if (splineContainer == null || splineContainer.Splines.Count == 0) return;

        var spline = splineContainer.Splines[0];
        lineRenderer.positionCount = resolution;

        for (int i = 0; i < resolution; i++) {
            float t = i / (float)(resolution - 1);
            Vector3 localPos = spline.EvaluatePosition(t);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);

            worldPos.y += heightOffset;
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    // Feromon deðiþtikçe bu metot çalýþacak
    public void UpdatePheromoneVisual(float currentPheromoneAmount) {
        //Feromon oranýný 0 ile 1 arasýna sýkýþtýrýyoruz(Eðer feromon eksiye düþerse veya max'ý aþarsa diye)
        float normalizedLevel = Mathf.Clamp01(currentPheromoneAmount / maxExpectedPheromone);

        // Ulaþmasý gereken net kalýnlýðý hesaplýyoruz
        float targetWidth = normalizedLevel * maxThickness;

        if (currentPheromoneAmount <= 0.1f) {
            targetWidth = 0f;
        }

        // Çarpan (multiplier) yerine doðrudan gerçek kalýnlýk deðerlerini ayarlýyoruz
        lineRenderer.startWidth = targetWidth;
        lineRenderer.endWidth = targetWidth;
    }
}