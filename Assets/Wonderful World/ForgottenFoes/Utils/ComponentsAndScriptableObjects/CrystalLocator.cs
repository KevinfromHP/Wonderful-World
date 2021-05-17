using System;
using UnityEngine;
using UnityEngine.Events;
using RoR2;
using UnityEngine.Networking;

namespace ForgottenFoes.Utils
{
    public class CrystalLocator : NetworkBehaviour
    {
        public CrystalTransform[] crystals;

        public struct CrystalTransform
        {
            public Transform crystalObject;
            public bool hasCrystal;
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
                        crystalObject = locator.FindChild("Crystal" + i),
                        hasCrystal = false
                    };
                }
            }
        }

        public void SpawnCrystal(Vector3 position, Vector3 rotation)
        {
            for (int i = 0; i < crystals.Length; i++)
            {
                if (!crystals[i].hasCrystal)
                {
                    LogCore.LogD("Spawning Imp Sorcerer Crystal");
                    GameObject crystalFollower = Instantiate(crystalPrefab, position, Quaternion.Euler(rotation + new Vector3(90f, 0f)));
                    crystalFollower.GetComponent<CrystalMotionManager>().referenceObject = crystals[i].crystalObject;
                    NetworkServer.Spawn(crystalFollower);
                    crystals[i].hasCrystal = true;
                }
            }
        }

        public static GameObject crystalPrefab;
    }
}