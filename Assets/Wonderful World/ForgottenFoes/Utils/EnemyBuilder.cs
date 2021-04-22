using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using System.Collections.Generic;
using EntityStates;

namespace ForgottenFoes.Utils
{
    internal abstract class EnemyBuilder
    {
        public static List<GameObject> bodyPrefabs = new List<GameObject>();
        public static List<GameObject> masterPrefabs = new List<GameObject>();
        public static List<SerializableEntityStateType> entityStates = new List<SerializableEntityStateType>();

        /// <summary>
        /// The body prefab of the enemy in the assetbundle;
        /// </summary>
        public abstract GameObject bodyPrefab { get; }

        /// <summary>
        /// The master prefab of the enemy in the assetbundle;
        /// </summary>
        public abstract GameObject masterPrefab { get; }

        /// <summary>
        /// A list of entity state types for adding to the content pack. Don't really want to do this, but they have to be registered and can't just be added to unity unfortunately
        /// </summary>
        public abstract Type[] entityStateTypes { get; }

        /// <summary>
        /// This is used so we can design the director cards in Unity instead of in code. Much easier this way.
        /// </summary>
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
            RegisterEntityStates();
            LogCore.LogI("h6");
            LogCore.LogI("h7");
            LogCore.LogI("h8");
            LogCore.LogI("h9");
            ModifyDirectorCardOrHolder();
            LogCore.LogI("h10");
            CreateDirectorCard();
            LogCore.LogI("h11");
            Hook();
        }

        public virtual void BuildConfig(ConfigFile config) { }

        public virtual void RegisterPrefabs()
        {
            bodyPrefabs.Add(bodyPrefab);
            masterPrefabs.Add(masterPrefab);
            //CloudUtils.RegisterNewBody(bodyPrefab);
            //CloudUtils.RegisterNewMaster(masterPrefab);
        }
        public virtual void RegisterEntityStates()
        {
            foreach (Type type in entityStateTypes)
                entityStates.Add(new SerializableEntityStateType(type));
        }

        public abstract void ModifyPrefabs();

        public abstract void ModifyDirectorCardOrHolder();

        public virtual void Hook()
        {

        }

        public virtual void CreateDirectorCard()
        {
            var directorCard = directorCardHolder.card;

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

        /// <summary>
        /// Adds enabled enemies to ContentPack
        /// </summary>
        public static void AddToContentPack()
        {
            ContentPackProvider.serializedContentPack.bodyPrefabs = EnemyBuilder.bodyPrefabs.ToArray();
            ContentPackProvider.serializedContentPack.bodyPrefabs = EnemyBuilder.masterPrefabs.ToArray();
            ContentPackProvider.serializedContentPack.entityStateTypes = entityStates.ToArray();
        }
    }
}