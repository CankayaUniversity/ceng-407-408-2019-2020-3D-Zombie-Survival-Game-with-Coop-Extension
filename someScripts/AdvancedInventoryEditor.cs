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
using UnityEngine;
using UnrealFPS.Runtime;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(AdvancedInventory))]
    public class AdvancedInventoryEditor : UEditor<AdvancedInventory>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent BaseSettings = new GUIContent("Base Settings");
            public readonly static GUIContent Weapons = new GUIContent("Weapons");
            public readonly static GUIContent Switch = new GUIContent("Switch Mode");
            public readonly static GUIContent Camera = new GUIContent("Camera", "First Person Camera");
            public readonly static GUIContent AllowIdenticalWeapons = new GUIContent("Allow Identical Weapons");

            public readonly static GUIContent AddWeaponButton = new GUIContent(" Add Weapon ", "Add new weapon in group.");
            public readonly static GUIContent AddGroupButton = new GUIContent(" Add Group ", "Add new group in inventory.");
            public readonly static GUIContent RemoveGroupButton = new GUIContent(" Remove Group ", "Remove group from inventory.");
            public readonly static GUIContent EditGroupButton = new GUIContent(" Edit ", "Edit inventory groups.");
            public readonly static GUIContent ApplyChangesButton = new GUIContent(" Apply ", "Apply all changes.");
        }

        public override string HeaderName()
        {
            return "First Person Inventory";
        }

        private SerializedProperty groupsProperty;
        private bool weaponsEditFoldout;
        private List<bool> radioToggles;
        private int savedGroupIndex = -1;
        private int savedSlotIndex = -1;
        private bool editingGroups;

        public override void InitializeProperties()
        {
            groupsProperty = serializedObject.FindProperty("groups");
            radioToggles = CreateRadioToggles();
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.Label(ContentProperties.BaseSettings, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);

            if (instance.GetFPCamera() != null)
            {
                instance.SetFPCamera((Transform) EditorGUILayout.ObjectField(ContentProperties.Camera, instance.GetFPCamera(), typeof(Transform), true));
            }
            else
            {
                GUILayout.BeginHorizontal();
                instance.SetFPCamera((Transform) EditorGUILayout.ObjectField(ContentProperties.Camera, instance.GetFPCamera(), typeof(Transform), true));
                if (SearchButton())
                {
                    Camera camera = UEditorInternal.FindFPCamera(instance.transform);
                    if (camera != null)
                    {
                        instance.SetFPCamera(camera.transform);
                    }
                    else
                    {
                        UDisplayDialogs.Message("Searching", "Camera not found, try find it manually.");
                    }
                }
                GUILayout.EndHorizontal();
                UEditorHelpBoxMessages.CameraError();
            }
            instance.SetSwitchMode((SwitchWeaponMode) EditorGUILayout.EnumPopup(ContentProperties.Switch, instance.GetSwitchMode()));
            instance.AllowIdenticalWeapons(EditorGUILayout.Toggle(ContentProperties.AllowIdenticalWeapons, instance.AllowIdenticalWeapons()));
            EndBox();

            BeginBox(3);
            IncreaseIndentLevel();
            weaponsEditFoldout = EditorGUILayout.Foldout(weaponsEditFoldout, ContentProperties.Weapons, true);
            if (weaponsEditFoldout)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                int pos = -1;
                for (int i = 0; i < groupsProperty.arraySize; i++)
                {
                    SerializedProperty inventoryGroup = groupsProperty.GetArrayElementAtIndex(i);

                    GUILayout.Space(5);
                    GUILayout.BeginVertical(GUI.skin.window);
                    Rect groupNameRect = GUILayoutUtility.GetRect(0, 0);
                    groupNameRect.y -= 19;
                    groupNameRect.height = 15;

                    if (editingGroups)
                    {
                        DecreaseIndentLevel();
                        EditorGUI.PropertyField(groupNameRect, inventoryGroup.FindPropertyRelative("name"), new GUIContent("Group Name"));
                        IncreaseIndentLevel();
                    }
                    else
                    {
                        GUI.Label(groupNameRect, inventoryGroup.FindPropertyRelative("name").stringValue, UEditorStyles.SectionHeaderLabel);
                    }

                    SerializedProperty inventorySlots = inventoryGroup.FindPropertyRelative("inventorySlots");
                    if (inventorySlots.arraySize == 0)
                    {
                        UEditorHelpBoxMessages.Message("Group is empty, add weapon!", MessageType.Warning, true);
                    }

                    for (int j = 0; j < inventorySlots.arraySize; j++)
                    {
                        pos++;
                        SerializedProperty slot = inventorySlots.GetArrayElementAtIndex(j);
                        InventorySlot _slot = instance.GetGroups() [i].GetInventorySlot(j);
                        string slotName = _slot.GetWeapon() != null ? _slot.GetWeapon().name : "Empty Slot " + (j + 1);
                        GUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(_slot.GetWeapon() == null || _slot.GetKey() == KeyCode.None);

                        radioToggles[pos] = EditorGUILayout.Toggle(radioToggles[pos], EditorStyles.radioButton, GUILayout.Width(15));
                        if (radioToggles[pos])
                        {
                            int except = !(_slot.GetKey() == KeyCode.None) ? pos : -1;
                            SetToggleValues(except, false);
                            savedGroupIndex = i;
                            savedSlotIndex = j;
                        }
                        EditorGUI.EndDisabledGroup();
                        GUILayout.Label(slotName, GUILayout.Width(70));
                        EditorGUILayout.PropertyField(slot.FindPropertyRelative("key"), GUIContent.none, GUILayout.MaxWidth(110));
                        EditorGUILayout.PropertyField(slot.FindPropertyRelative("weapon"), GUIContent.none);
                        GUILayout.BeginVertical();
                        GUILayout.Space(0.7f);
                        if (GUILayout.Button("", GUI.skin.GetStyle("OL Minus"), GUILayout.Width(20)))
                        {
                            inventorySlots.DeleteArrayElementAtIndex(j);
                            if (radioToggles[pos])
                            {
                                savedGroupIndex = -1;
                                savedSlotIndex = -1;
                            }
                            radioToggles.RemoveAt(j);
                            pos--;
                            break;
                        }
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.Space(3);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(ContentProperties.AddWeaponButton, editingGroups ? EditorStyles.miniButtonLeft : EditorStyles.miniButton, GUILayout.Height(17)))
                    {
                        inventorySlots.arraySize++;
                        radioToggles.Add(false);
                    }
                    if (editingGroups && GUILayout.Button(ContentProperties.RemoveGroupButton, EditorStyles.miniButtonRight, GUILayout.Height(17)))
                    {
                        groupsProperty.DeleteArrayElementAtIndex(i);
                        if (savedGroupIndex == i)
                        {
                            savedGroupIndex = -1;
                            savedSlotIndex = -1;
                        }
                        radioToggles.Clear();
                        if (editingGroups && groupsProperty.arraySize == 0)
                        {
                            editingGroups = false;
                        }
                        radioToggles = CreateRadioToggles();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(2);
                    GUILayout.EndVertical();
                    GUILayout.Space(5);
                }

                if (groupsProperty.arraySize == 0)
                {
                    UEditorHelpBoxMessages.Tip("Inventory is empty!", "Add new group and weapon.", true);
                }

                GUILayout.BeginHorizontal();

                if ((savedGroupIndex != -1 && savedSlotIndex != -1) && GUILayout.Button("Disable Start Weapon", EditorStyles.miniButton, GUILayout.Height(17)))
                {
                    savedGroupIndex = -1;
                    savedSlotIndex = -1;
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(ContentProperties.AddGroupButton, groupsProperty.arraySize > 0 ? EditorStyles.miniButtonLeft : EditorStyles.miniButton, GUILayout.Height(17)))
                {
                    if (groupsProperty != null)
                        instance.GetGroups().Add(new InventoryGroup("New Group " + instance.GetGroups().Count, new List<InventorySlot>()));
                }
                if (editingGroups && GUILayout.Button(ContentProperties.ApplyChangesButton, EditorStyles.miniButtonRight, GUILayout.Height(17)))
                {
                    editingGroups = false;
                }
                else if (!editingGroups && groupsProperty.arraySize > 0 && GUILayout.Button(ContentProperties.EditGroupButton, EditorStyles.miniButtonRight, GUILayout.Height(17)))
                {
                    editingGroups = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                ApplySavedToggle();
            }
            DecreaseIndentLevel();
            EndBox();
        }

        private List<bool> CreateRadioToggles()
        {
            List<bool> toggles = new List<bool>();
            for (int i = 0, length = groupsProperty.arraySize; i < length; i++)
            {
                SerializedProperty inventorySlots = groupsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("inventorySlots");
                for (int j = 0; j < inventorySlots.arraySize; j++)
                {
                    toggles.Add(false);
                }
            }
            return toggles;
        }

        private void ApplySavedToggle()
        {
            if (savedGroupIndex == -1 || savedSlotIndex == -1)
            {
                SetToggleValues(-1, false);
                instance.SetStartWeaponKey(KeyCode.None);
            }
            else
            {
                InventorySlot _slot = instance.GetGroups() [savedGroupIndex].GetInventorySlot(savedSlotIndex);
                instance.SetStartWeaponKey(_slot.GetKey());
            }
        }
        private void SetToggleValues(int except, bool isActive)
        {
            for (int i = 0, length = radioToggles.Count; i < length; i++)
            {
                if (except != i)
                {
                    radioToggles[i] = isActive;
                }
            }
        }
    }
}