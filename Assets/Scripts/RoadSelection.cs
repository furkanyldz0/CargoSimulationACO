using UnityEngine;

public class RoadSelection : MonoBehaviour
{
    public static RoadSelection Instance {get; private set;}

    private int estimateCount = 10;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Sahnede birden fazla RoadSelection var! " + this);
        }
        Instance = this;
    }

    public CitySO PickRoad(CitySO currentCity) {
        CitySO targetCity = null;
        int count = 0; 
        if(targetCity != currentCity) {
            if(count < estimateCount) {
                int rand = Random.Range(0, currentCity.neighbors.Count);
                targetCity = currentCity.neighbors[rand].neighborCitySO;
                count++;
            }
            else {
                Debug.Log("komþu þehir bulunamadý!");
                return null;
            }
        }
        return targetCity;
    }
}
