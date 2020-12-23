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
using UnrealFPS.Utility;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(AIReloadSystem))]
    [CanEditMultipleObjects]
    public class AIReloadSystemEditor : UEditor<AIReloadSystem>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent AmmoProperties = new GUIContent("Ammo");
            public readonly static GUIContent ReloadingProperties = new GUIContent("Reload");
            public readonly static GUIContent BulletCount = new GUIContent("Bullet Count");
            public readonly static GUIContent MaxBulletCount = new GUIContent("Max Bullet Count");
            public readonly static GUIContent ReloadTime = new GUIContent("Reload Time", "How long time the weapon will reload.");
            public readonly static GUIContent ReloadSound = new GUIContent("Reload Sound", "Sound that will played while AI will reloading.");
        }

        private AnimationClip[] clips;

        public override void InitializeProperties()
        {
            clips = UEditorInternal.GetAllClips(instance.GetComponent<Animator>());
        }

        public override string HeaderName()
        {
            return "AI Reload System";
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.Label(ContentProperties.AmmoProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(5);
            instance.SetBulletCount(EditorGUILayout.IntSlider(ContentProperties.BulletCount, instance.GetBulletCount(), 0, instance.GetMaxBulletCount()));
            instance.SetMaxBulletCount(EditorGUILayout.IntField(ContentProperties.MaxBulletCount, instance.GetMaxBulletCount()));

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.ReloadingProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
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
            }
            else
            {
                instance.SetReloadTime(EditorGUILayout.FloatField(ContentProperties.ReloadTime, instance.GetReloadTime()));
            }
            instance.SetReloadSound((AudioClip) EditorGUILayout.ObjectField(ContentProperties.ReloadSound, instance.GetReloadSound(), typeof(AudioClip), true));
            EndBox();
        }
    }
}