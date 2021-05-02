using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ForgottenFoes.Utils
{
    /// <summary>
    /// Attach to anything, and feed in a material that has the hgcloudintersectionremap shader.
    /// You then gain access to manipulate this in any Runtime Inspector of your choice.
    /// </summary>
    public class HGIntersectionController : MonoBehaviour
    {
        public Material Material;
        public Renderer Renderer;
        public string MaterialName;
        public bool replaceShaderData = false;
        public bool deleteOnStart = false;

        public enum _SrcBlendFloatEnum
        {
            Zero,
            One,
            DstColor,
            SrcColor,
            OneMinusDstColor,
            SrcAlpha,
            OneMinusSrcColor,
            DstAlpha,
            OneMinusDstAlpha,
            SrcAlphaSaturate,
            OneMinusSrcAlpha
        }
        public enum _DstBlendFloatEnum
        {
            Zero,
            One,
            DstColor,
            SrcColor,
            OneMinusDstColor,
            SrcAlpha,
            OneMinusSrcColor,
            DstAlpha,
            OneMinusDstAlpha,
            SrcAlphaSaturate,
            OneMinusSrcAlpha
        }
        public _SrcBlendFloatEnum _Source_Blend_Mode;
        public _DstBlendFloatEnum _Destination_Blend_Mode;

        public Color _Tint;
        public Texture _MainTex;
        public Vector2 _MainTexScale;
        public Vector2 _MainTexOffset;
        public Texture _Cloud1Tex;
        public Vector2 _Cloud1TexScale;
        public Vector2 _Cloud1TexOffset;
        public Texture _Cloud2Tex;
        public Vector2 _Cloud2TexScale;
        public Vector2 _Cloud2TexOffset;
        public Texture _RemapTex;
        public Vector2 _RemapTexScale;
        public Vector2 _RemapTexOffset;
        public Vector4 _CutoffScroll;

        [Range(0f, 30f)]
        public float _SoftFactor;

        [Range(0.1f, 20f)]
        public float _SoftPower;

        [Range(0f, 5f)]
        public float _BrightnessBoost;

        [Range(0.1f, 20f)]
        public float _RimPower;

        [Range(0f, 5f)]
        public float _RimStrength;

        [Range(0f, 20f)]
        public float _AlphaBoost;

        [Range(0f, 20f)]
        public float _IntersectionStrength;

        public enum _CullEnum
        {
            Off,
            Front,
            Back
        }
        public _CullEnum _Cull_Mode;

        public bool _IgnoreVertexColors;
        public bool _EnableTriplanarProjectionsForClouds;

        public void Start()
        {
            if (!replaceShaderData)
                GrabMaterialValues();
        }

        public void GrabMaterialValues()
        {
            if (Material)
            {
                _Source_Blend_Mode = (_SrcBlendFloatEnum)(int)Material.GetFloat("_SrcBlendFloat");
                _Destination_Blend_Mode = (_DstBlendFloatEnum)(int)Material.GetFloat("_DstBlendFloat");
                _Tint = Material.GetColor("_TintColor");
                _MainTex = Material.GetTexture("_MainTex");
                _MainTexScale = Material.GetTextureScale("_MainTex");
                _MainTexOffset = Material.GetTextureOffset("_MainTex");
                _Cloud1Tex = Material.GetTexture("_Cloud1Tex");
                _Cloud1TexScale = Material.GetTextureScale("_Cloud1Tex");
                _Cloud1TexOffset = Material.GetTextureOffset("_Cloud1Tex");
                _Cloud2Tex = Material.GetTexture("_Cloud2Tex");
                _Cloud2TexScale = Material.GetTextureScale("_Cloud2Tex");
                _Cloud2TexOffset = Material.GetTextureOffset("_Cloud2Tex");
                _RemapTex = Material.GetTexture("_RemapTex");
                _RemapTexScale = Material.GetTextureScale("_RemapTex");
                _RemapTexOffset = Material.GetTextureOffset("_RemapTex");
                _CutoffScroll = Material.GetVector("_CutoffScroll");
                _SoftFactor = Material.GetFloat("_InvFade");
                _SoftPower = Material.GetFloat("_SoftPower");
                _BrightnessBoost = Material.GetFloat("_Boost");
                _RimPower = Material.GetFloat("_RimPower");
                _RimStrength = Material.GetFloat("_RimStrength");
                _AlphaBoost = Material.GetFloat("_AlphaBoost");
                _IntersectionStrength = Material.GetFloat("_IntersectionStrength");
                _Cull_Mode = (_CullEnum)(int)Material.GetFloat("_Cull");
                _IgnoreVertexColors = Convert.ToBoolean(Material.GetFloat("_VertexColorsOn"));
                _EnableTriplanarProjectionsForClouds = Convert.ToBoolean(Material.GetFloat("_TriplanarOn"));
                MaterialName = Material.name;
            }
        }

        public void PutMaterialIntoRenderer(Renderer meshRenderer)
        {
            if (Material && meshRenderer)
            {
                meshRenderer.material = Material;
            }
        }

        public void Update()
        {

            if (Material)
            {
                if (Material.name != MaterialName && Renderer)
                {
                    GrabMaterialValues();
                    PutMaterialIntoRenderer(Renderer);
                }
                Material.SetFloat("_SrcBlendFloat", Convert.ToSingle(_Source_Blend_Mode));
                Material.SetFloat("_DstBlendFloat", Convert.ToSingle(_Destination_Blend_Mode));
                Material.SetColor("_TintColor", _Tint);
                if (_MainTex)
                {
                    Material.SetTexture("_MainTex", _MainTex);
                    Material.SetTextureScale("_MainTex", _MainTexScale);
                    Material.SetTextureOffset("_MainTex", _MainTexOffset);
                }
                else
                {
                    Material.SetTexture("_MainTex", null);
                }

                if (_Cloud1Tex)
                {
                    Material.SetTexture("_Cloud1Tex", _Cloud1Tex);
                    Material.SetTextureScale("_Cloud1Tex", _Cloud1TexScale);
                    Material.SetTextureOffset("_Cloud1Tex", _Cloud1TexOffset);
                }
                else
                {
                    Material.SetTexture("_Cloud1Tex", null);
                }

                if (_Cloud2Tex)
                {
                    Material.SetTexture("_Cloud2Tex", _Cloud2Tex);
                    Material.SetTextureScale("_Cloud2Tex", _Cloud2TexScale);
                    Material.SetTextureOffset("_Cloud2Tex", _Cloud2TexOffset);
                }
                else
                {
                    Material.SetTexture("_Cloud2Tex", null);
                }

                if (_RemapTex)
                {
                    Material.SetTexture("_RemapTex", _RemapTex);
                    Material.SetTextureScale("_RemapTex", _RemapTexScale);
                    Material.SetTextureOffset("_RemapTex", _RemapTexOffset);
                }
                else
                {
                    Material.SetTexture("_RemapTex", null);
                }

                Material.SetVector("_CutoffScroll", _CutoffScroll);
                Material.SetFloat("_InvFade", _SoftFactor);
                Material.SetFloat("_SoftPower", _SoftPower);
                Material.SetFloat("_Boost", _BrightnessBoost);
                Material.SetFloat("_RimPower", _RimPower);
                Material.SetFloat("_RimStrength", _RimStrength);
                Material.SetFloat("_AlphaBoost", _AlphaBoost);
                Material.SetFloat("_IntersectionStrength", _IntersectionStrength);
                Material.SetFloat("_Cull", Convert.ToSingle(_Cull_Mode));
                Material.SetFloat("_VertexColorsOn", Convert.ToSingle(_IgnoreVertexColors));
                Material.SetFloat("_TriplanarOn", Convert.ToSingle(_EnableTriplanarProjectionsForClouds));
            }
            if (deleteOnStart)
                Destroy(this);
        }
    }
}
