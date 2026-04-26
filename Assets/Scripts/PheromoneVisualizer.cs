using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Splines.ExtrusionShapes;

public class PheromoneVisualizer : MonoBehaviour {

    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private int resolution = 20; //spline'nýn kavisli olmasý durumunda yumuþak geçiþ için
    [SerializeField] private float maxThickness = 8.0f; // Maksimum kalýnlýk
    [SerializeField] private float maxPheromoneLimitToVisualized = 100f; // Ulaþýlmasýný beklediðimiz maksimum feromon

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private SplineContainer splineContainer;
    private Road road;

    void Start() {
        road = GetComponent<Road>();
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (splineContainer == null) splineContainer = GetComponentInChildren<SplineContainer>();

        DrawPheromonePath();

        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;
    }

    private void Update() {
        UpdatePheromoneVisual(road.pheromoneLevel);
    }

    private void DrawPheromonePath() {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
            Debug.LogError(this + " için splineContainer atanmamýþ veya spline içermiyor!");

        var spline = splineContainer.Splines[0];
        lineRenderer.positionCount = resolution;

        for (int i = 0; i < resolution; i++) {
            float t = i / (float)(resolution - 1); //noktalarýn yüzdeðiline göre spline üzerinde konumlarýný ayarlayacaðýz

            Vector3 localPos = spline.EvaluatePosition(t);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos); //localpos'dan worldpos'u buluyoruz

            worldPos.y += heightOffset;
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    public void UpdatePheromoneVisual(float currentPheromoneAmount) {
        //feromon zoranýný 0 ile 1 arasýna sabitliyoruz
        float normalizedLevel = Mathf.Clamp01(currentPheromoneAmount / maxPheromoneLimitToVisualized); //öncekinde alfa deðerinde nasýl 0 ile 1 arasý sabitliyorsak burada da kalýnlýðýn yüzdeliðini hesaplýyoruz

        float targetWidth = normalizedLevel * maxThickness; //bi nevi 0-1 clamp iþlemi yapýyoruz gibi ama çarparak

        if (currentPheromoneAmount <= 0.1f) {
            targetWidth = 0f;
        }

        lineRenderer.startWidth = targetWidth;
        lineRenderer.endWidth = targetWidth;
    }
}