/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnrealFPS.Runtime;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(WeaponShootingSystem))]
    [CanEditMultipleObjects]
    public class WeaponShootingSystemEditor : UEditor<WeaponShootingSystem>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent ShootProperties = new GUIContent("Fire Settings");
            public readonly static GUIContent SoundProperties = new GUIContent("Sounds");
            public readonly static GUIContent FireEffectProperties = new GUIContent("Effects");
            public readonly static GUIContent AdditionalSystem = new GUIContent("Additional System");

            public readonly static GUIContent ShootingMode = new GUIContent("Fire Mode");
            public readonly static GUIContent BulletType = new GUIContent("Bullet Type");
            public readonly static GUIContent ShootPoint = new GUIContent("Fire Point", "Fire point on weapon, the position from which the bullet will instantiate.");
            public readonly static GUIContent PhysicsBullet = new GUIContent("Physics Bullet", "Physics Bullet instance.");
            public readonly static GUIContent RayBullet = new GUIContent("Ray Bullet", "Ray Bullet asset.");
            public readonly static GUIContent Delay = new GUIContent("Delay", "Delay before the next fire (in sedonds).");
            public readonly static GUIContent FixedQueueCount = new GUIContent("Queue Count", "Number of fires in one queue cycle.");
            public readonly static GUIContent ShootImpulse = new GUIContent("Fire Impulse", "For physical bullets directly affect the speed of their departure.\nFor ray bullet directly affects the strength of impact on some physical objects.");
            public readonly static GUIContent ShootRange = new GUIContent("Fire Range", "The maximum range of the bullet.");
            public readonly static GUIContent ShootSound = new GUIContent("Fire Sounds", "Sounds of fire, played when weapon shoot\n(if sounds count > 0 will play random sound from array).");
            public readonly static GUIContent EmptySound = new GUIContent("Empty Sounds", "Sounds of fire with an empty ammo clip, played when weapon shoot\n(if sounds count > 0 will play random sound from array).");
            public readonly static GUIContent MuzzleFlash = new GUIContent("Muzzle Flash", "Weapon muzzle flash effect which will play while weapon shooting.");
            public readonly static GUIContent CartridgeEjection = new GUIContent("Cartridge Ejection", "Weapon cartridge ejection effect which will play while weapon shooting.");
            public readonly static GUIContent WeaponSpreadProperties = new GUIContent("Spread Properties");
            public readonly static GUIContent SpreadProperties = new GUIContent("Properties");
        }

        private SerializedProperty shootSounds;
        private SerializedProperty emptySounds;
        private bool shootSoundsFoldout;
        private bool emptySoundsFoldout;
        private bool weaponSpreadSystemFoldout;

        public override void InitializeProperties()
        {
            shootSounds = serializedObject.FindProperty("soundProperties").FindPropertyRelative("fireSounds");
            emptySounds = serializedObject.FindProperty("soundProperties").FindPropertyRelative("emptySounds");
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.Label(ContentProperties.ShootProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            instance.SetMode((WeaponShootingSystem.Mode) EditorGUILayout.EnumPopup(ContentProperties.ShootingMode, instance.GetMode()));
            instance.SetBulletType((WeaponShootingSystem.BulletType) EditorGUILayout.EnumPopup(ContentProperties.BulletType, instance.GetBulletType()));
            instance.SetFirePoint((Transform) EditorGUILayout.ObjectField(ContentProperties.ShootPoint, instance.GetFirePoint(), typeof(Transform), true));
            switch (instance.GetBulletType())
            {
                case WeaponShootingSystem.BulletType.RayBullet:
                    instance.SetRayBullet((RayBullet) EditorGUILayout.ObjectField(ContentProperties.RayBullet, instance.GetRayBullet(), typeof(RayBullet), false));
                    instance.SetFireRange(EditorGUILayout.FloatField(ContentProperties.ShootRange, instance.GetFireRange()));
                    break;
                case WeaponShootingSystem.BulletType.PhysicsBullet:
                    instance.SetPhysicsBullet((PhysicsBullet) EditorGUILayout.ObjectField(ContentProperties.BulletType, instance.GetPhysicsBullet(), typeof(PhysicsBullet), true));
                    break;
            }
            instance.SetFireImpulse(EditorGUILayout.FloatField(ContentProperties.ShootImpulse, instance.GetFireImpulse()));
            instance.SetDelay(EditorGUILayout.FloatField(ContentProperties.Delay, instance.GetDelay()));

            if (instance.GetMode() == WeaponShootingSystem.Mode.Queue)
            {
                instance.SetFixedQueueCount(EditorGUILayout.IntField(ContentProperties.FixedQueueCount, instance.GetFixedQueueCount()));
            }

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.FireEffectProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            WeaponShootingSystem.FireEffectProperties fireEffectProperties = instance.GetFireEffects();
            if (fireEffectProperties.muzzleFlash != null)
            {
                fireEffectProperties.muzzleFlash = (ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.MuzzleFlash, fireEffectProperties.muzzleFlash, typeof(ParticleSystem), true);
            }
            else
            {
                GUILayout.BeginHorizontal();
                fireEffectProperties.muzzleFlash = (ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.MuzzleFlash, fireEffectProperties.muzzleFlash, typeof(ParticleSystem), true);
                if (SearchButton())
                {
                    ParticleSystem muzzleFlash = UEditorInternal.FindMuzzleFlash(instance.transform);
                    if (muzzleFlash != null)
                    {
                        fireEffectProperties.muzzleFlash = muzzleFlash;
                    }
                    else
                    {
                        UDisplayDialogs.Message("Searching", "Muzzle Flash not found, try find it manually.");
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (fireEffectProperties.cartridgeEjection != null)
            {
                fireEffectProperties.cartridgeEjection = (ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.CartridgeEjection, fireEffectProperties.cartridgeEjection, typeof(ParticleSystem), true);
            }
            else
            {
                GUILayout.BeginHorizontal();
                fireEffectProperties.cartridgeEjection = (ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.CartridgeEjection, fireEffectProperties.cartridgeEjection, typeof(ParticleSystem), true);
                if (SearchButton())
                {
                    ParticleSystem cartridgeEjection = UEditorInternal.FindCartridgeEjection(instance.transform);
                    if (cartridgeEjection != null)
                    {
                        fireEffectProperties.cartridgeEjection = cartridgeEjection;
                    }
                    else
                    {
                        UDisplayDialogs.Message("Searching", "Cartridge Ejection not found, try find it manually.");
                    }
                }
                GUILayout.EndHorizontal();
            }
            instance.SetFireEffects(fireEffectProperties);


            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.SoundProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            BeginSubBox();
            GUILayout.Space(3);
            IncreaseIndentLevel();
            shootSoundsFoldout = EditorGUILayout.Foldout(shootSoundsFoldout, ContentProperties.ShootSound, true);
            if (shootSoundsFoldout)
            {
                if (instance.GetSoundProperties().GetFireSounds() == null || instance.GetSoundProperties().GetFireSoundsCount() == 0)
                {
                    UEditorHelpBoxMessages.Tip("Fire sounds is empty!", "Add new fire sound by click on [Add] button.", true);
                }
                else
                {
                    for (int i = 0; i < instance.GetSoundProperties().GetFireSoundsCount(); i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(3);
                        GUILayout.Label("Clip " + (i + 1), GUILayout.Width(35));
                        instance.GetSoundProperties().SetFireSound(i, (AudioClip) EditorGUILayout.ObjectField(instance.GetSoundProperties().GetFireSound(i), typeof(AudioClip), true));
                        if (GUILayout.Button("", GUI.skin.GetStyle("OL Minus"), GUILayout.Width(20)))
                        {
                            shootSounds.DeleteArrayElementAtIndex(i);
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
                    shootSounds.arraySize++;
                }
                GUILayout.Space(3);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(3);
            EndSubBox();

            BeginSubBox();
            GUILayout.Space(3);
            emptySoundsFoldout = EditorGUILayout.Foldout(emptySoundsFoldout, ContentProperties.EmptySound, true);
            if (emptySoundsFoldout)
            {
                if (instance.GetSoundProperties().GetEmptySounds() == null || instance.GetSoundProperties().GetEmptySoundsCount() == 0)
                {
                    UEditorHelpBoxMessages.Tip("Empty sounds is empty!", "Add new empty sound by click on [Add] button.", true);
                }
                else
                {
                    for (int i = 0; i < instance.GetSoundProperties().GetEmptySoundsCount(); i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(3);
                        GUILayout.Label("Clip " + (i + 1), GUILayout.Width(35));
                        instance.GetSoundProperties().SetEmptySound(i, (AudioClip) EditorGUILayout.ObjectField(instance.GetSoundProperties().GetEmptySound(i), typeof(AudioClip), true));
                        if (GUILayout.Button("", GUI.skin.GetStyle("OL Minus"), GUILayout.Width(20)))
                        {
                            emptySounds.DeleteArrayElementAtIndex(i);
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
                    emptySounds.arraySize++;
                }
                GUILayout.Space(3);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(3);
            EndSubBox();

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.AdditionalSystem, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            BeginSubBox();
            GUILayout.Space(3);
            weaponSpreadSystemFoldout = EditorGUILayout.Foldout(weaponSpreadSystemFoldout, ContentProperties.WeaponSpreadProperties, true);
            if(weaponSpreadSystemFoldout)
            {
                instance.SetSpreadProperties((SpreadProperties)EditorGUILayout.ObjectField(ContentProperties.SpreadProperties, instance.GetSpreadProperties(), typeof(SpreadProperties), true));
                if(instance.GetSpreadProperties() == null)
                {
                    UEditorHelpBoxMessages.Message("Spread properties is empty!\nWeapon won't have a bullet spread and camera shake while shoothing!", MessageType.Warning, true);
                }
            }
            GUILayout.Space(3);
            EndSubBox();
            DecreaseIndentLevel();
            EndBox();
        }

    }
}