using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class VisualManager : MonoBehaviour
{
    public static VisualManager Instance { get; private set; }

    public event EventHandler OnEnteredSelectionMode;
    public event EventHandler OnExitedSelectionMode;

    public event EventHandler<OnSelectedCityEventArgs> OnSelectedTargetCity;
    public event EventHandler<OnSelectedCityEventArgs> OnSelectedStartCity;

    public class OnSelectedCityEventArgs : EventArgs {
        public City city;
    }

    [SerializeField] private Volume selectionFilterVolume;

    [SerializeField] private TextMeshProUGUI selectStartCityText;
    [SerializeField] private TextMeshProUGUI startCityText;

    [SerializeField] private TextMeshProUGUI selectTargetCityText;
    [SerializeField] private TextMeshProUGUI destinationCityText;

    private void Start() {
        DisableSelectionFilter();
    }

    private void Awake() {
        if (Instance != null) { 
            Debug.LogError("Sahnede birden fazla VisualManager var!");
        }
        Instance = this;
    }

    public void EnterStartCitySelectionMode() {
        OnEnteredSelectionMode?.Invoke(this, EventArgs.Empty);

        ActivateSelectionFilter();
        selectStartCityText.gameObject.SetActive(true);
    }

    public void EnterTargetCitySelectionMode() {
        selectTargetCityText.gameObject.SetActive(true);

        selectStartCityText.gameObject.SetActive(false);
        startCityText.gameObject.SetActive(false);
        startCityText.text = "Baþlangýç Þehri:\n-";
    }

    public void ExitCitySelectionMode() {
        OnExitedSelectionMode?.Invoke(this, EventArgs.Empty);
        DisableSelectionFilter();

        selectTargetCityText.gameObject.SetActive(false);
        destinationCityText.text = "Hedef Þehir:\n-";
        destinationCityText.gameObject.SetActive(false);

        selectStartCityText.gameObject.SetActive(false);
        startCityText.gameObject.SetActive(false);
        startCityText.text = "Baþlangýç Þehri:\n-";
    }

    public void SelectCity(City city) {
        if (LevelManager.Instance.IsSelectingStartCity)
            SelectStartCity(city);
        else
            SelectTargetCity(city);
    }

    private void SelectTargetCity(City city) {
        OnSelectedTargetCity?.Invoke(this, new OnSelectedCityEventArgs{
            city = city
        });
        selectTargetCityText.gameObject.SetActive(false);
        destinationCityText.gameObject.SetActive(true);
        destinationCityText.text = $"Hedef Þehir:\n{city.GetCitySO().name}";
    }

    private void SelectStartCity(City city) {
        OnSelectedStartCity?.Invoke(this, new OnSelectedCityEventArgs {
            city = city
        });
        selectStartCityText.gameObject.SetActive(false);
        startCityText.gameObject.SetActive(true);
        startCityText.text = $"Baþlangýç Þehri:\n{city.GetCitySO().name}";
    }

    private void ActivateSelectionFilter() { //eðer setactive false true þeklinde yaparsam daha çok yük bindiriyormuþ
        selectionFilterVolume.weight = 1f;
    }
    private void DisableSelectionFilter() {
        selectionFilterVolume.weight = 0f;
    }
    

}
