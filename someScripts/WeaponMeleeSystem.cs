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

namespace UnrealFPS.Runtime
{
    [RequireComponent(typeof(IWeaponAnimator))]
    [RequireComponent(typeof(AudioSource))]
    public class WeaponMeleeSystem : MonoBehaviour, IWeaponAttack
    {
        [System.Serializable]
        public struct AttackProperties
        {
            public float range;
            public int damage;
            public float impulse;
            public float delay;
            public float hitTime;
        }

        [SerializeField] private Transform attackPoint;
        [SerializeField] private AttackProperties normalAttack;
        [SerializeField] private KeyCode specialAttackKey;
        [SerializeField] private AttackProperties specialAttack;
        [SerializeField] private DecalProperties decalProperties;

        private bool isAttacking;
        private float delayTimer = 0;
        private float currentDelay = 0;
        private AudioSource audioSource;
        private IWeaponAnimator weaponAnimator;

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
            if (!isAttacking)
            {
                if (UInput.GetButtonDown(INC.ATTACK))
                {
                    weaponAnimator.SetAttack(1);
                    StartCoroutine(DoAttack(normalAttack));
                }
                else if (Input.GetKeyDown(specialAttackKey))
                {
                    weaponAnimator.SetAttack(2);
                    StartCoroutine(DoAttack(specialAttack));
                }
            }
            else if (isAttacking && (Time.time - delayTimer) >= currentDelay)
            {
                isAttacking = false;
            }
        }

        protected virtual IEnumerator DoAttack(AttackProperties attackProperties)
        {
            WaitForSeconds hitTime = new WaitForSeconds(attackProperties.hitTime);
            WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
            RaycastHit raycastHit;
            isAttacking = true;
            delayTimer = Time.time;
            currentDelay = attackProperties.delay;
            yield return endOfFrame;
            weaponAnimator.SetAttack(-1);
            yield return hitTime;
            if (Physics.Raycast(attackPoint.position, attackPoint.forward, out raycastHit, attackProperties.range))
            {
                Debug.Log(raycastHit.transform.name);
                if (decalProperties != null)
                {
                    DecalProperty decalProperty = DecalHelper.GetDecalPropertyBySurface(decalProperties, raycastHit);
                    GameObject decal = decalProperty.GetRandomDecal();
                    AudioClip decalSoundEffect = decalProperty.GetRandomSound();
                    if (decal != null)
                        PoolManager.Instantiate(decal, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
                    if (decalSoundEffect != null)
                        audioSource.PlayOneShot(decalSoundEffect);
                }

                Rigidbody rigidbody = raycastHit.transform.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddForceAtPosition(attackPoint.forward * attackProperties.impulse, raycastHit.point);
                }

            }
        }

        public bool IsAttacking()
        {
            return isAttacking;
        }

        public void SendDamage(RaycastHit raycastHit, int damage)
        {
            IHealth health = raycastHit.transform.root.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                //crosshair.HitEffect();
            }
        }

        public Transform GetAttackPoint()
        {
            return attackPoint;
        }

        public void SetAttackPoint(Transform value)
        {
            attackPoint = value;
        }

        public void SetAttackProperties(AttackProperties normalAttack, AttackProperties specialAttack)
        {
            this.normalAttack = normalAttack;
            this.specialAttack = specialAttack;
        }

        public AttackProperties GetNormalAttackProperties()
        {
            return normalAttack;
        }

        public void SetNormalAttackProperties(AttackProperties value)
        {
            normalAttack = value;
        }

        public KeyCode GetSpecialAttackKey()
        {
            return specialAttackKey;
        }

        public void SetSpecialAttackKey(KeyCode value)
        {
            specialAttackKey = value;
        }

        public AttackProperties GetSpecialAttackProperties()
        {
            return specialAttack;
        }

        public void SetSpecialAttackProperties(AttackProperties value)
        {
            specialAttack = value;
        }

        public DecalProperties GetDecalProperties()
        {
            return decalProperties;
        }

        public void SetDecalProperties(DecalProperties value)
        {
            decalProperties = value;
        }

        public AudioSource GetAudioSource()
        {
            return audioSource;
        }

        public void SetAudioSource(AudioSource value)
        {
            audioSource = value;
        }

        public IWeaponAnimator GetweaponAnimator()
        {
            return weaponAnimator;
        }

        public void SetweaponAnimator(IWeaponAnimator value)
        {
            weaponAnimator = value;
        }
    }
}