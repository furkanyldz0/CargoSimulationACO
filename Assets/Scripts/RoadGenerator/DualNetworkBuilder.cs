using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using Unity.Mathematics;
using System.Collections.Generic;

public class DualNetworkBuilder : MonoBehaviour {
    [Header("Kurulum")]
    public GameObject roadPrefab;
    public Transform networkParent;

    [Header("Þerit Ayarlarý")]
    public float laneOffset = 1.5f;
    public float roadHeightOffset = 0.3f; // YENÝ: Yüksekliði Inspector'dan ayarlayabilmen için eklendi

    [ContextMenu("Yol Aðýný Baþtan Ör")]
    public void BuildDualNetwork() {
        if (roadPrefab == null) {
            Debug.LogError("HATA: Lütfen Road Prefab'ýný atayýn!");
            return;
        }

        if (networkParent != null) {
            for (int i = networkParent.childCount - 1; i >= 0; i--) {
                Undo.DestroyObjectImmediate(networkParent.GetChild(i).gameObject);
            }
        }

        City[] allCities = Object.FindObjectsByType<City>(FindObjectsSortMode.None);
        HashSet<string> createdConnections = new HashSet<string>();

        foreach (City startCity in allCities) {
            CitySO startSO = startCity.GetCitySO();
            if (startSO == null || startSO.neighbors == null) continue;

            foreach (CitySO neighborSO in startSO.neighbors) {
                City endCity = FindCityBySO(allCities, neighborSO);
                if (endCity == null) continue;

                CitySO endSO = endCity.GetCitySO();
                string directionalKey = $"{startSO.cityName}->{endSO.cityName}";

                Debug.Log(directionalKey);

                if (!createdConnections.Contains(directionalKey)) {
                    CreateRoad(startCity, endCity, startSO, endSO);
                    createdConnections.Add(directionalKey);
                }
            }
        }
    }

    private void CreateRoad(City startCity, City endCity, CitySO startSO, CitySO endSO) {
        Vector3 direction = (endCity.transform.position - startCity.transform.position).normalized;
        Vector3 rightVector = Vector3.Cross(direction, Vector3.up).normalized;

        Vector3 worldStart = startCity.transform.position + (rightVector * laneOffset);
        Vector3 worldEnd = endCity.transform.position + (rightVector * laneOffset);

        GameObject roadObj = (GameObject)PrefabUtility.InstantiatePrefab(roadPrefab, networkParent);
        roadObj.name = $"Road {startSO.cityName}-{endSO.cityName}";

        roadObj.transform.position = Vector3.zero;
        roadObj.transform.rotation = Quaternion.identity;

        // 1. Road verilerini ana objeden ata
        Road roadScript = roadObj.GetComponent<Road>();
        if (roadScript != null) {
            roadScript.startCitySO = startSO;
            roadScript.endCitySO = endSO;
        }

        // 2. Alt objedeki (Child) SplineContainer'ý bul
        SplineContainer container = roadObj.GetComponentInChildren<SplineContainer>();
        if (container != null) {
            Undo.RecordObject(container, "Spline Update");

            if (container.Splines.Count == 0) container.AddSpline();

            Spline mySpline = container.Splines[0];
            mySpline.Clear();

            // ÖNEMLÝ: Koordinat dönüþümünü spline'ý içeren objenin transformuyla yapýyoruz
            float3 localStart = container.transform.InverseTransformPoint(worldStart);
            float3 localEnd = container.transform.InverseTransformPoint(worldEnd);

            // YENÝ: Spline knot'larýnýn yerel Y (yükseklik) deðerini sabitliyoruz
            localStart.y = roadHeightOffset;
            localEnd.y = roadHeightOffset;

            mySpline.Add(new BezierKnot(localStart), TangentMode.AutoSmooth);
            mySpline.Add(new BezierKnot(localEnd), TangentMode.AutoSmooth);

            EditorUtility.SetDirty(container);
        }

        // 3. Alt objedeki Waypoint Generator'ý tetikle
        SplineWaypointGenerator wpGen = roadObj.GetComponentInChildren<SplineWaypointGenerator>();
        if (wpGen != null) {
            wpGen.sideOffset = 0f;
            wpGen.GenerateWaypoints();
        }

        Undo.RegisterCreatedObjectUndo(roadObj, "Create Road");
    }

    private City FindCityBySO(City[] cities, CitySO so) {
        foreach (var c in cities) if (c.GetCitySO() == so) return c;
        return null;
    }
}