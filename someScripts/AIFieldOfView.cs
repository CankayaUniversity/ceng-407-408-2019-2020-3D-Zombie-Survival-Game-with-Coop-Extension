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
using UnrealFPS.Runtime;

namespace UnrealFPS.AI
{
    public class AIFieldOfView : MonoBehaviour
    {
        [SerializeField] private float viewRadius = 15.0f;
        [SerializeField] private float viewAngle = 120.0f;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;

        private List<Transform> visibleTargets;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            visibleTargets = new List<Transform>();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            StartCoroutine(FindTargetsWithDelay(0.2f));
        }

        protected virtual IEnumerator FindTargetsWithDelay(float delay)
        {
            WaitForSeconds updateDelay = new WaitForSeconds(delay);
            while (true)
            {
                yield return updateDelay;
                FindVisibleTargets();
            }
        }

        protected virtual void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                if (target == this)
                {
                    continue;
                }

                IHealth health = targetsInViewRadius[i].transform.GetComponent<IHealth>();
                if (health == null)
                {
                    continue;
                }

                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2 && health.IsAlive())
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        protected virtual ViewCast ViewCast(float globalAngle)
        {
            Vector3 dir = DirectionFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
                return new ViewCast(true, hit.point, hit.distance, globalAngle);
            else
                return new ViewCast(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
                angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public float GetViewRadius()
        {
            return viewRadius;
        }

        public void SetViewRadius(float value)
        {
            viewRadius = value;
        }

        public float GetViewAngle()
        {
            return viewAngle;
        }

        public void SetViewAngle(float value)
        {
            viewAngle = value;
        }

        public LayerMask GetTargetMask()
        {
            return targetMask;
        }

        public void SetTargetMask(LayerMask value)
        {
            targetMask = value;
        }

        public LayerMask GetObstacleMask()
        {
            return obstacleMask;
        }

        public void SetObstacleMask(LayerMask value)
        {
            obstacleMask = value;
        }

        public List<Transform> GetVisibleTargets()
        {
            return visibleTargets;
        }

        public Transform GetVisibleTarget(int index)
        {
            return visibleTargets[index];
        }

        public int GetVisibleTargetsCount()
        {
            return visibleTargets.Count;
        }

        public void SetVisibleTargetsRange(List<Transform> value)
        {
            visibleTargets = value;
        }
        public void SetVisibleTarget(int index, Transform value)
        {
            visibleTargets[index] = value;
        }
    }
}