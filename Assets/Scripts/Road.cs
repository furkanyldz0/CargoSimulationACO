using System;
using TMPro;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

[SelectionBase]
public class Road : MonoBehaviour {

    public CitySO startCitySO;
    public CitySO endCitySO;
    public Transform waypointParent;

    private City startCity;
    private City endCity;

    public float distance;
    public float pheromoneLevel = 1f;
    public int useCount = 0;

    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI pheromoneText;
    [SerializeField] private TextMeshProUGUI useCountText;

    private int textUpdateCountPerSecond = 10;
    private float textUpdateTime, textUpdateTimeDelta;

    private void Start() {
        startCity = CityPool.Instance.GetCityForCitySO(startCitySO);
        endCity = CityPool.Instance.GetCityForCitySO(endCitySO);

        CalculateDistance();

        textUpdateTime = 1f / textUpdateCountPerSecond;
        distanceText.SetText("d: " + distance.ToString("F3"));
    }

    private void Update() {
        textUpdateTimeDelta -= Time.deltaTime;
        if(textUpdateTimeDelta <= 0f) {
            pheromoneText.SetText("f: " + pheromoneLevel.ToString("F4"));
            useCountText.SetText("u: " + useCount);

            textUpdateTimeDelta = textUpdateTime;
        }
    }

    public void SnapWaypoints() {
        waypointParent.GetChild(0).position =
            CityPool.Instance.GetCityForCitySO(startCitySO).transform.position;
  
        int lastIndex = waypointParent.childCount - 1;

        waypointParent.GetChild(lastIndex).position =
            CityPool.Instance.GetCityForCitySO(endCitySO).transform.position;
        
    }
    
    public void CalculateDistance() {
        SnapWaypoints(); // Önce uçlarý sabitle, sonra ölç
        distance = 0f;
        for (int i = 0; i < waypointParent.childCount - 1; i++) {
            distance += Vector3.Distance(waypointParent.GetChild(i).position, waypointParent.GetChild(i + 1).position);
        }
    }

}