using System;
using System.IO;
using System.Linq;
using gMKVToolNix.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolNix.Unit.Tests
{
    [TestClass]
    public class TranslationPathService_Tests
    {
        [TestMethod]
        public void GetTranslationFileName_ShouldPrefixCultureCode()
        {
            Assert.AreEqual("gmkvextract-de.json", TranslationPathService.GetTranslationFileName("de"));
            Assert.AreEqual("gmkvextract-pt-br.json", TranslationPathService.GetTranslationFileName("pt-br"));
        }

        [TestMethod]
        public void TryGetCultureFromPath_ShouldStripPrefixWhenPresent()
        {
            string culture;

            Assert.IsTrue(TranslationPathService.TryGetCultureFromPath(@"C:\Temp\gmkvextract-hi.json", out culture));
            Assert.AreEqual("hi", culture);

            Assert.IsTrue(TranslationPathService.TryGetCultureFromPath(@"C:\Temp\de.json", out culture));
            Assert.AreEqual("de", culture);
        }

        [TestMethod]
        public void EnumerateTranslationFiles_WhenPrefixedFilesExist_ShouldPreferOnlyPrefixedFiles()
        {
            string folder = CreateTempFolder();

            try
            {
                File.WriteAllText(Path.Combine(folder, "gmkvextract-en.json"), "{}");
                File.WriteAllText(Path.Combine(folder, "gmkvextract-de.json"), "{}");
                File.WriteAllText(Path.Combine(folder, "en.json"), "{}");
                File.WriteAllText(Path.Combine(folder, "otherapp-en.json"), "{}");

                string[] files = TranslationPathService.EnumerateTranslationFiles(folder)
                    .Select(Path.GetFileName)
                    .ToArray();

                CollectionAssert.AreEqual(
                    new[] { "gmkvextract-de.json", "gmkvextract-en.json" },
                    files);
            }
            finally
            {
                DeleteTempFolder(folder);
            }
        }

        [TestMethod]
        public void EnumerateTranslationFiles_WhenNoPrefixedFilesExist_ShouldFallbackToLegacyNames()
        {
            string folder = CreateTempFolder();

            try
            {
                File.WriteAllText(Path.Combine(folder, "en.json"), "{}");
                File.WriteAllText(Path.Combine(folder, "de.json"), "{}");

                string[] files = TranslationPathService.EnumerateTranslationFiles(folder)
                    .Select(Path.GetFileName)
                    .ToArray();

                CollectionAssert.AreEqual(
                    new[] { "de.json", "en.json" },
                    files);
            }
            finally
            {
                DeleteTempFolder(folder);
            }
        }

        private static string CreateTempFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "gMKVExtractGUI.TranslationPathTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            return folder;
        }

        private static void DeleteTempFolder(string folder)
        {
            if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }
    }
}
