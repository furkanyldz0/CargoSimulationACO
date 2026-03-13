using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private State state;

    [SerializeField] private float moveSpeed = 20f; //hareket hýzýný ileride vehiclespawner'dan ayarlayabiliriz
    private float rotateSpeed = 20f;
    private Vector3 targetRotation;

    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = -1;

    // Sýnýfýn üst kýsmýna ekle
    private List<Road> traveledRoads = new List<Road>(); //feromon eklenmesi için
    private List<CitySO> visitedCities = new List<CitySO>(); //önceki þehirlere tekrar gitmemesi için

    [SerializeField] private CitySO currentCity;
    private CitySO nextCity;

    public enum State {
        idle,
        traveling
    }

    private void Start() {
        state = State.traveling;

        if (!visitedCities.Contains(currentCity)) visitedCities.Add(currentCity);
        TravelNextCity();
    }

    private void Update() {
        switch (state) {
            case State.idle:
                //Debug.Log("Araç boþta...");
                break;

            case State.traveling:
                if (currentWaypointIndex < waypoints.Count - 1) {
                    transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex + 1].position,
                        Time.deltaTime * moveSpeed);

                    targetRotation = waypoints[currentWaypointIndex + 1].position - transform.position;
                    transform.forward = Vector3.Slerp(transform.forward, targetRotation, Time.deltaTime * rotateSpeed);

                    if (transform.position == waypoints[currentWaypointIndex + 1].position) {
                        currentWaypointIndex++;

                        if (currentWaypointIndex == waypoints.Count - 1) {
                            //mevcut waypointparentin tüm waypointleri ziyaret edildi
                            currentCity = nextCity;
                            if(currentCity == GraphManager.Instance.targetCity) {
                                //hedef þehre varýldý
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
        nextCity = ACOSelection.ChooseNextCity(currentCity, visitedCities);

        if (nextCity != null) {
            Road road = GraphManager.Instance.GetRoadBetween(currentCity, nextCity);

            if (!traveledRoads.Contains(road)) traveledRoads.Add(road);

            if (!visitedCities.Contains(nextCity)) visitedCities.Add(nextCity);

            road.useCount++;
            UpdateWaypoints(road.waypointParent);
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
        visitedCities.Clear();
    }

    private void UpdateWaypoints(Transform waypointParent) {
        waypoints.Clear();
        foreach (Transform child in waypointParent) {
            waypoints.Add(child);
        }
    }


}
