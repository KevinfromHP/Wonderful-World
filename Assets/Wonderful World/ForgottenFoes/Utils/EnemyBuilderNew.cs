using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace ForgottenFoes
{

    internal abstract class EnemyBuilderNew
    {
        /// <summary>
        /// The body prefab of the enemy in the assetbundle;
        /// </summary>
        public abstract GameObject bodyPrefab { get; }

        /// <summary>
        /// The master prefab of the enemy in the assetbundle;
        /// </summary>
        public abstract GameObject masterPrefab { get; }

        public abstract ForgottenFoesDirectorCardHolder directorCardHolder { get; }

        /// <summary>
        /// The name to refer to for config.
        /// </summary>
        public abstract string monsterName { get; }

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
            CreateDirectorCard();
        }


        public virtual void RegisterPrefabs()
        {
            //PrefabAPI.RegisterNetworkPrefab(bodyPrefab);
            //PrefabAPI.RegisterNetworkPrefab(masterPrefab);
            CloudUtils.RegisterNewBody(bodyPrefab);
            CloudUtils.RegisterNewMaster(masterPrefab);
        }
        public virtual void Hook()
        {

        }

        public virtual void BuildConfig(ConfigFile config) { }

        public abstract void ModifyPrefabs();

        public virtual void ModifyDirectorCardOrHolder(ForgottenFoesDirectorCardHolder card)
        {

        }

        public virtual void CreateDirectorCard()
        {
            var directorCard = directorCardHolder.card;            

            ModifyDirectorCardOrHolder(directorCardHolder);

            if (directorCardHolder.stagesToSpawnOn.Length > 0)
            {
                foreach (DirectorAPI.Stage stage in directorCardHolder.stagesToSpawnOn)
                {

                    DirectorAPI.Helpers.AddNewMonsterToStage(directorCard, directorCardHolder.category, stage);
                }
            }
            else
            {
                DirectorAPI.Helpers.AddNewMonster(directorCard, directorCardHolder.category);
            }
        }
    }
}