using System.Collections.Generic;
using UnityEngine;

public class ACOManager : MonoBehaviour {

    public static ACOManager Instance { get; private set; }

    [Header("Alpha: Feromonun önemi - Beta: Mesafenin önemi")]
    [SerializeField] private float alpha = 1.0f; // Feromonun önemi
    [SerializeField] private float beta = 1.0f;  // Mesafenin önemi (Mesafe kýsa olunca çekicilik artar)

    [Header("Yollar için feromon özellikleri")]
    [SerializeField] private float startPheremoneLevel = 1f;
    [SerializeField] private float evaporationRate = 0.05f; // Buharlaþma hýzý (0 ile 1 arasý)
    [SerializeField] private float minPheromone = 0.1f;    // Feromonun tamamen yok olmamasý için alt sýnýr

    [Header("Q sabit deðeri (Býrakýlan feromon: Q / L^2)")]
    [SerializeField] private float Q = 10000f;

    private List<Road> allRoads;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("Sahnede birden fazla ACOManager var!");
        }
        Instance = this;
    }

    private void Start() {
        allRoads = GraphManager.Instance.GetAllRoads();

        foreach(Road road in allRoads) {
            road.pheromoneLevel = startPheremoneLevel;
        }

        Debug.Log("ACO feromonun önemi: " + alpha + " - Yolun önemi: " + beta);
    }

    private void Update() {
        // Her karede veya belirli aralýklarla tüm yollarý buharlaþtýr
        foreach (Road road in allRoads) {
            road.pheromoneLevel *= (1f - evaporationRate * Time.deltaTime);

            if (road.pheromoneLevel < minPheromone) {
                road.pheromoneLevel = minPheromone;
            }
        }
    }

    public CitySO ChooseNextCity(CitySO currentCity, List<CitySO> visitedCities) {

        // 2. Sadece daha önce gidilmemiþ komþularý filtrele
        List<CitySO> availableNeighbors = new List<CitySO>();

        foreach (CitySO neighbor in currentCity.neighbors) {
            if (!visitedCities.Contains(neighbor)) {
                availableNeighbors.Add(neighbor);
            }
        }

        // 3. Eðer gidilecek yeni yer kalmadýysa (çýkmaz sokak), 
        // ping-pong yapmamak için rastgele bir komþuya dön
        if (availableNeighbors.Count == 0) {
            return currentCity.neighbors[Random.Range(0, currentCity.neighbors.Count)];
        }

        // 4. Olasýlýk Hesaplama (Sadece gidilebilir komþular için)
        List<float> scores = new List<float>();
        float totalScore = 0f;

        foreach (CitySO neighbor in availableNeighbors) {
            Road road = GraphManager.Instance.GetRoadBetween(currentCity, neighbor);
            if (road != null) {
                // Formül: P = (Feromon^alpha) * ((1/Mesafe)^beta)
                float tau = Mathf.Pow(road.pheromoneLevel, alpha);
                float eta = Mathf.Pow(1f / road.distance, beta);
                float score = tau * eta;

                scores.Add(score);
                totalScore += score;
            }
        }

        // 5. Rulet Tekerleði Seçimi
        float randomValue = Random.Range(0f, totalScore);
        float cumulativeScore = 0f;

        for (int i = 0; i < availableNeighbors.Count; i++) {
            cumulativeScore += scores[i];
            if (randomValue <= cumulativeScore) {
                return availableNeighbors[i];
            }
        }

        return availableNeighbors[0];
    }

    public void AddPheromones(List<Road> traveledRoads) {
        float totalDistance = 0;
        // 1. Toplam mesafeyi hesapla
        foreach (Road r in traveledRoads) {
            totalDistance += r.distance;
        }

        // 2. Yol ne kadar kýsaysa o kadar çok feromon býrak (Q / L formülü)
        // Q sabit bir deðerdir (örn: 100)
        float pheromoneToAdd = Q / (totalDistance * totalDistance);

        foreach (Road r in traveledRoads) {
            r.pheromoneLevel += pheromoneToAdd;
        }

    }
}