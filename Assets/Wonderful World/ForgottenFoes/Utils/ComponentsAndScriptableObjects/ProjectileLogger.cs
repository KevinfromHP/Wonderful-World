using System.Collections;
using UnityEngine;
using RoR2.Projectile;
using RoR2;

namespace ForgottenFoes.Utils
{
    //[CreateAssetMenu(fileName = "Projectile Logger", menuName = "ForgottenFoes/ProjectileLogger", order = 99)]
    public class ProjectileLogger : MonoBehaviour, IProjectileImpactBehavior
    {
        public void OnProjectileImpact(ProjectileImpactInfo info)
        {
            if (info.collider)
            {
                LogCore.LogW("Yes, it collided with collider");
                LogCore.LogW("HurtBox: " + info.collider.GetComponent<HurtBox>());
            }
            else
                LogCore.LogW("Did not collide.");
            LogCore.LogW("Point of collision: " + info.estimatedPointOfImpact);
            LogCore.LogW("Who gives a shit about this thing: " + info.estimatedImpactNormal);

            LogCore.LogW("component rotation :" + transform.eulerAngles);
        }
    }
}