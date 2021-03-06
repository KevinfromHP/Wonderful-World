using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShaderCustomizationKit
{
    /// <summary>
    /// Attach to anything, and feed in a material that has the hgcloudremap shader.
    /// You then gain access to manipulate this in any Runtime Inspector of your choice.
    /// </summary>
    public class HGCloudRemapController : HGShaders
    {
        public Color _Tint;
        public Texture _MainTex;
        public Vector2 _MainTexScale;
        public Vector2 _MainTexOffset;
        public Texture _RemapTex;
        public Vector2 _RemapTexScale;
        public Vector2 _RemapTexOffset;

        [Range(0f, 2f)]
        public float _SoftFactor;

        [Range(1f, 20f)]
        public float _BrightnessBoost;

        [Range(0f, 20f)]
        public float _AlphaBoost;

        [Range(0f, 1f)]
        public float _AlphaBias;

        [Range(0f, 1f)]
        public float _FadeCloseDistance;

        public enum _CullEnum
        {
            Off,
            Front,
            Back
        }
        public _CullEnum _Cull_Mode;

        public enum _ZTestEnum
        {
            Disabled,
            Never,
            Less,
            Equal,
            LessEqual,
            Greater,
            NotEqual,
            GreaterEqual,
            Always
        }
        public _ZTestEnum _ZTest_Mode;

        [Range(-10f, 10f)]
        public float _DepthOffset;

        [Range(-2f, 2f)]
        public float _DistortionStrength;

        public Texture _Cloud1Tex;
        public Vector2 _Cloud1TexScale;
        public Vector2 _Cloud1TexOffset;
        public Texture _Cloud2Tex;
        public Vector2 _Cloud2TexScale;
        public Vector2 _Cloud2TexOffset;
        public Vector4 _CutoffScroll;

        [Range(-20f, 20f)]
        public float _FresnelPower;

        [Range(0f, 3f)]
        public float _VertexOffsetAmount;

        public override void GrabMaterialValues()
        {
            if (Material)
            {
                shaderKeywords = Material.shaderKeywords;
                _Tint = Material.GetColor("_TintColor");
                _MainTex = Material.GetTexture("_MainTex");
                _MainTexScale = Material.GetTextureScale("_MainTex");
                _MainTexOffset = Material.GetTextureOffset("_MainTex");
                _RemapTex = Material.GetTexture("_RemapTex");
                _RemapTexScale = Material.GetTextureScale("_RemapTex");
                _RemapTexOffset = Material.GetTextureOffset("_RemapTex");
                _SoftFactor = Material.GetFloat("_InvFade");
                _BrightnessBoost = Material.GetFloat("_Boost");
                _AlphaBoost = Material.GetFloat("_AlphaBoost");
                _AlphaBias = Material.GetFloat("_AlphaBias");
                _FadeCloseDistance = Material.GetFloat("_FadeCloseDistance");
                _Cull_Mode = (_CullEnum)(int)Material.GetFloat("_Cull");
                _ZTest_Mode = (_ZTestEnum)(int)Material.GetFloat("_ZTest");
                _DepthOffset = Material.GetFloat("_DepthOffset");
                _DistortionStrength = Material.GetFloat("_DistortionStrength");
                _Cloud1Tex = Material.GetTexture("_Cloud1Tex");
                _Cloud1TexScale = Material.GetTextureScale("_Cloud1Tex");
                _Cloud1TexOffset = Material.GetTextureOffset("_Cloud1Tex");
                _Cloud2Tex = Material.GetTexture("_Cloud2Tex");
                _Cloud2TexScale = Material.GetTextureScale("_Cloud2Tex");
                _Cloud2TexOffset = Material.GetTextureOffset("_Cloud2Tex");
                _CutoffScroll = Material.GetVector("_CutoffScroll");
                _FresnelPower = Material.GetFloat("_FresnelPower");
                _VertexOffsetAmount = Material.GetFloat("_OffsetAmount");
                MaterialName = Material.name;
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
                Material.shaderKeywords = shaderKeywords;
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

                Material.SetFloat("_InvFade", _SoftFactor);
                Material.SetFloat("_Boost", _BrightnessBoost);
                Material.SetFloat("_AlphaBoost", _AlphaBoost);
                Material.SetFloat("_AlphaBias", _AlphaBias);
                Material.SetFloat("_FadeCloseDistance", _FadeCloseDistance);
                Material.SetFloat("_Cull", Convert.ToSingle(_Cull_Mode));
                Material.SetFloat("_ZTest", Convert.ToSingle(_ZTest_Mode));
                Material.SetFloat("_DepthOffset", _DepthOffset);
                Material.SetFloat("_DistortionStrength", _DistortionStrength);

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

                Material.SetVector("_CutoffScroll", _CutoffScroll);
                Material.SetFloat("_FresnelPower", _FresnelPower);
                Material.SetFloat("_OffsetAmount", _VertexOffsetAmount);
            }
        }
    }
}
