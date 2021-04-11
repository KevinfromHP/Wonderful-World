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

namespace ForgottenFoes.Enemies
{
    class ImpSorcerer : EnemyBuilderNew
    {
        public override int DirectorCost => 27;

        public override bool NoElites => false;

        public override bool ForbiddenAsBoss => false;

        public override HullClassification HullClassification => HullClassification.Human;

        public override MapNodeGroup.GraphType GraphType => MapNodeGroup.GraphType.Air;

        public override int SelectionWeight => 1;

        public override DirectorCore.MonsterSpawnDistance SpawnDistance => DirectorCore.MonsterSpawnDistance.Far;

        public override bool AmbushSpawn => true;

        public override DirectorAPI.Stage[] StagesToSpawnOn => new DirectorAPI.Stage[] { DirectorAPI.Stage.ScorchedAcres, DirectorAPI.Stage.RallypointDelta, DirectorAPI.Stage.AbyssalDepths };

        public override DirectorAPI.MonsterCategory MonsterCategory => DirectorAPI.MonsterCategory.BasicMonsters;

        public override string MonsterName => "Imp Cunt"; //Very classy, Enigma.

        protected override string bodyName => "ImpSorcerer";

        public static string deleteWhenYouGetModel = "ModelBase/mdlImp";

        public GameObject projectilePrefab;
        public GameObject spikePrefab;

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

        public override void Hook()
        {
            base.Hook();
            IL.RoR2.Projectile.ProjectileExplosion.FireChild += IL_ProjectileImpactChildFix;
        }

        //nice summary, bro.
        ///<summary>Because the FireChild stuff was designed in a completely **FUCKING STUPID** WAY AND TAKES THE Z OFFETS FOR BOTH Y AND Z OF THE VECTOR... This injects some code to make it take the y offset</summary>
        private void IL_ProjectileImpactChildFix(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.TryGotoNext(
                x => x.MatchCallOrCallvirt<ProjectileExplosion>("GetRandomDirectionForChild"),
                x => x.MatchStloc(0));
            c.Index += 2;

            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Func<ProjectileExplosion, Vector3, Vector3>>((self, vector) =>
            {
                if (self.gameObject)
                    return new Vector3(vector.x, -1f, vector.z);
                return vector;
            });
            c.Emit(OpCodes.Starg, 0);
        }

        //I FUCKING HATE CHICAGO
        //FUCK YOU CHICAGO

        public override void BuildPrefabs()
        {
            bodyPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/BellBody"), "ImpSorcererBody", true);

            UnityEngine.Object.DestroyImmediate(bodyPrefab.transform.Find("Model Base/mdlBell").gameObject);
            var model = PrefabAPI.InstantiateClone(Assets.mainAssetBundle.LoadAsset<GameObject>("mdlImpSorcerer"), "mdlImpSorcerer", false);
            model.transform.SetParent(bodyPrefab.transform.Find("Model Base"), false);

            #region Component Finding/Adding
            var cb = bodyPrefab.GetComponent<CharacterBody>();
            var healthComponent = bodyPrefab.GetComponent<HealthComponent>();
            var teamComponent = bodyPrefab.GetComponent<TeamComponent>();
            var modelLocator = bodyPrefab.GetComponent<ModelLocator>();
            var inputBank = bodyPrefab.GetComponent<InputBankTest>();
            var networkMachine = bodyPrefab.GetComponent<NetworkStateMachine>();
            var stateMachines = bodyPrefab.GetComponents<EntityStateMachine>();
            var deathBehavior = bodyPrefab.GetComponent<CharacterDeathBehavior>();
            var bodyCollider = bodyPrefab.AddComponent<BoxCollider>();
            var rigidbody = bodyPrefab.GetComponent<Rigidbody>();
            var rigidbodyDirection = bodyPrefab.GetComponent<RigidbodyDirection>();
            var rigidbodyMotor = bodyPrefab.GetComponent<RigidbodyMotor>();
            var vectorPIDs = bodyPrefab.GetComponents<VectorPID>();
            var quaternionPID = bodyPrefab.GetComponent<QuaternionPID>();
            var characterModel = model.AddComponent<CharacterModel>();
            var childLocator = model.AddComponent<ChildLocator>();
            var aimAnimator = model.AddComponent<AimAnimator>();
            var aimAssist = model.transform.Find("AimAssist").gameObject.AddComponent<AimAssistTarget>();
            var hurtBoxGroup = model.AddComponent<HurtBoxGroup>();
            var collisionDisable = bodyPrefab.AddComponent<DisableCollisionsBetweenColliders>();
            var printController = model.AddComponent<PrintController>();
            printController.enabled = false;
            #endregion

            #region CharacterBody
            cb.baseNameToken = "IMPSORCERER_BODY_NAME";
            cb.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage;
            cb.rootMotionInMainState = false;
            cb.baseMaxHealth = 250f;
            cb.baseMoveSpeed = 10f;
            cb.baseArmor = 5f;
            cb.sprintingSpeedMultiplier = 1.4f;
            cb.autoCalculateLevelStats = true;
            cb.levelMaxHealth = 60f;
            cb.levelDamage = 2;
            cb.aimOriginTransform = model.transform.Find("ImpSorcererArmature/ROOT/ROOTBone/Spine1/Spine2/AimOrigin");
            cb.hullClassification = HullClassification.Human;
            cb.portraitIcon = null;
            cb.isChampion = false;
            cb.preferredInitialStateType = new SerializableEntityStateType(typeof(SpawnState));
            #endregion

            #region ModelLocator
            modelLocator.modelTransform = model.transform;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.noCorpse = true;
            modelLocator.autoUpdateModelTransform = true;
            #endregion

            //why?
            //jfc
            /*#region EntityStates/Death Behavior
            foreach (EntityStateMachine stateMachine in stateMachines)
                UnityEngine.Object.Destroy(stateMachine);
            stateMachines[0] = bodyPrefab.AddComponent<EntityStateMachine>();
            stateMachines[0].initialStateType = LoadoutAPI.StateTypeOf<SpawnState>();
            stateMachines[0].mainStateType = LoadoutAPI.StateTypeOf<FlyState>();
            stateMachines[1] = bodyPrefab.AddComponent<EntityStateMachine>();
            stateMachines[1].initialStateType = LoadoutAPI.StateTypeOf<Idle>();
            stateMachines[1].mainStateType = LoadoutAPI.StateTypeOf<Idle>();

            networkMachine.stateMachines = stateMachines;
            deathBehavior.deathState = LoadoutAPI.StateTypeOf<DeathState>();
            deathBehavior.deathStateMachine = EntityStateMachine.FindByCustomName(bodyPrefab, "Body");
            deathBehavior.idleStateMachine = new EntityStateMachine[] { EntityStateMachine.FindByCustomName(bodyPrefab, "Weapon") };
            #endregion*/

            #region Body Collider  
            UnityEngine.Object.Destroy(bodyPrefab.GetComponent<CapsuleCollider>());
            bodyCollider.center = new Vector3(-0.05f, -0.35f, -0.15f);
            bodyCollider.size = new Vector3(1.55f, 3.5f, 1f);
            #endregion

            #region Rigidbody/Direction/Motor
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            rigidbody.centerOfMass = Vector3.zero;

            rigidbodyDirection.animatorTorqueScale = 1f;
            rigidbodyDirection.animatorZCycle = "aimPitchCycle";
            rigidbodyDirection.animatorYCycle = "aimYawCycle";
            //rigidbodyDirection.freezeZRotation = false;
            #endregion

            #region PIDs
            quaternionPID.gain = 2f;
            foreach (VectorPID vectorPID in vectorPIDs)
            {
                if (vectorPID.customName == "torquePID")
                    vectorPID.gain = 2f;
                else
                    vectorPID.PID = new Vector3(2f, 0f);
            }
            #endregion

            #region CharacterModel
            characterModel = ComponentCopier.GetCopyOf(characterModel, Resources.Load<GameObject>("Prefabs/CharacterBodies/ImpBossBody").transform.Find("ModelBase/mdlImpBoss").GetComponent<CharacterModel>());
            characterModel.body = cb;
            characterModel.baseRendererInfos[2].ignoreOverlays = true;



            var impBossMeshRenderer = Resources.Load<GameObject>("Prefabs/CharacterBodies/ImpBossBody").transform.Find("ModelBase/mdlImpBoss/ImpBossMesh").GetComponent<SkinnedMeshRenderer>();

            //ok then.
            LogCore.LogW("renderer " + impBossMeshRenderer.name);
            LogCore.LogW("material " + impBossMeshRenderer.material.name);
            LogCore.LogW("shader " + impBossMeshRenderer.material.shader.name);
            LogCore.LogW("EmTex " + impBossMeshRenderer.material.GetTexture("_EmTex").name);
            LogCore.LogW("FlowHeightRamp " + impBossMeshRenderer.material.GetTexture("_FlowHeightRamp").name);
            LogCore.LogW("FlowHeightmap " + impBossMeshRenderer.material.GetTexture("_FlowHeightmap").name);
            LogCore.LogW("FlowTex " + impBossMeshRenderer.material.GetTexture("_FlowTex").name);
            LogCore.LogW("FresnelRamp " + impBossMeshRenderer.material.GetTexture("_FresnelRamp").name);
            LogCore.LogW("MainTex " + impBossMeshRenderer.material.GetTexture("_MainTex").name);
            LogCore.LogW("NormalTex " + impBossMeshRenderer.material.GetTexture("_NormalTex").name);
            LogCore.LogW("PrintRamp " + impBossMeshRenderer.material.GetTexture("_PrintRamp").name);
            LogCore.LogW("SliceAlphaTex " + impBossMeshRenderer.material.GetTexture("_SliceAlphaTex").name);
            #endregion

            #region PrintController
            printController.printTime = 3f;
            var golem = Resources.Load<GameObject>("Prefabs/CharacterBodies/GolemBody");
            printController.printCurve = golem.transform.Find("ModelBase/mdlGolem").GetComponent<PrintController>().printCurve;
            printController.startingPrintHeight = -1.78f;
            printController.disableWhenFinished = true;
            printController.maxPrintHeight = 7f;
            printController.animateFlowmapPower = false;
            #endregion

            #region ChildLocator
            childLocator.SetFieldValue("transformPairs", new ChildLocator.NameTransformPair[]
            {
                new ChildLocator.NameTransformPair
                {
                    name = "ImpSorcererArmature",
                    transform = model.transform.Find("ImpSorcererArmature")
                },
                new ChildLocator.NameTransformPair
                {
                    name = "CrystalMesh",
                    transform = model.transform.Find("CrystalMesh")
                },
                new ChildLocator.NameTransformPair
                {
                    name = "GlowFlowerMesh",
                    transform = model.transform.Find("GlowFlowerMesh")
                },
                new ChildLocator.NameTransformPair
                {
                    name = "ImpSorcererMesh",
                    transform = model.transform.Find("ImpSorcererMesh")
                },
                new ChildLocator.NameTransformPair
                {
                    name = "ImpSorcererPortal",
                    transform = model.transform.Find("ImpSorcererPortal")
                },
                new ChildLocator.NameTransformPair
                {
                    name = "AimAssist",
                    transform = model.transform.Find("AimAssist")
                },
                new ChildLocator.NameTransformPair
                {
                    name = "Base",
                    transform = model.transform.Find("Base")
                },
                new ChildLocator.NameTransformPair
                {
                    name = "Chest",
                    transform = model.transform.Find("Chest")
                }
            });
            #endregion

            #region AimAnimator
            aimAnimator.inputBank = inputBank;
            aimAnimator.pitchRangeMin = -33f;
            aimAnimator.pitchRangeMax = 33f;
            aimAnimator.yawRangeMin = -45f;
            aimAnimator.yawRangeMax = 45f;
            aimAnimator.pitchGiveupRange = 70f;
            aimAnimator.yawGiveupRange = 100f;
            aimAnimator.giveupDuration = 4f;
            aimAnimator.raisedApproachSpeed = 720f;
            aimAnimator.loweredApproachSpeed = 360f;
            #endregion

            #region AimAssist
            aimAssist = ComponentCopier.GetCopyOf(aimAssist, Resources.Load<GameObject>("Prefabs/CharacterBodies/BellBody").transform.Find("Model Base/mdlBell/GameObject").GetComponent<AimAssistTarget>());
            aimAssist.point0 = model.transform.Find("Base");
            aimAssist.point1 = model.transform.Find("Chest");
            aimAssist.healthComponent = healthComponent;
            aimAssist.teamComponent = teamComponent;
            #endregion

            #region HurtBoxes
            var hurtBoxColliders = new List<Collider>();
            hurtBoxGroup.bullseyeCount = 1;
            short index = 0;

            //The great big HurtBox adding machine, just a really abstracted way of adding hurtboxes to bones.
            for (int i = 1; i <= 2; i++)
            {
                string path = "ImpSorcererArmature/ROOT/ROOTBone/Pelvis";
                int iterations = 2;
                if (i == 2)
                {
                    path = "ImpSorcererArmature/ROOT/ROOTBone/Spine1";
                    iterations = 3;
                }

                var hurtBox0 = model.transform.Find(path + "/HurtBox").gameObject.AddComponent<HurtBox>();
                hurtBox0.healthComponent = healthComponent;
                hurtBox0.isBullseye = false;
                hurtBox0.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBox0.hurtBoxGroup = hurtBoxGroup;
                hurtBox0.indexInGroup = index;
                hurtBoxColliders.Add(hurtBox0.GetComponent<Collider>());
                index++;


                if (i == 2)
                {
                    path += "/Spine2";
                    var hurtBoxSpine2 = model.transform.Find(path + "/HurtBox").gameObject.AddComponent<HurtBox>();
                    hurtBoxSpine2.healthComponent = healthComponent;
                    hurtBoxSpine2.isBullseye = true;
                    hurtBoxSpine2.damageModifier = HurtBox.DamageModifier.Normal;
                    hurtBoxSpine2.hurtBoxGroup = hurtBoxGroup;
                    hurtBoxSpine2.indexInGroup = index;
                    hurtBoxGroup.mainHurtBox = hurtBoxSpine2;
                    hurtBoxColliders.Add(hurtBoxSpine2.GetComponent<Collider>());
                    index++;


                }

                for (int k = 1; k <= iterations; k++)
                {
                    string path1 = path;
                    int iterations1;

                    if (i == 1)
                    {
                        if (k == 1)
                            path1 += "/UpperLeg.L/LowerLeg.L";
                        else
                            path1 += "/UpperLeg.R/LowerLeg.R";
                        iterations1 = 2;
                    }
                    else
                    {
                        iterations1 = 3;
                        if (k == 1)
                            path1 += "/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L";
                        else
                        if (k == 2)
                            path1 += "/Shoulder.R/UpperArm.R/LowerArm.R/Hand.R";
                        else
                        {
                            path1 += "/Neck/Head/Tentacle1/Tentacle2/Tentacle3/Tentacle4/Tentacle5/Tentacle6/Tentacle7/Tentacle8/Tentacle9/Tentacle10/Tentacle11/Tentacle12";
                            iterations1 = 13;
                        }

                    }

                    for (int l = iterations1; l > 0; l--)
                    {
                        var hurtBox = model.transform.Find(path1 + "/HurtBox").gameObject.AddComponent<HurtBox>();
                        hurtBox.healthComponent = healthComponent;
                        hurtBox.isBullseye = false;
                        hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                        hurtBox.hurtBoxGroup = hurtBoxGroup;
                        hurtBox.indexInGroup = (short)(index + l);
                        hurtBoxColliders.Add(hurtBox.GetComponent<Collider>());
                        path1.Remove(path.LastIndexOf('/'));
                    }
                    index = (short)(index + iterations1);
                }
            }
            #endregion

            #region Collision Disabling
            //collision disabling between intersecting colliders
            collisionDisable.collidersA = new Collider[] { bodyCollider };
            collisionDisable.collidersB = hurtBoxColliders.ToArray();
            #endregion
            CloudUtils.RegisterNewBody(bodyPrefab);
            BuildMaster();
        }


        //https://cdn.discordapp.com/attachments/785698006212804619/806241149412835338/Screenshot_20210202-121414_Discord.jpg
        private void BuildMaster()
        {
            masterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/ImpMaster"), "ImpSorcererMaster");
            CharacterMaster cm = masterPrefab.GetComponent<CharacterMaster>();
            cm.bodyPrefab = bodyPrefab;
            var baseAI = masterPrefab.GetComponent<BaseAI>();
            //this field doesn't exist anymore
            //baseAI.minDistanceFromEnemy = 50f;
            baseAI.stateMachine = masterPrefab.GetComponent<EntityStateMachine>();
            baseAI.enemyAttentionDuration = 7f;

            Component[] toDelete = masterPrefab.GetComponents<AISkillDriver>();
            foreach (AISkillDriver asd in toDelete)
            {
                switch (asd.customName)
                {
                    case "BlinkBecauseClose":
                        asd.customName = "BlinkBecauseTooClose";
                        //asd.requiredSkill = skillDefUtility;
                        asd.minDistance = 0f;
                        asd.maxDistance = 25f;
                        asd.movementType = AISkillDriver.MovementType.FleeMoveTarget;
                        break;
                    case "StrafeBecausePrimaryIsntReady":
                        asd.customName = "FleeBecauseCantAttack";
                        asd.maxDistance = 30f;
                        asd.movementType = AISkillDriver.MovementType.FleeMoveTarget;
                        break;
                    case "LeaveNodeGraph":
                        asd.movementType = AISkillDriver.MovementType.StrafeMovetarget;
                        break;
                    case "PathToTarget":
                        asd.customName = "CloseDistance";
                        asd.minDistance = 80f;
                        break;
                    default:
                        UnityEngine.Object.Destroy(asd);
                        break;
                }
            }

            #region VoidCluster
            AISkillDriver voidCluster = masterPrefab.AddComponent<AISkillDriver>();
            voidCluster.skillSlot = SkillSlot.Secondary;
            //voidCluster.requiredSkill = skillDefSecondary;
            voidCluster.requireSkillReady = false;
            voidCluster.requireEquipmentReady = false;
            voidCluster.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            voidCluster.minDistance = 35f;
            voidCluster.maxDistance = float.PositiveInfinity;
            voidCluster.selectionRequiresTargetLoS = true;
            voidCluster.activationRequiresTargetLoS = true;
            voidCluster.activationRequiresAimConfirmation = true;
            voidCluster.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            voidCluster.aimType = AISkillDriver.AimType.None;
            voidCluster.ignoreNodeGraph = false;
            voidCluster.noRepeat = false;
            voidCluster.shouldSprint = false;
            voidCluster.shouldFireEquipment = false;
            voidCluster.shouldTapButton = false;
            voidCluster.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            #endregion
            CloudUtils.RegisterNewMaster(masterPrefab);
            AddProjectiles();

        }
        private void AddProjectiles()
        {
            projectilePrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/VagrantTrackingBomb"), "ImpSorcererVoidClusterBomb", true);
            spikePrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/ImpVoidspikeProjectile"), "ImpSorcererVoidClusterSpikes", true);

            UnityEngine.Object.Destroy(projectilePrefab.GetComponent<ProjectileDirectionalTargetFinder>());
            UnityEngine.Object.Destroy(projectilePrefab.GetComponent<ProjectileSteerTowardTarget>());

            // add ghost version here


            //Does ProjectileController changes
            #region ProjectileController
            var projectileController = projectilePrefab.GetComponent<ProjectileController>();
            projectileController.allowPrediction = true;
            projectileController.shouldPlaySounds = true;
            #endregion

            //Does SimpleComponent changes
            #region SimpleComponent
            var simpleComponent = projectilePrefab.GetComponent<ProjectileSimple>();
            simpleComponent.updateAfterFiring = true;
            simpleComponent.velocity = 0;
            simpleComponent.lifetime = 99f;
            #endregion

            //Does DamageComponent changes
            #region DamageComponent
            var damageComponent = projectilePrefab.GetComponent<ProjectileDamage>();
            damageComponent.damageType = DamageType.BleedOnHit;
            #endregion

            //Does TargetingComponent changes
            #region TargetingComponent
            var targetingComponent = projectilePrefab.AddComponent<ProjectileSphereTargetFinder>();
            targetingComponent.lookRange = 400f;
            targetingComponent.onlySearchIfNoTarget = true;
            targetingComponent.allowTargetLoss = false;
            targetingComponent.flierAltitudeTolerance = float.PositiveInfinity;
            #endregion

            //Does SteerComponent changes
            #region SteerComponent
            var steerComponent = projectilePrefab.AddComponent<ProjectileSteerAboveTarget>();
            //steerComponent.rotationSpeed = 720f;
            steerComponent.maxVelocity = 10f;
            steerComponent.yAxisOnly = false;
            #endregion

            //Does ImpactComponentChanges
            #region ImpactComponent
            var impactComponent = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            impactComponent.lifetime = 10f;
            impactComponent.lifetimeAfterImpact = 5f;
            impactComponent.impactEffect = Resources.Load<GameObject>("prefabs/effects/ImpVoidspikeExplosion");
            impactComponent.destroyOnEnemy = true;
            impactComponent.destroyOnWorld = true;
            impactComponent.blastRadius = 2f;
            impactComponent.childrenDamageCoefficient = 0.75f;
            // These make it so the children always shoot downwards in a random radius. 0.26795 calculated with Mathf.Tan((float)Math.PI / 12f), or tan(15 deg)
            impactComponent.transformSpace = ProjectileImpactExplosion.TransformSpace.World;
            impactComponent.minAngleOffset = new Vector3(-0.26795f, -1f, -0.26795f);
            impactComponent.maxAngleOffset = new Vector3(0.26795f, -1f, 0.26795f);
            impactComponent.childrenCount = 8;
            impactComponent.childrenProjectilePrefab = spikePrefab;
            impactComponent.fireChildren = true;

            /* These need to be added once sound is done
            impactComponent.offsetForLifeTimeExpiredSound; //Subtracts this number from the lifetime to make the sound play early
            impactComponent.explosionSoundString;
            impactComponent.lifetimeExpiredSound;*/
            #endregion

            //Adds the halo indicator for the explosion downwards.
            #region Halo
            var haloPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("ImpSorcererHalo");
            PrefabAPI.RegisterNetworkPrefab(haloPrefab);
            projectilePrefab.AddComponent<VoidClusterBombIndicator>();
            #endregion

            var spikeSimpleComponent = spikePrefab.GetComponent<ProjectileSimple>();
            spikeSimpleComponent.velocity *= 0.75f;

            Projectiles.RegisterProjectile(projectilePrefab);
            Projectiles.RegisterProjectile(spikePrefab);


            FireVoidCluster.projectilePrefab = projectilePrefab;
        }

        public override void CreatePrimary(SkillLocator skillLocator, SkillFamily skillFamily)
        {
            base.CreatePrimary(skillLocator, skillFamily);
            SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationState = new SerializableEntityStateType(typeof(FireVoidCluster));
            def.activationStateMachineName = "Body";
            def.baseMaxStock = 100;
            def.baseRechargeInterval = 10f;
            def.beginSkillCooldownOnSkillEnd = true;
            def.canceledFromSprinting = false;
            def.fullRestockOnAssign = true;
            def.interruptPriority = InterruptPriority.Skill;
            def.isCombatSkill = true;
            def.mustKeyPress = true;
            def.rechargeStock = 1;
            def.requiredStock = 1;
            def.stockToConsume = 1;
            def.icon = null;
            def.skillDescriptionToken = "IMPSORCERER_SECONDARY_VOIDCLUSTER_DESCRIPTION";
            def.skillName = "IMPSORCERER_SECONDARY_VOIDCLUSTER_NAME";
            def.skillNameToken = "IMPSORCERER_SECONDARY_VOIDCLUSTER_NAME";

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = def,
                viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
            };
        }

        public override void CreateSecondary(SkillLocator skillLocator, SkillFamily skillFamily)
        {
            base.CreateSecondary(skillLocator, skillFamily);

            Loadouts.RegisterEntityState(typeof(FireVoidCluster));

            SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationState = new SerializableEntityStateType(typeof(FireVoidCluster));
            def.activationStateMachineName = "Body";
            def.baseMaxStock = 100;
            def.baseRechargeInterval = 1f;
            def.beginSkillCooldownOnSkillEnd = true;
            def.canceledFromSprinting = false;
            def.fullRestockOnAssign = true;
            def.interruptPriority = InterruptPriority.Skill;
            def.isCombatSkill = true;
            def.mustKeyPress = true;
            def.rechargeStock = 1;
            def.requiredStock = 1;
            def.stockToConsume = 1;
            def.icon = null;
            def.skillDescriptionToken = "IMPSORCERER_SECONDARY_VOIDCLUSTER_DESCRIPTION";
            def.skillName = "IMPSORCERER_SECONDARY_VOIDCLUSTER_NAME";
            def.skillNameToken = "IMPSORCERER_SECONDARY_VOIDCLUSTER_NAME";

            Loadouts.RegisterSkillDef(def);

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = def,
                viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
            };
        }
        public override void CreateUtility(SkillLocator skillLocator, SkillFamily skillFamily)
        {
            base.CreateUtility(skillLocator, skillFamily);
            SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationState = new SerializableEntityStateType(typeof(ImpSorcererBlinkState));
            def.activationStateMachineName = "Body";
            def.baseMaxStock = 1;
            def.baseRechargeInterval = 15f;
            def.beginSkillCooldownOnSkillEnd = true;
            def.canceledFromSprinting = false;
            def.fullRestockOnAssign = true;
            def.interruptPriority = InterruptPriority.Skill;
            def.isCombatSkill = false;
            def.mustKeyPress = true;
            def.rechargeStock = 1;
            def.requiredStock = 1;
            def.stockToConsume = 1;
            def.icon = null;
            def.skillDescriptionToken = "IMPSORCERER_UTILITY_BLINK_DESCRIPTION";
            def.skillName = "IMPSORCERER_UTILITY_BLINK_NAME";
            def.skillNameToken = "IMPSORCERER_UTILITY_BLINK_NAME";
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = def,
                viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
            };
        }

        public override void CreateSpecial(SkillLocator skillLocator, SkillFamily skillFamily)
        {
            base.CreateSpecial(skillLocator, skillFamily);

            SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationState = new SerializableEntityStateType(typeof(EyeAttackState));
            def.activationStateMachineName = "Body";
            def.baseMaxStock = 100;
            def.baseRechargeInterval = 10f;
            def.beginSkillCooldownOnSkillEnd = true;
            def.canceledFromSprinting = false;
            def.fullRestockOnAssign = true;
            def.interruptPriority = InterruptPriority.Skill;
            def.isCombatSkill = true;
            def.mustKeyPress = true;
            def.rechargeStock = 1;
            def.requiredStock = 1;
            def.stockToConsume = 1;
            def.icon = null;
            def.skillDescriptionToken = "IMPSORCERER_SPECIAL_EYEATTACK_DESCRIPTION";
            def.skillName = "IMPSORCERER_SPECIAL_EYEATTACK_NAME";
            def.skillNameToken = "IMPSORCERER_SPECIAL_EYEATTACK_NAME";
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = def,
                viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
            };
        }


        public override void ModifyDirectorCard(DirectorCard card)
        {
            base.ModifyDirectorCard(card);
        }

        public override void ModifySpawnCard(CharacterSpawnCard card)
        {
            base.ModifySpawnCard(card);
        }

        public override void OverrideSkills()
        {
            base.OverrideSkills();
        }
    }
    public class VoidClusterBombIndicator : MonoBehaviour
    {
        GameObject indicator;
        void Awake()
        {
            indicator = Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("ImpSorcererHalo"), gameObject.transform.position, Quaternion.identity);
            indicator.GetComponentInChildren<MeshRenderer>().material = Resources.Load<GameObject>("Prefabs/Projectiles/ImpVoidspikeProjectile").transform.Find("ImpactEffect").Find("AreaIndicator").GetComponent<MeshRenderer>().material;

            //I have no clue why I didn't just do a Position Constraint earlier, that old shit was way worse.
            var constraint = new ConstraintSource();
            constraint.sourceTransform = transform;
            indicator.GetComponent<PositionConstraint>().AddSource(constraint);
        }

        void OnDestroy()
        {
            Destroy(indicator);
        }
    }
    public class ProjectileSteerAboveTarget : MonoBehaviour
    {
        public bool yAxisOnly;
        public float maxVelocity;
        private float velocity;
        private Vector3 velocityAsVector;
        private new Transform transform;
        private ProjectileTargetComponent targetComponent;
        private ProjectileSimple projectileSimple;
        private Transform model;
        private void Start()
        {
            if (!NetworkServer.active)
            {
                enabled = false;
                return;
            }
            transform = gameObject.transform;
            targetComponent = GetComponent<ProjectileTargetComponent>();
            projectileSimple = GetComponent<ProjectileSimple>();
            model = gameObject.transform.Find("Model");
            velocity = maxVelocity;
            velocityAsVector = Vector3.zero;
        }

        /*private void FixedUpdate()
        {
            if (targetComponent.target)
            {
                Vector3 vector = targetComponent.target.transform.position + new Vector3(0f, 10f) - transform.position;
                if (Mathf.Abs(vector.y) < 1f)
                    vector.y = 0f;
                if (vector != Vector3.zero)
                {
                    transform.forward = Vector3.RotateTowards(transform.forward, vector, rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
                    model.forward = Vector3.RotateTowards(model.forward, -vector, rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
                }
                if (Mathf.Abs(vector.x) < 1f && Mathf.Abs(vector.z) < 1f && projectileSimple.velocity > 0.25f && vector.y > -1f)
                    projectileSimple.velocity -= 0.25f;
                else
                if (projectileSimple.velocity < 13f)
                    projectileSimple.velocity += 1f;
                if (projectileSimple.velocity > 13f)
                    projectileSimple.velocity = 13f;
            }
        }*/

        private void FixedUpdate()
        {
            if (targetComponent.target)
            {
                Vector3 vector = targetComponent.target.transform.position + new Vector3(0f, 10f) - transform.position;
                Vector3 movePosition = targetComponent.target.transform.position + new Vector3(0f, 10f);
                //if (Mathf.Abs(vector.y) < 0.1f)
                //  vector.y = 0f;
                if (vector.sqrMagnitude < 1.3f)
                {
                    velocity -= 0.15f;
                    if (velocity < 0.15f)
                        velocity = 0.15f;
                }
                else
                if (velocity < 13f)
                    velocity += 0.2f;
                if (velocity > 13f)
                    velocity = 13f;
                if (vector != Vector3.zero)
                    transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocityAsVector, vector.magnitude / velocity, maxVelocity, Time.deltaTime);
            }
        }
    }

    public class ImpSorcererCrystalController : MonoBehaviour
    {
        public static GameObject clonePrefab;
        public float rotationAngularVelocity = 30f;
        public float acceleration = 15f;
        public float damping = 0.3f;
        public bool enableSpringMotion = false;
        private GameObject crystals;
        private GameObject crystalClones;
        private Transform constraint;
        private ImpSorcererCrystalMovementManager movementManager;
        private ImpSorcererCrystalAttackManager attackManager;
        private Vector3 velocity = Vector3.zero;
        public MovementType movementType = MovementType.BasicFollowing;
        public enum MovementType
        {
            //This will contain all of the movement types that the crystals may have while doing stuff
            BasicFollowing,
            Attacking,
        }
        private void Start()
        {
            crystals = gameObject.transform.Find(ImpSorcerer.deleteWhenYouGetModel + "/Crystals").gameObject;
            crystalClones = Instantiate(clonePrefab, crystals.transform.position, crystals.transform.rotation);
            movementManager = crystalClones.GetComponent<ImpSorcererCrystalMovementManager>();
            attackManager = crystalClones.GetComponent<ImpSorcererCrystalAttackManager>();

            movementManager.originalCrystalsObject = crystals;



            //Instantiates the constraint
            constraint = new GameObject().transform;
            var rotationConstraint = crystals.GetComponent<RotationConstraint>();
            var constraintSource = new ConstraintSource
            {
                sourceTransform = constraint,
                weight = 1f
            };
            rotationConstraint.AddSource(constraintSource);
        }

        private void Update()
        {
            //UpdateMotion();
            //transform.position += velocity * Time.deltaTime;
            UpdateRotation();
        }

        // This shit does not need to be implemented until the unique movement types are developed
        /*private void UpdateMotion()
        {
            Vector3 desiredPosition = crystals.transform.parent.position;
            if (enableSpringMotion)
            {
                Vector3 lhs = desiredPosition - transform.position;
                if (lhs != Vector3.zero)
                {
                    Vector3 a = lhs.normalized * acceleration;
                    Vector3 b = velocity * -damping;
                    velocity += (a + b) * Time.deltaTime;
                    return;
                }
            }
            else
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, damping, 14f);
        }*/

        private void UpdateRotation()
        {
            switch (movementType)
            {
                default:
                    /*var aimRay = parentBaseObject.GetComponent<InputBankTest>().GetAimRay();
                    for (int i = 0; i < 3; i++)
                    {
                        var offset = transform.Find("Offset" + i);
                        offset.forward = Vector3.RotateTowards(offset.forward, aimRay.direction, 180f * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
                    }
                    transform.rotation = Quaternion.AngleAxis(rotationAngularVelocity * Time.deltaTime, Vector3.up) * transform.rotation;*/
                    constraint.rotation = Quaternion.AngleAxis(rotationAngularVelocity * Time.deltaTime, Vector3.up) * constraint.rotation;
                    break;
            }
        }

        public void SetTarget(GameObject target, float attackSpeed, float attackInterval, float damage)
        {
            if (movementType == MovementType.Attacking)
                return;
            movementType = MovementType.Attacking;
            movementManager.enabled = false;



            attackManager.ownerBodyObject = gameObject;
            attackManager.targetBodyObject = target;

            attackManager.attackSpeed = attackSpeed;
            attackManager.attackInterval = attackInterval;
            attackManager.damage = damage;
            attackManager.enabled = true;
        }
    }
    public class ImpSorcererCrystalAttackManager : NetworkBehaviour
    {
        public static GameObject effectPrefab;
        [SyncVar]
        public GameObject ownerBodyObject;
        [SyncVar]
        public GameObject targetBodyObject;
        public GameObject NetworkownerBodyObject
        {
            get
            {
                return ownerBodyObject;
            }
            [param: In]
            set
            {
                SetSyncVarGameObject(value, ref ownerBodyObject, 1U, ref ___ownerBodyObjectNetId);
            }
        }
        public GameObject NetworktargetBodyObject
        {
            get
            {
                return targetBodyObject;
            }
            [param: In]
            set
            {
                SetSyncVarGameObject(value, ref targetBodyObject, 2U, ref ___targetBodyObjectNetId);
            }
        }

        // attackTime is what percent of the attack interval it is through when they attack. This just makes it easier to work with attack speeds.
        public float attackSpeed;
        public float attackInterval;
        public float damage;
        public float rotationAngularVelocity = 30f;
        public float acceleration = 15f;
        public float damping = 0.3f;
        public bool enableSpringMotion = false;

        // attack timer only starts ticking down once they get close enough to the target
        private float attackTimer = 0f;
        private float attackTime = 0.75f;
        private bool isTelegraphing = false;
        private bool attacked = false;
        private bool hasDoneEffect = false;
        private float angle = 2.0944f;
        private Vector3 velocity = Vector3.zero;
        private Vector3 lastKnownPosition;
        private GameObject cachedTargetBodyObject;
        private HurtBox cachedTargetHurtBox;
        private GameObject originalCrystalsObject;
        private Transform[] crystals = new Transform[3];
        private Rigidbody[] rigidBodies = new Rigidbody[3];
        private NetworkInstanceId ___ownerBodyObjectNetId;
        private NetworkInstanceId ___targetBodyObjectNetId;

        private void Start()
        {
            originalCrystalsObject = GetComponent<ImpSorcererCrystalMovementManager>().originalCrystalsObject;
            crystals = GetComponent<ImpSorcererCrystalMovementManager>().crystals;
            rigidBodies = GetComponent<ImpSorcererCrystalMovementManager>().rigidBodies;
        }
        private void FixedUpdate()
        {
            if (cachedTargetBodyObject != targetBodyObject)
            {
                cachedTargetBodyObject = targetBodyObject;
                OnTargetChanged();
            }
            if (NetworkServer.active)
                FixedUpdateServer();

            if (cachedTargetHurtBox)
                lastKnownPosition = cachedTargetHurtBox.transform.position;

            if (isTelegraphing)
            {
                if (hasDoneEffect == false)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var offset = transform.Find("Offset" + i);
                        EffectData effectData = new EffectData
                        {
                            origin = offset.position,
                            scale = 0.6f,
                            rotation = Quaternion.Euler(90f, offset.eulerAngles.y, 0f)
                        };
                        EffectManager.SpawnEffect(effectPrefab, effectData, false);
                        hasDoneEffect = true;
                    }
                }
            }
            else
            {
                UpdateMotion();
                UpdateRotation();
                if (Vector3.SqrMagnitude(lastKnownPosition - transform.position) <= 0.0001f)
                {
                    isTelegraphing = true;
                    velocity = Vector3.zero;
                }
            }
        }
        public void FixedUpdateServer()
        {
            if (isTelegraphing)
            {
                attackTimer += Time.fixedDeltaTime;
                if (attackTimer >= attackInterval)
                {
                    ownerBodyObject.GetComponent<ImpSorcererCrystalController>().movementType = ImpSorcererCrystalController.MovementType.BasicFollowing;
                    GetComponent<ImpSorcererCrystalMovementManager>().enabled = true;
                    ResetStats();
                    enabled = false;
                }
                else
                {
                    if (attackTimer >= attackInterval * attackTime && attacked == false)
                        DoAttack();
                }
            }
            if (!ownerBodyObject)
                Destroy(gameObject);
        }
        private void UpdateMotion()
        {
            Vector3 desiredPosition = GetDesiredPosition();
            angle = (angle + rotationAngularVelocity * Mathf.Deg2Rad * Time.fixedDeltaTime) % (2f * Mathf.PI);
            for (int i = 1; i < 4; i++)
            {
                rigidBodies[i - 1].MovePosition(transform.position + 2f * new Vector3(Mathf.Cos(angle * i), 0f, Mathf.Sin(angle * i)));
            }
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, damping, 14f);
        }
        private void UpdateRotation()
        {
            for (int i = 0; i < 3; i++)
            {
                crystals[i].forward = Vector3.RotateTowards(crystals[i].forward, lastKnownPosition - crystals[i].position, 180f * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
            }
            transform.rotation = Quaternion.AngleAxis(rotationAngularVelocity * Time.deltaTime, Vector3.up) * transform.rotation;
        }

        [Server]
        private void DoAttack()
        {
            attacked = true;
            for (int i = 0; i < 3; i++)
            {
                var offset = gameObject.transform.Find("Offset" + i);
                ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectileSimple"), offset.position, Util.QuaternionSafeLookRotation(offset.forward), ownerBodyObject, damage, 0f, false, DamageColorIndex.Default);
            }
        }
        private Vector3 GetTargetPosition()
        {
            GameObject gameObject = targetBodyObject;
            if (!gameObject)
                return lastKnownPosition;
            CharacterBody component = gameObject.GetComponent<CharacterBody>();
            if (!component)
                return gameObject.transform.position;
            return component.corePosition;
        }
        private Vector3 GetDesiredPosition()
        {
            return GetTargetPosition();
        }
        private void ResetStats()
        {
            attackTimer = 0f;
            isTelegraphing = false;
            attacked = false;
            hasDoneEffect = false;
            angle = 2.0944f;
            velocity = Vector3.zero;
        }
        private void OnTargetChanged()
        {
            cachedTargetHurtBox = (cachedTargetBodyObject ? cachedTargetBodyObject.GetComponent<CharacterBody>().mainHurtBox : null);
        }
        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write(ownerBodyObject);
                writer.Write(targetBodyObject);
                return true;
            }
            bool flag = false;
            if ((syncVarDirtyBits & 1U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(ownerBodyObject);
            }
            if ((syncVarDirtyBits & 2U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(targetBodyObject);
            }
            if (!flag)
            {
                writer.WritePackedUInt32(syncVarDirtyBits);
            }
            return flag;
        }
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                ___ownerBodyObjectNetId = reader.ReadNetworkId();
                ___targetBodyObjectNetId = reader.ReadNetworkId();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if ((num & 1) != 0)
            {
                ownerBodyObject = reader.ReadGameObject();
            }
            if ((num & 2) != 0)
            {
                targetBodyObject = reader.ReadGameObject();
            }
        }
        public override void PreStartClient()
        {
            if (!___ownerBodyObjectNetId.IsEmpty())
            {
                NetworkownerBodyObject = ClientScene.FindLocalObject(___ownerBodyObjectNetId);
            }
            if (!___targetBodyObjectNetId.IsEmpty())
            {
                NetworktargetBodyObject = ClientScene.FindLocalObject(___targetBodyObjectNetId);
            }
        }
        public override void OnStartClient()
        {
            OnStartClient();
            //sets the crystal parent object position/rotation to the original without changing the position of the other crystals
            for (int i = 0; i < 3; i++)
                crystals[i].SetParent(originalCrystalsObject.transform, true);
            transform.position = originalCrystalsObject.transform.position;
            transform.rotation = originalCrystalsObject.transform.rotation;
            for (int i = 0; i < 3; i++)
                crystals[i].SetParent(transform, true);
        }
    }

    /*public class ImpSorcererCrystalAttackManager : MonoBehaviour
    {
        public static GameObject effectPrefab;
        public GameObject ownerBodyObject;
        public GameObject targetBodyObject;

        // attackTime is what percent of the attack interval it is through when they attack. This just makes it easier to work with attack speeds.
        public float attackSpeed;
        public float attackInterval;
        public float damage;
        public float rotationAngularVelocity;
        public float acceleration = 15f;
        public float damping = 0.3f;
        public bool enableSpringMotion = false;

        // attack timer only starts ticking down once they get close enough to the target
        private float attackTimer = 0f;
        private float attackTime = 0.75f;
        private bool isTelegraphing = false;
        private bool attacked = false;
        private bool hasDoneEffect = false;
        private float angle = 2.0944f;
        private Vector3 velocity = Vector3.zero;
        private Vector3 lastKnownPosition;
        private GameObject cachedTargetBodyObject;
        private HurtBox cachedTargetHurtBox;
        private GameObject originalCrystalsObject;
        private Transform[] crystals = new Transform[3];
        private Rigidbody[] rigidBodies = new Rigidbody[3];


        private void Start()
        {
            originalCrystalsObject = GetComponent<ImpSorcererCrystalMovementManager>().originalCrystalsObject;
            crystals = GetComponent<ImpSorcererCrystalMovementManager>().crystals;
            rigidBodies = GetComponent<ImpSorcererCrystalMovementManager>().rigidBodies;

            //sets the crystal parent object position/rotation to the original without changing the position of the other crystals
            for (int i = 0; i < 3; i++)
                crystals[i].SetParent(originalCrystalsObject.transform, true);
            transform.position = originalCrystalsObject.transform.position;
            transform.rotation = originalCrystalsObject.transform.rotation;
            for (int i = 0; i < 3; i++)
                crystals[i].SetParent(transform, true);
        }

        private void FixedUpdate()
        {
            if (cachedTargetBodyObject != targetBodyObject)
            {
                cachedTargetBodyObject = targetBodyObject;
                OnTargetChanged();
            }
            if (NetworkServer.active)
                FixedUpdateServer();

            if (cachedTargetHurtBox)
                lastKnownPosition = cachedTargetHurtBox.transform.position;
            if (isTelegraphing)
            {
                if (hasDoneEffect == false)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var offset = transform.Find("Offset" + i);
                        EffectData effectData = new EffectData
                        {
                            origin = offset.position,
                            scale = 0.6f,
                            rotation = Quaternion.Euler(90f, offset.eulerAngles.y, 0f)
                        };
                        EffectManager.SpawnEffect(effectPrefab, effectData, false);
                        hasDoneEffect = true;
                    }
                }
            }
            else
            {
                UpdateMotion();
                UpdateRotation();
                if ((targetBodyObject.GetComponent<CharacterBody>().corePosition - transform.position).sqrMagnitude <= 0.01f)
                {
                    isTelegraphing = true;
                    velocity = Vector3.zero;
                }
            }
        }
        public void FixedUpdateServer()
        {
            if (isTelegraphing)
            {
                attackTimer += Time.fixedDeltaTime;
                if (attackTimer >= attackInterval)
                {
                    originalCrystalsObject.GetComponent<ImpSorcererCrystalController>().movementType = ImpSorcererCrystalController.MovementType.BasicFollowing;
                    GetComponent<ImpSorcererCrystalMovementManager>().enabled = true;
                    ResetStats();
                    enabled = false;
                }
                else
                {
                    if (attackTimer >= attackInterval * attackTime && attacked == false)
                        DoAttack();
                }
            }
            if (!ownerBodyObject)
                Destroy(gameObject);
        }
        private void UpdateMotion()
        {
            Vector3 desiredPosition = GetDesiredPosition();
            angle = (angle + rotationAngularVelocity * Mathf.Deg2Rad * Time.fixedDeltaTime) % (2f * Mathf.PI);
            for (int i = 1; i < 4; i++)
            {

                rigidBodies[i - 1].MovePosition(transform.position + 2f * new Vector3(Mathf.Cos(angle * i), 0f, Mathf.Sin(angle * i)));
            }
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, damping, 14f);
        }
        private void UpdateRotation()
        {
            for (int i = 0; i < 3; i++)
            {
                rigidBodies[i].MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(crystals[i].forward, lastKnownPosition - crystals[i].position, 180f * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f)));
            }
            transform.rotation = Quaternion.AngleAxis(rotationAngularVelocity * Time.deltaTime, Vector3.up) * transform.rotation;
        }

        private void DoAttack()
        {
            attacked = true;
            for (int i = 0; i < 3; i++)
            {
                var offset = gameObject.transform.Find("Offset" + i);
                ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectileSimple"), offset.position, Util.QuaternionSafeLookRotation(offset.forward), ownerBodyObject, damage, 0f, false, DamageColorIndex.Default);
            }
        }
        private Vector3 GetTargetPosition()
        {
            GameObject gameObject = targetBodyObject;
            if (!gameObject)
                return lastKnownPosition;
            CharacterBody component = gameObject.GetComponent<CharacterBody>();
            if (!component)
                return gameObject.transform.position;
            return component.corePosition;
        }
        private Vector3 GetDesiredPosition()
        {
            return GetTargetPosition();
        }
        private void ResetStats()
        {
            attackTimer = 0f;
            attackTime = 0.75f;
            isTelegraphing = false;
            attacked = false;
            hasDoneEffect = false;
            angle = 2.0944f;
            velocity = Vector3.zero;
        }
        private void OnTargetChanged()
        {
            cachedTargetHurtBox = (cachedTargetBodyObject ? cachedTargetBodyObject.GetComponent<CharacterBody>().mainHurtBox : null);
        }
    }*/

    public class ImpSorcererCrystalMovementManager : MonoBehaviour
    {
        public GameObject originalCrystalsObject;
        public float rotationAngularVelocity = 30f;
        public float acceleration = 20f;
        public float damping = 0.3f;
        public bool enableSpringMotion = false;
        private Vector3 velocity = Vector3.zero;
        public Transform[] crystals = new Transform[3];
        public Rigidbody[] rigidBodies = new Rigidbody[3];
        public Transform[] originalCrystals = new Transform[3];

        private void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                var offset = gameObject.transform.Find("Offset" + i);
                crystals[i] = offset;
                rigidBodies[i] = offset.GetComponent<Rigidbody>();
                originalCrystals[i] = originalCrystalsObject.transform.Find("Offset" + i);
            }
        }
        private void Update()
        {
            if (!originalCrystalsObject)
                Destroy(gameObject);
        }
        public void FixedUpdate()
        {
            //This is some janky ass code that makes it so this game object always stays with the original one while keeping its children at their position
            /*transform.DetachChildren();
            transform.position = originalCrystalsObject.transform.position;
            transform.rotation = originalCrystalsObject.transform.rotation;
            foreach (Transform crystal in crystals)
                crystal.SetParent(transform, true);*/

            UpdateMotion();
            UpdateRotation();
        }
        private void UpdateMotion()
        {
            var desiredPosition = (originalCrystalsObject.transform.position - gameObject.transform.position) / 1.2f + gameObject.transform.position;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, damping, 14f);
            for (int i = 0; i < 3; i++)
            {
                //crystals[i].localPosition = Vector3.SmoothDamp(crystals[i].localPosition, originalCrystalsObject.transform.Find("Offset" + i).localPosition, ref velocityOffsets[i], damping, 14f);
                var position = (originalCrystals[i].position - rigidBodies[i].position) / 1.2f + rigidBodies[i].position;
                rigidBodies[i].MovePosition(position);
            }
        }
        private void UpdateRotation()
        {
            gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, originalCrystalsObject.transform.forward, 30f * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
            for (int i = 0; i < 3; i++)
            {
                //crystals[i].forward = Vector3.RotateTowards(crystals[i].forward, originalCrystalsObject.transform.Find("Offset" + i).forward, 180f * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
                rigidBodies[i].MoveRotation(originalCrystals[i].rotation);
            }
        }
    }
}


namespace EntityStates.ForgottenFoes.ImpSorcererStates
{
    /*Imp has these animation layers:
     * Layer 0: Body
     * Layer 1: Turn
     * Layer 2: Gesture, Override
     * Layer 3: Gesture, Additive
     * Layer 4: Impact
     * Layer 5: AimYaw
     * Layer 6: AimPitch
     * Layer 7: Flinch
     * Layer 8: Idle, Additive
     * Layer 9: Blink, Additive
     */

    public class SpawnState : BaseState
    {
        public static float duration = 3f;
        public static string spawnSoundString;
        public static GameObject spawnEffectPrefab;
        private float stopwatch;
        public override void OnEnter()
        {
            OnEnter();
            var modelTransform = GetModelTransform();
            if (modelTransform)
                modelTransform.GetComponent<PrintController>().enabled = true;
            PlayAnimation("Body", "Spawn", "Spawn.playbackRate", duration);
            //Util.PlaySound(SpawnState.spawnSoundString, gameObject);
            //if (spawnEffectPrefab)
            //EffectManager.SimpleMuzzleFlash(spawnEffectPrefab, gameObject, "Base", false);
        }

        public override void FixedUpdate()
        {
            FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
    public class DeathState : GenericCharacterDeath
    {
        public static GameObject initialEffect;
        public static GameObject deathEffect;
        private static float duration = 3.3166666f;
        private float stopwatch;
        private Animator animator;
        private bool hasPlayedDeathEffect;
        private bool attemptedDeathBehavior;
        public override void OnEnter()
        {
            OnEnter();
            animator = GetModelAnimator();
            if (characterMotor)
            {
                characterMotor.enabled = false;
            }
            if (modelLocator)
            {
                Transform modelTransform = modelLocator.modelTransform;
                ChildLocator component = modelTransform.GetComponent<ChildLocator>();
                CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
                if (component)
                {
                    component.FindChild("DustCenter").gameObject.SetActive(false);
                    if (initialEffect)
                    {
                        EffectManager.SimpleMuzzleFlash(initialEffect, gameObject, "DeathCenter", false);
                    }
                }
                if (component2)
                {
                    for (int i = 0; i < component2.baseRendererInfos.Length; i++)
                    {
                        component2.baseRendererInfos[i].ignoreOverlays = true;
                    }
                }
            }
            PlayAnimation("Body", "Death");
        }

        public override void FixedUpdate()
        {
            if (animator)
            {
                stopwatch += Time.fixedDeltaTime;
                if (!hasPlayedDeathEffect && animator.GetFloat("DeathEffect") > 0.5f)
                {
                    hasPlayedDeathEffect = true;
                    EffectManager.SimpleMuzzleFlash(deathEffect, gameObject, "DeathCenter", false);
                }
                if (stopwatch >= duration)
                {
                    AttemptDeathBehavior();
                }
            }
        }

        private void AttemptDeathBehavior()
        {
            if (attemptedDeathBehavior)
            {
                return;
            }
            attemptedDeathBehavior = true;
            if (modelLocator.modelBaseTransform)
            {
                EntityState.Destroy(modelLocator.modelBaseTransform.gameObject);
            }
            if (NetworkServer.active)
            {
                EntityState.Destroy(gameObject);
            }
        }

        public override void OnExit()
        {
            if (!outer.destroying)
            {
                AttemptDeathBehavior();
            }
            OnExit();
        }

    }

    public class FireVoidCluster : BaseSkillState
    {
        public static GameObject projectilePrefab;
        public static GameObject effectPrefab;
        public static float baseDuration = 3f;
        public static float damageCoefficient = 4f;
        public static float procCoefficient;
        public static float selfForce;
        public static float forceMagnitude = 16f;
        public static GameObject hitEffectPrefab;
        public static GameObject swipeEffectPrefab;
        public static string enterSoundString;
        public static string slashSoundString;
        public static float walkSpeedPenaltyCoefficient;
        private Animator modelAnimator;
        private float duration;
        private Ray aimRay;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnEnter()
        {
            OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            rigidbodyMotor.enabled = false;
            //Util.PlayScaledSound(enterSoundString, gameObject, attackSpeedStat);

            if (modelAnimator)
            {
                PlayAnimation("Body", "Secondary", "Secondary.playbackRate", duration);
            }
        }
        public override void OnExit()
        {
            aimRay = GetAimRay();
            ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.direction, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * damageCoefficient, 0f, Util.CheckRoll(critStat, characterBody.master), DamageColorIndex.Default, null, -1f);
            if (characterBody)
                characterBody.SetAimTimer(2f);
            rigidbodyMotor.enabled = true;
            OnExit();
        }

        public override void FixedUpdate()
        {
            FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
    }
    public class ImpSorcererBlinkState : BaseSkillState
    {
        private Transform modelTransform;
        public static GameObject blinkPrefab = Resources.Load<GameObject>("prefabs/effects/ImpBlinkEffect");
        public static Material destealthMaterial = EntityStates.ImpMonster.BlinkState.destealthMaterial;
        private float stopwatch;
        private Vector3 blinkDestination = Vector3.zero;
        private Vector3 blinkStart = Vector3.zero;
        public static float durationFirstAnim = 1.6167f;
        public static float duration = 6.66f;
        public static float waitTime = 1.15f;
        public static float blinkDistance = 40f;
        public static string beginSoundString;
        public static string endSoundString;
        private Animator animator;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;

        public override void OnEnter()
        {
            OnEnter();
            Util.PlaySound(beginSoundString, gameObject);
            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                animator = modelTransform.GetComponent<Animator>();
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
            }
            if (characterModel)
            {
                characterModel.invisibilityCount++;
            }
            if (hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (rigidbodyMotor)
                rigidbodyMotor.enabled = false;
            Vector3 b = inputBank.moveVector * blinkDistance;
            blinkDestination = transform.position;
            blinkStart = transform.position;
            NodeGraph airNodes = SceneInfo.instance.airNodes;
            NodeGraph.NodeIndex nodeIndex = airNodes.FindClosestNode(transform.position + b, characterBody.hullClassification);
            airNodes.GetNodePosition(nodeIndex, out blinkDestination);
            blinkDestination += transform.position - characterBody.footPosition;
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(blinkDestination - blinkStart);
            effectData.origin = origin;
            EffectManager.SpawnEffect(blinkPrefab, effectData, false);
            PlayAnimation("Body", "Utility1");
        }

        private void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        public override void FixedUpdate()
        {
            FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            SetPosition(Vector3.Lerp(blinkStart, blinkDestination, stopwatch / duration));
            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
            else
            {
                if (fixedAge >= waitTime + durationFirstAnim)
                    CreateBlinkEffect(blinkDestination);
                else
                {
                    if (fixedAge >= duration - waitTime && isAuthority)
                    {

                        Util.PlaySound(endSoundString, gameObject);
                        CreateBlinkEffect(Util.GetCorePosition(gameObject));
                        modelTransform = GetModelTransform();
                        if (modelTransform && destealthMaterial)
                        {
                            TemporaryOverlay temporaryOverlay = animator.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = 1f;
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = destealthMaterial;
                            temporaryOverlay.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            temporaryOverlay.animateShaderAlpha = true;
                        }
                        if (characterModel)
                            characterModel.invisibilityCount--;
                        if (hurtboxGroup)
                        {
                            HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                            int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                            hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                        }
                    }
                }
            }
        }

        public override void OnExit()
        {
            OnExit();
        }
    }

    //word dissassacion

    public class EyeAttackState : BaseSkillState
    {
        public static GameObject projectilePrefab;
        public static GameObject effectPrefab;
        public static float baseDuration = 3.5f;
        public static float damageCoefficient = 4f;
        public static float procCoefficient;
        public static float selfForce;
        public static float forceMagnitude = 16f;
        public static GameObject hitEffectPrefab;
        public static GameObject swipeEffectPrefab;
        public static string enterSoundString;
        public static string slashSoundString;
        public static float walkSpeedPenaltyCoefficient;
        private Animator modelAnimator;
        private float duration;
        private Transform modelTransform;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnEnter()
        {
            OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            modelTransform = GetModelTransform();
            Ray aimRay = GetAimRay();
            StartAimMode(aimRay, 2f, false);
            characterMotor.walkSpeedPenaltyCoefficient = walkSpeedPenaltyCoefficient;
            //Util.PlayScaledSound(enterSoundString, gameObject, attackSpeedStat);
            if (modelAnimator)
            {
                PlayAnimation("Gesture, Additive", "DoubleSlash", "DoubleSlash.playbackRate", duration);
                PlayAnimation("Gesture, Override", "DoubleSlash", "DoubleSlash.playbackRate", duration);
            }
            if (isAuthority)
            {
                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
                if (teamComponent)
                {
                    bullseyeSearch.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
                }
                bullseyeSearch.maxDistanceFilter = 70f;
                bullseyeSearch.maxAngleFilter = 100f;
                bullseyeSearch.searchOrigin = aimRay.origin;
                bullseyeSearch.searchDirection = aimRay.direction;
                bullseyeSearch.filterByLoS = true;
                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
                bullseyeSearch.RefreshCandidates();
                HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
                if (hurtBox)
                {
                    gameObject.GetComponent<ImpSorcererCrystalController>().SetTarget(hurtBox.healthComponent.body.gameObject, attackSpeedStat, duration, damageStat * damageCoefficient);
                }
            }
            if (characterBody)
            {
                characterMotor.walkSpeedPenaltyCoefficient = 1f;
                characterBody.SetAimTimer(duration + 2f);
            }
        }
        public override void OnExit()
        {
            OnExit();
        }

        public override void FixedUpdate()
        {
            FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
    }
}