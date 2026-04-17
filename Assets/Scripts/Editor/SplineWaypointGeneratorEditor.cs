using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineWaypointGenerator))]
public class SplineWaypointGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        // Standart deðiþkenleri (spacing, prefix vb.) çiz
        DrawDefaultInspector();

        SplineWaypointGenerator generator = (SplineWaypointGenerator)target;

        GUILayout.Space(15);
        GUI.backgroundColor = new Color(0.1f, 0.8f, 0.4f); // Güzel bir yeþil buton

        if (GUILayout.Button("Generate Waypoints", GUILayout.Height(30))) {
            generator.GenerateWaypoints();
        }

        GUI.backgroundColor = Color.white;
    }
}