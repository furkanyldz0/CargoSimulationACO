using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

[RequireComponent(typeof(SplineContainer))]
public class SplineWaypointGenerator : MonoBehaviour {

    [SerializeField] private float waypointSpacing = 5.0f;
    [SerializeField] private Transform outputParent;

    public void GenerateWaypoints() {
        if (outputParent == null)
            Debug.LogError(this + ", Road'da waypointler için outputParent atanmamýþ!");

        DestroyPreviousWaypoints();
        CreateNewWaypoints();
    }

    private void DestroyPreviousWaypoints() {
        List<GameObject> waypointsToDestroy = new List<GameObject>();

        foreach (Transform child in outputParent) {
            waypointsToDestroy.Add(child.gameObject);
        }

        foreach (var waypoint in waypointsToDestroy) {
#if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(waypoint); //undo silme iþlemini hafýzaya almasý için
#endif //build ederken unity if bölgesini görmezden gelir, unityeditor kütüphanesi sadece oyun geliþtirilirken geçerli olur
        }//bu bölge belirtilmezse build edilirken bu kýsýmda hata verir
    }

    private void CreateNewWaypoints() {
        //spline uzunluðuna göre waypoint aralýklarýndan wp adedini hesaplýyoruz sonra aralýklarla oluþturuyoruz
        SplineContainer splineContainer = GetComponent<SplineContainer>();
        var spline = splineContainer.Splines[0];

        if (splineContainer.Splines.Count > 1)
            Debug.LogError(this + "'da birden fazla spline var!");

        float length = spline.GetLength();
        int waypointCount = Mathf.FloorToInt(length / waypointSpacing);

        for (int i = 0; i <= waypointCount; i++) {
            float t = (i * waypointSpacing) / length;
            if (t > 1.0f) t = 1.0f;

            float3 localPos = spline.EvaluatePosition(t); //yüzdelik olarak spline'da i indexinin konum olarak karþýlýðýný buluyor
            float3 tangent = spline.EvaluateTangent(t); //ileri yön vektörü karþýlýðý

            Vector3 worldPos = transform.TransformPoint((Vector3)localPos);

            GameObject waypoint = new GameObject($"Waypoint {i}");
            waypoint.transform.position = worldPos;
            waypoint.transform.SetParent(outputParent);
            waypoint.transform.forward = transform.TransformDirection((Vector3)tangent);

#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(waypoint, "Create Waypoint");
#endif
        }
    }

    
}