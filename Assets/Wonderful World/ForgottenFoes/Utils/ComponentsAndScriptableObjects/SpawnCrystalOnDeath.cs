using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace ForgottenFoes.Utils
{
    [Serializable]
    public class SpawnCrystalOnDeath : MonoBehaviour
    {
        public CrystalManager locator;
        public ParticleSystem particleSystemToTrack;
        public void Start()
        {
            particles = new ParticleSystem.Particle[particleSystemToTrack.main.maxParticles];
        }
        public void Update()
        {
            int particlesAlive = particleSystemToTrack.GetParticles(particles);
            if (particleSystemToTrack && locator && stopwatch > 0f)
            {
                bool disable = false;
                for (int i = 0; i < 3; i++)
                {
                    if (stopwatch >= particleSystemToTrack.main.startLifetime.constant)
                    {
                        locator.SpawnCrystal(cachedPositions[i], cachedRotations[i]);
                        disable = true;
                    }
                    else
                    {
                        cachedPositions[i] = particles[i].position;
                        cachedRotations[i] = particles[i].rotation3D;
                    }
                }
                if (disable)
                    enabled = false;
            }
            stopwatch += Time.deltaTime;

        }

        float stopwatch = 0f;
        private ParticleSystem.Particle[] particles;
        private Vector3[] cachedPositions = new Vector3[3];
        private Vector3[] cachedRotations = new Vector3[3];

    }
}
