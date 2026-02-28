//using System.Collections.Generic;
//using UnityEngine;

//public class RouteManager : MonoBehaviour
//{
//    [SerializeField] private Transform[] waypointParents;
//    [SerializeField] private float moveSpeed = 10f;
//    [SerializeField] private Vehicle vehicle;
    
//    private Transform currentWaypointParent;
//    private List<Transform> waypoints = new List<Transform>();
//    private int currentWaypointIndex = 0;
//    private int currentWaypointParentIndex;

//    private void Start() {
//        currentWaypointParentIndex = 0;
//        currentWaypointParent = waypointParents[currentWaypointParentIndex];
//        UpdateCourse(currentWaypointParent);
        
//        vehicle.transform.position = waypoints[currentWaypointIndex].position;
//        Debug.Log(currentWaypointParent + ", index: " + currentWaypointIndex);
//    }

    
//    private void Update() {  //rota tamamlanýnca if koþulunun içine girmeyi býrakýyor, currentindex sýfýrlanmýyor çünkü
//        if (currentWaypointIndex < waypoints.Count - 1) { //-1 koymazsak currentindex+1 eriþirken sýkýntý çýkýyor
//            vehicle.transform.position = Vector3.MoveTowards(vehicle.transform.position, waypoints[currentWaypointIndex + 1].position,
//                Time.deltaTime * moveSpeed);

//            if (vehicle.transform.position == waypoints[currentWaypointIndex + 1].position) {
//                currentWaypointIndex++;
//                Debug.Log(currentWaypointParent + ", index: " + currentWaypointIndex);

//                if (currentWaypointIndex == waypoints.Count - 1) {
//                    //mevcut waypointparentin tüm waypointleri ziyaret edildi
//                    Debug.Log(currentWaypointParent + " rota tamamlandý");

//                    if(currentWaypointParentIndex >= waypointParents.Length - 1) {
//                        //verilen tüm waypointler ziyaret edildi
//                        Debug.Log("tüm rotalar ziyaret edildi!, son currentwaypointParentIndex: " + currentWaypointParentIndex);
//                    }
//                    else{
//                        //waypoint güncellemek adýna waypointparent update ediliyor
//                        currentWaypointParentIndex++;
//                        currentWaypointParent = waypointParents[currentWaypointParentIndex];
//                        UpdateCourse(currentWaypointParent);
//                    }
//                }
//            }
//        }
//    }

//    private void UpdateCourse(Transform waypointParent) {
//        currentWaypointIndex = 0;
//        waypoints.Clear(); //gerek yok silinebilir
//        waypoints = GetChildrenFromParent(currentWaypointParent);

//        Debug.Log("yeni rota: " + currentWaypointParent);
//    }
    
//    private List<Transform> GetChildrenFromParent(Transform parent) {
//        List<Transform> childrenList = new List<Transform>();
//        foreach (Transform child in parent) {
//            childrenList.Add(child);
//        }

//        return childrenList;
//    }

//    //private void GetWaypointsFromParent(Transform waypointParent) {
//    //    waypoints.Clear();
//    //    foreach (Transform child in waypointParent) {
//    //        waypoints.Add(child);
//    //    }
//    //    //for (int i = 0; i < waypointParent.childCount; i++) {
//    //    //    waypoints.Add(waypointParent.GetChild(i));
//    //    //}
//    //}
//}
