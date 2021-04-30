using R2API;
using RoR2;
using UnityEngine;

//The only reason this exists is because R2API's director card holder is not compatible with the unity editor.
namespace ForgottenFoes.Utils
{
    [CreateAssetMenu(fileName = "Director Card Holder", menuName = "ForgottenFoes/Director Card Holder", order = 1)]
    public class ForgottenFoesDirectorCardHolder : ScriptableObject
    {
        public DirectorAPI.Stage[] stagesToSpawnOn;
        public DirectorAPI.MonsterCategory category;
        public DirectorCard card;
    }
}
