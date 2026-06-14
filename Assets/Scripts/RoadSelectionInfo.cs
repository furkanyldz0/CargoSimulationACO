using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoadSelectionInfo : MonoBehaviour
{
    [SerializeField] private LayerMask roadLayer = new LayerMask();
    [SerializeField] private TextMeshProUGUI roadInfoText;

    private int raycastFreqPerSecond = 60;
    private float checkRaycastTime;
    private float checkRaycastTimeDelta;

    private void Start() {
        checkRaycastTime = 1f / (float) raycastFreqPerSecond;
    }

    private void Update()
    {
        if (!LevelManager.Instance.IsSimulationInitiated)
            return;


        if (checkRaycastTimeDelta > 0f) {
            checkRaycastTimeDelta -= Time.deltaTime; 
        }

        if(checkRaycastTimeDelta <= 0f) {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, 10000f, roadLayer) && !EventSystem.current.IsPointerOverGameObject()) {
                Road highlightedRoad = raycastHit.transform.GetComponentInParent<Road>();
                if (highlightedRoad != null) {
                    ShowRoadInfo(mousePosition, highlightedRoad);
                }
            }
            else { //raycast ile herhangi bir yol nesnesi algýlanmadýðý zaman
                roadInfoText.gameObject.SetActive(false);
            }

            checkRaycastTimeDelta = checkRaycastTime;
        }

        
    }

    private void ShowRoadInfo(Vector3 mousePosition, Road highlightedRoad) {
        string startCityName = highlightedRoad.startCitySO.name;
        string endCityName = highlightedRoad.endCitySO.name;

        roadInfoText.SetText(($"{startCityName} - {endCityName} Yolu" +
            $"\r\n{highlightedRoad.distance.ToString("F2")} km"));
        roadInfoText.transform.position = mousePosition;
        roadInfoText.gameObject.SetActive(true);
    }
}
