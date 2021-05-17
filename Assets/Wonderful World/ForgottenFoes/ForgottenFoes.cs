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
using RoR2.Projectile;


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
#if DEBUG
        private void Update()
        {
            var input0 = Input.GetKeyDown(KeyCode.F2);
            var input1 = Input.GetKeyDown(KeyCode.F3);
            var input2 = Input.GetKeyDown(KeyCode.F4);
            //add more if necessary
            if (input0)
            {
                var inputBank = PlayerCharacterMasterController.instances[0].master.GetBodyObject().GetComponent<InputBankTest>();
                var position = inputBank.aimOrigin + inputBank.aimDirection * 5;
                var quaternion = Quaternion.LookRotation(inputBank.GetAimRay().direction, Vector3.up);
                var materialTester = Assets.mainAssetBundle.LoadAsset<GameObject>("MaterialTester");
                Instantiate(materialTester, position, quaternion);
            }
            if (input1)
            {
                var inputBank = PlayerCharacterMasterController.instances[0].master.GetBodyObject().GetComponent<InputBankTest>();
                var position = inputBank.aimOrigin + inputBank.aimDirection * 5;
                var quaternion = Quaternion.LookRotation(-inputBank.GetAimRay().direction, Vector3.up);
                var effectPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("ImpSorcererSpawnEffect");
                EffectManager.SimpleEffect(effectPrefab, position, quaternion, false);
            }
            if (input2)
            {
                var inputBank = PlayerCharacterMasterController.instances[0].master.GetBodyObject().GetComponent<InputBankTest>();
                var quaternion = Quaternion.LookRotation(inputBank.GetAimRay().direction);
                ProjectileManager.instance.FireProjectile(Assets.mainAssetBundle.LoadAsset<GameObject>("ImpVoidLanceProjectile"), inputBank.aimOrigin, quaternion, inputBank.gameObject, 1, 0f, false, DamageColorIndex.Bleed);
            }
        }
#endif

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
