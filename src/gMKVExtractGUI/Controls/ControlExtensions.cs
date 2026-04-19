using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace gMKVToolNix.Controls
{
    public static class ControlExtensions
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

        private const int WM_SETREDRAW = 11;

        /// <summary>
        /// Suspends ALL drawing for the specified control
        /// </summary>
        /// <param name="parent"></param>
        public static void SuspendDrawing(this Control parent)
        {
            // If we are on Linux, we can't use P/Invoke to user32.dll
            // So this function can't do anything
            if (PlatformExtensions.IsOnLinux) return;

            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        /// <summary>
        /// Resumes ALL drawing for the specified control
        /// </summary>
        /// <param name="parent"></param>
        public static void ResumeDrawing(this Control parent)
        {
            // If we are on Linux, we can't use P/Invoke to user32.dll
            if (!PlatformExtensions.IsOnLinux)
            {
                SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            }
            parent.Invalidate(true);
        }

        private static readonly BindingFlags _designModeBindFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

        private static bool GetDesignMode(this Control control)
        {
            PropertyInfo prop = control.GetType().GetProperty("DesignMode", _designModeBindFlags);
            return (bool)prop.GetValue(control, null);
        }

        /// <summary>
        /// Returns true if the control is currently in design mode, false otherwise
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsInDesignMode(this Control control)
        {
            Control parent = control.Parent;
            while (parent != null)
            {
                if (parent.GetDesignMode())
                {
                    return true;
                }
                parent = parent.Parent;
            }
            return control.GetDesignMode();
        }

        public static int GetPreferredWidth(this Control control, int minimumWidth = 0, int extraPadding = 0)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            int preferredWidth = control.GetPreferredSize(Size.Empty).Width + extraPadding;
            return Math.Max(minimumWidth, preferredWidth);
        }

        public static void ApplyLocalizedButtonSize(this Button button, int minimumWidth, int minimumHeight = 30, int extraPadding = 12)
        {
            if (button == null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            button.AutoSize = false;
            string buttonText = string.IsNullOrWhiteSpace(button.Text) ? " " : button.Text;
            Size textSize = TextRenderer.MeasureText(
                buttonText,
                button.Font,
                Size.Empty,
                TextFormatFlags.SingleLine | TextFormatFlags.LeftAndRightPadding);

            int preferredWidth = textSize.Width + button.Padding.Horizontal + extraPadding;
            int preferredHeight = textSize.Height + button.Padding.Vertical + 8;
            button.Size = new Size(Math.Max(minimumWidth, preferredWidth), Math.Max(minimumHeight, preferredHeight));
        }

        public static void ApplyLocalizedButtonSize(this Button button, Size minimumSize, int extraPadding = 12)
        {
            if (button == null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            ApplyLocalizedButtonSize(button, minimumSize.Width, minimumSize.Height, extraPadding);
        }
    }
}
