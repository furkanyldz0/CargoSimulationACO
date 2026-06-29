using System.Collections.Generic;
using UnityEngine;

public class MenuVehicleManager : MonoBehaviour {
    [SerializeField] private List<MenuVehicle> vehiclePool = new List<MenuVehicle>();

    [SerializeField] private Transform waypointParentAB; // A→B yolu (paket açık)
    [SerializeField] private Transform waypointParentBA; // B→A yolu (paket kapalı)

    [SerializeField] private float vehicleSpeed = 40f;
    [SerializeField] private float minSpawnInterval = 1.5f;
    [SerializeField] private float maxSpawnInterval = 4f;

    private float spawnTimer;
    private float nextSpawnDelay;

    private void Start() {
        foreach (MenuVehicle vehicle in vehiclePool)
            vehicle.gameObject.SetActive(false);

        nextSpawnDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update() {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= nextSpawnDelay) {
            spawnTimer = 0f;
            nextSpawnDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
            SpawnVehicle();
        }
    }

    private void SpawnVehicle() {
        foreach (MenuVehicle vehicle in vehiclePool) {
            if (!vehicle.gameObject.activeSelf) {
                bool goAB = Random.value < 0.5f; // rastgele yön
                if (goAB)
                    vehicle.Travel(waypointParentAB, vehicleSpeed, true);  // A→B, paket açık
                else
                    vehicle.Travel(waypointParentBA, vehicleSpeed, false); // B→A, paket kapalı
                return;
            }
        }
    }
}