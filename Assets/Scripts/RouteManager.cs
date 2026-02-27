using System.Collections.Generic;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    [SerializeField] private Transform[] waypointParents;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Vehicle vehicle;
    
    private Transform currentWaypointParent;
    private List<Transform> waypoints = new List<Transform>();
    private int currentIndex = 0;
    private int currentWaypointParentIndex = 0;

    private void Start() {
        currentWaypointParent = waypointParents[currentWaypointParentIndex];
        UpdateWaypoints(currentWaypointParent);
        
        vehicle.transform.position = waypoints[currentIndex].position;
        Debug.Log("index: " + currentIndex);
    }

    
    private void Update() {  //rota tamamlanýnca if koþulunun içine girmeyi býrakýyor, currentindex sýfýrlanmýyor çünkü
        if (currentIndex < waypoints.Count - 1) { //-1 koymazsak currentindex+1 eriþirken sýkýntý çýkýyor
            vehicle.transform.position = Vector3.MoveTowards(vehicle.transform.position, waypoints[currentIndex + 1].position,
                Time.deltaTime * moveSpeed);

            if (vehicle.transform.position == waypoints[currentIndex + 1].position) {
                currentIndex++;
                Debug.Log("index: " + currentIndex);
                if (currentIndex == waypoints.Count - 1) {
                    Debug.Log("rota tamamlandý");
                    if(currentWaypointParentIndex >= waypointParents.Length - 1) {
                        Debug.Log("tüm rotalar gezildi!, son currentwaypointParentIndex: " + currentWaypointParentIndex);
                    }
                    else{
                        Debug.Log(currentWaypointParent + "rota güncelleniyor...");
                        currentWaypointParentIndex++;
                        UpdateCourse(currentWaypointParentIndex);
                    }
                }
            }
        }
    }

    private void UpdateWaypoints(Transform waypointParent) {
        waypoints.Clear();
        foreach(Transform child in waypointParent) {
            waypoints.Add(child);
        }
        //for (int i = 0; i < waypointParent.childCount; i++) {
        //    waypoints.Add(waypointParent.GetChild(i));
        //}
    }

    private void UpdateCourse(int waypointParentIndex) {
        currentIndex = 0;
        currentWaypointParent = waypointParents[waypointParentIndex];
        UpdateWaypoints(currentWaypointParent);

        Debug.Log("Rota güncellendi");
    }
}
