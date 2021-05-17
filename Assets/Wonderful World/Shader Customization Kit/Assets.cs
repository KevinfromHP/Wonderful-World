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

namespace ShaderCustomizationKit
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;

        public static void PopulateAssets()
        {
#if DEBUG
            var path = Assembly.GetExecutingAssembly().Location.Remove(Assembly.GetExecutingAssembly().Location.LastIndexOf('\\') + 1);
            mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(path, "shadercustomizationkit_assets"));
#else
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ShaderCustomizationKit.shadercustomizationkit_assets"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
#endif
        }
    }
}
