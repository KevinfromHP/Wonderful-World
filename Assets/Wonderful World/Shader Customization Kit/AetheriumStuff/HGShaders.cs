using System.Collections;
using UnityEngine;

namespace ShaderCustomizationKit
{
    public abstract class HGShaders: MonoBehaviour
    {
        public Material Material;
        public Renderer Renderer;
        public string MaterialName;
        public string[] shaderKeywords;

        public void Start()
        {
            GrabMaterialValues();
        }

        public void PutMaterialIntoRenderer(Renderer meshRenderer)
        {
            if (Material && meshRenderer)
                meshRenderer.material = Material;
        }

        public abstract void GrabMaterialValues();
    }
}