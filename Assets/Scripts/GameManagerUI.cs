using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    [SerializeField] private Button initiateButton;
    [SerializeField] private Button selectStartCityButton;
    [SerializeField] private Button selectTargetCityButton;
    [SerializeField] private Button cancelSelectCityButton;
    [SerializeField] private Button stopButton;

    [SerializeField] private Slider vehicleSpawnTimeSlider;
    [SerializeField] private Slider startPheromoneLevelSlider;

    private void Start() {
        EnterIdleScreen();
    }

    public void EnterSelectingStartCityProcess() {
        LevelManager.Instance.EnterSelectingCityProcess();
        VisualManager.Instance.EnterStartCitySelectionMode();

        selectStartCityButton.gameObject.SetActive(false);
        selectTargetCityButton.gameObject.SetActive(true);
        cancelSelectCityButton.gameObject.SetActive(true);
    }

    public void EnterSelectingTargetCityProcess() {
        City startCity = CitySelection.Instance.GetSelectedCity();

        if(startCity != null) {
            LevelManager.Instance.ExitSelectingStartCityProcess(startCity);
            VisualManager.Instance.EnterTargetCitySelectionMode();
            selectTargetCityButton.gameObject.SetActive(false);
            initiateButton.gameObject.SetActive(true);
        }
        else {
            string notificationText = "Baþlangýç þehri seçiniz!";
            NotificationManagerUI.Instance.Notificate(notificationText);
            Debug.Log(notificationText);
        }
        
    }

    public void CancelSelection() {
        LevelManager.Instance.ResetLevel();
        VisualManager.Instance.ExitCitySelectionMode();
        EnterIdleScreen();
        
        Debug.Log("Þehir seçimi iptal edildi.");
    }

    public void InitiateSimulation() {
        City targetCity = CitySelection.Instance.GetSelectedCity();

        if (targetCity != null) {
            LevelManager.Instance.InitiateSimulation(targetCity);

            vehicleSpawnTimeSlider.interactable = false;
            startPheromoneLevelSlider.interactable = false;
            initiateButton.gameObject.SetActive(false);
            cancelSelectCityButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);

            VisualManager.Instance.ExitCitySelectionMode();

            Debug.Log("Simülasyon baþladý, hedef þehir: " + targetCity.GetCitySO().name);
        }
        else if(targetCity == null){
            string notificationText = "Hedef þehir seçiniz!";
            NotificationManagerUI.Instance.Notificate(notificationText);
            Debug.Log(notificationText);
        }
    }

    public void StopSimulation() {
        LevelManager.Instance.ResetLevel();
        EnterIdleScreen();

        Debug.Log("Simülasyon durduruldu.");
    }

    public void EnterIdleScreen() {
        vehicleSpawnTimeSlider.interactable = true;
        startPheromoneLevelSlider.interactable = true;
        initiateButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(false);
        selectTargetCityButton.gameObject.SetActive(false);
        cancelSelectCityButton.gameObject.SetActive(false);
        selectStartCityButton.gameObject.SetActive(true);
    }
}
