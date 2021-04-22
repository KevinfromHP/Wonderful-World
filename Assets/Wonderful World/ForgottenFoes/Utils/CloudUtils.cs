using EnigmaticThunder.Util;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

namespace ForgottenFoes.Utils
{
    public static class CloudUtils
    {
        #region Catalog Helpers
        /// <summary>
        /// Adds a GameObject to the projectile catalog and returns true
        /// GameObject must be non-null and have a ProjectileController component
        /// returns false if GameObject is null or is missing the component        
        /// </summary>
        /// <param name="projectileObject">The projectile to register to the projectile catalog.</param>
        /// <returns></returns>
        public static bool RegisterNewProjectile(GameObject projectileObject)
        {
            if (projectileObject.HasComponent<ProjectileController>())
            {
                EnigmaticThunder.Modules.Projectiles.RegisterProjectile(projectileObject);
                LogCore.LogD("Registered projectile " + projectileObject.name + " to the projectile catalog!");
                return true;
            }
            LogCore.LogF("FATAL ERROR:" + projectileObject.name + " failed to register to the projectile catalog!");
            return false;
        }
        /// <summary>
        /// Adds a GameObject to the body catalog and returns true
        /// GameObject must be non-null and have a CharacterBody component
        /// returns false if GameObject is null or is missing the component
        /// </summary>
        /// <param name="bodyObject">The body to register to the body catalog.</param>
        /// <returns></returns>
        public static bool RegisterNewBody(GameObject bodyObject)
        {
            if (bodyObject)
            {
                EnigmaticThunder.Modules.Bodies.RegisterBody(bodyObject);// += list => list.Add(bodyObject);
                LogCore.LogD("Registered body " + bodyObject.name + " to the body catalog!");
                return true;
            }
            LogCore.LogF("FATAL ERROR:" + bodyObject.name + " failed to register to the body catalog!");
            return false;
        }
        /// <summary>
        /// Adds a GameObject to the master catalog and returns true
        /// GameObject must be non-null and have a CharacterMaster component
        /// returns false if GameObject is null or is missing the component
        /// </summary>
        /// <param name="master">The master to register to the master catalog.</param>
        /// <returns></returns>
        public static bool RegisterNewMaster(GameObject master)
        {
            if (master && master.HasComponent<CharacterMaster>())
            {
                EnigmaticThunder.Modules.Masters.RegisterMaster(master);
                LogCore.LogD("Registered master " + master.name + " to the master catalog!");
                return true;
            }
            LogCore.LogF("FATAL ERROR: " + master.name + " failed to register to the master catalog!");
            return false;
        }
        /// <summary>
        /// Destroys all the AISkillDrivers on the master GameObject
        /// </summary>
        /// <param name="masterObject"></param>
        public static void DestroySkillDrivers(GameObject master)
        {
            foreach (AISkillDriver skill in master.GetComponentsInChildren<AISkillDriver>())
            {
                Object.Destroy(skill);
            }
        }


        #endregion
        #region R2API Expanded
        #endregion
        #region Projectiles


        /// <summary>
        /// Creates a valid projectile from a GameObject 
        /// </summary>
        /// <param name="projectile"></param>
        public static void CreateValidProjectile(GameObject projectile, float lifeTime, float velocity, bool updateAfterFiring)
        {
            var networkIdentity = projectile.AddComponent<NetworkIdentity>();
            var teamFilter = projectile.AddComponent<TeamFilter>();
            var projectileController = projectile.AddComponent<ProjectileController>();
            var networkTransform = projectile.AddComponent<ProjectileNetworkTransform>();
            var projectileSimple = projectile.AddComponent<ProjectileSimple>();
            var projectileDamage = projectile.AddComponent<ProjectileDamage>();

            //setup the projectile controller
            projectileController.allowPrediction = false;
            projectileController.predictionId = 0;
            projectileController.procCoefficient = 1;
            projectileController.owner = null;

            //setup the network transform
            networkTransform.allowClientsideCollision = false;
            networkTransform.interpolationFactor = 1;
            networkTransform.positionTransmitInterval = 0.03333334f;

            //setup the projectile simple
            projectileSimple.desiredForwardSpeed = velocity;
            projectileSimple.lifetime = lifeTime;
            projectileSimple.updateAfterFiring = updateAfterFiring;
            projectileSimple.enableVelocityOverLifetime = false;
            //projectileSimple.velocityOverLifetime = UnityEngine.AnimationCurve.;
            projectileSimple.oscillate = false;
            projectileSimple.oscillateMagnitude = 20;
            projectileSimple.oscillateSpeed = 0;

            projectileDamage.damage = 0;
            projectileDamage.crit = false;
            projectileDamage.force = 0;
            projectileDamage.damageColorIndex = DamageColorIndex.Default;
            projectileDamage.damageType = DamageType.Shock5s;
        }
        #endregion
        #region AI
        /// <summary>
        /// Copies skilldriver settings from "beingCopiedFrom" to "copier"
        /// Don't forget to set requiredSkill!
        /// </summary>
        /// <param name="beingCopiedFrom"></param>
        /// <param name="copier"></param>
        public static void CopyAISkillSettings(AISkillDriver beingCopiedFrom, AISkillDriver copier)
        {
            copier.activationRequiresAimConfirmation = beingCopiedFrom.activationRequiresAimConfirmation;
            copier.activationRequiresTargetLoS = beingCopiedFrom.activationRequiresTargetLoS;
            copier.aimType = beingCopiedFrom.aimType;
            copier.buttonPressType = beingCopiedFrom.buttonPressType;
            copier.customName = beingCopiedFrom.customName;
            copier.driverUpdateTimerOverride = beingCopiedFrom.driverUpdateTimerOverride;
            copier.ignoreNodeGraph = beingCopiedFrom.ignoreNodeGraph;
            copier.maxDistance = beingCopiedFrom.maxDistance;
            copier.maxTargetHealthFraction = beingCopiedFrom.maxTargetHealthFraction;
            copier.maxUserHealthFraction = beingCopiedFrom.maxUserHealthFraction;
            copier.minDistance = beingCopiedFrom.minDistance;
            copier.minTargetHealthFraction = beingCopiedFrom.minTargetHealthFraction;
            copier.minUserHealthFraction = beingCopiedFrom.minUserHealthFraction;
            copier.moveInputScale = beingCopiedFrom.moveInputScale;
            copier.movementType = beingCopiedFrom.movementType;
            copier.moveTargetType = beingCopiedFrom.moveTargetType;
            copier.nextHighPriorityOverride = beingCopiedFrom.nextHighPriorityOverride;
            copier.noRepeat = beingCopiedFrom.noRepeat;
            //Don't do this because the skilldef is not the same.
            //_out.requiredSkill = _in.requiredSkill;
            copier.requireEquipmentReady = beingCopiedFrom.requireEquipmentReady;
            copier.requireSkillReady = beingCopiedFrom.requireSkillReady;
            copier.resetCurrentEnemyOnNextDriverSelection = beingCopiedFrom.resetCurrentEnemyOnNextDriverSelection;
            copier.selectionRequiresOnGround = beingCopiedFrom.selectionRequiresOnGround;
            copier.selectionRequiresTargetLoS = beingCopiedFrom.selectionRequiresTargetLoS;
            copier.shouldFireEquipment = beingCopiedFrom.shouldFireEquipment;
            copier.shouldSprint = beingCopiedFrom.shouldSprint;
            //shouldTapButton is deprecated, don't use it!
            //_out.shouldTapButton = _in.shouldTapButton;
            copier.skillSlot = beingCopiedFrom.skillSlot;

        }
        #endregion
        #region UNITY2ROR2
        public static void AddExplosionForce(CharacterMotor body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 0, bool useWearoff = false)
        {
            var dir = (body.transform.position - explosionPosition);

            Vector3 baseForce = Vector3.zero;

            if (useWearoff)
            {
                float wearoff = 1 - (dir.magnitude / explosionRadius);
                baseForce = dir.normalized * explosionForce * wearoff;
            }
            else
            {
                baseForce = dir.normalized * explosionForce;
            }
            //baseForce.z = 0;
            body.ApplyForce(baseForce);

            //if (upliftModifier != 0)
            //{
            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            //upliftForce.z = 0;
            body.ApplyForce(upliftForce);
            //}

        }
        #endregion

        public static HitBoxGroup FindHitBoxGroup(string groupName, Transform modelTransform)
        {
            if (!modelTransform)
            {
                return null;
            }
            HitBoxGroup result = null;
            List<HitBoxGroup> gameObjectComponents = GetComponentsCache<HitBoxGroup>.GetGameObjectComponents(modelTransform.gameObject);
            int i = 0;
            int count = gameObjectComponents.Count;
            while (i < count)
            {
                if (gameObjectComponents[i].groupName == groupName)
                {
                    result = gameObjectComponents[i];
                    break;
                }
                i++;
            }
            GetComponentsCache<HitBoxGroup>.ReturnBuffer(gameObjectComponents);
            return result;
        }

        /*public static void RefreshALLBuffStacks(CharacterBody body, BuffDef def, float duration)
        {
            int num6 = 0;
            for (int j = 0; j < body.timedBuffs.Count; j++)
            {
                if (body.timedBuffs[j].buffIndex == def.buffIndex)
                {
                    num6++;
                    if (body.timedBuffs[j].timer < duration)
                    {
                        body.timedBuffs[j].timer = duration;
                    }
                }
            }
        }*/


        public static void AlterCurrentPostProcessing(PostProcessProfile profile, float weight = 0.85f)
        {
            PostProcessVolume ppv = FindCurrentPostProcessing();
            if (ppv)
            {
                ppv.profile = profile;
                ppv.weight = weight;
            }
        }

        public static void PostProcessingOverlay(PostProcessProfile profile, float weight = 0.85f, float blendDistance = 227.5f, float priority = 9999)
        {
            //basic rundown, some scenes like the Moon and Limbo will throw a hissy fit (the moon morso) if you alter their post processing,
            //to get around this, i basically add an overlay over the current post processing, which won't be touched by stupid shit (fuck you Hook Lighting Into Post Processing Volume)
            //therefore, nothing is altered, and nothing acts like a 4 year old in a burger king
            PostProcessVolume ppv = FindCurrentPostProcessing();
            if (ppv)
            {
                PostProcessVolume ppvv = ppv.AddComponent<PostProcessVolume>();

                //know your fucking place, trash
                ppv.priority /= ppv.priority;
                ppv.weight /= ppv.weight;
                ppv.blendDistance /= ppv.blendDistance;



                ppvv.profile = profile;
                ppvv.weight = weight;
                ppvv.priority = priority;
                ppvv.blendDistance = blendDistance;
            }
        }

        public static PostProcessVolume FindCurrentPostProcessing()
        {
            PostProcessVolume postProcessVolume = null;
            SceneInfo instance = SceneInfo.instance;

            if (instance)
            {
                postProcessVolume = instance.GetComponent<PostProcessVolume>();
            }
            if (!postProcessVolume)
            {
                GameObject postProcessing = GameObject.Find("PP + Amb");
                if (!postProcessing)
                {
                    postProcessing = GameObject.Find("PP, Global");
                }
                if (!postProcessing)
                {
                    postProcessing = GameObject.Find("GlobalPostProcessVolume, Base");
                }
                if (postProcessing)
                {
                    postProcessVolume = postProcessing.GetComponent<PostProcessVolume>();
                }
                else
                {
                    postProcessVolume = null;
                }
            }
            return postProcessVolume;
        }

        public static Vector3 FindBestPosition(HurtBox target)
        {
            float radius = 15f;
            var originPoint = target.transform.position;
            originPoint.x += UnityEngine.Random.Range(-radius, radius);
            originPoint.z += UnityEngine.Random.Range(-radius, radius);
            originPoint.y += UnityEngine.Random.Range(radius, radius);
            return originPoint;
        }

        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static List<T> Join<T>(this List<T> first, List<T> second)
        {
            if (first == null)
            {
                return second;
            }
            if (second == null)
            {
                return first;
            }

            return first.Concat(second).ToList();
        }


        #region Skills
        /// <summary>
        /// Destroys generic skill components attached to the survivor object and creates an empty SkillFamily.
        /// </summary>
        /// <param name="survivor"></param>
        public static void CreateEmptySkills(GameObject survivor)
        {
            if (survivor)
            {
                DestroyGenericSkillComponents(survivor);
                CreateEmptySkillFamily(survivor);
            }
            else LogCore.LogF("Tried to create empty skills on a null GameObject!");
        }
        /// <summary>
        /// Destroys generic skill components attached to the survivor object
        /// </summary>
        /// <param name="survivor"></param>
        public static void DestroyGenericSkillComponents(GameObject survivor)
        {
            foreach (GenericSkill skill in survivor.GetComponentsInChildren<GenericSkill>())
            {
                Object.DestroyImmediate(skill);
            }
        }

        /// <summary>
        /// Sets up an array full of render infos so they like hopoo models
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CharacterModel.RendererInfo[] GatherRenderInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] renderInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                renderInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false //We allow the mesh to be affected by overlays like OnFire or PredatoryInstinctsCritOverlay.
                };
            }

            return renderInfos;

        }

        /// <summary>
        /// Safely removes a buff from the target character body
        /// </summary>
        /// <param name="buffToRemove">The buff you want to safely remove</param>
        /// <param name="body">The body you safely want to remove a buff from.</param>
        public static void SafeRemoveBuff(BuffDef buffToRemove, CharacterBody body)
        {
            if (body && body.HasBuff(buffToRemove))
            {
                body.RemoveBuff(buffToRemove);
            }
        }
        /// <summary>
        /// Safely removes buffs from the target character body
        /// </summary>
        /// <param name="buffToRemove">The buff you want to safely remove</param>
        /// <param name="body">The body you safely want to remove buffs from.</param>
        public static void SafeRemoveBuffs(BuffDef buffToRemove, CharacterBody body, int stacksToRemove)
        {
            if (body)
            {
                for (int i = 0; i < stacksToRemove; i++)
                {
                    SafeRemoveBuff(buffToRemove, body);
                }
            }
        }

        /// <summary>
        /// Safely removes ALL of target buff from the target character body
        /// </summary>
        /// <param name="buffToRemove">The buff you want to safely remove all of</param>
        /// <param name="body">The body you safely want to remove buffs from.</param>
        public static void SafeRemoveAllOfBuff(BuffDef buffToRemove, CharacterBody body)
        {
            if (body)
            {
                int stacks = body.GetBuffCount(buffToRemove);
                for (int i = 0; i < stacks; i++)
                {
                    body.RemoveBuff(buffToRemove);
                }

            }
        }

        /// <summary>
        /// Creates an EmptySkillFamily. Be sure to call DestroyGenericSkillComponents before doing this.
        /// </summary>
        /// <param name="survivor"></param>
        public static void CreateEmptySkillFamily(GameObject survivor)
        {
            SkillLocator skillLocator = survivor.GetComponent<SkillLocator>();
            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);
            {
                skillLocator.primary = survivor.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                EnigmaticThunder.Modules.Loadouts.RegisterSkillFamily(newFamily);
                skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.secondary = survivor.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                EnigmaticThunder.Modules.Loadouts.RegisterSkillFamily(newFamily);
                skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.utility = survivor.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                EnigmaticThunder.Modules.Loadouts.RegisterSkillFamily(newFamily);
                skillLocator.utility.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.special = survivor.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                EnigmaticThunder.Modules.Loadouts.RegisterSkillFamily(newFamily);
                skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            }
        }

        /// <summary>
        /// Copies skilldef settings from beingCopiedFrom to copier.
        /// </summary>
        /// <param name="beingCopiedFrom"></param>
        /// <param name="copier"></param>
        public static void CopySkillDefSettings(SkillDef beingCopiedFrom, SkillDef copier)
        {
            copier.activationState = beingCopiedFrom.activationState;
            copier.activationStateMachineName = beingCopiedFrom.activationStateMachineName;
            copier.baseMaxStock = beingCopiedFrom.baseMaxStock;
            copier.baseRechargeInterval = beingCopiedFrom.baseRechargeInterval;
            copier.beginSkillCooldownOnSkillEnd = beingCopiedFrom.beginSkillCooldownOnSkillEnd;
            copier.canceledFromSprinting = beingCopiedFrom.canceledFromSprinting;
            copier.dontAllowPastMaxStocks = beingCopiedFrom.dontAllowPastMaxStocks;
            copier.forceSprintDuringState = beingCopiedFrom.forceSprintDuringState;
            copier.fullRestockOnAssign = beingCopiedFrom.fullRestockOnAssign;
            copier.icon = beingCopiedFrom.icon;
            copier.interruptPriority = beingCopiedFrom.interruptPriority;
            copier.isCombatSkill = beingCopiedFrom.isCombatSkill;
            copier.keywordTokens = beingCopiedFrom.keywordTokens;
            copier.mustKeyPress = beingCopiedFrom.mustKeyPress;
            copier.canceledFromSprinting = beingCopiedFrom.canceledFromSprinting;
            copier.rechargeStock = beingCopiedFrom.rechargeStock;
            copier.cancelSprintingOnActivation = beingCopiedFrom.cancelSprintingOnActivation;
            copier.skillDescriptionToken = beingCopiedFrom.skillDescriptionToken;
            copier.skillName = beingCopiedFrom.skillName;
            copier.skillNameToken = beingCopiedFrom.skillNameToken;
            copier.stockToConsume = beingCopiedFrom.stockToConsume;
        }

        public static CharacterModel GetCharacterModelFromCharacterBody(CharacterBody body)
        {
            var modelLocator = body.modelLocator;
            if (modelLocator)
            {
                var modelTransform = body.modelLocator.modelTransform;
                if (modelTransform)
                {
                    var model = modelTransform.GetComponent<CharacterModel>();
                    if (model)
                    {
                        return model;
                    }
                }

            }
            return null;
        }
        #endregion
        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }
    }
}