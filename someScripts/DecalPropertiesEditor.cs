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
    [CustomEditor(typeof(DecalProperties))]
    [CanEditMultipleObjects]
    public class DecalPropertiesEditor : UEditor<DecalProperties>
    {
        private SerializedProperty properties;
        private List<bool> propertiesFoldout;
        private bool decalFoldout;
        private bool soundFoldout;

        public override void InitializeProperties()
        {
            properties = serializedObject.FindProperty("properties");
            propertiesFoldout = CreatePropertyFoldouts();
        }

        public override void BaseGUI()
        {
            for (int i = 0; i < instance.GetLength(); i++)
            {
                BeginBox();
                IncreaseIndentLevel();
                DecalProperty property = instance.GetProperty(i);
                SerializedProperty serializedProperty = properties.GetArrayElementAtIndex(i);
                string propertyName = "New Property " + (i + 1);
                if (property.GetPhysicMaterial() != null)
                {
                    propertyName = property.GetPhysicMaterial().name;
                }
                else if (property.GetTexture() != null)
                {
                    propertyName = property.GetTexture().name;
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
                    property.SetTexture((Texture2D) EditorGUILayout.ObjectField("Surface Texture", property.GetTexture(), typeof(Texture2D), true));
                    if (property.GetPhysicMaterial() == null && property.GetTexture() == null)
                    {
                        UEditorHelpBoxMessages.Tip("Add Physic Material or Texture2D for handle surface.", "For create Physic Material press right mouse button Create > Physic Material.", true);
                    }

                    GUILayout.Space(10);
                    GUILayout.Label("Decals and Sounds", UEditorStyles.SectionHeaderLabel);
                    GUILayout.Space(5);
                    BeginSubBox();
                    GUILayout.Space(3);
                    decalFoldout = EditorGUILayout.Foldout(decalFoldout, "Decals", true);
                    if (decalFoldout)
                    {
                        if (property.GetDecalsCount() == 0)
                        {
                            UEditorHelpBoxMessages.Tip("Decal instance is empty!", "Add new GetDecal instance by click on [Add] button.", true);
                        }
                        for (int s = 0; s < property.GetDecalsCount(); s++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(3);
                            GUILayout.Label("Decal " + (s + 1), GUILayout.Width(50));
                            property.SetDecal(s, (GameObject) EditorGUILayout.ObjectField(property.GetDecal(s), typeof(GameObject), true));
                            if (GUILayout.Button("", GUI.skin.GetStyle("OL Minus"), GUILayout.Width(20)))
                            {
                                serializedProperty.FindPropertyRelative("decals").DeleteArrayElementAtIndex(s);
                            }
                            GUILayout.Space(3);
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.Space(3);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(" Add "))
                        {
                            serializedProperty.FindPropertyRelative("decals").arraySize++;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(3);
                    EndSubBox();

                    BeginSubBox();
                    GUILayout.Space(3);
                    soundFoldout = EditorGUILayout.Foldout(soundFoldout, "Hit Sounds", true);
                    if (soundFoldout)
                    {
                        if (property.GetSoundsCount() == 0)
                        {
                            UEditorHelpBoxMessages.Tip("Hit sounds is empty!", "Add new hit GetSound() by click on [Add] button.", true);
                        }
                        for (int j = 0; j < property.GetSoundsCount(); j++)
                        {
                            string clipName = property.GetSound(j) != null ? property.GetSound(j).name : "Clip " + (j + 1);
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(3);
                            GUILayout.Label(clipName, GUILayout.Width(35));
                            property.SetSound(j, (AudioClip) EditorGUILayout.ObjectField(property.GetSound(j), typeof(AudioClip), true));
                            if (GUILayout.Button("", GUI.skin.GetStyle("OL Minus"), GUILayout.Width(20)))
                            {
                                serializedProperty.FindPropertyRelative("sounds").DeleteArrayElementAtIndex(j);
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
                if (UDisplayDialogs.Confirmation("Are you really want to remove all properties from this Decal Properties asset?"))
                {
                    properties.ClearArray();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private List<bool> CreatePropertyFoldouts()
        {
            List<bool> foldouts = new List<bool>();

            for (int i = 0, length = instance.GetLength(); i < length; i++)
            {
                foldouts.Add(false);
            }
            return foldouts;
        }
    }
}