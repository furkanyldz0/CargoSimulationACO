using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineWaypointGenerator))]
public class SplineWaypointGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector(); //varsayýlan inspector

        SplineWaypointGenerator generator = (SplineWaypointGenerator)target;

        GUILayout.Space(15);
        GUI.backgroundColor = new Color(0.1f, 0.8f, 0.4f);

        if (GUILayout.Button("Generate Waypoints", GUILayout.Height(30))) {
            generator.GenerateWaypoints();
        }

        GUI.backgroundColor = Color.white;
    }
}