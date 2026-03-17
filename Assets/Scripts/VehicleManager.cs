using System;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance { get; private set; }

    [SerializeField] private GameObject vehiclePrefab;

    [Header("Araç özellikleri")]
    [SerializeField] private float vehicleSpeed = 40f;
    [SerializeField] private float spawnTime = 1f;
    [SerializeField] private int vehicleSpawnCount;

    private int currentVehicleCount = 0;
    private float spawnTimeDelta;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("Sahnede birden fazla VehicleSpawner var!");
        }
        Instance = this;
    }

    private void Update() {
        if(spawnTimeDelta > 0) {
            spawnTimeDelta -= Time.deltaTime;
        }

        if(spawnTimeDelta <= 0) {
            if(currentVehicleCount >= vehicleSpawnCount) {
                Debug.Log("Maksimum araç limitine ulaþýldý: " + vehicleSpawnCount
                    + ", spawner devre dýþý býrakýlýyor...");
                enabled = false;
            }
            else {
                GenerateVehicle();
                spawnTimeDelta = spawnTime;
            }
        }
    }

    private void GenerateVehicle() {//sýnýfa liste tanýmlanýp oluþturulan araçlar bu listeye atanabilir
        currentVehicleCount++;
        Instantiate(vehiclePrefab, transform.position, Quaternion.identity);
    }

    public float GetVehicleSpeed() {
        return vehicleSpeed;
    }
}
