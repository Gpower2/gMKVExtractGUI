using System;
using System.IO;
using Newtonsoft.Json;

namespace gMKVToolNix.Localization
{
    public static class TranslationFileService
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public static TranslationFile LoadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A translation file path is required.", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Translation file not found.", path);
            }

            string json = File.ReadAllText(path);
            var translationFile = JsonConvert.DeserializeObject<TranslationFile>(json);

            if (translationFile == null || translationFile.Metadata == null || translationFile.Entries == null)
            {
                throw new InvalidDataException(string.Format("Translation file '{0}' is not valid.", path));
            }

            return translationFile;
        }

        public static void SaveFile(TranslationFile file, string path)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A translation file path is required.", nameof(path));
            }

            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(file, _settings);
            File.WriteAllText(path, json);
        }
    }
}
