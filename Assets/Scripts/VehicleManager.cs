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
    [SerializeField] private float timeScale = 1;

    private List<Vehicle> currentAllVehicles = new List<Vehicle>();
    private int currentVehicleCount = 0;
    private float spawnTimeDelta;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("Sahnede birden fazla VehicleSpawner var!");
        }
        Instance = this;
    }

    private void Start() {
        SetTimeScale(1f);
    }

    private void Update() {
        if (!LevelManager.Instance.IsSimulationInitiated)
            return; //simülasyon levelmanager'da baþlatýlmadýðý sürece buranýn update'i çalýþmayacak

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

    public void SendAllVehiclesToHome() {
        currentAllVehicles = GetAllVehiclesInScene(); //performans açýsýndan sýkýntý yaratýr mý acaba
        foreach (Vehicle vehicle in currentAllVehicles) {
            vehicle.TravelHome();
        }
    }

    public void ResetCurrentVehicleCount() {
        currentVehicleCount = 0;
    }

    private void ChangeAllVehicleSpeeds(int vehicleSpeed) {
        currentAllVehicles = GetAllVehiclesInScene(); //performans açýsýndan sýkýntý yaratýr mý acaba
        foreach (Vehicle vehicle in currentAllVehicles) {
            vehicle.SetSpeed(vehicleSpeed);
        }
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
        ChangeAllVehicleSpeeds(vehicleSpeed); //sahnedeki mevcut araçlarýn da hýzlarýný güncellememiz gerekiyor
    }
    
    public void SetSpawnTime(float spawnTime) {
        this.spawnTime = spawnTime;
        spawnTimeDelta = spawnTime; //sayaç bunun üzerinden hesaplandýðýndan spawntime güncellendiðinde gecikme oluyor
    }

    public float GetSpawnTime() {
        return spawnTime;
    }

    public void SetTimeScale(float timeScale) {
        this.timeScale = timeScale;
        Time.timeScale = timeScale;
    }

    public float GetTimeScale() {
        return timeScale;
    }

    public int GetVehicleSpawnCount() {
        return vehicleSpawnCount;
    }

    public void SetVehicleSpawnCount(int vehicleSpawnCount) {
        this.vehicleSpawnCount = vehicleSpawnCount;
    }

    

}
