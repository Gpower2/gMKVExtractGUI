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

            float scaleFactor = button.GetCurrentScaleFactor();
            int scaledMinimumWidth = (int)Math.Ceiling(minimumWidth * scaleFactor);
            int scaledMinimumHeight = (int)Math.Ceiling(minimumHeight * scaleFactor);
            int scaledExtraPadding = (int)Math.Ceiling(extraPadding * scaleFactor);
            int scaledVerticalPadding = (int)Math.Ceiling(8 * scaleFactor);

            button.AutoSize = false;
            string buttonText = string.IsNullOrWhiteSpace(button.Text) ? " " : button.Text;
            Size textSize = TextRenderer.MeasureText(
                buttonText,
                button.Font,
                Size.Empty,
                TextFormatFlags.SingleLine | TextFormatFlags.LeftAndRightPadding);

            int preferredWidth = textSize.Width + button.Padding.Horizontal + scaledExtraPadding;
            int preferredHeight = textSize.Height + button.Padding.Vertical + scaledVerticalPadding;
            button.Size = new Size(Math.Max(scaledMinimumWidth, preferredWidth), Math.Max(scaledMinimumHeight, preferredHeight));
        }

        public static void ApplyLocalizedButtonSize(this Button button, Size minimumSize, int extraPadding = 12)
        {
            if (button == null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            ApplyLocalizedButtonSize(button, minimumSize.Width, minimumSize.Height, extraPadding);
        }

        public static int GetGroupBoxContentTop(this GroupBox groupBox, int fallbackTop = 19)
        {
            if (groupBox == null)
            {
                throw new ArgumentNullException(nameof(groupBox));
            }

            int contentTop = groupBox.DisplayRectangle.Top;
            return contentTop > 0 ? contentTop : fallbackTop;
        }

        public static int GetVerticallyCenteredTop(this Control control, int referenceTop, int referenceHeight)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            return referenceTop + ((referenceHeight - control.Height) / 2);
        }

        public static float GetCurrentScaleFactor(this Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            try
            {
                using (Graphics graphics = control.CreateGraphics())
                {
                    return Math.Max(1F, graphics.DpiX / 96F);
                }
            }
            catch
            {
                return 1F;
            }
        }
    }
}
