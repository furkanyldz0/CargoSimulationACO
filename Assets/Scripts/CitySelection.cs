using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CitySelection : MonoBehaviour
{
    public static CitySelection Instance { get; private set; }

    [SerializeField] private LayerMask cityLayer = new LayerMask();

    private City selectedCity;

    private void Awake() {
        if (Instance != null)
            Debug.LogError("Sahnede birden fazla CitySelection var!");

        Instance = this;
    }

    private void Update() {
        if (!LevelManager.Instance.IsSelectingCity)
            return; //þehir seçmiyorsa bu scriptin update'i çalýþmasýn



        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Vector3 raycastPosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(raycastPosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, 3000, cityLayer)) {
                if (raycastHit.transform.TryGetComponent(out City city)) {
                    if(IsValidToSelect(city)) {
                        SetSelectedCity(city);
                        Debug.Log(city.GetCitySO().name);
                        AudioManager.Instance.PlayCitySelectSound();
                    }
                    else {
                        string notificationText = "Hedef þehir ile baþlangýç þehri ayný olamaz!";
                        NotificationManagerUI.Instance.Notificate(notificationText);
                        Debug.Log(notificationText);
                    }
                }
            }
        }
    }

    public void SetSelectedCity(City city) {
        selectedCity = city;
        if(selectedCity != null) {
            VisualManager.Instance.SelectCity(city);
        }
    }

    public City GetSelectedCity() {
        return selectedCity;
    }

    private bool IsValidToSelect(City targetCity) {
        return targetCity.GetCitySO() != GraphManager.Instance.StartCitySO;
    }
}
