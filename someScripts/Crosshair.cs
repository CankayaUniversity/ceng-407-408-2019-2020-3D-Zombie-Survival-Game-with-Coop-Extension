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
using UnrealFPS.Runtime;
using UnrealFPS.Utility;

namespace UnrealFPS.UI
{
    public class Crosshair : MonoBehaviour
    {
        public enum Mode
        {
            Static,
            Dynamic
        }

        public enum Preset
        {
            Default,
            Shotgun,
            Triangle
        }

        public enum RotationSide
        {
            Left,
            Right
        }

        [System.Serializable]
        public struct AnimationState
        {
            public WeaponActionState state;
            public float spread;
            public float smooth;
        }

        [SerializeField] private Mode mode;
        [SerializeField] private AnimationState[] animationStates;
        [SerializeField] private Preset preset = Preset.Default;
        [SerializeField] private Texture2D verticalTexture;
        [SerializeField] private Texture2D horizontalTexture;
        [SerializeField] private Color crosshairColor = Color.white;
        [SerializeField] private float height = 10.0f;
        [SerializeField] private float width = 3.0f;
        [SerializeField] private float spread = 20;
        [SerializeField] private float angle = 0.0f;
        [SerializeField] private bool showCrosshair = true;
        [SerializeField] private bool hideWhileSight = true;
        [SerializeField] private RotationSide rotationSide = RotationSide.Right;
        [SerializeField] private float rotationSpeed = 25.0f;
        [SerializeField] private bool rotationAnimation = false;

        [SerializeField] private Texture2D hitEffectTexture;
        [SerializeField] private Color hitEffectColor = Color.white;
        [SerializeField] private float hitEffectHeight = 10.0f;
        [SerializeField] private float hitEffectWidth = 3.0f;
        [SerializeField] private float hitEffectSpread = 15.0f;
        [SerializeField] private float hitEffectHideSpeed = 7.5f;
        [SerializeField] private float hitEffecntAngle = 45.0f;
        [SerializeField] private bool hitEffectIsActive = true;

        private IEnumerator dynamicModeCoroutine;
        private IEnumerator rotationAnimationCoroutine;
        private Vector2 center;

        protected virtual void Awake()
        {
            center = new Vector2(Screen.width / 2, Screen.height / 2);
        }

        protected virtual void OnEnable()
        {
            switch (mode)
            {
                case Mode.Static:
                    break;
                case Mode.Dynamic:
                    dynamicModeCoroutine = Dynamic();
                    StartCoroutine(dynamicModeCoroutine);
                    break;
            }
            if (rotationAnimation)
            {
                rotationAnimationCoroutine = RotateAnimation();
                StartCoroutine(rotationAnimationCoroutine);
            }
        }

        protected virtual void OnGUI()
        {
            if (hideWhileSight)
                showCrosshair = !UInput.GetButton(INC.SIGHT);

            if (showCrosshair && verticalTexture && horizontalTexture)
            {
                GUI.color = crosshairColor;
                switch (preset)
                {
                    case Preset.Default:
                        GUIUtility.RotateAroundPivot(angle, center);
                        GUI.DrawTexture(new Rect((Screen.width - width) / 2, (Screen.height - spread) / 2 - height, width, height), horizontalTexture);
                        GUI.DrawTexture(new Rect((Screen.width - width) / 2, (Screen.height + spread) / 2, width, height), horizontalTexture);
                        GUI.DrawTexture(new Rect((Screen.width - spread) / 2 - height, (Screen.height - width) / 2, height, width), verticalTexture);
                        GUI.DrawTexture(new Rect((Screen.width + spread) / 2, (Screen.height - width) / 2, height, width), verticalTexture);
                        break;
                    case Preset.Shotgun:
                        GUIUtility.RotateAroundPivot(angle, center);
                        GUI.DrawTexture(new Rect((Screen.width - height) / 2, (Screen.height - spread) / 2 - width, height, width), horizontalTexture);
                        GUI.DrawTexture(new Rect((Screen.width - height) / 2, (Screen.height + spread) / 2, height, width), horizontalTexture);
                        GUI.DrawTexture(new Rect((Screen.width - spread) / 2 - width, (Screen.height - height) / 2, width, height), verticalTexture);
                        GUI.DrawTexture(new Rect((Screen.width + spread) / 2, (Screen.height - height) / 2, width, height), verticalTexture);
                        break;
                    case Preset.Triangle:
                        GUIUtility.RotateAroundPivot(angle, center);
                        GUI.DrawTexture(new Rect((Screen.width - 2) / 2, (Screen.height - spread) / 2 - 14, width, height), horizontalTexture);
                        GUIUtility.RotateAroundPivot(45, center);
                        GUI.DrawTexture(new Rect((Screen.width + spread) / 2, (Screen.height - 2) / 2, height, width), horizontalTexture);
                        GUIUtility.RotateAroundPivot(0, center);
                        GUI.DrawTexture(new Rect((Screen.width - 2) / 2, (Screen.height + spread) / 2, width, height), horizontalTexture);
                        break;
                }
                GUI.color = Color.white;
            }

            if (hitEffectIsActive && hitEffectTexture)
            {
                if (!UMathf.Approximately(hitEffectColor.a, 0, 0.001f))
                    hitEffectColor.a = Mathf.SmoothStep(hitEffectColor.a, 0, hitEffectHideSpeed * Time.deltaTime);
                GUI.color = hitEffectColor;
                GUIUtility.RotateAroundPivot(hitEffecntAngle, center);
                GUI.DrawTexture(new Rect((Screen.width - hitEffectWidth) / 2, (Screen.height - hitEffectSpread) / 2 - hitEffectHeight, hitEffectWidth, hitEffectHeight), hitEffectTexture);
                GUI.DrawTexture(new Rect((Screen.width - hitEffectWidth) / 2, (Screen.height + hitEffectSpread) / 2, hitEffectWidth, hitEffectHeight), hitEffectTexture);
                GUI.DrawTexture(new Rect((Screen.width - hitEffectSpread) / 2 - hitEffectHeight, (Screen.height - hitEffectWidth) / 2, hitEffectHeight, hitEffectWidth), hitEffectTexture);
                GUI.DrawTexture(new Rect((Screen.width + hitEffectSpread) / 2, (Screen.height - hitEffectWidth) / 2, hitEffectHeight, hitEffectWidth), hitEffectTexture);
            }
        }

        public virtual IEnumerator Dynamic()
        {
            yield return new WaitForEndOfFrame();
            IWeaponAnimator weaponAnimator = GetComponent<IWeaponAnimator>();
            IEnumerator setSpreadIEnumerator = null;
            WeaponActionState lastState = WeaponActionState.Idle;
            while (true)
            {
                if (lastState != weaponAnimator.GetActiveState())
                {
                    for (int i = 0; i < animationStates.Length; i++)
                    {
                        AnimationState animationState = animationStates[i];
                        if (animationState.state == weaponAnimator.GetActiveState())
                        {
                            if (setSpreadIEnumerator != null)
                                StopCoroutine(setSpreadIEnumerator);
                            setSpreadIEnumerator = SetSpread(animationState.spread, animationState.smooth);
                            StartCoroutine(setSpreadIEnumerator);
                            lastState = weaponAnimator.GetActiveState();
                            break;
                        }
                    }
                }
                yield return null;
            }
        }

        public virtual IEnumerator RotateAnimation()
        {
            while (true)
            {
                angle += (rotationSpeed * (rotationSide == RotationSide.Right ? 1 : -1)) * Time.deltaTime;
                angle = UMathf.Loop(angle, -360, 360);
                yield return null;
            }
        }

        public virtual IEnumerator SetSpread(float spread, float smooth)
        {
            while (true)
            {
                this.spread = Mathf.SmoothStep(this.spread, spread, smooth);
                if (UMathf.Approximately(this.spread, spread, 0.01f))
                    yield break;
                yield return null;
            }
        }

        /// <summary>
        /// Start crosshair dynamic mode.
        /// </summary>
        public void StartDynamicMode()
        {
            if (dynamicModeCoroutine != null)
                return;

            mode = Mode.Dynamic;
            dynamicModeCoroutine = Dynamic();
            StartCoroutine(dynamicModeCoroutine);
        }

        /// <summary>
        /// Stop crosshair dynamic mode.
        /// </summary>
        public void StopDynamicMode()
        {
            if (dynamicModeCoroutine == null)
                return;

            mode = Mode.Static;
            StopCoroutine(dynamicModeCoroutine);
            dynamicModeCoroutine = null;
        }

        /// <summary>
        /// Start rotation animation.
        /// </summary>
        public void StartRotationAnimation()
        {
            if (rotationAnimationCoroutine != null)
                return;

            rotationAnimation = true;
            rotationAnimationCoroutine = RotateAnimation();
            StartCoroutine(rotationAnimationCoroutine);
        }

        /// <summary>
        /// Stop rotation animation.
        /// </summary>
        public void StopRotationAnimation(float resetAngle = 0)
        {
            if (rotationAnimationCoroutine == null)
                return;

            rotationAnimation = false;
            angle = resetAngle;
            StopCoroutine(rotationAnimationCoroutine);
            rotationAnimationCoroutine = null;
        }

        public void PlayHitEffect()
        {
            hitEffectColor.a = 1;
        }

        public Mode GetMode()
        {
            return mode;
        }

        public void SetMode(Mode value)
        {
            mode = value;
        }

        public AnimationState[] GetAnimationStates()
        {
            return animationStates;
        }

        public void SetAnimationStates(AnimationState[] value)
        {
            animationStates = value;
        }

        public Preset GetPreset()
        {
            return preset;
        }

        public void SetPreset(Preset value)
        {
            preset = value;
        }

        public Texture2D GetVerticalTexture()
        {
            return verticalTexture;
        }

        public void SetVerticalTexture(Texture2D value)
        {
            verticalTexture = value;
        }

        public Texture2D GetHorizontalTexture()
        {
            return horizontalTexture;
        }

        public void SetHorizontalTexture(Texture2D value)
        {
            horizontalTexture = value;
        }

        public Color GetCrosshairColor()
        {
            return crosshairColor;
        }

        public void SetCrosshairColor(Color value)
        {
            crosshairColor = value;
        }

        public float GetHeight()
        {
            return height;
        }

        public void SetHeight(float value)
        {
            height = value;
        }

        public float GetWidth()
        {
            return width;
        }

        public void SetWidth(float value)
        {
            width = value;
        }

        public float GetAngle()
        {
            return angle;
        }

        public void SetAngle(float value)
        {
            angle = value;
        }

        public bool ShowCrosshair()
        {
            return showCrosshair;
        }

        public void ShowCrosshair(bool value)
        {
            showCrosshair = value;
        }

        public bool HideWhileSight()
        {
            return hideWhileSight;
        }

        public void HideWhileSight(bool value)
        {
            hideWhileSight = value;
        }

        public RotationSide GetRotationSide()
        {
            return rotationSide;
        }

        public void SetRotationSide(RotationSide value)
        {
            rotationSide = value;
        }

        public float GetRotationSpeed()
        {
            return rotationSpeed;
        }

        public void SetRotationSpeed(float value)
        {
            rotationSpeed = value;
        }

        public bool GetRotationAnimation()
        {
            return rotationAnimation;
        }

        public void SetRotationAnimation(bool value)
        {
            rotationAnimation = value;
        }

        public float GetSpread()
        {
            return spread;
        }

        public void SetSpread(float value)
        {
            spread = value;
        }

        public Texture2D GetHitEffectTexture()
        {
            return hitEffectTexture;
        }

        public void SetHitEffectTexture(Texture2D value)
        {
            hitEffectTexture = value;
        }

        public Color GetHitEffectColor()
        {
            return hitEffectColor;
        }

        public void SetHitEffectColor(Color value)
        {
            hitEffectColor = value;
        }

        public float GetHitEffectHeight()
        {
            return hitEffectHeight;
        }

        public void SetHitEffectHeight(float value)
        {
            hitEffectHeight = value;
        }

        public float GetHitEffectWidth()
        {
            return hitEffectWidth;
        }

        public void SetHitEffectWidth(float value)
        {
            hitEffectWidth = value;
        }

        public float GetHitEffectSpread()
        {
            return hitEffectSpread;
        }

        public void SetHitEffectSpread(float value)
        {
            hitEffectSpread = value;
        }

        public float GetHitEffectHideSpeed()
        {
            return hitEffectHideSpeed;
        }

        public void SetHitEffectHideSpeed(float value)
        {
            hitEffectHideSpeed = value;
        }

        public float GetHitEffecntAngle()
        {
            return hitEffecntAngle;
        }

        public void SetHitEffecntAngle(float value)
        {
            hitEffecntAngle = value;
        }

        public bool HitEffectIsActive()
        {
            return hitEffectIsActive;
        }

        public void HitEffectIsActive(bool value)
        {
            hitEffectIsActive = value;
        }
    }
}