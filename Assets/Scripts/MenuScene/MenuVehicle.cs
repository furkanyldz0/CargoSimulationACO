using System.Collections.Generic;
using UnityEngine;

public class MenuVehicle : MonoBehaviour {
    [SerializeField] private GameObject cargoPackageVisual;
    [SerializeField] private float rotateSpeed = 10f;

    private float moveSpeed;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex;

    public void Travel(Transform waypointParent, float speed, bool packageVisible) {
        moveSpeed = speed;

        waypoints.Clear();
        foreach (Transform child in waypointParent)
            waypoints.Add(child);

        waypoints.Reverse();

        transform.position = waypoints[0].position;
        transform.forward = (waypoints[1].position - waypoints[0].position).normalized;
        currentWaypointIndex = 0;

        cargoPackageVisual.SetActive(packageVisible);
        gameObject.SetActive(true);
    }

    private void Update() {
        if (currentWaypointIndex >= waypoints.Count - 1) return;

        Transform target = waypoints[currentWaypointIndex + 1];
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * moveSpeed);

        Vector3 dir = target.position - transform.position;
        if (dir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotateSpeed);

        if (transform.position == target.position) {
            currentWaypointIndex++;
            if (currentWaypointIndex == waypoints.Count - 1)
                gameObject.SetActive(false); // vardı → havuza dön
        }
    }
}