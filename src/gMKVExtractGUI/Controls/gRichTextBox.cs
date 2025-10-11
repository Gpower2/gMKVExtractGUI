using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using gMKVToolNix.Controls;

namespace gMKVToolNix
{
    public class gRichTextBox : RichTextBox
    {
        public bool DarkMode { get; set; } = false;

        public gRichTextBox()
            : base()
        {
            this.DoubleBuffered = true;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.ShortcutsEnabled = false;
            this.DetectUrls = false;            
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.A))
                {
                    this.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ex.ShowException(this.FindForm());
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            // If we are on Linux, we can't use P/Invoke to user32.dll
            // So this function can't do anything
            if (PlatformExtensions.IsOnLinux)
            {
                base.WndProc(ref m); 
                return;
            }

            const int WM_PAINT = 0x000F;
            const int WM_PRINTCLIENT = 0x0318;
            const int WM_ERASEBKGND = 0x0014;

            if (DarkMode)
            {
                if (!Enabled && (m.Msg == WM_PAINT || m.Msg == WM_PRINTCLIENT))
                {
                    // Handle the paint message ourselves when disabled
                    var ps = new PAINTSTRUCT();
                    var hdc = BeginPaint(m.HWnd, ref ps);

                    using (var graphics = Graphics.FromHdc(hdc))
                    {
                        PaintDisabledRichTextBox(graphics);
                    }

                    EndPaint(m.HWnd, ref ps);
                    m.Result = IntPtr.Zero;
                    return;
                }
                else if (!Enabled && m.Msg == WM_ERASEBKGND)
                {
                    // Handle background erasing when disabled
                    using (var graphics = Graphics.FromHdc(m.WParam))
                    using (var brush = new SolidBrush(BackColor))
                    {
                        graphics.FillRectangle(brush, ClientRectangle);
                    }
                    m.Result = new IntPtr(1);
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private void PaintDisabledRichTextBox(Graphics g)
        {
            // Fill background
            using (var backBrush = new SolidBrush(BackColor))
            {
                g.FillRectangle(backBrush, ClientRectangle);
            }

            // Draw text
            if (!string.IsNullOrEmpty(Text))
            {
                TextRenderer.DrawText(g, Text, Font, ClientRectangle,
                    ForeColor, BackColor,
                    TextFormatFlags.Left | TextFormatFlags.Top |
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hwnd, ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);
    }
}
