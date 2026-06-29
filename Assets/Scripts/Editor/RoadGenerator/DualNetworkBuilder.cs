using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using Unity.Mathematics;
using System.Collections.Generic;

public class DualNetworkBuilder : MonoBehaviour {

    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private Transform networkParent;

    [Header("Yol dizilimi ayarlarý")]
    [SerializeField] private float laneOffset = -2.15f; //iki yolun birbirine yakýnlýðý
    [SerializeField] private float roadHeightOffset = 0f;

    public void BuildDualNetwork() {
        if (roadPrefab == null) 
            Debug.LogError(this + " için Road prefabi atanmamýþ!");

        if (networkParent == null)
            Debug.LogError(this + " için networkParent belirlenmemiþ!");

        for (int i = networkParent.childCount - 1; i >= 0; i--) {
            Undo.DestroyObjectImmediate(networkParent.GetChild(i).gameObject); //undo silme iþlemini hafýzaya almasý için
        }

        City[] allCities = FindObjectsByType<City>(FindObjectsSortMode.None);

        System.Array.Sort(allCities, (cityA, cityB) => {
            string nameA = cityA.GetCitySO().cityName;
            string nameB = cityB.GetCitySO().cityName;

            return nameA.CompareTo(nameB);
        }); //so isimlerine göre sýralýyoruz

        HashSet<string> createdConnections = new HashSet<string>(); //set -> liste yapýsýna göre contains metodunu daha hýzlý bir þekilde gerçekleþtirir

        foreach (City startCity in allCities) {
            CitySO startCitySO = startCity.GetCitySO();

            if (startCitySO.neighbors == null) continue; //komþusu yoksa atla

            foreach (CitySO neighborCitySO in startCitySO.neighbors) {
                City endCity = GetCityForCitySO(neighborCitySO, allCities);
                if (endCity == null) {
                    Debug.Log($"{neighborCitySO} için transform atanmamýþ!");
                    continue;
                }

                CitySO endCitySO = endCity.GetCitySO();
                string directionalKey = $"{startCitySO.cityName}->{endCitySO.cityName}";

                if (!createdConnections.Contains(directionalKey)) {
                    CreateRoad(startCity, endCity);
                    createdConnections.Add(directionalKey);
                }
            }
        }
    }

    private void CreateRoad(City startCity, City endCity) {
        CitySO startCitySO = startCity.GetCitySO();
        CitySO endCitySO = endCity.GetCitySO();

        // 1. Yön ve Þerit Kaydýrma
        Vector3 direction = (endCity.transform.position - startCity.transform.position).normalized;
        Vector3 rightVector = Vector3.Cross(direction, Vector3.up).normalized; //yönün sað vektörünü verir
        //örn a->b b->a yönleri zýt olduðundan kendi yönlerinin saðýný verir

        Vector3 roadStartPosition = startCity.transform.position + (rightVector * laneOffset);
        Vector3 roadEndPosition = endCity.transform.position + (rightVector * laneOffset);

        // Ýki noktanýn tam ortasýný buluyoruz
        Vector3 midPoint = (roadStartPosition + roadEndPosition) / 2f;

        GameObject roadObject = (GameObject)PrefabUtility.InstantiatePrefab(roadPrefab, networkParent);
        roadObject.name = $"Road {startCitySO.cityName}-{endCitySO.cityName}";

        // Objeyi 0,0,0 yerine tam ortaya yerleþtiriyoruz ve hedefe doðru döndürüyoruz
        roadObject.transform.position = midPoint;
        if (direction != Vector3.zero) {
            roadObject.transform.rotation = Quaternion.LookRotation(direction);
        }

        Road roadScript = roadObject.GetComponent<Road>();
        if (roadScript != null) {
            roadScript.startCitySO = startCitySO;
            roadScript.endCitySO = endCitySO;
        }

        GenerateSplineConnection(roadObject, roadStartPosition, roadEndPosition);
        GenerateWaypoints(roadObject);
    }

    private void GenerateSplineConnection(GameObject roadObject, Vector3 roadStartPosition, Vector3 roadEndPosition) {
        SplineContainer container = roadObject.GetComponentInChildren<SplineContainer>();
        if (container != null) {
            Undo.RecordObject(container, "Spline Update");

            if (container.Splines.Count == 0) container.AddSpline();

            Spline mySpline = container.Splines[0];
            mySpline.Clear();

            // Objeyi taþýdýðýmýz ve döndürdüðümüz için InverseTransformPoint bize 
            // merkezden eþit uzaklýkta (+Z ve -Z yönünde) "yerel" noktalar verecektir.
            float3 localStart = container.transform.InverseTransformPoint(roadStartPosition);
            float3 localEnd = container.transform.InverseTransformPoint(roadEndPosition);

            localStart.y = roadHeightOffset;
            localEnd.y = roadHeightOffset;

            mySpline.Add(new BezierKnot(localStart), TangentMode.AutoSmooth);
            mySpline.Add(new BezierKnot(localEnd), TangentMode.AutoSmooth);

            EditorUtility.SetDirty(container);
        }
    }

    private void GenerateWaypoints(GameObject roadObj) {
        SplineWaypointGenerator waypointGenerator = roadObj.GetComponentInChildren<SplineWaypointGenerator>();
        if (waypointGenerator != null) {
            waypointGenerator.GenerateWaypoints();
        }

        Undo.RegisterCreatedObjectUndo(roadObj, "Create Road");
    }

    private City GetCityForCitySO(CitySO citySO, City[] cities) {
        foreach (City c in cities) {
            if (c.GetCitySO() == citySO) {
                return c;
            }
        }
        return null;
    }

}