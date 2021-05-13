using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BepInEx.Configuration;
using EnigmaticThunder.Modules;
using EnigmaticThunder.Util;
using EntityStates;
using ForgottenFoes.EntityStates.ImpSorcerer;
using ForgottenFoes.Utils;
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
    class ImpSorcerer : EnemyBuilder
    {
        public override GameObject bodyPrefab => Assets.mainAssetBundle.LoadAsset<GameObject>("ImpSorcererBody");
        public override GameObject masterPrefab => Assets.mainAssetBundle.LoadAsset<GameObject>("ImpSorcererMaster");
        public override ForgottenFoesDirectorCardHolder directorCardHolder => Assets.mainAssetBundle.LoadAsset<ForgottenFoesDirectorCardHolder>("ImpSorcererDirectorCardHolder");
        public override Type[] entityStateTypes => new Type[]
        {
            typeof(SpawnState),
            typeof(DeathState),
            typeof(BlinkState),
            typeof(FireVoidClusterState)
        };
        public override string monsterName => "ImpSorcerer";

        public override void BuildConfig(ConfigFile config)
        {
            base.BuildConfig(config);
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
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<ProjectileExplosion, Vector3, Vector3>>((self, vector) =>
            {
                //Fix this shit once projectile is implemented
                if (self.gameObject)
                    return new Vector3(vector.x, -1f, vector.z);
                return vector;
            });
            c.Emit(OpCodes.Stloc_0);
        }

        //I FUCKING HATE CHICAGO
        //FUCK YOU CHICAGO

        public override void ModifyAssets()
        {
            //Adds the surfaceDef's impact prefab
            var surfaceDef = Assets.mainAssetBundle.LoadAsset<SurfaceDef>("sdImpSorcerer");
            surfaceDef.impactEffectPrefab = Resources.Load<SurfaceDef>("surfacedefs/sdImp").impactEffectPrefab;
            CrystalLocator.crystalPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("ImpSorcererCrystal");
        }

        public override void ModifyPrefabs()
        {
        }

        public override void RegisterPrefabs()
        {
            base.RegisterPrefabs();
            //where we would add projectiles
        }
        public override void ModifyDirectorCardOrHolder()
        {
        }

        /*private void AddProjectiles()
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
        }*/


    }
}


namespace ForgottenFoes.EntityStates.ImpSorcerer
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
        [SerializeField]
        public GameObject spawnEffect;
        [SerializeField]
        public float duration = 3f;

        private Animator animator;
        private CrystalLocator crystalLocator;
        private ChildLocator childLocator;
        private Transform spawnPortalCenter;
        private bool hasFoundEffect = false;
        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            if (rigidbodyMotor)
                rigidbodyMotor.enabled = false;
            if (modelLocator)
            {
                Transform modelTransform = modelLocator.modelTransform;
                childLocator = modelTransform.GetComponent<ChildLocator>();
                crystalLocator = modelTransform.GetComponent<CrystalLocator>();
                if (childLocator && crystalLocator && spawnEffect)
                {
                    EffectManager.SimpleMuzzleFlash(spawnEffect, gameObject, "SpawnPortalCenter", false);
                    spawnPortalCenter = childLocator.FindChild("SpawnPortalCenter");
                }
            }
            PlayAnimation("Body", "Spawn", "Spawn.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //SleepRigid();
            if (!hasFoundEffect && childLocator && spawnPortalCenter.childCount > 0)
            {
                spawnPortalCenter.GetChild(0).GetComponent<SpawnCrystalOnDeath>().locator = crystalLocator;
                hasFoundEffect = true;
            }
            if (fixedAge >= duration && isAuthority)
                outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbodyMotor.enabled = true;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

        private void SleepRigid()
        {
            if(rigidbodyMotor)
            {
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.velocity = Vector3.zero;
            }
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

    public class FireVoidClusterState : BaseSkillState
    {
        public static GameObject projectilePrefab;
        public static float baseDuration = 3f;
        public static float damageCoefficient = 4f;
        private Animator modelAnimator;
        private float duration;
        private Ray aimRay;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / characterBody.attackSpeed;
            PlayAnimation("Gesture, Override", "FireVoidCluster", "FireVoidCluster.playbackRate", duration);
            ProjectileManager.instance.FireProjectile(projectilePrefab, inputBank.aimOrigin + inputBank.aimDirection * 1.8f, Quaternion.identity, gameObject, damageStat * damageCoefficient, 0f, Util.CheckRoll(critStat, characterBody.master), DamageColorIndex.Default, null, -1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration)
                outer.SetNextStateToMain();
        }


    }

    public class BlinkState : BaseSkillState
    {
        [SerializeField]
        public GameObject blinkPrefab;
        [SerializeField]
        public Material destealthMaterial;
        [SerializeField]
        public float startDuration = 1.23f;
        [SerializeField]
        public float exitDuration = 3.2f;
        [SerializeField]
        public float offGridDuration = 0.5f;
        [SerializeField]
        public float blinkDistance = 45f;

        private Transform modelTransform;
        private Animator animator;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;
        private ChildLocator childLocator;
        private Vector3 blinkStart;
        private Vector3 blinkDestination;

        public override void OnEnter()
        {
            base.OnEnter();
            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                animator = modelTransform.GetComponent<Animator>();
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
                childLocator = modelTransform.GetComponent<ChildLocator>();
            }
            if (rigidbodyMotor)
                rigidbodyMotor.enabled = false;
            LogCore.LogM("Check");
            CreateBlinkEffect(Util.GetCorePosition(gameObject));
            LogCore.LogM("Check");
            CalculateBlinkDestination();
            PlayAnimation("Body", "TeleportIn", "Teleport.playbackRate", startDuration);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            SleepRigid();
            if (fixedAge >= startDuration && !ranStart)
                RunOffGrid();
            if (fixedAge >= startDuration + offGridDuration && !ranGrid)
                SNAPBACKTOREALITY();
            if (fixedAge >= startDuration + offGridDuration + exitDuration)
            { 
                outer.SetNextStateToMain();
                return;
            }
        }

        private void RunOffGrid()
        {
            LogCore.LogM("Check");
            ranStart = true;
            if (characterModel)
                characterModel.invisibilityCount++;
            LogCore.LogM("Check");
            if (hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            LogCore.LogM("Check");
            gameObject.layer = LayerIndex.fakeActor.intVal;
            SetPosition(blinkDestination);
        }
        private void SNAPBACKTOREALITY()
        {
            ExitCleanup();
            LogCore.LogM("Check");
            PlayAnimation("Body", "TeleportIn", "Teleport.playbackRate", exitDuration);
            LogCore.LogM("Check");
            ranGrid = true;
        }

        private void CalculateBlinkDestination()
        {
            Vector3 vector = inputBank.moveVector.normalized * blinkDistance;
            Ray aimRay = GetAimRay();
            blinkDestination = transform.position;
            blinkStart = transform.position;
            NodeGraph airNodes = SceneInfo.instance.airNodes;
            NodeGraph.NodeIndex nodeIndex = airNodes.FindClosestNode(transform.position + vector, characterBody.hullClassification, float.PositiveInfinity);
            airNodes.GetNodePosition(nodeIndex, out blinkDestination);
            blinkDestination += transform.position;
            rigidbodyDirection.aimDirection = aimRay.origin;
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if (blinkPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkDestination - this.blinkStart);
                effectData.origin = origin;
                EffectManager.SpawnEffect(blinkPrefab, effectData, false);
            }
        }
        private void SetPosition(Vector3 newPosition)
        {
            if (rigidbody)
                rigidbody.position = newPosition;
        }
        private void ExitCleanup()
        {
            if (isExiting)
                return;
            LogCore.LogM("Check");
            isExiting = true;
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            LogCore.LogM("Check");
            //Util.PlaySound(endSoundString, gameObject);
            LogCore.LogM("Check");
            CreateBlinkEffect(Util.GetCorePosition(gameObject));
            LogCore.LogM("Check");
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
            LogCore.LogM("Check");
            if (characterModel)
                characterModel.invisibilityCount--;
            LogCore.LogM("Check");
            if (hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            LogCore.LogM("Check");
        }

        public override void OnExit()
        {
            base.OnExit();
            if (rigidbodyMotor)
            {
                rigidbodyMotor.enabled = true;
                rigidbody.WakeUp();
            }
        }

        private void SleepRigid()
        {
            if (rigidbodyMotor)
            {
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.velocity = Vector3.zero;
            }
        }

        private bool isExiting = false;
        private bool ranStart = false;
        private bool ranGrid = false;
    }

}