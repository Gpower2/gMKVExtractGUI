namespace gMKVToolNix.Localization
{
    public interface ILocalizationService
    {
        string GetString(string key, string cultureName);

        string GetString(string key, string cultureName, params object[] formatArgs);
    }
}