using UnityEngine;

[SelectionBase]
public class Road : MonoBehaviour {
    public CitySO startCity;
    public CitySO endCity;
    public Transform waypointParent;

    [Header("Sabitleme Ayarlarý")]
    public Transform startCityTransform; // Sahnedeki baþlangýç objesi
    public Transform endCityTransform;   // Sahnedeki bitiþ objesi

    public float distance;
    public float pheromoneLevel = 1f;

    // Unity Editor'de herhangi bir deðer deðiþtiðinde otomatik çalýþýr
    private void OnValidate() {
        if (waypointParent != null && waypointParent.childCount >= 2) {
            SnapWaypoints();
        }
    }

    private void Start() {
        CalculateDistance();
        Debug.Log(this + " mesafe: " + distance);
    }

    public void SnapWaypoints() {
        if (startCityTransform != null) {
            // Ýlk waypoint'i baþlangýç þehrine sabitle
            waypointParent.GetChild(0).position = startCityTransform.position;
        }

        if (endCityTransform != null) {
            // Son waypoint'i hedef þehre sabitle
            int lastIndex = waypointParent.childCount - 1;
            waypointParent.GetChild(lastIndex).position = endCityTransform.position;
        }
    }

    //[ContextMenu("Mesafe Hesapla")]
    public void CalculateDistance() {
        SnapWaypoints(); // Önce uçlarý sabitle, sonra ölç
        distance = 0f;
        for (int i = 0; i < waypointParent.childCount - 1; i++) {
            distance += Vector3.Distance(waypointParent.GetChild(i).position, waypointParent.GetChild(i + 1).position);
        }
        distance = (int) distance * 10;
    }
}