using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using gMKVToolNix.Localization;

namespace gMKVToolNix
{
    public class gForm : Form
    {
        public static short LOWORD(int number)
        {
            return (short)number;
        }


        /// <summary>
        /// Gets the form's border width in pixels
        /// </summary>
        public int BorderWidth
        {
            get { return Convert.ToInt32((double)(this.Width - this.ClientSize.Width) / 2.0); }
        }

        /// <summary>
        /// Gets the form's Title Bar Height in pixels
        /// </summary>
        public int TitlebarHeight
        {
            get { return this.Height - this.ClientSize.Height - 2 * BorderWidth; }
        }

        public gForm() :base()
        {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected string GetLocalizedString(string key, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                return LocalizationManager.GetString(key, args);
            }

            return LocalizationManager.GetString(key);
        }

        protected Exception CreateLocalizedException(string key, params object[] args)
        {
            return new Exception(GetLocalizedString(key, args));
        }

        protected void ShowLocalizedSuccessMessage(string messageKey, bool dialogOnTop = false, params object[] args)
        {
            ShowSuccessMessage(GetLocalizedString(messageKey, args), dialogOnTop);
        }

        protected void ShowLocalizedErrorMessage(string messageKey, bool dialogOnTop = false, params object[] args)
        {
            ShowErrorMessage(GetLocalizedString(messageKey, args), dialogOnTop);
        }

        protected DialogResult ShowLocalizedQuestion(string messageKey, string titleKey, bool showCancel = true, params object[] args)
        {
            return ShowQuestion(GetLocalizedString(messageKey, args), GetLocalizedString(titleKey), showCancel);
        }


        /// <summary>
        /// Returns the full path and filename of the executing assembly
        /// </summary>
        /// <returns></returns>
        protected string GetExecutingAssemblyLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// Returns the current directory of the executing assembly
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Returns the version of the executing assembly
        /// </summary>
        /// <returns></returns>
        protected Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        protected void ShowErrorMessage(string argMessage, bool dialogOnTop = false)
        {
            string dialogMessage = LocalizationManager.GetString("UI.Common.Dialog.ErrorMessage", Environment.NewLine, argMessage);
            string dialogTitle = LocalizationManager.GetString("UI.Common.Dialog.ErrorTitle");

            if (dialogOnTop)
            {
                // Create a dummy form that is on top of all other windows in desktop
                using (Form form = new Form { TopMost = true })
                {
                    MessageBox.Show(
                        form,
                        dialogMessage,
                        dialogTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(
                    this,
                    dialogMessage,
                    dialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Return to the original form
            this.BringToFront();
            this.Activate();
            this.Show();
        }

        protected void ShowSuccessMessage(string argMessage, bool dialogOnTop = false)
        {
            string dialogTitle = LocalizationManager.GetString("UI.Common.Dialog.SuccessTitle");

            if (dialogOnTop)
            {
                // Create a dummy form that is on top of all other windows in desktop
                using (Form form = new Form { TopMost = true })
                {
                    MessageBox.Show(
                        form,
                        argMessage,
                        dialogTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            else 
            {
                MessageBox.Show(
                    this,
                    argMessage,
                    dialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            // Return to the original form
            this.BringToFront();
            this.Activate();
            this.Show();
        }

        protected DialogResult ShowQuestion(string argQuestion, string argTitle, bool argShowCancel = true)
        {
            MessageBoxButtons msgBoxBtns = MessageBoxButtons.YesNoCancel;
            if (!argShowCancel)
            {
                msgBoxBtns = MessageBoxButtons.YesNo;
            }

            return MessageBox.Show(
                this, 
                argQuestion, 
                argTitle, 
                msgBoxBtns, 
                MessageBoxIcon.Question);
        }

        protected void ToggleControls(Control argRootControl, bool argStatus)
        {
            foreach (Control ctrl in argRootControl.Controls)
            {
                if (ctrl is IContainer)
                {
                    ToggleControls(ctrl, argStatus);
                }
                else
                {
                    ctrl.Enabled = argStatus;
                }
            }
        }
    }
}
