/* =====================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System.Collections;
using System.Linq;
using UnityEngine;

namespace UnrealFPS.Runtime
{
    [RequireComponent(typeof(IWeaponAnimator))]
    [RequireComponent(typeof(AudioSource))]
    public class WeaponReloadSystem : MonoBehaviour, IWeaponReloading
    {
        public enum ReloadMode
        {
            /// <summary>
            /// Default suitable for pistols, rifles weapons style.
            /// </summary>
            Default,
            /// <summary>
            /// Sequential suitable for shotgun weapons style.
            /// </summary>
            Sequential
        }

        public enum ReloadType
        {
            /// <summary>
            /// Removed all remaining bullets clip.
            /// </summary>
            Realistic,
            /// <summary>
            /// Bullets remaining in the clip are saved.
            /// </summary>
            Standard
        }

        [SerializeField] private ReloadType reloadType;
        [SerializeField] private ReloadMode reloadMode;
        [SerializeField] private int bulletCount;
        [SerializeField] private int clipCount;
        [SerializeField] private int maxBulletCount;
        [SerializeField] private int maxClipCount;

        //Default
        [SerializeField] private float normalReloadTime;
        [SerializeField] private float emptyReloadTime;

        //Sequential
        [SerializeField] private float startTime;
        [SerializeField] private float iterationTime;

        [SerializeField] private AnimationEventProperties reloadEventProperties;

        private bool isReloading;
        private float reloadTime;
        private IWeaponAnimator weaponAnimator;
        private AudioSource audioSource;
        private IEnumerator reloadEvent;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            weaponAnimator = GetComponent<IWeaponAnimator>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (UInput.GetButtonDown(INC.RELOAD) &&
                !ClipsIsEmpty() &&
                !isReloading &&
                bulletCount < maxBulletCount)
            {
                isReloading = true;
                switch (reloadMode)
                {
                    case ReloadMode.Default:
                        DefaultReload();
                        break;
                    case ReloadMode.Sequential:
                        SequentialReload();
                        break;
                }
            }
        }

        /// <summary>
        /// Calculate bullet and clip count after a specified time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public IEnumerator Reload(float time)
        {
            yield return new WaitForSeconds(time);
            weaponAnimator.SetReload(-1);
            ReCalculateAmmo(reloadType);
            isReloading = false;
            reloadTime = 0;
            yield break;
        }

        /// <summary>
        /// Calculate bullet and clip count
        /// </summary>
        public virtual void ReCalculateAmmo(ReloadType reloadType)
        {
            switch (reloadType)
            {
                case ReloadType.Standard:
                    StandardCalculate();
                    break;
                case ReloadType.Realistic:
                    RealisticCalculate();
                    break;
            }
        }

        /// <summary>
        /// Calculate bullet and clip count
        /// </summary>
        public virtual void ReCalculateAmmo()
        {
            switch (reloadType)
            {
                case ReloadType.Standard:
                    StandardCalculate();
                    break;
                case ReloadType.Realistic:
                    RealisticCalculate();
                    break;
            }
        }

        protected virtual void StandardCalculate()
        {
            if (clipCount >= maxBulletCount)
            {
                clipCount -= (maxBulletCount - bulletCount);
                bulletCount = maxBulletCount;
            }
            else if (clipCount < maxBulletCount)
            {
                bulletCount = clipCount + bulletCount;
                clipCount = 0;
            }
        }

        protected virtual void RealisticCalculate()
        {
            if (clipCount >= maxBulletCount)
            {
                clipCount -= maxBulletCount;
                bulletCount = maxBulletCount;
            }
            else if (clipCount < maxBulletCount)
            {
                bulletCount = clipCount + bulletCount;
                clipCount = 0;
            }
        }

        /// <summary>
        /// Default reload handler
        /// </summary>
        protected virtual void DefaultReload()
        {
            if (!BulletsIsEmpty())
            {
                weaponAnimator.SetReload(1);
                reloadTime = normalReloadTime;
            }
            else
            {
                weaponAnimator.SetReload(0);
                reloadTime = emptyReloadTime;
            }
            StartCoroutine(Reload(reloadTime));

        }

        /// <summary>
        /// Sequential reload handler
        /// </summary>
        protected virtual void SequentialReload()
        {
            reloadTime = CalculateMaxTime(bulletCount, maxBulletCount, startTime, iterationTime);
            weaponAnimator.SetReload(1);
            StartCoroutine(Reload(reloadTime));
        }

        /// <summary>
        /// Calculates the maximum time required for recharging
        /// </summary>
        /// <param name="bulletCount"></param>
        /// <param name="maxBulletCount"></param>
        /// <param name="startTime"></param>
        /// <param name="iterationTime"></param>
        /// <returns></returns>
        public virtual float CalculateMaxTime(float bulletCount, float maxBulletCount, float startTime, float iterationTime)
        {
            float totalTime;
            float requiredBullet = maxBulletCount - bulletCount;
            totalTime = iterationTime * requiredBullet;
            totalTime += startTime;
            return totalTime;
        }

        public bool IsReloading()
        {
            return isReloading;
        }

        protected void IsReloading(bool value)
        {
            isReloading = value;
        }

        public bool AddBullets(int value)
        {
            if (bulletCount + value <= maxBulletCount)
            {
                bulletCount += value;
            }
            else if (clipCount + (value - (maxBulletCount - bulletCount)) < maxClipCount)
            {
                clipCount = value - (maxBulletCount - bulletCount);
                bulletCount = maxBulletCount;
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool AddClips(int value)
        {
            if (clipCount == maxClipCount)
                return false;
            else if (clipCount + value < maxClipCount)
                clipCount += value;
            return true;
        }

        /// <summary>
        /// Bullets
        /// </summary>
        /// <returns>Bullet Count</returns>
        public int GetBulletCount()
        {
            return bulletCount;
        }

        /// <summary>
        /// Bullets
        /// </summary>
        /// <returns>Bullet Count</returns>
        public void SetBulletCount(int value)
        {
            if (value <= maxBulletCount)
                bulletCount = value;
            else
                bulletCount = maxBulletCount;
        }

        /// <summary>
        /// Clips
        /// </summary>
        /// <returns>Clip Count</returns>
        public int GetClipCount()
        {
            return clipCount;
        }

        /// <summary>
        /// Clips
        /// </summary>
        /// <returns>Clip Count</returns>
        public void SetClipCount(int value)
        {
            if (value <= maxClipCount)
                clipCount = value;
            else
                clipCount = maxClipCount;
        }

        public virtual void BulletSubtraction()
        {
            bulletCount--;
        }

        /// <summary>
        /// 
        /// </summary>
        public int GetMaxBulletCount()
        {
            return maxBulletCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetMaxBulletCount(int value)
        {
            maxBulletCount = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int GetMaxClipCount()
        {
            return maxClipCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetMaxClipCount(int value)
        {
            maxClipCount = value;
        }

        public bool ClipsIsEmpty()
        {
            return (clipCount <= 0);
        }

        public bool BulletsIsEmpty()
        {
            return (bulletCount <= 0);
        }

        public ReloadMode GetReloadMode()
        {
            return reloadMode;
        }

        public void SetReloadMode(ReloadMode value)
        {
            reloadMode = value;
        }

        public ReloadType GetReloadType()
        {
            return reloadType;
        }

        public void SetReloadType(ReloadType value)
        {
            reloadType = value;
        }

        public float GetStartTime()
        {
            return startTime;
        }

        public void SetStartTime(float value)
        {
            startTime = value;
        }

        public float GetIterationTime()
        {
            return iterationTime;
        }

        public void SetIterationTime(float value)
        {
            iterationTime = value;
        }

        public float GetReloadTime()
        {
            return normalReloadTime;
        }

        public void SetReloadTime(float value)
        {
            normalReloadTime = value;
        }

        public float GetEmptyReloadTime()
        {
            return emptyReloadTime;
        }

        public void SetEmptyReloadTime(float value)
        {
            emptyReloadTime = value;
        }

        public AnimationEventProperties GetReloadEventProperties()
        {
            return reloadEventProperties;
        }

        public void SetReloadEventProperties(AnimationEventProperties value)
        {
            reloadEventProperties = value;
        }
    }
}