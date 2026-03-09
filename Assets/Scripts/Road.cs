using UnityEngine;

[SelectionBase]
public class Road : MonoBehaviour {
    public CitySO startCity; // Baþlangýç (Örn: B)
    public CitySO endCity;   // Hedef (Örn: D)
    public Transform waypointParent; // Bu iki þehir arasýndaki fiziksel yol

    public float distance = 100f;       // Yolun uzunluðu (Dijkstra ve ACO için)
    public float pheromoneLevel = 1f; // Baþlangýç feromon miktarý
}

