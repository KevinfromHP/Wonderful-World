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


        public GameObject projectilePrefab;
        public GameObject spikePrefab;

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
        public static GameObject spawnEffect;
        public static float duration = 3f;
        private Animator animator;
        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            if (rigidbodyMotor)
                rigidbodyMotor.enabled = false;
            if (modelLocator)
            {
                Transform modelTransform = modelLocator.modelTransform;
                ChildLocator component = modelTransform.GetComponent<ChildLocator>();
                CrystalLocator crystalLocator = modelTransform.GetComponent<CrystalLocator>();
                if (component && crystalLocator && spawnEffect)
                {
                    EffectManager.SimpleMuzzleFlash(spawnEffect, gameObject, "SpawnPortalCenter", false);
                    modelTransform.Find("SpawnPortalCenter/ImpSorcererSpawnEffect(Clone)").GetComponent<SpawnCrystalOnDeath>().locator = crystalLocator;
                }
            }
            PlayAnimation("Body", "Spawn", "Spawn.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                rigidbodyMotor.enabled = true;
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
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
            PlayAnimation("Gesture, Override", "FireVoidCluster", "FireVoidCluster.PlaybackRate", duration);
            ProjectileManager.instance.FireProjectile(projectilePrefab, characterBody.aimOrigin + characterBody.aimOriginTransform.forward, Quaternion.identity, gameObject, damageStat * damageCoefficient, 0f, Util.CheckRoll(critStat, characterBody.master), DamageColorIndex.Default, null, -1f);
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
        private Transform modelTransform;
        public static GameObject blinkPrefab = Resources.Load<GameObject>("prefabs/effects/ImpBlinkEffect");
        public static Material destealthMaterial;
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

}