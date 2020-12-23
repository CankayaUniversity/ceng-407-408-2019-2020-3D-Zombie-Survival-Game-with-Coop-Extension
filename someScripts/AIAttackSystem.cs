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
    [RequireComponent(typeof(AIReloadSystem))]
    [RequireComponent(typeof(AudioSource))]
    public class AIAttackSystem : MonoBehaviour, IWeaponAttack
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private RayBullet bullet;
        [SerializeField] private float spreadValue = 2.5f;
        [SerializeField] private float range = 100.0f;
        [SerializeField] private float delay = 0.1f;
        [SerializeField] private float impulse = 50.0f;
        [SerializeField] private AudioClip[] fireSounds;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private ParticleSystem cartridgeEjection;

        private AIReloadSystem reloadSystem;
        private AudioSource audioSource;
        private RaycastHit raycastHit;
        private IEnumerator shootingCoroutine;
        private Transform targetCache;
        private bool isAttacking;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            reloadSystem = GetComponent<AIReloadSystem>();
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Start shooting process.
        /// </summary>
        /// <param name="target"></param>
        public void StartShooting(Transform target)
        {
            targetCache = target;
            if (shootingCoroutine != null)
                return;
            isAttacking = true;
            shootingCoroutine = ShootingProcess();
            StartCoroutine(shootingCoroutine);
        }

        /// <summary>
        /// Stop shooting process.
        /// </summary>
        public void StopShooting()
        {
            if (shootingCoroutine == null)
                return;
            isAttacking = false;
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }

        protected virtual IEnumerator ShootingProcess()
        {
            WaitForSeconds updateDelay = new WaitForSeconds(delay);
            while (true)
            {
                yield return updateDelay;
                if (!reloadSystem.BulletsIsEmpty())
                {
                    DoShoot();
                }
            }
        }

        protected virtual void DoShoot()
        {
            if (targetCache == null)
                return;

            ApplySpread();
            if (bullet.GetNumberBullet() < 100)
            {
                firePoint.localRotation = Quaternion.Euler(Random.Range(-bullet.GetVariance(), bullet.GetVariance()), Random.Range(-bullet.GetVariance(), bullet.GetVariance()), 0);
            }

            Vector3 targetDirection = targetCache.position - firePoint.position;
            if (Physics.Raycast(firePoint.position, targetDirection, out raycastHit, range))
            {
                Debug.DrawRay(firePoint.position, targetDirection, Color.red);
                PlayFireSound();
                reloadSystem.BulletSubtraction();
                SendDamage(raycastHit, bullet.GetDamage());
                CreateDecal(raycastHit);
                AddPhysicsImpact(raycastHit);
            }
        }

        /// <summary>
        /// Play fire sound.
        /// </summary>
        public void PlayFireSound()
        {
            if (fireSounds.Length == 0)
                return;

            AudioClip sound = fireSounds[Random.Range(0, fireSounds.Length)];
            if (sound != null)
            {
                audioSource.PlayOneShot(sound);
            }
        }

        /// <summary>
        /// Send damage from bullet.
        /// </summary>
        /// <param name="raycastHit"></param>
        /// <param name="damage"></param>
        public void SendDamage(RaycastHit raycastHit, int damage)
        {
            IHealth healthCallbacks = raycastHit.transform.root.GetComponent<IHealth>();
            if (healthCallbacks != null)
            {
                healthCallbacks.TakeDamage(damage);
            }
        }

        /// <summary>
        /// Add physics impact from bullet on physics object. 
        /// </summary>
        /// <param name="raycastHit"></param>
        public void AddPhysicsImpact(RaycastHit raycastHit)
        {
            Rigidbody rigidbody = raycastHit.transform.root.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForceAtPosition(firePoint.forward * impulse, raycastHit.point);
            }
        }

        public void CreateDecal(RaycastHit raycastHit)
        {
            if (bullet.GetDecalProperties() != null)
            {
                DecalProperty decalProperty = DecalHelper.GetDecalPropertyBySurface(bullet.GetDecalProperties(), raycastHit);
                GameObject decal = decalProperty.GetRandomDecal();
                AudioClip decalSoundEffect = decalProperty.GetRandomSound();
                if (decal != null)
                    PoolManager.Instantiate(decal, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
                if (decalSoundEffect != null)
                    audioSource.PlayOneShot(decalSoundEffect);
            }
        }

        public void ApplySpread()
        {
            float xRot = Random.Range(-spreadValue, spreadValue);
            float yRot = Random.Range(-spreadValue, spreadValue);
            firePoint.localRotation = Quaternion.Euler(xRot, yRot, 0);
        }

        public Transform GetFirePoint()
        {
            return firePoint;
        }

        public void SetFirePoint(Transform value)
        {
            firePoint = value;
        }

        public RayBullet GetBullet()
        {
            return bullet;
        }

        public void SetBullet(RayBullet value)
        {
            bullet = value;
        }

        public float GetSpreadValue()
        {
            return spreadValue;
        }

        public void SetSpreadValue(float value)
        {
            spreadValue = value;
        }

        public float GetRange()
        {
            return range;
        }

        public void SetRange(float value)
        {
            range = value;
        }

        public float GetDelay()
        {
            return delay;
        }

        public void SetDelay(float value)
        {
            delay = value;
        }

        public float GetImpulse()
        {
            return impulse;
        }

        public void SetImpulse(float value)
        {
            impulse = value;
        }

        public AudioClip[] GetFireSounds()
        {
            return fireSounds;
        }

        public void SetFireSoundsRange(AudioClip[] value)
        {
            fireSounds = value;
        }

        public AudioClip GetFireSound(int index)
        {
            return fireSounds[index];
        }

        public void SetFireSound(int index, AudioClip value)
        {
            fireSounds[index] = value;
        }

        public ParticleSystem GetMuzzleFlash()
        {
            return muzzleFlash;
        }

        public void SetMuzzleFlash(ParticleSystem value)
        {
            muzzleFlash = value;
        }

        public ParticleSystem GetCartridgeEjection()
        {
            return cartridgeEjection;
        }

        public void SetCartridgeEjection(ParticleSystem value)
        {
            cartridgeEjection = value;
        }

        public bool IsAttacking()
        {
            return isAttacking;
        }

        public AIReloadSystem GetReloadSystem()
        {
            return reloadSystem;
        }

        public void SetReloadSystem(AIReloadSystem value)
        {
            reloadSystem = value;
        }

        public AudioSource GetAudioSource()
        {
            return audioSource;
        }

        public void SetAudioSource(AudioSource value)
        {
            audioSource = value;
        }
    }
}