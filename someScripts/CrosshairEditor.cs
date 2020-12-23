/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnrealFPS.Runtime;
using UnrealFPS.UI;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(Crosshair))]
    [CanEditMultipleObjects]
    public class CrosshairEditor : UEditor<Crosshair>
    {
        internal new static class ContentProperties
        {
            public static readonly GUIContent Properties = new GUIContent("Base Settings");
            public static readonly GUIContent Mode = new GUIContent("Mode");
            public static readonly GUIContent AnimationStates = new GUIContent("Animation States");
            public static readonly GUIContent Preset = new GUIContent("Preset");
            public static readonly GUIContent VerticalTexture = new GUIContent("Vertical Texture");
            public static readonly GUIContent HorizontalTexture = new GUIContent("Horizontal Texture");
            public static readonly GUIContent CrosshairColor = new GUIContent("Crosshair Color");
            public static readonly GUIContent Height = new GUIContent("Height");
            public static readonly GUIContent Width = new GUIContent("Width");
            public static readonly GUIContent Spread = new GUIContent("Spread");
            public static readonly GUIContent Angle = new GUIContent("Angle");
            public static readonly GUIContent ShowCrosshair = new GUIContent("Show Crosshair");
            public static readonly GUIContent HideWhileSight = new GUIContent("Hide While Sight", "Hide crosshair while active sight state.");
            public static readonly GUIContent RotationSide = new GUIContent("Rotation Side");
            public static readonly GUIContent RotationSpeed = new GUIContent("Rotation Speed");
            public static readonly GUIContent RotationAnimation = new GUIContent("Rotation Animation");
            public static readonly GUIContent HitEffect = new GUIContent("Hit Effect");
        }

        private bool rotationAnimationFoldout;
        private ReorderableList animationStatesList;
        private bool hitEffectFoldout;

        public override void InitializeProperties()
        {
            SerializedProperty animationStates = serializedObject.FindProperty("animationStates");
            animationStatesList = new ReorderableList(serializedObject, animationStates, true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(new Rect(rect.x + 35, rect.y + 1, 70, EditorGUIUtility.singleLineHeight), "State");
                        EditorGUI.LabelField(new Rect(rect.x + 150, rect.y + 1, 70, EditorGUIUtility.singleLineHeight), "Spread");
                        EditorGUI.LabelField(new Rect(rect.x + 268, rect.y + 1, 70, EditorGUIUtility.singleLineHeight), "Smooth");
                    },

                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        SerializedProperty state = animationStates.GetArrayElementAtIndex(index);
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, 100, EditorGUIUtility.singleLineHeight), state.FindPropertyRelative("state"), GUIContent.none);
                        EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y + 2, 100, EditorGUIUtility.singleLineHeight), state.FindPropertyRelative("spread"), GUIContent.none);
                        EditorGUI.PropertyField(new Rect(rect.x + 240, rect.y + 2, 100, EditorGUIUtility.singleLineHeight), state.FindPropertyRelative("smooth"), GUIContent.none);
                    },

                    onAddCallback = (list) =>
                    {
                        animationStates.arraySize++;
                    }
            };

        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.Label(ContentProperties.Properties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);

            instance.SetMode((Crosshair.Mode) EditorGUILayout.EnumPopup(ContentProperties.Mode, instance.GetMode()));
            instance.SetPreset((Crosshair.Preset) EditorGUILayout.EnumPopup(ContentProperties.Preset, instance.GetPreset()));

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(ContentProperties.VerticalTexture);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            instance.SetVerticalTexture((Texture2D) EditorGUILayout.ObjectField(instance.GetVerticalTexture(), typeof(Texture2D), false, GUILayout.Width(170)));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(ContentProperties.HorizontalTexture);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            instance.SetHorizontalTexture((Texture2D) EditorGUILayout.ObjectField(instance.GetHorizontalTexture(), typeof(Texture2D), false, GUILayout.Width(170)));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (instance.GetVerticalTexture() == null && instance.GetHorizontalTexture() == null && GUILayout.Button(" Generate auto "))
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.white);
                texture.wrapMode = TextureWrapMode.Repeat;
                texture.Apply();
                if (texture != null)
                {
                    instance.SetVerticalTexture(texture);
                    instance.SetHorizontalTexture(texture);
                }
            }

            GUILayout.Space(10);

            instance.SetCrosshairColor(EditorGUILayout.ColorField(ContentProperties.CrosshairColor, instance.GetCrosshairColor()));

            instance.SetHeight(EditorGUILayout.FloatField(ContentProperties.Height, instance.GetHeight()));
            if (instance.GetHeight() < 1)
            {
                instance.SetHeight(1);
            }
            instance.SetWidth(EditorGUILayout.FloatField(ContentProperties.Width, instance.GetWidth()));
            if (instance.GetWidth() < 1)
            {
                instance.SetWidth(1);
            }

            if (instance.GetMode() == Crosshair.Mode.Static)
            {
                instance.SetSpread(EditorGUILayout.FloatField(ContentProperties.Spread, instance.GetSpread()));
            }
            else if (EditorApplication.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.FloatField(ContentProperties.Spread, instance.GetSpread());
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(ContentProperties.Spread, "Processed automatically");
                EditorGUI.EndDisabledGroup();
            }

            if (!instance.GetRotationAnimation() && !EditorApplication.isPlaying)
            {
                instance.SetAngle(EditorGUILayout.Slider(ContentProperties.Angle, instance.GetAngle(), -360, 360));
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Slider(ContentProperties.Angle, instance.GetAngle(), -360, 360);
                EditorGUI.EndDisabledGroup();
            }

            instance.ShowCrosshair(EditorGUILayout.Toggle(ContentProperties.ShowCrosshair, instance.ShowCrosshair()));
            instance.HideWhileSight(EditorGUILayout.Toggle(ContentProperties.HideWhileSight, instance.HideWhileSight()));

            GUILayout.Space(10);

            if (instance.GetMode() == Crosshair.Mode.Dynamic)
            {
                animationStatesList.DoLayoutList();
                bool contains = false;
                HashSet<WeaponActionState> states = new HashSet<WeaponActionState>();
                List<WeaponActionState> dupStates = new List<WeaponActionState>();
                for (int i = 0; i < instance.GetAnimationStates().Length; i++)
                {
                    if (!states.Add(instance.GetAnimationStates() [i].state))
                    {
                        dupStates.Add(instance.GetAnimationStates() [i].state);
                        contains = true;
                    }
                }
                string dupStatesString = string.Join(", ", dupStates);
                if (contains)
                {
                    UEditorHelpBoxMessages.Error("The animation states can not contain the same states!", "Modify or delete duplicate states.\nDuplicate states: " + dupStatesString, true);
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Effects", UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(5);
            BeginSubBox();
            EditorGUI.BeginDisabledGroup(!instance.GetRotationAnimation());
            IncreaseIndentLevel();
            rotationAnimationFoldout = EditorGUILayout.Foldout(rotationAnimationFoldout, "Rotation Animation", true);
            if (rotationAnimationFoldout)
            {
                instance.SetRotationSide((Crosshair.RotationSide) EditorGUILayout.EnumPopup(ContentProperties.RotationSide, instance.GetRotationSide()));
                instance.SetRotationSpeed(EditorGUILayout.FloatField(ContentProperties.RotationSpeed, instance.GetRotationSpeed()));
            }
            DecreaseIndentLevel();
            EditorGUI.EndDisabledGroup();
            if (rotationAnimationFoldout && !instance.GetRotationAnimation())
            {
                Rect notificationBackgroungRect = GUILayoutUtility.GetLastRect();
                Rect notificationTextRect = GUILayoutUtility.GetLastRect();

                notificationBackgroungRect.y -= 20;
                notificationBackgroungRect.height = 38;

                notificationTextRect.y -= 12;
                notificationTextRect.height = 17;

                Notification("Rotation Animation Disabled", notificationBackgroungRect, notificationTextRect);
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            string rotationAnimationToggleName = instance.GetRotationAnimation() ? "Rotation Animation Enabled" : "Rotation Animation Disabled";
            GUILayout.BeginHorizontal();
            GUILayout.Label(rotationAnimationToggleName);
            instance.SetRotationAnimation(EditorGUILayout.Toggle(instance.GetRotationAnimation()));
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EndSubBox();

            BeginSubBox();
            EditorGUI.BeginDisabledGroup(!instance.HitEffectIsActive());
            IncreaseIndentLevel();
            hitEffectFoldout = EditorGUILayout.Foldout(hitEffectFoldout, "Hit Effect", true);
            if (hitEffectFoldout)
            {
                instance.SetHitEffectTexture(ObjectField<Texture2D>("Texture", instance.GetHitEffectTexture(), true));
                if (instance.GetHitEffectTexture() == null && GUILayout.Button(" Generate auto "))
                {
                    Texture2D texture = new Texture2D(1, 1);
                    texture.SetPixel(0, 0, Color.white);
                    texture.wrapMode = TextureWrapMode.Repeat;
                    texture.Apply();
                    if (texture != null)
                    {
                        instance.SetHitEffectTexture(texture);
                    }
                }
                instance.SetHitEffectColor(EditorGUILayout.ColorField("Color", instance.GetHitEffectColor()));
                instance.SetHitEffectHeight(EditorGUILayout.FloatField(ContentProperties.Height, instance.GetHitEffectHeight()));
                instance.SetHitEffectWidth(EditorGUILayout.FloatField(ContentProperties.Width, instance.GetHitEffectWidth()));
                instance.SetHitEffectSpread(EditorGUILayout.FloatField(ContentProperties.Spread, instance.GetHitEffectSpread()));
                instance.SetHitEffecntAngle(EditorGUILayout.Slider(ContentProperties.Angle, instance.GetHitEffecntAngle(), -360, 360));
                instance.SetHitEffectHideSpeed(EditorGUILayout.FloatField("Hide Speed", instance.GetHitEffectHideSpeed()));

                if (MiniButton(" Play "))
                {
                    instance.PlayHitEffect();
                }
            }
            DecreaseIndentLevel();
            EditorGUI.EndDisabledGroup();
            if (hitEffectFoldout && !instance.HitEffectIsActive())
            {
                Rect notificationBackgroungRect = GUILayoutUtility.GetLastRect();
                Rect notificationTextRect = GUILayoutUtility.GetLastRect();

                notificationBackgroungRect.y -= 158;
                notificationBackgroungRect.height = 175;

                notificationTextRect.y -= 75;
                notificationTextRect.height = 17;

                Notification(ContentProperties.HitEffect.text + " Disabled", notificationBackgroungRect, notificationTextRect);
            }
            string hitEffectToggleName = instance.HitEffectIsActive() ? ContentProperties.HitEffect.text + " Enabled" : ContentProperties.HitEffect.text + " Disabled";
            GUILayout.BeginHorizontal();
            GUILayout.Label(hitEffectToggleName);
            instance.HitEffectIsActive(EditorGUILayout.Toggle(instance.HitEffectIsActive()));
            GUILayout.EndHorizontal();
            EndSubBox();
            EndBox();
        }
    }
}