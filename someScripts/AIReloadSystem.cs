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

namespace UnrealFPS.AI
{
    [RequireComponent(typeof(AudioSource))]
    public class AIReloadSystem : MonoBehaviour, IWeaponReloading
    {
        [SerializeField] private int bulletCount = 31;
        [SerializeField] private int maxBulletCount = 31;
        [SerializeField] private float reloadTime = 3.0f;
        [SerializeField] private AudioClip reloadSound;

        private AudioSource audioSource;
        private bool isReloading;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Start relading process.
        /// </summary>
        public void StartRelading()
        {
            StartCoroutine(ReloadProcess());
        }

        /// <summary>
        /// Reload process coroutine.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator ReloadProcess()
        {
            WaitForSeconds updateDelay = new WaitForSeconds(reloadTime);
            isReloading = true;
            while (true)
            {
                audioSource.PlayOneShot(reloadSound);
                yield return updateDelay;
                isReloading = false;
                bulletCount = maxBulletCount;
                yield break;
            }
        }

        public bool IsReloading()
        {
            return isReloading;
        }

        public bool AddBullets(int value)
        {
            if (bulletCount == maxBulletCount)
                return false;
            else if (bulletCount + value <= maxBulletCount)
                bulletCount += value;
            return true;
        }

        public bool AddClips(int value)
        {
            return false;
        }

        public void BulletSubtraction()
        {
            if (bulletCount - 1 >= 0)
                bulletCount--;
            if (BulletsIsEmpty())
                StartRelading();
        }

        public void SetBulletCount(int value)
        {
            bulletCount = value;
        }

        public int GetBulletCount()
        {
            return bulletCount;
        }

        public void SetClipCount(int value)
        {
            // ----
        }

        public int GetClipCount()
        {
            return -1;
        }

        public void SetMaxBulletCount(int value)
        {
            maxBulletCount = value;
        }

        public int GetMaxBulletCount()
        {
            return maxBulletCount;
        }

        public void SetMaxClipCount(int value)
        {
            // ----
        }

        public int GetMaxClipCount()
        {
            return -1;
        }

        public bool ClipsIsEmpty()
        {
            return false;
        }

        public bool BulletsIsEmpty()
        {
            return bulletCount == 0;
        }

        public float GetReloadTime()
        {
            return reloadTime;
        }

        public void SetReloadTime(float value)
        {
            reloadTime = value;
        }

        public AudioClip GetReloadSound()
        {
            return reloadSound;
        }

        public void SetReloadSound(AudioClip value)
        {
            reloadSound = value;
        }
    }
}