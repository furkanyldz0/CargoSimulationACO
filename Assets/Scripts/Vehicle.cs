using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private State state;

    private static int MoveSpeed;

    private float rotateSpeed = 100f;
    private Vector3 targetRotation;

    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = -1;

    // Sýnýfýn üst kýsmýna ekle
    private List<Road> traveledRoads = new List<Road>(); //feromon eklenmesi için
    private List<CitySO> visitedCities = new List<CitySO>(); //önceki þehirlere tekrar gitmemesi için

    private CitySO homeCity, currentCity, nextCity;

    [SerializeField] private GameObject cargoPackageVisual;

    public enum State {
        Idle,
        Traveling,
        Returning
    }

    private void Start() {
        MoveSpeed = VehicleManager.Instance.GetVehicleSpeed();
        homeCity = GraphManager.Instance.StartCitySO;
        currentCity = homeCity;

        state = State.Traveling;

        if (!visitedCities.Contains(currentCity)) visitedCities.Add(currentCity);
        TravelNextCity();
    }

    private void Update() {

        switch (state) {
            case State.Idle:
                //Debug.Log("Araç boþta...");
                break;

            case State.Traveling:
                if (currentWaypointIndex < waypoints.Count - 1) {
                    transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex + 1].position,
                        Time.deltaTime * MoveSpeed * LevelManager.TimeScale);

                    targetRotation = waypoints[currentWaypointIndex + 1].position - transform.position;
                    if (targetRotation != Vector3.zero) {
                        transform.forward = Vector3.Slerp(transform.forward, targetRotation, Time.deltaTime * rotateSpeed * LevelManager.TimeScale);
                    }

                    if (transform.position == waypoints[currentWaypointIndex + 1].position) {
                        currentWaypointIndex++;

                        if (currentWaypointIndex == waypoints.Count - 1) {
                            //mevcut waypointparentin tüm waypointleri ziyaret edildi
                            currentCity = nextCity;
                            if(currentCity == GraphManager.Instance.TargetCitySO) {
                                //hedef þehre varýldý
                                DepositPheromones();

                                TravelHome();
                                //state = State.Returning; //djikstra ile eve dönecek
                                cargoPackageVisual.SetActive(false);
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
                        Time.deltaTime * MoveSpeed * LevelManager.TimeScale);

                    targetRotation = waypoints[currentWaypointIndex + 1].position - transform.position;
                    if (targetRotation != Vector3.zero) {
                        transform.forward = Vector3.Slerp(transform.forward, targetRotation, Time.deltaTime * rotateSpeed * LevelManager.TimeScale);
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
        //Debug.Log(nextCity);

        if (nextCity != null) {
            Road road = GraphManager.Instance.GetRoadBetween(currentCity, nextCity);

            if (!traveledRoads.Contains(road)) traveledRoads.Add(road);

            if (!visitedCities.Contains(nextCity)) visitedCities.Add(nextCity);

            road.useCount++;
            UpdateWaypoints(road.waypointParent);
            currentWaypointIndex = -1;
        }
    }

    public void TravelHome() {
        if(state != State.Returning) { //bunu koymazsam dýþarýdan çaðýrdýðýmda uçarak targetcity'e dönüyorlar
            waypoints.Clear();

            if (currentCity == homeCity) { //baþlangýçtan sonra bir þehri ziyaret edememiþse
                Road road = GraphManager.Instance.GetRoadBetween(nextCity, homeCity);
                foreach (Transform child in road.waypointParent)
                    waypoints.Add(child);
            }
            else {
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
            }
            currentWaypointIndex = -1;
            state = State.Returning;
        }
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

    public static void SetSpeed(int moveSpeed) {
        MoveSpeed = moveSpeed;
    }

    public void SetState(State state) {
        this.state = state;
    }

}
