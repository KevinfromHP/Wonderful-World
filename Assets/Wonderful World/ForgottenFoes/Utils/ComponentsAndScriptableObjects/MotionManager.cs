﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BepInEx.Configuration;
using EnigmaticThunder.Modules;
using EnigmaticThunder.Util;
using EntityStates;
using ForgottenFoes.EntityStates.ImpSorcerer;
using ForgottenFoes.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Networking;
using UnityEngine.Events;


namespace ForgottenFoes.Utils
{
    public class MotionManager : MonoBehaviour
    {
        public GameObject crystalPrefab;
        public Transform referenceObject;
        public float acceleration = 20f;
        public float smoothTime = 1f;
        private Vector3 velocity = Vector3.zero;
        private Rigidbody rigidBody;


        public void Initialize(ParticleSystem particleSystem)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[3];
            Debug.Log("1");
            particleSystem.GetParticles(particles);
            Debug.Log("2");
            if (particleSystem.transform.parent && particleSystem.transform.parent.parent && particleSystem.transform.parent.parent.parent)
            {
                Debug.Log("3");
                var crystalLocator = particleSystem.transform.parent.parent.parent.GetComponent<CrystalLocator>();
                for (int i = 0; i < 3; i++)
                {
                    GameObject crystal = Instantiate(crystalPrefab, particles[i].position, Quaternion.Euler(particles[i].rotation3D + new Vector3(90f, 0f)));
                    crystal.GetComponent<MotionManager>().referenceObject = crystalLocator.crystals[i].crystalObject;
                    crystalLocator.crystals[i].hasCrystal = true;
                    crystal.SetActive(true);
                }
            }
        }

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            if (!referenceObject)
                Destroy(gameObject);
            else
            {
                UpdateMotion();
                UpdateRotation();
            }
        }

        private void UpdateMotion()
        {
            Vector3 desiredPosition = referenceObject.position;
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime, 6f);
        }

        /*private void UpdateMotion()
        {
            var desiredPosition = (originalCrystalsObject.transform.position - gameObject.transform.position) / 1.2f + gameObject.transform.position;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, damping, 14f);
            for (int i = 0; i < 3; i++)
            {
                //crystals[i].localPosition = Vector3.SmoothDamp(crystals[i].localPosition, originalCrystalsObject.transform.Find("Offset" + i).localPosition, ref velocityOffsets[i], damping, 14f);
                var position = (originalCrystals[i].position - rigidBodies[i].position) / 1.2f + rigidBodies[i].position;
                rigidBodies[i].MovePosition(position);
            }
        }*/

        private void UpdateRotation()
        {
            transform.forward = Vector3.RotateTowards(transform.forward, referenceObject.forward, 90f * Mathf.Deg2Rad * Time.deltaTime, 0f);
            //rigidBody.MoveRotation(referenceObject.rotation);
        }

    }
}