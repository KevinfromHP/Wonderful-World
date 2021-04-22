using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using EnigmaticThunder.Modules;
using EnigmaticThunder.Util;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace TrinketsAndBaubles.Utils
{
    internal abstract class EquipBuilder
    {
        /// <summary>
        /// The name to refer to for config.
        /// </summary>
        public abstract string equipName { get; }

        /// <summary>
        /// The ItemDef for the item;
        /// </summary>
        public abstract EquipmentDef equipDef { get; }

        public virtual void Create(ConfigFile config)
        {
            LogCore.LogI("h1");
            BuildConfig(config);
            LogCore.LogI("h2");
            ModifyEquipment();
            LogCore.LogI("h3");
            RegisterEquipment();
            LogCore.LogI("h5");
            LogCore.LogI("h6");
            LogCore.LogI("h7");
            LogCore.LogI("h8");
            LogCore.LogI("h9");
            LogCore.LogI("h10");
            Hook();
            LogCore.LogI("h11");
        }

        public abstract void ModifyEquipment();

        public virtual void RegisterEquipment()
        {
            EquipmentHandler.equipmentDefs.Add(equipDef);
            EquipmentHandler.fireEquipmentCases.Add(new Func<EquipmentSlot, bool>(FireEquipment));
        }

        public abstract bool FireEquipment(EquipmentSlot slot);

        public virtual void Hook()
        {

        }

        public virtual void BuildConfig(ConfigFile config) { }

    }
}