/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using UnityEngine;

namespace UnrealFPS.Runtime
{
    public class WeaponIdentifier : MonoBehaviour
    {
        [SerializeField] WeaponID weapon;

        public WeaponID GetWeapon()
        {
            return weapon;
        }

        public void SetWeapon(WeaponID value)
        {
            weapon = value;
        }
    }
}