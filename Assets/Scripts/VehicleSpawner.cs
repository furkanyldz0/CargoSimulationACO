using System;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private int maxVehicleCount;
    [SerializeField] private float spawnTime = 1f;

    private int currentVehicleCount = 0;
    private float spawnTimeDelta;

    private void Update() {
        if(spawnTimeDelta > 0) {
            spawnTimeDelta -= Time.deltaTime;
        }

        if(spawnTimeDelta <= 0) {
            if(currentVehicleCount > maxVehicleCount) {
                Debug.Log("Maksimum araç limitine ulaþýldý: " + maxVehicleCount
                    + ", spawner devre dýþý býrakýlýyor...");
                enabled = false;
            }
            else {
                GenerateVehicle();
                spawnTimeDelta = spawnTime;
            }
        }
    }

    private void GenerateVehicle() {
        currentVehicleCount++;
        Instantiate(vehiclePrefab, transform.position, Quaternion.identity);
    }
}
