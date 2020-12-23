/* =====================================================================
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

namespace UnrealFPS.Runtime
{
    public class AdvancedInventory : MonoBehaviour, IInventory, IAdvancedInventory
    {
        [SerializeField] private Transform _FPCamera;
        [SerializeField] private List<InventoryGroup> groups;
        [SerializeField] private bool allowIdenticalWeapons = false;
        [SerializeField] private SwitchWeaponMode switchMode = SwitchWeaponMode.Both;
        [SerializeField] private KeyCode startWeaponKey;

        private Dictionary<string, List<InventorySlot>> cacheGroupToSlots;
        private Dictionary<KeyCode, WeaponID> cacheKeyToID;
        private Dictionary<WeaponID, Transform> cacheIDToTransform;

        private KeyCode currentWeaponKey;
        private IEnumerator switchWeaponAnimation;
        private IEnumerator replaceWeaponAnimation;
        private IEnumerator hideWeaponAnimation;
        private IEnumerator dropWeaponAnimation;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            cacheKeyToID = GetKeyToID();
            cacheIDToTransform = GetIDToTransform();
            cacheGroupToSlots = GetGroupToSlots();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            if (startWeaponKey != KeyCode.None)
            {
                ActivateWeapon(startWeaponKey);
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (groups.Count == 0)
            {
                return;
            }

            switch (switchMode)
            {
                case SwitchWeaponMode.ByKey:
                    SelectWeaponByKey();
                    break;
                case SwitchWeaponMode.ByMouseWheel:
                    SelectWeaponByMouseWheel();
                    break;
                case SwitchWeaponMode.Both:
                    SelectWeaponByKey();
                    SelectWeaponByMouseWheel();
                    break;
            }

            if (UInput.GetButtonDown(INC.DROP))
            {
                Drop();
            }
        }

        /// <summary>
        /// Add new weapon in available slot in inventory.
        /// </summary>
        public virtual bool Add(WeaponID weapon)
        {
            if (weapon == null || !cacheGroupToSlots.ContainsKey(weapon.GetGroup()) || (allowIdenticalWeapons && cacheKeyToID.ContainsValue(weapon)))
                return false;

            List<InventorySlot> slots = cacheGroupToSlots[weapon.GetGroup()];
            if (slots == null || slots.Count == 0)
                return false;

            for (int i = 0, length = slots.Count; i < length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.GetWeapon() == null || (i == length - 1 && slot.GetWeapon() != null))
                {
                    cacheKeyToID[slot.GetKey()] = weapon;
                    slot.SetWeapon(weapon);
                    cacheGroupToSlots[weapon.GetGroup()][i] = slot;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove weapon from inventory.
        /// </summary>
        public virtual bool Remove(WeaponID weapon)
        {
            if (weapon == null || !cacheGroupToSlots.ContainsKey(weapon.GetGroup()) || !cacheKeyToID.ContainsValue(weapon))
                return false;

            List<InventorySlot> slots = cacheGroupToSlots[weapon.GetGroup()];
            if (slots == null || slots.Count == 0)
                return false;

            for (int i = 0, length = slots.Count; i < length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.GetWeapon() == weapon)
                {
                    cacheKeyToID[slot.GetKey()] = null;
                    slot.SetWeapon(null);
                    cacheGroupToSlots[weapon.GetGroup()][i] = slot;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replace current weapon on new.
        /// </summary>
        public bool Replace(WeaponID weapon)
        {
            if (weapon == null || currentWeaponKey == KeyCode.None || !cacheGroupToSlots.ContainsKey(weapon.GetGroup()))
                return false;

            WeaponID currentWeapon = GetActiveWeaponID();
            if (currentWeapon.GetGroup() != weapon.GetGroup())
            {
                return false;
            }

            replaceWeaponAnimation = ReplaceWeaponAnimation(weapon);
            StartCoroutine(replaceWeaponAnimation);

            List<InventorySlot> slots = cacheGroupToSlots[weapon.GetGroup()];
            if (slots == null || slots.Count == 0)
                return false;

            for (int i = 0, length = slots.Count; i < length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.GetWeapon() == currentWeapon)
                {
                    cacheKeyToID[slot.GetKey()] = weapon;
                    slot.SetWeapon(weapon);
                    cacheGroupToSlots[weapon.GetGroup()][i] = slot;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Replace inventory weapon on some new weapon.
        /// </summary>
        public bool Replace(WeaponID inventoryWeapon, WeaponID someWeapon)
        {
            if (inventoryWeapon == null || someWeapon != null ||
                currentWeaponKey == KeyCode.None ||
                !cacheGroupToSlots.ContainsKey(inventoryWeapon.GetGroup()) || !cacheKeyToID.ContainsValue(inventoryWeapon) ||
                !cacheGroupToSlots.ContainsKey(someWeapon.GetGroup()) || !cacheKeyToID.ContainsValue(someWeapon))
                return false;

            if (inventoryWeapon.GetGroup() != someWeapon.GetGroup())
            {
                return false;
            }

            WeaponID currentWeapon = GetActiveWeaponID();
            if (currentWeapon == inventoryWeapon)
            {
                replaceWeaponAnimation = ReplaceWeaponAnimation(someWeapon);
                StartCoroutine(replaceWeaponAnimation);
            }

            List<InventorySlot> slots = cacheGroupToSlots[inventoryWeapon.GetGroup()];
            if (slots == null || slots.Count == 0)
                return false;

            for (int i = 0, length = slots.Count; i < length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.GetWeapon() == inventoryWeapon)
                {
                    cacheKeyToID[slot.GetKey()] = someWeapon;
                    slot.SetWeapon(someWeapon);
                    cacheGroupToSlots[someWeapon.GetGroup()][i] = slot;
                    switchWeaponAnimation = SwitchWeaponAnimation(someWeapon);
                    StartCoroutine(switchWeaponAnimation);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add new group in inventory.
        /// </summary>
        public virtual bool AddGroup(string groupName, params InventorySlot[] slots)
        {
            if (string.IsNullOrEmpty(groupName))
                return false;

            List<InventorySlot> _slots = new List<InventorySlot>();
            if (slots != null && slots.Length > 0)
            {
                for (int i = 0, length = slots.Length; i < length; i++)
                {
                    InventorySlot slot = slots[i];
                    _slots.Add(slot);
                    cacheKeyToID.Add(slot.GetKey(), slot.GetWeapon());
                }
            }
            groups.Add(new InventoryGroup(groupName, _slots));
            cacheGroupToSlots.Add(groupName, _slots);
            return true;
        }

        /// <summary>
        /// Remove group from inventory including weapons contained in this group.
        /// </summary>
        public virtual bool RemoveGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return false;

            List<InventorySlot> _slots = cacheGroupToSlots[groupName];
            if (_slots != null && _slots.Count > 0)
            {
                for (int i = 0, length = _slots.Count; i < length; i++)
                {
                    InventorySlot slot = _slots[i];
                    if (cacheKeyToID.ContainsKey(slot.GetKey()))
                    {
                        if (currentWeaponKey == slot.GetKey())
                        {
                            HideWeapon(true);
                            currentWeaponKey = KeyCode.None;
                        }
                        cacheKeyToID.Remove(slot.GetKey());
                    }
                }
            }

            for (int i = 0, length = groups.Count; i < length; i++)
            {
                InventoryGroup group = groups[i];
                if (group.GetName() == groupName)
                {
                    groups.RemoveAt(i);
                    break;
                }
            }

            if (cacheGroupToSlots.ContainsKey(groupName))
                cacheGroupToSlots.Remove(groupName);
            return true;
        }

        /// <summary>
        /// Add new slot in group.
        /// </summary>
        public virtual bool AddSlotInGroup(string groupName, InventorySlot slot)
        {
            if (string.IsNullOrEmpty(groupName) || slot == null || !cacheGroupToSlots.ContainsKey(groupName))
                return false;

            for (int i = 0, length = groups.Count; i < length; i++)
            {
                if (groups[i].GetName() == groupName)
                {
                    groups[i].GetInventorySlots().Add(slot);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove slot from group including weapon contained in this slot.
        /// </summary>
        public virtual bool RemoveSlotFromGroup(string groupName, KeyCode key)
        {
            if (string.IsNullOrEmpty(groupName) || !cacheGroupToSlots.ContainsKey(groupName))
                return false;

            for (int i = 0, length = groups.Count; i < length; i++)
            {
                InventoryGroup group = groups[i];
                if (group.GetName() == groupName)
                {
                    for (int j = 0, _length = group.GetInventorySlotsLength(); j < _length; j++)
                    {
                        InventorySlot slot = group.GetInventorySlot(j);
                        if (slot.GetKey() == key)
                        {
                            if (currentWeaponKey == slot.GetKey())
                            {
                                HideWeapon(true);
                                currentWeaponKey = KeyCode.None;
                            }
                            groups[i].GetInventorySlots().RemoveAt(j);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Activate weapon by key.
        /// </summary>
        public virtual bool ActivateWeapon(KeyCode key)
        {
            if (!cacheKeyToID.ContainsKey(key))
                return false;

            if (hideWeaponAnimation != null || switchWeaponAnimation != null || dropWeaponAnimation != null)
                return false;

            switchWeaponAnimation = SwitchWeaponAnimation(key);
            StartCoroutine(switchWeaponAnimation);
            return true;
        }

        /// <summary>
        /// Activate weapon by ID.
        /// </summary>
        public virtual bool ActivateWeapon(WeaponID weapon)
        {
            if (!cacheKeyToID.ContainsValue(weapon))
                return false;

            if (hideWeaponAnimation != null || switchWeaponAnimation != null || dropWeaponAnimation != null)
                return false;

            switchWeaponAnimation = SwitchWeaponAnimation(weapon);
            StartCoroutine(switchWeaponAnimation);
            return true;
        }

        /// <summary>
        /// Get weapon transform by id.
        /// </summary>
        public virtual Transform GetWeapon(WeaponID weapon)
        {
            if (weapon == null || !cacheKeyToID.ContainsValue(weapon))
                return null;
            return cacheIDToTransform[weapon];
        }

        /// <summary>
        /// Hide active weapon.
        /// </summary>
        public virtual void HideWeapon(bool hide)
        {
            if ((currentWeaponKey == KeyCode.None) ||
                (hideWeaponAnimation != null || switchWeaponAnimation != null || dropWeaponAnimation != null))
                return;

            hideWeaponAnimation = HideWeaponAnimation();
            StartCoroutine(hideWeaponAnimation);
        }

        /// <summary>
        /// Drop active weapon.
        /// </summary>
        public virtual void Drop()
        {
            if ((currentWeaponKey == KeyCode.None) ||
                (hideWeaponAnimation != null || switchWeaponAnimation != null || dropWeaponAnimation != null))
                return;

            dropWeaponAnimation = DropWeaponAnimation();
            StartCoroutine(dropWeaponAnimation);
        }

        /// <summary>
        /// Get active weapon transform.
        /// </summary>
        public virtual Transform GetActiveWeaponTransform()
        {
            if (currentWeaponKey == KeyCode.None || !cacheKeyToID.ContainsKey(currentWeaponKey))
                return null;

            WeaponID id = cacheKeyToID[currentWeaponKey];
            if (id == null)
                return null;

            return cacheIDToTransform[id];
        }

        /// <summary>
        /// Get active WeaponID.
        /// </summary>
        /// <returns></returns>
        public virtual WeaponID GetActiveWeaponID()
        {
            if (currentWeaponKey == KeyCode.None || !cacheKeyToID.ContainsKey(currentWeaponKey))
                return null;
            return cacheKeyToID[currentWeaponKey];
        }

        /// <summary>
        /// Weapon count.
        /// </summary>
        /// <returns></returns>
        public virtual int WeaponCount()
        {
            int count = 0;
            for (int i = 0, length = groups.Count; i < length; i++)
            {
                InventoryGroup group = groups[i];
                for (int j = 0, _length = group.GetInventorySlotsLength(); j < length; j++)
                {
                    InventorySlot slot = group.GetInventorySlot(i);
                    if (slot.GetWeapon() != null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Slot count, the maximum possible wearable number of weapons.
        /// </summary>
        public virtual int SlotCount(string groupName)
        {
            if (string.IsNullOrEmpty(groupName) || !cacheGroupToSlots.ContainsKey(groupName) || cacheGroupToSlots[groupName] == null)
                return 0;
            return cacheGroupToSlots[groupName].Count;
        }

        /// <summary>
        /// Group count.
        /// </summary>
        public virtual int GroupCount(string groupName)
        {
            return groups.Count;
        }

        public virtual bool IsFull(string group)
        {
            if (string.IsNullOrEmpty(group) || !cacheGroupToSlots.ContainsKey(group))
            {
                return true;
            }

            List<InventorySlot> slots = cacheGroupToSlots[group];
            for (int i = 0, length = slots.Count; i < length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.GetWeapon() == null)
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool IsFull(object message = null)
        {
            if (message == null)
            {
                return true;
            }

            string group = message as string;
            if (string.IsNullOrEmpty(group) || !cacheGroupToSlots.ContainsKey(group))
            {
                return true;
            }

            List<InventorySlot> slots = cacheGroupToSlots[group];
            for (int i = 0, length = slots.Count; i < length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.GetWeapon() == null)
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual void SelectWeaponByKey()
        {
            if (!Input.anyKeyDown || (switchWeaponAnimation != null || hideWeaponAnimation != null) || Input.GetKeyDown(currentWeaponKey))
                return;

            for (int i = 0, length = groups.Count; i < length; i++)
            {
                InventoryGroup group = groups[i];
                for (int j = 0, _length = group.GetInventorySlotsLength(); j < _length; j++)
                {
                    InventorySlot slot = group.GetInventorySlot(j);
                    if (Input.GetKeyDown(slot.GetKey()))
                    {
                        if (slot.GetWeapon() == null)
                            break;
                        ActivateWeapon(slot.GetKey());
                    }
                }
            }
        }

        protected virtual void SelectWeaponByMouseWheel()
        {
            if (UInput.GetAxisRaw(INC.MOUSE_WHEEL) == 0 || (switchWeaponAnimation != null || hideWeaponAnimation != null))
                return;

            int sign = UInput.GetAxisRaw(INC.MOUSE_WHEEL) > 0 ? 1 : -1;
            int weaponIndex = 0;
            int weaponGroup = 0;

            if (currentWeaponKey != KeyCode.None && cacheKeyToID.ContainsKey(currentWeaponKey))
            {
                string currentGroup = cacheKeyToID[currentWeaponKey].GetGroup();
                if (!cacheGroupToSlots.ContainsKey(currentGroup))
                {
                    return;
                }
                List<InventorySlot> slots = cacheGroupToSlots[currentGroup];

                for (int i = 0, length = groups.Count; i < length; i++)
                {
                    InventoryGroup group = groups[i];
                    if (group.GetName() == currentGroup)
                    {
                        weaponGroup = i;
                        slots = group.GetInventorySlots();
                        break;
                    }
                }

                if (slots != null)
                {
                    int s_length = slots.Count;
                    for (int i = 0; i < s_length; i++)
                    {
                        InventorySlot slot = slots[i];
                        if (slot.GetKey() == currentWeaponKey)
                        {
                            weaponIndex = i;
                        }
                    }
                    if (sign == 1)
                    {
                        if (weaponIndex - 1 >= 0)
                        {
                            KeyCode acivateKey = slots[weaponIndex - 1].GetKey();
                            if (acivateKey != currentWeaponKey)
                            {
                                ActivateWeapon(acivateKey);
                            }
                            return;
                        }
                        else if (weaponIndex - 1 < 0 && groups.Count == 1)
                        {
                            KeyCode acivateKey = slots[slots.Count - 1].GetKey();
                            if (acivateKey != currentWeaponKey)
                            {
                                ActivateWeapon(acivateKey);
                            }
                            return;
                        }
                        else if (weaponIndex - 1 < 0 && weaponGroup - 1 < 0)
                        {
                            List<InventorySlot> _slots = groups[groups.Count - 1].GetInventorySlots();
                            for (int i = 0, length = _slots.Count; i < length; i++)
                            {
                                InventorySlot slot = _slots[i];
                                if (slot.GetWeapon() != null)
                                {
                                    ActivateWeapon(slot.GetKey());
                                    return;
                                }
                            }
                        }
                        else if (weaponIndex - 1 < 0 && weaponGroup - 1 >= 0)
                        {
                            List<InventorySlot> _slots = groups[weaponGroup - 1].GetInventorySlots();
                            for (int i = 0, length = _slots.Count; i < length; i++)
                            {
                                InventorySlot slot = _slots[i];
                                if (slot.GetWeapon() != null)
                                {
                                    ActivateWeapon(slot.GetKey());
                                    return;
                                }
                            }
                        }
                    }
                    else if (sign == -1)
                    {
                        if (weaponIndex + 1 <= slots.Count - 1)
                        {
                            KeyCode acivateKey = slots[weaponIndex + 1].GetKey();
                            if (acivateKey != currentWeaponKey)
                            {
                                ActivateWeapon(acivateKey);
                            }
                            return;
                        }
                        else if (weaponIndex + 1 > (slots.Count - 1) && groups.Count == 1)
                        {
                            KeyCode acivateKey = slots[0].GetKey();
                            if (acivateKey != currentWeaponKey)
                            {
                                ActivateWeapon(acivateKey);
                            }
                            return;
                        }
                        else if (weaponIndex + 1 > (slots.Count - 1) && weaponGroup + 1 < (groups.Count - 1))
                        {
                            List<InventorySlot> _slots = groups[0].GetInventorySlots();
                            for (int i = 0, length = _slots.Count; i < length; i++)
                            {
                                InventorySlot slot = _slots[i];
                                if (slot.GetWeapon() != null)
                                {
                                    ActivateWeapon(slot.GetKey());
                                    return;
                                }
                            }
                        }
                        else if (weaponIndex + 1 > (slots.Count - 1) && weaponGroup + 1 <= (groups.Count - 1))
                        {
                            List<InventorySlot> _slots = groups[weaponGroup + 1].GetInventorySlots();
                            for (int i = 0, length = _slots.Count; i < length; i++)
                            {
                                InventorySlot slot = _slots[i];
                                if (slot.GetWeapon() != null)
                                {
                                    ActivateWeapon(slot.GetKey());
                                    return;
                                }
                            }
                        }
                    }
                }

            }
            else if (currentWeaponKey == KeyCode.None)
            {
                List<InventorySlot> _slots = groups[0].GetInventorySlots();
                for (int i = 0, _s_length = _slots.Count; i < _s_length; i++)
                {
                    InventorySlot _slot = _slots[i];
                    if (_slot.GetWeapon() != null)
                    {
                        ActivateWeapon(_slot.GetKey());
                        break;
                    }
                }
            }
        }

        protected virtual IEnumerator SwitchWeaponAnimation(KeyCode targetWeaponKey)
        {
            if (currentWeaponKey != KeyCode.None)
            {
                WeaponID currentID = cacheKeyToID[currentWeaponKey];
                Transform currentTransform = cacheIDToTransform[currentID];
                IWeaponAnimator currrentAnimator = null;
                if (currentTransform != null)
                {
                    currrentAnimator = currentTransform.GetComponent<IWeaponAnimator>();
                }
                else
                {
                    switchWeaponAnimation = null;
                    yield break;
                }

                if (currrentAnimator != null)
                {
                    currrentAnimator.PutAway();
                    yield return new WaitForSeconds(currrentAnimator.GetPutAwayTime());
                }

                currentTransform.gameObject.SetActive(false);
                currentWeaponKey = KeyCode.None;
            }

            WeaponID targetID = cacheKeyToID[targetWeaponKey];
            if (targetID == null)
            {
                switchWeaponAnimation = null;
                yield break;
            }
            Transform targetTransform = cacheIDToTransform[targetID];
            IWeaponAnimator targetAnimator = null;
            if (targetTransform != null)
            {
                targetAnimator = targetTransform.GetComponent<IWeaponAnimator>();
            }
            else
            {
                switchWeaponAnimation = null;
                yield break;
            }

            targetTransform.gameObject.SetActive(true);

            if (targetAnimator != null)
                yield return new WaitForSeconds(targetAnimator.GetTakeTime());

            currentWeaponKey = targetWeaponKey;
            switchWeaponAnimation = null;
            yield break;
        }

        protected virtual IEnumerator SwitchWeaponAnimation(WeaponID targetWeaponID)
        {
            if (currentWeaponKey != KeyCode.None)
            {
                WeaponID currentID = cacheKeyToID[currentWeaponKey];
                Transform currentTransform = cacheIDToTransform[currentID];
                IWeaponAnimator currrentAnimator = null;
                if (currentTransform != null)
                {
                    currrentAnimator = currentTransform.GetComponent<IWeaponAnimator>();
                }
                else
                {
                    switchWeaponAnimation = null;
                    yield break;
                }

                if (currrentAnimator != null)
                {
                    currrentAnimator.PutAway();
                    yield return new WaitForSeconds(currrentAnimator.GetPutAwayTime());
                }

                currentTransform.gameObject.SetActive(false);
                currentWeaponKey = KeyCode.None;
            }

            Transform targetTransform = cacheIDToTransform[targetWeaponID];
            IWeaponAnimator targetAnimator = null;
            if (targetTransform != null)
            {
                targetAnimator = targetTransform.GetComponent<IWeaponAnimator>();
            }
            else
            {
                switchWeaponAnimation = null;
                yield break;
            }

            targetTransform.gameObject.SetActive(true);

            if (targetAnimator != null)
                yield return new WaitForSeconds(targetAnimator.GetTakeTime());

            List<InventorySlot> slots = cacheGroupToSlots[targetWeaponID.GetGroup()];
            for (int i = 0, length = slots.Count; i < length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.GetWeapon() == targetWeaponID)
                {
                    currentWeaponKey = slot.GetKey();
                    break;
                }
            }
            switchWeaponAnimation = null;
            yield break;
        }

        protected virtual IEnumerator ReplaceWeaponAnimation(WeaponID targetWeaponID)
        {
            if (currentWeaponKey != KeyCode.None)
            {
                WeaponID currentID = cacheKeyToID[currentWeaponKey];
                Transform currentTransform = cacheIDToTransform[currentID];
                DropProperties dropProperties = currentID.GetDropProperties();
                IWeaponAnimator currrentAnimator = null;
                if (currentTransform != null)
                {
                    currrentAnimator = currentTransform.GetComponent<IWeaponAnimator>();
                }
                else
                {
                    switchWeaponAnimation = null;
                    yield break;
                }

                if (currrentAnimator != null)
                {
                    currrentAnimator.PutAway();
                    yield return new WaitForSeconds(currrentAnimator.GetPutAwayTime());
                }

                if (dropProperties != DropProperties.Empty)
                {
                    Vector3 pos = _FPCamera.position + (_FPCamera.forward * dropProperties.GetDistance());
                    GameObject dropObject = Instantiate(dropProperties.GetDropObject(), pos, Quaternion.Euler(dropProperties.GetRotation()));
                    if (dropObject != null)
                    {
                        Rigidbody rigidbody = dropObject.GetComponent<Rigidbody>();
                        if (rigidbody != null)
                        {
                            rigidbody.AddForce(_FPCamera.forward * dropProperties.GetForce(), ForceMode.Impulse);
                        }
                    }
                }

                currentTransform.gameObject.SetActive(false);
                currentWeaponKey = KeyCode.None;
            }

            Transform targetTransform = cacheIDToTransform[targetWeaponID];
            IWeaponAnimator targetAnimator = null;
            if (targetTransform != null)
            {
                targetAnimator = targetTransform.GetComponent<IWeaponAnimator>();
            }
            else
            {
                switchWeaponAnimation = null;
                yield break;
            }

            targetTransform.gameObject.SetActive(true);

            if (targetAnimator != null)
                yield return new WaitForSeconds(targetAnimator.GetTakeTime());

            List<InventorySlot> slots = cacheGroupToSlots[targetWeaponID.GetGroup()];
            for (int i = 0, length = slots.Count; i < length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.GetWeapon() == targetWeaponID)
                {
                    currentWeaponKey = slot.GetKey();
                    break;
                }
            }
            switchWeaponAnimation = null;
            yield break;
        }

        protected virtual IEnumerator HideWeaponAnimation()
        {
            WeaponID currentID = cacheKeyToID[currentWeaponKey];
            Transform curTransform = cacheIDToTransform[currentID];
            IWeaponAnimator currrentAnimator = null;
            if (curTransform != null)
            {
                currrentAnimator = curTransform.GetComponent<IWeaponAnimator>();
            }
            else
            {
                hideWeaponAnimation = null;
                yield break;
            }

            if (currrentAnimator != null)
            {
                currrentAnimator.PutAway();
                yield return new WaitForSeconds(currrentAnimator.GetPutAwayTime());

            }

            curTransform.gameObject.SetActive(false);
            hideWeaponAnimation = null;
            yield break;
        }

        protected virtual IEnumerator DropWeaponAnimation()
        {
            WeaponID currentID = cacheKeyToID[currentWeaponKey];
            Transform curTransform = cacheIDToTransform[currentID];
            DropProperties dropProperties = currentID.GetDropProperties();
            IWeaponAnimator currrentAnimator = null;
            if (curTransform != null)
                currrentAnimator = curTransform.GetComponent<IWeaponAnimator>();

            if (currrentAnimator != null)
            {
                currrentAnimator.PutAway();
                yield return new WaitForSeconds(currrentAnimator.GetPutAwayTime());
            }

            curTransform.gameObject.SetActive(false);
            Remove(currentID);

            if (dropProperties != DropProperties.Empty)
            {
                Vector3 pos = _FPCamera.position + (_FPCamera.forward * dropProperties.GetDistance());
                GameObject dropObject = Instantiate(dropProperties.GetDropObject(), pos, Quaternion.Euler(dropProperties.GetRotation()));
                if (dropObject != null)
                {
                    Rigidbody rigidbody = dropObject.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddForce(_FPCamera.forward * dropProperties.GetForce(), ForceMode.Impulse);
                    }
                }
            }

            currentWeaponKey = KeyCode.None;
            dropWeaponAnimation = null;
            yield break;
        }

        protected Dictionary<KeyCode, WeaponID> GetKeyToID()
        {
            Dictionary<KeyCode, WeaponID> cacheKeyToID = new Dictionary<KeyCode, WeaponID>();
            for (int i = 0, length = groups.Count; i < length; i++)
            {
                for (int j = 0, s_Length = groups[i].GetInventorySlotsLength(); j < s_Length; j++)
                {
                    InventorySlot slot = groups[i].GetInventorySlot(j);
                    cacheKeyToID.Add(slot.GetKey(), slot.GetWeapon());
                }
            }
            return cacheKeyToID;
        }

        protected Dictionary<WeaponID, Transform> GetIDToTransform()
        {
            Dictionary<WeaponID, Transform> cacheIDToTransform = new Dictionary<WeaponID, Transform>();
            for (int i = 0, length = _FPCamera.childCount; i < length; i++)
            {
                Transform weapon = _FPCamera.GetChild(i);
                if (!weapon.CompareTag(TNC.WEAPON))
                    continue;

                WeaponIdentifier weaponIdentifier = weapon.GetComponent<WeaponIdentifier>();
                if (weaponIdentifier.GetWeapon() == null)
                    continue;

                cacheIDToTransform.Add(weaponIdentifier.GetWeapon(), weapon);
            }
            return cacheIDToTransform;
        }

        protected Dictionary<string, List<InventorySlot>> GetGroupToSlots()
        {
            Dictionary<string, List<InventorySlot>> cacheGroupToSlots = new Dictionary<string, List<InventorySlot>>();
            for (int i = 0, length = groups.Count; i < length; i++)
            {
                InventoryGroup group = groups[i];
                cacheGroupToSlots.Add(group.GetName(), group.GetInventorySlots());
            }
            return cacheGroupToSlots;
        }

        public Transform GetFPCamera()
        {
            return _FPCamera;
        }

        public void SetFPCamera(Transform value)
        {
            _FPCamera = value;
        }

        public List<InventoryGroup> GetGroups()
        {
            return groups;
        }

        protected void SetGroups(List<InventoryGroup> value)
        {
            groups = value;
        }

        public bool AllowIdenticalWeapons()
        {
            return allowIdenticalWeapons;
        }

        public void AllowIdenticalWeapons(bool value)
        {
            allowIdenticalWeapons = value;
        }

        public SwitchWeaponMode GetSwitchMode()
        {
            return switchMode;
        }

        public void SetSwitchMode(SwitchWeaponMode value)
        {
            switchMode = value;
        }

        public Dictionary<string, List<InventorySlot>> GetCacheGroupToSlots()
        {
            return cacheGroupToSlots;
        }

        protected void SetCacheGroupToSlots(Dictionary<string, List<InventorySlot>> value)
        {
            cacheGroupToSlots = value;
        }

        public Dictionary<KeyCode, WeaponID> GetCacheKeyToID()
        {
            return cacheKeyToID;
        }

        protected void SetCacheKeyToID(Dictionary<KeyCode, WeaponID> value)
        {
            cacheKeyToID = value;
        }

        public Dictionary<WeaponID, Transform> GetCacheIDToTransform()
        {
            return cacheIDToTransform;
        }

        protected void SetCacheIDToTransform(Dictionary<WeaponID, Transform> value)
        {
            cacheIDToTransform = value;
        }

        public KeyCode GetCurrentWeaponKey()
        {
            return currentWeaponKey;
        }

        protected void SetCurrentWeaponKey(KeyCode value)
        {
            currentWeaponKey = value;
        }

        public KeyCode GetStartWeaponKey()
        {
            return startWeaponKey;
        }

        public void SetStartWeaponKey(KeyCode value)
        {
            startWeaponKey = value;
        }
    }
}