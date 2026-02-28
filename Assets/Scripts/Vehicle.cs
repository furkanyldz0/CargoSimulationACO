using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private State state;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Transform currentWaypointParent;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;

    [SerializeField] private CitySO currentCity;
    [SerializeField] private CitySO targetCity;
    [SerializeField] private CitySO city1;

    public enum State {
        idle,
        traveling
    }

    private void Start() {
        state = State.traveling;
        UpdateCourse(currentWaypointParent);
        transform.position = waypoints[currentWaypointIndex].position;
        Debug.Log(currentWaypointParent + ", index: " + currentWaypointIndex);
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

                    if (transform.position == waypoints[currentWaypointIndex + 1].position) {
                        currentWaypointIndex++;
                        Debug.Log(currentWaypointParent + ", index: " + currentWaypointIndex);

                        if (currentWaypointIndex == waypoints.Count - 1) {
                            //mevcut waypointparentin tüm waypointleri ziyaret edildi
                            Debug.Log(currentWaypointParent + " rota tamamlandý");
                            currentCity = targetCity;
                            targetCity = city1; //Deneme amaçlý
                            DecideNextMove();
                            //state = State.idle;
                        }
                    }
                }
                break;

        }
    }

    public void DecideNextMove() {
        // ACO algoritmasý çalýþýr ve bize bir "CitySO" döner
        //targetCity = ACO_Logic.PickNextCity(currentCity);

        // Ýþte burada GraphManager'a o iki "objeyi" veririz
        currentWaypointParent = GraphManager.Instance.GetWaypointParentBetween(currentCity, targetCity);
        waypoints = GetChildrenFromParent(currentWaypointParent);
        currentWaypointIndex = 0;
        // Yolu takip etmeye baþla...
    }

    private void UpdateCourse(Transform waypointParent) {
        currentWaypointIndex = 0;
        waypoints.Clear(); //gerek yok silinebilir
        waypoints = GetChildrenFromParent(currentWaypointParent);

        Debug.Log("yeni rota: " + currentWaypointParent);
    }

    private List<Transform> GetChildrenFromParent(Transform parent) {
        List<Transform> childrenList = new List<Transform>();
        foreach (Transform child in parent) {
            childrenList.Add(child);
        }

        return childrenList;
    }
}
