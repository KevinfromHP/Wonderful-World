using System;
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
    public class CrystalMotionManager : MonoBehaviour
    {
        public Transform referenceObject;
        public float smoothTime = .1f;
        public float maxVelocityFactor = 5;


        private CrystalManager crystalManager;
        private Vector3 velocity = Vector3.zero;
        private Vector3 desiredPosition;
        private Transform proximityTransform;
        public State state = State.FollowImp;
        private float scaleStopwatch;
        private Vector3 radius;
        private SphereCollider sphereCollider;
        private bool destroyedGhost;

        public enum State
        {
            FollowImp,
            IsPlanting,
            HasPlanted
        }

        //I am feral and cannot be stopped
        private void Start()
        {
            var owner = GetComponent<ProjectileController>().owner;
            if (owner)
            {
                crystalManager = owner.GetComponent<ModelLocator>().modelTransform.GetComponent<CrystalManager>();
                crystalManager.RequestReference(this);
                proximityTransform = gameObject.transform.Find("ProximityTrigger");
            }
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            if (!referenceObject)
                Destroy(gameObject);
            else
                UpdateMovement();
        }
        private void UpdateMovement()
        {
            float lhs;

            //The state just decides which of these it does. if it's planted, scale up the proximity trigger. If it's planting, go to the plant site. If not, just follow the Imp.
            switch (state)
            {
                case State.HasPlanted:
                    scaleStopwatch += Time.deltaTime;
                    proximityTransform.localScale = Vector3.Lerp(proximityTransform.localScale, radius, Time.deltaTime * 3f);
                    break;
                case State.IsPlanting:
                    lhs = Vector3.Distance(transform.position, desiredPosition);
                    if (lhs < 0.05f && 1 - transform.up.y < 0.0015f)
                    {
                        LogCore.LogW("Crystal has arrived at desiredPosition.");
                        velocity = Vector3.zero;

                        sphereCollider = proximityTransform.gameObject.GetComponent<SphereCollider>();
                        radius = proximityTransform.localScale;
                        proximityTransform.localScale = Vector3.zero;
                        sphereCollider.enabled = true;
                        proximityTransform.GetComponent<MeshRenderer>().enabled = true;
                        proximityTransform.localPosition = Vector3.zero;
                        state = State.HasPlanted;
                    }
                    transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime, 6f);
                    transform.up = Vector3.RotateTowards(transform.up, Vector3.up, 70f * Mathf.Deg2Rad * Time.deltaTime, 0f);
                    break;
                default:
                    desiredPosition = referenceObject.position;
                    transform.forward = Vector3.RotateTowards(transform.forward, referenceObject.forward, 90f * Mathf.Deg2Rad * Time.deltaTime, 0f);
                    lhs = Vector3.Distance(transform.position, desiredPosition);
                    transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime, Mathf.Max(lhs * maxVelocityFactor, 10f));
                    break;
            }
        }

        public void DeployAsMine(Vector3 plantPosition)
        {
            state = State.IsPlanting;
            desiredPosition = plantPosition;
            //the 0.055 give it a bit more clearance to stick out of the ground
            desiredPosition.y += 0.055f;
            smoothTime = 0.3f;
            crystalManager.RemoveThisCrystal(gameObject);
        }

        public void JoeRogan()
        {
            LogCore.LogD("jamie pull that one up");
            var ghost = gameObject.GetComponent<ProjectileController>().ghost;
            Destroy(ghost.gameObject);
        }

        //moves up the y by 3.1 meters go fuck yourself
        public void KissMyAss()
        {
            Vector3 eatShit = transform.localPosition;
            eatShit.y += 3.1f;
            transform.localPosition = eatShit;
            //gameObject.GetComponent<ProjectileImpactExplosion>().SetAlive(false);
        }

    }
}