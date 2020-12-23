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
using UnrealFPS.AI;
using UnrealFPS.Runtime;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(AIController))]
    [CanEditMultipleObjects]
    public class AIControllerEditor : UEditor<AIController>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent MovementProperties = new GUIContent("Movement Properies");
            public readonly static GUIContent TransitionsProperties = new GUIContent("Transitions Properties");
            public readonly static GUIContent FollowProperties = new GUIContent("Follow Properties");
            public readonly static GUIContent PatrolProperties = new GUIContent("Patrol Properties");
        }

        private bool movementPropertiesFoldout;
        private bool transitionsPropertiesFoldout;
        private bool followPropertiesFoldout;
        private bool patrolPropertiesFoldout;
        private bool canCrouchFoldout;
        private ReorderableList partrolPoints;

        public override void InitializeProperties()
        {
            SerializedProperty partrolPointsSP = serializedObject.FindProperty("patrolSystem").FindPropertyRelative("points");
            partrolPoints = new ReorderableList(serializedObject, partrolPointsSP, true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(new Rect(rect.x - 10, rect.y + 1, rect.width, rect.height), "Patrol points");
                    },

                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        SerializedProperty point = partrolPointsSP.GetArrayElementAtIndex(index);
                        EditorGUI.LabelField(new Rect(rect.x - 10, rect.y + 1.5f, 70, EditorGUIUtility.singleLineHeight), "Point: " + (index + 1));
                        EditorGUI.PropertyField(new Rect(rect.x + 50, rect.y + 1.5f, rect.width - 55, EditorGUIUtility.singleLineHeight), point, GUIContent.none);
                    }
            };
        }

        public override string HeaderName()
        {
            return "AI Controller";
        }

        public override void BaseGUI()
        {
            BeginBox();
            IncreaseIndentLevel();
            movementPropertiesFoldout = EditorGUILayout.Foldout(movementPropertiesFoldout, ContentProperties.MovementProperties, true);
            if (movementPropertiesFoldout)
            {
                GUILayout.Label("Speed", UEditorStyles.SectionHeaderLabel);
                GUILayout.Space(5);
                instance.SetWalkSpeed(EditorGUILayout.FloatField("Walk Speed", instance.GetWalkSpeed()));
                instance.SetRunSpeed(EditorGUILayout.FloatField("Run Speed", instance.GetRunSpeed()));
                instance.SetSprintSpeed(EditorGUILayout.FloatField("Sprint Speed", instance.GetSprintSpeed()));
                BeginSubBox();
                EditorGUI.BeginDisabledGroup(!instance.CanCrouch());
                canCrouchFoldout = EditorGUILayout.Foldout(canCrouchFoldout, "Crouch System", true);
                if (canCrouchFoldout)
                {
                    instance.SetCrouchSpeed(EditorGUILayout.FloatField("Crouch Speed", instance.GetCrouchSpeed()));
                    instance.SetCrouchStepLength(EditorGUILayout.FloatField("Crouch Lenghten", instance.GetCrouchStepLength()));
                }
                string fovSystemToggleName = instance.CanCrouch() ? "Crouch Enabled" : "Crouch Disabled";
                EditorGUI.EndDisabledGroup();
                if (canCrouchFoldout && !instance.CanCrouch())
                {
                    Rect notificationBackgroungRect = GUILayoutUtility.GetLastRect();
                    Rect notificationTextRect = GUILayoutUtility.GetLastRect();

                    notificationBackgroungRect.y -= 20.5f;
                    notificationBackgroungRect.height = 38;

                    notificationTextRect.y -= 12.5f;
                    notificationTextRect.height = 20;

                    Notification("Crouch Disabled", notificationBackgroungRect, notificationTextRect);
                }
                instance.CanCrouch(EditorGUILayout.Toggle(fovSystemToggleName, instance.CanCrouch()));
                EndSubBox();

                GUILayout.Space(10);
                GUILayout.Label("Step Properties", UEditorStyles.SectionHeaderLabel);
                GUILayout.Space(5);
                instance.SetStepInterval(EditorGUILayout.FloatField("Step Interval", instance.GetStepInterval()));
                instance.SetWalkStepLength(EditorGUILayout.FloatField("Walk Lenghten", instance.GetWalkStepLength()));
                instance.SetRunStepLength(EditorGUILayout.FloatField("Run Lenghten", instance.GetRunStepLength()));
                instance.SetSprintStepLength(EditorGUILayout.FloatField("Sprint Lenghten", instance.GetSprintStepLength()));

                GUILayout.Space(10);
                GUILayout.Label("Footstep", UEditorStyles.SectionHeaderLabel);
                GUILayout.Space(5);
                instance.SetFootstepProperties((FootstepProperties) EditorGUILayout.ObjectField("Properties", instance.GetFootstepProperties(), typeof(FootstepProperties), true));
                if (instance.GetFootstepProperties() == null)
                {
                    UEditorHelpBoxMessages.Tip("Footstep sounds will not be played.", "For create Footstep Properties asset press right mouse button on Project window and select Create > Unreal FPS > Player > Footstep Properties.", true);
                }

                GUILayout.Space(10);
                GUILayout.Label("Other", UEditorStyles.SectionHeaderLabel);
                GUILayout.Space(5);
                instance.SetRotateSpeed(EditorGUILayout.FloatField("Rotate Speed", instance.GetRotateSpeed()));
                GUILayout.Space(10);
            }
            DecreaseIndentLevel();
            EndBox();

            BeginBox();
            IncreaseIndentLevel();
            transitionsPropertiesFoldout = EditorGUILayout.Foldout(transitionsPropertiesFoldout, ContentProperties.TransitionsProperties, true);
            if (transitionsPropertiesFoldout)
            {
                GUILayout.Label("Follow", UEditorStyles.SectionHeaderLabel);
                GUILayout.Space(5);
                instance.SetStoppingFollowDistance(EditorGUILayout.FloatField("Stopping Distance", instance.GetStoppingFollowDistance()));
                instance.SetFollowWalkDistance(EditorGUILayout.FloatField("Walk Distance", instance.GetFollowWalkDistance()));
                instance.SetFollowRunDistance(EditorGUILayout.FloatField("Run Distance", instance.GetFollowRunDistance()));
                instance.SetFollowSprintDistance(EditorGUILayout.FloatField("Sprint Distance", instance.GetFollowSprintDistance()));

                GUILayout.Space(10);
                GUILayout.Label("Attack", UEditorStyles.SectionHeaderLabel);
                float min = instance.GetMinShoothingDistance();
                float max = instance.GetMaxShoothingDistance();
                GUILayout.BeginHorizontal();
                GUILayout.Space(17);
                GUILayout.Label("Distance Min/Max");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                min = EditorGUILayout.FloatField(min, GUILayout.Width(55));
                EditorGUILayout.MinMaxSlider(ref min, ref max, 0.0f, 100.0f);
                max = EditorGUILayout.FloatField(max, GUILayout.Width(55));
                GUILayout.EndHorizontal();
                instance.SetMinShoothingDistance(min);
                instance.SetMaxShoothingDistance(max);
                instance.SetChangePositionDelay(EditorGUILayout.FloatField("Change position delay", instance.GetChangePositionDelay()));
            }
            DecreaseIndentLevel();
            EndBox();

            BeginBox();
            IncreaseIndentLevel();
            followPropertiesFoldout = EditorGUILayout.Foldout(followPropertiesFoldout, ContentProperties.FollowProperties, true);
            if (followPropertiesFoldout)
            {
                GUILayout.Label("Base Settings", UEditorStyles.SectionHeaderLabel);
                GUILayout.Space(7);
                instance.SetTarget((Transform) EditorGUILayout.ObjectField("Target", instance.GetTarget(), typeof(Transform), true));
            }
            DecreaseIndentLevel();
            EndBox();

            BeginBox();
            IncreaseIndentLevel();
            patrolPropertiesFoldout = EditorGUILayout.Foldout(patrolPropertiesFoldout, ContentProperties.PatrolProperties, true);
            if (patrolPropertiesFoldout)
            {
                GUILayout.Label("Base Settings", UEditorStyles.SectionHeaderLabel);
                GUILayout.Space(7);
                instance.GetPatrolSystem().SetPatrolType((PatrolSystem.PatrolType) EditorGUILayout.EnumPopup("Type", instance.GetPatrolSystem().GetPatrolType()));
                instance.GetPatrolSystem().SetUpdatePointDistance(EditorGUILayout.FloatField("Update Point Distance", instance.GetPatrolSystem().GetUpdatePointDistance()));
                partrolPoints.DoLayoutList();
            }
            DecreaseIndentLevel();
            EndBox();
        }
    }
}