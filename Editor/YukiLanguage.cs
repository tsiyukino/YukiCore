using System;
using UnityEditor;

namespace TsiYuki.Core.Editor
{
    public enum YukiLanguageCode
    {
        English = 0,
        Chinese = 1,
        Japanese = 2,
    }

    /// <summary>
    /// The UI language shared by every TsiYuki tool. English is the default; the choice is
    /// stored per user in EditorPrefs so switching it in one tool switches all of them.
    /// </summary>
    public static class YukiLanguage
    {
        private const string PrefKey = "moe.tsiyuki.language";

        public static readonly string[] DisplayNames = { "English", "简体中文", "日本語" };
        internal static readonly string[] FileCodes = { "en", "zh-Hans", "ja" };

        public static event Action Changed;

        public static YukiLanguageCode Current
        {
            get
            {
                var value = EditorPrefs.GetInt(PrefKey, 0);
                return value >= 0 && value < FileCodes.Length ? (YukiLanguageCode)value : YukiLanguageCode.English;
            }
            set
            {
                if (value == Current) return;
                EditorPrefs.SetInt(PrefKey, (int)value);
                Changed?.Invoke();
            }
        }

        internal static void NotifyChanged() => Changed?.Invoke();

        internal static string CurrentFileCode => FileCodes[(int)Current];
    }
}
