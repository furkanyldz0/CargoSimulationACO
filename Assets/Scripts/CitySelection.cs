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

        Vector3 raycastPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(raycastPosition); //

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) { //yeni input system ile güncellenecek
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 3000, cityLayer)) {
                if (raycastHit.transform.TryGetComponent(out City city)) {
                    SetSelectedCity(city);
                    Debug.Log(city.GetCitySO().name);
                }
            }
        }
    }

    public void SetSelectedCity(City city) {
        selectedCity = city;
        if(city != null)
            VisualManager.Instance.SelectCity(selectedCity);
    }

    public City GetSelectedCity() {
        return selectedCity;
    }
}
