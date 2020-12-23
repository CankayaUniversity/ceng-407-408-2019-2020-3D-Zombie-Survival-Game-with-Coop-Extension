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
using UnrealFPS.AI;

namespace UnrealFPS.Editor
{
    [CustomEditor(typeof(AIAnimatorHandler))]
    [CanEditMultipleObjects]
    public class AIAnimatorHandlerEditor : UEditor<AIAnimatorHandler>
    {
        public override string HeaderName()
        {
            return "AI Animaton Handler";
        }

        public override void BaseGUI()
        {
            UEditorHelpBoxMessages.Message("Animator handled by AI Animaton Handler component.", MessageType.Info);
        }
    }
}