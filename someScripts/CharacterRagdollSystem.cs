/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealFPS.Runtime
{
    [RequireComponent(typeof(IController))]
    public sealed class CharacterRagdollSystem : MonoBehaviour, IRagdollCallbacks
    {
        /// <summary>
        /// Declare a class that will hold useful information for each body part
        /// </summary>
        internal sealed class TransformComponent
        {
            private Transform transform;
            private Vector3 position;
            private Quaternion rotation;
            private Vector3 storedPosition;
            private Quaternion storedRotation;

            public TransformComponent(Transform transform)
            {
                this.transform = transform;
            }

            public Transform GetTransform()
            {
                return transform;
            }

            public Vector3 GetPosition()
            {
                return position;
            }

            public void SetPosition(Vector3 value)
            {
                position = value;
            }

            public Quaternion GetRotation()
            {
                return rotation;
            }

            public void SetRotation(Quaternion value)
            {
                rotation = value;
            }

            public Vector3 GetStoredPosition()
            {
                return storedPosition;
            }

            public void SetStoredPosition(Vector3 value)
            {
                storedPosition = value;
            }

            public Quaternion GetStoredRotation()
            {
                return storedRotation;
            }

            public void SetStoredRotation(Quaternion value)
            {
                storedRotation = value;
            }
        }

        internal struct RigidbodyComponent
        {
            private Rigidbody rigidBody;
            private CharacterJoint joint;
            private Vector3 connectedAnchorDefault;

            public RigidbodyComponent(Rigidbody rigidBody)
            {
                this.rigidBody = rigidBody;
                joint = rigidBody.GetComponent<CharacterJoint>();
                if (joint != null)
                    connectedAnchorDefault = joint.connectedAnchor;
                else
                    connectedAnchorDefault = Vector3.zero;
            }

            public Rigidbody GetRigidBody()
            {
                return rigidBody;
            }

            public CharacterJoint GetJoint()
            {
                return joint;
            }

            public Vector3 GetConnectedAnchorDefault()
            {
                return connectedAnchorDefault;
            }
        }

        //Possible states of the ragdoll
        internal enum RagdollState
        {
            /// <summary>
            /// Mecanim is fully in control
            /// </summary>
            Animated,
            /// <summary>
            /// Mecanim turned off, but when stable position will be found, the transition to Animated will heppend
            /// </summary>
            WaitStablePosition,
            /// <summary>
            /// Mecanim turned off, physics controls the ragdoll
            /// </summary>
            Ragdolled,
            /// <summary>
            /// Mecanim in control, but LateUpdate() is used to partially blend in the last ragdolled pose
            /// </summary>
            BlendToAnim,
        }

        /// <summary>
        /// How long do we blend when transitioning from ragdolled to animated
        /// </summary>
        public const float RAGDOLL_TO_MECANIM_BLEND_TIME = 0.5f;

        [SerializeField] private string animationGetUpFromBelly = "GetUp.GetUpFromBelly";
        [SerializeField] private string animationGetUpFromBack = "GetUp.GetUpFromBack";
        [SerializeField] private float AirSpeed = 5f; // determines the max speed of the character while airborne

        private float ragdollingEndTime;
        private RagdollState ragdollState = RagdollState.Animated;
        private Transform hipsTransform;
        private Rigidbody hipsTransformRigid;
        private Vector3 storedHipsPosition;
        private Vector3 storedHipsPositionPrivAnim;
        private Vector3 storedHipsPositionPrivBlend;

        private Animator animator;
        private IController controller;
        private List<RigidbodyComponent> rigidbodyComponents;
        private List<TransformComponent> transformComponents;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            animator = GetComponent<Animator>();
            hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
            hipsTransformRigid = hipsTransform.GetComponent<Rigidbody>();
            controller = GetComponent<IController>();

            rigidbodyComponents = new List<RigidbodyComponent>();
            transformComponents = new List<TransformComponent>();

            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>(true);
            for (int i = 0, length = rigidbodies.Length; i < length; i++)
            {
                Rigidbody rigid = rigidbodies[i];
                if (rigid.transform == transform)
                    continue;

                RigidbodyComponent rigidCompontnt = new RigidbodyComponent(rigid);
                rigidbodyComponents.Add(rigidCompontnt);
            }

            Transform[] array = GetComponentsInChildren<Transform>();
            for (int i = 0, length = array.Length; i < length; i++)
            {
                Transform t = array[i];
                TransformComponent trComp = new TransformComponent(t);
                transformComponents.Add(trComp);
            }
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            ActivateRagdollParts(false);
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void FixedUpdate()
        {
            if (ragdollState == RagdollState.WaitStablePosition &&
                hipsTransformRigid.velocity.magnitude < 0.1f)
            {
                GetUp();
            }

            if (ragdollState == RagdollState.Animated &&
                controller.GetVelocity().y < -10f)
            {
                RagdollIn();
                RagdollOut();
            }
        }

        /// <summary>
        /// LateUpdate is called after all Update functions have been called. 
        /// </summary>
        private void LateUpdate()
        {
            if (ragdollState != RagdollState.BlendToAnim)
                return;

            float ragdollBlendAmount = 1f - Mathf.InverseLerp(
                ragdollingEndTime,
                ragdollingEndTime + RAGDOLL_TO_MECANIM_BLEND_TIME,
                Time.time);

            if (storedHipsPositionPrivBlend != hipsTransform.position)
            {
                storedHipsPositionPrivAnim = hipsTransform.position;
            }
            storedHipsPositionPrivBlend = Vector3.Lerp(storedHipsPositionPrivAnim, storedHipsPosition, ragdollBlendAmount);
            hipsTransform.position = storedHipsPositionPrivBlend;

            for (int i = 0; i < transformComponents.Count; i++)
            {
                TransformComponent tc = transformComponents[i];
                if (tc.GetRotation() != tc.GetTransform().localRotation)
                {
                    tc.SetRotation(Quaternion.Slerp(tc.GetTransform().localRotation, tc.GetStoredRotation(), ragdollBlendAmount));
                    tc.GetTransform().localRotation = tc.GetRotation();
                }

                if (tc.GetPosition() != tc.GetTransform().localPosition)
                {
                    tc.SetPosition(Vector3.Slerp(tc.GetTransform().localPosition, tc.GetStoredPosition(), ragdollBlendAmount));
                    tc.GetTransform().localPosition = tc.GetPosition();
                }
            }

            if (Mathf.Abs(ragdollBlendAmount) < Mathf.Epsilon)
            {
                ragdollState = RagdollState.Animated;
            }
        }

        /// <summary>
        /// Prevents jittering (as a result of applying joint limits) of bone and smoothly translate rigid from animated mode to ragdoll
        /// </summary>
        /// <param name="rigid"></param>
        /// <returns></returns>
        private static IEnumerator FixTransformAndEnableJoint(RigidbodyComponent joint)
        {
            if (joint.GetJoint() == null || !joint.GetJoint().autoConfigureConnectedAnchor)
                yield break;

            SoftJointLimit highTwistLimit = new SoftJointLimit();
            SoftJointLimit lowTwistLimit = new SoftJointLimit();
            SoftJointLimit swing1Limit = new SoftJointLimit();
            SoftJointLimit swing2Limit = new SoftJointLimit();

            SoftJointLimit curHighTwistLimit = highTwistLimit = joint.GetJoint().highTwistLimit;
            SoftJointLimit curLowTwistLimit = lowTwistLimit = joint.GetJoint().lowTwistLimit;
            SoftJointLimit curSwing1Limit = swing1Limit = joint.GetJoint().swing1Limit;
            SoftJointLimit curSwing2Limit = swing2Limit = joint.GetJoint().swing2Limit;

            float aTime = 0.3f;
            Vector3 startConPosition = joint.GetJoint().connectedBody.transform.InverseTransformVector(joint.GetJoint().transform.position - joint.GetJoint().connectedBody.transform.position);

            joint.GetJoint().autoConfigureConnectedAnchor = false;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Vector3 newConPosition = Vector3.Lerp(startConPosition, joint.GetConnectedAnchorDefault(), t);
                joint.GetJoint().connectedAnchor = newConPosition;

                curHighTwistLimit.limit = Mathf.Lerp(177, highTwistLimit.limit, t);
                curLowTwistLimit.limit = Mathf.Lerp(-177, lowTwistLimit.limit, t);
                curSwing1Limit.limit = Mathf.Lerp(177, swing1Limit.limit, t);
                curSwing2Limit.limit = Mathf.Lerp(177, swing2Limit.limit, t);

                joint.GetJoint().highTwistLimit = curHighTwistLimit;
                joint.GetJoint().lowTwistLimit = curLowTwistLimit;
                joint.GetJoint().swing1Limit = curSwing1Limit;
                joint.GetJoint().swing2Limit = curSwing2Limit;

                yield return null;
            }
            joint.GetJoint().connectedAnchor = joint.GetConnectedAnchorDefault();
            yield return new WaitForFixedUpdate();
            joint.GetJoint().autoConfigureConnectedAnchor = true;

            joint.GetJoint().highTwistLimit = highTwistLimit;
            joint.GetJoint().lowTwistLimit = lowTwistLimit;
            joint.GetJoint().swing1Limit = swing1Limit;
            joint.GetJoint().swing2Limit = swing2Limit;
        }

        /// <summary>
        /// Ragdoll character
        /// </summary>
        private void RagdollIn()
        {
            ActivateRagdollParts(true);
            animator.enabled = false;
            ragdollState = RagdollState.Ragdolled;
            ApplyVelocity(controller.GetVelocity());
        }

        /// <summary>
        /// Smoothly translate to animator's bone positions when character stops falling
        /// </summary>
        private void RagdollOut()
        {
            if (ragdollState == RagdollState.Ragdolled)
                ragdollState = RagdollState.WaitStablePosition;
        }

        private void GetUp()
        {
            ragdollingEndTime = Time.time;
            animator.enabled = true;
            ragdollState = RagdollState.BlendToAnim;
            storedHipsPositionPrivAnim = Vector3.zero;
            storedHipsPositionPrivBlend = Vector3.zero;

            storedHipsPosition = hipsTransform.position;

            Vector3 shiftPos = hipsTransform.position - transform.position;
            shiftPos.y = GetDistanceToFloor(shiftPos.y);

            MoveNodeWithoutChildren(shiftPos);

            for (int i = 0; i < transformComponents.Count; i++)
            {
                TransformComponent tc = transformComponents[i];
                tc.SetStoredRotation(tc.GetTransform().localRotation);
                tc.SetRotation(tc.GetTransform().localRotation);

                tc.SetStoredPosition(tc.GetTransform().localPosition);
                tc.SetPosition(tc.GetTransform().localPosition);
            }

            string getUpAnim = CheckIfLieOnBack() ? animationGetUpFromBack : animationGetUpFromBelly;
            animator.Play(getUpAnim, 0, 0);
            ActivateRagdollParts(false);
        }

        private float GetDistanceToFloor(float currentY)
        {
            RaycastHit[] hits = Physics.RaycastAll(new Ray(hipsTransform.position, Vector3.down));
            float distFromFloor = float.MinValue;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (!hit.transform.IsChildOf(transform))
                {
                    distFromFloor = Mathf.Max(distFromFloor, hit.point.y);
                }
            }

            if (Mathf.Abs(distFromFloor - float.MinValue) > Mathf.Epsilon)
                currentY = distFromFloor - transform.position.y;

            return currentY;
        }

        private void MoveNodeWithoutChildren(Vector3 shiftPos)
        {
            Vector3 ragdollDirection = GetRagdollDirection();

            hipsTransform.position -= shiftPos;
            transform.position += shiftPos;

            Vector3 forward = transform.forward;
            transform.rotation = Quaternion.FromToRotation(forward, ragdollDirection) * transform.rotation;
            hipsTransform.rotation = Quaternion.FromToRotation(ragdollDirection, forward) * hipsTransform.rotation;
        }

        private bool CheckIfLieOnBack()
        {
            Vector3 left = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
            Vector3 right = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
            Vector3 hipsPos = hipsTransform.position;

            left -= hipsPos;
            left.y = 0f;
            right -= hipsPos;
            right.y = 0f;

            Quaternion q = Quaternion.FromToRotation(left, Vector3.right);
            Vector3 t = q * right;

            return t.z < 0f;
        }

        private Vector3 GetRagdollDirection()
        {
            Vector3 ragdolledFeetPosition = (
                animator.GetBoneTransform(HumanBodyBones.Hips).position);
            Vector3 ragdolledHeadPosition = animator.GetBoneTransform(HumanBodyBones.Head).position;
            Vector3 ragdollDirection = ragdolledFeetPosition - ragdolledHeadPosition;
            ragdollDirection.y = 0;
            ragdollDirection = ragdollDirection.normalized;

            if (CheckIfLieOnBack())
                return ragdollDirection;
            else
                return -ragdollDirection;
        }

        /// <summary>
        /// Apply velocity 'predieVelocity' to to each rigid of character
        /// </summary>
        private void ApplyVelocity(Vector3 predieVelocity)
        {
            for (int i = 0, length = rigidbodyComponents.Count; i < length; i++)
            {
                rigidbodyComponents[i].GetRigidBody().velocity = predieVelocity;
            }
        }

        private void ActivateRagdollParts(bool activate)
        {
            controller.ControllerEnabled(!activate);

            for (int i = 0, length = rigidbodyComponents.Count; i < length; i++)
            {
                RigidbodyComponent rigidbody = rigidbodyComponents[i];
                Collider partColider = rigidbody.GetRigidBody().GetComponent<Collider>();

                if (partColider == null)
                {
                    const string colliderNodeSufix = "_ColliderRotator";
                    string childName = rigidbody.GetRigidBody().name + colliderNodeSufix;
                    Transform transform = rigidbody.GetRigidBody().transform.Find(childName);
                    partColider = transform.GetComponent<Collider>();
                }

                partColider.isTrigger = !activate;

                if (activate)
                {
                    rigidbody.GetRigidBody().isKinematic = false;
                    StartCoroutine(FixTransformAndEnableJoint(rigidbody));
                }
                else
                {
                    rigidbody.GetRigidBody().isKinematic = true;
                }
            }
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float distance)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, distance);

            for (int i = 0; i < hits.Length; ++i)
            {
                RaycastHit h = hits[i];
                if (h.transform != transform && h.transform.root == transform.root)
                {
                    hit = h;
                    return true;
                }
            }
            hit = new RaycastHit();
            return false;
        }

        public bool IsRagdolled()
        {
            return ragdollState == RagdollState.Ragdolled ||
                ragdollState == RagdollState.WaitStablePosition;
        }

        public void IsRagdolled(bool value)
        {
            if (value)
                RagdollIn();
            else
                RagdollOut();
        }

        public void AddExtraMove(Vector3 move)
        {
            if (IsRagdolled())
            {
                Vector3 airMove = new Vector3(move.x * AirSpeed, 0f, move.z * AirSpeed);
                for (int i = 0, length = rigidbodyComponents.Count; i < length; i++)
                {
                    RigidbodyComponent rigidbodyComponent = rigidbodyComponents[i];
                    rigidbodyComponent.GetRigidBody().AddForce(airMove / 100f, ForceMode.VelocityChange);
                }
            }
        }

        public string GetAnimationGetUpFromBelly()
        {
            return animationGetUpFromBelly;
        }

        public void SetAnimationGetUpFromBelly(string value)
        {
            animationGetUpFromBelly = value;
        }

        public string GetAnimationGetUpFromBack()
        {
            return animationGetUpFromBack;
        }

        public void SetAnimationGetUpFromBack(string value)
        {
            animationGetUpFromBack = value;
        }

        public float GetAirSpeed()
        {
            return AirSpeed;
        }

        public void SetAirSpeed(float value)
        {
            AirSpeed = value;
        }

        public Transform GetHipsTransform()
        {
            return hipsTransform;
        }

        public void SetHipsTransform(Transform value)
        {
            hipsTransform = value;
        }

        public Rigidbody GetHipsTransformRigid()
        {
            return hipsTransformRigid;
        }

        public void SetHipsTransformRigid(Rigidbody value)
        {
            hipsTransformRigid = value;
        }

        public float GetRagdollingEndTime()
        {
            return ragdollingEndTime;
        }

        public Vector3 GetStoredHipsPosition()
        {
            return storedHipsPosition;
        }

        public void SetStoredHipsPosition(Vector3 value)
        {
            storedHipsPosition = value;
        }

        public Vector3 GetStoredHipsPositionPrivAnim()
        {
            return storedHipsPositionPrivAnim;
        }

        public void SetStoredHipsPositionPrivAnim(Vector3 value)
        {
            storedHipsPositionPrivAnim = value;
        }

        public Vector3 GetStoredHipsPositionPrivBlend()
        {
            return storedHipsPositionPrivBlend;
        }

        public void SetStoredHipsPositionPrivBlend(Vector3 value)
        {
            storedHipsPositionPrivBlend = value;
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        public IController GetControllerCallbacks()
        {
            return controller;
        }
    }
}