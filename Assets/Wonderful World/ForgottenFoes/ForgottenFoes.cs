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


        //For debugging purposes, remove this shit on release/figure out how to build in debug config
        private void Update()
        {
            var input0 = Input.GetKeyDown(KeyCode.F3);
            //add more if necessary
            if (input0)
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                var position = transform.position + transform.forward * 5;
                var quaternion = Util.QuaternionSafeLookRotation(-transform.forward);
                var effectPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("ImpSorcererSpawnEffect");
                EffectManager.SimpleEffect(effectPrefab, position, quaternion, false);
            }
        }

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
            Assets.AddEffectDefs();

            LogCore.LogD("Adding Monsters Complete.");

            ContentPackProvider.Init();
        }

        internal bool ValidateEnemy(EnemyBuilder enemy, List<EnemyBuilder> enemyList)
        {
            var enabled = Config.Bind("Enemy: " + enemy.monsterName, "Enable Enemy?", true, "Should this enemy appear in runs?").Value;
            if (enabled)
            {
                enemyList.Add(enemy);
                LogCore.LogM("Added " + enemy.monsterName);
            }
            return enabled;
        }
    }
}
