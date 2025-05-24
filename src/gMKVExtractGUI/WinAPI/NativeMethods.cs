using System;
using System.Runtime.InteropServices;

namespace gMKVToolNix.WinAPI // Or any appropriate namespace
{
    public static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);

        public enum DWMWINDOWATTRIBUTE : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20, // For Windows 10 18985+ and Windows 11
            DWMWA_USE_IMMERSIVE_DARK_MODE_PRE_20H1 = 19, // For Windows 10 17763 to 18985 (optional to support both)
        }

        // Helper method to attempt setting the dark mode
        // Returns true if successful, false otherwise.
        public static bool TrySetImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            try
            {
                int useDarkMode = enabled ? 1 : 0;
                // First, try with attribute 20 (DWMWA_USE_IMMERSIVE_DARK_MODE)
                int result = DwmSetWindowAttribute(handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));

                // if the above fails and you want to try the older attribute value (19)
                if (result != 0) // 0 means S_OK (success)
                {
                    result = DwmSetWindowAttribute(handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_PRE_20H1, ref useDarkMode, sizeof(int));
                }

                return result == 0;
            }
            catch (Exception)
            {
                // This can happen if dwmapi.dll is not found or the attribute is not supported.
                return false;
            }
        }
    }
}
