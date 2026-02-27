using UnityEngine;

public class City : MonoBehaviour
{
    [SerializeField] private CitySO citySO;



    public CitySO GetCitySO() {
        return citySO;
    }
}
