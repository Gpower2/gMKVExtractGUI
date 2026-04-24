using System;
using System.Collections.Generic;
using System.IO;
using gMKVToolNix.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace gMKVToolNix.Unit.Tests
{
    [TestClass]
    public class JsonLocalizationService_Fallback_Tests
    {
        private string _testFolder;

        [TestInitialize]
        public void CreateTestFolder()
        {
            _testFolder = Path.Combine(Path.GetTempPath(), "gMKVExtractGUI.LocalizationTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testFolder);
        }

        [TestCleanup]
        public void CleanupTestFolder()
        {
            if (!string.IsNullOrWhiteSpace(_testFolder) && Directory.Exists(_testFolder))
            {
                Directory.Delete(_testFolder, true);
            }
        }

        [TestMethod]
        public void GetAvailableCultures_WhenNoLocaleFilesExist_StillIncludesEnglishFallback()
        {
            var service = new JsonLocalizationService(_testFolder);

            CollectionAssert.Contains(service.GetAvailableCultures(), "en");
        }

        [TestMethod]
        public void GetStringForCulture_WhenNoLocaleFilesExist_UsesBuiltInEnglishDefaults()
        {
            var service = new JsonLocalizationService(_testFolder);

            string actual = service.GetStringForCulture("UI.Common.Dialog.AreYouSureTitle", "ja");

            Assert.AreEqual("Are you sure?", actual);
        }

        [TestMethod]
        public void GetStringForCulture_WhenEnglishFileIsMissing_UsesBuiltInEnglishFallback()
        {
            WriteTranslationFile(TranslationPathService.GetTranslationFileName("de"), "de", new Dictionary<string, string>
            {
                { "UI.Common.Dialog.AreYouSureTitle", "Sind Sie sicher?" }
            });

            var service = new JsonLocalizationService(_testFolder);

            Assert.AreEqual("Error!", service.GetStringForCulture("UI.Common.Dialog.ErrorTitle", "fr"));
            CollectionAssert.Contains(service.GetAvailableCultures(), "en");
            CollectionAssert.Contains(service.GetAvailableCultures(), "de");
        }

        [TestMethod]
        public void ResolveCultureName_WhenSpecificCultureMissingButNeutralExists_ReturnsNeutralCulture()
        {
            WriteTranslationFile(TranslationPathService.GetTranslationFileName("de"), "de", new Dictionary<string, string>
            {
                { "UI.Common.Dialog.AreYouSureTitle", "Sind Sie sicher?" }
            });

            var service = new JsonLocalizationService(_testFolder);

            Assert.AreEqual("de", service.ResolveCultureName("de-DE"));
        }

        [TestMethod]
        public void ResolveCultureName_WhenChineseAliasesAreRequested_ReturnsCanonicalChineseLocales()
        {
            WriteTranslationFile(TranslationPathService.GetTranslationFileName("zh-cn"), "zh-cn", new Dictionary<string, string>
            {
                { "UI.Common.Dialog.AreYouSureTitle", "你确定吗？" }
            });

            WriteTranslationFile(TranslationPathService.GetTranslationFileName("zh-tw"), "zh-tw", new Dictionary<string, string>
            {
                { "UI.Common.Dialog.AreYouSureTitle", "您確定嗎？" }
            });

            var service = new JsonLocalizationService(_testFolder);

            Assert.AreEqual("zh-tw", service.ResolveCultureName("cn"));
            Assert.AreEqual("zh-cn", service.ResolveCultureName("zh"));
            Assert.AreEqual("zh-cn", service.ResolveCultureName("zh-Hans"));
            Assert.AreEqual("zh-tw", service.ResolveCultureName("zh-Hant"));
        }

        [TestMethod]
        public void GetAvailableCultures_WhenLegacyChineseFileExists_UsesCanonicalCultureOnly()
        {
            WriteTranslationFile(TranslationPathService.GetTranslationFileName("cn"), "cn", new Dictionary<string, string>
            {
                { "UI.Common.Dialog.AreYouSureTitle", "您確定嗎？" }
            });

            WriteTranslationFile(TranslationPathService.GetTranslationFileName("zh-tw"), "zh-tw", new Dictionary<string, string>
            {
                { "UI.Common.Dialog.AreYouSureTitle", "您確定嗎？" }
            });

            var service = new JsonLocalizationService(_testFolder);
            var cultures = service.GetAvailableCultures();

            CollectionAssert.Contains(cultures, "en");
            CollectionAssert.Contains(cultures, "zh-tw");
            CollectionAssert.DoesNotContain(cultures, "cn");
        }

        [TestMethod]
        public void BuiltInEnglishDefaults_ShouldMatchEnglishJsonEntries()
        {
            var service = new JsonLocalizationService(_testFolder);

            string englishJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TranslationPathService.GetTranslationFileName("en"));
            var translationFile = JsonConvert.DeserializeObject<TranslationFile>(File.ReadAllText(englishJsonPath));

            foreach (var entry in translationFile.Entries)
            {
                string expected = string.IsNullOrWhiteSpace(entry.Value.Translation)
                    ? entry.Value.Source
                    : entry.Value.Translation;

                Assert.AreEqual(expected, service.GetStringForCulture(entry.Key, "en"), entry.Key);
            }
        }

        private void WriteTranslationFile(string fileName, string culture, IDictionary<string, string> entries)
        {
            var translationFile = new TranslationFile
            {
                Metadata = new Metadata
                {
                    Culture = culture,
                    CreationDate = new DateTime(2026, 4, 18),
                    LastEditDate = new DateTime(2026, 4, 18)
                }
            };

            foreach (var entry in entries)
            {
                translationFile.Entries[entry.Key] = new TranslationEntry
                {
                    Source = entry.Value,
                    Translation = entry.Value,
                    IsTranslated = true,
                    Notes = null
                };
            }

            File.WriteAllText(
                Path.Combine(_testFolder, fileName),
                JsonConvert.SerializeObject(translationFile));
        }
    }
}
