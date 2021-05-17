using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShaderCustomizationKit
{
    public class HGStandardController : HGShaders
    {
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

        [Range(0f, 1f)]
        public float _FadeBias;


        public Texture _FresnelRamp;

        [Range(0.1f, 20f)]
        public float _FresnelPower;

        public Texture _FresnelMask;

        [Range(0f, 20f)]
        public float _FresnelBoost;

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

        public override void GrabMaterialValues()
        {
            if (Material)
            {
                shaderKeywords = Material.shaderKeywords;
                _Color = Material.GetColor("_Color");
                _MainTex = Material.GetTexture("_MainTex");
                _MainTexScale = Material.GetTextureScale("_MainTex");
                _MainTexOffset = Material.GetTextureOffset("_MainTex");
                _NormalStrength = Material.GetFloat("_NormalStrength");
                _NormalTex = Material.GetTexture("_NormalTex");
                _NormalTexScale = Material.GetTextureScale("_NormalTex");
                _NormalTexOffset = Material.GetTextureOffset("_NormalTex");
                _EmColor = Material.GetColor("_EmColor");
                _EmTex = Material.GetTexture("_EmTex");
                _EmPower = Material.GetFloat("_EmPower");
                _Smoothness = Material.GetFloat("_Smoothness");
                _RampChoice = (_RampInfoEnum)(int)Material.GetFloat("_RampInfo");
                _DecalLayer = (_DecalLayerEnum)(int)Material.GetFloat("_DecalLayer");
                _SpecularStrength = Material.GetFloat("_SpecularStrength");
                _SpecularExponent = Material.GetFloat("_SpecularExponent");
                _Cull_Mode = (_CullEnum)(int)Material.GetFloat("_Cull");
                _FadeBias = Material.GetFloat("_FadeBias");
                _FresnelRamp = Material.GetTexture("_FresnelRamp");
                _FresnelPower = Material.GetFloat("_FresnelPower");
                _FresnelMask = Material.GetTexture("_FresnelMask");
                _FresnelBoost = Material.GetFloat("_FresnelBoost");
                _SliceHeight = Material.GetFloat("_SliceHeight");
                _PrintBandHeight = Material.GetFloat("_SliceBandHeight");
                _PrintAlphaDepth = Material.GetFloat("_SliceAlphaDepth");
                _PrintAlphaTexture = Material.GetTexture("_SliceAlphaTex");
                _PrintAlphaTextureScale = Material.GetTextureScale("_SliceAlphaTex");
                _PrintAlphaTextureOffset = Material.GetTextureOffset("_SliceAlphaTex");
                _PrintColorBoost = Material.GetFloat("_PrintBoost");
                _PrintAlphaBias = Material.GetFloat("_PrintBias");
                _PrintEmissionToAlbedoLerp = Material.GetFloat("_PrintEmissionToAlbedoLerp");
                _PrintDirection = (_PrintDirectionEnum)(int)Material.GetFloat("_PrintDirection");
                _PrintRamp = Material.GetTexture("_PrintRamp");
                _EliteBrightnessMin = Material.GetFloat("_EliteBrightnessMin");
                _EliteBrightnessMax = Material.GetFloat("_EliteBrightnessMax");
                _BlendDepth = Material.GetFloat("_Depth");
                _SplatmapTex = Material.GetTexture("_SplatmapTex");
                _SplatmapTexScale = Material.GetTextureScale("_SplatmapTex");
                _SplatmapTexOffset = Material.GetTextureOffset("_SplatmapTex");
                _SplatmapTileScale = Material.GetFloat("_SplatmapTileScale");
                _GreenChannelTex = Material.GetTexture("_GreenChannelTex");
                _GreenChannelNormalTex = Material.GetTexture("_GreenChannelNormalTex");
                _GreenChannelSmoothness = Material.GetFloat("_GreenChannelSmoothness");
                _GreenChannelBias = Material.GetFloat("_GreenChannelBias");
                _BlueChannelTex = Material.GetTexture("_BlueChannelTex");
                _BlueChannelNormalTex = Material.GetTexture("_BlueChannelNormalTex");
                _BlueChannelSmoothness = Material.GetFloat("_BlueChannelSmoothness");
                _BlueChannelBias = Material.GetFloat("_BlueChannelBias");
                _FlowTexture = Material.GetTexture("_FlowTex");
                _FlowHeightmap = Material.GetTexture("_FlowHeightmap");
                _FlowHeightmapScale = Material.GetTextureScale("_FlowHeightmap");
                _FlowHeightmapOffset = Material.GetTextureOffset("_FlowHeightmap");
                _FlowHeightRamp = Material.GetTexture("_FlowHeightRamp");
                _FlowHeightRampScale = Material.GetTextureScale("_FlowHeightRamp");
                _FlowHeightRampOffset = Material.GetTextureOffset("_FlowHeightRamp");
                _FlowHeightBias = Material.GetFloat("_FlowHeightBias");
                _FlowHeightPower = Material.GetFloat("_FlowHeightPower");
                _FlowEmissionStrength = Material.GetFloat("_FlowEmissionStrength");
                _FlowSpeed = Material.GetFloat("_FlowSpeed");
                _MaskFlowStrength = Material.GetFloat("_FlowMaskStrength");
                _NormalFlowStrength = Material.GetFloat("_FlowNormalStrength");
                _FlowTextureScaleFactor = Material.GetFloat("_FlowTextureScaleFactor");
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
                Material.SetColor("_Color", _Color);

                if (_MainTex)
                {
                    Material.SetTexture("_MainTex", _MainTex);
                    Material.SetTextureScale("_MainTex", _MainTexScale);
                    Material.SetTextureOffset("_MainTex", _MainTexOffset);
                }
                else
                    Material.SetTexture("_MainTex", null);

                Material.SetFloat("_NormalStrength", _NormalStrength);

                if (_NormalTex)
                {
                    Material.SetTexture("_NormalTex", _NormalTex);
                    Material.SetTextureScale("_NormalTex", _NormalTexScale);
                    Material.SetTextureOffset("_NormalTex", _NormalTexOffset);
                }
                else
                    Material.SetTexture("_NormalTex", null);

                Material.SetColor("_EmColor", _EmColor);

                if (_EmTex)
                    Material.SetTexture("_EmTex", _EmTex);
                else
                    Material.SetTexture("_EmTex", null);

                Material.SetFloat("_EmPower", _EmPower);
                Material.SetFloat("_Smoothness", _Smoothness);
                Material.SetFloat("_RampInfo", Convert.ToSingle(_RampChoice));
                Material.SetFloat("_DecalLayer", Convert.ToSingle(_DecalLayer));
                Material.SetFloat("_SpecularStrength", _SpecularStrength);
                Material.SetFloat("_SpecularExponent", _SpecularExponent);
                Material.SetFloat("_Cull", Convert.ToSingle(_Cull_Mode));
                Material.SetFloat("_FadeBias", _FadeBias);

                if (_FresnelRamp)
                    Material.SetTexture("_FresnelRamp", _FresnelRamp);
                else
                    Material.SetTexture("_FresnelRamp", null);

                Material.SetFloat("_FresnelPower", _FresnelPower);

                if (_FresnelMask)
                    Material.SetTexture("_FresnelMask", _FresnelMask);
                else
                    Material.SetTexture("_FresnelMask", null);

                Material.SetFloat("_FresnelBoost", _FresnelBoost);
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
                    Material.SetTexture("_SliceAlphaTex", null);

                Material.SetFloat("_PrintBoost", _PrintColorBoost);
                Material.SetFloat("_PrintBias", _PrintAlphaBias);
                Material.SetFloat("_PrintEmissionToAlbedoLerp", _PrintEmissionToAlbedoLerp);
                Material.SetFloat("_PrintDirection", Convert.ToSingle(_PrintDirection));

                if (_PrintRamp)
                    Material.SetTexture("_PrintRamp", _PrintRamp);
                else
                    Material.SetTexture("_PrintRamp", null);

                Material.SetFloat("_EliteBrightnessMin", _EliteBrightnessMin);
                Material.SetFloat("_EliteBrightnessMax", _EliteBrightnessMax);
                Material.SetFloat("_Depth", _BlendDepth);

                if (_SplatmapTex)
                {
                    Material.SetTexture("_SplatmapTex", _SplatmapTex);
                    Material.SetTextureScale("_SplatmapTex", _SplatmapTexScale);
                    Material.SetTextureOffset("_SplatmapTex", _SplatmapTexOffset);
                }
                else
                    Material.SetTexture("_SplatmapTex", null);

                Material.SetFloat("_SplatmapTileScale", _SplatmapTileScale);

                if (_GreenChannelTex)
                    Material.SetTexture("_GreenChannelTex", _GreenChannelTex);
                else
                    Material.SetTexture("_GreenChannelTex", null);

                if (_GreenChannelNormalTex)
                    Material.SetTexture("_GreenChannelNormalTex", _GreenChannelNormalTex);
                else
                    Material.SetTexture("_GreenChannelNormalTex", null);

                Material.SetFloat("_GreenChannelSmoothness", _GreenChannelSmoothness);
                Material.SetFloat("_GreenChannelBias", _GreenChannelBias);

                if (_BlueChannelTex)
                    Material.SetTexture("_BlueChannelTex", _BlueChannelTex);
                else
                    Material.SetTexture("_BlueChannelTex", null);

                if (_BlueChannelNormalTex)
                    Material.SetTexture("_BlueChannelNormalTex", _BlueChannelNormalTex);
                else
                    Material.SetTexture("_BlueChannelNormalTex", null);

                Material.SetFloat("_BlueChannelSmoothness", _BlueChannelSmoothness);
                Material.SetFloat("_BlueChannelBias", _BlueChannelBias);


                if (_FlowTexture)
                    Material.SetTexture("_FlowTex", _FlowTexture);
                else
                    Material.SetTexture("_FlowTex", null);

                if (_FlowHeightmap)
                {
                    Material.SetTexture("_FlowHeightmap", _FlowHeightmap);
                    Material.SetTextureScale("_FlowHeightmap", _FlowHeightmapScale);
                    Material.SetTextureOffset("_FlowHeightmap", _FlowHeightmapOffset);
                }
                else
                    Material.SetTexture("_FlowHeightmap", null);

                if (_FlowHeightRamp)
                {
                    Material.SetTexture("_FlowHeightRamp", _FlowHeightRamp);
                    Material.SetTextureScale("_FlowHeightRamp", _FlowHeightRampScale);
                    Material.SetTextureOffset("_FlowHeightRamp", _FlowHeightRampOffset);
                }
                else
                    Material.SetTexture("_FlowHeightRamp", null);

                Material.SetFloat("_FlowHeightBias", _FlowHeightBias);
                Material.SetFloat("_FlowHeightPower", _FlowHeightPower);
                Material.SetFloat("_FlowEmissionStrength", _FlowEmissionStrength);
                Material.SetFloat("_FlowSpeed", _FlowSpeed);
                Material.SetFloat("_FlowMaskStrength", _MaskFlowStrength);
                Material.SetFloat("_FlowNormalStrength", _NormalFlowStrength);
                Material.SetFloat("_FlowTextureScaleFactor", _FlowTextureScaleFactor);
            }
        }

    }
}
