using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BepInEx.Configuration;
using EnigmaticThunder.Modules;
using EnigmaticThunder.Util;
using EntityStates;
using EntityStates.ForgottenFoes.ImpSorcererStates;
using ForgottenFoes.Enemies;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Networking;

//The only reason this exists is because R2API's director card holder is not compatible with the unity editor.
namespace ForgottenFoes.Utils
{
    [CreateAssetMenu(fileName = "Director Card Holder", menuName = "ForgottenFoes/Director Card Holder", order = 1)]
    public class ForgottenFoesDirectorCardHolder : ScriptableObject
    {
        public DirectorAPI.Stage[] stagesToSpawnOn;
        public DirectorAPI.MonsterCategory category;
        public DirectorCard card;
    }
}
