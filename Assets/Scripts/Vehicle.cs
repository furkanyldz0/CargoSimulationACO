using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private State state;

    private float moveSpeed;
    private float rotateSpeed = 20f;
    private Vector3 targetRotation;

    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = -1;

    // Sýnýfýn üst kýsmýna ekle
    private List<Road> traveledRoads = new List<Road>(); //feromon eklenmesi için
    private List<CitySO> visitedCities = new List<CitySO>(); //önceki þehirlere tekrar gitmemesi için

    [SerializeField] private CitySO currentCity;
    private CitySO homeCity, nextCity;

    public enum State {
        Idle,
        Traveling,
        Returning
    }

    private void Start() {
        moveSpeed = VehicleManager.Instance.GetVehicleSpeed();
        homeCity = currentCity;
        state = State.Traveling;

        if (!visitedCities.Contains(currentCity)) visitedCities.Add(currentCity);
        TravelNextCity();
    }

    private void Update() {
        //Time.timeScale = 3; simülasyonu hýzlandýrýyor ama hýzlandýrmayý böyle yapmayacaðým

        switch (state) {
            case State.Idle:
                //Debug.Log("Araç boþta...");
                break;

            case State.Traveling:
                if (currentWaypointIndex < waypoints.Count - 1) {
                    transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex + 1].position,
                        Time.deltaTime * moveSpeed);

                    targetRotation = waypoints[currentWaypointIndex + 1].position - transform.position;
                    if (targetRotation != Vector3.zero) {
                        transform.forward = Vector3.Slerp(transform.forward, targetRotation, Time.deltaTime * rotateSpeed);
                    }

                    if (transform.position == waypoints[currentWaypointIndex + 1].position) {
                        currentWaypointIndex++;

                        if (currentWaypointIndex == waypoints.Count - 1) {
                            //mevcut waypointparentin tüm waypointleri ziyaret edildi
                            currentCity = nextCity;
                            if(currentCity == GraphManager.Instance.targetCity) {
                                //hedef þehre varýldý
                                DepositPheromones();

                                TravelHome();
                                state = State.Returning; //djikstra ile eve dönecek
                                transform.localScale = Vector3.one * 0.5f; 
                            }
                            else {
                                TravelNextCity();
                            }                       
                        }  
                    }
                }
                break;

            case State.Returning:
                if (currentWaypointIndex < waypoints.Count - 1) {
                    transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex + 1].position,
                        Time.deltaTime * moveSpeed);

                    targetRotation = waypoints[currentWaypointIndex + 1].position - transform.position;
                    if (targetRotation != Vector3.zero) {
                        transform.forward = Vector3.Slerp(transform.forward, targetRotation, Time.deltaTime * rotateSpeed);
                    }

                    if (transform.position == waypoints[currentWaypointIndex + 1].position) {
                        currentWaypointIndex++;

                        if (currentWaypointIndex == waypoints.Count - 1) {
                            //araç eve vardý
                            Destroy(gameObject);
                        }
                    }
                }
                break;
        }
    }

    private void TravelNextCity() {
        // BURASI KRÝTÝK: Listeyi metoda parametre olarak gönderiyoruz
        nextCity = ACOManager.Instance.ChooseNextCity(currentCity, visitedCities);

        if (nextCity != null) {
            Road road = GraphManager.Instance.GetRoadBetween(currentCity, nextCity);

            if (!traveledRoads.Contains(road)) traveledRoads.Add(road);

            if (!visitedCities.Contains(nextCity)) visitedCities.Add(nextCity);

            road.useCount++;
            UpdateWaypoints(road.waypointParent);
            currentWaypointIndex = -1;
        }
    }

    private void TravelHome() {
        waypoints.Clear();
        List<CitySO> path = Djikstra.FindShortestPath(currentCity, homeCity);

        CitySO stepStart = currentCity; // Baþlangýcýmýz þu anki hedef þehir

        foreach (CitySO stepEnd in path) {
            Road road = GraphManager.Instance.GetRoadBetween(stepStart, stepEnd);
            if (road != null) {
                // O yola ait tüm waypointleri sýrayla ana listeye ekle
                foreach (Transform child in road.waypointParent) {
                    waypoints.Add(child);
                }
            }
            stepStart = stepEnd; // Bir sonraki yolun baþlangýcý için þehri kaydýr
        }

        currentWaypointIndex = -1;
    }

    private void DepositPheromones() {
        ACOManager.Instance.AddPheromones(traveledRoads);

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
