/* ==================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System.Collections;
using UnityEngine;

namespace UnrealFPS.Runtime
{
    public partial class FPHealth
    {
        [System.Serializable]
        public class DeathCamera : IDeathCamera
        {
            public struct FreezeRotation
            {
                public bool x;
                public bool y;
                public bool z;

                public FreezeRotation(bool x, bool y, bool z)
                {
                    this.x = x;
                    this.y = y;
                    this.z = z;
                }
            }

            [SerializeField] private Transform camera;
            [SerializeField] private Transform body;
            [SerializeField] private Transform lookAtTransform;
            [SerializeField] private float rotationSpeed;
            [SerializeField] private FreezeRotation freezeRotation;
            [SerializeField] private Transform[] transformsToDisable;
            [SerializeField] private Behaviour[] componentsToDisable;

            private Vector3 originalCameraPosition;
            private Quaternion originalCameraRotation;
            private MonoBehaviour mono;
            private IEnumerator processing;

            /// <summary>
            /// Initialize Death Camera system.
            /// </summary>
            /// <param name="mono">IHealth MonoBehaviour.</param>
            public void Initialize(MonoBehaviour mono)
            {
                this.mono = mono;
                originalCameraPosition = camera.localPosition;
                originalCameraRotation = camera.localRotation;
            }

            /// <summary>
            /// Start processing player death camera.
            /// </summary>
            public virtual void StartProcessing()
            {
                if (processing != null)
                {
                    return;
                }

                SetComponentsToDisableEnabled(false);
                SetTransformsToDisableEnabled(false);
                SetDeathCameraComponentsEnabled(true);
                processing = Processing();
                mono.StartCoroutine(processing);
            }

            /// <summary>
            /// Stop processing player death camera.
            /// </summary>
            public virtual void StopProcessing()
            {
                if (processing == null)
                {
                    return;
                }

                SetComponentsToDisableEnabled(true);
                SetTransformsToDisableEnabled(true);
                SetDeathCameraComponentsEnabled(false);
                mono.StopCoroutine(processing);
                processing = null;
            }

            /// <summary>
            /// Start processing player death camera.
            /// </summary>
            protected virtual IEnumerator Processing()
            {
                WaitForEndOfFrame delay = new WaitForEndOfFrame();
                camera.localPosition = originalCameraPosition;
                camera.localRotation = originalCameraRotation;
                while (true)
                {
                    yield return delay;

                    Vector3 dir = lookAtTransform.localPosition - camera.localPosition;
                    Quaternion rot = Quaternion.LookRotation(dir);
                    if (freezeRotation.x) rot.x = 0;
                    if (freezeRotation.y) rot.y = 0;
                    if (freezeRotation.z) rot.z = 0;
                    camera.localRotation = Quaternion.Slerp(camera.localRotation, rot, rotationSpeed * Time.deltaTime);
                }
            }

            /// <summary>
            /// Set components enabled state.
            /// </summary>
            /// <param name="enabled"></param>
            protected virtual void SetComponentsToDisableEnabled(bool enabled)
            {
                if (componentsToDisable == null)
                {
                    return;
                }

                for (int i = 0; i < componentsToDisable.Length; i++)
                {
                    componentsToDisable[i].enabled = enabled;
                }
            }

            /// <summary>
            /// Set components enabled state.
            /// </summary>
            /// <param name="enabled"></param>
            protected virtual void SetTransformsToDisableEnabled(bool enabled)
            {
                if (transformsToDisable == null)
                {
                    return;
                }

                for (int i = 0; i < transformsToDisable.Length; i++)
                {
                    transformsToDisable[i].gameObject.SetActive(enabled);
                }
            }

            /// <summary>
            /// Set Death Camera require components enabled.
            /// </summary>
            /// <param name="enabled"></param>
            protected virtual void SetDeathCameraComponentsEnabled(bool enabled)
            {
                SetKinematicEnabled(body, !enabled);
                SetCollidersEnabled(body, enabled);
                body.gameObject.SetActive(enabled);
                camera.gameObject.SetActive(enabled);
            }

            /// <summary>
            /// Set enabled all rigidbody components that contains in body.
            /// </summary>
            /// <param name="body"></param>
            /// <param name="enabled"></param>
            protected void SetKinematicEnabled(Transform body, bool enabled)
            {
                Rigidbody[] rigidbodies = body.GetComponentsInChildren<Rigidbody>(true);
                for (int i = 0; i < rigidbodies.Length; i++)
                {
                    rigidbodies[i].isKinematic = enabled;
                }
            }

            /// <summary>
            /// Set enabled all collider components that contains in body.
            /// </summary>
            /// <param name="body"></param>
            /// <param name="enabled"></param>
            protected void SetCollidersEnabled(Transform body, bool enabled)
            {
                Collider[] rigidbodies = body.GetComponentsInChildren<Collider>(true);
                for (int i = 0; i < rigidbodies.Length; i++)
                {
                    rigidbodies[i].enabled = enabled;
                }
            }

            public Transform GetCamera()
            {
                return camera;
            }

            public void SetCamera(Transform value)
            {
                camera = value;
            }

            public Transform GetBody()
            {
                return body;
            }

            public void SetBody(Transform value)
            {
                body = value;
            }

            public Transform GetLookAtTransform()
            {
                return lookAtTransform;
            }

            public void SetLookAtTransform(Transform value)
            {
                lookAtTransform = value;
            }

            public FreezeRotation GetFreezeRotation()
            {
                return freezeRotation;
            }

            public void SetFreezeRotation(FreezeRotation value)
            {
                freezeRotation = value;
            }

            public float GetRotationSpeed()
            {
                return rotationSpeed;
            }

            public void SetRotationSpeed(float value)
            {
                rotationSpeed = value;
            }

            public Behaviour[] GetComponents()
            {
                return componentsToDisable;
            }

            public Behaviour GetComponent(int index)
            {
                return componentsToDisable[index];
            }

            public int GetComponentsCount()
            {
                return componentsToDisable.Length;
            }

            public void SetComponentsRange(Behaviour[] value)
            {
                componentsToDisable = value;
            }

            public void SetComponent(int index, Behaviour value)
            {
                componentsToDisable[index] = value;
            }

            public Transform[] GetTransforms()
            {
                return transformsToDisable;
            }

            public Transform GetTransform(int index)
            {
                return transformsToDisable[index];
            }

            public int GetTransformsCount()
            {
                return transformsToDisable.Length;
            }

            public void SetTransformsRange(Transform[] value)
            {
                transformsToDisable = value;
            }

            public void SetTransform(int index, Transform value)
            {
                transformsToDisable[index] = value;
            }

            protected MonoBehaviour GetMono()
            {
                return mono;
            }
        }
    }
}