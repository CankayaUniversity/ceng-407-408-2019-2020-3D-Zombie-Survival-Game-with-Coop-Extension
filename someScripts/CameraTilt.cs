/* ================================================================
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
using UnrealFPS.Utility;

namespace UnrealFPS.Runtime
{
    [Serializable]
    public class CameraTilt
    {

        [SerializeField] private float angle = 45.0f;
        [SerializeField] private float outputRange = 0.5f;
        [SerializeField] private float rotateSpeed = 2.0f;
        [SerializeField] private float outputSpeed = 2.0f;

        private Transform camera;
        private FPCameraLook cameraLook;
        private bool isTilt;
        private float originalAngle;
        private float originalOutput;
        private Vector3 targetCameraPos;
        private Quaternion targetCameraQuatRot;
        private Vector3 targetCameraEulerRot;

        /// <summary>
        /// Initialize CameraTilt instance.
        /// </summary>
        /// <param name="camera"></param>
        public virtual void Initialize(Transform camera, FPCameraLook cameraLook)
        {
            this.camera = camera;
            this.cameraLook = cameraLook;
            originalAngle = camera.localEulerAngles.x;
            originalOutput = camera.localPosition.x;
        }

        public virtual void Update()
        {
            targetCameraPos = camera.localPosition;
            targetCameraQuatRot = cameraLook.GetCameraTargetRot();
            targetCameraEulerRot = targetCameraQuatRot.eulerAngles;
            targetCameraEulerRot.y = 0;
            if (UInput.GetButton(INC.RIGHT_TILT))
            {
                targetCameraPos.x = Mathf.SmoothStep(targetCameraPos.x, outputRange, outputSpeed * Time.deltaTime);
                targetCameraEulerRot.z = -angle;
                targetCameraQuatRot = Quaternion.Euler(targetCameraEulerRot);
                isTilt = true;
            }
            else if (UInput.GetButton(INC.LEFT_TILT))
            {
                targetCameraPos.x = Mathf.SmoothStep(targetCameraPos.x, -outputRange, outputSpeed * Time.deltaTime);
                targetCameraEulerRot.z = angle;
                targetCameraQuatRot = Quaternion.Euler(targetCameraEulerRot);
                isTilt = true;
            }
            else if (!UInput.GetButton(INC.RIGHT_TILT) && !UInput.GetButton(INC.LEFT_TILT))
            {
                targetCameraPos.x = Mathf.SmoothStep(targetCameraPos.x, originalOutput, outputSpeed * Time.deltaTime);
                targetCameraEulerRot.z = -originalAngle;
                targetCameraQuatRot = Quaternion.Euler(targetCameraEulerRot);
                isTilt = false;
            }
            camera.localPosition = targetCameraPos;
            cameraLook.SetCameraTargetRot(Quaternion.Slerp(cameraLook.GetCameraTargetRot(), targetCameraQuatRot, rotateSpeed * Time.deltaTime));
        }

        public float GetAngle()
        {
            return angle;
        }

        public void SetAngle(float value)
        {
            angle = value;
        }

        public float GetOutputRange()
        {
            return outputRange;
        }

        public void SetOutputRange(float value)
        {
            outputRange = value;
        }

        public float GetRotateSpeed()
        {
            return rotateSpeed;
        }

        public void SetRotateSpeed(float value)
        {
            rotateSpeed = value;
        }

        public float GetOutputSpeed()
        {
            return outputSpeed;
        }

        public void SetOutputSpeed(float value)
        {
            outputSpeed = value;
        }

        public Transform GetCamera()
        {
            return camera;
        }

        public bool IsTilt()
        {
            return isTilt;
        }

        protected void IsTilt(bool isTilt)
        {
            this.isTilt = isTilt;
        }
    }
}