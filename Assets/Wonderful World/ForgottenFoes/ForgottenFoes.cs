using BepInEx;
using BepInEx.Configuration;
using EnigmaticThunder;
using ForgottenFoes.Utils;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using Path = System.IO.Path;


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
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ResourcesAPI), nameof(DirectorAPI), nameof(EffectAPI), nameof(PrefabAPI), nameof(LoadoutAPI))]

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

        internal static List<EnemyBuilder> enemies = new List<EnemyBuilder>();

        public void Awake()
        {
            LogCore.logger = Logger;

            Assets.PopulateAssets();
            Assets.ApplyShaders();

            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            LogCore.LogD("Adding Monsters...");
            var EnemyTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBuilder)));
            foreach (var enemyType in EnemyTypes)
            {
                EnemyBuilder enemy = (EnemyBuilder)Activator.CreateInstance(enemyType);

                LogCore.LogI(enemy);
                if (ValidateEnemy(enemy, enemies))
                {
                    enemy.Create(cfgFile);
                }
            }
            EnemyBuilder.AddToContentPack();

            LogCore.LogD("Adding Monsters Complete.");

            ContentPackProvider.Init();
        }

        internal bool ValidateEnemy(EnemyBuilder enemy, List<EnemyBuilder> enemyList)
        {
            var enabled = Config.Bind<bool>("Enemy: " + enemy.monsterName, "Enable Enemy?", true, "Should this enemy appear in runs?").Value;
            if (enabled)
            {
                enemyList.Add(enemy);
            }
            return enabled;
        }
    }


    public static class Assets
    {
        static bool imTooGodDamnLazyToSetUpConfigurationForThisSoJustSetThisToTrueIfAssetBundleIsntEmbeddedOkay = true;

        public static AssetBundle mainAssetBundle = null;

        public static void PopulateAssets()
        {
            if (imTooGodDamnLazyToSetUpConfigurationForThisSoJustSetThisToTrueIfAssetBundleIsntEmbeddedOkay)
            {
                var path = Assembly.GetExecutingAssembly().Location.Remove(Assembly.GetExecutingAssembly().Location.LastIndexOf('\\') + 1);
                mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(path, "forgottenfoes_assets"));
            }
            else
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ForgottenFoes.forgottenfoes_assets"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }
            ContentPackProvider.serializedContentPack = mainAssetBundle.LoadAsset<SerializableContentPack>("ContentPack");
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
                    case "matIndicator":
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Projectiles/ImpVoidspikeProjectile").transform.Find("ImpactEffect/TeamAreaIndicator, FullSphere/Mesh").GetComponent<MeshRenderer>().material);
                        break;
                    case "matIndicatorEnemy":
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Projectiles/ImpVoidspikeProjectile").transform.Find("ImpactEffect/TeamAreaIndicator, FullSphere").GetComponent<TeamAreaIndicator>().teamMaterialPairs[0].sharedMaterial);
                            break;
                    case "matIndicatorFriendly":
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Projectiles/ImpVoidspikeProjectile").transform.Find("ImpactEffect/TeamAreaIndicator, FullSphere").GetComponent<TeamAreaIndicator>().teamMaterialPairs[1].sharedMaterial);
                        break;
                    /*case "matVoidCluster":
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/ProjectileGhosts/ImpVoidspikeProjectileGhost").transform.Find("Mesh").GetComponent<MeshRenderer>().material);
                        break;*/

                }
            }
        }
    }

    public class ContentPackProvider : IContentPackProvider
    {
        public static SerializableContentPack serializedContentPack;
        public static ContentPack contentPack;

        public string identifier
        {
            get
            {
                return "ForgottenFoes";
            }
        }

        internal static void Init()
        {
            contentPack = serializedContentPack.CreateContentPack();
            ContentManager.collectContentPackProviders += AddCustomContent;
        }

        private static void AddCustomContent(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentPackProvider());
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
