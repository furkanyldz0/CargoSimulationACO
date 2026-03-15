using System.Collections.Generic;
using UnityEngine;

public static class Djikstra
{
    public static List<CitySO> FindShortestPath(CitySO startCity, CitySO endCity) {
        Dictionary<CitySO, float> distances = new Dictionary<CitySO, float>();
        Dictionary<CitySO, CitySO> previousNodes = new Dictionary<CitySO, CitySO>();
        List<CitySO> unvisited = new List<CitySO>();

        // 1. Tüm þehirleri topla ve mesafeleri sonsuz yap
        foreach (Road road in GraphManager.Instance.GetAllRoads()) {
            if (!unvisited.Contains(road.startCitySO)) unvisited.Add(road.startCitySO);
            if (!unvisited.Contains(road.endCitySO)) unvisited.Add(road.endCitySO);
        }
        foreach (CitySO city in unvisited) {
            distances[city] = float.MaxValue;
        }

        // Baþlangýç noktasýnýn mesafesi 0'dýr
        distances[startCity] = 0f;

        // 2. Ana Döngü
        while (unvisited.Count > 0) {
            // Ziyaret edilmemiþler arasýndan en kýsa mesafeli þehri bul
            CitySO current = unvisited[0];
            foreach (CitySO city in unvisited) {
                if (distances[city] < distances[current]) {
                    current = city;
                }
            }

            unvisited.Remove(current);

            // Hedefe ulaþtýysak aramayý bitir
            if (current == endCity) break;

            // Komþularýn mesafelerini güncelle
            foreach (CitySO neighbor in current.neighbors) {
                if (!unvisited.Contains(neighbor)) continue;

                Road road = GraphManager.Instance.GetRoadBetween(current, neighbor);
                if (road != null) {
                    float alt = distances[current] + road.distance;
                    if (alt < distances[neighbor]) {
                        distances[neighbor] = alt;
                        previousNodes[neighbor] = current; // Nereden geldiðimizi kaydet
                    }
                }
            }
        }

        // 3. Rotayý Geriye Doðru Çiz
        List<CitySO> path = new List<CitySO>();
        CitySO step = endCity;

        while (previousNodes.ContainsKey(step)) {
            path.Add(step);
            step = previousNodes[step];
        }

        path.Reverse(); // Listeyi baþtan sona doðru çevir
        return path;
    }
}
