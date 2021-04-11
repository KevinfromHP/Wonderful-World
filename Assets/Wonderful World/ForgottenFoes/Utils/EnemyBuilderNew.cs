using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

internal abstract class EnemyBuilderNew
{
    /// <summary>
    /// The body of the enemy
    /// </summary>
    public GameObject bodyPrefab;

    /// <summary>
    /// The master of the enemy;
    /// </summary>
    public GameObject masterPrefab;

    /// <summary>
    /// How much this enemy costs to spawn.
    /// </summary>
    public abstract int DirectorCost { get; }
    public abstract bool NoElites { get; }
    public abstract bool ForbiddenAsBoss { get; }
    public abstract HullClassification HullClassification { get; }
    public abstract RoR2.Navigation.MapNodeGroup.GraphType GraphType { get; }

    public abstract int SelectionWeight { get; }
    public abstract DirectorCore.MonsterSpawnDistance SpawnDistance { get; }
    public abstract bool AmbushSpawn { get; }
    public abstract DirectorAPI.Stage[] StagesToSpawnOn { get; }
    public abstract DirectorAPI.MonsterCategory MonsterCategory { get; }


    /// <summary>
    /// The name of the newly cloned body
    /// </summary>
    protected abstract string bodyName { get; }

    /// <summary>
    /// The user friendly name
    /// </summary>
    public abstract string MonsterName { get; }

    public CharacterSpawnCard characterSpawnCard;

    public virtual void Create(ConfigFile config)
    {
        LogCore.LogI("h1");
        BuildConfig(config);
        LogCore.LogI("h2");
        BuildPrefabs();
        LogCore.LogI("h3");
        LogCore.LogI("h5");
        BuildModel();
        LogCore.LogI("h6");
        BuildMasterAI();
        LogCore.LogI("h7");
        BuildCharacterbody(bodyPrefab.GetComponent<CharacterBody>());
        LogCore.LogI("h8");
        OverrideSkills();
        LogCore.LogI("h9");
        LogCore.LogI("h10");
        Hook();
        LogCore.LogI("h11");
        CreateDirectorCard();
    }

    public virtual void Hook() {

    }

    public virtual void BuildConfig(ConfigFile config) { }

    public abstract void BuildPrefabs();

    public virtual void ModifySpawnCard(CharacterSpawnCard card)
    {

    }

    public virtual void ModifyDirectorCard(DirectorCard card)
    {

    }
    public static void CharacterSpawnCard_Awake(On.RoR2.CharacterSpawnCard.orig_Awake orig, CharacterSpawnCard self)
    {
        self.loadout = new SerializableLoadout();
        orig(self);
    } 

    public virtual void CreateDirectorCard()
    {
        On.RoR2.CharacterSpawnCard.Awake += CharacterSpawnCard_Awake;
        characterSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
        On.RoR2.CharacterSpawnCard.Awake -= CharacterSpawnCard_Awake;
        characterSpawnCard.directorCreditCost = DirectorCost;
        characterSpawnCard.forbiddenAsBoss = ForbiddenAsBoss;
        characterSpawnCard.name = "csc" + bodyName;
        //characterSpawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.None;
        characterSpawnCard.hullSize = HullClassification;
        characterSpawnCard.loadout = new SerializableLoadout();
        characterSpawnCard.nodeGraphType = GraphType;
        characterSpawnCard.noElites = NoElites;
        characterSpawnCard.occupyPosition = false;
        characterSpawnCard.prefab = masterPrefab;
        characterSpawnCard.sendOverNetwork = true;

        ModifySpawnCard(characterSpawnCard);

        DirectorCard directorCard = new DirectorCard
        {
            spawnCard = characterSpawnCard,
            selectionWeight = SelectionWeight,
            spawnDistance = SpawnDistance,
            allowAmbushSpawn = AmbushSpawn,
            preventOverhead = true,
        };

        ModifyDirectorCard(directorCard);

        if (StagesToSpawnOn.Length > 0)
        {
            foreach (DirectorAPI.Stage stage in StagesToSpawnOn)
            {

                DirectorAPI.Helpers.AddNewMonsterToStage(directorCard, MonsterCategory, stage);
            }
        }
        else
        {
            DirectorAPI.Helpers.AddNewMonster(directorCard, MonsterCategory);
        }
    }

    public virtual void BuildMasterAI()
    {

    }

    public virtual void BuildCharacterbody(CharacterBody body)
    {

    }

    public virtual void BuildModel()
    {

    }

    public virtual void OverrideSkills()
    {
        CloudUtils.CreateEmptySkills(bodyPrefab);
        SkillLocator locator = bodyPrefab.GetComponent<SkillLocator>();
        CreatePrimary(locator, locator.primary.skillFamily);
        CreateSecondary(locator, locator.secondary.skillFamily);
        CreateUtility(locator, locator.utility.skillFamily);
        CreateSpecial(locator, locator.special.skillFamily);
    }



    public virtual void CreatePrimary(SkillLocator skillLocator, SkillFamily skillFamily)
    {

    }
    public virtual void CreateSecondary(SkillLocator skillLocator, SkillFamily skillFamily)
    {

    }
    public virtual void CreateUtility(SkillLocator skillLocator, SkillFamily skillFamily)
    {

    }
    public virtual void CreateSpecial(SkillLocator skillLocator, SkillFamily skillFamily)
    {

    }
}