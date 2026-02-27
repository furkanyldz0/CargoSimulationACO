using UnityEngine;

public class Vehicle : MonoBehaviour
{
    //[SerializeField] private Transform[] waypoints;
    //[SerializeField] private float moveSpeed = 10f;
    //private int currentIndex = 0;
    

    //private void Start()
    //{
    //    transform.position = waypoints[currentIndex].position;
    //    Debug.Log("index: " + currentIndex);
    //}

    //// Update is called once per frame
    //private void Update()
    //{
    //    if(currentIndex < waypoints.Length - 1) { //-1 koymazsak currentindex+1 eriþirken sýkýntý çýkýyor
    //        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentIndex + 1].position,
    //            Time.deltaTime * moveSpeed);

    //        if(transform.position == waypoints[currentIndex + 1].position) {
    //            currentIndex++;
    //            Debug.Log("index: " + currentIndex);
    //            if (currentIndex == waypoints.Length - 1) {
    //                Debug.Log("rota tamamlandý");
    //                gameObject.SetActive(false);
    //            }
    //        }
    //    }
    //}
}
