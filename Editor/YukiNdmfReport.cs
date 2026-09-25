using System;
using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
using nadena.dev.ndmf.localization;

namespace TsiYuki.Core.Editor
{
    /// <summary>
    /// Reports build errors to NDMF in a package's own words. NDMF shows them in
    /// its own language setting, so it is handed every language's table from the
    /// package's <see cref="YukiLocalizer"/>, and a key missing in one language
    /// falls back to English on NDMF's side.
    /// </summary>
    public sealed class YukiNdmfReport
    {
        // NDMF's name for each language a TsiYuki package is translated into.
        static readonly Dictionary<YukiLanguageCode, string> NdmfLocales = new Dictionary<YukiLanguageCode, string>
        {
            { YukiLanguageCode.English, "en-us" },
            { YukiLanguageCode.Chinese, "zh-hans" },
            { YukiLanguageCode.Japanese, "ja-jp" },
        };

        readonly YukiLocalizer _localizer;
        Localizer _ndmf;

        public YukiNdmfReport(YukiLocalizer localizer) => _localizer = localizer;

        internal Localizer Localizer => _ndmf ?? (_ndmf = new Localizer(NdmfLocales[YukiLanguageCode.English], Tables));

        /// <summary>
        /// Strings in <paramref name="args"/> fill the message's placeholders;
        /// <paramref name="context"/>, when given, becomes a clickable reference
        /// in NDMF's error window.
        /// </summary>
        public void Report(ErrorSeverity severity, string key, UnityEngine.Object context, params object[] args)
        {
            var all = (args ?? new object[0]).Select(a => (object)(a?.ToString() ?? "")).ToList();
            if (context != null) all.Add(context);
            ErrorReport.ReportError(Localizer, severity, key, all.ToArray());
        }

        List<(string, Func<string, string>)> Tables() =>
            Enum.GetValues(typeof(YukiLanguageCode)).Cast<YukiLanguageCode>()
                .Select(code => (NdmfLocales[code], Lookup(YukiLanguage.FileCodes[(int)code])))
                .ToList();

        Func<string, string> Lookup(string fileCode) => key => _localizer.Find(fileCode, key);
    }
}
