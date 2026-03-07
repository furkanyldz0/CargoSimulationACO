using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set; }

    [SerializeField] private List<Road> allEdges; // Tüm yollarý buraya sürükleyip býrakacaksýn

    public CitySO targetCity;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("sahnede birden fazla GraphManager var!");
        }
        Instance = this;
    }

    public Transform GetWaypointParentBetween(CitySO startCity, CitySO endCity) {
        foreach (Road edge in allEdges) {
            if (edge.startCity == startCity && edge.endCity == endCity) {
                return edge.waypointParent;
            }
        }
        Debug.LogError($"{startCity.name} ile {endCity.name} arasýnda bir yol tanýmlanmamýþ!");
        return null;
    }
}
