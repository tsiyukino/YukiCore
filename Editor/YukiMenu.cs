using UnityEditor;

namespace TsiYuki.Core.Editor
{
    /// <summary>
    /// Every TsiYuki tool lives under one top-level "TsiYuki" menu. Tools use the default priority (1000);
    /// shared settings sit below them, separated.
    /// </summary>
    public static class YukiMenu
    {
        public const string Root = "TsiYuki/";
        public const int SettingsPriority = 2000;

        private const string LanguagePath = Root + "Language/";

        [MenuItem(LanguagePath + "English", priority = SettingsPriority)]
        private static void English() => YukiLanguage.Current = YukiLanguageCode.English;

        [MenuItem(LanguagePath + "简体中文", priority = SettingsPriority + 1)]
        private static void Chinese() => YukiLanguage.Current = YukiLanguageCode.Chinese;

        [MenuItem(LanguagePath + "日本語", priority = SettingsPriority + 2)]
        private static void Japanese() => YukiLanguage.Current = YukiLanguageCode.Japanese;

        [MenuItem(LanguagePath + "English", true)]
        private static bool EnglishCheck() => Check(YukiLanguageCode.English, "English");

        [MenuItem(LanguagePath + "简体中文", true)]
        private static bool ChineseCheck() => Check(YukiLanguageCode.Chinese, "简体中文");

        [MenuItem(LanguagePath + "日本語", true)]
        private static bool JapaneseCheck() => Check(YukiLanguageCode.Japanese, "日本語");

        private static bool Check(YukiLanguageCode code, string name)
        {
            Menu.SetChecked(LanguagePath + name, YukiLanguage.Current == code);
            return true;
        }
    }
}
