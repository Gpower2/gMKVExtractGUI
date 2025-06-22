using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace gMKVToolNix
{
    public static class gTaskbarProgress
    {
        public enum TaskbarStates
        {
            NoProgress = 0,
            Indeterminate = 0x1,
            Normal = 0x2,
            Error = 0x4,
            Paused = 0x8,
        }

        [ComImportAttribute()]
        [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList3
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            [PreserveSig]
            int SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
            [PreserveSig]
            int SetProgressState(IntPtr hwnd, TaskbarStates state);
            [PreserveSig]
            int SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
        }

        [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterfaceAttribute(ClassInterfaceType.None)]
        [ComImportAttribute()]
        private class TaskbarInstance { }

        private static ITaskbarList3 _TaskbarInstance = null;

        private static ITaskbarList3 Instance()
        {
            if (_TaskbarInstance == null) { _TaskbarInstance = (ITaskbarList3)new TaskbarInstance(); }
            return _TaskbarInstance;
        }

        private static bool IsTaskbarSupported 
        {
            get
            {
                return !Program.IsOnLinux && Environment.OSVersion.Version >= new Version(6, 1);
            }
        }

        public static void SetState(Form frm, TaskbarStates taskbarState)
        {
            if (IsTaskbarSupported) { Instance().SetProgressState(frm.Handle, taskbarState); }
        }

        public static void SetValue(Form frm, ulong progressValue, ulong progressMax)
        {
            if (IsTaskbarSupported) { Instance().SetProgressValue(frm.Handle, (ulong)progressValue, (ulong)progressMax); }
        }

        public static void SetOverlayIcon(Form frm, System.Drawing.Icon icn, string description)
        {
            if (IsTaskbarSupported) 
            {
                Debug.WriteLine(string.Format(
                    "{1}: HRESULT:0x{0:X8}", 
                    Instance().SetOverlayIcon(frm.Handle, icn == null ? IntPtr.Zero : icn.Handle, description ?? ""), 
                    description ?? ""));
            }
        }
    }
}
