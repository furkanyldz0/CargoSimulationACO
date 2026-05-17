using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance { get; private set; }

    [SerializeField] private GameObject vehiclePrefab;

    [Header("Araç özellikleri")]
    [SerializeField] private int vehicleSpeed = 40;
    [SerializeField] private float spawnTime = 1f;
    [SerializeField] private int vehicleSpawnCount;

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

    private void GenerateVehicle() {//sýnýfa liste tanýmlanýp oluþturulan araçlar bu listeye atanabilir
        currentVehicleCount++;
        Instantiate(vehiclePrefab, transform.position, Quaternion.identity);
    }

    public void SendAllVehiclesToHome() {
        currentAllVehicles = GetAllVehiclesInScene(); //performans açýsýndan sýkýntý yaratýr mý acaba
        foreach (Vehicle vehicle in currentAllVehicles) {
            vehicle.TravelHome(true);
        }
    }

    public void ResetVariables() {
        currentVehicleCount = 0;
        isMaximumVehicleCountReached = false;
    }

    //private void ChangeAllVehicleSpeeds(int vehicleSpeed) {
    //    currentAllVehicles = GetAllVehiclesInScene(); //performans açýsýndan sýkýntý yaratýr mý acaba
    //    foreach (Vehicle vehicle in currentAllVehicles) {
    //        vehicle.SetSpeed(vehicleSpeed);
    //    }
    //} tüm araçlar ayný hýzla gideceði için speedi static yapmaya karar verdim, ayný hýzla gitsin istersem güncellenebilir

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

        //ChangeAllVehicleSpeeds(vehicleSpeed); //sahnedeki mevcut araçlarýn da hýzlarýný güncellememiz gerekiyor
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
