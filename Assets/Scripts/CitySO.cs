using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CitySO : ScriptableObject
{
    public List<NeighborCity> neighbors = new List<NeighborCity>();

    [System.Serializable]
    public struct NeighborCity {
        public CitySO neighborCitySO;
        public float distance;

    }
}
