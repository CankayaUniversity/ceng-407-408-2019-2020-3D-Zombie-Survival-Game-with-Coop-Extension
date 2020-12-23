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
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class WeaponAnimationSystem : MonoBehaviour, IWeaponAnimator
    {
        public const string SPEED = "Speed";
        public const string FIRE = "Fire";
        public const string SIGHT = "Sight";
        public const string RELOAD = "Reload";
        public const string TAKE_IN = "TakeIn";
        public const string TAKE_OUT = "TakeOut";

        public readonly static int SPEED_HASH = Animator.StringToHash(SPEED);
        public readonly static int FIRE_HASH = Animator.StringToHash(FIRE);
        public readonly static int SIGHT_HASH = Animator.StringToHash(SIGHT);
        public readonly static int RELOAD_HASH = Animator.StringToHash(RELOAD);
        public readonly static int TAKE_IN_HASH = Animator.StringToHash(TAKE_IN);
        public readonly static int TAKE_OUT_HASH = Animator.StringToHash(TAKE_OUT);

        [SerializeField] private Transform player;
        [SerializeField] private float takeTime;
        [SerializeField] private float putAwayTime;
        [SerializeField] private float positionSensitivity = 0.02f;
        [SerializeField] private float maxPositionSensitivity = 0.03f;
        [SerializeField] private float smoothPosition = 3;
        [SerializeField] private float smoothRotation = 2;
        [SerializeField] private float rotationSensitivity = 2;
        [SerializeField] private float originalYAxis;
        [SerializeField] private float maxYPosJump;
        [SerializeField] private float smoothJump;
        [SerializeField] private float smoothLand;
        [SerializeField] private bool useRotationSway;
        [SerializeField] private bool useJumpSway;

        [SerializeField] AnimationEventProperties animationEventProperties;

        private Animator animator;
        private AudioSource audioSource;
        private IPlayerController controller;
        private CharacterController characterController;
        private Vector3 originalPostion;
        private IEnumerator animationEventsProcessing;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            controller = player.GetComponent<IPlayerController>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            originalPostion = transform.localPosition;

            if (controller == null)
            {
                Debug.LogError(string.Format("Unreal FPS Runtime Error: Weapon[{0}], Component[{1}]\nField [1. Controller] is empty!", gameObject.name, this.GetType().FullName));
                return;
            }
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            characterController = controller.GetCharacterController();
        }

        protected virtual void OnEnable()
        {
            if (animationEventProperties == null || animationEventsProcessing != null)
            {
                return;
            }
            animationEventsProcessing = AnimationEventsProcessing();
            StartCoroutine(animationEventsProcessing);
        }

        protected virtual void OnDisable()
        {
            if (animationEventsProcessing == null)
            {
                return;
            }
            StopCoroutine(animationEventsProcessing);
            animationEventsProcessing = null;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (controller == null)
                return;

            if (controller.IsSprinting())
            {
                animator.SetInteger(SPEED_HASH, 3);
            }
            else if (controller.IsRunning())
            {
                animator.SetInteger(SPEED_HASH, 2);
            }
            else if (controller.IsWalking())
            {
                animator.SetInteger(SPEED_HASH, 1);
            }
            else if (!UInput.GetButton(INC.ATTACK) && !UInput.GetButton(INC.JUMP))
            {
                animator.SetInteger(SPEED_HASH, 0);
            }

            if (UInput.GetButtonDown(INC.SIGHT))
            {
                animator.SetBool(SIGHT_HASH, true);
            }
            else if (UInput.GetButtonUp(INC.SIGHT))
            {
                animator.SetBool(SIGHT_HASH, false);
            }

            if (useRotationSway)
            {
                RotationSwayProcessing(transform);
            }

            if (useJumpSway)
            {
                JumpSwayProcessing(transform);
            }
        }

        protected virtual IEnumerator AnimationEventsProcessing()
        {
            string clipName = "None";
            string lastClipName = "None";
            float startTime = 0;
            float clipLength = 0;
            bool loopReady = false;
            int xCount = 0;
            AnimationEventProperty.EventProperty[] eventProperty = null;
            ShakeCamera shakeCamera = ShakeCamera.Instance;
            while (true)
            {
                if (animator.GetCurrentAnimatorClipInfoCount(0) > 0)
                {
                    AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
                    clipName = clipInfo.clip.name;
                    clipLength = clipInfo.clip.length;
                }
                if (clipName != lastClipName)
                {
                    for (int i = 0, length = animationEventProperties.GetLength(); i < length; i++)
                    {
                        AnimationEventProperty property = animationEventProperties.GetProperty(i);
                        if (property.GetAnimationName() == clipName)
                        {
                            eventProperty = property.GetEventProperties();
                            lastClipName = clipName;
                            startTime = Time.time;
                            loopReady = false;
                            xCount = 0;
                            break;
                        }
                        lastClipName = clipName;
                        eventProperty = null;
                        startTime = 0;
                        loopReady = true;
                        xCount = 0;
                    }
                }

                if (eventProperty != null)
                {
                    if (Time.time - startTime >= clipLength)
                    {
                        startTime = Time.time;
                        loopReady = false;
                    }
                    else if (!loopReady)
                    {
                        for (int i = 0, length = eventProperty.Length; i < length; i++)
                        {
                            AnimationEventProperty.EventProperty property = eventProperty[i];
                            if (Time.time - startTime >= property.GetAnimationTime())
                            {
                                if (xCount == i)
                                {
                                    if (property.GetSoundEffect() != null)
                                        audioSource.PlayOneShot(property.GetSoundEffect());
                                    if (property.GetShakeProperties().GetTarget() != ShakeCamera.ShakeEvent.Target.None)
                                        shakeCamera.AddShakeEvent(property.GetShakeProperties());

                                    if (i == length - 1)
                                    {
                                        loopReady = true;
                                        xCount = 0;
                                        break;
                                    }
                                    xCount++;
                                }
                            }
                        }
                    }
                }
                yield return null;
            }
        }

        public void SetAttack(int state)
        {
            animator.SetInteger(FIRE_HASH, state);
        }

        public void SetSpeed(int state)
        {
            animator.SetInteger(SPEED_HASH, state);
        }

        public void SetReload(int state)
        {
            animator.SetInteger(RELOAD_HASH, state);
        }

        public void SetSight(bool state)
        {
            animator.SetBool(SIGHT_HASH, state);
        }

        public void PutAway()
        {
            animator.SetTrigger(TAKE_OUT_HASH);
        }

        public WeaponActionState GetActiveState()
        {
            if (UInput.GetButton(INC.SIGHT))
            {
                return WeaponActionState.Sight;
            }
            else if (!characterController.isGrounded)
            {
                return WeaponActionState.InAir;
            }
            else if (controller.GetCrouchSystem().IsCrouch())
            {
                return WeaponActionState.Crouch;
            }
            else if (animator.GetInteger(SPEED_HASH) == 3)
            {
                return WeaponActionState.Sprint;
            }
            else if (animator.GetInteger(SPEED_HASH) == 2)
            {
                return WeaponActionState.Run;
            }
            else if (animator.GetInteger(SPEED_HASH) == 1)
            {
                return WeaponActionState.Walk;
            }
            else
            {
                return WeaponActionState.Idle;
            }
        }

        /// <summary>
        /// Weapon sway then camera rotation.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="speed"></param>
        protected virtual void RotationSwayProcessing(Transform weapon)
        {
            float factorX = -UInput.GetAxis(INC.CAM_VERTICAL) * positionSensitivity;
            float factorY = -UInput.GetAxis(INC.CAM_HORIZONTAL) * positionSensitivity;

            if (factorX > maxPositionSensitivity)
                factorX = maxPositionSensitivity;

            if (factorX < -maxPositionSensitivity)
                factorX = -maxPositionSensitivity;

            if (factorY > maxPositionSensitivity)
                factorY = maxPositionSensitivity;

            if (factorY < -maxPositionSensitivity)
                factorY = -maxPositionSensitivity;

            Vector3 final = new Vector3(originalPostion.x + factorX, originalPostion.y + factorY, originalPostion.z);
            weapon.localPosition = Vector3.Lerp(weapon.localPosition, final, Time.deltaTime * smoothPosition);

            float tiltAroundZ = UInput.GetAxis(INC.CAM_VERTICAL) * rotationSensitivity;
            float tiltAroundX = UInput.GetAxis(INC.CAM_HORIZONTAL) * rotationSensitivity;
            Quaternion target = Quaternion.Euler(weapon.localRotation.x + tiltAroundX, originalYAxis, weapon.localRotation.z + tiltAroundZ);
            weapon.localRotation = Quaternion.Slerp(weapon.localRotation, target, Time.deltaTime * smoothRotation);
        }

        /// <summary>
        /// Sway weapon then player jumped.
        /// </summary>
        /// <param name="weapon"></param>
        protected virtual void JumpSwayProcessing(Transform weapon)
        {
            if (!characterController.isGrounded)
            {
                float newY = Mathf.Lerp(weapon.localPosition.y, weapon.localPosition.y + maxYPosJump, smoothJump * Time.deltaTime);
                weapon.localPosition = new Vector3(weapon.localPosition.x, newY, weapon.localPosition.z);
            }
            else
            {
                float newY = Mathf.Lerp(weapon.localPosition.y, originalPostion.y, smoothLand * Time.deltaTime);
                weapon.localPosition = new Vector3(weapon.localPosition.x, newY, weapon.localPosition.z);
            }
        }

        public float GetTakeTime()
        {
            return takeTime;
        }

        public void SetTakeTime(float value)
        {
            takeTime = value;
        }

        public float GetPutAwayTime()
        {
            return putAwayTime;
        }

        public void SetPutAwayTime(float value)
        {
            putAwayTime = value;
        }

        public float GetPositionSensitivity()
        {
            return positionSensitivity;
        }

        public void SetPositionSensitivity(float value)
        {
            positionSensitivity = value;
        }

        public float GetMaxPositionSensitivity()
        {
            return maxPositionSensitivity;
        }

        public void SetMaxPositionSensitivity(float value)
        {
            maxPositionSensitivity = value;
        }

        public float GetSmoothPosition()
        {
            return smoothPosition;
        }

        public void SetSmoothPosition(float value)
        {
            smoothPosition = value;
        }

        public float GetSmoothRotation()
        {
            return smoothRotation;
        }

        public void SetSmoothRotation(float value)
        {
            smoothRotation = value;
        }

        public float GetRotationSensitivity()
        {
            return rotationSensitivity;
        }

        public void SetRotationSensitivity(float value)
        {
            rotationSensitivity = value;
        }

        public float GetOriginalYAxis()
        {
            return originalYAxis;
        }

        public void SetOriginalYAxis(float value)
        {
            originalYAxis = value;
        }

        public float GetMaxYPosJump()
        {
            return maxYPosJump;
        }

        public void SetMaxYPosJump(float value)
        {
            maxYPosJump = value;
        }

        public float GetSmoothJump()
        {
            return smoothJump;
        }

        public void SetSmoothJump(float value)
        {
            smoothJump = value;
        }

        public float GetSmoothLand()
        {
            return smoothLand;
        }

        public void SetSmoothLand(float value)
        {
            smoothLand = value;
        }

        public bool UseRotationSway()
        {
            return useRotationSway;
        }

        public void RotationSwayActive(bool value)
        {
            useRotationSway = value;
        }

        public bool UseJumpSway()
        {
            return useJumpSway;
        }

        public void JumpSwayActive(bool value)
        {
            useJumpSway = value;
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        public CharacterController GetCharacterController()
        {
            return characterController;
        }

        public Vector3 GetOriginalPostion()
        {
            return originalPostion;
        }

        public IPlayerController GetControllerCallbacks()
        {
            return controller;
        }

        public Transform GetPlayer()
        {
            return player;
        }

        public void SetPlayer(Transform value)
        {
            player = value;
        }

        public AnimationEventProperties GetAnimationEventProperties()
        {
            return animationEventProperties;
        }

        public void SetAnimationEventProperties(AnimationEventProperties value)
        {
            animationEventProperties = value;
        }
    }
}