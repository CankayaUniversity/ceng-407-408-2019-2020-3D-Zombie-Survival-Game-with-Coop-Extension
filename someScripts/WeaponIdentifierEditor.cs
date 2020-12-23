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

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(WeaponIdentifier))]
    [CanEditMultipleObjects]
    public class WeaponIdentifierEditor : UEditor<WeaponIdentifier>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent ID = new GUIContent("ID", "Weapon unique ID.");
            public readonly static GUIContent DisplayName = new GUIContent("Display Name", "Weapon name, that dispayed on HUD.");
            public readonly static GUIContent Description = new GUIContent("Description", "Description of weapon.");
            public readonly static GUIContent Group = new GUIContent("Group", "Weapon group, require for advanced inventory component.");
            public readonly static GUIContent Image = new GUIContent("Image", "Weapon image, that displayed on HUD.");
            public readonly static GUIContent DropProperties = new GUIContent("Drop Properties", "Weapon drop properties.");
            public readonly static GUIContent DropObject = new GUIContent("Drop Object", "Object that be created when weapon will be dropped.");
            public readonly static GUIContent Force = new GUIContent("Force", "Drop object drop force.");
            public readonly static GUIContent SoundEffect = new GUIContent("Sound Effect", "Drop sound effect, will be player when weapon will be droppped.");
            public readonly static GUIContent Distance = new GUIContent("Distance", "Drop object create distance between player.");
            public readonly static GUIContent Rotation = new GUIContent("Rotation", "Drop object rotation, when weapon will be dropped.");
        }

        private AdvancedInventory inventory;
        private bool weaponDisplayFoldout;
        private bool dropPropertiesFoldout;

        public override void InitializeProperties()
        {
            Transform player = UEditorInternal.FindPlayer();
            if (player != null)
                inventory = UEditorInternal.FindComponent<AdvancedInventory>(player);
        }

        public override void BaseGUI()
        {
            BeginBox();
            if (instance.GetWeapon() == null)
            {
                instance.SetWeapon((WeaponID)EditorGUILayout.ObjectField("Weapon ID", instance.GetWeapon(), typeof(WeaponID), false));
                UEditorHelpBoxMessages.Error("Current weapon object not active in Unreal FPS system.", "For create Weapon ID asset press right mouse button on Project window and select Create > Unreal FPS > Weapon > Weapon ID.");
            }
            else
            {
                string weaponName = instance.GetWeapon() != null ? instance.GetWeapon().name : "Weapon";
                GUILayout.BeginHorizontal();
                GUILayout.Space(3);
                GUILayout.Label(weaponName, GUILayout.Width(70));
                GUILayout.Space(EditorGUIUtility.labelWidth - 90);
                instance.SetWeapon((WeaponID)EditorGUILayout.ObjectField(instance.GetWeapon(), typeof(WeaponID), false));
                if (GUILayout.Button("", GUI.skin.GetStyle("OL Minus"), GUILayout.Width(20)))
                {
                    instance.SetWeapon(null);
                }
                GUILayout.Space(3);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.Label(weaponName + " Properties", UEditorStyles.SectionHeaderLabel);
                GUILayout.Space(5);
                BeginSubBox();
                GUILayout.Space(3);
                IncreaseIndentLevel();
                string weaponDisplayFoldoutName = weaponDisplayFoldout ? "Hide " + weaponName + " Properties" : "Edit " + weaponName + " Properties";
                weaponDisplayFoldout = EditorGUILayout.Foldout(weaponDisplayFoldout, weaponDisplayFoldoutName, true);
                if (weaponDisplayFoldout && instance.GetWeapon() != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(17);
                    GUILayout.Label(ContentProperties.ID, GUILayout.Width(100));
                    instance.GetWeapon().SetID(EditorGUILayout.TextField(instance.GetWeapon().GetID()));
                    GUI.SetNextControlName("");
                    if (GenerateIDButton())
                    {
                        string id = System.Guid.NewGuid().ToString().ToUpper();
                        id = id.Replace("-", "");
                        instance.GetWeapon().SetID(id);
                        GUI.FocusControl("");
                    }
                    GUILayout.EndHorizontal();
                    instance.GetWeapon().SetDisplayName(EditorGUILayout.TextField(ContentProperties.DisplayName, instance.GetWeapon().GetDisplayName()));

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(ContentProperties.Description, GUILayout.Width(100));
                    instance.GetWeapon().SetDescription(EditorGUILayout.TextArea(instance.GetWeapon().GetDescription()));
                    GUILayout.EndHorizontal();

                    if (inventory == null)
                    {
                        instance.GetWeapon().SetGroup(EditorGUILayout.TextField(ContentProperties.Group, instance.GetWeapon().GetGroup()));
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        instance.GetWeapon().SetGroup(EditorGUILayout.TextField(ContentProperties.Group, instance.GetWeapon().GetGroup()));
                        GUI.SetNextControlName(ContentProperties.Group.text);
                        if (ListButton())
                        {
                            InventoryGroup[] groups = inventory.GetGroups().ToArray();
                            if (groups != null && groups.Length > 0)
                            {
                                GenericMenu popup = new GenericMenu();
                                for (int i = 0; i < groups.Length; i++)
                                {
                                    popup.AddItem(new GUIContent(groups[i].GetName()), false, (x) => { instance.GetWeapon().SetGroup(x.ToString()); }, groups[i].GetName());

                                }
                                popup.ShowAsContext();
                            }
                            GUI.FocusControl(ContentProperties.Group.text);
                        }
                        GUILayout.EndHorizontal();
                    }
                    instance.GetWeapon().SetImage((Sprite)EditorGUILayout.ObjectField(ContentProperties.Image, instance.GetWeapon().GetImage(), typeof(Sprite), false));

                    IncreaseIndentLevel();
                    BeginSubBox();
                    dropPropertiesFoldout = EditorGUILayout.Foldout(dropPropertiesFoldout, ContentProperties.DropProperties, true);
                    if (dropPropertiesFoldout)
                    {
                        DropProperties dropProperties = instance.GetWeapon().GetDropProperties();
                        dropProperties.SetDropObject(ObjectField<GameObject>(ContentProperties.DropObject, dropProperties.GetDropObject(), false));
                        dropProperties.SetForce(EditorGUILayout.FloatField(ContentProperties.Force, dropProperties.GetForce()));
                        dropProperties.SetSoundEffect(ObjectField<AudioClip>(ContentProperties.SoundEffect, dropProperties.GetSoundEffect(), false));
                        dropProperties.SetDistance(EditorGUILayout.FloatField(ContentProperties.Distance, dropProperties.GetDistance()));
                        dropProperties.SetRotation(EditorGUILayout.Vector3Field(ContentProperties.Rotation, dropProperties.GetRotation()));
                        instance.GetWeapon().SetDropProperties(dropProperties);
                    }
                    EndSubBox();
                    DecreaseIndentLevel();
                }
                GUILayout.Space(3);
                EndSubBox();
                DecreaseIndentLevel();
            }
            EndBox();
        }
    }
}