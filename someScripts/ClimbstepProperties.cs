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
    [CreateAssetMenu(fileName = "Climbstep Properties", menuName = UnrealFPSInfo.NAME + "/Player/Climbstep Properties", order = 122)]
    public class ClimbstepProperties : ScriptableObject, IProperties<ClimbstepProperty>
    {
        [SerializeField] private ClimbstepProperty[] properties;

        public ClimbstepProperty GetProperty(int index)
        {
            return properties[index];
        }

        public ClimbstepProperty[] GetProperties()
        {
            return properties;
        }

        public void SetProperties(ClimbstepProperty[] properties)
        {
            this.properties = properties;
        }

        public void SetProperty(int index, ClimbstepProperty property)
        {
            properties[index] = property;
        }

        public int GetLength()
        {
            return properties != null ? properties.Length : 0;
        }
    }
}