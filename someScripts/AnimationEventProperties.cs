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
    [CreateAssetMenu(fileName = "Animation Event Properties", menuName = UnrealFPSInfo.NAME + "/Weapon/Animation Event Properties", order = 127)]
    public class AnimationEventProperties : ScriptableObject, IProperties<AnimationEventProperty>
    {
        [SerializeField] private AnimationEventProperty[] properties;

        public AnimationEventProperty GetProperty(int index)
        {
            return properties[index];
        }

        public AnimationEventProperty[] GetProperties()
        {
            return properties;
        }

        public void SetProperties(AnimationEventProperty[] properties)
        {
            this.properties = properties;
        }

        public void SetProperty(int index, AnimationEventProperty property)
        {
            properties[index] = property;
        }

        public int GetLength()
        {
            return properties != null ? properties.Length : 0;
        }
    }
}