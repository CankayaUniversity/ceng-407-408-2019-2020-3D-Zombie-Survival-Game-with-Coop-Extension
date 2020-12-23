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
using UnrealFPS.Utility;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(WeaponReloadSystem))]
    [CanEditMultipleObjects]
    public class WeaponReloadSystemEditor : UEditor<WeaponReloadSystem>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent AmmoProperties = new GUIContent("Ammo");
            public readonly static GUIContent ReloadProperties = new GUIContent("Reload");
            public readonly static GUIContent ReloadType = new GUIContent("Reload Type", "Realistic: Removed all remaining bullets clip.\nStandard: Bullets remaining in the clip are saved.");
            public readonly static GUIContent ReloadMode = new GUIContent("Reload Mode", "Default: Suitable for pistols, rifles weapons style.\nSequential: Suitable for shotgun weapons style.");
            public readonly static GUIContent BulletCount = new GUIContent("Bullet Count");
            public readonly static GUIContent ClipCount = new GUIContent("Clip Count");
            public readonly static GUIContent MaxBulletCount = new GUIContent("Max Bullet Count");
            public readonly static GUIContent MaxClipCount = new GUIContent("Max Clip Count");
            public readonly static GUIContent ReloadTime = new GUIContent("Reload Time", "How long time the weapon will reload.");
            public readonly static GUIContent EmptyReloadTime = new GUIContent("Empty Reload Time", "How long the weapon will reload when the clip is empty.");
            public readonly static GUIContent StartTime = new GUIContent("Start Time", "The time (in seconds) of the animation on which the first bullet/clip is inserted.");
            public readonly static GUIContent IterationTime = new GUIContent("Iteration Time", "The time (in seconds), of the animation of the iteration.");
            public readonly static GUIContent ReloadEventProperties = new GUIContent("Event Properties", "Reload event properties asset.");

        }

        private AnimationClip[] clips;

        public override void InitializeProperties()
        {
            clips = UEditorInternal.GetAllClips(instance.GetComponent<Animator>());
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.BeginVertical();
            GUILayout.Label(ContentProperties.AmmoProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(5);
            instance.SetBulletCount(EditorGUILayout.IntSlider(ContentProperties.BulletCount, instance.GetBulletCount(), 0, instance.GetMaxBulletCount()));
            instance.SetClipCount(EditorGUILayout.IntSlider(ContentProperties.ClipCount, instance.GetClipCount(), 0, instance.GetMaxClipCount()));
            instance.SetMaxBulletCount(EditorGUILayout.IntField(ContentProperties.MaxBulletCount, instance.GetMaxBulletCount()));
            instance.SetMaxClipCount(EditorGUILayout.IntField(ContentProperties.MaxClipCount, instance.GetMaxClipCount()));

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.ReloadProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            instance.SetReloadType((WeaponReloadSystem.ReloadType)EditorGUILayout.EnumPopup(ContentProperties.ReloadType, instance.GetReloadType()));
            instance.SetReloadMode((WeaponReloadSystem.ReloadMode)EditorGUILayout.EnumPopup(ContentProperties.ReloadMode, instance.GetReloadMode()));
            switch (instance.GetReloadMode())
            {
                case WeaponReloadSystem.ReloadMode.Default:
                    if (clips != null && clips.Length > 0)
                    {
                        GUILayout.BeginHorizontal();
                        instance.SetReloadTime(EditorGUILayout.FloatField(ContentProperties.ReloadTime, instance.GetReloadTime()));
                        if (ListButton())
                        {
                            GenericMenu menu = new GenericMenu();
                            for (int i = 0, length = clips.Length; i < length; i++)
                            {
                                AnimationClip clip = clips[i];
                                menu.AddItem(new GUIContent(clip.name), UMathf.Approximately(clip.length, instance.GetReloadTime(), 0.01f), (x) => { instance.SetReloadTime(UMathf.AllocatePart((float)x)); }, clip.length);
                            }
                            menu.ShowAsContext();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        instance.SetEmptyReloadTime(EditorGUILayout.FloatField(ContentProperties.EmptyReloadTime, instance.GetEmptyReloadTime()));
                        if (ListButton())
                        {
                            GenericMenu menu = new GenericMenu();
                            for (int i = 0, length = clips.Length; i < length; i++)
                            {
                                AnimationClip clip = clips[i];
                                menu.AddItem(new GUIContent(clip.name), UMathf.Approximately(clip.length, instance.GetEmptyReloadTime(), 0.01f), (x) => { instance.SetEmptyReloadTime(UMathf.AllocatePart((float)x)); }, clip.length);
                            }
                            menu.ShowAsContext();
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        instance.SetReloadTime(EditorGUILayout.FloatField(ContentProperties.ReloadTime, instance.GetReloadTime()));
                        instance.SetEmptyReloadTime(EditorGUILayout.FloatField(ContentProperties.EmptyReloadTime, instance.GetEmptyReloadTime()));
                    }
                    break;
                case WeaponReloadSystem.ReloadMode.Sequential:
                    if (clips != null && clips.Length > 0)
                    {
                        GUILayout.BeginHorizontal();
                        instance.SetStartTime(EditorGUILayout.FloatField(ContentProperties.StartTime, instance.GetStartTime()));
                        if (ListButton())
                        {
                            GenericMenu menu = new GenericMenu();
                            for (int i = 0, length = clips.Length; i < length; i++)
                            {
                                AnimationClip clip = clips[i];
                                menu.AddItem(new GUIContent(clip.name), UMathf.Approximately(clip.length, instance.GetStartTime(), 0.01f), (x) => { instance.SetStartTime(UMathf.AllocatePart((float)x)); }, clip.length);
                            }
                            menu.ShowAsContext();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        instance.SetIterationTime(EditorGUILayout.FloatField(ContentProperties.IterationTime, instance.GetIterationTime()));
                        if (ListButton())
                        {
                            GenericMenu menu = new GenericMenu();
                            for (int i = 0, length = clips.Length; i < length; i++)
                            {
                                AnimationClip clip = clips[i];
                                menu.AddItem(new GUIContent(clip.name), UMathf.Approximately(clip.length, instance.GetIterationTime(), 0.01f), (x) => { instance.SetIterationTime(UMathf.AllocatePart((float)x)); }, clip.length);
                            }
                            menu.ShowAsContext();
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        instance.SetStartTime(EditorGUILayout.FloatField(ContentProperties.StartTime, instance.GetStartTime()));
                        instance.SetIterationTime(EditorGUILayout.FloatField(ContentProperties.IterationTime, instance.GetIterationTime()));
                    }
                    break;
                default:
                    UEditorHelpBoxMessages.Message("Select some one ReloadType...", MessageType.Warning, true);
                    break;
            }
            GUILayout.EndVertical();
            EndBox();

            if (GUI.changed)
            {
                clips = UEditorInternal.GetAllClips(instance.GetComponent<Animator>());
            }
        }

    }
}