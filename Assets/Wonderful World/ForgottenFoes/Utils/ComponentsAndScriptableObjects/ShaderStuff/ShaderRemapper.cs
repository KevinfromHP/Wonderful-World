using UnityEngine;

namespace ForgottenFoes.Utils
{
    public abstract class ShaderRemapper : ScriptableObject
    {
        public Material Material;
        public string[] shaderKeywords;
        public abstract void UpdateMaterial();
    }
}