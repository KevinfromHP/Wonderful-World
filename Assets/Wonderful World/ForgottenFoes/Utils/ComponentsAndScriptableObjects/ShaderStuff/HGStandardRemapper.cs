using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForgottenFoes.Utils
{
    [CreateAssetMenu(fileName = "HGStandardRemapper", menuName = "ForgottenFoes/HGStandardRemapper", order = 2)]
    public class HGStandardRemapper : ShaderRemapper
    {
        public bool _EnableCutout;
        public Color _Color;
        public Texture _MainTex;
        public Vector2 _MainTexScale;
        public Vector2 _MainTexOffset;

        [Range(0f, 5f)]
        public float _NormalStrength;

        public Texture _NormalTex;
        public Vector2 _NormalTexScale;
        public Vector2 _NormalTexOffset;
        public Color _EmColor;
        public Texture _EmTex;

        [Range(0f, 10f)]
        public float _EmPower;

        [Range(0f, 1f)]
        public float _Smoothness;

        public bool _IgnoreDiffuseAlphaForSpeculars;

        public enum _RampInfoEnum
        {
            TwoTone,
            SmoothedTwoTone,
            Unlitish,
            Subsurface,
            Grass
        }
        public _RampInfoEnum _RampChoice;

        public enum _DecalLayerEnum
        {
            Default,
            Environment,
            Character,
            Misc
        }
        public _DecalLayerEnum _DecalLayer;

        [Range(0f, 1f)]
        public float _SpecularStrength;

        [Range(0.1f, 20f)]
        public float _SpecularExponent;

        public enum _CullEnum
        {
            Off,
            Front,
            Back
        }
        public _CullEnum _Cull_Mode;

        public bool _EnableDither;

        [Range(0f, 1f)]
        public float _FadeBias;

        public bool _EnableFresnelEmission;

        public Texture _FresnelRamp;

        [Range(0.1f, 20f)]
        public float _FresnelPower;

        public Texture _FresnelMask;

        [Range(0f, 20f)]
        public float _FresnelBoost;

        public bool _EnablePrinting;

        [Range(-25f, 25f)]
        public float _SliceHeight;

        [Range(0f, 10f)]
        public float _PrintBandHeight;

        [Range(0f, 1f)]
        public float _PrintAlphaDepth;

        public Texture _PrintAlphaTexture;
        public Vector2 _PrintAlphaTextureScale;
        public Vector2 _PrintAlphaTextureOffset;

        [Range(0f, 10f)]
        public float _PrintColorBoost;

        [Range(0f, 4f)]
        public float _PrintAlphaBias;

        [Range(0f, 1f)]
        public float _PrintEmissionToAlbedoLerp;

        public enum _PrintDirectionEnum
        {
            BottomUp,
            TopDown,
            BackToFront
        }
        public _PrintDirectionEnum _PrintDirection;

        public Texture _PrintRamp;

        [Range(-10f, 10f)]
        public float _EliteBrightnessMin;

        [Range(-10f, 10f)]
        public float _EliteBrightnessMax;

        public bool _EnableSplatmap;
        public bool _UseVertexColorsInstead;

        [Range(0f, 1f)]
        public float _BlendDepth;

        public Texture _SplatmapTex;
        public Vector2 _SplatmapTexScale;
        public Vector2 _SplatmapTexOffset;

        [Range(0f, 20f)]
        public float _SplatmapTileScale;

        public Texture _GreenChannelTex;
        public Texture _GreenChannelNormalTex;

        [Range(0f, 1f)]
        public float _GreenChannelSmoothness;

        [Range(-2f, 5f)]
        public float _GreenChannelBias;

        public Texture _BlueChannelTex;
        public Texture _BlueChannelNormalTex;

        [Range(0f, 1f)]
        public float _BlueChannelSmoothness;

        [Range(-2f, 5f)]
        public float _BlueChannelBias;

        public bool _EnableFlowmap;
        public Texture _FlowTexture;
        public Texture _FlowHeightmap;
        public Vector2 _FlowHeightmapScale;
        public Vector2 _FlowHeightmapOffset;
        public Texture _FlowHeightRamp;
        public Vector2 _FlowHeightRampScale;
        public Vector2 _FlowHeightRampOffset;

        [Range(-1f, 1f)]
        public float _FlowHeightBias;

        [Range(0.1f, 20f)]
        public float _FlowHeightPower;

        [Range(0.1f, 20f)]
        public float _FlowEmissionStrength;

        [Range(0f, 15f)]
        public float _FlowSpeed;

        [Range(0f, 5f)]
        public float _MaskFlowStrength;

        [Range(0f, 5f)]
        public float _NormalFlowStrength;

        [Range(0f, 10f)]
        public float _FlowTextureScaleFactor;

        public bool _EnableLimbRemoval;

        public void PutMaterialIntoRenderer(Renderer meshRenderer)
        {
            if (Material && meshRenderer)
            {
                meshRenderer.material = Material;
            }
        }

        public void UpdateMaterial()
        {
            if (Material)
            {
                Material.shader = Resources.Load<Shader>("shaders/deferred/hgstandard");
                Material.SetFloat("_EnableCutout", Convert.ToSingle(_EnableCutout));
                Material.SetColor("_Color", _Color);

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

                Material.SetFloat("_NormalStrength", _NormalStrength);

                if (_NormalTex)
                {
                    Material.SetTexture("_NormalTex", _NormalTex);
                    Material.SetTextureScale("_NormalTex", _NormalTexScale);
                    Material.SetTextureOffset("_NormalTex", _NormalTexOffset);
                }
                else
                {
                    Material.SetTexture("_NormalTex", null);
                }

                Material.SetColor("_EmColor", _EmColor);

                if (_EmTex)
                {
                    Material.SetTexture("_EmTex", _EmTex);
                }
                else
                {
                    Material.SetTexture("_EmTex", null);
                }

                Material.SetFloat("_EmPower", _EmPower);
                Material.SetFloat("_Smoothness", _Smoothness);
                Material.SetFloat("_ForceSpecOn", Convert.ToSingle(_IgnoreDiffuseAlphaForSpeculars));
                Material.SetFloat("_RampInfo", Convert.ToSingle(_RampChoice));
                Material.SetFloat("_DecalLayer", Convert.ToSingle(_DecalLayer));
                Material.SetFloat("_SpecularStrength", _SpecularStrength);
                Material.SetFloat("_SpecularExponent", _SpecularExponent);
                Material.SetFloat("_Cull", Convert.ToSingle(_Cull_Mode));
                Material.SetFloat("_DitherOn", Convert.ToSingle(_EnableDither));
                Material.SetFloat("_FadeBias", _FadeBias);
                Material.SetFloat("_FEON", Convert.ToSingle(_EnableFresnelEmission));

                if (_FresnelRamp)
                {
                    Material.SetTexture("_FresnelRamp", _FresnelRamp);
                }
                else
                {
                    Material.SetTexture("_FresnelRamp", null);
                }

                Material.SetFloat("_FresnelPower", _FresnelPower);

                if (_FresnelMask)
                {
                    Material.SetTexture("_FresnelMask", _FresnelMask);
                }
                else
                {
                    Material.SetTexture("_FresnelMask", null);
                }

                Material.SetFloat("_FresnelBoost", _FresnelBoost);
                Material.SetFloat("_PrintOn", Convert.ToSingle(_EnablePrinting));
                Material.SetFloat("_SliceHeight", _SliceHeight);
                Material.SetFloat("_SliceBandHeight", _PrintBandHeight);
                Material.SetFloat("_SliceAlphaDepth", _PrintAlphaDepth);

                if (_PrintAlphaTexture)
                {
                    Material.SetTexture("_SliceAlphaTex", _PrintAlphaTexture);
                    Material.SetTextureScale("_SliceAlphaTex", _PrintAlphaTextureScale);
                    Material.SetTextureOffset("_SliceAlphaTex", _PrintAlphaTextureOffset);
                }
                else
                {
                    Material.SetTexture("_SliceAlphaTex", null);
                }

                Material.SetFloat("_PrintBoost", _PrintColorBoost);
                Material.SetFloat("_PrintBias", _PrintAlphaBias);
                Material.SetFloat("_PrintEmissionToAlbedoLerp", _PrintEmissionToAlbedoLerp);
                Material.SetFloat("_PrintDirection", Convert.ToSingle(_PrintDirection));

                if (_PrintRamp)
                {
                    Material.SetTexture("_PrintRamp", _PrintRamp);
                }
                else
                {
                    Material.SetTexture("_PrintRamp", null);
                }

                Material.SetFloat("_EliteBrightnessMin", _EliteBrightnessMin);
                Material.SetFloat("_EliteBrightnessMax", _EliteBrightnessMax);
                Material.SetFloat("_SplatmapOn", Convert.ToSingle(_EnableSplatmap));
                Material.SetFloat("_ColorsOn", Convert.ToSingle(_UseVertexColorsInstead));
                Material.SetFloat("_Depth", _BlendDepth);

                if (_SplatmapTex)
                {
                    Material.SetTexture("_SplatmapTex", _SplatmapTex);
                    Material.SetTextureScale("_SplatmapTex", _SplatmapTexScale);
                    Material.SetTextureOffset("_SplatmapTex", _SplatmapTexOffset);
                }
                else
                {
                    Material.SetTexture("_SplatmapTex", null);
                }

                Material.SetFloat("_SplatmapTileScale", _SplatmapTileScale);

                if (_GreenChannelTex)
                {
                    Material.SetTexture("_GreenChannelTex", _GreenChannelTex);
                }
                else
                {
                    Material.SetTexture("_GreenChannelTex", null);
                }

                if (_GreenChannelNormalTex)
                {
                    Material.SetTexture("_GreenChannelNormalTex", _GreenChannelNormalTex);
                }
                else
                {
                    Material.SetTexture("_GreenChannelNormalTex", null);
                }

                Material.SetFloat("_GreenChannelSmoothness", _GreenChannelSmoothness);
                Material.SetFloat("_GreenChannelBias", _GreenChannelBias);

                if (_BlueChannelTex)
                {
                    Material.SetTexture("_BlueChannelTex", _BlueChannelTex);
                }
                else
                {
                    Material.SetTexture("_BlueChannelTex", null);
                }

                if (_BlueChannelNormalTex)
                {
                    Material.SetTexture("_BlueChannelNormalTex", _BlueChannelNormalTex);
                }
                else
                {
                    Material.SetTexture("_BlueChannelNormalTex", null);
                }

                Material.SetFloat("_BlueChannelSmoothness", _BlueChannelSmoothness);
                Material.SetFloat("_BlueChannelBias", _BlueChannelBias);

                Material.SetFloat("_FlowmapOn", Convert.ToSingle(_EnableFlowmap));

                if (_FlowTexture)
                {
                    Material.SetTexture("_FlowTex", _FlowTexture);
                }
                else
                {
                    Material.SetTexture("_FlowTex", null);
                }

                if (_FlowHeightmap)
                {
                    Material.SetTexture("_FlowHeightmap", _FlowHeightmap);
                    Material.SetTextureScale("_FlowHeightmap", _FlowHeightmapScale);
                    Material.SetTextureOffset("_FlowHeightmap", _FlowHeightmapOffset);
                }
                else
                {
                    Material.SetTexture("_FlowHeightmap", null);
                }

                if (_FlowHeightRamp)
                {
                    Material.SetTexture("_FlowHeightRamp", _FlowHeightRamp);
                    Material.SetTextureScale("_FlowHeightRamp", _FlowHeightRampScale);
                    Material.SetTextureOffset("_FlowHeightRamp", _FlowHeightRampOffset);
                }
                else
                {
                    Material.SetTexture("_FlowHeightRamp", null);
                }

                Material.SetFloat("_FlowHeightBias", _FlowHeightBias);
                Material.SetFloat("_FlowHeightPower", _FlowHeightPower);
                Material.SetFloat("_FlowEmissionStrength", _FlowEmissionStrength);
                Material.SetFloat("_FlowSpeed", _FlowSpeed);
                Material.SetFloat("_FlowMaskStrength", _MaskFlowStrength);
                Material.SetFloat("_FlowNormalStrength", _NormalFlowStrength);
                Material.SetFloat("_FlowTextureScaleFactor", _FlowTextureScaleFactor);
                Material.SetFloat("_LimbRemovalOn", Convert.ToSingle(_EnableLimbRemoval));
            }
        }

    }
}