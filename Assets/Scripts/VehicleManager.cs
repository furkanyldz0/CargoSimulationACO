using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance { get; private set; }

    [SerializeField] private GameObject vehiclePrefab;

    [Header("Araç özellikleri")]
    [SerializeField] private int vehicleSpeed = 80;
    [SerializeField] private float spawnTime = .2f;
    [SerializeField] private int vehicleSpawnCount = 5000;

    private int currentVehicleCount = 0;
    private bool isMaximumVehicleCountReached;

    private List<Vehicle> currentAllVehicles = new List<Vehicle>();
    private float spawnTimeDelta;
    

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("Sahnede birden fazla VehicleSpawner var!");
        }
        Instance = this;
    }


    private void Update() {
        if (!LevelManager.Instance.IsSimulationInitiated || isMaximumVehicleCountReached)
            return; //simülasyon levelmanager'da baþlatýlmadýðý sürece buranýn update'i çalýþmayacak

        if(spawnTimeDelta > 0) {
            spawnTimeDelta -= Time.deltaTime * LevelManager.TimeScale;
        }

        if(spawnTimeDelta <= 0) {
            if(currentVehicleCount >= vehicleSpawnCount) {
                Debug.Log("Maksimum araç limitine ulaþýldý: " + vehicleSpawnCount
                    + ", spawner devre dýþý býrakýlýyor...");
                isMaximumVehicleCountReached = true;
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

    public void SendAllVehiclesToHome() {
        currentAllVehicles = GetAllVehiclesInScene();
        foreach (Vehicle vehicle in currentAllVehicles) {
            vehicle.TravelHome(isForceReturn: true);
        }
    }

    //Araç üretim þartlarýný sýfýrlar
    public void ResetCurrentVehicleCount() {
        currentVehicleCount = 0;
        isMaximumVehicleCountReached = false;
    }

    public List<Vehicle> GetAllVehiclesInScene() {
        currentAllVehicles = FindObjectsByType<Vehicle>(FindObjectsSortMode.None).ToList();
        return currentAllVehicles;
    }

    public int GetVehicleSpeed() {
        return vehicleSpeed;
    }

    public void SetVehicleSpeed(int vehicleSpeed) {
        this.vehicleSpeed = vehicleSpeed;
        Vehicle.SetSpeed(vehicleSpeed);
    }

    public float GetSpawnTime() {
        return spawnTime;
    }

    public void SetSpawnTime(float spawnTime) {
        this.spawnTime = spawnTime;
        spawnTimeDelta = spawnTime; //sayaç bunun üzerinden hesaplandýðýndan spawntime güncellendiðinde gecikme oluyor
    }

    public int GetVehicleSpawnCount() {
        return vehicleSpawnCount;
    }

    public void SetVehicleSpawnCount(int vehicleSpawnCount) {
        this.vehicleSpawnCount = vehicleSpawnCount;
    }


}
