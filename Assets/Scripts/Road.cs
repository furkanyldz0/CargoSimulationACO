using TMPro;
using UnityEngine;

public class Road : MonoBehaviour {

    public CitySO startCitySO;
    public CitySO endCitySO;
    public Transform waypointParent;

    public float distance;
    public float pheromoneLevel;
    public int useCount = 0;


    private void Start() {
        CalculateDistance();
    }

    public void SnapWaypoints() {
        waypointParent.GetChild(0).position =
            CityPool.Instance.GetCityForCitySO(startCitySO).transform.position;
  
        int lastIndex = waypointParent.childCount - 1;

        waypointParent.GetChild(lastIndex).position =
            CityPool.Instance.GetCityForCitySO(endCitySO).transform.position;
    }

    //Üstündeki waypointler ile yolun mesafesini hesaplar
    public void CalculateDistance() {
        SnapWaypoints();
        distance = 0f;
        for (int i = 0; i < waypointParent.childCount - 1; i++) {
            distance += Vector3.Distance(waypointParent.GetChild(i).position, waypointParent.GetChild(i + 1).position);
        }
    }

}