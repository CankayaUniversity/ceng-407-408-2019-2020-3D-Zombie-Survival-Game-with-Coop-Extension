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
    [CustomEditor(typeof(AudioEvent), true)]
    [CanEditMultipleObjects]
    [System.Obsolete("AudioEvent component is obsolete.\nUse animation event properties.")]
    public class AudioEventEditor : UEditor
    {
        internal new static class ContentProperties
        {
            public readonly static string InfoMessage = "Management of the weapon animation sounds is carried out by means of the controller: Audio Event.";
        }

        public override string HeaderName()
        {
            return "Audio Event";
        }

        public override void BaseGUI()
        {
            UEditorHelpBoxMessages.Message(ContentProperties.InfoMessage, MessageType.Info);
        }
    }
}