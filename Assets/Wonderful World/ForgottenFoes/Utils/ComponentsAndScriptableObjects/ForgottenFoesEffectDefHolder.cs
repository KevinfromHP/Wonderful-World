using UnityEngine;
using RoR2;

namespace ForgottenFoes.Utils
{
    //EffectDefs aren't scriptable Objects atm so this is a workaround
    [CreateAssetMenu(fileName = "EffectDef Holder", menuName = "ForgottenFoes/EffectDef Holder", order = 2)]
    public class ForgottenFoesEffectDefHolder : ScriptableObject
    {
        public GameObject effectPrefab;

        public EffectDef ToEffectDef()
        {
            return new EffectDef(effectPrefab);
        }
        
    }
}