using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set; }

    [SerializeField] private List<Road> allEdges; // Tüm yollarý buraya sürükleyip býrakacaksýn

    public CitySO targetCity;

    public float evaporationRate = 0.05f; // Buharlaþma hýzý (0 ile 1 arasý)
    public float minPheromone = 0.1f;    // Feromonun tamamen yok olmamasý için alt sýnýr
    
    private void Awake() {
        if(Instance != null) {
            Debug.LogError("sahnede birden fazla GraphManager var!");
        }
        Instance = this;
    }

    private void Update() {
        // Her karede veya belirli aralýklarla tüm yollarý buharlaþtýr
        foreach (Road road in allEdges) {
            road.pheromoneLevel *= (1f - evaporationRate * Time.deltaTime);

            if (road.pheromoneLevel < minPheromone) {
                road.pheromoneLevel = minPheromone;
            }
        }
    }

    //public Transform GetWaypointParentBetween(CitySO startCity, CitySO endCity) {
    //    foreach (Road edge in allEdges) {
    //        if (edge.startCity == startCity && edge.endCity == endCity) {
    //            return edge.waypointParent;
    //        }
    //    }
    //    Debug.LogError($"{startCity.name} ile {endCity.name} arasýnda bir yol tanýmlanmamýþ!");
    //    return null;
    //}

    public Road GetRoadBetween(CitySO startCity, CitySO endCity) {
        foreach (Road road in allEdges) {
            if (road.startCitySO == startCity && road.endCitySO == endCity) {
                return road;
            }
        }
        return null;
    }
}
