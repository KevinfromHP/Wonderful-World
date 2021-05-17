using BepInEx;
using BepInEx.Configuration;
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
namespace ShaderCustomizationKit
{
    [BepInPlugin(ModGuid, ModName, ModVer)]

    public class ShaderCustomizationKit : BaseUnityPlugin
    {
        public const string ModVer =
#if DEBUG
                "0." +
#endif
            "1.0.0";
        public const string ModName = "ShaderCustomizationKit";
        public const string ModGuid = "com.WonderWorld.ShaderCustomizationKit";


        private void Update()
        {
            var input0 = Input.GetKeyDown(KeyCode.F5);
            if (input0)
            {
                var inputBank = PlayerCharacterMasterController.instances[0].master.GetBodyObject().GetComponent<InputBankTest>();
                var position = inputBank.aimOrigin + inputBank.aimDirection * 5;
                var rotation = inputBank.transform.eulerAngles;
                var quaternion = Quaternion.Euler(rotation.x, rotation.y + 180f, rotation.z);
                var materialTester = Assets.mainAssetBundle.LoadAsset<GameObject>("MaterialTester");
                Instantiate(materialTester, position, quaternion);
            }
        }

        public void Awake()
        {
            Assets.PopulateAssets();
        }
    }
}
