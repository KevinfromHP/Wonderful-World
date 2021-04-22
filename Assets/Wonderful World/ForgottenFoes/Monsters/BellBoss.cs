/*using System;
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

namespace ForgottenFoes.Enemies
{
    class BellBoss : EnemyBuilder
    {
        public override int DirectorCost => 27;

        public override bool NoElites => false;

        public override bool ForbiddenAsBoss => false;

        public override HullClassification HullClassification => HullClassification.BeetleQueen;

        public override MapNodeGroup.GraphType GraphType => MapNodeGroup.GraphType.Air;

        public override int SelectionWeight => 1;

        public override DirectorCore.MonsterSpawnDistance SpawnDistance => DirectorCore.MonsterSpawnDistance.Standard;

        public override bool AmbushSpawn => false;

        public override DirectorAPI.Stage[] StagesToSpawnOn => new DirectorAPI.Stage[] { DirectorAPI.Stage.AbyssalDepths, DirectorAPI.Stage.SirensCall, DirectorAPI.Stage.SkyMeadow }; // Add Sundered Grove
        public override DirectorAPI.MonsterCategory MonsterCategory => DirectorAPI.MonsterCategory.Champions;

        public override string MonsterName => "Tolling Bell";

        protected override string bodyName => "BellBoss";

        public override void BuildCharacterbody(CharacterBody body)
        {
            base.BuildCharacterbody(body);
        }

        public override void BuildConfig(ConfigFile config)
        {
            base.BuildConfig(config);
        }

        public override void BuildMasterAI()
        {
            base.BuildMasterAI();
        }

        public override void BuildModel()
        {
            base.BuildModel();
        }


    }
}*/