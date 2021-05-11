using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForgottenFoes.Utils
{
    [CreateAssetMenu(fileName = "HGCloudRemapRemapper", menuName = "ForgottenFoes/HGCloudRemapRemapper", order = 3)]
    public class HGCloudRemapRemapper : ShaderRemapper
    {
        public Color _Tint;
        public bool _DisableRemapping;
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

        public bool _UseUV1;
        public bool _FadeWhenNearCamera;

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

        public bool _CloudRemapping;
        public bool _DistortionClouds;

        [Range(-2f, 2f)]
        public float _DistortionStrength;

        public Texture _Cloud1Tex;
        public Vector2 _Cloud1TexScale;
        public Vector2 _Cloud1TexOffset;
        public Texture _Cloud2Tex;
        public Vector2 _Cloud2TexScale;
        public Vector2 _Cloud2TexOffset;
        public Vector4 _CutoffScroll;
        public bool _VertexColors;
        public bool _LuminanceForVertexAlpha;
        public bool _LuminanceForTextureAlpha;
        public bool _VertexOffset;
        public bool _FresnelFade;
        public bool _SkyboxOnly;

        [Range(-20f, 20f)]
        public float _FresnelPower;

        [Range(0f, 3f)]
        public float _VertexOffsetAmount;

        public override void UpdateMaterial()
        {

            if (Material)
            {
                Material.shader = Resources.Load<Shader>("shaders/fx/hgcloudremap");
                Material.shaderKeywords = shaderKeywords;
                Material.SetColor("_TintColor", _Tint);
                Material.SetFloat("_DisableRemapOn", Convert.ToSingle(_DisableRemapping));

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
                Material.SetFloat("_UseUV1On", Convert.ToSingle(_UseUV1));
                Material.SetFloat("_FadeCloseOn", Convert.ToSingle(_FadeWhenNearCamera));
                Material.SetFloat("_FadeCloseDistance", _FadeCloseDistance);
                Material.SetFloat("_Cull", Convert.ToSingle(_Cull_Mode));
                Material.SetFloat("_ZTest", Convert.ToSingle(_ZTest_Mode));
                Material.SetFloat("_DepthOffset", _DepthOffset);
                Material.SetFloat("_CloudsOn", Convert.ToSingle(_CloudRemapping));
                Material.SetFloat("_CloudOffsetOn", Convert.ToSingle(_DistortionClouds));
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
                Material.SetFloat("_VertexColorOn", Convert.ToSingle(_VertexColors));
                Material.SetFloat("_VertexAlphaOn", Convert.ToSingle(_LuminanceForVertexAlpha));
                Material.SetFloat("_CalcTextureAlphaOn", Convert.ToSingle(_LuminanceForTextureAlpha));
                Material.SetFloat("_VertexOffsetOn", Convert.ToSingle(_VertexOffset));
                Material.SetFloat("_FresnelOn", Convert.ToSingle(_FresnelFade));
                Material.SetFloat("_SkyboxOnly", Convert.ToSingle(_SkyboxOnly));
                Material.SetFloat("_FresnelPower", _FresnelPower);
                Material.SetFloat("_OffsetAmount", _VertexOffsetAmount);
            }
        }
    }
}