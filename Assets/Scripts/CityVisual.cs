using UnityEngine;

public class CityVisual : MonoBehaviour
{
    [SerializeField] private City city;

    [SerializeField] private GameObject defaultVisual;
    [SerializeField] private GameObject unselectedVisual;
    [SerializeField] private GameObject selectedVisual;

    private void Start() {
        VisualManager.Instance.OnEnteredSelectionMode += Instance_OnEnteredSelectionMode;
        VisualManager.Instance.OnExitedSelectionMode += Instance_OnExitedSelectionMode;
        VisualManager.Instance.OnSelectedCity += Instance_OnSelectedCity;
    }

    private void Instance_OnSelectedCity(object sender, VisualManager.OnSelectedCityEventArgs e) {
        if(e.city == city) { //bu event her bir þehir seçildiðinde tüm cityvisual'larda tetikleniyor, nesne kontrolü yaparak tek bir city için görsel güncelleme yapýyoruz
            Select();
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

    private void Select() {
        unselectedVisual.SetActive(false);
        defaultVisual.SetActive(false);
        selectedVisual.SetActive(true);
    }

    private void Unselect() {
        unselectedVisual.SetActive(true);
        defaultVisual.SetActive(false);
        selectedVisual.SetActive(false);
    }

    private void ActivateDefaultVisual() {
        unselectedVisual.SetActive(false);
        selectedVisual.SetActive(false);
        defaultVisual.SetActive(true);
    }

}
