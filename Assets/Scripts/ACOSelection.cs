using System.Collections.Generic;
using UnityEngine;

public static class ACOSelection {
    // ACO Katsayýlarý (Bunlarý daha sonra bir Manager'dan da çekebiliriz)
    public static float alpha = 1.0f; // Feromonun önemi
    public static float beta = 1.0f;  // Mesafenin önemi (Mesafe kýsa olunca çekicilik artar)

    public static CitySO ChooseNextCity(CitySO currentCity, List<CitySO> visitedCities) {

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
}