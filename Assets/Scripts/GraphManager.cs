using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set; }

    //[SerializeField] private List<Road> allRoads; //sürekli sürükleyip býrakmamak için parent kullanacaðým
    [SerializeField] private Transform RoadParent;
    private List<Road> allRoads = new List<Road>();

    public CitySO targetCity;

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
