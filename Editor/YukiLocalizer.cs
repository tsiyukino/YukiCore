using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace TsiYuki.Core.Editor
{
    /// <summary>
    /// Loads UI strings for one package from <c>Packages/&lt;package&gt;/Localization/&lt;code&gt;.txt</c>.
    /// File format: one <c>key = value</c> per line, <c>#</c> starts a comment, <c>\n</c> in a value is a
    /// line break. Missing keys fall back to English, then to the key itself.
    /// </summary>
    public sealed class YukiLocalizer
    {
        private readonly string _directory;
        private readonly Dictionary<string, Dictionary<string, string>> _tables = new Dictionary<string, Dictionary<string, string>>();

        public YukiLocalizer(string packageName) : this(new DirectoryInfo($"Packages/{packageName}/Localization")) { }

        /// <summary>Reads the tables from any folder; tests point it at a temporary one.</summary>
        internal YukiLocalizer(DirectoryInfo directory)
        {
            _directory = directory.FullName;
            YukiLanguage.Changed += () => _tables.Clear();
        }

        public string this[string key] => Tr(key);

        public string Tr(string key)
        {
            if (Table(YukiLanguage.CurrentFileCode).TryGetValue(key, out var value)) return value;
            if (Table("en").TryGetValue(key, out value)) return value;
            return key;
        }

        public string Tr(string key, params object[] args)
        {
            var format = Tr(key);
            try { return string.Format(format, args); }
            catch (System.FormatException) { return format; }
        }

        public bool Has(string key) => Table("en").ContainsKey(key);

        /// <summary>
        /// The value in one language's table, or null when that table lacks the key. No fallback:
        /// callers that have their own (NDMF's) need to see the gap.
        /// </summary>
        internal string Find(string fileCode, string key) =>
            Table(fileCode).TryGetValue(key, out var value) ? value : null;

        private Dictionary<string, string> Table(string code)
        {
            if (_tables.TryGetValue(code, out var table)) return table;
            table = new Dictionary<string, string>();
            var fullPath = Path.Combine(_directory, code + ".txt");
            if (File.Exists(fullPath))
            {
                foreach (var rawLine in File.ReadAllLines(fullPath))
                {
                    var line = rawLine.Trim();
                    if (line.Length == 0 || line.StartsWith("#")) continue;
                    var eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    var key = line.Substring(0, eq).Trim();
                    var value = line.Substring(eq + 1).Trim().Replace("\\n", "\n");
                    table[key] = value;
                }
            }
            _tables[code] = table;
            return table;
        }

        /// <summary>Forces a reload, e.g. after editing a localization file.</summary>
        public void Reload() => _tables.Clear();

        [MenuItem(YukiMenu.Root + "Reload Localization", priority = YukiMenu.SettingsPriority + 20)]
        private static void ReloadAll() => YukiLanguage.NotifyChanged();
    }
}
