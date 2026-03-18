using System;
using System.IO;
using System.Reflection;
using gMKVToolNix.Log;

namespace gMKVToolNix.Localization
{
    public static class LocalizationManager
    {
        private static JsonLocalizationService _service = null;
        private static string _currentCulture = "en";
        private static bool _initialized = false;

        public static string CurrentCulture
        {
            get { return _currentCulture; }
            set 
            { 
                _currentCulture = value;
                gMKVLogger.Log(string.Format("Culture changed to: {0}", _currentCulture));
            }
        }

        public static bool IsInitialized
        {
            get { return _initialized; }
        }

        public static void Initialize(string culture = "en")
        {
            if (_initialized)
            {
                gMKVLogger.Log("LocalizationManager already initialized.");
                return;
            }

            try
            {
                gMKVLogger.Log("Initializing LocalizationManager...");
                
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                
                _service = new JsonLocalizationService(assemblyDirectory);
                _currentCulture = culture;
                _initialized = true;

                gMKVLogger.Log(string.Format("LocalizationManager initialized successfully with culture: {0}", _currentCulture));
            }
            catch (Exception ex)
            {
                gMKVLogger.Log(string.Format("Error initializing LocalizationManager: {0}", ex.Message));
                throw;
            }
        }

        public static string GetString(string key)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("LocalizationManager not initialized. Call Initialize() first.");
            }

            return _service.GetString(key, _currentCulture);
        }

        public static string GetString(string key, string culture)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("LocalizationManager not initialized. Call Initialize() first.");
            }

            return _service.GetString(key, culture);
        }
    }
}
