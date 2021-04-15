using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace ForgottenFoes
{
    public class ProjectileSteerAboveTarget : MonoBehaviour
    {
        public bool yAxisOnly;
        public float maxVelocity;
        private float velocity;
        private Vector3 velocityAsVector;
        private new Transform transform;
        private ProjectileTargetComponent targetComponent;
        private ProjectileSimple projectileSimple;
        private Transform model;
        private void Start()
        {
            if (!NetworkServer.active)
            {
                enabled = false;
                return;
            }
            transform = gameObject.transform;
            targetComponent = GetComponent<ProjectileTargetComponent>();
            projectileSimple = GetComponent<ProjectileSimple>();
            model = gameObject.transform.Find("Model");
            velocity = maxVelocity;
            velocityAsVector = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (targetComponent.target)
            {
                Vector3 vector = targetComponent.target.transform.position + new Vector3(0f, 10f) - transform.position;
                Vector3 movePosition = targetComponent.target.transform.position + new Vector3(0f, 10f);
                //if (Mathf.Abs(vector.y) < 0.1f)
                //  vector.y = 0f;
                if (vector.sqrMagnitude < 1.3f)
                {
                    velocity -= 0.15f;
                    if (velocity < 0.15f)
                        velocity = 0.15f;
                }
                else
                if (velocity < 13f)
                    velocity += 0.2f;
                if (velocity > 13f)
                    velocity = 13f;
                if (vector != Vector3.zero)
                    transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocityAsVector, vector.magnitude / velocity, maxVelocity, Time.deltaTime);
            }
        }
    }

}