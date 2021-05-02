using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace ForgottenFoes.Utils
{
    [Serializable]
    public class OnParticleEndEventSaveData : MonoBehaviour
    {
        public void Update()
        {
            particleSystemToTrack.GetParticles(particles);
            if (particleSystemToTrack && !particleEnded && (particles[0].remainingLifetime <= 0.01f && stopwatch > 0.01f))
            {
                particleEnded = true;
                onEnd.Invoke();
            }
            stopwatch += Time.deltaTime;
        }

        public ParticleSystem.Particle[] particles = new ParticleSystem.Particle[3];
        public ParticleSystem particleSystemToTrack;
        float stopwatch;
        public UnityEvent onEnd;

        private bool particleEnded;
    }
}
