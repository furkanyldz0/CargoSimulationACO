using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private State state;

    [SerializeField] private float moveSpeed = 20f;
    private Vector3 targetRotation;
    private float rotateSpeed = 20f;

    private Transform currentWaypointParent;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = -1;

    // Sýnýfýn üst kýsmýna ekle
    private List<Road> traveledRoads = new List<Road>();

    [SerializeField] private CitySO currentCity;
    private CitySO nextCity;

    public enum State {
        idle,
        traveling
    }

    private void Start() {
        state = State.traveling;
        TravelNextCity();
    }

    private void Update() {
        switch (state) {
            case State.idle:
                //Debug.Log("Araç boþta...");
                break;

            case State.traveling:
                if (currentWaypointIndex < waypoints.Count - 1) { //-1 koymazsak currentindex+1 eriþirken sýkýntý çýkýyor
                    transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex + 1].position,
                        Time.deltaTime * moveSpeed);

                    targetRotation = waypoints[currentWaypointIndex + 1].position - transform.position;
                    transform.forward = Vector3.Slerp(transform.forward, targetRotation, Time.deltaTime * rotateSpeed);

                    if (transform.position == waypoints[currentWaypointIndex + 1].position) {
                        currentWaypointIndex++;
                        //Debug.Log(currentWaypointParent + ", index: " + currentWaypointIndex);

                        if (currentWaypointIndex == waypoints.Count - 1) {
                            //mevcut waypointparentin tüm waypointleri ziyaret edildi
                            //Debug.Log(currentWaypointParent + " rota tamamlandý");
                            currentCity = nextCity;
                            if(currentCity == GraphManager.Instance.targetCity) {
                                //Debug.Log("Hedef þehre varýldý!");
                                DepositPheromones();
                                state = State.idle; //bundan sonra djikstra ile eve gidecek, ama þimdilik destroy edelim
                                Destroy(gameObject);

                            }
                            else {
                                TravelNextCity();
                            }                       
                        }  
                    }
                }
                break;
        }
    }

    private void TravelNextCity() {
        // BURASI KRÝTÝK: Listeyi metoda parametre olarak gönderiyoruz
        nextCity = ACOSelection.ChooseNextCity(currentCity, traveledRoads);

        if (nextCity != null) { //burada kullaným adedini eklemek için güncelleme yapabilirim (road scripti için)
            Road road = GraphManager.Instance.GetRoadBetween(currentCity, nextCity);

            // Bu yeni yolu hafýzaya ekle ki bir sonraki seçimde buraya geri dönmesin
            if (!traveledRoads.Contains(road)) {
                traveledRoads.Add(road);
            }

            currentWaypointParent = road.waypointParent;
            UpdateWaypoints(currentWaypointParent);
            currentWaypointIndex = -1;
        }
    }

    private void DepositPheromones() {
        float totalDistance = 0;
        // 1. Toplam mesafeyi hesapla
        foreach (Road r in traveledRoads) {
            totalDistance += r.distance;
        }

        // 2. Yol ne kadar kýsaysa o kadar çok feromon býrak (Q / L formülü)
        // Q sabit bir deðerdir (örn: 100)
        float pheromoneToAdd = 100f / totalDistance;

        foreach (Road r in traveledRoads) {
            r.pheromoneLevel += pheromoneToAdd;
        }

        // Hafýzayý temizle (bir sonraki görev için)
        traveledRoads.Clear();
    }

    private void UpdateWaypoints(Transform waypointParent) {
        waypoints.Clear();
        foreach (Transform child in waypointParent) {
            waypoints.Add(child);
        }
    }

    //private void TravelNextCity() {
    //    //nextCity = RoadSelection.Instance.PickRoad(currentCity);

    //    currentWaypointParent = GraphManager.Instance.GetWaypointParentBetween(currentCity, nextCity);
    //    //waypoints = GetChildrenFromParent(currentWaypointParent);
    //    UpdateWaypoints(currentWaypointParent);
    //    currentWaypointIndex = -1;
    //}

    //private List<Transform> GetChildrenFromParent(Transform parent) {
    //    List<Transform> childrenList = new List<Transform>();
    //    foreach (Transform child in parent) {
    //        childrenList.Add(child);
    //    }

    //    return childrenList;
    //}

}
