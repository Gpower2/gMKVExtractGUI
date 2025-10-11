using System;
using System.Diagnostics;
using System.Windows.Forms;
using gMKVToolNix.Controls;

namespace gMKVToolNix
{
    public class gTextBox : TextBox
    {
        public gTextBox()
            : base()
        {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.A))
                {
                    this.SelectAll();
                }
                else if (keyData == (Keys.Control | Keys.C))
                {
                    if (!string.IsNullOrWhiteSpace(this.SelectedText))
                    {
                        Clipboard.SetText(this.SelectedText, TextDataFormat.UnicodeText);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ex.ShowException(this.FindForm());
            }

            return base.ProcessCmdKey(ref m, keyData);
        }
    }
}
