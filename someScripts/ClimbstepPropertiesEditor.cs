/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnrealFPS.Runtime;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(ClimbstepProperties))]
    [CanEditMultipleObjects]
    public class ClimbStepPropertiesEditor : UEditor<ClimbstepProperties>
    {
        private SerializedProperty properties;
        private List<bool> propertiesFoldout;
        private bool climbFoldout;

        public override void InitializeProperties()
        {
            properties = serializedObject.FindProperty("properties");
            propertiesFoldout = new List<bool>();
            FillPropertyFoldouts();
        }

        public override string HeaderName()
        {
            return "Climbstep Properties";
        }

        public override void BaseGUI()
        {
            if (instance == null)
            {
                return;
            }

            for (int i = 0; i < instance.GetLength(); i++)
            {
                BeginBox();
                IncreaseIndentLevel();
                ClimbstepProperty property = instance.GetProperty(i);
                SerializedProperty serializedProperty = properties.GetArrayElementAtIndex(i);
                string propertyName = "New Property " + (i + 1);
                if (property.GetPhysicMaterial() != null)
                {
                    propertyName = property.GetPhysicMaterial().name;
                }

                Rect removeButtonRect = GUILayoutUtility.GetRect(0, 0);
                removeButtonRect.x = removeButtonRect.width + 5;
                removeButtonRect.y += 1;
                removeButtonRect.width = 16.5f;
                removeButtonRect.height = 16.5f;
                if (GUI.Button(removeButtonRect, GUIContent.none, GUI.skin.GetStyle("OL Minus")))
                {
                    properties.DeleteArrayElementAtIndex(i);
                    propertiesFoldout.RemoveAt(i);
                    break;
                }

                propertiesFoldout[i] = EditorGUILayout.Foldout(propertiesFoldout[i], propertyName, true);
                if (propertiesFoldout[i])
                {
                    GUILayout.Space(3);
                    GUILayout.Label("Surface", UEditorStyles.SectionHeaderLabel);
                    GUILayout.Space(5);
                    property.SetPhysicMaterial((PhysicMaterial) EditorGUILayout.ObjectField("Physic Material", property.GetPhysicMaterial(), typeof(PhysicMaterial), true));
                    if (property.GetPhysicMaterial() == null)
                    {
                        UEditorHelpBoxMessages.Tip("Add Physic Material for handle surface.", "For create Physic Material press right mouse button Create > Physic Material.", true);
                    }

                    GUILayout.Space(10);
                    GUILayout.Label("Sounds", UEditorStyles.SectionHeaderLabel);
                    GUILayout.Space(5);
                    BeginSubBox();
                    GUILayout.Space(3);
                    climbFoldout = EditorGUILayout.Foldout(climbFoldout, "Climb ounds", true);
                    if (climbFoldout)
                    {
                        if (property.GetSoundsLength() == 0)
                        {
                            UEditorHelpBoxMessages.Tip("Climb sounds is empty!", "Add new climb sound by click on [Add] button.", true);
                        }
                        for (int s = 0; s < property.GetSoundsLength(); s++)
                        {
                            string clipName = property.GetSound(i) != null ? property.GetSound(i).name : "Clip " + (i + 1);
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(3);
                            GUILayout.Label(clipName, GUILayout.Width(35));
                            property.SetSound(i, (AudioClip) EditorGUILayout.ObjectField(property.GetSound(i), typeof(AudioClip), true));
                            if (GUILayout.Button("", GUI.skin.GetStyle("OL Minus"), GUILayout.Width(20)))
                            {
                                serializedProperty.FindPropertyRelative("sounds").DeleteArrayElementAtIndex(s);
                            }
                            GUILayout.Space(3);
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.Space(3);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(" Add "))
                        {
                            serializedProperty.FindPropertyRelative("sounds").arraySize++;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(3);
                    EndSubBox();

                    GUILayout.Space(3);
                    instance.SetProperty(i, property);
                }
                DecreaseIndentLevel();
                EndBox();
            }

            if (instance.GetLength() == 0)
            {
                UEditorHelpBoxMessages.Tip("Properties is empty!", "Add new properties.");
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(" Add Property ", "ButtonLeft", GUILayout.Width(120)))
            {
                properties.arraySize++;
                propertiesFoldout.Add(false);
            }

            if (GUILayout.Button(" Clear All Properties ", "ButtonRight", GUILayout.Width(120)))
            {
                if (UDisplayDialogs.Confirmation("Are you really want to remove all properties from this Climbstep Properties asset?"))
                {
                    properties.ClearArray();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        protected virtual void FillPropertyFoldouts()
        {
            if (propertiesFoldout == null)
                propertiesFoldout = new List<bool>();

            int propertyFoldoutLenght = instance.GetLength();

            for (int i = 0; i < propertyFoldoutLenght; i++)
            {
                propertiesFoldout.Add(false);
            }
        }
    }
}