using BepInEx;
using BepInEx.Configuration;
using EnigmaticThunder;
using TrinketsAndBaubles.Utils;
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
using MonoMod.Cil;
using Mono.Cecil.Cil;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace TrinketsAndBaubles
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(EnigmaticThunderPlugin.guid, BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ResourcesAPI), nameof(DirectorAPI), nameof(EffectAPI), nameof(PrefabAPI), nameof(LoadoutAPI))]

    public class TrinketsAndBaubles : BaseUnityPlugin
    {
        public const string ModVer =
#if DEBUG
                "0." +
#endif
            "1.0.0";
        public const string ModName = "TrinketsAndBaubles";
        public const string ModGuid = "com.WonderWorld.TrinketsAndBaubles";


        private static ConfigFile cfgFile;

        internal static List<ItemBuilder> itemList = new List<ItemBuilder>();
        internal static List<EquipBuilder> equipList = new List<EquipBuilder>();

        public void Awake()
        {
            LogCore.logger = Logger;

            Assets.PopulateAssets();

            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            LogCore.LogD("Adding Items...");

            /*var EnemyTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBuilderNew)));
            foreach (var enemyType in EnemyTypes)
            {
                EnemyBuilderNew item = (EnemyBuilderNew)System.Activator.CreateInstance(enemyType);

                LogCore.LogI(item);
                if (ValidateEnemy(item, enemies))
                {
                    item.Create(cfgFile);
                }
            }*/

            //IL.RoR2.EquipmentSlot.PerformEquipmentAction += EquipBuilder.FireEquipment;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentHandler.FireEquipment;

            LogCore.LogD("Adding Items Complete.");
        }


        internal bool ValidateItem(ItemBuilder item, List<ItemBuilder> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.itemName, "Enable Item?", true, "Should this item appear in runs?").Value;
            if (enabled)
                itemList.Add(item);
            return enabled;
        }
    }

    public static class Assets
    {
        public static AssetBundle mainAssetBundle;

        static bool imTooGodDamnLazyToSetUpConfigurationForThisSoJustSetThisToTrueIfAssetBundleIsntEmbeddedOkay = true;

        public static void PopulateAssets()
        {
            if (imTooGodDamnLazyToSetUpConfigurationForThisSoJustSetThisToTrueIfAssetBundleIsntEmbeddedOkay)
            {
                var path = Assembly.GetExecutingAssembly().Location.Remove(Assembly.GetExecutingAssembly().Location.LastIndexOf('\\') + 1);
                mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(path, "trinketsandbaubles_assets"));
            }
            else
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TrinketsAndBaubles.trinketsandbaubles_assets"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }
        }
    }

    public class ContentPackProvider : IContentPackProvider
    {
        public static ContentPack ContentPack = Assets.mainAssetBundle.LoadAsset<SerializableContentPack>("ContentPack").CreateContentPack();

        public string identifier
        {
            get
            {
                return "ForgottenFoes";
            }
        }

        internal static void Init()
        {
            ContentManager.collectContentPackProviders += AddCustomContent;
        }

        private static void AddCustomContent(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            bool flag = WhenContentPackReady != null;
            if (flag)
            {
                foreach (Action<ContentPack> action in WhenContentPackReady.GetInvocationList())
                {
                    try
                    {
                        action(ContentPack);
                    }
                    catch (Exception ex)
                    {
                        LogCore.LogE("Error making ContentPack");
                    }
                }
            }
            addContentPackProvider(new ContentPackProvider());
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(ContentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        internal static Action<ContentPack> WhenContentPackReady;
    }

}

