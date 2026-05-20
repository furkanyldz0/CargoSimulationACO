using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour //levelmanager yerine başka isim yazabilirim
{
    public static LevelManager Instance { get; private set; }

    public event EventHandler OnSimulationStarted;
    public event EventHandler OnSimulationStopped;

    public static float TimeScale { get; private set; } = 1f;

    public bool IsSimulationInitiated { get; private set; }
    public bool IsSelectingCity { get; private set; }
    public bool IsSelectingStartCity { get; private set; }


    private void Awake() {
        if (Instance != null)
            Debug.LogError("Sahnede birden fazla LevelManager var!");

        Instance = this;
    }

    private void Start() {
        ResetLevel();
    }

    public void EnterSelectingCityProcess() {
        IsSelectingCity = true;
        IsSelectingStartCity = true;
    }

    public void ExitSelectingStartCityProcess(City startCity) {
        IsSelectingStartCity = false;

        DesignateStartCity(startCity);
        CitySelection.Instance.SetSelectedCity(null);
    }

    public void InitiateSimulation(City targetCity) {
        GraphManager.Instance.SetTargetCity(targetCity.GetCitySO());

        ACOManager.Instance.AddStartPheromone();

        IsSimulationInitiated = true;
        IsSelectingCity = false;

        Dijkstra.CacheStartToTargetPath();   // ← ÖNCE cache (StartCitySO ve TargetCitySO ikisi de set edilmiş olmalı)
        StatsManager.Instance.StartRun();

        OnSimulationStarted?.Invoke(this, EventArgs.Empty);
    }

    public void ResetLevel() { //en baştaki durum
        IsSimulationInitiated = false;
        IsSelectingCity = false; //gerek yok aslında ama dursun
        IsSelectingStartCity = false;

        StatsManager.Instance.StopRun();
        Dijkstra.ClearCache();

        GraphManager.Instance.SetTargetCity(null);
        GraphManager.Instance.SetStartCity(null); 

        CitySelection.Instance.SetSelectedCity(null); //bunu başlatma kısmında da çağrabiliriz

        VehicleManager.Instance.ResetVariables();
        VehicleManager.Instance.SendAllVehiclesToHome();

        ACOManager.Instance.ResetStartPheromone();
        ResetAllPheromoneTrails();

        OnSimulationStopped?.Invoke(this, EventArgs.Empty);
    }

    private void ResetAllPheromoneTrails() {
        List<Road> roads = GraphManager.Instance.GetAllRoads();
        foreach (Road road in roads) {
            road.GetComponent<PheromoneVisualizer>().ResetPheromoneTrail();
        }
    }

    public static void SetTimeScale(float timeScale) {
        TimeScale = timeScale;
    }

    public void DesignateStartCity(City startCity) {
        CitySO citySO = startCity.GetCitySO();

        GraphManager.Instance.SetStartCity(citySO);
        VehicleManager.Instance.transform.position = CityPool.Instance.GetCityForCitySO(citySO).transform.position;
        
        Debug.Log("Başlangıç şehri: " + startCity);
    }
}
