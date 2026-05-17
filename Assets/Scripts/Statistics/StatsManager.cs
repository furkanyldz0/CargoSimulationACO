using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Globalization;

public class StatsManager : MonoBehaviour {
    public static StatsManager Instance { get; private set; }

    private StreamWriter csvWriter;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public event EventHandler OnStatsUpdated;

    // Kayan pencere boyutu (son N teslimat)
    private const int WINDOW_SIZE = 50;

    // Run başında bir kez hesaplanıp cache'lenen Dijkstra referansı
    private List<CitySO> dijkstraPath;
    private float dijkstraLength;

    // Run durumu
    private bool isRunActive;
    private int deliveryCount;
    private float runStartTime;

    // En iyi (en kısa) ACO rotası
    private List<CitySO> bestAcoRoute;
    private float bestAcoLength = float.MaxValue;
    private int bestAcoRouteCount;

    // Oran istatistikleri
    private float lastRatio;
    private float bestRatio = float.MaxValue;
    private Queue<float> recentRatios = new Queue<float>();

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Sahnede birden fazla StatsManager var!");
        }
        Instance = this;
    }

    public void StartRun() {
        // Tüm istatistikleri sıfırla
        deliveryCount = 0;
        runStartTime = Time.time;
        bestAcoRoute = null;
        bestAcoLength = float.MaxValue;
        bestAcoRouteCount = 0;
        lastRatio = 0;
        bestRatio = float.MaxValue;
        recentRatios.Clear();

        // Dijkstra cache'inden referans değerleri al
        dijkstraPath = Djikstra.CachedStartToTargetPath;
        dijkstraLength = Djikstra.CachedStartToTargetLength;

        isRunActive = true;

        Debug.Log($"[Stats] Run başladı | Dijkstra: {PathToString(dijkstraPath)} | Uzunluk: {dijkstraLength:F2}");

        OpenCsvFile();
    }

    public void StopRun() {
        if (!isRunActive) return;
        isRunActive = false;
        Debug.Log($"[Stats] Run durdu | Toplam teslimat: {deliveryCount}");

        CloseCsvFile();
    }

    public void RegisterDelivery(List<CitySO> acoPath, float acoLength) {
        if (!isRunActive) return;

        deliveryCount++;
        float ratio = acoLength / dijkstraLength;
        lastRatio = ratio;

        if (ratio < bestRatio) bestRatio = ratio;

        recentRatios.Enqueue(ratio);
        if (recentRatios.Count > WINDOW_SIZE) recentRatios.Dequeue();

        // En iyi ACO rotası: yeni minimum bulunduysa değiştir,
        // aynı en iyi rotayı tekrar bulan araç varsa sayacı arttır
        if (acoLength < bestAcoLength) {
            bestAcoRoute = new List<CitySO>(acoPath);
            bestAcoLength = acoLength;
            bestAcoRouteCount = 1;
        }
        else if (PathsEqual(acoPath, bestAcoRoute)) {
            bestAcoRouteCount++;
        }

        Debug.Log($"[Stats] Teslimat #{deliveryCount} | " +
                  $"ACO: {PathToString(acoPath)} ({acoLength:F2}) | " +
                  $"Oran: {ratio:F3} | " +
                  $"Ortalama(son {recentRatios.Count}): {GetAverageRatio():F3} | " +
                  $"En iyi oran: {bestRatio:F3} | " +
                  $"En iyi rota kaç araç: {bestAcoRouteCount}");

        AppendCsvRow(acoPath, acoLength, ratio);

        OnStatsUpdated?.Invoke(this, EventArgs.Empty);
    }

    // --- Yardımcı metodlar ---

    private string PathToString(List<CitySO> path) {
        if (path == null || path.Count == 0) return "-";
        return string.Join("->", path.Select(c => c.cityName));
    }

    private bool PathsEqual(List<CitySO> a, List<CitySO> b) {
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++) {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    public float GetAverageRatio() {
        if (recentRatios.Count == 0) return 0;
        return recentRatios.Average();
    }

    public List<CitySO> ComputeDominantPath() {
        CitySO start = GraphManager.Instance.StartCitySO;
        CitySO target = GraphManager.Instance.TargetCitySO;

        if (start == null || target == null) return null;

        List<CitySO> path = new List<CitySO> { start };
        CitySO current = start;
        int safetyLimit = 100; // çok uzun döngülerden koruma

        while (current != target && safetyLimit-- > 0) {
            Road bestRoad = null;
            float maxPheromone = -1f;

            foreach (CitySO neighbor in current.neighbors) {
                if (path.Contains(neighbor)) continue; //aynı şehri atlayarak döngü oluşmasını engeller

                Road road = GraphManager.Instance.GetRoadBetween(current, neighbor);
                if (road != null && road.pheromoneLevel > maxPheromone) {
                    maxPheromone = road.pheromoneLevel;
                    bestRoad = road;
                }
            }

            if (bestRoad == null) break; // çıkmaz, koloni henüz buraya kadar yakınsamış

            current = bestRoad.endCitySO;
            path.Add(current);
        }

        return path;
    }

    private void OpenCsvFile() {
        string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "aco_stats.csv"));
        csvWriter = new StreamWriter(fullPath, append: false); // append:false → her run'da üzerine yazılır
        csvWriter.WriteLine("deliveryIndex,timestamp,alpha,beta,evaporationRate,Q,acoLength,acoPath,dijkstraLength,dijkstraPath,ratio");

        Debug.Log($"[Stats] CSV açıldı: {fullPath}");
    }

    private void CloseCsvFile() {
        if (csvWriter != null) {
            csvWriter.Flush();
            csvWriter.Close();
            csvWriter = null;
        }
    }

    private void AppendCsvRow(List<CitySO> acoPath, float acoLength, float ratio) {
        if (csvWriter == null) return;

        float timestamp = Time.time - runStartTime;
        float alpha = ACOManager.Instance.GetAlpha();
        float beta = ACOManager.Instance.GetBeta();
        float evap = ACOManager.Instance.GetEvaporationRate();
        int q = ACOManager.Instance.GetQ();
        string acoPathStr = PathToString(acoPath);
        string dijkstraPathStr = PathToString(dijkstraPath);

        string line = string.Join(",",
            deliveryCount.ToString(Inv),
            timestamp.ToString("F2", Inv),
            alpha.ToString("F2", Inv),
            beta.ToString("F2", Inv),
            evap.ToString("F3", Inv),
            q.ToString(Inv),
            acoLength.ToString("F2", Inv),
            acoPathStr,
            dijkstraLength.ToString("F2", Inv),
            dijkstraPathStr,
            ratio.ToString("F4", Inv)
        );

        csvWriter.WriteLine(line);
        csvWriter.Flush(); // her satırda flush — oyun çökerse veri kaybolmasın
    }

    // Oyun kapatıldığında veya GameObject destroy olduğunda dosyayı düzgün kapat
    private void OnApplicationQuit() {
        CloseCsvFile();
    }

    private void OnDestroy() {
        CloseCsvFile();
    }

    // --- UI için getter'lar (Faz 3'te kullanacağız) ---

    public bool IsRunActive() { return isRunActive; }
    public int GetDeliveryCount() { return deliveryCount; }
    public float GetLastRatio() { return lastRatio; }
    public float GetBestRatio() { return bestRatio; }
    public List<CitySO> GetBestAcoRoute() { return bestAcoRoute; }
    public float GetBestAcoLength() { return bestAcoLength; }
    public int GetBestAcoRouteCount() { return bestAcoRouteCount; }
    public List<CitySO> GetDijkstraPath() { return dijkstraPath; }
    public float GetDijkstraLength() { return dijkstraLength; }

}