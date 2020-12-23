/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnrealFPS.AI;
using UnrealFPS.Runtime;
using static UnrealFPS.Editor.UProjectManager;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace UnrealFPS.Editor
{
    [UPMItem("Artificial Intelligence", 3, ItemType.General)]
    public class AIUItem
    {
        internal static class ContentProperties
        {
            public readonly static GUIContent Version = new GUIContent("Alpha: 0.017b1", "Version of AI System.");
            public readonly static GUIContent Notification = new GUIContent("The system is currently under development, if we find any bugs or errors, you can email us to help improve the system.");
            public readonly static GUIContent Type = new GUIContent("Type", "Type of AI.");
            public readonly static GUIContent Name = new GUIContent("Name", "Name of AI.");
            public readonly static GUIContent Model = new GUIContent("Model", "Model of AI.");
            public readonly static GUIContent Controller = new GUIContent("Controller", "Animator Controller of AI.");
            public readonly static GUIContent DefaultPreset = new GUIContent("Use Default Preset", "Default preset of AI settings.");
            public readonly static GUIContent Tagrets = new GUIContent("Enemys", "Enemys layers for this AI.");
            public readonly static GUIContent Obstacles = new GUIContent("Obstacles", "Obstacle object layers through which the AI can not detect the enemys.");
        }

        internal class AIProperties
        {
            public enum AIType
            {
                AgainstAll,
                Friendly,
                Enemy
            }

            private const string DEFAULT_NAME = "Write AI name here...";

            private AIType type;
            private string name;
            private GameObject model;
            private AnimatorController controller;
            private LayerMask targets;
            private LayerMask obstacles;
            private bool defaultPreset;

            public AIProperties()
            {
                this.name = DEFAULT_NAME;
                this.defaultPreset = true;
            }

            public AIProperties(AIType type, string name)
            {
                this.type = type;
                this.name = name;
                this.defaultPreset = true;
            }

            public AIType GetAIType()
            {
                return type;
            }

            public void SetAIType(AIType value)
            {
                type = value;
            }

            public string GetName()
            {
                return name;
            }

            public void SetName(string value)
            {
                name = value;
            }

            public GameObject GetModel()
            {
                return model;
            }

            public void SetModel(GameObject value)
            {
                model = value;
            }

            public AnimatorController GetController()
            {
                return controller;
            }

            public void SetController(AnimatorController value)
            {
                controller = value;
            }

            public LayerMask GetTargets()
            {
                return targets;
            }

            public void SetTargets(LayerMask value)
            {
                targets = value;
            }

            public LayerMask GetObstacles()
            {
                return obstacles;
            }

            public void SetObstacles(LayerMask value)
            {
                obstacles = value;
            }

            public bool DefaultPreset()
            {
                return defaultPreset;
            }

            public void DefaultPreset(bool value)
            {
                defaultPreset = value;
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

            public void SetDefaultPreset()
            {
                switch (type)
                {
                    case AIProperties.AIType.AgainstAll:
                        targets = 1 << LayerMask.NameToLayer(LNC.AI) |
                            1 << LayerMask.NameToLayer(LNC.AI_FRIENDLY) |
                            1 << LayerMask.NameToLayer(LNC.AI_ENEMY);
                        obstacles = 1 << LayerMask.NameToLayer(LNC.OBSTACLE);
                        break;
                    case AIProperties.AIType.Friendly:
                        targets = 1 << LayerMask.NameToLayer(LNC.AI) |
                            1 << LayerMask.NameToLayer(LNC.AI_ENEMY);
                        obstacles = 1 << LayerMask.NameToLayer(LNC.OBSTACLE);
                        break;
                    case AIProperties.AIType.Enemy:
                        targets = 1 << LayerMask.NameToLayer(LNC.AI) |
                            1 << LayerMask.NameToLayer(LNC.AI_FRIENDLY);
                        obstacles = 1 << LayerMask.NameToLayer(LNC.OBSTACLE);
                        break;
                }
            }
        }

        private static AIProperties properties;
        private static IEditorDelay delay;
        private static GameObject _AI;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        public static void OnEnable()
        {
            properties = new AIProperties();
            delay = new EditorDelay(0.1f);
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// </summary>
        public static void OnGUI()
        {
            DrawProperties();
            DrawVersion();
        }

        private static void DrawProperties()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label("Base Options", UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(5);
            if (properties != null)
            {
                properties.SetAIType((AIProperties.AIType)EditorGUILayout.EnumPopup(ContentProperties.Type, properties.GetAIType()));
                properties.SetName(EditorGUILayout.DelayedTextField(ContentProperties.Name, properties.GetName()));
                if (properties.GetName() == "")
                {
                    properties.SetDefaultName();
                }
                properties.SetModel((GameObject)EditorGUILayout.ObjectField(ContentProperties.Model, properties.GetModel(), typeof(GameObject), true));
                if (properties.GetModel() == null)
                {
                    UEditorHelpBoxMessages.Error("AI model cannot be empty!", "Add AI model.");
                }
                properties.SetController((AnimatorController)EditorGUILayout.ObjectField(ContentProperties.Controller, properties.GetController(), typeof(AnimatorController), true));
                properties.DefaultPreset(EditorGUILayout.Toggle(ContentProperties.DefaultPreset, properties.DefaultPreset()));

                if (properties.DefaultPreset())
                {
                    properties.SetDefaultPreset();
                }

                EditorGUI.BeginDisabledGroup(properties.DefaultPreset());
                LayerMask targetMask = EditorGUILayout.MaskField(ContentProperties.Tagrets, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(properties.GetTargets()), InternalEditorUtility.layers);
                properties.SetTargets(InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(targetMask));
                LayerMask obstacleMask = EditorGUILayout.MaskField(ContentProperties.Obstacles, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(properties.GetObstacles()), InternalEditorUtility.layers);
                properties.SetObstacles(InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(obstacleMask));
                EditorGUI.EndDisabledGroup();

                GUILayout.Space(5);
                UEditor.HorizontalLine();
                GUILayout.Space(5);
                EditorGUI.BeginDisabledGroup(!properties.GetModel());
                if (UEditor.Button("Create", "Right", GUILayout.Width(70)))
                {
                    _AI = CreateAI(properties);
                }
                if (_AI != null && delay.WaitForSeconds())
                {
                    if (UDisplayDialogs.Message("Create Successful", "AI was created on scene!\nSetup AI components before start play.", "Select", "Ok"))
                    {
                        Selection.activeGameObject = _AI;
                    }
                    _AI = null;
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                UEditorHelpBoxMessages.Error("Properties not loaded...", "Try to reload UProject Manager window.", true);
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private static void DrawVersion()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(ContentProperties.Version, UEditorStyles.MiniGrayLabel);
            GUILayout.Space(3);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();
        }

        private static GameObject CreateAI(AIProperties properties)
        {
            // Instantiate AI object
            GameObject ai = GameObject.Instantiate(properties.GetModel(), Vector3.zero, Quaternion.identity);

            // Set tag and layer
            ai.name = properties.GetName();
            ai.tag = TNC.AI;
            switch (properties.GetAIType())
            {
                case AIProperties.AIType.AgainstAll:
                    ai.layer = LayerMask.NameToLayer(LNC.AI);
                    break;
                case AIProperties.AIType.Friendly:
                    ai.layer = LayerMask.NameToLayer(LNC.AI_FRIENDLY);
                    break;
                case AIProperties.AIType.Enemy:
                    ai.layer = LayerMask.NameToLayer(LNC.AI_ENEMY);
                    break;
            }

            // Set components
            Animator animator = ai.GetComponent<Animator>();
            if (animator == null)
                animator = ai.AddComponent<Animator>();
            if (properties.GetController() != null)
                animator.runtimeAnimatorController = properties.GetController();

            AIController controller = ai.GetComponent<AIController>();
            if (controller == null)
                controller = ai.AddComponent<AIController>();

            AIHealth health = ai.GetComponent<AIHealth>();
            if (health == null)
                health = ai.AddComponent<AIHealth>();

            AIFieldOfView fieldOfView = ai.GetComponent<AIFieldOfView>();
            if (fieldOfView == null)
                fieldOfView = ai.AddComponent<AIFieldOfView>();

            AIAttackSystem attackSystem = ai.GetComponent<AIAttackSystem>();
            if (attackSystem == null)
                attackSystem = ai.AddComponent<AIAttackSystem>();

            AIReloadSystem reloadSystem = ai.GetComponent<AIReloadSystem>();
            if (reloadSystem == null)
                reloadSystem = ai.AddComponent<AIReloadSystem>();

            AIAnimatorHandler animatorHandler = ai.GetComponent<AIAnimatorHandler>();
            if (animatorHandler == null)
                animatorHandler = ai.AddComponent<AIAnimatorHandler>();

            CharacterRagdollSystem ragdollSystem = ai.GetComponent<CharacterRagdollSystem>();
            if (ragdollSystem == null)
                ragdollSystem = ai.AddComponent<CharacterRagdollSystem>();

            NavMeshAgent navMeshAgent = ai.GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
                navMeshAgent = ai.AddComponent<NavMeshAgent>();

            CapsuleCollider capsuleCollider = ai.GetComponent<CapsuleCollider>();
            if (capsuleCollider == null)
                capsuleCollider = ai.AddComponent<CapsuleCollider>();

            AudioSource audioSource = ai.GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = ai.AddComponent<AudioSource>();

            // Set component positions
            UEditorInternal.MoveComponentBottom<Animator>(ai.transform);
            UEditorInternal.MoveComponentBottom<AIController>(ai.transform);
            UEditorInternal.MoveComponentBottom<AIHealth>(ai.transform);
            UEditorInternal.MoveComponentBottom<AIFieldOfView>(ai.transform);
            UEditorInternal.MoveComponentBottom<AIAttackSystem>(ai.transform);
            UEditorInternal.MoveComponentBottom<AIReloadSystem>(ai.transform);
            UEditorInternal.MoveComponentBottom<AIAnimatorHandler>(ai.transform);
            UEditorInternal.MoveComponentBottom<CharacterRagdollSystem>(ai.transform);
            UEditorInternal.MoveComponentBottom<NavMeshAgent>(ai.transform);
            UEditorInternal.MoveComponentBottom<CapsuleCollider>(ai.transform);
            UEditorInternal.MoveComponentBottom<AudioSource>(ai.transform);

            // Set properties settings
            fieldOfView.SetTargetMask(properties.GetTargets());
            fieldOfView.SetObstacleMask(properties.GetObstacles());

            return ai;
        }
    }
}