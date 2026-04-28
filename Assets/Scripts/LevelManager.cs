using UnityEngine;

public class LevelManager : MonoBehaviour //levelmanager yerine baþka isim yazabilirim
{
    public static LevelManager Instance { get; private set; }

    public bool IsSimulationInitiated { get; private set; }

    private void Awake() {
        if (Instance != null)
            Debug.LogError("Sahnede birden fazla LevelManager var!");

        Instance = this;
    }

    private void Start() {
        IsSimulationInitiated = false;
    }

}
