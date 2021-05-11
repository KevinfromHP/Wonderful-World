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

namespace ForgottenFoes
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;

        public static void PopulateAssets()
        {
#if DEBUG
            var path = Assembly.GetExecutingAssembly().Location.Remove(Assembly.GetExecutingAssembly().Location.LastIndexOf('\\') + 1);
            mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(path, "forgottenfoes_assets"));
#else
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ForgottenFoes.forgottenfoes_assets"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
#endif
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
                        material.shader = Resources.Load<Shader>("shaders/deferred/hgstandard");
                        Material impMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/ImpBody").transform.Find("ModelBase/mdlImp/ImpMesh").GetComponent<SkinnedMeshRenderer>().material;
                        material.CopyPropertiesFromMaterial(impMat);
                        material.mainTexture = mainAssetBundle.LoadAsset<Texture2D>("texImpSorcerer");
                        material.SetTexture("_EmTex", mainAssetBundle.LoadAsset<Texture2D>("texImpSorcererEmission"));
                        material.SetTexture("_NormalTex", mainAssetBundle.LoadAsset<Texture2D>("texImpSorcererNormal"));
                        break;
                    /*case "matIndicatorEnemy":
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Projectiles/TitanPreFistProjectile").transform.Find("ImpactEffect/TeamAreaIndicator, GroundOnly").GetComponent<TeamAreaIndicator>().teamMaterialPairs[0].sharedMaterial);
                        break;
                    case "matIndicatorFriendly":
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Projectiles/TitanPreFistProjectile").transform.Find("ImpactEffect/TeamAreaIndicator, GroundOnly").GetComponent<TeamAreaIndicator>().teamMaterialPairs[1].sharedMaterial);
                        break;*/
                    case "matVoidCluster":
                        material.shader = Resources.Load<Shader>("shaders/fx/hgcloudremap");
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/ProjectileGhosts/ImpVoidspikeProjectileGhost").transform.Find("Mesh").GetComponent<MeshRenderer>().material);
                        break;
                    case "matPortal":
                        material.shader = Resources.Load<Shader>("shaders/fx/hgcloudremap");
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/effects/ImpBossDeathEffect").transform.Find("Ring").GetComponent<ParticleSystemRenderer>().material);
                        material.SetTexture("_MainTex", mainAssetBundle.LoadAsset<Texture2D>("texPortal"));
                        break;
                    case "matPortalCrack":
                        material.shader = Resources.Load<Shader>("shaders/fx/hgcloudremap");
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Projectileghosts/ImpVoidspikeProjectileGhost").transform.Find("Mesh").GetComponent<MeshRenderer>().material);
                        foreach (string keyword in material.shaderKeywords)
                            LogCore.LogE(keyword);
                        break;
                    case "matCrystal":
                        material.shader = Resources.Load<Shader>("shaders/deferred/hgstandard");
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/pickupmodels/PickupDiamond").transform.Find("TonicCap").GetComponent<MeshRenderer>().material);
                        break;
                    case "matPortalParticles":
                        material.shader = Resources.Load<Shader>("shaders/fx/hgcloudremap");
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Effects/ImpBossBlink").transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().trailMaterial);
                        break;
                    case "matPortalParticlesBright":
                        material.shader = Resources.Load<Shader>("shaders/fx/hgcloudremap");
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Effects/ImpBossBlink").transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material);
                        break;
                    case "matPortalDistortion":
                        material.shader = Resources.Load<Shader>("shaders/fx/hgdistortion");
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Effects/ImpBossBlink").transform.Find("Particles/Distortion").GetComponent<ParticleSystemRenderer>().material);
                        material.SetFloat("_Magnitude", 0.1f);
                        break;
                    case "matPortalExplosion":
                        material.shader = Resources.Load<Shader>("shaders/fx/hgintersectioncloudremap");
                        material.CopyPropertiesFromMaterial(Resources.Load<GameObject>("Prefabs/Effects/ImpBossBlink").transform.Find("Particles/Sphere").GetComponent<ParticleSystemRenderer>().material);
                        break;
                }
            }
            var remappers = Assets.mainAssetBundle.LoadAllAssets<ShaderRemapper>();
            for (int i = 0; i < remappers.Length; i++)
                remappers[i].UpdateMaterial();
        }
        /// <summary>
        /// This is only here because EffectDefs can't be made through Unity due to Hopoo forgetting to make them scriptable/serializable. You can delete this retarded shit once it's patched.
        /// </summary>
        public static void AddEffectDefs()
        {
            var effectHolders = Assets.mainAssetBundle.LoadAllAssets<ForgottenFoesEffectDefHolder>();
            var effectDefs = new List<EffectDef>();
            for (int i = 0; i < effectHolders.Length; i++)
            {
                foreach (GameObject effectPrefab in effectHolders[i].effectPrefabs)
                    effectDefs.Add(ForgottenFoesEffectDefHolder.ToEffectDef(effectPrefab));
            }
            ContentPackProvider.serializedContentPack.effectDefs = effectDefs.ToArray();
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
