using System;
using UnityEngine;

public class PheremoneVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer roadMeshRenderer;
    private Road road;
    private MaterialPropertyBlock propertyBlock;
    private Color color;
    private float pheremoneMaxAlphaValue = 50f;

    private void Start() {
        road = GetComponent<Road>();
        color = roadMeshRenderer.sharedMaterial.color;

        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update() {

        color.a = Math.Clamp(road.pheromoneLevel / pheremoneMaxAlphaValue, 0f, 1f);

        propertyBlock.SetColor("_BaseColor", color);
        roadMeshRenderer.SetPropertyBlock(propertyBlock);
    }
}
