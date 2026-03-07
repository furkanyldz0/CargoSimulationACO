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
                        Debug.Log(currentWaypointParent + ", index: " + currentWaypointIndex);

                        if (currentWaypointIndex == waypoints.Count - 1) {
                            //mevcut waypointparentin tüm waypointleri ziyaret edildi
                            Debug.Log(currentWaypointParent + " rota tamamlandý");
                            currentCity = nextCity;
                            if(currentCity == GraphManager.Instance.targetCity) {
                                Debug.Log("Hedef þehre varýldý!");
                                state = State.idle; //bundan sonra djikstra ile eve gidecek
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
        // ACO algoritmasý çalýþýr ve bize bir "CitySO" döner
        //targetCity = ACO_Logic.PickNextCity(currentCity);
        nextCity = RoadSelection.Instance.PickRoad(currentCity);

        currentWaypointParent = GraphManager.Instance.GetWaypointParentBetween(currentCity, nextCity);
        //waypoints = GetChildrenFromParent(currentWaypointParent);
        UpdateWaypoints(currentWaypointParent);
        currentWaypointIndex = -1;
    }

    private void UpdateWaypoints(Transform waypointParent) {
        waypoints.Clear();
        foreach (Transform child in waypointParent) {
            waypoints.Add(child);
        }
    }

        //private List<Transform> GetChildrenFromParent(Transform parent) {
        //    List<Transform> childrenList = new List<Transform>();
        //    foreach (Transform child in parent) {
        //        childrenList.Add(child);
        //    }

        //    return childrenList;
        //}

    }
