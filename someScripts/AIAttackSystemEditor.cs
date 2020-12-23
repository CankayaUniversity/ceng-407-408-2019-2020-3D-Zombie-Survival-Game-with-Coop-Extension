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
    [CustomEditor(typeof(AIAttackSystem))]
    [CanEditMultipleObjects]
    public class AIAttackSystemEditor : UEditor<AIAttackSystem>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent ShootProperties = new GUIContent("Fire Settings");
            public readonly static GUIContent SoundProperties = new GUIContent("Sounds");
            public readonly static GUIContent FireEffectProperties = new GUIContent("Effects");

            public readonly static GUIContent ShootPoint = new GUIContent("Fire Point", "Fire point on weapon, the position from which the bullet will instantiate.");
            public readonly static GUIContent RayBullet = new GUIContent("Ray Bullet", "Ray Bullet asset.");
            public readonly static GUIContent Delay = new GUIContent("Delay", "Delay before the next fire (in sedonds).");
            public readonly static GUIContent ShootImpulse = new GUIContent("Fire Impulse", "For physical bullets directly affect the speed of their departure.\nFor ray bullet directly affects the strength of impact on some physical objects.");
            public readonly static GUIContent ShootRange = new GUIContent("Fire Range", "The maximum range of the bullet.");
            public readonly static GUIContent ShootSound = new GUIContent("Fire Sounds", "Sounds of fire, played when weapon shoot\n(if sounds count > 0 will play random sound from array).");
            public readonly static GUIContent MuzzleFlash = new GUIContent("Muzzle Flash", "Weapon muzzle flash effect which will play while weapon shooting.");
            public readonly static GUIContent CartridgeEjection = new GUIContent("Cartridge Ejection", "Weapon cartridge ejection effect which will play while weapon shooting.");
        }

        private SerializedProperty fireSounds;
        private bool shootSoundsFoldout;

        public override void InitializeProperties()
        {
            fireSounds = serializedObject.FindProperty("fireSounds");
        }

        public override string HeaderName()
        {
            return "AI Attack System";
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.Label(ContentProperties.ShootProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            instance.SetFirePoint((Transform) EditorGUILayout.ObjectField(ContentProperties.ShootPoint, instance.GetFirePoint(), typeof(Transform), true));
            instance.SetBullet((RayBullet) EditorGUILayout.ObjectField(ContentProperties.RayBullet, instance.GetBullet(), typeof(RayBullet), false));
            instance.SetRange(EditorGUILayout.FloatField(ContentProperties.ShootRange, instance.GetRange()));
            instance.SetImpulse(EditorGUILayout.FloatField(ContentProperties.ShootImpulse, instance.GetImpulse()));
            instance.SetDelay(EditorGUILayout.FloatField(ContentProperties.Delay, instance.GetDelay()));

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.FireEffectProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            if (instance.GetMuzzleFlash() != null)
            {
                instance.SetMuzzleFlash((ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.MuzzleFlash, instance.GetMuzzleFlash(), typeof(ParticleSystem), true));
            }
            else
            {
                GUILayout.BeginHorizontal();
                instance.SetMuzzleFlash((ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.MuzzleFlash, instance.GetMuzzleFlash(), typeof(ParticleSystem), true));
                if (SearchButton())
                {
                    ParticleSystem muzzleFlash = UEditorInternal.FindMuzzleFlash(instance.transform);
                    if (muzzleFlash != null)
                    {
                        instance.SetMuzzleFlash(muzzleFlash);
                    }
                    else
                    {
                        UDisplayDialogs.Message("Searching", "Muzzle Flash not found, try find it manually.");
                    }
                }
                GUILayout.EndHorizontal();
                UEditorHelpBoxMessages.Tip("Muzzle flash is empty!", "Muzzle flash won't play.");
            }

            if (instance.GetCartridgeEjection() != null)
            {
                instance.SetCartridgeEjection((ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.CartridgeEjection, instance.GetCartridgeEjection(), typeof(ParticleSystem), true));
            }
            else
            {
                GUILayout.BeginHorizontal();
                instance.SetCartridgeEjection((ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.CartridgeEjection, instance.GetCartridgeEjection(), typeof(ParticleSystem), true));
                if (SearchButton())
                {
                    ParticleSystem cartridgeEjection = UEditorInternal.FindCartridgeEjection(instance.transform);
                    if (cartridgeEjection != null)
                    {
                        instance.SetCartridgeEjection(cartridgeEjection);
                    }
                    else
                    {
                        UDisplayDialogs.Message("Searching", "Cartridge Ejection not found, try find it manually.");
                    }
                }
                GUILayout.EndHorizontal();
                UEditorHelpBoxMessages.Tip("Cartridge ejection is empty!", "Cartridge ejection won't play.");
            }

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.SoundProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            IncreaseIndentLevel();
            BeginSubBox();
            GUILayout.Space(3);
            shootSoundsFoldout = EditorGUILayout.Foldout(shootSoundsFoldout, ContentProperties.ShootSound, true);
            if (shootSoundsFoldout)
            {
                if (instance.GetFireSounds() == null || instance.GetFireSounds().Length == 0)
                {
                    UEditorHelpBoxMessages.Tip("Fire sounds is empty!", "Add new fire sound by click on [Add] button.", true);
                }
                else
                {
                    for (int i = 0; i < instance.GetFireSounds().Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(3);
                        GUILayout.Label("Clip " + (i + 1), GUILayout.Width(35));
                        instance.SetFireSound(i, (AudioClip) EditorGUILayout.ObjectField(instance.GetFireSound(i), typeof(AudioClip), true));
                        if (GUILayout.Button("", GUI.skin.GetStyle("OL Minus"), GUILayout.Width(20)))
                        {
                            fireSounds.DeleteArrayElementAtIndex(i);
                        }
                        GUILayout.Space(3);
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.Space(3);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(" Add "))
                {
                    fireSounds.arraySize++;
                }
                GUILayout.Space(3);
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.Space(3);
            EndSubBox();
            DecreaseIndentLevel();
            EndBox();
        }
    }
}