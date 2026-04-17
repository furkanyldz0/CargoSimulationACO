using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DualNetworkBuilder))]
public class DualNetworkBuilderEditor : Editor {
    public override void OnInspectorGUI() {
        // Standart deðiþkenleri çiz (roadPrefab, networkParent, laneOffset)
        DrawDefaultInspector();

        DualNetworkBuilder builder = (DualNetworkBuilder)target;

        GUILayout.Space(15);

        // Dikkat çekici bir mavi/mor tonu (Ýsteðe baðlý)
        GUI.backgroundColor = new Color(0.2f, 0.6f, 1.0f);

        // Butonu çiz ve týklandýðýnda metodu tetikle
        if (GUILayout.Button("Create Roads", GUILayout.Height(30))) {
            builder.BuildDualNetwork();
        }

        // Rengi eski haline döndür ki diðer bileþenler etkilenmesin
        GUI.backgroundColor = Color.white;
    }
}