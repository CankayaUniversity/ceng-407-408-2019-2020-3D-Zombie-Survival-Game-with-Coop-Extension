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
    [CustomEditor(typeof(WeaponAnimationSystem))]
    [CanEditMultipleObjects]
    public class WeaponAnimationSystemEditor : UEditor<WeaponAnimationSystem>
    {
        /// <summary>
        /// GUI content proprties that used in Weapon Animation System editor.
        /// </summary>
        internal new static class ContentProperties
        {
            public readonly static GUIContent MainProperties = new GUIContent("Base Settings");
            public readonly static GUIContent AnimationProperties = new GUIContent("Animation Properties");
            public readonly static GUIContent RotationSway = new GUIContent("Rotation Sway");
            public readonly static GUIContent JumpSway = new GUIContent("Jump Sway");
            public readonly static GUIContent Player = new GUIContent("Player", "First Person Player transform.");
            public readonly static GUIContent OriginalYAxis = new GUIContent("Original Rotation by Y", "Set rotation by Y axis for this weapon.");
            public readonly static GUIContent TakeTime = new GUIContent("Take Time", "How long time weapons will get out of inventory.");
            public readonly static GUIContent PutAwayTime = new GUIContent("Put Away Time", "How long time weapons will put in inventory.");
            public readonly static GUIContent PositionSensitivity = new GUIContent("Position Sensitivity", "Sensitivity to changes in position when moving input mouse or joystick.");
            public readonly static GUIContent MaxPositionSensitivity = new GUIContent("Max Position Sensitivity", "Max sensitivity to changes in position when moving input mouse or joystick.");
            public readonly static GUIContent SmoothPosition = new GUIContent("Smooth Position", "Smoothing value when changing weapon position.");
            public readonly static GUIContent SmoothRotation = new GUIContent("Smooth Rotation", "Smoothing value when changing weapon rotation.");
            public readonly static GUIContent RotationSensitivity = new GUIContent("Rotation Sensitivity", "Sensitivity to changes in rotation when moving input mouse or joystick.");
            public readonly static GUIContent MaxYPosJump = new GUIContent("Max Height", "Max weapon height(position by Y axis), while player falling.");
            public readonly static GUIContent SmoothJump = new GUIContent("Smooth Jump", "Smoothing value when changing weapon position by Y axis while player jumping.");
            public readonly static GUIContent SmoothLand = new GUIContent("Smooth Land", "Smoothing value when changing weapon position by Y axis while player landing.");
            public readonly static GUIContent EventProperties = new GUIContent("Event Properties", "Animation event properties asset.");

        }

        private bool animationFoldout;
        private bool rotationSwayFoldout;
        private bool jumpSwayFoldout;
        private AnimationClip[] clips;

        public override void InitializeProperties()
        {
            clips = UEditorInternal.GetAllClips(instance.GetComponent<Animator>());
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.Label(ContentProperties.MainProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            instance.SetPlayer((Transform)EditorGUILayout.ObjectField(ContentProperties.Player, instance.GetPlayer(), typeof(Transform), true));
            if (instance.GetPlayer() == null)
            {
                if (MiniButton("Try find Camera"))
                {
                    Transform player = UEditorInternal.FindPlayer();
                    if (player != null)
                    {
                        instance.SetPlayer(player);
                    }
                    else
                    {
                        UDisplayDialogs.Message("Searching", "Player not found, try find it manually.");
                    }
                }
                UEditorHelpBoxMessages.PlayerError();
            }
            instance.SetOriginalYAxis(EditorGUILayout.Slider(ContentProperties.OriginalYAxis, instance.GetOriginalYAxis(), -360.0f, 360.0f));
            instance.SetAnimationEventProperties(ObjectField<AnimationEventProperties>(ContentProperties.EventProperties, instance.GetAnimationEventProperties(), true));

            GUILayout.Space(10);
            GUILayout.Label("Animator", UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(5);

            if (clips != null && clips.Length > 0)
            {
                GUILayout.BeginHorizontal();
                instance.SetTakeTime(EditorGUILayout.FloatField(ContentProperties.TakeTime, instance.GetTakeTime()));
                if (ListButton())
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0, length = clips.Length; i < length; i++)
                    {
                        AnimationClip clip = clips[i];
                        menu.AddItem(new GUIContent(clip.name), UMathf.Approximately(clip.length, instance.GetTakeTime(), 0.01f), (x) => { instance.SetTakeTime(UMathf.AllocatePart((float)x)); }, clip.length);
                    }
                    menu.ShowAsContext();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                instance.SetPutAwayTime(EditorGUILayout.FloatField(ContentProperties.PutAwayTime, instance.GetPutAwayTime()));
                if (ListButton())
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0, length = clips.Length; i < length; i++)
                    {
                        AnimationClip clip = clips[i];
                        menu.AddItem(new GUIContent(clip.name), UMathf.Approximately(clip.length, instance.GetPutAwayTime(), 0.01f), (x) => { instance.SetPutAwayTime(UMathf.AllocatePart((float)x)); }, clip.length);
                    }
                    menu.ShowAsContext();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                instance.SetTakeTime(EditorGUILayout.FloatField(ContentProperties.TakeTime, instance.GetTakeTime()));
                instance.SetPutAwayTime(EditorGUILayout.FloatField(ContentProperties.PutAwayTime, instance.GetPutAwayTime()));
            }

            GUILayout.Space(10);
            GUILayout.Label("Vector Animations", UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(5);
            BeginSubBox();
            EditorGUI.BeginDisabledGroup(!instance.UseRotationSway());
            IncreaseIndentLevel();
            rotationSwayFoldout = EditorGUILayout.Foldout(rotationSwayFoldout, ContentProperties.RotationSway, true);
            if (rotationSwayFoldout)
            {
                instance.SetPositionSensitivity(EditorGUILayout.FloatField(ContentProperties.PositionSensitivity, instance.GetPositionSensitivity()));
                instance.SetMaxPositionSensitivity(EditorGUILayout.FloatField(ContentProperties.MaxPositionSensitivity, instance.GetMaxPositionSensitivity()));
                instance.SetSmoothPosition(EditorGUILayout.FloatField(ContentProperties.SmoothPosition, instance.GetSmoothPosition()));
                instance.SetSmoothRotation(EditorGUILayout.FloatField(ContentProperties.SmoothRotation, instance.GetSmoothRotation()));
                instance.SetRotationSensitivity(EditorGUILayout.FloatField(ContentProperties.RotationSensitivity, instance.GetRotationSensitivity()));

            }
            string rotationSwayToggleName = instance.UseRotationSway() ? "Rotation Sway Enabled" : "Rotation Sway Disabled";
            EditorGUI.EndDisabledGroup();
            if (rotationSwayFoldout && !instance.UseRotationSway())
            {
                Rect notificationBackgroungRect = GUILayoutUtility.GetLastRect();
                Rect notificationTextRect = GUILayoutUtility.GetLastRect();

                notificationBackgroungRect.y -= 75;
                notificationBackgroungRect.height = 93.5f;

                notificationTextRect.y -= 58.5f;
                notificationTextRect.height = 60;

                Notification("Rotation Sway Disabled", notificationBackgroungRect, notificationTextRect);
            }
            instance.RotationSwayActive(EditorGUILayout.Toggle(rotationSwayToggleName, instance.UseRotationSway()));
            EndSubBox();

            BeginSubBox();
            EditorGUI.BeginDisabledGroup(!instance.UseJumpSway());
            jumpSwayFoldout = EditorGUILayout.Foldout(jumpSwayFoldout, ContentProperties.JumpSway, true);
            if (jumpSwayFoldout)
            {
                instance.SetMaxYPosJump(EditorGUILayout.FloatField(ContentProperties.MaxYPosJump, instance.GetMaxYPosJump()));
                instance.SetSmoothJump(EditorGUILayout.FloatField(ContentProperties.SmoothJump, instance.GetSmoothJump()));
                instance.SetSmoothLand(EditorGUILayout.FloatField(ContentProperties.SmoothLand, instance.GetSmoothLand()));

            }
            string jumpSwayToggleName = instance.UseJumpSway() ? "Jump Sway Enabled" : "Jump Sway Disabled";
            EditorGUI.EndDisabledGroup();
            if (jumpSwayFoldout && !instance.UseJumpSway())
            {
                Rect notificationBackgroungRect = GUILayoutUtility.GetLastRect();
                Rect notificationTextRect = GUILayoutUtility.GetLastRect();

                notificationBackgroungRect.y -= 39;
                notificationBackgroungRect.height = 58;

                notificationTextRect.y -= 36.5f;
                notificationTextRect.height = 50;

                Notification("Jump Sway Disabled", notificationBackgroungRect, notificationTextRect);
            }
            instance.JumpSwayActive(EditorGUILayout.Toggle(jumpSwayToggleName, instance.UseJumpSway()));
            EndSubBox();
            DecreaseIndentLevel();
            EndBox();

            if (GUI.changed)
            {
                clips = UEditorInternal.GetAllClips(instance.GetComponent<Animator>());
            }
        }
    }
}