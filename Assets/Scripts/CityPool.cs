using UnityEngine;

public class CityPool : MonoBehaviour
{
    public static CityPool Instance {get; private set;}

    [SerializeField] private City[] cities;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("Sahnede birden fazla citypool var!");
        }
        Instance = this;
    }

    public City GetCityForCitySO(CitySO citySO) {
        foreach(City c in cities) {
            if (c.GetCitySO() == citySO) {
                return c;
            }
        }
        return null;
    }

    public City[] GetAllCities() {
        return cities;
    }
}
