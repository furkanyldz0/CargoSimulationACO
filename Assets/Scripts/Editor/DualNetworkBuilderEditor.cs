using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DualNetworkBuilder))]
public class DualNetworkBuilderEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        DualNetworkBuilder builder = (DualNetworkBuilder)target;

        GUILayout.Space(15);

        GUI.backgroundColor = new Color(0.2f, 0.6f, 1.0f);

        if (GUILayout.Button("Create Roads", GUILayout.Height(30))) {
            builder.BuildDualNetwork();
        }

        GUI.backgroundColor = Color.white;
    }
}