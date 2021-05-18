using System;
using UnityEngine;
using UnityEngine.Events;
using RoR2;
using UnityEngine.Networking;
using RoR2.Projectile;

namespace ForgottenFoes.Utils
{
    public class CrystalManager : NetworkBehaviour
    {
        public float damageCoefficient = 4.5f;
        public CrystalTransform[] crystals;

        public struct CrystalTransform
        {
            public Transform crystalReference;
            public GameObject crystal;
            public CrystalState crystalState;
        }

        public enum CrystalState
        {
            NoCrystal,
            AwaitingCrystal,
            HasCrystal
        }

        public void Awake()
        {
            ChildLocator locator = GetComponent<ChildLocator>();
            if (locator)
            {
                crystals = new CrystalTransform[3];
                for (int i = 1; i <= 3; i++)
                {
                    crystals[i - 1] = new CrystalTransform()
                    {
                        crystalReference = locator.FindChild("Crystal" + i),
                        crystal = null,
                        crystalState = CrystalState.NoCrystal
                    };
                }
            }
        }

        public void SpawnCrystal(Vector3 position, Vector3 rotation)
        {
            for (int i = 0; i < crystals.Length; i++)
            {
                if (crystals[i].crystalState == CrystalState.NoCrystal)
                {
                    LogCore.LogD("Spawning Imp Sorcerer Crystal");
                    var body = gameObject.GetComponent<CharacterModel>().body;
                    GameObject crystalPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("ImpSorcererCrystalProjectile");
                    ProjectileManager.instance.FireProjectile(crystalPrefab, position, Quaternion.Euler(rotation + new Vector3(90f, 0f)), body.gameObject, body.damage * damageCoefficient, 0f, false);
                    crystals[i].crystalState = CrystalState.AwaitingCrystal;
                }
            }
        }

        public void RequestReference(CrystalMotionManager crystal)
        {
            for (int i = 0; i < crystals.Length; i++)
            {
                if (crystals[i].crystalState == CrystalState.AwaitingCrystal)
                {
                    crystals[i].crystal = crystal.gameObject;
                    crystal.referenceObject = crystals[i].crystalReference.transform;
                    crystals[i].crystalState = CrystalState.HasCrystal;
                    return;
                }
            }
            LogCore.LogE("Error: Imp Sorcerer Crystal spawning failed. No valid reference object found.");
        }

        public void RemoveThisCrystal(GameObject crystal)
        {
            for (int i = 0; i < crystals.Length; i++)
                if (crystals[i].crystal.Equals(crystal))
                    crystals[i].crystalState = CrystalState.NoCrystal;
        }

        public GameObject GetCrystal()
        {
            foreach (CrystalTransform crystal in crystals)
                if (crystal.crystal && crystal.crystal.GetComponent<CrystalMotionManager>().state != CrystalMotionManager.State.HasPlanted)
                    return crystal.crystal;
            return null;
        }
    }
}