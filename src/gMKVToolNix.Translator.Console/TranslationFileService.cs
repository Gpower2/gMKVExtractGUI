using System.IO;
using gMKVToolNix.Localization;
using Newtonsoft.Json;

namespace gMKVToolNix.Translator.Console
{
    public static class TranslationFileService
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public static TranslationFile LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                System.Console.WriteLine($"ERROR: File not found: {path}");
                return null;
            }
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<TranslationFile>(json);
        }

        public static void SaveFile(TranslationFile file, string path)
        {
            string json = JsonConvert.SerializeObject(file, _settings);
            File.WriteAllText(path, json);
        }
    }
}
