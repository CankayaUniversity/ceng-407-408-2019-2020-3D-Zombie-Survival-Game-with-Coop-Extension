/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System;
using UnityEngine;

namespace UnrealFPS.Runtime
{
    [Serializable]
    public partial struct AnimationEventProperty : IEquatable<AnimationEventProperty>
    {
        [SerializeField] private string animationName;
        [SerializeField] private EventProperty[] eventProperties;

        /// <summary>
        /// Constructor of the reload event property struct.
        /// </summary>
        public AnimationEventProperty(string animationName, EventProperty[] eventProperties)
        {
            this.animationName = animationName;
            this.eventProperties = eventProperties;
        }

        public string GetAnimationName()
        {
            return animationName;
        }

        public void SetAnimationName(string value)
        {
            animationName = value;
        }

        public EventProperty[] GetEventProperties()
        {
            return eventProperties;
        }

        public void SetEventProperties(EventProperty[] value)
        {
            eventProperties = value;
        }

        public EventProperty GetEventProperty(int index)
        {
            return eventProperties[index];
        }

        public void SetEventProperty(int index, EventProperty value)
        {
            eventProperties[index] = value;
        }

        /// <summary>
        /// Empty reload event property.
        /// </summary>
        public readonly static AnimationEventProperty Empty = new AnimationEventProperty("None", null);

        public static bool operator ==(AnimationEventProperty left, AnimationEventProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AnimationEventProperty left, AnimationEventProperty right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            return (obj is AnimationEventProperty metrics) && Equals(metrics);
        }

        public bool Equals(AnimationEventProperty other)
        {
            return (animationName) == (other.animationName);
        }

        public override int GetHashCode()
        {
            return (animationName).GetHashCode();
        }
    }
}