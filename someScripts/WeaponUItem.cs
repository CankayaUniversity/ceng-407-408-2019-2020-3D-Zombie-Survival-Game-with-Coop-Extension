/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnrealFPS.Runtime;
using UnrealFPS.UI;
using static UnrealFPS.Editor.UProjectManager;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace UnrealFPS.Editor
{
    [UPMItem("Weapon", 2, ItemType.General)]
    public sealed class WeaponUItem
    {
        internal static class ContentProperties
        {
            public readonly static GUIContent BaseOptions = new GUIContent("Base Options");
            public readonly static GUIContent Preview = new GUIContent("Preview");
            public readonly static GUIContent PreviewMessage = new GUIContent("No objects to preview...");
            public readonly static GUIContent WeaponType = new GUIContent("Type");
            public readonly static GUIContent WeaponName = new GUIContent("Name");
            public readonly static GUIContent WeaponObject = new GUIContent("Object");
            public readonly static GUIContent WeaponID = new GUIContent("Weapon ID");
            public readonly static GUIContent WeaponAnimator = new GUIContent("Animator Controller");
            public readonly static GUIContent ID = new GUIContent("ID", "Weapon unique ID.");
            public readonly static GUIContent DisplayName = new GUIContent("Display Name", "Weapon name, that dispayed on HUD.");
            public readonly static GUIContent Group = new GUIContent("Group", "Weapon group, require for advanced inventory component.");
            public readonly static GUIContent Image = new GUIContent("Image", "Weapon image, that displayed on HUD.");
            public readonly static GUIContent DropProperties = new GUIContent("Drop Properties", "Weapon drop properties.");
            public readonly static GUIContent DropObject = new GUIContent("Drop Object", "Object that be created when weapon will be dropped.");
            public readonly static GUIContent Force = new GUIContent("Force", "Drop object drop force.");
            public readonly static GUIContent D_SoundEffect = new GUIContent("Sound Effect", "Drop sound effect, will be player when weapon will be droppped.");
            public readonly static GUIContent Distance = new GUIContent("Distance", "Drop object create distance between player.");
            public readonly static GUIContent Rotation = new GUIContent("Rotation", "Drop object rotation, when weapon will be dropped.");

            public readonly static GUIContent ItemProperties = new GUIContent("Item Properties");
            public readonly static GUIContent Effects = new GUIContent("Effects");
            public readonly static GUIContent UI = new GUIContent("UI Properties");

            public readonly static GUIContent Name = new GUIContent("Name", "Pickable item name.");
            public readonly static GUIContent Item = new GUIContent("Item", "Item transform.");
            public readonly static GUIContent PickupType = new GUIContent("Pickup Type", "Pickable system processing type.");
            public readonly static GUIContent ObjecftType = new GUIContent("Item Type", "Pickable item type.");
            public readonly static GUIContent Target = new GUIContent("Target", "How can pick up this object.");
            public readonly static GUIContent PickupKey = new GUIContent("Button", "Button for pickup obejct.");
            public readonly static GUIContent Weapon = new GUIContent("Weapon", "Weapon ID asset.");
            public readonly static GUIContent AutoActivate = new GUIContent("Auto Activate", "Automatically activate new pickup weapon.");
            public readonly static GUIContent IsReusable = new GUIContent("Is Reusable", "This object is reusable.");
            public readonly static GUIContent ReusableDelay = new GUIContent("Reusable Delay", "Reusable delay (in seconds).");
            public readonly static GUIContent DestroyAfterUse = new GUIContent("Destroy After Use", "Destroy object after used.");
            public readonly static GUIContent P_SoundEffect = new GUIContent("Sound Effect", "Sound effect that will player when pickup.");
            public readonly static GUIContent PickUpMessage = new GUIContent("Pick Up Message", "Message that will displayed on HUD, when player pickup new weapon.");
            public readonly static GUIContent ReplaceMessage = new GUIContent("Replace Message", "Message that will displayed on HUD, when player already have active.");
        }

        internal sealed class WeaponProperties
        {
            public enum Type
            {
                Gun,
                Melee,
                Throw
            }

            private const string DEFAULT_NAME = "Write weapon name here...";

            private Type weaponType;
            private string name;
            private GameObject weapon;
            private WeaponID weaponID;
            private RuntimeAnimatorController controller;

            private GameObject item;
            private PickableItem.PickupType pickupType;
            private PickableItem.ItemType objectType;
            private string pickUpmessage;
            private string replaceMessage;

            private int value;
            private bool autoActivate;
            private bool isReusable;
            private float reusableDelay;
            private bool destroyAfterUse;
            private KeyCode pickUpKey;
            private AudioClip soundEffect;

            public WeaponProperties()
            {
                this.name = DEFAULT_NAME;
            }

            public WeaponProperties(Type weaponType, string name, GameObject weaponObject)
            {
                this.weaponType = weaponType;
                this.name = name;
                this.weapon = weaponObject;
            }

            internal Type GetWeaponType()
            {
                return weaponType;
            }

            internal void SetWeaponType(Type value)
            {
                weaponType = value;
            }

            public string GetName()
            {
                return name;
            }

            public void SetName(string value)
            {
                name = value;
            }

            public GameObject GetWeapon()
            {
                return weapon;
            }

            public void SetWeapon(GameObject value)
            {
                weapon = value;
            }

            public WeaponID GetWeaponID()
            {
                return weaponID;
            }

            public void SetWeaponID(WeaponID value)
            {
                weaponID = value;
            }

            public RuntimeAnimatorController GetController()
            {
                return controller;
            }

            public void SetController(RuntimeAnimatorController value)
            {
                controller = value;
            }

            public GameObject GetClearWeapon()
            {
                return item;
            }

            public void SetClearWeapon(GameObject value)
            {
                item = value;
            }

            public bool IsReusable()
            {
                return isReusable;
            }

            public void IsReusable(bool value)
            {
                isReusable = value;
            }

            public float GetReusableDelay()
            {
                return reusableDelay;
            }

            public void SetReusableDelay(float value)
            {
                reusableDelay = value;
            }

            public bool DestroyAfterUse()
            {
                return destroyAfterUse;
            }

            public void DestroyAfterUse(bool value)
            {
                destroyAfterUse = value;
            }

            public PickableItem.ItemType GetObjectType()
            {
                return objectType;
            }

            public void SetObjectType(PickableItem.ItemType value)
            {
                objectType = value;
            }

            public int GetValue()
            {
                return value;
            }

            public void SetValue(int value)
            {
                this.value = value;
            }

            public PickableItem.PickupType GetProcessingType()
            {
                return pickupType;
            }

            public void SetProcessingType(PickableItem.PickupType value)
            {
                pickupType = value;
            }

            public KeyCode GetPickUpKey()
            {
                return pickUpKey;
            }

            public void SetPickUpKey(KeyCode value)
            {
                pickUpKey = value;
            }

            public AudioClip GetSoundEffect()
            {
                return soundEffect;
            }

            public void SetSoundEffect(AudioClip value)
            {
                soundEffect = value;
            }

            public bool AutoActivate()
            {
                return autoActivate;
            }

            public void AutoActivate(bool value)
            {
                autoActivate = value;
            }

            public string GetReplaceMessage()
            {
                return replaceMessage;
            }

            public void SetReplaceMessage(string value)
            {
                replaceMessage = value;
            }

            public string GetPickUpMessage()
            {
                return pickUpmessage;
            }

            public void SetPickUpMessage(string value)
            {
                pickUpmessage = value;
            }

            public bool NameIsValid()
            {
                return name != DEFAULT_NAME &&
                    !string.IsNullOrEmpty(name);
            }

            public void SetDefaultName()
            {
                name = DEFAULT_NAME;
            }
        }

        internal struct AdditionalComponents
        {
            public string name;
            public Type component;
            public bool isActive;
        }

        private static WeaponProperties properties;
        private static GameObject weapon;
        private static IEditorDelay delay;
        private static UnityEditor.Editor previewEditor;
        private static GameObject wasObjectPreview;
        private static bool showPreview;
        private static string[] toolbarItems;
        private static int toolbarIndex;
        private static bool weaponDisplayFoldout;
        private static bool dropPropertiesFoldout;
        private static AdvancedInventory inventory;
        private static ReorderableList additionalComponentsRL;
        private static List<AdditionalComponents> additionalComponents;
        private static bool useSoundEffect;

        /// <summary>
        /// This function is called when the window becomes enabled and active.
        /// </summary>
        public static void OnEnable()
        {
            properties = new WeaponProperties();
            delay = new EditorDelay(0.1f);
            toolbarItems = new string[] { "First Person Weapon", "Pickable Weapon" };
            Transform player = UEditorInternal.FindPlayer();
            if (player != null)
                inventory = UEditorInternal.FindComponent<AdvancedInventory>(player);
            additionalComponents = new List<AdditionalComponents>()
            {
                new AdditionalComponents
                {
                name = "Crosshair",
                component = typeof(Crosshair),
                isActive = true
                },
                new AdditionalComponents
                {
                name = "Camera Zoom",
                component = typeof(FPCameraZoom),
                isActive = true
                }
            };
            additionalComponentsRL = new ReorderableList(additionalComponents, typeof(AdditionalComponents), true, true, false, false)
            {
                drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(new Rect(rect.x, rect.y + 1, 200, EditorGUIUtility.singleLineHeight), "Additional Components");
                    },

                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        AdditionalComponents additionalSystem = additionalComponents[index];
                        EditorGUI.LabelField(new Rect(rect.x, rect.y + 2, 100, EditorGUIUtility.singleLineHeight), additionalSystem.name);
                        additionalSystem.isActive = EditorGUI.Toggle(new Rect(rect.width + 10, rect.y + 2, 30, EditorGUIUtility.singleLineHeight), additionalSystem.isActive);
                        additionalComponents[index] = additionalSystem;
                    }
            };
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// </summary>
        public static void OnGUI()
        {
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarItems);

            GUILayout.Space(10);

            switch (toolbarIndex)
            {
                case 0:
                    DrawFPWeaponGUI();
                    break;
                case 1:
                    DrawPickableWeaponGUI();
                    break;
            }
        }

        private static void DrawFPWeaponGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label(ContentProperties.BaseOptions, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            properties.SetName(EditorGUILayout.TextField(ContentProperties.WeaponName, properties.GetName()));
            properties.SetWeaponType((WeaponProperties.Type) EditorGUILayout.EnumPopup(ContentProperties.WeaponType, properties.GetWeaponType()));
            properties.SetWeapon(UEditor.ObjectField<GameObject>(ContentProperties.WeaponObject, properties.GetWeapon(), true));
            if (properties.GetWeapon() == null)
            {
                UEditorHelpBoxMessages.Error("Weapon model cannot be empty!", "Add weapon model.");
            }

            properties.SetWeaponID(UEditor.ObjectField<WeaponID>(ContentProperties.WeaponID, properties.GetWeaponID(), true));

            properties.SetController(UEditor.ObjectField<RuntimeAnimatorController>(ContentProperties.WeaponAnimator, properties.GetController(), false));
            GUILayout.Space(10);
            additionalComponentsRL.DoLayoutList();

            GUILayout.Space(5);
            UEditor.HorizontalLine();
            GUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(!properties.NameIsValid() || properties.GetWeapon() == null);
            if (UEditor.Button("Create", "Right", GUILayout.Width(70)))
            {
                weapon = CreateFPWeapon(properties);
            }
            EditorGUI.EndDisabledGroup();

            if (weapon != null && delay.WaitForSeconds())
            {
                if (UDisplayDialogs.Message("Create Successful", "Weapon was created on scene!\nSetup weapon components before start play.", "Select", "Ok"))
                {
                    Selection.activeGameObject = weapon;
                }
                weapon = null;
            }

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private static void DrawPickableWeaponGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label(ContentProperties.BaseOptions, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);

            properties.SetName(EditorGUILayout.TextField(ContentProperties.Name, properties.GetName()));
            properties.SetProcessingType((PickableItem.PickupType) EditorGUILayout.EnumPopup(ContentProperties.PickupType, properties.GetProcessingType()));
            properties.SetObjectType((PickableItem.ItemType) EditorGUILayout.EnumPopup(ContentProperties.ObjecftType, properties.GetObjectType()));

            if (properties.GetProcessingType() == PickableItem.PickupType.ByKey)
            {
                properties.SetPickUpKey((KeyCode) EditorGUILayout.EnumPopup(ContentProperties.PickupKey, properties.GetPickUpKey()));
            }

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.ItemProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            properties.SetClearWeapon(UEditor.ObjectField<GameObject>(ContentProperties.Item, properties.GetClearWeapon(), true));
            if (properties.GetClearWeapon() == null)
            {
                UEditorHelpBoxMessages.Error("Item cannot be empty!", "Add item game object!");
            }
            switch (properties.GetObjectType())
            {
                case PickableItem.ItemType.Weapon:
                    properties.SetWeaponID(UEditor.ObjectField<WeaponID>(ContentProperties.Weapon, properties.GetWeaponID(), true));
                    properties.AutoActivate(EditorGUILayout.Toggle(ContentProperties.AutoActivate, properties.AutoActivate()));
                    break;
                case PickableItem.ItemType.Ammo:
                    properties.SetWeaponID(UEditor.ObjectField<WeaponID>(new GUIContent("Target Weapon", "Target weapon for ammo."), properties.GetWeaponID(), true));
                    properties.SetValue(EditorGUILayout.IntField(new GUIContent("Bullets", "How many bullet add in weapon ammo."), properties.GetValue()));
                    break;
                case PickableItem.ItemType.HealthBox:
                    properties.SetValue(EditorGUILayout.IntField(new GUIContent("Health Point", "How many health point add in target."), properties.GetValue()));
                    break;
            }
            if (!properties.DestroyAfterUse())
            {
                float refReusableDelay = properties.GetReusableDelay();
                bool refIsReusable = properties.IsReusable();
                UEditor.HiddenFloatField(ContentProperties.ReusableDelay, ContentProperties.IsReusable, ref refReusableDelay, ref refIsReusable);
                properties.SetReusableDelay(refReusableDelay);
                properties.IsReusable(refIsReusable);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                properties.IsReusable(EditorGUILayout.Toggle(ContentProperties.IsReusable, false));
                EditorGUI.EndDisabledGroup();
            }

            if (!properties.IsReusable())
            {
                properties.DestroyAfterUse(EditorGUILayout.Toggle(ContentProperties.DestroyAfterUse, properties.DestroyAfterUse()));
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                properties.DestroyAfterUse(EditorGUILayout.Toggle(ContentProperties.DestroyAfterUse, false));
                EditorGUI.EndDisabledGroup();
            }

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.UI, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);

            if (properties.GetWeaponID() != null)
            {
                GUILayout.BeginHorizontal();
                properties.SetPickUpMessage(EditorGUILayout.TextField(ContentProperties.PickUpMessage, properties.GetPickUpMessage()));
                if (UEditor.GenerateButton())
                {
                    properties.SetPickUpMessage("HOLD [{0}] TO PICKUP \"{1}\"");
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                properties.SetPickUpMessage(EditorGUILayout.TextField(ContentProperties.PickUpMessage, properties.GetPickUpMessage()));
            }

            if (properties.GetWeapon() != null)
            {
                GUILayout.BeginHorizontal();
                properties.SetReplaceMessage(EditorGUILayout.TextField(ContentProperties.ReplaceMessage, properties.GetReplaceMessage()));
                if (UEditor.GenerateButton())
                {
                    properties.SetPickUpMessage("HOLD [{0}] TO CHANGE \"{1}\"");
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                properties.SetReplaceMessage(EditorGUILayout.TextField(ContentProperties.ReplaceMessage, properties.GetReplaceMessage()));
            }

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.Effects, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            AudioClip se = properties.GetSoundEffect();
            UEditor.ObjectHiddenField<AudioClip>(ContentProperties.P_SoundEffect, ContentProperties.P_SoundEffect, ref se, ref useSoundEffect);
            properties.SetSoundEffect(useSoundEffect ? se : null);

            GUILayout.Space(5);
            UEditor.HorizontalLine();
            GUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(!properties.NameIsValid() || properties.GetClearWeapon() == null);
            if (UEditor.Button("Create", "Right", GUILayout.Width(70)))
            {
                weapon = CreatePickableWeapon(properties);
            }
            EditorGUI.EndDisabledGroup();

            if (weapon != null && delay.WaitForSeconds())
            {
                if (UDisplayDialogs.Message("Create Successful", "Pickable weapon was created on scene!\nSetup weapon components before start play.", "Select", "Ok"))
                {
                    Selection.activeGameObject = weapon;
                }
                weapon = null;
            }

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        public static WeaponID[] FindWeaponIDInstances()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(WeaponID).Name);
            WeaponID[] weaponIDs = new WeaponID[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                weaponIDs[i] = AssetDatabase.LoadAssetAtPath<WeaponID>(path);
            }

            return weaponIDs;
        }

        /// <summary>
        /// Create new weapon by properties.
        /// </summary>
        private static GameObject CreateFPWeapon(WeaponProperties properties)
        {
            // Initialize gameobjects.
            GameObject weapon = GameObject.Instantiate<GameObject>(properties.GetWeapon(), Vector3.zero, Quaternion.identity);
            weapon.name = properties.GetName();
            weapon.tag = TNC.WEAPON;
            weapon.layer =  LayerMask.NameToLayer(LNC.WEAPON);
            for (int i = 0, length = weapon.transform.childCount; i < length; i++)
            {
                weapon.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LNC.WEAPON);
            }

            // Initialize weapon components.
            Animator animator = UEditorInternal.AddComponent<Animator>(weapon);
            WeaponIdentifier weaponIdentifier = UEditorInternal.AddComponent<WeaponIdentifier>(weapon);
            WeaponAnimationSystem weaponAnimationSystem = UEditorInternal.AddComponent<WeaponAnimationSystem>(weapon);
            switch (properties.GetWeaponType())
            {
                case WeaponProperties.Type.Gun:
                    {
                        WeaponShootingSystem weaponShootingSystem = UEditorInternal.AddComponent<WeaponShootingSystem>(weapon);
                        WeaponReloadSystem weaponReloadSystem = UEditorInternal.AddComponent<WeaponReloadSystem>(weapon);
                        break;
                    }
                case WeaponProperties.Type.Melee:
                    {
                        WeaponMeleeSystem weaponMeleeSystem = UEditorInternal.AddComponent<WeaponMeleeSystem>(weapon);
                        break;
                    }
                case WeaponProperties.Type.Throw:
                    {
                        ThrowingWeaponSystem throwingWeaponSystem = UEditorInternal.AddComponent<ThrowingWeaponSystem>(weapon);
                        WeaponReloadSystem weaponReloadSystem = UEditorInternal.AddComponent<WeaponReloadSystem>(weapon);
                        break;
                    }
            }
            AudioSource audioSource = UEditorInternal.AddComponent<AudioSource>(weapon);

            // Setup Animator component.
            if (properties.GetController() != null)
            {
                animator.runtimeAnimatorController = properties.GetController();
            }

            // Setup WeaponID component.
            if (properties.GetWeaponID() != null)
            {
                weaponIdentifier.SetWeapon(properties.GetWeaponID());
            }

            // Apply components position.
            UEditorInternal.MoveComponentBottom<Animator>(weapon.transform);
            UEditorInternal.MoveComponentBottom<WeaponIdentifier>(weapon.transform);
            UEditorInternal.MoveComponentBottom<WeaponAnimationSystem>(weapon.transform);
            switch (properties.GetWeaponType())
            {
                case WeaponProperties.Type.Gun:
                    {
                        UEditorInternal.MoveComponentBottom<WeaponShootingSystem>(weapon.transform);
                        UEditorInternal.MoveComponentBottom<WeaponReloadSystem>(weapon.transform);
                        break;
                    }
                case WeaponProperties.Type.Melee:
                    {
                        UEditorInternal.MoveComponentBottom<WeaponMeleeSystem>(weapon.transform);
                        break;
                    }
                case WeaponProperties.Type.Throw:
                    {
                        UEditorInternal.MoveComponentBottom<ThrowingWeaponSystem>(weapon.transform);
                        UEditorInternal.MoveComponentBottom<WeaponReloadSystem>(weapon.transform);
                        break;
                    }
            }
            for (int i = 0, length = additionalComponents.Count; i < length; i++)
            {
                AdditionalComponents component = additionalComponents[i];
                if (component.isActive)
                {
                    weapon.AddComponent(component.component);
                }
            }
            UEditorInternal.MoveComponentBottom<AudioSource>(weapon.transform);

            return weapon;
        }

        /// <summary>
        /// Create new weapon by properties.
        /// </summary>
        private static GameObject CreatePickableWeapon(WeaponProperties properties)
        {
            // Initialize gameobjects.
            GameObject weponObject = new GameObject(properties.GetName());
            GameObject weapon = GameObject.Instantiate(properties.GetClearWeapon(), Vector3.zero, Quaternion.identity, weponObject.transform);

            // Initialize weapon components.
            PickableItem pickupItem = UEditorInternal.AddComponent<PickableItem>(weponObject);
            Rigidbody physicsRigidbody = UEditorInternal.AddComponent<Rigidbody>(weponObject);
            AudioSource audioSource = UEditorInternal.AddComponent<AudioSource>(weponObject);
            SphereCollider triggerCollider = UEditorInternal.AddComponent<SphereCollider>(weponObject);
            BoxCollider physicsCollider = UEditorInternal.AddComponent<BoxCollider>(weponObject);

            // Setup PickableItem component.
            pickupItem.SetProcessingType(properties.GetProcessingType());
            pickupItem.SetObjectType(properties.GetObjectType());
            pickupItem.SetPickUpKey(properties.GetPickUpKey());
            pickupItem.SetItem(weapon.transform);
            pickupItem.SetWeapon(properties.GetWeaponID());
            pickupItem.SetValue(properties.GetValue());
            pickupItem.SetReusableDelay(properties.GetReusableDelay());
            pickupItem.IsReusable(properties.IsReusable());
            pickupItem.DestroyAfterUse(properties.DestroyAfterUse());
            pickupItem.SetPickUpMessage(properties.GetPickUpMessage());
            pickupItem.SetSoundEffect(properties.GetSoundEffect());

            // Setup SphereCollider component.
            triggerCollider.isTrigger = true;
            triggerCollider.radius = 1.25f;

            // Setup BoxCollider component.
            Renderer renderer = weapon.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                physicsCollider.center = renderer.bounds.center;
                physicsCollider.size = renderer.bounds.size;
            }

            // Apply components position.
            UEditorInternal.MoveComponentBottom<PickableItem>(weponObject.transform);
            UEditorInternal.MoveComponentBottom<Rigidbody>(weponObject.transform);
            UEditorInternal.MoveComponentBottom<SphereCollider>(weponObject.transform);
            UEditorInternal.MoveComponentBottom<BoxCollider>(weponObject.transform);
            UEditorInternal.MoveComponentBottom<AudioSource>(weponObject.transform);

            return weponObject;
        }
    }
}