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
    [CreateAssetMenu(fileName = "Decal Properties", menuName = UnrealFPSInfo.NAME + "/Weapon/Decal Properties", order = 125)]
    public class DecalProperties : ScriptableObject, IProperties<DecalProperty>
    {
        [SerializeField] private DecalProperty[] properties;

        public DecalProperty GetProperty(int index)
        {
            return properties[index];
        }

        public DecalProperty[] GetProperties()
        {
            return properties;
        }

        public void SetProperties(DecalProperty[] properties)
        {
            this.properties = properties;
        }

        public void SetProperty(int index, DecalProperty property)
        {
            properties[index] = property;
        }

        public int GetLength()
        {
            return properties != null ? properties.Length : 0;
        }
    }
}