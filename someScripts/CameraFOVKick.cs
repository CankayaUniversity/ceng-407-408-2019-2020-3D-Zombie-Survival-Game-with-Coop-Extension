/* ==================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System;
using System.Collections;
using UnityEngine;

namespace UnrealFPS.Runtime
{
    [Serializable]
    public class CameraFOVKick
    {
        [SerializeField][Range(0.0f, 179.0f)] private float originalFov = 60;
        [SerializeField][Range(0.0f, 50.0f)] private float fovIncrease = 5.0f;
        [SerializeField][Range(0.0f, 50.0f)] private float increaseSpeed = 10.0f;
        [SerializeField][Range(0.0f, 50.0f)] private float decreaseSpeed = 10.0f;
        
        private Camera _FPCamera;

        public virtual void Initialize(Camera _FPCamera)
        {
            if (_FPCamera == null)
                throw new Exception(string.Format("Instance - [{0}], Field - [{1} is empty]\nMessage: Please fill {1} paramerter!",
                    this, "FP Camera"));
            this._FPCamera = _FPCamera;
        }

        public virtual IEnumerator Increase()
        {
            float targetFOV = originalFov + fovIncrease;
            while (true)
            {
                _FPCamera.fieldOfView = Mathf.SmoothStep(_FPCamera.fieldOfView, targetFOV, increaseSpeed * Time.deltaTime);
                if (Utility.UMathf.Approximately(_FPCamera.fieldOfView, targetFOV, 0.5f))
                    yield break;
                yield return null;
            }
        }

        public virtual IEnumerator Decrease()
        {
            while (true)
            {
                _FPCamera.fieldOfView = Mathf.SmoothStep(_FPCamera.fieldOfView, originalFov, decreaseSpeed * Time.deltaTime);
                if (Utility.UMathf.Approximately(_FPCamera.fieldOfView, originalFov, 0.5f))
                    yield break;
                yield return null;
            }
        }

        public virtual IEnumerator CustomIncrease(float fovIncrease, AnimationCurve increaseCurve, float timeToIncrease)
        {
            float t = Mathf.Abs((_FPCamera.fieldOfView - originalFov) / fovIncrease);
            while (t < timeToIncrease)
            {
                _FPCamera.fieldOfView = originalFov + (increaseCurve.Evaluate(t / timeToIncrease) * fovIncrease);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        #region [Getter / Setter]
        public Camera GetCamera()
        {
            return _FPCamera;
        }

        public void SetCamera(Camera value)
        {
            _FPCamera = value;
        }

        public float GetOriginalFov()
        {
            return originalFov;
        }

        public void SetOriginalFov(float value)
        {
            originalFov = value;
        }

        public float GetFovIncrease()
        {
            return fovIncrease;
        }

        public void SetFovIncrease(float value)
        {
            fovIncrease = value;
        }

        public float GetIncreaseSpeed()
        {
            return increaseSpeed;
        }

        public void SetIncreaseSpeed(float value)
        {
            increaseSpeed = value;
        }

        public float GetDecreaseSpeed()
        {
            return decreaseSpeed;
        }

        public void SetDecreaseSpeed(float value)
        {
            decreaseSpeed = value;
        }
        #endregion
    }
}