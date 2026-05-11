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
            Debug.Log("Þehir seçilmedi ya da seçilen þehir baþlangýç þehir ile ayný!");
        }
    }

    public void StopSimulation() {
        LevelManager.Instance.ResetLevel();
        EnterIdleScreen();

        Debug.Log("Simülasyon durduruldu.");
    }

    public void EnterSelectingCityProcess() {
        LevelManager.Instance.EnterSelectingCityProcess();
        VisualManager.Instance.EnterCitySelectionMode();

        selectCityButton.gameObject.SetActive(false);
        initiateButton.gameObject.SetActive(true);
        cancelSelectCityButton.gameObject.SetActive(true);
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
