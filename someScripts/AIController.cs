/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnrealFPS.Runtime;
using UnrealFPS.Utility;

namespace UnrealFPS.AI
{
    [RequireComponent(typeof(AIFieldOfView))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AudioSource))]
    public class AIController : MonoBehaviour, IController
    {
        #region [Movement Properties]
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float walkStepLength = 1.5f;
        [SerializeField] private float runSpeed = 5.0f;
        [SerializeField] private float runStepLength = 1.2f;
        [SerializeField] private float sprintSpeed = 7.0f;
        [SerializeField] private float sprintStepLength = 1.0f;
        [SerializeField] private float stepInterval = 5;
        [SerializeField] private float rotateSpeed = 5.0f;
        [SerializeField] private bool canCrouch = true;
        [SerializeField] private float crouchSpeed = 1.5f;
        [SerializeField] private float crouchStepLength = 2.0f;
        [SerializeField] private float changePositionDelay = 3.0f;
        [SerializeField] private FootstepProperties footstepProperties;
        #endregion

        #region [Transitions Properties]
        [SerializeField] private float stoppingFollowDistance = 3.0f;
        [SerializeField] private float followWalkDistance = 5.0f;
        [SerializeField] private float followRunDistance = 7.0f;
        [SerializeField] private float followSprintDistance = 11.0f;
        [SerializeField] private float maxShoothingDistance = 10.0f;
        [SerializeField] private float minShoothingDistance = 3.0f;
        #endregion

        [SerializeField] private Transform target;
        [SerializeField] private PatrolSystem patrolSystem = new PatrolSystem();

        private AIAttackSystem attackSystem;
        private AIFieldOfView fieldOfView;
        private NavMeshAgent navMeshAgent;
        private AudioSource audioSource;

        private Transform enemyTarget;
        private IHealth enemyHealth;
        private float speed;
        private bool isCrouch;
        private float stepCycle;
        private float nextStep;
        private RaycastHit raycastHit;
        private Vector3 chPosition;
        private bool nextCrouch;
        private float cacheChangePositionDelay;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            attackSystem = GetComponent<AIAttackSystem>();
            fieldOfView = GetComponent<AIFieldOfView>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();

            stepCycle = 0f;
            nextStep = stepCycle / 2f;;
            cacheChangePositionDelay = changePositionDelay;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            patrolSystem.Initialize(transform, navMeshAgent);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            UpdateAIStates();
            ProgressStepCycle(speed);
        }

        protected virtual void UpdateAIStates()
        {
            if (fieldOfView.GetVisibleTargetsCount() == 0 && enemyTarget == null)
            {
                attackSystem.StopShooting();
                isCrouch = false;
                if (target != null)
                {
                    UpdateFollow();
                }
                else
                {
                    navMeshAgent.speed = runSpeed;
                    patrolSystem.Update();
                }
            }
            else if (fieldOfView.GetVisibleTargetsCount() > 0)
            {
                enemyTarget = fieldOfView.GetVisibleTarget(0);
                UpdateAttack();
            }

            if (enemyHealth == null && enemyTarget != null)
            {
                enemyHealth = enemyTarget.GetComponent<IHealth>();
            }
            else if (enemyHealth != null && !enemyHealth.IsAlive())
            {
                enemyTarget = null;
                enemyHealth = null;
            }
        }

        protected virtual void UpdateFollow()
        {
            if (target == null)
            {
                navMeshAgent.isStopped = true;
                return;
            }

            Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);

            float distance = Vector3.Distance(transform.position, target.position);
            navMeshAgent.SetDestination(target.position);
            navMeshAgent.stoppingDistance = stoppingFollowDistance;

            if (distance <= followWalkDistance)
            {
                navMeshAgent.speed = walkSpeed;
            }
            else if (distance <= followRunDistance)
            {
                navMeshAgent.speed = runSpeed;
            }
            else if (distance <= followSprintDistance)
            {
                navMeshAgent.speed = sprintSpeed;
            }
        }

        protected virtual void UpdateAttack()
        {
            if (enemyTarget == null)
                return;

            float distance = Vector3.Distance(transform.position, enemyTarget.position);
            if (distance <= minShoothingDistance)
            {
                attackSystem.StartShooting(enemyTarget);
                Quaternion rotation = Quaternion.LookRotation(enemyTarget.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);

                Vector3 behindPosition = transform.position - (transform.forward * 2);
                navMeshAgent.SetDestination(behindPosition);
                isCrouch = false;
                navMeshAgent.speed = runSpeed;
            }
            else if (distance >= maxShoothingDistance)
            {
                attackSystem.StopShooting();
                navMeshAgent.SetDestination(enemyTarget.position);
                isCrouch = false;
                navMeshAgent.speed = runSpeed;
            }
            else if (distance >= minShoothingDistance && distance <= maxShoothingDistance)
            {
                attackSystem.StartShooting(enemyTarget);
                cacheChangePositionDelay -= Time.deltaTime;
                if (cacheChangePositionDelay <= 0)
                {
                    navMeshAgent.isStopped = false;
                    float radius = Mathf.Abs(distance - maxShoothingDistance);
                    nextCrouch = (canCrouch && Random.Range(0, 2) == 1) ? true : false;
                    chPosition = UMathf.RandomPositionInCircle(transform.position, radius);
                    navMeshAgent.SetDestination(chPosition);
                    cacheChangePositionDelay = changePositionDelay;
                }
                else
                {
                    float newPosDistance = Vector3.Distance(transform.position, chPosition);
                    if (newPosDistance <= 0.5f)
                    {
                        isCrouch = nextCrouch;
                        if (isCrouch) navMeshAgent.speed = crouchSpeed;
                        navMeshAgent.isStopped = true;
                    }
                    else
                    {
                        Quaternion rotation = Quaternion.LookRotation(enemyTarget.position - transform.position);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
                    }
                }
            }
        }

        /// <summary>
        /// Calculating AI step cycle.
        /// </summary>
        /// <param name="speed"></param>
        protected virtual void ProgressStepCycle(float speed)
        {
            if (navMeshAgent.velocity.sqrMagnitude > 0 && IsMoving())
            {
                stepCycle += (navMeshAgent.velocity.magnitude + (speed * GetStepLength())) *
                    Time.fixedDeltaTime;
            }

            if (!(stepCycle > nextStep))
            {
                return;
            }

            nextStep = stepCycle + stepInterval;

            AudioClip stepSound = GetStepSound();
            if (stepSound != null)
            {
                audioSource.PlayOneShot(stepSound);
            }
        }

        /// <summary>
        /// Returning step sound by current surface on which is stand AI.
        /// </summary>
        /// <returns>AudioClip: Step sound</returns>
        public virtual AudioClip GetStepSound()
        {
            if (footstepProperties == null || footstepProperties.GetLength() == 0)
                return null;

            RaycastHit footstepshit;
            if (Physics.Raycast(transform.position, Vector3.down, out footstepshit))
            {
                Object surfaceInfo = SurfaceHelper.GetSurfaceAuto(footstepshit.collider, transform.position);
                if (!surfaceInfo)
                    return null;

                for (int i = 0; i < footstepProperties.GetLength(); i++)
                {
                    if (footstepProperties.GetProperty(i).GetPhysicMaterial() == surfaceInfo || footstepProperties.GetProperty(i).GetTexture() == surfaceInfo)
                    {
                        if (footstepProperties.GetProperty(i).GetStepSoundsLength() > 0)
                        {
                            int randomSound = Random.Range(0, footstepProperties.GetProperty(i).GetStepSoundsLength());
                            return footstepProperties.GetProperty(i).GetStepSound(randomSound);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Return step length value by current AI speed.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public virtual float GetStepLength()
        {
            if (speed == crouchSpeed)
                return crouchStepLength;
            else if (speed == walkSpeed)
                return walkStepLength;
            else if (speed == runSpeed)
                return runStepLength;
            else if (speed == sprintSpeed)
                return sprintStepLength;
            return 1.0f;
        }

        public Vector3 GetVelocity()
        {
            return navMeshAgent.velocity;
        }

        public void ControllerEnabled(bool enabled)
        {
            this.enabled = enabled;
            fieldOfView.enabled = enabled;
            attackSystem.enabled = enabled;
            attackSystem.GetReloadSystem().enabled = enabled;
        }

        public float GetWalkSpeed()
        {
            return walkSpeed;
        }

        public void SetWalkSpeed(float value)
        {
            walkSpeed = value;
        }

        public float GetWalkStepLength()
        {
            return walkStepLength;
        }

        public void SetWalkStepLength(float value)
        {
            walkStepLength = value;
        }

        public float GetRunSpeed()
        {
            return runSpeed;
        }

        public void SetRunSpeed(float value)
        {
            runSpeed = value;
        }

        public float GetRunStepLength()
        {
            return runStepLength;
        }

        public void SetRunStepLength(float value)
        {
            runStepLength = value;
        }

        public float GetSprintSpeed()
        {
            return sprintSpeed;
        }

        public void SetSprintSpeed(float value)
        {
            sprintSpeed = value;
        }

        public float GetSprintStepLength()
        {
            return sprintStepLength;
        }

        public void SetSprintStepLength(float value)
        {
            sprintStepLength = value;
        }

        public float GetStepInterval()
        {
            return stepInterval;
        }

        public void SetStepInterval(float value)
        {
            stepInterval = value;
        }

        public float GetRotateSpeed()
        {
            return rotateSpeed;
        }

        public void SetRotateSpeed(float value)
        {
            rotateSpeed = value;
        }

        public bool CanCrouch()
        {
            return canCrouch;
        }

        public void CanCrouch(bool value)
        {
            canCrouch = value;
        }

        public float GetCrouchSpeed()
        {
            return crouchSpeed;
        }

        public void SetCrouchSpeed(float value)
        {
            crouchSpeed = value;
        }

        public float GetCrouchStepLength()
        {
            return crouchStepLength;
        }

        public void SetCrouchStepLength(float value)
        {
            crouchStepLength = value;
        }

        public float GetChangePositionDelay()
        {
            return changePositionDelay;
        }

        public void SetChangePositionDelay(float value)
        {
            changePositionDelay = value;
        }

        public FootstepProperties GetFootstepProperties()
        {
            return footstepProperties;
        }

        public void SetFootstepProperties(FootstepProperties value)
        {
            footstepProperties = value;
        }

        public float GetStoppingFollowDistance()
        {
            return stoppingFollowDistance;
        }

        public void SetStoppingFollowDistance(float value)
        {
            stoppingFollowDistance = value;
        }

        public float GetFollowWalkDistance()
        {
            return followWalkDistance;
        }

        public void SetFollowWalkDistance(float value)
        {
            followWalkDistance = value;
        }

        public float GetFollowRunDistance()
        {
            return followRunDistance;
        }

        public void SetFollowRunDistance(float value)
        {
            followRunDistance = value;
        }

        public float GetFollowSprintDistance()
        {
            return followSprintDistance;
        }

        public void SetFollowSprintDistance(float value)
        {
            followSprintDistance = value;
        }

        public float GetMaxShoothingDistance()
        {
            return maxShoothingDistance;
        }

        public void SetMaxShoothingDistance(float value)
        {
            maxShoothingDistance = value;
        }

        public float GetMinShoothingDistance()
        {
            return minShoothingDistance;
        }

        public void SetMinShoothingDistance(float value)
        {
            minShoothingDistance = value;
        }

        public bool IsMoving()
        {
            if (navMeshAgent.velocity.x > 0 || navMeshAgent.velocity.z > 0)
                return true;
            return false;
        }

        public float GetSpeed()
        {
            return speed;
        }

        protected void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public bool IsCrouch()
        {
            return isCrouch;
        }

        public AIAttackSystem GetAIAttackSystem()
        {
            return attackSystem;
        }

        public Transform GetTarget()
        {
            return target;
        }

        public void SetTarget(Transform value)
        {
            target = value;
        }

        public PatrolSystem GetPatrolSystem()
        {
            return patrolSystem;
        }
    }
}