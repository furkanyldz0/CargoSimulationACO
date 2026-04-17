using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

[RequireComponent(typeof(SplineContainer))]
public class SplineWaypointGenerator : MonoBehaviour {
    [Header("Ayarlar")]
    public float spacing = 2.0f;
    public float sideOffset = 0f;
    public string waypointPrefix = "Waypoint ";

    [Header("Hiyerarþi")]
    public Transform outputParent;

    public void GenerateWaypoints() {
        SplineContainer container = GetComponent<SplineContainer>();
        Transform parentTransform = outputParent != null ? outputParent : transform;

        // 1. ESKÝLERÝ TEMÝZLE: Artýk kontrol yok, parent altýndaki her þeyi siliyoruz.
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in parentTransform) {
            toDestroy.Add(child.gameObject);
        }

        foreach (var obj in toDestroy) {
#if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(obj);
#endif
        }

        // 2. YENÝLERÝ OLUÞTUR
        for (int sIndex = 0; sIndex < container.Splines.Count; sIndex++) {
            var spline = container.Splines[sIndex];
            float length = spline.GetLength();
            int count = Mathf.FloorToInt(length / spacing);

            for (int i = 0; i <= count; i++) {
                float t = (i * spacing) / length;
                if (t > 1.0f) t = 1.0f;

                float3 localPos = spline.EvaluatePosition(t);
                float3 tangent = spline.EvaluateTangent(t);
                float3 up = spline.EvaluateUpVector(t);

                float3 right = math.cross(tangent, up);
                right = math.normalize(right);

                float3 offsetPos = localPos + (right * sideOffset);
                Vector3 worldPos = transform.TransformPoint((Vector3)offsetPos);

                // Direkt numaralandýrma
                GameObject wp = new GameObject($"{waypointPrefix} {i}");
                wp.transform.position = worldPos;

                // DOÐRUDAN PARENT ATAMASI (Araya grup objesi koymadan)
                wp.transform.SetParent(parentTransform);

                wp.transform.forward = transform.TransformDirection((Vector3)tangent);

#if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo(wp, "Create Waypoint");
#endif
            }
        }

        Debug.Log($"Toplam {container.Splines.Count} spline için waypointler doðrudan parent altýna üretildi.");
    }
}