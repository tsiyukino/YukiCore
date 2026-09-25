using System.IO;
using nadena.dev.ndmf;
using nadena.dev.ndmf.localization;
using NUnit.Framework;
using UnityEngine;

namespace TsiYuki.Core.Editor.Tests
{
    // Tables are written to a temporary folder. NDMF's language is a user
    // preference, so each test puts it back the way it found it.
    public class YukiNdmfReportTests
    {
        string _dir;
        string _language;
        YukiLocalizer _localizer;
        YukiNdmfReport _report;

        [SetUp]
        public void SetUp()
        {
            _dir = Path.Combine(Path.GetTempPath(), "YukiNdmfReportTests_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            Write("en", "warn.greet = Hello {0} ({1})", "warn.english_only = Only English");
            Write("zh-Hans", "warn.greet = 你好 {0}（{1}）");
            Write("ja", "# nothing translated yet");

            _language = LanguagePrefs.Language;
            _localizer = new YukiLocalizer(new DirectoryInfo(_dir));
            _report = new YukiNdmfReport(_localizer);
        }

        [TearDown]
        public void TearDown()
        {
            LanguagePrefs.Language = _language;
            Directory.Delete(_dir, true);
        }

        void Write(string code, params string[] lines) => File.WriteAllLines(Path.Combine(_dir, code + ".txt"), lines);

        SimpleError ReportOne(UnityEngine.Object context, params object[] args)
        {
            var errors = ErrorReport.CaptureErrors(() => _report.Report(ErrorSeverity.NonFatal, "warn.greet", context, args));
            Assert.That(errors.Count, Is.EqualTo(1));
            return (SimpleError)errors[0].TheError;
        }

        [Test]
        public void ReportFillsTheMessageInNdmfsLanguage()
        {
            LanguagePrefs.Language = "en-us";
            var error = ReportOne(null, "Yuki", 3);

            Assert.That(error.Severity, Is.EqualTo(ErrorSeverity.NonFatal));
            Assert.That(error.TitleKey, Is.EqualTo("warn.greet"));
            Assert.That(error.FormatTitle(), Is.EqualTo("Hello Yuki (3)"));
            Assert.That(error.References, Is.Empty);
        }

        [Test]
        public void ContextBecomesAReference()
        {
            var go = new GameObject("YukiNdmfReportTests");
            try
            {
                var error = ReportOne(go, "Yuki", 3);
                Assert.That(error.References.Length, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void TranslatedKeyIsShownInThatLanguage()
        {
            LanguagePrefs.Language = "zh-hans";
            Assert.That(ReportOne(null, "Yuki", 3).FormatTitle(), Is.EqualTo("你好 Yuki（3）"));
        }

        [Test]
        public void KeyMissingInALanguageFallsBackToEnglish()
        {
            LanguagePrefs.Language = "ja-jp";
            Assert.That(ReportOne(null, "Yuki", 3).FormatTitle(), Is.EqualTo("Hello Yuki (3)"));
        }

        [Test]
        public void NullArgsAndNullArgumentsAreReportedAsEmpty()
        {
            LanguagePrefs.Language = "en-us";
            Assert.That(ReportOne(null, null).TitleSubst, Is.Empty);
            Assert.That(ReportOne(null, "Yuki", null).TitleSubst, Is.EqualTo(new[] { "Yuki", "" }));
        }

        [Test]
        public void EveryLanguageHasAnNdmfLocale()
        {
            // Building the NDMF localizer walks every YukiLanguageCode and throws on one with no locale.
            Assert.DoesNotThrow(() => _report.Localizer.GetLocalizedString("warn.greet"));
        }

        [Test]
        public void FindHasNoFallback()
        {
            Assert.That(_localizer.Find("zh-Hans", "warn.greet"), Is.EqualTo("你好 {0}（{1}）"));
            Assert.IsNull(_localizer.Find("zh-Hans", "warn.english_only"));
            Assert.IsNull(_localizer.Find("en", "warn.nowhere"));
        }

        [Test]
        public void FindSeesEditsAfterReload()
        {
            Assert.That(_localizer.Find("en", "warn.english_only"), Is.EqualTo("Only English"));
            Write("en", "warn.english_only = Edited");
            _localizer.Reload();
            Assert.That(_localizer.Find("en", "warn.english_only"), Is.EqualTo("Edited"));
        }
    }
}
