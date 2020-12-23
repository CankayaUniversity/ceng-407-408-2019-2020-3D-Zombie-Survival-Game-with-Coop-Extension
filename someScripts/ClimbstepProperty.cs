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
    /// <summary>
    /// Climb step properties to distinguish between the surface. 
    /// </summary>
    [System.Serializable]
    public struct ClimbstepProperty : IEquatable<ClimbstepProperty>
    {
        [SerializeField] private PhysicMaterial physicMaterial;
        [SerializeField] private AudioClip[] sounds;

        /// <summary>
        /// Climbstep property constructor.
        /// </summary>
        /// <param name="physicMaterial">Climb physic material.</param>
        /// <param name="sounds">Climb step sounds.</param>
        public ClimbstepProperty(PhysicMaterial physicMaterial, AudioClip[] sounds)
        {
            this.physicMaterial = physicMaterial;
            this.sounds = sounds;
        }
        
        /// <summary>
        /// Return climb physic material.
        /// </summary>
        /// <returns></returns>
        public PhysicMaterial GetPhysicMaterial()
        {
            return physicMaterial;
        }

        /// <summary>
        /// Set climb physic material.
        /// </summary>
        /// <param name="physicMaterial">Climb physic material</param>
        public void SetPhysicMaterial(PhysicMaterial physicMaterial)
        {
            this.physicMaterial = physicMaterial;
        }

        /// <summary>
        /// Return climb step sounds.
        /// </summary>
        /// <returns></returns>
        public AudioClip[] GetSounds()
        {
            return sounds;
        }

        /// <summary>
        /// Set range climb step sounds.
        /// </summary>
        /// <param name="sounds">Climb step sounds.</param>
        public void SetSoundRange(AudioClip[] sounds)
        {
            this.sounds = sounds;
        }

        /// <summary>
        /// Return climb step sound.
        /// </summary>
        /// <param name="index">Climb step sound.</param>
        /// <returns></returns>
        public AudioClip GetSound(int index)
        {
            return sounds[index];
        }

        /// <summary>
        /// Set climb step sound.
        /// </summary>
        /// <param name="index">climb step sound index.</param>
        /// <param name="sound">Climb step sound.</param>
        public void SetSound(int index, AudioClip sound)
        {
            sounds[index] = sound;
        }
        
        /// <summary>
        /// Return climb step sounds array length.
        /// </summary>
        /// <returns></returns>
        public int GetSoundsLength()
        {
            return sounds.Length;
        }
        
        /// <summary>
        /// Empty Climbstep property.
        /// </summary>
        /// <value></value>
        public readonly static ClimbstepProperty Empty = new ClimbstepProperty(null, null);

        public static bool operator ==(ClimbstepProperty left, ClimbstepProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClimbstepProperty left, ClimbstepProperty right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            return (obj is ClimbstepProperty metrics) && Equals(metrics);
        }

        public bool Equals(ClimbstepProperty other)
        {
            return (physicMaterial) == (other.physicMaterial);
        }

        public override int GetHashCode()
        {
            return (physicMaterial, sounds).GetHashCode();
        }
    }
}