using UnityEngine;

[System.Serializable]
public class Road : MonoBehaviour {
    public CitySO startCity; // Baþlangýç (Örn: B)
    public CitySO endCity;   // Hedef (Örn: D)
    public Transform waypointParent; // Bu iki þehir arasýndaki fiziksel yol
}

