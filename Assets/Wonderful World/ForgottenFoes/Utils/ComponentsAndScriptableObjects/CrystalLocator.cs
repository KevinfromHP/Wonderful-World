using System;
using UnityEngine;
using UnityEngine.Events;
using RoR2;

namespace ForgottenFoes.Utils
{
    public class CrystalLocator : MonoBehaviour
    {
        public CrystalTransform[] crystals;

        [Serializable]
        public struct CrystalTransform
        {
            public Transform crystalObject;
            public bool hasCrystal;
        }

        public void SpawnCrystal(Vector3 position, Vector3 rotation)
        {
            for (int i = 0; i < crystals.Length; i++)
                if (!crystals[i].hasCrystal)
                {
                    GameObject crystalFollower = Instantiate(crystalPrefab, position, Quaternion.Euler(rotation + new Vector3(90f, 0f)));
                    crystalFollower.GetComponent<CrystalMotionManager>().referenceObject = crystals[i].crystalObject;
                    crystals[i].hasCrystal = true;
                    crystalFollower.SetActive(true);
                }
        }

        public static GameObject crystalPrefab;
    }
}