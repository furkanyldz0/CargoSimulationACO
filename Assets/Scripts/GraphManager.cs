using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set; }

    public CitySO TargetCitySO { get; private set; }
    public CitySO StartCitySO { get; private set; }

    [SerializeField] private Transform RoadParent; //tüm road'larý içeren gameobject
    private List<Road> allRoads = new List<Road>();
    private Dictionary<(CitySO, CitySO), Road> roadLookup = new Dictionary<(CitySO, CitySO), Road>();

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("sahnede birden fazla GraphManager var!");
        }
        Instance = this;

        allRoads = RoadParent.GetComponentsInChildren<Road>().ToList();
        foreach (Road road in allRoads) {
            roadLookup[(road.startCitySO, road.endCitySO)] = road;
        }
    }

    public Road GetRoadBetween(CitySO startCity, CitySO endCity) {
        if (roadLookup.TryGetValue((startCity, endCity), out Road road))
            return road; //dictionary O(1) olduðu için listede aramaya göre daha performanslý

        //foreach (Road road in allRoads) {
        //    if (road.startCitySO == startCity && road.endCitySO == endCity) {
        //        return road;
        //    }
        //}
        Debug.LogError($"{startCity.name} ile {endCity.name} arasýnda bir yol tanýmlanmamýþ!");
        return null;
    }

    public List<Road> GetAllRoads() {
        return allRoads;
    }

    public void SetTargetCity(CitySO citySO) {
        TargetCitySO = citySO;
    }

    public void SetStartCity(CitySO citySO) {
        StartCitySO = citySO;
    }
}
