/* =====================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System.Collections;
using UnityEngine;
using UnrealFPS.UI;
using UnrealFPS.Utility;

namespace UnrealFPS.Runtime
{
    /// <summary>
    ///     /// Base weapon shooth system class
    /// </summary>
    [RequireComponent(typeof(IWeaponReloading))]
    [RequireComponent(typeof(IWeaponAnimator))]
    [RequireComponent(typeof(AudioSource))]
    public partial class WeaponShootingSystem : MonoBehaviour, IWeaponAttack
    {
        /// <summary>
        /// Weapon shooting type
        /// 	RayCast - Shoot by ray.
        /// 	Physics - Shoot by physics (Rigidbody).
        /// </summary>
        public enum BulletType
        {
            RayBullet,
            PhysicsBullet
        }

        /// <summary>
        /// Shooting mode of the weapon.
        ///     Single - Single fire.
        ///     Fixed queue - Fire with fixed queue (for example 3 fire).
        ///     Free - Free fire without restrictions.
        /// </summary>
        public enum Mode
        {
            Single,
            Queue,
            Free
        }

        [System.Serializable]
        public struct FireEffectProperties
        {
            public ParticleSystem muzzleFlash;
            public ParticleSystem cartridgeEjection;
        }

        [SerializeField] private Mode fireMode;
        [SerializeField] private BulletType bulletType;
        [SerializeField] private Transform firePoint;
        [SerializeField] private PhysicsBullet physicsBullet;
        [SerializeField] private RayBullet rayBullet;
        [SerializeField] private float delay;
        [SerializeField] private int queueCount;
        [SerializeField] private float fireImpulse;
        [SerializeField] private float fireRange;
        [SerializeField] private SoundProperties soundProperties;
        [SerializeField] private FireEffectProperties fireEffects;
        [SerializeField] private SpreadProperties spreadProperties;

        private bool isShooting;
        private IEnumerator shootingProcessing;
        private IWeaponReloading weaponReloading;
        private IWeaponAnimator weaponAnimator;
        private AudioSource audioSource;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            weaponReloading = GetComponent<IWeaponReloading>();
            weaponAnimator = GetComponent<IWeaponAnimator>();

            if (spreadProperties != null)
                spreadProperties.Initialize(firePoint);
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (shootingProcessing != null)
            {
                return;
            }
            shootingProcessing = ShootingProcessing();
            StartCoroutine(shootingProcessing);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (shootingProcessing == null)
            {
                return;
            }
            StopCoroutine(shootingProcessing);
            shootingProcessing = null;
        }

        protected virtual void Update()
        {
            AnimatorStateProcessing();
        }

        /// <summary>
        /// Shooting system processing.
        /// </summary>
        protected virtual IEnumerator ShootingProcessing()
        {
            WaitForSeconds updateDelay = new WaitForSeconds(delay);
            WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
            while (true)
            {
                if (!weaponReloading.BulletsIsEmpty() && !weaponReloading.IsReloading())
                {
                    if (fireMode == Mode.Free && UInput.GetButton(INC.ATTACK) || fireMode == Mode.Single && UInput.GetButtonDown(INC.ATTACK))
                    {
                        DoShoot();
                        yield return updateDelay;
                    }
                    else if (fireMode == Mode.Queue && UInput.GetButtonDown(INC.ATTACK))
                    {
                        for (int i = 0; i < queueCount; i++)
                        {
                            DoShoot();
                            weaponAnimator.SetAttack(1);
                            yield return endOfFrame;
                            weaponAnimator.SetAttack(-1);
                            yield return updateDelay;
                        }
                    }
                }
                else if (UInput.GetButtonDown(INC.ATTACK))
                {
                    AudioClip sound = soundProperties.GetRandomEmptySound();
                    if (sound != null)
                    {
                        audioSource.PlayOneShot(sound);
                    }
                    weaponAnimator.SetAttack(0);
                    yield return endOfFrame;
                    weaponAnimator.SetAttack(-1);
                }
                yield return null;
            }
        }

        /// <summary>
        /// Do shoot.
        /// </summary>
        protected virtual void DoShoot()
        {
            if (spreadProperties != null)
                spreadProperties.ApplySpread(weaponAnimator.GetActiveState());

            switch (bulletType)
            {
                case BulletType.RayBullet:
                    RayBulletShootingProcess();
                    break;
                case BulletType.PhysicsBullet:
                    PhysicsBulletShootingProcess();
                    break;
            }

            AudioClip sound = soundProperties.GetRandomFireSound();
            if (sound != null)
            {
                audioSource.PlayOneShot(sound);
            }

            weaponReloading.BulletSubtraction();
            AttackParticleEffect();

            if (spreadProperties != null)
                spreadProperties.ResetRotation();
        }

        private void AnimatorStateProcessing()
        {
            if (fireMode == Mode.Free && UInput.GetButton(INC.ATTACK) && !weaponReloading.BulletsIsEmpty())
                weaponAnimator.SetAttack(1);
            else if (fireMode == Mode.Free && !UInput.GetButton(INC.ATTACK))
                weaponAnimator.SetAttack(-1);

            if (fireMode == Mode.Single && UInput.GetButtonDown(INC.ATTACK) && !weaponReloading.BulletsIsEmpty())
                weaponAnimator.SetAttack(1);
            else if (fireMode == Mode.Single && !UInput.GetButtonDown(INC.ATTACK))
                weaponAnimator.SetAttack(-1);
        }

        /// <summary>
        /// Shooting using RayCast.
        /// </summary>
        protected virtual void RayBulletShootingProcess()
        {
            RaycastHit raycastHit;
            for (int i = 0; i < rayBullet.GetNumberBullet(); i++)
            {
                if (rayBullet.GetNumberBullet() > 1)
                {
                    firePoint.localRotation = SpreadProperties.GetSpreadRotation(-rayBullet.GetVariance(), rayBullet.GetVariance());
                }
                if (Physics.Raycast(firePoint.position, firePoint.forward, out raycastHit, fireRange))
                {
                    Debug.DrawLine(firePoint.position, firePoint.forward, Color.red);
                    Transform hitTransform = raycastHit.transform;
                    SendDamage(hitTransform, rayBullet.GetDamage());
                    if (rayBullet.GetDecalProperties() != null)
                    {
                        DecalProperty decalProperty = DecalHelper.GetDecalPropertyBySurface(rayBullet.GetDecalProperties(), raycastHit);
                        GameObject decal = decalProperty.GetRandomDecal();
                        AudioClip decalSoundEffect = decalProperty.GetRandomSound();
                        if (decal != null)
                            PoolManager.Instantiate(decal, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
                        if (decalSoundEffect != null)
                            audioSource.PlayOneShot(decalSoundEffect);
                    }
                    AddImpulse(transform, raycastHit.point);
                }
            }
        }

        /// <summary>
        /// Shooting using physics bullet.
        /// </summary>
        protected virtual void PhysicsBulletShootingProcess()
        {
            for (int i = 0; i < physicsBullet.GetNumberBullet(); i++)
            {
                if (physicsBullet.GetNumberBullet() > 1)
                {
                    firePoint.localRotation = SpreadProperties.GetSpreadRotation(-physicsBullet.GetVariance(), physicsBullet.GetVariance());
                }
                GameObject physicsBulletClone = PoolManager.Instantiate(physicsBullet.gameObject, firePoint.position, firePoint.rotation);
                Rigidbody rigidbody = physicsBulletClone.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddForce(firePoint.forward * fireImpulse, ForceMode.Impulse);
                }
            }
        }

        /// <summary>
        /// Send damage from attack.
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="damage"></param>
        protected virtual void SendDamage(Transform transform, int damage)
        {
            IHealth health = transform.GetComponent<IHealth>();
            if (health != null)
            {
                Debug.Log("Health " + transform.name);
                health.TakeDamage(damage);
                //crosshair.HitEffect();
            }
        }

        protected void AddImpulse(Transform transform, Vector3 point)
        {
            Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForceAtPosition(firePoint.forward * fireImpulse, point);
            }
        }

        /// <summary>
        /// Play attack particle effect.
        /// </summary>
        protected void AttackParticleEffect()
        {
            if (fireEffects.muzzleFlash != null)fireEffects.muzzleFlash.Play();
            if (fireEffects.cartridgeEjection != null)fireEffects.cartridgeEjection.Play();
        }

        protected void PlayFireSound()
        {
            AudioClip sound = soundProperties.GetRandomFireSound();
            if (sound != null)
                audioSource.PlayOneShot(sound);
        }

        protected void PlayEmptySound()
        {
            AudioClip sound = soundProperties.GetRandomEmptySound();
            if (sound != null)
                audioSource.PlayOneShot(sound);
        }

        public bool IsAttacking()
        {
            return isShooting;
        }

        protected void IsAttacking(bool isShooting)
        {
            this.isShooting = isShooting;
        }

        public Mode GetMode()
        {
            return fireMode;
        }

        public void SetMode(Mode value)
        {
            fireMode = value;
        }

        public BulletType GetBulletType()
        {
            return bulletType;
        }

        public void SetBulletType(BulletType value)
        {
            bulletType = value;
        }

        public Transform GetFirePoint()
        {
            return firePoint;
        }

        public void SetFirePoint(Transform value)
        {
            firePoint = value;
        }

        public RayBullet GetRayBullet()
        {
            return rayBullet;
        }

        public void SetRayBullet(RayBullet value)
        {
            rayBullet = value;
        }

        public PhysicsBullet GetPhysicsBullet()
        {
            return physicsBullet;
        }

        public void SetPhysicsBullet(PhysicsBullet value)
        {
            physicsBullet = value;
        }

        public float GetDelay()
        {
            return delay;
        }

        public void SetDelay(float value)
        {
            delay = value;
        }

        public int GetFixedQueueCount()
        {
            return queueCount;
        }

        public void SetFixedQueueCount(int value)
        {
            queueCount = value;
        }

        public float GetFireImpulse()
        {
            return fireImpulse;
        }

        public void SetFireImpulse(float value)
        {
            fireImpulse = value;
        }

        public float GetFireRange()
        {
            return fireRange;
        }

        public void SetFireRange(float value)
        {
            fireRange = value;
        }

        public FireEffectProperties GetFireEffects()
        {
            return fireEffects;
        }

        public void SetFireEffects(FireEffectProperties value)
        {
            fireEffects = value;
        }

        public SoundProperties GetSoundProperties()
        {
            return soundProperties;
        }

        public void SetSoundProperties(SoundProperties value)
        {
            soundProperties = value;
        }

        public SpreadProperties GetSpreadProperties()
        {
            return spreadProperties;
        }

        public void SetSpreadProperties(SpreadProperties value)
        {
            spreadProperties = value;
        }
    }
}