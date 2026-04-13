using System;
using System.Diagnostics;
using System.Windows.Forms;
using gMKVToolNix.Localization;

namespace gMKVToolNix.Controls
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Displays a messagebox containing the message of the exception and aldo writes the exception stacktrace to the Debug console
        /// </summary>
        /// <param name="ex"></param>
        public static void ShowException(this Exception ex, Form form)
        {
            Debug.WriteLine(ex);
            MessageBox.Show(
                form,
                LocalizationManager.GetString("UI.Common.Dialog.ExceptionMessage", Environment.NewLine, ex.Message), 
                LocalizationManager.GetString("UI.Common.Dialog.ExceptionTitle"), 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error);
        }
    }
}
