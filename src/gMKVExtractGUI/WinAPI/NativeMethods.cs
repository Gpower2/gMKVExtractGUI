using System;
using System.Runtime.InteropServices;

namespace gMKVToolNix.WinAPI // Or any appropriate namespace
{
    public static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);

        public enum DWMWINDOWATTRIBUTE : int
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20, // For Windows 10 18985+ and Windows 11
            DWMWA_USE_IMMERSIVE_DARK_MODE_PRE_20H1 = 19, // For Windows 10 17763 to 18985 (optional to support both)
        }

        /// <summary>
        /// Helper method to attempt setting the dark mode
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="enabled"></param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool TrySetImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (PlatformExtensions.IsOnLinux)
            {
                return true;
            }

            try
            {
                int darkModeValue = enabled ? 1 : 0;
                // First, try with attribute 20 (DWMWA_USE_IMMERSIVE_DARK_MODE)
                int result = DwmSetWindowAttribute(
                    handle, 
                    DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, 
                    ref darkModeValue, 
                    Marshal.SizeOf(typeof(int)));

                // if the above fails and you want to try the older attribute value (19)
                if (result != 0) // 0 means S_OK (success)
                {
                    result = DwmSetWindowAttribute(
                        handle, 
                        DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_PRE_20H1, 
                        ref darkModeValue, 
                        Marshal.SizeOf(typeof(int)));
                }

                return result == 0;
            }
            catch (Exception)
            {
                // This can happen if dwmapi.dll is not found or the attribute is not supported.
                return false;
            }
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        /// <summary>
        /// Sets the window theme to dark mode or light mode based on the provided boolean value.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="darkModeEnabled"></param>
        /// <param name="pszSubIdList"></param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool SetWindowThemeManaged(IntPtr hWnd, bool darkModeEnabled, string pszSubIdList = null)        {
            string mode = darkModeEnabled ? "DarkMode_Explorer" : "ClearMode_Explorer";
            return SetWindowThemeInternal(hWnd, mode, pszSubIdList);
        }

        /// <summary>
        /// Sets the window theme to dark mode or light mode based on the provided boolean value.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="darkModeEnabled"></param>
        /// <param name="pszSubIdList"></param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool SetWindowThemeForComboBoxManaged(IntPtr hWnd, bool darkModeEnabled, string pszSubIdList = null)        {
            string mode = darkModeEnabled ? "DarkMode_CFD" : "ClearMode_CFD";
            return SetWindowThemeInternal(hWnd, mode, pszSubIdList);
        }

        private static bool SetWindowThemeInternal(IntPtr hWnd, string mode, string pszSubIdList = null)
        {
            if (PlatformExtensions.IsOnLinux)
            {
                return true;
            }

            int result = SetWindowTheme(hWnd, mode, pszSubIdList);

            return result == 0;
        }
    }
}
