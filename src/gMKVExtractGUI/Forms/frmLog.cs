using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using gMKVToolNix;
using System.Diagnostics;
using System.IO;

namespace gMKVToolNix
{
    public partial class frmLog : gForm
    {
        public frmLog()
        {
            InitializeComponent();
            InitForm();

            // Initialize the DPI aware scaling
            InitDPI();
        }

        private void InitForm()
        {
            Icon = Icon.ExtractAssociatedIcon(GetExecutingAssemblyLocation());
            Text = string.Format("gMKVExtractGUI v{0} -- Log", GetCurrentVersion());
        }

        private void frmLog_Activated(object sender, EventArgs e)
        {
            txtLog.Text = gMKVLogger.LogText;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.Select(txtLog.TextLength + 1, 0);
            txtLog.ScrollToCaret();
            grpLog.Text = string.Format("Log ({0})", txtLog.Lines.LongLength);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtLog.SelectedText))
                {
                    Clipboard.SetData(DataFormats.UnicodeText, txtLog.SelectedText);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                txtLog.Text = gMKVLogger.LogText;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            // To avoid getting disposed
            e.Cancel = true;
            this.Hide();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (ShowQuestion("Are you sure you want to clear the log?", "Are you sure?") == DialogResult.Yes)
                {
                    gMKVLogger.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Select filename for log...";
                sfd.CheckFileExists = true;
                sfd.DefaultExt = "txt";
                sfd.Filter = "*.txt|*.txt";
                sfd.FileName = string.Format("[{0}][{1}][gMKVExtractGUI_v{2}].txt", 
                    DateTime.Now.ToString("yyyy-MM-dd"),
                    DateTime.Now.ToString("HH-mm-ss"),
                    Assembly.GetExecutingAssembly().GetName().Version);
                if(sfd.ShowDialog() == DialogResult.Yes)
                {
                    using(StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        sw.Write(gMKVLogger.LogText);
                    }
                    ShowSuccessMessage(string.Format("The log was saved to {0}!", sfd.FileName));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }
    }
}
