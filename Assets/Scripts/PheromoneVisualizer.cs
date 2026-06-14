using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Splines.ExtrusionShapes;

public class PheromoneVisualizer : MonoBehaviour {

    [SerializeField] private float heightOffset = 0.05f; //Feromon çizgisinin yol mesh'i ile iç içe geçmemesi için yükseklik payý
    [SerializeField] private int resolution = 50; //Çizgideki nokta sayýsý
    [SerializeField] private float maxThickness = 6f; //Maksimum kalýnlýk
    [SerializeField] private float maxPheromoneLimitToVisualized = 100f; //Alabileceði maksimum feromon

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private SplineContainer splineContainer;
    private Road road;

    void Start() {
        road = GetComponent<Road>();
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (splineContainer == null) splineContainer = GetComponentInChildren<SplineContainer>();

        DrawPheromoneTrail();
        ResetPheromoneTrailThickness();

        VisualManager.Instance.OnPheromoneTrailsVisibilityChanged += Instance_OnPheromoneTrailsVisibilityChanged;
    }

    private void OnDestroy() {
        VisualManager.Instance.OnPheromoneTrailsVisibilityChanged -= Instance_OnPheromoneTrailsVisibilityChanged;
    }

    private void Instance_OnPheromoneTrailsVisibilityChanged(object sender, VisualManager.OnPheromoneTrailsVisibilityChangedEventArgs e) {
        if (e.isEnabled)
            lineRenderer.gameObject.SetActive(true);
        else
            lineRenderer.gameObject.SetActive(false);
    }

    private void Update() {
        if (!LevelManager.Instance.IsSimulationInitiated)
            return;

        UpdatePheromoneTrail(road.pheromoneLevel);
    }

    //Simülasyon baþlatýldýðýnda feromon çizgilerini çizer ve konumlarýný spline üzerine tanýmlar(çizgiler önceden tanýmlý deðil)
    private void DrawPheromoneTrail() {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
            Debug.LogError(this + " için splineContainer atanmamýþ veya spline içermiyor!");

        var spline = splineContainer.Splines[0];
        lineRenderer.positionCount = resolution;

        for (int i = 0; i < resolution; i++) {
            float t = i / (float)(resolution - 1); //spline üzerinde noktalarý daðýtýyoruz, -1 for döngüsünün exception fýrlatmamasý için, zaten 0 ila 1 arasýnda deðerler alacak

            Vector3 localPos = spline.EvaluatePosition(t); //noktalarýn yüzdeðiline göre spline üzerinde konumlarýný alýyoruz
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos); //localpos'dan worldpos'u buluyoruz

            worldPos.y += heightOffset;
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    //Feromon seviyesine göre çizgileri günceller
    private void UpdatePheromoneTrail(float currentPheromoneAmount) {
        //feromon oranýný 0 ile 1 arasýna sabitliyoruz
        float normalizedLevel = Mathf.Clamp01(currentPheromoneAmount / maxPheromoneLimitToVisualized); //öncekinde alfa deðerinde nasýl 0 ile 1 arasý sabitliyorsak burada da kalýnlýðýn yüzdeliðini hesaplýyoruz

        float targetWidth = normalizedLevel * maxThickness;

        if (currentPheromoneAmount <= 0.1f) {
            targetWidth = 0f; //feromon seviyesi 0.1 altýnda ise çizgiyi gösterme
        }

        lineRenderer.startWidth = targetWidth;
        lineRenderer.endWidth = targetWidth;
    }

    public void ResetPheromoneTrailThickness() {
        lineRenderer.startWidth = 0;
        lineRenderer.endWidth = 0;
    }
}