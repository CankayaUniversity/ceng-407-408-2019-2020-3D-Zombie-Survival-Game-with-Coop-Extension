/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using UnityEngine;
using UnityEngine.AI;
using UnrealFPS.Runtime;

namespace UnrealFPS.AI
{
    [RequireComponent(typeof(AIController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIAnimatorHandler : MonoBehaviour
    {
        public const string SPEED = "Speed";
        public const string DIRECTION = "Direction";
        public const string CROUCH = "IsCrouching";
        public const string SHOOT = "IsShooting";
        public const string RELOAD = "IsReloading";

        public readonly static int SPEED_HASH = Animator.StringToHash(SPEED);
        public readonly static int DIRECTION_HASH = Animator.StringToHash(DIRECTION);
        public readonly static int CROUCH_HASH = Animator.StringToHash(CROUCH);
        public readonly static int SHOOT_HASH = Animator.StringToHash(SHOOT);
        public readonly static int RELOAD_HASH = Animator.StringToHash(RELOAD);

        private bool isAiming;
        private bool isCrouching;

        private Vector2 smoothDeltaPosition;
        private Vector2 velocity;

        private AIController behaviour;
        private IWeaponAttack attackCallbacks;
        private IWeaponReloading weaponReloading;
        private Animator animator;
        private NavMeshAgent navMeshAgent;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            behaviour = GetComponent<AIController>();
            smoothDeltaPosition = Vector2.zero;
            velocity = Vector2.zero;
            navMeshAgent.updatePosition = false;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            attackCallbacks = behaviour.GetAIAttackSystem();
            weaponReloading = behaviour.GetAIAttackSystem().GetReloadSystem();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            Vector3 worldDeltaPosition = navMeshAgent.nextPosition - transform.position;

            float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dx, dy);

            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            if (Time.deltaTime > 1e-5f)
                velocity = smoothDeltaPosition / Time.deltaTime;

            animator.SetFloat(SPEED_HASH, velocity.y);
            animator.SetFloat(DIRECTION_HASH, velocity.x);
            animator.SetBool(CROUCH_HASH, behaviour.IsCrouch());
            animator.SetBool(SHOOT_HASH, attackCallbacks.IsAttacking());
            animator.SetBool(RELOAD_HASH, weaponReloading.IsReloading());
        }

        /// <summary>
        /// Callback for processing animation movements for modifying root motion.
        /// </summary>
        private void OnAnimatorMove()
        {
            transform.position = navMeshAgent.nextPosition;
        }

        public bool IsAiming()
        {
            return isAiming;
        }

        public bool IsCrouching()
        {
            return isCrouching;
        }

        protected void IsAiming(bool isAiming)
        {
            this.isAiming = isAiming;
        }

        protected void IsCrouching(bool isCrouching)
        {
            this.isCrouching = isCrouching;
        }

        public Vector2 GetSmoothDeltaPosition()
        {
            return smoothDeltaPosition;
        }

        public Vector2 GetVelocity()
        {
            return velocity;
        }

        public AIController GetBehaviour()
        {
            return behaviour;
        }

        public IWeaponAttack GetAttackCallbacks()
        {
            return attackCallbacks;
        }

        public IWeaponReloading GetReloadingCallbacks()
        {
            return weaponReloading;
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        public NavMeshAgent GetNavMeshAgent()
        {
            return navMeshAgent;
        }
    }
}