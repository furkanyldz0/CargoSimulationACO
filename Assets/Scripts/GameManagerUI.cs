using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    [SerializeField] private Button initiateButton;
    [SerializeField] private Button selectCityButton;
    [SerializeField] private Button stopButton;

    [SerializeField] private Slider vehicleSpawnTimeSlider;

    private void Start() {
        EnterIdleScreen();
    }

    public void EnterSelectingCityProcess() {
        selectCityButton.gameObject.SetActive(false);
        initiateButton.gameObject.SetActive(true);
        LevelManager.Instance.EnterSelectingCityProcess();
    }

    public void InitiateSimulation() {
        City targetCity = CitySelection.Instance.GetSelectedCity();

        if (targetCity != null) {
            LevelManager.Instance.InitiateSimulation(targetCity);
            vehicleSpawnTimeSlider.interactable = false;

            initiateButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);

            Debug.Log("Simülasyon baþladý, hedef þehir: " + targetCity.GetCitySO().name);
        }
        else {
            Debug.Log("Þehir seçiniz!");
        }
    }

    public void StopSimulation() {
        LevelManager.Instance.ResetLevel();
        EnterIdleScreen();

        Debug.Log("Simülasyon durduruldu.");
    }

    public void EnterIdleScreen() {
        vehicleSpawnTimeSlider.interactable = true;
        initiateButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(false);
        selectCityButton.gameObject.SetActive(true);
    }
}
