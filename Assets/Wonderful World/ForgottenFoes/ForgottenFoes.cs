using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
//using TILER2;
using UnityEngine;
//using static TILER2.MiscUtil;
using Path = System.IO.Path;
using System.Security;
using System.Security.Permissions;
using System.Linq;
using EnigmaticThunder;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

/* TO DO:
 * Add Logbook support
 * Flesh out config options for the boilerplate
 * Fuck with entity states
 */
namespace ForgottenFoes
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(EnigmaticThunderPlugin.guid, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ResourcesAPI), nameof(DirectorAPI), nameof(EffectAPI), nameof(PrefabAPI))]

    public class ForgottenFoes : BaseUnityPlugin
    {
        public const string ModVer =
#if DEBUG
                "0." +
#endif
            "1.0.0";
        public const string ModName = "ForgottenFoes";
        public const string ModGuid = "com.WonderWorld.ForgottenFoes";


        private static ConfigFile cfgFile;

        internal static List<EnemyBuilderNew> enemies = new List<EnemyBuilderNew>();

        public void Awake()
        {
            LogCore.logger = Logger;

            Assets.PopulateAssets();
            Assets.ApplyShaders();

            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            LogCore.LogD("Adding Monsters...");
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBuilderNew)));
            foreach (var itemType in ItemTypes)
            {
                EnemyBuilderNew item = (EnemyBuilderNew)System.Activator.CreateInstance(itemType);

                LogCore.LogI(item);
                if (ValidateEnemy(item, enemies))
                {
                    item.Create(cfgFile);
                }
            }
            LogCore.LogD("Adding Monsters Complete.");
        }

        internal bool ValidateEnemy(EnemyBuilderNew item, List<EnemyBuilderNew> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.monsterName, "Enable Item?", true, "Should this item appear in runs?").Value;
            if (enabled)
            {
                itemList.Add(item);
            }
            return enabled;
        }
    }
}

public static class Assets
{
    public static AssetBundle mainAssetBundle = null;
    public static AssetBundleResourcesProvider Provider;

    public static void PopulateAssets()
    {
        using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ForgottenFoes.forgottenfoes_assets"))
        {
            mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
        }
    }

    public static void ApplyShaders()
    {
        var materials = mainAssetBundle.LoadAllAssets<Material>();

        foreach (Material material in materials)
        {
            switch (material.name)
            {
                case "matImpSorcerer":
                    Material impMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/ImpBody").transform.Find("ModelBase/mdlImp/ImpMesh").GetComponent<SkinnedMeshRenderer>().material;
                    material.CopyPropertiesFromMaterial(impMat);
                    material.mainTexture = mainAssetBundle.LoadAsset<Texture2D>("texImpSorcerer");

                    //material.SetTexture("_EmTex", null);
                    material.SetTexture("_SliceAlphaText", mainAssetBundle.LoadAsset<Texture2D>("texCloudOrganic2"));

                    /*material.shader = Resources.Load<Shader>("shaders/deferred/hgstandard");
                    string[] texProperties = new string[] { "_FlowHeightRamp", "_FlowHeightMap", "_FlowTex", "_FresnelRamp", "_PrintRamp", "_SliceAlphaTest" };
                    foreach (string property in texProperties)
                        material.SetTexture(property, impMat.GetTexture(property));

                    //impMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/ImpBody").transform.Find("ModelBase/mdlImp/ImpMesh").GetComponent<SkinnedMeshRenderer>().material;
                    string[] floatProperties = new string[] { "_BlueChannelBias", "_BlueChannelSmoothness", "_BumpScale", "_ColorsOn", "_Cull", "_Cutoff", "_DecalLayer", "_Depth", "_DetailNormalMapScale", "_DiffuseBias", "_DiffuseExponent", "_DiffuseScale", "_DitherOn", "_DstBlend", "_EliteBrightnessMax", "_EliteBrightnessMin", "_EliteIndex", "_EmPower", "_EnableCutout", "_FEON", "_Fade", "_FadeBias", "_FlowDiffuseStrength", "_FlowEmissionStrength", "_FlowHeightBias", "_FlowHeightPower", "_FlowMaskStrength", "_FlowNormalStrength", "_FlowSpeed", "_FlowTextureScaleFactor", "_FlowmapOn", "_ForceSpecOn", "_FresnelBoost", "_FresnelPower", "_GlossMapScale", "_Glossiness", "_GlossyReflections", "_GreenChannelBias", "_GreenChannelSmoothness", "_LimbPrimeMask", "_LimbRemovalOn", "_Metallic", "_Mode", "_NormalStrength", "_OcclusionStrength", "_Parallax", "_PrintBias", "_PrintBoost", "_PrintDirection", "_PrintEmissionToAlbedoLerp", "_PrintOn", "_RampInfo", "_RimPower", "_RimStrength", "_SliceAlphaDepth", "_SliceBandHeight", "_SliceHeight", "_Smoothness", "_SmoothnessTextureChannel", "_SpecularExponent", "_SpecularHighlights", "_SpecularStrength", "_SplatmapOn", "_SplatmapTileScale", "_SrcBlend", "_UVSec", "_ZWrite" };
                    foreach (string property in floatProperties)
                            material.SetFloat(property, impMat.GetFloat(property));*/
                    material.SetFloat("_LimbRemoval", 1);
                    material.SetFloat("_PrintDirection", 0f);
                    material.SetFloat("_SliceBandHeight", 1f);
                    material.SetFloat("_SliceHeight", 1f);
                    break;
            }
        }
    }
}
