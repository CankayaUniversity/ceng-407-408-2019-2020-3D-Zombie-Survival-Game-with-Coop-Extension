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
    [CustomEditor(typeof(WeaponID))]
    [CanEditMultipleObjects]
    public class WeaponIDEditor : UEditor<WeaponID>
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
        private bool dropPropertiesFoldout;

        public override void InitializeProperties()
        {
            Transform player = UEditorInternal.FindPlayer();
            if (player != null)
                inventory = UEditorInternal.FindComponent<AdvancedInventory>(player);
        }

        public override string HeaderName()
        {
            return "Weapon ID";
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.ID, GUILayout.Width(100));
            instance.SetID(EditorGUILayout.TextField(instance.GetID()));
            GUI.SetNextControlName("");
            if (GenerateIDButton())
            {
                string id = System.Guid.NewGuid().ToString().ToUpper();
                id = id.Replace("-", "");
                instance.SetID(id);
                GUI.FocusControl("");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.DisplayName, GUILayout.Width(100));
            instance.SetDisplayName(EditorGUILayout.TextField(instance.GetDisplayName()));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.Description, GUILayout.Width(100));
            instance.SetDescription(EditorGUILayout.TextArea(instance.GetDescription()));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.Group, GUILayout.Width(100));
            instance.SetGroup(EditorGUILayout.TextField(instance.GetGroup()));
            if (inventory != null)
            {
                GUI.SetNextControlName(ContentProperties.Group.text);
                if (ListButton())
                {
                    InventoryGroup[] groups = inventory.GetGroups().ToArray();
                    if (groups != null && groups.Length > 0)
                    {
                        GenericMenu popup = new GenericMenu();
                        for (int i = 0; i < groups.Length; i++)
                        {
                            popup.AddItem(new GUIContent(groups[i].GetName()), false, (x) => { instance.SetGroup(x.ToString()); }, groups[i].GetName());

                        }
                        popup.ShowAsContext();
                    }
                    GUI.FocusControl(ContentProperties.Group.text);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ContentProperties.Image, GUILayout.Width(100));
            instance.SetImage((Sprite)EditorGUILayout.ObjectField(instance.GetImage(), typeof(Sprite), false));
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            IncreaseIndentLevel();
            BeginSubBox();
            dropPropertiesFoldout = EditorGUILayout.Foldout(dropPropertiesFoldout, ContentProperties.DropProperties, true);
            if (dropPropertiesFoldout)
            {
                DropProperties dropProperties = instance.GetDropProperties();
                dropProperties.SetDropObject(ObjectField<GameObject>(ContentProperties.DropObject, dropProperties.GetDropObject(), false));
                dropProperties.SetForce(EditorGUILayout.FloatField(ContentProperties.Force, dropProperties.GetForce()));
                dropProperties.SetSoundEffect(ObjectField<AudioClip>(ContentProperties.SoundEffect, dropProperties.GetSoundEffect(), false));
                dropProperties.SetDistance(EditorGUILayout.FloatField(ContentProperties.Distance, dropProperties.GetDistance()));
                dropProperties.SetRotation(EditorGUILayout.Vector3Field(ContentProperties.Rotation, dropProperties.GetRotation()));
                instance.SetDropProperties(dropProperties);
            }
            EndSubBox();
            DecreaseIndentLevel();
            EndBox();
        }

    }
}