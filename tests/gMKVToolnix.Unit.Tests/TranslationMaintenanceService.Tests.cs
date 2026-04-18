using System;
using System.IO;
using gMKVToolNix.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolNix.Unit.Tests
{
    [TestClass]
    public class TranslationMaintenanceService_Tests
    {
        [TestMethod]
        public void CreateTemplate_ShouldCopyEntriesAndMarkThemUntranslated()
        {
            var master = new TranslationFile();
            master.Metadata.Culture = "en";
            master.Entries["UI.Sample.Title"] = new TranslationEntry
            {
                Source = "Sample",
                Translation = "Sample",
                IsTranslated = true,
                Notes = "Sample note"
            };

            var template = TranslationMaintenanceService.CreateTemplate(master, "it", "Translator");

            Assert.AreEqual("it", template.Metadata.Culture);
            Assert.AreEqual("Translator", template.Metadata.Translator);
            Assert.AreEqual("Sample", template.Entries["UI.Sample.Title"].Source);
            Assert.AreEqual("Sample", template.Entries["UI.Sample.Title"].Translation);
            Assert.IsFalse(template.Entries["UI.Sample.Title"].IsTranslated);
            Assert.AreEqual("Sample note", template.Entries["UI.Sample.Title"].Notes);
        }

        [TestMethod]
        public void Synchronize_ShouldAddRemoveAndResetUpdatedEntries()
        {
            var master = new TranslationFile();
            master.Metadata.Culture = "en";
            master.Entries["Existing.Same"] = new TranslationEntry { Source = "Source 1", Translation = "Source 1", IsTranslated = true, Notes = "Note 1" };
            master.Entries["Existing.Changed"] = new TranslationEntry { Source = "Updated Source", Translation = "Updated Source", IsTranslated = true, Notes = "Note 2" };
            master.Entries["New.Key"] = new TranslationEntry { Source = "New Source", Translation = "New Source", IsTranslated = true, Notes = "Note 3" };

            var target = new TranslationFile();
            target.Metadata.Culture = "fr";
            target.Metadata.Translator = "Translator";
            target.Entries["Existing.Same"] = new TranslationEntry { Source = "Source 1", Translation = "Traduction", IsTranslated = true, Notes = "Old note" };
            target.Entries["Existing.Changed"] = new TranslationEntry { Source = "Old Source", Translation = "Ancienne traduction", IsTranslated = true, Notes = "Old note 2" };
            target.Entries["Removed.Key"] = new TranslationEntry { Source = "Removed", Translation = "Supprime", IsTranslated = true, Notes = "Old note 3" };

            var result = TranslationMaintenanceService.Synchronize(master, target);

            Assert.AreEqual(1, result.AddedCount);
            Assert.AreEqual(1, result.UpdatedCount);
            Assert.AreEqual(1, result.RemovedCount);
            Assert.AreEqual(3, result.TranslationFile.Entries.Count);
            Assert.IsFalse(result.TranslationFile.Entries.ContainsKey("Removed.Key"));
            Assert.AreEqual("Traduction", result.TranslationFile.Entries["Existing.Same"].Translation);
            Assert.AreEqual("Note 1", result.TranslationFile.Entries["Existing.Same"].Notes);
            Assert.AreEqual("Updated Source", result.TranslationFile.Entries["Existing.Changed"].Translation);
            Assert.IsFalse(result.TranslationFile.Entries["Existing.Changed"].IsTranslated);
            Assert.AreEqual("New Source", result.TranslationFile.Entries["New.Key"].Translation);
            Assert.IsFalse(result.TranslationFile.Entries["New.Key"].IsTranslated);
        }

        [TestMethod]
        public void TranslationFileService_ShouldRoundTripTranslationFiles()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), "gMKVExtractGUI.LocalizationTests", Guid.NewGuid().ToString("N"), "roundtrip.json");

            try
            {
                var file = new TranslationFile();
                file.Metadata.Culture = "es";
                file.Metadata.Translator = "Translator";
                file.Metadata.CreationDate = new DateTime(2026, 4, 18, 0, 0, 0, DateTimeKind.Utc);
                file.Metadata.LastEditDate = file.Metadata.CreationDate;
                file.Entries["UI.Sample.Title"] = new TranslationEntry
                {
                    Source = "Sample",
                    Translation = "Ejemplo",
                    IsTranslated = true,
                    Notes = "Note"
                };

                TranslationFileService.SaveFile(file, tempFile);
                var loaded = TranslationFileService.LoadFile(tempFile);

                Assert.AreEqual("es", loaded.Metadata.Culture);
                Assert.AreEqual("Translator", loaded.Metadata.Translator);
                Assert.AreEqual("Ejemplo", loaded.Entries["UI.Sample.Title"].Translation);
                Assert.IsTrue(loaded.Entries["UI.Sample.Title"].IsTranslated);
            }
            finally
            {
                string directory = Path.GetDirectoryName(tempFile);
                if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }
    }
}
