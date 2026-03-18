using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using gMKVToolNix.Forms;
using gMKVToolNix.Localization;

namespace gMKVToolNix
{
    static class Program
    {
        /// <summary>
        /// Returns if the running Platform is Linux Or MacOSX
        /// </summary>
        public static bool IsOnLinux
        {
            get
            {
                PlatformID myPlatform = Environment.OSVersion.Platform;
                // 128 is Mono 1.x specific value for Linux systems, so it's there to provide compatibility
                return (myPlatform == PlatformID.Unix) || (myPlatform == PlatformID.MacOSX) || ((int)myPlatform == 128);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "gMKVToolNix.dll")))
            {
                MessageBox.Show(
                    "The gMKVToolNix.dll was not found! Please download and reinstall gMKVExtractGUI!", 
                    "An error has occured!", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);

                Environment.Exit(1);
            }

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Newtonsoft.Json.dll")))
            {
                MessageBox.Show(
                    "The Newtonsoft.Json.dll was not found! Please download and reinstall gMKVExtractGUI!",
                    "An error has occured!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Environment.Exit(1);
            }

            try
            {
                LocalizationManager.Initialize("en");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("Failed to initialize localization: {0}", ex.Message),
                    "Localization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Application.Run(new frmMain2());
        }
    }
}
