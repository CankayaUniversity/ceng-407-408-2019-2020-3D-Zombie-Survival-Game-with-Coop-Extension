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
    public class AIHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private int health = 100;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int minHealth = 0;
        [SerializeField] private bool useRegeniration = false;
        [SerializeField] private HealthRegenerationSystem regenerationSystem = new HealthRegenerationSystem();

        private IRagdollCallbacks ragdollSystem;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            ragdollSystem = GetComponent<IRagdollCallbacks>();
            regenerationSystem.Initialize(this, this);
        }

        protected virtual void LateUpdate()
        {
            if (ragdollSystem != null)
            {
                if (!IsAlive() && !ragdollSystem.IsRagdolled())
                    ragdollSystem.IsRagdolled(true);
                else if (IsAlive() && ragdollSystem.IsRagdolled())
                    ragdollSystem.IsRagdolled(false);
            }
        }

        /// <summary>
        /// Take damage by damage
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int amount)
        {
            amount = Mathf.Abs(amount);
            health = (health - amount) >= minHealth ? health - amount : minHealth;

            if (useRegeniration)
                regenerationSystem.StartRegeneraionProcessing();
        }

        /// <summary>
        /// Health point.
        /// </summary>
        public int GetHealth()
        {
            return health;
        }

        /// <summary>
        /// Health point.
        /// </summary>
        public void SetHealth(int value)
        {
            health = value;
        }

        /// <summary>
        /// Max health point.
        /// </summary>
        public int GetMaxHealth()
        {
            return maxHealth;
        }

        /// <summary>
        /// Max health point.
        /// </summary>
        public void SetMaxHealth(int value)
        {
            maxHealth = value;
        }

        /// <summary>
        /// AI alive state.
        /// </summary>
        public bool IsAlive()
        {
            return health > 0;
        }

        /// <summary>
        /// Min health point.
        /// </summary>
        /// <returns></returns>
        public int GetMinHealth()
        {
            return minHealth;
        }

        /// <summary>
        /// Min health point.
        /// </summary>
        /// <param name="value"></param>
        public void SetMinHealth(int value)
        {
            minHealth = value;
        }

        /// <summary>
        /// Regeneration system.
        /// </summary>
        /// <returns></returns>
        public HealthRegenerationSystem GetRegenerationSystem()
        {
            return regenerationSystem;
        }

        public bool RegenirationActive()
        {
            return useRegeniration;
        }

        public void RegenirationActive(bool value)
        {
            useRegeniration = value;
        }
    }
}