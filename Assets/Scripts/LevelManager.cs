using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour //levelmanager yerine baþka isim yazabilirim
{
    public static LevelManager Instance { get; private set; }

    public bool IsSimulationInitiated { get; private set; }
    public bool IsSelectingCity { get; private set; }

    private void Awake() {
        if (Instance != null)
            Debug.LogError("Sahnede birden fazla LevelManager var!");

        Instance = this;
    }

    private void Start() {
        ResetLevel();
    }

    public void EnterSelectingCityProcess() {
        Instance.IsSelectingCity = true;
    }

    public void InitiateSimulation(City targetCity) {
        SetTargetCity(targetCity);

        ACOManager.Instance.SetStartPheromone(); //bunu deðerler yapacaðýz, paneli daha eklemediðim için acomanager'in kendi deðerini kullanýyor

        IsSimulationInitiated = true;
        IsSelectingCity = false;
    }

    public void ResetLevel() { //en baþtaki durum
        IsSimulationInitiated = false;
        IsSelectingCity = false; //gerek yok aslýnda ama dursun

        GraphManager.Instance.TargetCity = null;
        VehicleManager.Instance.ResetCurrentVehicleCount();
        VehicleManager.Instance.SendAllVehiclesToHome();
        ACOManager.Instance.ResetStartPheromone();
        ResetAllPheromoneTrails();
        //pheromonevisualizer için de yazmaya gerek yok feromonlarý sýfýrlayýnca gidecek
    }

    public void SetTargetCity(City city) {
        GraphManager.Instance.TargetCity = city.GetCitySO();
    }

    private void ResetAllPheromoneTrails() {
        List<Road> roads = GraphManager.Instance.GetAllRoads();
        foreach (Road road in roads) {
            road.GetComponent<PheromoneVisualizer>().ResetPheromoneTrail();
        }
    }
}
