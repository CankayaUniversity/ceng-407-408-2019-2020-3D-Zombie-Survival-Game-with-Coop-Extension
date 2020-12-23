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
using UnrealFPS.Runtime;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(WeaponMeleeSystem))]
    [CanEditMultipleObjects]
    public class WeaponMeleeSystemEditor : UEditor<WeaponMeleeSystem>
    {
        internal new static class ContentProperties
        {
            public static readonly GUIContent BaseSettings = new GUIContent("Base Settings");
            public static readonly GUIContent NormalAttackProperties = new GUIContent("Normal Attack Properties");
            public static readonly GUIContent SpecialAttackProperties = new GUIContent("Special Attack Properties");

            public static readonly GUIContent Range = new GUIContent("Attack Range", "The maximum range of the impact.");
            public readonly static GUIContent AttackPoint = new GUIContent("Attack Point", "Attack point on weapon, the position from which the bullet will instantiate.");
            public readonly static GUIContent HitTime = new GUIContent("Hit Time", "Moment of impact (in seconds). After this time there will be a hit.");
            public readonly static GUIContent Delay = new GUIContent("Delay", "Delay before the next attack (in sedonds).");
            public readonly static GUIContent Damage = new GUIContent("Damage", "Damage from hit.");
            public readonly static GUIContent Impulse = new GUIContent("Impact Impulse", "Directly affects the strength of impact on some physical objects.");
            public readonly static GUIContent SpecialKey = new GUIContent("Special Key", "Special Key for activate special attack.");
            public readonly static GUIContent DecalProperties = new GUIContent("Decal Properties");
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.Space(5);
            GUILayout.Label(ContentProperties.BaseSettings, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            if (instance.GetAttackPoint() != null)
            {
                instance.SetAttackPoint((Transform)EditorGUILayout.ObjectField(ContentProperties.AttackPoint, instance.GetAttackPoint(), typeof(Transform), true));
            }
            else
            {
                GUILayout.BeginHorizontal();
                instance.SetAttackPoint((Transform)EditorGUILayout.ObjectField(ContentProperties.AttackPoint, instance.GetAttackPoint(), typeof(Transform), true));
                if (SearchButton())
                {
                    Transform attackPoint = UEditorInternal.FindFirePoint(instance.transform.root);
                    if (attackPoint != null)
                    {
                        instance.SetAttackPoint(attackPoint);
                    }
                    else
                    {
                        UDisplayDialogs.Message("Searching", "Attack point not found, try find it manually.");
                    }
                }
                GUILayout.EndHorizontal();
                UEditorHelpBoxMessages.Error("Attack point cannot be empty!", "Add attack point manually or try to use auto search button.");
            }

            instance.SetDecalProperties((DecalProperties)EditorGUILayout.ObjectField(ContentProperties.DecalProperties, instance.GetDecalProperties(), typeof(DecalProperties), true));
            if (instance.GetDecalProperties() == null)
            {
                UEditorHelpBoxMessages.Tip("Decals won't be created.", "For create Decal Properties asset press right mouse button on Project window and select Create > Unreal FPS > Weapon > Decal Properties.");
            }
            GUILayout.Space(10);

            GUILayout.Label(ContentProperties.NormalAttackProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            WeaponMeleeSystem.AttackProperties normalAttackProperties = instance.GetNormalAttackProperties();
            normalAttackProperties.range = EditorGUILayout.FloatField(ContentProperties.Range, normalAttackProperties.range);
            normalAttackProperties.impulse = EditorGUILayout.FloatField(ContentProperties.Impulse, normalAttackProperties.impulse);
            normalAttackProperties.damage = EditorGUILayout.IntField(ContentProperties.Damage, normalAttackProperties.damage);
            normalAttackProperties.hitTime = EditorGUILayout.Slider(ContentProperties.HitTime, normalAttackProperties.hitTime, 0, normalAttackProperties.delay);
            normalAttackProperties.delay = EditorGUILayout.FloatField(ContentProperties.Delay, normalAttackProperties.delay);

            GUILayout.Space(10);

            GUILayout.Label(ContentProperties.SpecialAttackProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            WeaponMeleeSystem.AttackProperties specialAttackProperties = instance.GetSpecialAttackProperties();
            instance.SetSpecialAttackKey((KeyCode)EditorGUILayout.EnumPopup(ContentProperties.SpecialKey, instance.GetSpecialAttackKey()));
            specialAttackProperties.range = EditorGUILayout.FloatField(ContentProperties.Range, specialAttackProperties.range);
            specialAttackProperties.impulse = EditorGUILayout.FloatField(ContentProperties.Impulse, specialAttackProperties.impulse);
            specialAttackProperties.damage = EditorGUILayout.IntField(ContentProperties.Damage, specialAttackProperties.damage);
            specialAttackProperties.hitTime = EditorGUILayout.Slider(ContentProperties.HitTime, specialAttackProperties.hitTime, 0, specialAttackProperties.delay);
            specialAttackProperties.delay = EditorGUILayout.FloatField(ContentProperties.Delay, specialAttackProperties.delay);

            instance.SetAttackProperties(normalAttackProperties, specialAttackProperties);
            EndBox();
        }
    }
}