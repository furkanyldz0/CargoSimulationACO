using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour //levelmanager yerine baþka isim yazabilirim
{
    public static LevelManager Instance { get; private set; }

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

        ACOManager.Instance.AddStartPheromone(); //bunu deðerler yapacaðýz, paneli daha eklemediðim için acomanager'in kendi deðerini kullanýyor

        IsSimulationInitiated = true;
        IsSelectingCity = false;
    }

    public void ResetLevel() { //en baþtaki durum
        IsSimulationInitiated = false;
        IsSelectingCity = false; //gerek yok aslýnda ama dursun
        IsSelectingStartCity = false;

        GraphManager.Instance.SetTargetCity(null);
        GraphManager.Instance.SetStartCity(null); 

        CitySelection.Instance.SetSelectedCity(null); //bunu baþlatma kýsmýnda da çaðrabiliriz

        VehicleManager.Instance.ResetVariables();
        VehicleManager.Instance.SendAllVehiclesToHome();

        ACOManager.Instance.ResetStartPheromone();
        ResetAllPheromoneTrails();
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
        
        Debug.Log("Baþlangýç þehri: " + startCity);
    }
}
