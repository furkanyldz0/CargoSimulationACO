using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set; }

    public CitySO TargetCity { get; set; }

    [SerializeField] private Transform RoadParent; //tüm road'larý içeren gameobject
    private List<Road> allRoads = new List<Road>();

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("sahnede birden fazla GraphManager var!");
        }
        Instance = this;

        allRoads = RoadParent.GetComponentsInChildren<Road>().ToList();
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
