using UnityEngine;

public class CityVisual : MonoBehaviour
{
    [SerializeField] private City city;

    [SerializeField] private GameObject defaultVisual;
    [SerializeField] private GameObject unselectedVisual;
    [SerializeField] private GameObject selectedTargetVisual;
    [SerializeField] private GameObject selectedStartVisual;

    private void Start() {
        VisualManager.Instance.OnEnteredSelectionMode += Instance_OnEnteredSelectionMode;
        VisualManager.Instance.OnExitedSelectionMode += Instance_OnExitedSelectionMode;
        VisualManager.Instance.OnSelectedTargetCity += Instance_OnSelectedTargetCity;
        VisualManager.Instance.OnSelectedStartCity += Instance_OnSelectedStartCity;
    }

    private void Instance_OnSelectedStartCity(object sender, VisualManager.OnSelectedCityEventArgs e) {
        if (e.city == city) {
            SelectAsStart();
        }
        else {
            Unselect();
        }
    }

    private void Instance_OnSelectedTargetCity(object sender, VisualManager.OnSelectedCityEventArgs e) {
        if (e.city == city) { //bu event her bir þehir seçildiðinde tüm cityvisual'larda tetikleniyor, nesne kontrolü yaparak tek bir city için görsel güncelleme yapýyoruz
            SelectAsTarget();
        }
        else if (city.GetCitySO() == GraphManager.Instance.StartCitySO) {
            SelectAsStart();
        }
        else {
            Unselect();
        }
    }

    private void Instance_OnEnteredSelectionMode(object sender, System.EventArgs e) {
        Unselect();
    }

    private void Instance_OnExitedSelectionMode(object sender, System.EventArgs e) {
        ActivateDefaultVisual();
    }

    private void SelectAsTarget() {
        unselectedVisual.SetActive(false);
        defaultVisual.SetActive(false);
        selectedTargetVisual.SetActive(true);
        selectedStartVisual.SetActive(false);
    }

    private void SelectAsStart() {
        unselectedVisual.SetActive(false);
        defaultVisual.SetActive(false);
        selectedTargetVisual.SetActive(false);
        selectedStartVisual.SetActive(true);
    }

    private void Unselect() {
        unselectedVisual.SetActive(true);
        defaultVisual.SetActive(false);
        selectedTargetVisual.SetActive(false);
        selectedStartVisual.SetActive(false);
    }

    private void ActivateDefaultVisual() {
        unselectedVisual.SetActive(false);
        selectedTargetVisual.SetActive(false);
        defaultVisual.SetActive(true);
        selectedStartVisual.SetActive(false);
    }

}
