using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CitySO : ScriptableObject
{
    public City city;
    public List<NeighborCity> neighbors = new List<NeighborCity>();
    
    public enum City {
        Base,
        A,
        B,
        C,
        D
    }

    [System.Serializable]
    public struct NeighborCity {
        public CitySO neighborCitySO;
        public float distance;

    }
}
