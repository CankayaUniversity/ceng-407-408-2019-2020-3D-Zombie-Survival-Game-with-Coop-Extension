/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine;
using UnrealFPS.AI;
using UnrealFPS.Runtime;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(CharacterRagdollSystem))]
    [CanEditMultipleObjects]
    public class CharacterRagdollSystemEditor : UEditor<CharacterRagdollSystem>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent FromBelly = new GUIContent("From Belly", "Animation get up from belly");
            public readonly static GUIContent FromBack = new GUIContent("From Back", "Animation get up from back");
            public readonly static GUIContent AirSpeed = new GUIContent("Air Speed", "Determines the max speed of the character while airborne.");
        }

        public override string HeaderName()
        {
            return "Character Ragdoll System";
        }

        public override void BaseGUI()
        {
            BeginBox();
            instance.SetAnimationGetUpFromBelly(EditorGUILayout.TextField(ContentProperties.FromBelly, instance.GetAnimationGetUpFromBelly()));
            instance.SetAnimationGetUpFromBack(EditorGUILayout.TextField(ContentProperties.FromBack, instance.GetAnimationGetUpFromBack()));
            instance.SetAirSpeed(EditorGUILayout.FloatField(ContentProperties.AirSpeed, instance.GetAirSpeed()));
            EndBox();
        }
    }
}