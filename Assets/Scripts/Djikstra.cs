using System.Collections.Generic;
using UnityEngine;

public static class Djikstra
{
    // Run boyunca sabit kalan başlangıç→hedef rotasının cache'i
    public static List<CitySO> CachedStartToTargetPath { get; private set; }
    private static List<CitySO> CachedReturnPath;
    public static float CachedStartToTargetLength { get; private set; }

    public static void CacheStartToTargetPath() {
        CitySO start = GraphManager.Instance.StartCitySO;
        CitySO target = GraphManager.Instance.TargetCitySO;

        CachedStartToTargetPath = FindShortestPath(start, target);

        // FindShortestPath başlangıç şehrini path'e dahil etmiyor, ekleyelim
        if (CachedStartToTargetPath.Count == 0 || CachedStartToTargetPath[0] != start) {
            CachedStartToTargetPath.Insert(0, start);
        }

        CachedStartToTargetLength = CalculatePathLength(CachedStartToTargetPath);

        Debug.Log($"[Djikstra] Cache hazır: {string.Join("->", CachedStartToTargetPath.ConvertAll(c => c.cityName))} | Uzunluk: {CachedStartToTargetLength:F2}");
    }

    public static List<CitySO> GetCachedReturnPath() {
        if (CachedStartToTargetPath == null) return null;

        if(CachedReturnPath == null) {
            List<CitySO> reversed = new List<CitySO>(CachedStartToTargetPath);
            reversed.Reverse();         // [target, c2, c1, start]
            reversed.RemoveAt(0);       // [c2, c1, start] — currentCity'yi çıkar
            CachedReturnPath = reversed;
        }

        return CachedReturnPath;
    }

    public static void ClearCache() {
        CachedStartToTargetPath = null;
        CachedReturnPath = null;
        CachedStartToTargetLength = 0f;
    }

    public static float CalculatePathLength(List<CitySO> path) {
        float total = 0;
        for (int i = 0; i < path.Count - 1; i++) {
            Road r = GraphManager.Instance.GetRoadBetween(path[i], path[i + 1]);
            if (r != null) total += r.distance;
        }
        return total;
    }

    public static List<CitySO> FindShortestPath(CitySO startCity, CitySO endCity) {
        Dictionary<CitySO, float> distances = new Dictionary<CitySO, float>();
        Dictionary<CitySO, CitySO> previousNodes = new Dictionary<CitySO, CitySO>();
        List<CitySO> unvisited = new List<CitySO>();

        // 1. Tüm şehirleri topla ve mesafeleri sonsuz yap
        foreach (Road road in GraphManager.Instance.GetAllRoads()) {
            if (!unvisited.Contains(road.startCitySO)) unvisited.Add(road.startCitySO);
            if (!unvisited.Contains(road.endCitySO)) unvisited.Add(road.endCitySO);
        }
        foreach (CitySO city in unvisited) {
            distances[city] = float.MaxValue;
        }

        // Başlangıç noktasının mesafesi 0'dır
        distances[startCity] = 0f;

        // 2. Ana Döngü
        while (unvisited.Count > 0) {
            // Ziyaret edilmemişler arasından en kısa mesafeli şehri bul
            CitySO current = unvisited[0];
            foreach (CitySO city in unvisited) {
                if (distances[city] < distances[current]) {
                    current = city;
                }
            }

            unvisited.Remove(current);

            // Hedefe ulaştıysak aramayı bitir
            if (current == endCity) break;

            // Komşuların mesafelerini güncelle
            foreach (CitySO neighbor in current.neighbors) {
                if (!unvisited.Contains(neighbor)) continue;

                Road road = GraphManager.Instance.GetRoadBetween(current, neighbor);
                if (road != null) {
                    float alt = distances[current] + road.distance;
                    if (alt < distances[neighbor]) {
                        distances[neighbor] = alt;
                        previousNodes[neighbor] = current; // Nereden geldiğimizi kaydet
                    }
                }
            }
        }

        // 3. Rotayı Geriye Doğru Çiz
        List<CitySO> path = new List<CitySO>();
        CitySO step = endCity;

        while (previousNodes.ContainsKey(step)) {
            path.Add(step);
            step = previousNodes[step];
        }

        path.Reverse(); // Listeyi baştan sona doğru çevir
        return path;
    }
}
