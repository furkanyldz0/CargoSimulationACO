using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    [SerializeField] private Button initiateButton;
    [SerializeField] private Button selectCityButton;
    [SerializeField] private Button cancelSelectCityButton;
    [SerializeField] private Button stopButton;

    [SerializeField] private Slider vehicleSpawnTimeSlider;

    private void Start() {
        EnterIdleScreen();
    }

    public void EnterSelectingCityProcess() {
        selectCityButton.gameObject.SetActive(false);
        initiateButton.gameObject.SetActive(true);
        cancelSelectCityButton.gameObject.SetActive(true);

        LevelManager.Instance.EnterSelectingCityProcess();
        VisualManager.Instance.EnterCitySelectionMode();
    }

    public void InitiateSimulation() {
        City targetCity = CitySelection.Instance.GetSelectedCity();

        if (targetCity != null) {
            LevelManager.Instance.InitiateSimulation(targetCity);

            vehicleSpawnTimeSlider.interactable = false;
            initiateButton.gameObject.SetActive(false);
            cancelSelectCityButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);

            VisualManager.Instance.ExitCitySelectionMode();

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

    public void CancelSelection() {
        LevelManager.Instance.ResetLevel();
        VisualManager.Instance.ExitCitySelectionMode();
        EnterIdleScreen();
        
        Debug.Log("Þehir seçimi iptal edildi.");
    }

    public void EnterIdleScreen() {
        vehicleSpawnTimeSlider.interactable = true;
        initiateButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(false);
        selectCityButton.gameObject.SetActive(true);
        cancelSelectCityButton.gameObject.SetActive(false);
    }
}
