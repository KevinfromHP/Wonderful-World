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
    public static class EquipmentHandler
    {
        public static List<EquipmentDef> equipmentDefs = new List<EquipmentDef>();
        public static List<Func<EquipmentSlot, bool>> fireEquipmentCases = new List<Func<EquipmentSlot, bool>>();

        public static bool FireEquipment(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef def)
        {
            if (equipmentDefs.Contains(def))
                return fireEquipmentCases[equipmentDefs.IndexOf(def)](self);
            else
                return orig(self, def);
        }
    }
}
