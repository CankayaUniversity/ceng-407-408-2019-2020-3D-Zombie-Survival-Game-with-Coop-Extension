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
using UnityEditor.Animations;
using UnityEngine;
using UnrealFPS.Runtime;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(AnimationEventProperties))]
    public class AnimationEventPropertiesEditor : UEditor<AnimationEventProperties>
    {
        internal new static class ContentProperties
        {
            public readonly static GUIContent Animator = new GUIContent("Animator", "Add animator for easy access to animation clips.");
            public readonly static GUIContent AnimationName = new GUIContent("Name", "Animation clip name.");
            public readonly static GUIContent AnimationTime = new GUIContent("Time", "Time to play event.");
            public readonly static GUIContent SoundEffect = new GUIContent("Sound Effect", "Sound effect that will be played with event started.");
            public readonly static GUIContent ApplyShake = new GUIContent("Apply Shake", "Apply shake effect for this animation event.");
            public readonly static GUIContent CameraShake = new GUIContent("Camera Shake Effect", "Camera shake effect that will be played with event started.");
        }

        private SerializedProperty properties;
        private List<bool> propertiesFoldout;
        private Dictionary<int, List<bool>> eventsFoldout;
        private RuntimeAnimatorController animatorController;
        private AnimationClip[] clips;

        public override void InitializeProperties()
        {
            properties = serializedObject.FindProperty("properties");
            propertiesFoldout = CreatePropertiesFoldout();
            eventsFoldout = CreateEventsFoldouts();
        }

        public override void BaseGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            animatorController = ObjectField<RuntimeAnimatorController>(ContentProperties.Animator, animatorController, true);
            if (animatorController != null)
            {
                clips = animatorController.animationClips;
            }
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            IncreaseIndentLevel();
            for (int i = 0, length = instance.GetLength(); i < length; i++)
            {
                BeginBox();
                AnimationEventProperty animationEventProperty = instance.GetProperty(i);
                bool foldout = propertiesFoldout[i];
                string name = animationEventProperty.GetAnimationName().AddSpaces();
                AnimationClip clip = null;
                if (animatorController != null && clips != null)
                {
                    for (int c = 0, c_length = clips.Length; c < c_length; c++)
                    {
                        AnimationClip _clip = clips[c];
                        if (_clip.name == animationEventProperty.GetAnimationName())
                        {
                            clip = _clip;
                            break;
                        }
                    }
                }

                Rect animationRemoveButtonRect = GUILayoutUtility.GetRect(0, 0);
                animationRemoveButtonRect.x = animationRemoveButtonRect.width + 5;
                animationRemoveButtonRect.y += 1;
                animationRemoveButtonRect.width = 16.5f;
                animationRemoveButtonRect.height = 16.5f;
                if (GUI.Button(animationRemoveButtonRect, GUIContent.none, GUI.skin.GetStyle("OL Minus")))
                {
                    properties.DeleteArrayElementAtIndex(i);
                    propertiesFoldout.RemoveAt(i);
                    eventsFoldout.Remove(i);
                    break;
                }

                foldout = EditorGUILayout.Foldout(foldout, name, true);
                if (foldout)
                {
                    GUILayout.Space(3);
                    if (animatorController != null && clips != null)
                    {
                        GUILayout.BeginHorizontal();
                        animationEventProperty.SetAnimationName(EditorGUILayout.TextField(ContentProperties.AnimationName, animationEventProperty.GetAnimationName()));
                        if (ListButton())
                        {
                            GenericMenu menu = new GenericMenu();
                            for (int c = 0, c_length = clips.Length; c < c_length; c++)
                            {
                                AnimationClip _clip = clips[c];
                                string sendMessage = i + "," + _clip.name;
                                menu.AddItem(new GUIContent(_clip.name), _clip.name == animationEventProperty.GetAnimationName(), (_object) =>
                                {
                                    string message = (string)_object as string;
                                    string[] parseMessage = message.Split(',');
                                    int index = int.Parse(parseMessage[0]);
                                    AnimationEventProperty _animationEventProperty = instance.GetProperty(index);
                                    _animationEventProperty.SetAnimationName(parseMessage[1]);
                                    instance.SetProperty(index, _animationEventProperty);
                                }, sendMessage);
                            }
                            menu.ShowAsContext();

                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        animationEventProperty.SetAnimationName(EditorGUILayout.TextField(ContentProperties.AnimationName, animationEventProperty.GetAnimationName()));
                    }

                    AnimationEventProperty.EventProperty[] eventProperties = animationEventProperty.GetEventProperties();
                    SerializedProperty serializedEventProperties = properties.GetArrayElementAtIndex(i).FindPropertyRelative("eventProperties");
                    for (int j = 0, _length = eventProperties.Length; j < _length; j++)
                    {
                        if (!eventsFoldout.ContainsKey(i) || eventsFoldout[i].Count == 0 || j >= eventsFoldout[i].Count)
                            break;

                        BeginSubBox();
                        bool eventFoldout = eventsFoldout[i][j];

                        Rect eventRemoveButtonRect = GUILayoutUtility.GetRect(0, 0);
                        eventRemoveButtonRect.x = eventRemoveButtonRect.width + 15;
                        eventRemoveButtonRect.y += 1;
                        eventRemoveButtonRect.width = 16.5f;
                        eventRemoveButtonRect.height = 16.5f;
                        if (GUI.Button(eventRemoveButtonRect, GUIContent.none, GUI.skin.GetStyle("OL Minus")))
                        {
                            serializedEventProperties.DeleteArrayElementAtIndex(j);
                            eventsFoldout[i].RemoveAt(j);
                            break;
                        }

                        eventFoldout = EditorGUILayout.Foldout(eventFoldout, "Event " + (j + 1), true);
                        if (eventFoldout)
                        {
                            GUILayout.Space(3);
                            AnimationEventProperty.EventProperty eventProperty = eventProperties[j];
                            if (animatorController != null && clips != null && clip != null)
                            {
                                eventProperty.SetAnimationTime(EditorGUILayout.Slider(ContentProperties.AnimationTime, eventProperty.GetAnimationTime(), 0, clip.length));
                            }
                            else
                            {
                                eventProperty.SetAnimationTime(EditorGUILayout.FloatField(ContentProperties.AnimationTime, eventProperty.GetAnimationTime()));
                            }

                            GUILayout.Space(10);
                            GUILayout.Label(ContentProperties.SoundEffect, UEditorStyles.SectionHeaderLabel);
                            GUILayout.Space(5);
                            eventProperty.SetSoundEffect(ObjectField<AudioClip>(ContentProperties.SoundEffect, eventProperty.GetSoundEffect(), false));

                            GUILayout.Space(10);
                            GUILayout.Label(ContentProperties.CameraShake, UEditorStyles.SectionHeaderLabel);
                            GUILayout.Space(5);
                            ShakeCamera.ShakeProperties shakeProperties = eventProperty.GetShakeProperties();
                            shakeProperties.SetTarget((ShakeCamera.ShakeEvent.Target)EditorGUILayout.EnumPopup("Target", shakeProperties.GetTarget()));
                            EditorGUI.BeginDisabledGroup(shakeProperties.GetTarget() == ShakeCamera.ShakeEvent.Target.None);
                            shakeProperties.SetAmplitude(EditorGUILayout.FloatField("Amplitude", shakeProperties.GetAmplitude()));
                            shakeProperties.SetFrequency(EditorGUILayout.FloatField("Frequency", shakeProperties.GetFrequency()));
                            shakeProperties.SetDuration(EditorGUILayout.FloatField("Duration", shakeProperties.GetDuration()));
                            shakeProperties.SetBlendOverLifetime(EditorGUILayout.CurveField("Blend Over Life time", shakeProperties.GetBlendOverLifetime()));
                            EditorGUI.EndDisabledGroup();
                            eventProperty.SetShakeProperties(shakeProperties);
                            eventProperties[j] = eventProperty;
                            GUILayout.Space(3);
                        }
                        eventsFoldout[i][j] = eventFoldout;
                        EndSubBox();
                    }
                    animationEventProperty.SetEventProperties(eventProperties);

                    GUILayout.Space(3);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    bool clipContains = (animatorController != null && clips != null && clip != null && (clip.events != null && clip.events.Length > 0));

                    if (GUILayout.Button(" Add Event ", clipContains ? "ButtonLeft" : "Button"))
                    {
                        serializedEventProperties.arraySize++;
                        eventsFoldout[i].Add(false);
                        break;
                    }

                    if (clipContains)
                    {
                        if (GUILayout.Button(new GUIContent(" Parse Event ", "Current clip contains event.\nPress \"Parse events\" to read and add this events here."), "ButtonRight"))
                        {
                            AnimationEvent[] unityAnimationEvents = clip.events;
                            for (int e = 0, e_length = unityAnimationEvents.Length; e < e_length; e++)
                            {
                                serializedEventProperties.arraySize++;
                                eventsFoldout[i].Add(false);
                                AnimationEvent unityAnimationEvent = unityAnimationEvents[e];
                                SerializedProperty animationTime = serializedEventProperties.GetArrayElementAtIndex(serializedEventProperties.arraySize - 1).FindPropertyRelative("animationTime");
                                SerializedProperty soundEffect = serializedEventProperties.GetArrayElementAtIndex(serializedEventProperties.arraySize - 1).FindPropertyRelative("soundEffect");
                                soundEffect.objectReferenceValue = unityAnimationEvent.objectReferenceParameter;
                                animationTime.floatValue = unityAnimationEvent.time;
                            }
                            break;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3);
                }
                propertiesFoldout[i] = foldout;
                instance.SetProperty(i, animationEventProperty);
                EndBox();
            }
            DecreaseIndentLevel();

            GUILayout.Space(3);

            if (instance.GetLength() == 0)
            {
                UEditorHelpBoxMessages.Tip("Properties is empty!", "Add new properties.");
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Animation", "ButtonLeft", GUILayout.Width(120)))
            {
                properties.arraySize++;
                properties.GetArrayElementAtIndex(properties.arraySize - 1).FindPropertyRelative("animationName").stringValue = "Empty " + properties.arraySize;
                properties.GetArrayElementAtIndex(properties.arraySize - 1).FindPropertyRelative("eventProperties").arraySize = 0;
                propertiesFoldout.Add(false);
                eventsFoldout.Add(properties.arraySize - 1, new List<bool>());
            }

            if (GUILayout.Button("Clear All Animation", "ButtonRight", GUILayout.Width(120)))
            {
                if (UDisplayDialogs.Confirmation(string.Format("Are you sure want clear camera shake properites:[{0}]", properties.arraySize)))
                {
                    properties.arraySize = 0;
                    propertiesFoldout.Clear();
                    eventsFoldout.Clear();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private List<bool> CreatePropertiesFoldout()
        {
            List<bool> foldouts = new List<bool>();

            for (int i = 0, length = instance.GetLength(); i < length; i++)
            {
                foldouts.Add(false);
            }
            return foldouts;
        }

        private Dictionary<int, List<bool>> CreateEventsFoldouts()
        {
            Dictionary<int, List<bool>> eventsFoldout = new Dictionary<int, List<bool>>();

            for (int i = 0, length = instance.GetLength(); i < length; i++)
            {
                AnimationEventProperty animationEventProperty = instance.GetProperty(i);
                eventsFoldout.Add(i, new List<bool>());
                for (int j = 0, _length = animationEventProperty.GetEventProperties().Length; j < _length; j++)
                {
                    eventsFoldout[i].Add(false);
                }
            }
            return eventsFoldout;
        }
    }
}