using System;
using System.Collections.Generic;
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
                Reload(culture);
                gMKVLogger.Log(string.Format("LocalizationManager initialized successfully with culture: {0}", _currentCulture));
            }
            catch (Exception ex)
            {
                gMKVLogger.Log(string.Format("Error initializing LocalizationManager: {0}", ex.Message));
                throw;
            }
        }

        public static void Reload(string culture = null)
        {
            try
            {
                string targetCulture = string.IsNullOrWhiteSpace(culture)
                    ? _currentCulture
                    : culture;

                _service = new JsonLocalizationService(GetTranslationDirectory());
                _currentCulture = targetCulture;
                _initialized = true;

                gMKVLogger.Log(string.Format("LocalizationManager reloaded successfully with culture: {0}", _currentCulture));
            }
            catch (Exception ex)
            {
                gMKVLogger.Log(string.Format("Error reloading LocalizationManager: {0}", ex.Message));
                throw;
            }
        }

        public static List<string> GetAvailableCultures()
        {
            EnsureInitialized();

            return _service.GetAvailableCultures();
        }

        public static string GetString(string key)
        {
            EnsureInitialized();

            return _service.GetStringForCulture(key, _currentCulture);
        }

        public static string GetString(string key, params object[] formatArgs)
        {
            EnsureInitialized();

            return _service.GetStringForCulture(key, _currentCulture, formatArgs);
        }

        public static string GetStringForCulture(string key, string culture)
        {
            EnsureInitialized();

            return _service.GetStringForCulture(key, culture);
        }

        public static string GetStringForCulture(string key, string culture, params object[] formatArgs)
        {
            EnsureInitialized();

            return _service.GetStringForCulture(key, culture, formatArgs);
        }

        private static void EnsureInitialized()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("LocalizationManager not initialized. Call Initialize() first.");
            }
        }

        private static string GetTranslationDirectory()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(assemblyLocation);
        }
    }
}
