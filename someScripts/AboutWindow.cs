/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System;
using UnityEditor;
using UnityEngine;

namespace UnrealFPS.Editor
{
    public class AboutWindow : EditorWindow
    {
        internal static class StyleProperties
        {
            public static GUIStyle Logo
            {
                get
                {
                    GUIStyle style = new GUIStyle();
                    style.fontSize = 50;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = new Color32(50, 55, 60, 255);
                    return style;
                }
            }

            public static GUIStyle Header
            {
                get
                {
                    GUIStyle style = new GUIStyle();
                    style.fontSize = 17;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = new Color32(70, 70, 70, 255);
                    return style;
                }
            }

            public readonly static GUIStyle Label = new GUIStyle()
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };

            public static GUIStyle Link
            {
                get
                {
                    GUIStyle style = new GUIStyle();
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 13;
                    style.normal.textColor = new Color32(0, 70, 255, 255);
                    return style;
                }
            }
        }

        private static Vector2 WindowSize = new Vector2(505, 315);
        private Texture2D mainLogo;

        /// <summary>
        /// Open About window.
        /// </summary>
        [MenuItem("Unreal FPS/About", false, 0)]
        public static void Open()
        {
            AboutWindow window = ScriptableObject.CreateInstance(typeof(AboutWindow)) as AboutWindow;
            window.titleContent = new GUIContent("About Unreal FPS");
            window.maxSize = WindowSize;
            window.minSize = WindowSize;
            window.ShowUtility();
        }

        protected virtual void OnEnable()
        {
            mainLogo = UEditorResourcesHelper.GetIcon("Logo Short");
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// </summary>
        protected virtual void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (mainLogo != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(7);
                GUILayout.Label(mainLogo);
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(15);
                GUILayout.Label(UnrealFPSInfo.NAME, StyleProperties.Logo);
                GUILayout.Space(15);
                GUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(3);
            Label("Product:", UnrealFPSInfo.NAME);
            Label("Release:", UnrealFPSInfo.RELEASE);
            Label("Publisher:", UnrealFPSInfo.PUBLISHER);
            Label("Author:", UnrealFPSInfo.AUTHOR);
            Label("Version:", UnrealFPSInfo.VERSION);
            Label(UnrealFPSInfo.COPYRIGHT);
            GUILayout.Space(3);
            GUILayout.EndVertical();

            GUILayout.Space(7);

            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(3);
            OpenSupportButton("For get full informations about " + UnrealFPSInfo.NAME + " see - ", "Documentation", UserHelper.OpenDocumentation);
            GUILayout.Space(1);
            OpenSupportButton("To keep abreast of all the new news, follow us on - ", "Twitter", UserHelper.OFFICIAL_TWITTER_URL);
            GUILayout.Space(1);
            OpenSupportButton("If you have any questions you can ask them in the - ", "Official Thread", UserHelper.OFFICIAL_THREAD_URL);
            GUILayout.Space(2);
            SelectableContent("Official support - ", UserHelper.OFFICIAL_EMAIL);
            GUILayout.Space(3);
            GUILayout.EndVertical();
        }

        private static void Label(string title, string message = "", float space = 20.0f)
        {
            GUILayout.Space(1);
            GUILayout.BeginHorizontal(GUILayout.Width(1));
            GUILayout.Label(title, StyleProperties.Label, GUILayout.Width(75));
            if (message != "")
            {
                GUILayout.Space(space);
                GUILayout.Label(message, StyleProperties.Label);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(1);
        }

        public static void SelectableContent(string content, string selectableContent)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(content, StyleProperties.Label, GUILayout.Height(17));
            EditorGUILayout.SelectableLabel(selectableContent, StyleProperties.Link, GUILayout.Height(17));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void OpenSupportButton(string content, string buttonContent, string url)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(1));
            GUILayout.Label(content, StyleProperties.Label, GUILayout.Height(17));
            if (GUILayout.Button(buttonContent, StyleProperties.Link, GUILayout.Height(17)))
                Application.OpenURL(url);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void OpenSupportButton(string content, string buttonContent, Action buttonAction)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(1));
            GUILayout.Label(content, StyleProperties.Label, GUILayout.Height(17));
            if (GUILayout.Button(buttonContent, StyleProperties.Link, GUILayout.Height(17)))
                buttonAction.Invoke();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}