﻿using SharpCompress.Archives;
using SharpCompress.Readers;
using System.IO;
using System.Linq;
using System.Net;
using ThunderKit.Core.Data;
using ThunderKit.Core.Editor;
using UnityEditor;

namespace ThunderKit.Integrations.Thunderstore
{
    using PV = Core.Data.PackageVersion;
    public class ThunderstoreSource : PackageSource
    {
        private readonly static string CachePath = $"Assets/ThunderKitSettings/{typeof(ThunderstoreSource).Name}.asset";

        [InitializeOnLoadMethod]
        public static void SetupInitialization()
        {
            InitializeSources -= PackageSource_InitializeSources;
            InitializeSources += PackageSource_InitializeSources;
        }

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            AssetDatabase.DeleteAsset(CachePath);
            ThunderstoreAPI.ReloadPages();

            var isNew = false;
            var source = ScriptableHelper.EnsureAsset<ThunderstoreSource>(CachePath, so =>
            {
                isNew = true;
            });
            if (isNew)
            {
                source.hideFlags = UnityEngine.HideFlags.NotEditable;
                source.LoadPackages();
                //EditorApplication.update += ProcessDownloads;
            }
        }

        private static void PackageSource_InitializeSources(object sender, System.EventArgs e)
        {
            Initialize();
        }

        public override string Name => "Thunderstore";

        public override string SourceGroup => "Thunderstore";
        protected override string VersionIdToGroupId(string dependencyId) => dependencyId.Substring(0, dependencyId.LastIndexOf("-"));

        protected override void OnLoadPackages()
        {
            var loadedPackages = ThunderstoreAPI.LookupPackage(string.Empty);
            var realMods = loadedPackages.Where(tsp => !tsp.categories.Contains("Modpacks"));
            var orderByPinThenName = realMods.OrderByDescending(tsp => tsp.is_pinned).ThenBy(tsp => tsp.name);
            foreach (var tsp in orderByPinThenName)
            {
                var versions = tsp.versions.Select(v => new PackageVersionInfo(v.version_number, v.full_name, v.dependencies));
                AddPackageGroup(tsp.owner, tsp.name, tsp.Latest.description, tsp.full_name, tsp.categories, versions);
            }
        }

        protected override void OnInstallPackageFiles(PV version, string packageDirectory)
        {
            var tsPackage = ThunderstoreAPI.LookupPackage(version.group.DependencyId).First();
            var tsPackageVersion = tsPackage.versions.First(tspv => tspv.version_number.Equals(version.version));
            var filePath = Path.Combine(packageDirectory, $"{tsPackageVersion.full_name}.zip");

            using (var client = new WebClient())
            {
                client.DownloadFile(tsPackageVersion.download_url, filePath);
            }

            using (var archive = ArchiveFactory.Open(filePath))
            {
                foreach (var entry in archive.Entries.Where(entry => entry.IsDirectory))
                {
                    var path = Path.Combine(packageDirectory, entry.Key);
                    Directory.CreateDirectory(path);
                }

                var extractOptions = new ExtractionOptions { ExtractFullPath = true, Overwrite = true };
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    entry.WriteToDirectory(packageDirectory, extractOptions);
            }

            File.Delete(filePath);
        }
    }
}