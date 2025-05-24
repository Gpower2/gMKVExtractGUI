using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gMKVToolNix
{
    public class gSettings
    {
        private string _MkvToolnixPath = "";
        public string MkvToolnixPath
        {
            get { return _MkvToolnixPath; }
            set { _MkvToolnixPath = value; }
        }

        private MkvChapterTypes _ChapterType = MkvChapterTypes.XML;
        public MkvChapterTypes ChapterType
        {
            get { return _ChapterType; }
            set { _ChapterType = value; }
        }

        private bool _LockedOutputDirectory;
        public bool LockedOutputDirectory
        {
            get { return _LockedOutputDirectory; }
            set { _LockedOutputDirectory = value; }
        }

        private string _OutputDirectory;
        public string OutputDirectory
        {
            get { return _OutputDirectory; }
            set { _OutputDirectory = value; }
        }

        private string _DefaultOutputDirectory;
        public string DefaultOutputDirectory
        {
            get { return _DefaultOutputDirectory; }
            set { _DefaultOutputDirectory = value; }
        }

        private int _WindowPosX;
        public int WindowPosX
        {
            get { return _WindowPosX; }
            set { _WindowPosX = value; }
        }

        private int _WindowPosY;
        public int WindowPosY
        {
            get { return _WindowPosY; }
            set { _WindowPosY = value; }
        }

        private int _WindowSizeWidth = 640;
        public int WindowSizeWidth
        {
            get { return _WindowSizeWidth; }
            set { _WindowSizeWidth = value; }
        }

        private int _WindowSizeHeight = 600;
        public int WindowSizeHeight
        {
            get { return _WindowSizeHeight; }
            set { _WindowSizeHeight = value; }
        }

        private bool _JobMode;
        public bool JobMode
        {
            get { return _JobMode; }
            set { _JobMode = value; }
        }

        private FormWindowState _WindowState;
        public FormWindowState WindowState
        {
            get { return _WindowState; }
            set { _WindowState = value; }
        }

        private bool _ShowPopup = true;
        public bool ShowPopup
        {
            get { return _ShowPopup; }
            set { _ShowPopup = value; }
        }

        private bool _ShowPopupInJobManager = true;
        public bool ShowPopupInJobManager
        {
            get { return _ShowPopupInJobManager; }
            set { _ShowPopupInJobManager = value; }
        }

        private bool _AppendOnDragAndDrop = false;
        public bool AppendOnDragAndDrop
        {
            get { return _AppendOnDragAndDrop; }
            set { _AppendOnDragAndDrop = value; }
        }

        private bool _DarkMode = false;
        public bool DarkMode
        {
            get { return _DarkMode; }
            set { _DarkMode = value; }
        }

        private String _VideoTrackFilenamePattern = "{FilenameNoExt}_track{TrackNumber}_[{Language}]";
        [DefaultValue("{FilenameNoExt}_track{TrackNumber}_[{Language}]")]
        public string VideoTrackFilenamePattern
        {
            get { return _VideoTrackFilenamePattern; }
            set { _VideoTrackFilenamePattern = value; }
        }

        private string _AudioTrackFilenamePattern = "{FilenameNoExt}_track{TrackNumber}_[{Language}]_DELAY {EffectiveDelay}ms";
        [DefaultValue("{FilenameNoExt}_track{TrackNumber}_[{Language}]_DELAY {EffectiveDelay}ms")]
        public string AudioTrackFilenamePattern
        {
            get { return _AudioTrackFilenamePattern; }
            set { _AudioTrackFilenamePattern = value; }
        }

        private string _SubtitleTrackFilenamePattern = "{FilenameNoExt}_track{TrackNumber}_[{Language}]";
        [DefaultValue("{FilenameNoExt}_track{TrackNumber}_[{Language}]")]
        public string SubtitleTrackFilenamePattern
        {
            get { return _SubtitleTrackFilenamePattern; }
            set { _SubtitleTrackFilenamePattern = value; }
        }

        private string _ChapterFilenamePattern = "{FilenameNoExt}_chapters";
        [DefaultValue("{FilenameNoExt}_chapters")]
        public string ChapterFilenamePattern
        {
            get { return _ChapterFilenamePattern; }
            set { _ChapterFilenamePattern = value; }
        }

        private string _AttachmentFilenamePattern = "{AttachmentFilename}";
        [DefaultValue("{AttachmentFilename}")]
        public string AttachmentFilenamePattern
        {
            get { return _AttachmentFilenamePattern; }
            set { _AttachmentFilenamePattern = value; }
        }

        /// <summary>
        /// Gets the Default Value Attribute value for a specific property
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="argPropertyName">The property name</param>
        /// <returns></returns>
        public T GetPropertyDefaultValue<T>(string argPropertyName)
        {
            var v = (this.GetType().GetProperty(argPropertyName)?.GetCustomAttributes(false)?.
            FirstOrDefault(a => ((a as Attribute).TypeId as Type).UnderlyingSystemType == typeof(DefaultValueAttribute)) as DefaultValueAttribute)?.Value;

            return (T)v;
        }

        private static readonly string _SETTINGS_FILE = "gMKVExtractGUI.ini";
        private readonly string _SettingsPath = "";

        public gSettings(string appPath)
        {
            // check if user has permission for appPath
            bool userHasPermission = false;
            try
            {
                using (FileStream tmp = File.Open(Path.Combine(appPath, _SETTINGS_FILE), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    tmp.Flush();
                }
                userHasPermission = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                userHasPermission = false;
            }

            // If user doesn't have permissions to the application path,
            // use the current user appdata folder
            if (userHasPermission)
            {
                _SettingsPath = appPath;
            }
            else
            {
                _SettingsPath = Application.UserAppDataPath;
            }

            // Log the detected settings path
            gMKVLogger.Log(string.Format("Detected settings path: {0}", _SettingsPath));
        }

        public void Reload()
        {
            if (!File.Exists(Path.Combine(_SettingsPath, _SETTINGS_FILE)))
            {
                gMKVLogger.Log(string.Format("Settings file '{0}' not found! Saving defaults...", Path.Combine(_SettingsPath, _SETTINGS_FILE)));
                Save();
            }
            else
            {
                gMKVLogger.Log("Begin loading settings...");
                using (StreamReader sr = new StreamReader(Path.Combine(_SettingsPath, _SETTINGS_FILE), Encoding.UTF8))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("MKVToolnix Path:"))
                        {
                            try
                            {
                                _MkvToolnixPath = line.Substring(line.IndexOf(":") + 1);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading MKVToolnix Path! {0}", ex.Message));
                                _MkvToolnixPath = "";
                            }
                        }
                        else if (line.StartsWith("Chapter Type:"))
                        {
                            try
                            {
                                _ChapterType = (MkvChapterTypes)Enum.Parse(typeof(MkvChapterTypes), line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Chapter Type! {0}", ex.Message));
                                _ChapterType = MkvChapterTypes.XML;
                            }
                        }
                        else if (line.StartsWith("Output Directory:"))
                        {
                            try
                            {
                                _OutputDirectory = line.Substring(line.IndexOf(":") + 1);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Output Directory! {0}", ex.Message));
                                _OutputDirectory = "";
                            }
                        }
                        else if (line.StartsWith("Default Output Directory:"))
                        {
                            try
                            {
                                _DefaultOutputDirectory = line.Substring(line.IndexOf(":") + 1);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Default Output Directory! {0}", ex.Message));
                                _DefaultOutputDirectory = "";
                            }
                        }
                        else if (line.StartsWith("Lock Output Directory:"))
                        {
                            try
                            {
                                _LockedOutputDirectory = bool.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Lock Output Directory! {0}", ex.Message));
                                _LockedOutputDirectory = false;
                            }
                        }
                        else if (line.StartsWith("Initial Window Position X:"))
                        {
                            try
                            {
                                _WindowPosX = int.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Initial Window Position X! {0}", ex.Message));
                                _WindowPosX = 0;
                            }
                        }
                        else if (line.StartsWith("Initial Window Position Y:"))
                        {
                            try
                            {
                                _WindowPosY = int.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Initial Window Position Y! {0}", ex.Message));
                                _WindowPosY = 0;
                            }
                        }
                        else if (line.StartsWith("Initial Window Size Width:"))
                        {
                            try
                            {
                                _WindowSizeWidth = int.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Initial Window Size Width! {0}", ex.Message));
                                _WindowSizeWidth = 640;
                            }
                        }
                        else if (line.StartsWith("Initial Window Size Height:"))
                        {
                            try
                            {
                                _WindowSizeHeight = int.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Initial Window Size Height! {0}", ex.Message));
                                _WindowSizeHeight = 600;
                            }
                        }
                        else if (line.StartsWith("Job Mode:"))
                        {
                            try
                            {
                                _JobMode = bool.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Job Mode! {0}", ex.Message));
                                _JobMode = false;
                            }
                        }
                        else if (line.StartsWith("Window State:"))
                        {
                            try
                            {
                                _WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), line.Substring(line.IndexOf(":") + 1), true);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Window State! {0}", ex.Message));
                                _WindowState = FormWindowState.Normal;
                            }
                        }
                        else if (line.StartsWith("Show Popup:"))
                        {
                            try
                            {
                                _ShowPopup = bool.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Show Popup! {0}", ex.Message));
                                _ShowPopup = true;
                            }
                        }
                        else if (line.StartsWith("Show Popup In Job Manager:"))
                        {
                            try
                            {
                                _ShowPopupInJobManager = bool.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Show Popup In Job Manager! {0}", ex.Message));
                                _ShowPopupInJobManager = true;
                            }
                        }
                        else if (line.StartsWith("VideoTrackFilenamePattern:"))
                        {
                            try
                            {
                                string tmp = line.Substring(line.IndexOf(":") + 1);
                                if (!string.IsNullOrWhiteSpace(tmp))
                                {
                                    _VideoTrackFilenamePattern = tmp;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading VideoTrackFilenamePattern! {0}", ex.Message));
                                _VideoTrackFilenamePattern = "";
                            }
                        }
                        else if (line.StartsWith("AudioTrackFilenamePattern:"))
                        {
                            try
                            {
                                string tmp = line.Substring(line.IndexOf(":") + 1);
                                if (!string.IsNullOrWhiteSpace(tmp))
                                {
                                    _AudioTrackFilenamePattern = tmp;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading AudioTrackFilenamePattern! {0}", ex.Message));
                                _AudioTrackFilenamePattern = "";
                            }
                        }
                        else if (line.StartsWith("SubtitleTrackFilenamePattern:"))
                        {
                            try
                            {
                                string tmp = line.Substring(line.IndexOf(":") + 1);
                                if (!string.IsNullOrWhiteSpace(tmp))
                                {
                                    _SubtitleTrackFilenamePattern = tmp;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading SubtitleTrackFilenamePattern! {0}", ex.Message));
                                _SubtitleTrackFilenamePattern = "";
                            }
                        }
                        else if (line.StartsWith("ChapterFilenamePattern:"))
                        {
                            try
                            {
                                string tmp = line.Substring(line.IndexOf(":") + 1);
                                if (!string.IsNullOrWhiteSpace(tmp))
                                {
                                    _ChapterFilenamePattern = tmp;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading ChapterFilenamePattern! {0}", ex.Message));
                                _ChapterFilenamePattern = "";
                            }
                        }
                        else if (line.StartsWith("AttachmentFilenamePattern:"))
                        {
                            try
                            {
                                string tmp = line.Substring(line.IndexOf(":") + 1);
                                if (!string.IsNullOrWhiteSpace(tmp))
                                {
                                    _AttachmentFilenamePattern = tmp;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading AttachmentFilenamePattern! {0}", ex.Message));
                                _AttachmentFilenamePattern = "";
                            }
                        }
                        else if (line.StartsWith("Append On Drag and Drop:"))
                        {
                            try
                            {
                                _AppendOnDragAndDrop = bool.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading Append On Drag and Drop! {0}", ex.Message));
                                _AppendOnDragAndDrop = false;
                            }
                        }
                        else if (line.StartsWith("DarkMode:"))
                        {
                            try
                            {
                                _DarkMode = bool.Parse(line.Substring(line.IndexOf(":") + 1));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                gMKVLogger.Log(string.Format("Error reading DarkMode! {0}", ex.Message));
                                _DarkMode = false; // Default to false on error
                            }
                        }
                    }
                }
                gMKVLogger.Log("Finished loading settings!");
            }
        }

        public void Save()
        {
            gMKVLogger.Log("Saving settings...");
            using (StreamWriter sw = new StreamWriter(Path.Combine(_SettingsPath, _SETTINGS_FILE), false, Encoding.UTF8))
            {
                sw.WriteLine(String.Format("MKVToolnix Path:{0}", _MkvToolnixPath));
                sw.WriteLine(String.Format("Chapter Type:{0}", _ChapterType));
                sw.WriteLine(String.Format("Output Directory:{0}", _OutputDirectory));
                sw.WriteLine(String.Format("Default Output Directory:{0}", _DefaultOutputDirectory));
                sw.WriteLine(String.Format("Lock Output Directory:{0}", _LockedOutputDirectory));
                sw.WriteLine(String.Format("Initial Window Position X:{0}", _WindowPosX));
                sw.WriteLine(String.Format("Initial Window Position Y:{0}", _WindowPosY));
                sw.WriteLine(String.Format("Initial Window Size Width:{0}", _WindowSizeWidth));
                sw.WriteLine(String.Format("Initial Window Size Height:{0}", _WindowSizeHeight));
                sw.WriteLine(String.Format("Job Mode:{0}", _JobMode));
                sw.WriteLine(String.Format("Window State:{0}", _WindowState.ToString()));
                sw.WriteLine(String.Format("Show Popup:{0}", _ShowPopup));
                sw.WriteLine(String.Format("Show Popup In Job Manager:{0}", _ShowPopupInJobManager));
                sw.WriteLine(String.Format("Append On Drag and Drop:{0}", _AppendOnDragAndDrop));
                sw.WriteLine(String.Format("DarkMode:{0}", _DarkMode));

                sw.WriteLine(string.Format("VideoTrackFilenamePattern:{0}", _VideoTrackFilenamePattern));
                sw.WriteLine(string.Format("AudioTrackFilenamePattern:{0}", _AudioTrackFilenamePattern));
                sw.WriteLine(string.Format("SubtitleTrackFilenamePattern:{0}", _SubtitleTrackFilenamePattern));
                sw.WriteLine(string.Format("ChapterFilenamePattern:{0}", _ChapterFilenamePattern));
                sw.WriteLine(string.Format("AttachmentFilenamePattern:{0}", _AttachmentFilenamePattern));
            }
        }
    }
}
