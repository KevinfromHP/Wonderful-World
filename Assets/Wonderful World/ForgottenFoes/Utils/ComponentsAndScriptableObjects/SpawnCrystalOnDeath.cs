using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace ForgottenFoes.Utils
{
    [Serializable]
    public class SpawnCrystalOnDeath : MonoBehaviour
    {
        public CrystalLocator locator;
        public ParticleSystem particleSystemToTrack;
        public void Start()
        {
            particles = new ParticleSystem.Particle[particleSystemToTrack.main.maxParticles];
        }
        public void Update()
        {
            particleSystemToTrack.GetParticles(particles, 3);
            if (particleSystemToTrack && particleSystemToTrack.particleCount > 0 && locator)
            {
                foreach (ParticleSystem.Particle particle in particles)
                    if (particle.remainingLifetime <= 0f)
                        locator.SpawnCrystal(particle.position, particle.rotation3D);
            }
        }

        private ParticleSystem.Particle[] particles;
    }
}
