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
using UnityEditorInternal;
using UnityEngine;
using UnrealFPS.AI;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(AIFieldOfView))]
    [CanEditMultipleObjects]
    public class AIFieldOfViewEditor : UEditor<AIFieldOfView>
    {
        internal new static class ContentProperties
        {
            public static readonly GUIContent ViewProperties = new GUIContent("View Properties");
            public static readonly GUIContent MasksProperties = new GUIContent("Mask Properties");

            public static readonly GUIContent ViewRadius = new GUIContent("View Radius", "Max AI field of view radius.");
            public static readonly GUIContent ViewAngle = new GUIContent("View Angle", "Max AI field of view angle.");
            public static readonly GUIContent TargetMask = new GUIContent("Targets", "Enemy target masks for this AI.");
            public static readonly GUIContent ObstacleMask = new GUIContent("Obstacles", "Obstacle masks for field of view.");
        }

        private Collider collider;

        public override void InitializeProperties()
        {
            collider = instance.GetComponent<Collider>();
        }

        public override string HeaderName()
        {
            return "AI Field Of View";
        }

        public override void BaseGUI()
        {
            BeginBox();
            GUILayout.Label(ContentProperties.ViewProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            instance.SetViewRadius(EditorGUILayout.FloatField(ContentProperties.ViewRadius, instance.GetViewRadius()));
            instance.SetViewAngle(EditorGUILayout.Slider(ContentProperties.ViewAngle, instance.GetViewAngle(), 0, 360));

            GUILayout.Space(10);
            GUILayout.Label(ContentProperties.MasksProperties, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            LayerMask targetMask = EditorGUILayout.MaskField(ContentProperties.TargetMask,InternalEditorUtility.LayerMaskToConcatenatedLayersMask(instance.GetTargetMask()), InternalEditorUtility.layers);
            instance.SetTargetMask(InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(targetMask));
            LayerMask obstacleMask = EditorGUILayout.MaskField(ContentProperties.ObstacleMask,InternalEditorUtility.LayerMaskToConcatenatedLayersMask(instance.GetObstacleMask()), InternalEditorUtility.layers);
            instance.SetObstacleMask(InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(obstacleMask));
            EndBox();
        }

        protected virtual void OnSceneGUI()
        {
            Handles.color = Color.white;
            Vector3 position = instance.transform.position;
            if (collider != null)
                position = collider.bounds.center;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, instance.GetViewRadius());
            Vector3 viewAngleA = instance.DirectionFromAngle(-instance.GetViewAngle() / 2, false);
            Vector3 viewAngleB = instance.DirectionFromAngle(instance.GetViewAngle() / 2, false);

            Handles.DrawLine(position, position + viewAngleA * instance.GetViewRadius());
            Handles.DrawLine(position, position + viewAngleB * instance.GetViewRadius());

            Handles.color = Color.red;
            if (instance.GetVisibleTargets() != null)
            {
                for (int i = 0, length = instance.GetVisibleTargetsCount(); i < length; i++)
                {
                    Handles.DrawLine(position, instance.GetVisibleTarget(i).position);
                }
            }
        }
    }
}