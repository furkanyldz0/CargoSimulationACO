using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set; }

    [SerializeField] private List<Road> allRoads; // Tüm yollarý buraya sürükleyip býrakacaksýn
    [SerializeField] private float evaporationRate = 0.05f; // Buharlaþma hýzý (0 ile 1 arasý)
    [SerializeField] private float minPheromone = 0.1f;    // Feromonun tamamen yok olmamasý için alt sýnýr

    public CitySO targetCity;
    
    private void Awake() {
        if(Instance != null) {
            Debug.LogError("sahnede birden fazla GraphManager var!");
        }
        Instance = this;

        Debug.Log("ACO feromonun önemi: " + ACOSelection.alpha +
            " - Yolun önemi: " + ACOSelection.beta);
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

    public Road GetRoadBetween(CitySO startCity, CitySO endCity) {
        foreach (Road road in allRoads) {
            if (road.startCitySO == startCity && road.endCitySO == endCity) {
                return road;
            }
        }
        Debug.LogError($"{startCity.name} ile {endCity.name} arasýnda bir yol tanýmlanmamýþ!");
        return null;
    }

    public List<Road> GetAllRoads() {
        return allRoads;
    }

}
