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
using static UnrealFPS.Editor.UProjectManager;

namespace UnrealFPS.Editor
{
    [UPMItem("Weapon ID", 1, ItemType.Properties)]
    public sealed class WeaponIDItem
    {
        internal static class ContentProperties
        {
            public readonly static GUIContent BaseOptions = new GUIContent("Base Options");
            public readonly static GUIContent DropProperties = new GUIContent("Drop Properties", "Weapon drop properties.");

            public readonly static GUIContent ID = new GUIContent("ID", "Weapon unique ID.");
            public readonly static GUIContent DisplayName = new GUIContent("Display Name", "Weapon name, that dispayed on HUD.");
            public readonly static GUIContent Description = new GUIContent("Description", "Description of weapon.");
            public readonly static GUIContent Group = new GUIContent("Group", "Weapon group, require for advanced inventory component.");
            public readonly static GUIContent Image = new GUIContent("Image", "Weapon image, that displayed on HUD.");
            public readonly static GUIContent DropObject = new GUIContent("Drop Object", "Object that be created when weapon will be dropped.");
            public readonly static GUIContent Force = new GUIContent("Force", "Drop object drop force.");
            public readonly static GUIContent SoundEffect = new GUIContent("Sound Effect", "Drop sound effect, will be player when weapon will be droppped.");
            public readonly static GUIContent Distance = new GUIContent("Distance", "Drop object create distance between player.");
            public readonly static GUIContent Rotation = new GUIContent("Rotation", "Drop object rotation, when weapon will be dropped.");
        }

        private static WeaponID weaponID;
        private static AdvancedInventory inventory;
        private static List<WeaponID> weaponIDs;

        private static string[] toolbarItems;
        private static int toolbarActiveIndex;

        public static void OnEnable()
        {
            weaponID = ScriptableObject.CreateInstance<WeaponID>();
            if (weaponID != null)
            {
                weaponID.SetID(WeaponID.GenerateID());
                weaponID.SetDisplayName("Write weapon name here...");
            }
            Transform player = UEditorInternal.FindPlayer();
            if (player != null)
                inventory = UEditorInternal.FindComponent<AdvancedInventory>(player);

            weaponIDs = UEditorInternal.FindAssetsByType<WeaponID>();
            toolbarItems = new string[2] { "New", "List" };
        }

        public static void OnGUI()
        {
            toolbarActiveIndex = GUILayout.Toolbar(toolbarActiveIndex, toolbarItems);
            GUILayout.Space(5);
            switch (toolbarActiveIndex)
            {
                case 0:
                    NewWeaponGUI();
                    break;
                case 1:
                    WeaponIDsListGUI();
                    break;
                default:
                    NewWeaponGUI();
                    break;
            }
        }

        private static void NewWeaponGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label(ContentProperties.BaseOptions, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.ID, GUILayout.Width(100));
            weaponID.SetID(EditorGUILayout.TextField(weaponID.GetID()));
            GUI.SetNextControlName("");
            if (UEditor.GenerateIDButton())
            {
                string id = System.Guid.NewGuid().ToString().ToUpper();
                id = id.Replace("-", "");
                weaponID.SetID(id);
                GUI.FocusControl("");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.DisplayName, GUILayout.Width(100));
            weaponID.SetDisplayName(EditorGUILayout.TextField(weaponID.GetDisplayName()));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.Description, GUILayout.Width(100));
            weaponID.SetDescription(EditorGUILayout.TextArea(weaponID.GetDescription()));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.Group, GUILayout.Width(100));
            weaponID.SetGroup(EditorGUILayout.TextField(weaponID.GetGroup()));
            if (inventory != null)
            {
                GUI.SetNextControlName(ContentProperties.Group.text);
                if (UEditor.ListButton())
                {
                    InventoryGroup[] groups = inventory.GetGroups().ToArray();
                    if (groups != null && groups.Length > 0)
                    {
                        GenericMenu popup = new GenericMenu();
                        for (int i = 0; i < groups.Length; i++)
                        {
                            popup.AddItem(new GUIContent(groups[i].GetName()), false, (x) => { weaponID.SetGroup(x.ToString()); }, groups[i].GetName());

                        }
                        popup.ShowAsContext();
                    }
                    GUI.FocusControl(ContentProperties.Group.text);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.Image, GUILayout.Width(100));
            weaponID.SetImage((Sprite)EditorGUILayout.ObjectField(weaponID.GetImage(), typeof(Sprite), false));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.DropProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            DropProperties dropProperties = weaponID.GetDropProperties();
            dropProperties.SetDropObject(UEditor.ObjectField<GameObject>(ContentProperties.DropObject, dropProperties.GetDropObject(), false));
            dropProperties.SetForce(EditorGUILayout.FloatField(ContentProperties.Force, dropProperties.GetForce()));
            dropProperties.SetSoundEffect(UEditor.ObjectField<AudioClip>(ContentProperties.SoundEffect, dropProperties.GetSoundEffect(), false));
            dropProperties.SetDistance(EditorGUILayout.FloatField(ContentProperties.Distance, dropProperties.GetDistance()));
            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.Rotation, GUILayout.Width(145));
            dropProperties.SetRotation(EditorGUILayout.Vector3Field(GUIContent.none, dropProperties.GetRotation()));
            GUILayout.EndHorizontal();

            weaponID.SetDropProperties(dropProperties);

            GUILayout.Space(5);
            UEditor.HorizontalLine();
            GUILayout.Space(5);

            if (UEditor.Button("Create", "Right", GUILayout.Width(70)))
            {
                string path = EditorUtility.SaveFilePanelInProject("Create new Property", weaponID.GetDisplayName(), "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    string name = System.IO.Path.GetFileName(path);
                    path = path.Replace(name, "");
                    ScriptableObjectUtility.CreateAsset(weaponID, path, name);
                    OnEnable();
                }
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private static void WeaponIDsListGUI()
        {
            if (weaponIDs != null && weaponIDs.Count > 0)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(3);

                for (int i = 0; i < weaponIDs.Count; i++)
                {
                    if (i >= weaponIDs.Count)
                    {
                        break;
                    }

                    Object _weaponID = weaponIDs[i];
                    if (_weaponID == null)
                    {
                        OnEnable();
                        continue;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(3);
                    GUILayout.Label(_weaponID.name, EditorStyles.boldLabel);

                    GUI.color = new Color32(70, 150, 255, 255);
                    if (GUILayout.Button("Open", "ButtonLeft", GUILayout.Width(70)))
                    {
                        Selection.activeObject = _weaponID;
                    }

#if UNITY_EDITOR_OSX
                    EditorGUI.BeginDisabledGroup(true);
#endif

                    GUI.color = new Color32(70, 220, 70, 255);
                    if (GUILayout.Button("Duplicate", "ButtonMid", GUILayout.Width(70)))
                    {
                        string path = AssetDatabase.GetAssetPath(_weaponID);
                        string name = _weaponID.name;
                        path = path.Replace(_weaponID.name + ".asset", "");
                        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(_weaponID), path + name + ".asset");
                        AssetDatabase.Refresh();
                        OnEnable();
                    }

#if UNITY_EDITOR_OSX
                    EditorGUI.EndDisabledGroup();
#endif

                    GUI.color = new Color32(255, 55, 55, 255);
                    if (GUILayout.Button("Delete", "ButtonRight", GUILayout.Width(70)))
                    {
                        if (UDisplayDialogs.Confirmation("Are you really want to delete \"" + _weaponID.name + "\" asset?"))
                        {
                            string path = AssetDatabase.GetAssetPath(_weaponID);
                            AssetDatabase.DeleteAsset(path);
                            AssetDatabase.Refresh();
                            OnEnable();
                            break;
                        }
                    }
                    GUI.color = Color.white;
                    GUILayout.Space(3);
                    GUILayout.EndHorizontal();

                    if (weaponIDs.Count > 1 && i < weaponIDs.Count - 1)
                    {
                        GUILayout.Space(3);
                        UEditor.HorizontalLine();
                        GUILayout.Space(3);
                    }
                    else
                    {
                        GUILayout.Space(3);
                    }
                }
                GUILayout.Space(3);
                GUILayout.EndVertical();
            }
            else
            {
                UEditorHelpBoxMessages.Message("WeaponID's not found...\n" + "Create new property or reload properties", MessageType.Warning);
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(" Refresh Assets ", GUILayout.Width(105.5f)))
            {
                weaponIDs = UEditorInternal.FindAssetsByType<WeaponID>();
            }
            GUILayout.Space(8);
            GUILayout.EndHorizontal();
        }
    }
}