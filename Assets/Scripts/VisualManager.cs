using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    public static VisualManager Instance { get; private set; }

    public event EventHandler OnEnteredSelectionMode;
    public event EventHandler OnExitedSelectionMode;

    public event EventHandler<OnSelectedCityEventArgs> OnSelectedCity;
    public class OnSelectedCityEventArgs : EventArgs {
        public City city;
    }

    [SerializeField] private GameObject selectionFilter;
    [SerializeField] private TextMeshProUGUI selectCityText;
    [SerializeField] private TextMeshProUGUI destinationCityText;

    private void Start() {
        selectionFilter.SetActive(false);
    }

    private void Awake() {
        if (Instance != null) { 
            Debug.LogError("Sahnede birden fazla VisualManager var!");
        }
        Instance = this;
    }

    public void EnterCitySelectionMode() {
        OnEnteredSelectionMode?.Invoke(this, EventArgs.Empty);

        selectionFilter.SetActive(true);
        selectCityText.gameObject.SetActive(true);
    }

    public void ExitCitySelectionMode() {
        OnExitedSelectionMode?.Invoke(this, EventArgs.Empty);

        selectionFilter.SetActive(false);
        selectCityText.gameObject.SetActive(false);
        destinationCityText.text = $"Hedef Þehir:\n-";
        destinationCityText.gameObject.SetActive(false);
    }

    public void SelectCity(City city) {
        OnSelectedCity?.Invoke(this, new OnSelectedCityEventArgs{
            city = city
        });

        selectCityText.gameObject.SetActive(false);
        destinationCityText.gameObject.SetActive(true);
        destinationCityText.text = $"Hedef Þehir:\n{city.GetCitySO().name}";
    }


}
