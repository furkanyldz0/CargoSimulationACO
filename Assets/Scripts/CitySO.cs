using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CitySO : ScriptableObject
{
    public string cityName;
    public List<CitySO> neighbors = new List<CitySO>();
}
