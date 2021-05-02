using System;
using UnityEngine;
using UnityEngine.Events;

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
    }

}