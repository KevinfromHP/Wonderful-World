/*using BepInEx;
using R2API;
using RoR2;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using TILER2;
using UnityEngine;
using static TILER2.MiscUtil;
using RoR2.Skills;
using R2API.Utils;

namespace WonderWorld.ForgottenFoes.Utils
{
    public abstract class MonsterBoilerplate : T2Module
    {
        public string nameToken { get; private protected set; }
        public string loreToken { get; private protected set; }
        public SpawnCard spawnCard { get; private set; }
        public DirectorCard directorCard { get; private set; }

        /// <summary>Stores the body prefab that will be used for the monster. Should be declared with PrefabAPI.InstantiateClone().</summary>
        public GameObject bodyPrefab;

        ///<summary>Stores the CharacterMaster prefab for use by AI.</summary>
        public GameObject masterPrefab;

        /// <summary>Used by TILER2 to request language token value updates (object name). If langID is null, the request is for the invariant token.</summary>
        protected string GetNameString(string langID = null)
        {
            return displayName;
        }

        /// <summary>Used by TILER2 to request language token value updates (lore text, where applicable). If langID is null, the request is for the invariant token.</summary>
        protected abstract string GetLoreString(string langID = null);

        ///<summary>The object's display name in the mod's default language. Will be used in config files; should also be used in generic language tokens.</summary>
        public abstract string displayName { get; }

        ///<summary>The standard named used in files without any prefixes. If the names for these files are not consistent, there may be null references.</summary>
        public abstract string nameTag { get; }

        ///<summary>entity states to be registered should be here.</summary>
        public abstract Type[] entityStates { get; }

        ///<summary>All SkillDefs for this monster should be inside this array. The first parameter corresponds to the skill slot (primary, secondary, utility, special, passive), the second parameter is optional for skill families. If you need more than 4 slots in a monster skill family, you are likely a madman.</summary>
        public SkillDef[,] skillDefs = new SkillDef[5, 5];

        ///<summary>How big the monster is.</summary>
        public abstract HullClassification hullSize { get; }

        ///<summary>How the monster moves around.</summary>
        public abstract MapNodeGroup.GraphType graphType { get; }

        ///<summary>How many Director Credits it costs to spawn this monster in.</summary>
        public abstract int creditCost { get; }

        ///<summary>Whether the monster should occupy its position.</summary>
        public abstract bool occupyPosition { get; }

        ///<summary>How likely the monster is to spawn.</summary>
        public abstract int selectionWeight { get; }

        ///<summary>How far away should this monster spawn.</summary>
        public abstract DirectorCore.MonsterSpawnDistance spawnDistance { get; }

        ///<summary>Can the monster ambush the player.</summary>
        public abstract bool ambush { get; }

        ///<summary>The index of the stage the monster should begin spawning at. This should usually be set to 5 so they start spawning during the second loop of stages.</summary>
        public abstract int minimumStage { get; }

        ///<summary>What stages should the monster spawn on normally. Create empty array if the monster should spawn on all stages.</summary>
        public abstract DirectorAPI.Stage[] homeStages { get; }

        ///<summary>What category of monster the monster is.</summary>
        public abstract DirectorAPI.MonsterCategory monsterCategory { get; }

        ///<summary>Whether the monster can spawn as a boss (e.g. Elder Lemurians can spawn as bosses).</summary>
        public abstract bool canBeBoss { get; }





        ///<summary>Creates the prefab for the monster.</summary>
        public abstract void CreatePrefab();

        ///<summary>Sets up skills. Need I say more?</summary>
        public virtual void SkillSetup()
        {
            foreach (GenericSkill obj in bodyPrefab.GetComponentsInChildren<GenericSkill>())
                BaseUnityPlugin.DestroyImmediate(obj);
            PrimarySetup();
            SecondarySetup();
            UtilitySetup();
            SpecialSetup();
            PassiveSetup();
        }

        public virtual void PrimarySetup() { }
        public virtual void SecondarySetup() { }
        public virtual void UtilitySetup() { }
        public virtual void SpecialSetup() { }
        public virtual void PassiveSetup() { }

        ///<summary>Creates the master prefab for AI use.</summary>
        public abstract void CreateMaster();

        public override void SetupConfig()
        {
            base.SetupConfig();

            ConfigEntryChanged += (sender, args) =>
            {
                if (args.target.boundProperty.Name == nameof(enabled))
                {
                    if (args.oldValue != args.newValue)
                    {
                        if ((bool)args.newValue == true)
                        {
                            if (Run.instance != null && Run.instance.enabled) Chat.AddMessage($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Blood)}>{displayName}</color> has been <color=#aaffaa>ENABLED</color>. It will now spawn, and existing copies will start working again.");
                        }
                        else
                        {
                            if (Run.instance != null && Run.instance.enabled) Chat.AddMessage($"<color=#{ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.Blood)}>{displayName}</color> has been <color=#ffaaaa>DISABLED</color>. It will no longer spawn, and existing copies will stop working.");
                        }
                    }
                }
            };
        }

        public override void InstallLanguage()
        {
            genericLanguageTokens[nameToken] = GetNameString();
            genericLanguageTokens[loreToken] = GetLoreString();
            base.InstallLanguage();
        }

        public override void SetupAttributes()
        {
            base.SetupAttributes();

            //These current have no use, I can probably do something with them
            nameToken = $"{modInfo.longIdentifier}_{name.ToUpper()}_NAME";
            loreToken = $"{modInfo.longIdentifier}_{name.ToUpper()}_LORE";

            CreatePrefab();

            //adds the bodyPrefab and masterPrefab to the entry list
            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(bodyPrefab);
            };
            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(masterPrefab);
            };

            RegisterSkills();
            CreateMaster();


            //Makes a SpawnCard for the enemy
            CharacterSpawnCard spawnCard1 = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            spawnCard1.name = "csc" + nameTag;
            spawnCard1.prefab = bodyPrefab;
            spawnCard1.sendOverNetwork = true;
            spawnCard1.hullSize = hullSize;
            spawnCard1.nodeGraphType = graphType;
            spawnCard1.requiredFlags = NodeFlags.None;
            spawnCard1.forbiddenFlags = NodeFlags.None;
            spawnCard1.directorCreditCost = creditCost;
            spawnCard1.occupyPosition = true;
            spawnCard1.loadout = new SerializableLoadout();
            spawnCard1.noElites = false;
            spawnCard1.forbiddenAsBoss = canBeBoss;
            directorCard = new DirectorCard
            {
                spawnCard = spawnCard,
                selectionWeight = selectionWeight,
                spawnDistance = spawnDistance,
                allowAmbushSpawn = ambush,
                preventOverhead = true,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            //If the monster has home stages, add them to those stages only. Else, add it to all stages.
            if (homeStages.Length > 0)
                foreach (DirectorAPI.Stage stage in homeStages)
                    DirectorAPI.Helpers.AddNewMonsterToStage(directorCard, monsterCategory, stage);
            else
                DirectorAPI.Helpers.AddNewMonster(directorCard, monsterCategory);
        }

        public void RegisterSkills()
        {
            //Registers the entitystates
            for (int i = 0; i < entityStates.Length; i++)
                LoadoutAPI.AddSkill(entityStates[i]);

            SkillSetup();
            SkillLocator component = bodyPrefab.GetComponent<SkillLocator>();
            for (int k = 0; k < 4; k++)
            {
                if (skillDefs[k, 0] == null)
                    continue;


                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                var skillVariants = new List<SkillFamily.Variant>();

                for (int i = 0; i < 4; i++)
                {
                    if (skillDefs[k, i] == null)
                        break;

                    LoadoutAPI.AddSkillDef(skillDefs[k, i]);
                    LanguageAPI.Add(skillDefs[k, i].skillNameToken, "");
                    LanguageAPI.Add(skillDefs[k, i].skillDescriptionToken, "");

                    skillVariants.Add(new SkillFamily.Variant
                    {
                        skillDef = skillDefs[k, i],
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(skillDefs[k, i].skillNameToken, false, null)
                    });
                }
                newFamily.variants = skillVariants.ToArray();
                LoadoutAPI.AddSkillFamily(newFamily);
                switch (k)
                {
                    case 0:
                        component.primary = bodyPrefab.AddComponent<GenericSkill>();
                        component.primary.SetFieldValue("_skillFamily", newFamily);
                        break;
                    case 1:
                        component.secondary = bodyPrefab.AddComponent<GenericSkill>();
                        component.secondary.SetFieldValue("_skillFamily", newFamily);
                        break;
                    case 2:
                        component.utility = bodyPrefab.AddComponent<GenericSkill>();
                        component.utility.SetFieldValue("_skillFamily", newFamily);
                        break;
                    default:
                        component.special = bodyPrefab.AddComponent<GenericSkill>();
                        component.special.SetFieldValue("_skillFamily", newFamily);
                        break;
                }
            }
        }

        public override void SetupLate()
        {
            base.SetupLate();
        }
    }

}*/