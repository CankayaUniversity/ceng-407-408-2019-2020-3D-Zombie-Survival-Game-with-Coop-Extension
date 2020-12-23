/* ================================================================
   ---------------------------------------------------
   Project   :    #0001
   Publisher :    #0002
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
    [CustomEditor(typeof(AIHealth))]
    [CanEditMultipleObjects]
    public class AIHealthEditor : UEditor<AIHealth>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent HealthProperties = new GUIContent("Health Properties");
            public readonly static GUIContent RegenerationSystem = new GUIContent("Regeneration System");
            public readonly static GUIContent RegenerationProperties = new GUIContent("Regeneration Properties");
            public readonly static GUIContent Health = new GUIContent("Health", "Health point value.");
            public readonly static GUIContent MaxHealth = new GUIContent("Max Health", "Max health point value.");
            public readonly static GUIContent MinHealth = new GUIContent("Min Health", "Min health point value.");
            public readonly static GUIContent Rate = new GUIContent("Rate", "Rate (in seconds) of adding health points.\n(V/R - Value per rate).");
            public readonly static GUIContent Value = new GUIContent("Value", "Health point value.");
            public readonly static GUIContent Delay = new GUIContent("Delay", "Delay before start adding health.");
        }

        private bool regenerationFoldout;

        public override string HeaderName()
        {
            return "AI Health";
        }

        public override void BaseGUI()
        {
            CheckHealthPointsValue();
            BeginBox();
            GUILayout.Label(ContentProperties.HealthProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(5);
            instance.SetHealth(EditorGUILayout.IntSlider(ContentProperties.Health, instance.GetHealth(), 0, instance.GetMaxHealth()));
            instance.SetMaxHealth(EditorGUILayout.IntField(ContentProperties.MaxHealth, instance.GetMaxHealth()));
            instance.SetMinHealth(EditorGUILayout.IntField(ContentProperties.MinHealth, instance.GetMinHealth()));

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.RegenerationSystem, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            BeginSubBox();
            EditorGUI.BeginDisabledGroup(!instance.RegenirationActive());
            IncreaseIndentLevel();
            regenerationFoldout = EditorGUILayout.Foldout(regenerationFoldout, ContentProperties.RegenerationProperties, true);
            if (regenerationFoldout)
            {
                HealthRegenirationProperties regenerationSystem = instance.GetRegenerationSystem().GetRegenerationProperties();
                regenerationSystem.SetRate(EditorGUILayout.FloatField(ContentProperties.Rate, regenerationSystem.GetRate()));
                regenerationSystem.SetValue(EditorGUILayout.IntField(ContentProperties.Value, regenerationSystem.GetValue()));
                regenerationSystem.SetDelay(EditorGUILayout.FloatField(ContentProperties.Delay, regenerationSystem.GetDelay()));
                instance.GetRegenerationSystem().SetRegenerationProperties(regenerationSystem);
            }
            string rpToggleName = instance.RegenirationActive() ? "Regeniration Enabled" : "Regeneration Disabled";
            EditorGUI.EndDisabledGroup();
            if (regenerationFoldout && !instance.RegenirationActive())
            {
                Notification("Regeneration Disabled");
            }
            instance.RegenirationActive(EditorGUILayout.Toggle(rpToggleName, instance.RegenirationActive()));
            DecreaseIndentLevel();
            EndSubBox();
            EndBox();
        }

        protected virtual void CheckHealthPointsValue()
        {
            if (instance.GetHealth() < instance.GetMinHealth())
            {
                instance.SetHealth(instance.GetMinHealth());
            }
        }
    }
}