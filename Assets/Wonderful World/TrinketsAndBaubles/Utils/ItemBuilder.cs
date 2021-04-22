using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace TrinketsAndBaubles.Utils
{
    internal abstract class ItemBuilder
    {
        /// <summary>
        /// The name to refer to for config.
        /// </summary>
        public abstract string itemName { get; }

        /// <summary>
        /// The ItemDef for the item;
        /// </summary>
        public abstract ItemDef itemDef { get; }


        public virtual void Create(ConfigFile config)
        {
            LogCore.LogI("h1");
            BuildConfig(config);
            LogCore.LogI("h2");
            ModifyPrefabs();
            LogCore.LogI("h3");
            RegisterPrefabs();
            LogCore.LogI("h5");
            LogCore.LogI("h6");
            LogCore.LogI("h7");
            LogCore.LogI("h8");
            LogCore.LogI("h9");
            LogCore.LogI("h10");
            Hook();
            LogCore.LogI("h11");
        }

        public abstract void ModifyPrefabs();

        public virtual void RegisterPrefabs()
        {
            CloudUtils.RegisterNewItem(itemDef);
        }
        public virtual void Hook()
        {

        }

        public virtual void BuildConfig(ConfigFile config) { }

    }
}