using UnityEditor;
using UnityEngine;

namespace TsiYuki.Core.Editor
{
    /// <summary>Shared IMGUI helpers so every TsiYuki tool looks and behaves the same.</summary>
    public static class YukiGUI
    {
        private static GUIStyle _title;
        private static GUIStyle _sectionHeader;
        private static GUIStyle _wrapMini;

        public static GUIStyle TitleStyle => _title ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 };
        public static GUIStyle SectionHeaderStyle => _sectionHeader ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
        public static GUIStyle WrapMini => _wrapMini ??= new GUIStyle(EditorStyles.miniLabel) { wordWrap = true, richText = true };

        /// <summary>Title row with the tool name, version and the shared language selector.</summary>
        public static void Header(string title, string version)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(title, TitleStyle);
                if (!string.IsNullOrEmpty(version)) GUILayout.Label(version, EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
                LanguagePopup(GUILayout.Width(90));
            }
        }

        public static void LanguagePopup(params GUILayoutOption[] options)
        {
            var current = (int)YukiLanguage.Current;
            var next = EditorGUILayout.Popup(current, YukiLanguage.DisplayNames, options);
            if (next != current) YukiLanguage.Current = (YukiLanguageCode)next;
        }

        public static void Section(string title)
        {
            EditorGUILayout.Space(6);
            GUILayout.Label(title, SectionHeaderStyle);
        }

        /// <summary>A coloured status dot followed by a label; colours follow the editor skin.</summary>
        public static void StatusLabel(YukiStatus status, string text, params GUILayoutOption[] options)
        {
            var previous = GUI.contentColor;
            GUI.contentColor = StatusColor(status);
            GUILayout.Label("●", EditorStyles.label, GUILayout.Width(14));
            GUI.contentColor = previous;
            GUILayout.Label(text, EditorStyles.label, options);
        }

        public static Color StatusColor(YukiStatus status)
        {
            var pro = EditorGUIUtility.isProSkin;
            switch (status)
            {
                case YukiStatus.Ok: return pro ? new Color(0.45f, 0.85f, 0.5f) : new Color(0.15f, 0.55f, 0.2f);
                case YukiStatus.Approximate: return pro ? new Color(0.95f, 0.8f, 0.35f) : new Color(0.7f, 0.5f, 0.05f);
                case YukiStatus.Problem: return pro ? new Color(1f, 0.45f, 0.4f) : new Color(0.75f, 0.15f, 0.1f);
                default: return pro ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.4f, 0.4f, 0.4f);
            }
        }
    }

    public enum YukiStatus
    {
        None,
        Ok,
        Approximate,
        Problem,
    }
}
