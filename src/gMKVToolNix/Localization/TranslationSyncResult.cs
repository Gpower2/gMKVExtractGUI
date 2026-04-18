namespace gMKVToolNix.Localization
{
    public sealed class TranslationSyncResult
    {
        public TranslationSyncResult(TranslationFile translationFile, int addedCount, int updatedCount, int removedCount)
        {
            TranslationFile = translationFile;
            AddedCount = addedCount;
            UpdatedCount = updatedCount;
            RemovedCount = removedCount;
        }

        public TranslationFile TranslationFile { get; private set; }

        public int AddedCount { get; private set; }

        public int UpdatedCount { get; private set; }

        public int RemovedCount { get; private set; }
    }
}
