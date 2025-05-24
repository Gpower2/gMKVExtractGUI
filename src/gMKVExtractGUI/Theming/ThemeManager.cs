using System;
using System.Drawing;
using System.Windows.Forms;
using gMKVToolNix.Controls; // For custom gControls

namespace gMKVToolNix.Theming
{
    public static class ThemeManager
    {
        // Define Light and Dark Colors
        // Basic Colors
        public static Color LightModeFormBackColor { get; set; } = SystemColors.Control;
        public static Color LightModeFormForeColor { get; set; } = SystemColors.ControlText;
        public static Color LightModeContainerBackColor { get; set; } = SystemColors.Control;
        public static Color LightModeContainerForeColor { get; set; } = SystemColors.ControlText;
        public static Color LightModeTextBackColor { get; set; } = SystemColors.Window;
        public static Color LightModeTextForeColor { get; set; } = SystemColors.WindowText;
        public static Color LightModeButtonBackColor { get; set; } = SystemColors.Control;
        public static Color LightModeButtonForeColor { get; set; } = SystemColors.ControlText;
        public static Color LightModeMenuBackColor { get; set; } = SystemColors.Control; // Or SystemColors.MenuBar
        public static Color LightModeMenuForeColor { get; set; } = SystemColors.MenuText;
        public static Color LightModeGridBackColor { get; set; } = SystemColors.ControlDark; // Background of the DGV control itself
        public static Color LightModeGridCellBackColor { get; set; } = SystemColors.Window; // Cell background
        public static Color LightModeGridHeaderBackColor { get; set; } = SystemColors.Control; // Header background
        // LightModeGridForeColor will be set per cell type in ApplyTheme


        public static Color DarkModeFormBackColor { get; set; } = Color.FromArgb(45, 45, 48);
        public static Color DarkModeFormForeColor { get; set; } = Color.White;
        public static Color DarkModeContainerBackColor { get; set; } = Color.FromArgb(45, 45, 48); // For GroupBox, Panel, TabControl
        public static Color DarkModeContainerForeColor { get; set; } = Color.White;
        public static Color DarkModeTextBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public static Color DarkModeTextForeColor { get; set; } = Color.White;
        public static Color DarkModeButtonBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public static Color DarkModeButtonForeColor { get; set; } = Color.White;
        public static Color DarkModeMenuBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public static Color DarkModeMenuForeColor { get; set; } = Color.White;
        public static Color DarkModeGridBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public static Color DarkModeGridCellBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public static Color DarkModeGridHeaderBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public static Color DarkModeGridForeColor { get; set; } = Color.White; // General text for grid (headers) in dark mode


        public static void ApplyTheme(Control control, bool darkMode)
        {
            Color formBackColor = darkMode ? DarkModeFormBackColor : LightModeFormBackColor;
            Color formForeColor = darkMode ? DarkModeFormForeColor : LightModeFormForeColor;
            Color containerBackColor = darkMode ? DarkModeContainerBackColor : LightModeContainerBackColor;
            Color containerForeColor = darkMode ? DarkModeContainerForeColor : LightModeContainerForeColor;
            Color textBackColor = darkMode ? DarkModeTextBackColor : LightModeTextBackColor;
            Color textForeColor = darkMode ? DarkModeTextForeColor : LightModeTextForeColor;
            // Button colors are handled within the Button specific block
            Color menuBackColor = darkMode ? DarkModeMenuBackColor : LightModeMenuBackColor;
            Color menuForeColor = darkMode ? DarkModeMenuForeColor : LightModeMenuForeColor;


            if (control is Form || control is gForm)
            {
                control.BackColor = formBackColor;
                control.ForeColor = formForeColor;
            }
            else if (control is GroupBox || control is Panel || control is TabControl || control is gGroupBox || control is gTableLayoutPanel)
            {
                control.BackColor = containerBackColor;
                control.ForeColor = containerForeColor; // This sets the default for child controls that inherit

                if (darkMode && !control.Enabled)
                {
                    // Re-assert ForeColor for the container itself (may affect GroupBox title if lucky, but often doesn't)
                    control.ForeColor = DarkModeContainerForeColor;

                    // Explicitly set ForeColor for common child text controls
                    foreach (Control child in control.Controls)
                    {
                        if (child is Label || child is CheckBox || child is RadioButton) // Add other relevant types if needed
                        {
                            // Forcing ForeColor on children of a disabled container.
                            // Standard behavior is for child.Enabled to also be false, 
                            // and they'd typically render with a system disabled text color.
                            // This override attempts to keep them white.
                            child.ForeColor = DarkModeContainerForeColor;
                        }
                        // Potentially recurse if there are nested containers, though ApplyTheme handles this by iterating all controls.
                        // However, a direct recursive call here for children of a disabled parent might be more targeted.
                        // For now, let's stick to immediate children.
                    }
                }
            }
            else if (control is TextBox || control is RichTextBox || control is gTextBox || control is gRichTextBox)
            {
                control.BackColor = textBackColor;
                control.ForeColor = textForeColor;

                if (control is RichTextBox rich)
                {
                    // For RichTextBox, ensure the selection colors are set correctly
                    if (darkMode)
                    {
                        rich.BorderStyle = BorderStyle.FixedSingle;
                        rich.SelectionBackColor = Color.FromArgb(80, 80, 80); // Dark selection background
                        rich.SelectionColor = Color.White; // White text on dark selection
                    }
                    else
                    {
                        rich.BorderStyle = BorderStyle.Fixed3D;
                        rich.SelectionBackColor = SystemColors.Highlight; // Standard highlight color
                        rich.SelectionColor = SystemColors.HighlightText; // Standard highlight text color
                    }
                }
            }
            else if (control is Button btn)
            {
                if (darkMode)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.DarkGray;
                    btn.BackColor = DarkModeButtonBackColor;
                    btn.ForeColor = DarkModeButtonForeColor;
                }
                else
                {
                    btn.FlatStyle = FlatStyle.Standard;
                    btn.UseVisualStyleBackColor = true;
                    btn.ForeColor = LightModeButtonForeColor;
                    // Explicitly clear any custom border color for light mode standard buttons
                    // or set to a system default if absolutely necessary, but usually not needed for standard.
                    btn.FlatAppearance.BorderColor = SystemColors.ControlDark; // Or remove this line
                }
            }
            else if (control is CheckBox chk)
            {
                if (darkMode)
                {
                    chk.BackColor = DarkModeContainerBackColor;
                    chk.ForeColor = DarkModeContainerForeColor;
                }
                else
                {
                    chk.BackColor = LightModeContainerBackColor;
                    chk.ForeColor = LightModeContainerForeColor;
                }
            }
            else if (control is RadioButton rdo)
            {
                if (darkMode)
                {
                    rdo.BackColor = DarkModeContainerBackColor;
                    rdo.ForeColor = DarkModeContainerForeColor;
                }
                else
                {
                    rdo.BackColor = LightModeContainerBackColor;
                    rdo.ForeColor = LightModeContainerForeColor;
                }
            }
            else if (control is ComboBox cb)
            {
                // Existing ComboBox theming logic (user's sequence from previous step)
                ComboBoxStyle originalStyle = cb.DropDownStyle;
                try
                {
                    cb.DropDownStyle = ComboBoxStyle.Simple;
                    if (darkMode)
                    {
                        cb.FlatStyle = FlatStyle.Flat;
                        cb.BackColor = DarkModeTextBackColor;
                        cb.ForeColor = DarkModeFormForeColor; // Using DarkModeFormForeColor for text
                    }
                    else // Light Mode
                    {
                        cb.FlatStyle = FlatStyle.Standard;
                        cb.BackColor = SystemColors.Window;
                        cb.ForeColor = SystemColors.ControlText;
                    }
                    cb.Invalidate();
                }
                finally
                {
                    cb.DropDownStyle = originalStyle;
                }

                // New logic for ComboBox ContextMenuStrip (from user snippets)
                if (cb is gMKVToolNix.Controls.gComboBox gcmb && gcmb.ContextMenuStrip != null)
                {
                    if (darkMode)
                    {
                        gcmb.ContextMenuStrip.BackColor = DarkModeButtonBackColor; // Use ThemeManager's color
                        gcmb.ContextMenuStrip.ForeColor = DarkModeFormForeColor;   // Use ThemeManager's color for text
                        gcmb.ContextMenuStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode; // Ensure consistency
                        foreach (ToolStripItem item in gcmb.ContextMenuStrip.Items)
                        {
                            ApplyToolStripItemThemeForComboBox(item, darkMode); // Use a dedicated helper
                        }
                    }
                    else // Light Mode for ComboBox ContextMenuStrip
                    {
                        gcmb.ContextMenuStrip.BackColor = SystemColors.ControlLightLight; // User snippet
                        gcmb.ContextMenuStrip.ForeColor = SystemColors.ControlText;       // User snippet
                        gcmb.ContextMenuStrip.RenderMode = ToolStripRenderMode.System; // User snippet implies system default look
                        foreach (ToolStripItem item in gcmb.ContextMenuStrip.Items)
                        {
                            ApplyToolStripItemThemeForComboBox(item, darkMode); // Use a dedicated helper
                        }
                    }
                }
            }
            else if (control is ListBox lb)
            {
                control.BackColor = textBackColor;
                control.ForeColor = textForeColor;
            }
            else if (control is TreeView tv)
            {
                tv.BackColor = textBackColor;
                tv.ForeColor = textForeColor;
                tv.BorderStyle = BorderStyle.FixedSingle; // Ensure a border is drawn

                if (darkMode)
                {
                    tv.LineColor = Color.LightGray;
                }
                else
                {
                    // Try a very light gray for lines in light mode
                    // SystemColors.ControlLight can sometimes be too subtle or vary too much.
                    // A specific light gray might be more reliable.
                    tv.LineColor = Color.FromArgb(220, 220, 220); // A light gray, less aggressive than ControlDark or GrayText
                }
            }
            else if (control is DataGridView dgv)
            {
                dgv.BackgroundColor = darkMode ? DarkModeGridBackColor : SystemColors.Window; // Changed for light mode
                dgv.GridColor = darkMode ? Color.Gray : SystemColors.ControlDarkDark;

                if (darkMode)
                {
                    dgv.DefaultCellStyle.BackColor = DarkModeGridCellBackColor;
                    dgv.DefaultCellStyle.ForeColor = DarkModeGridForeColor;
                    dgv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    dgv.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

                    dgv.ColumnHeadersDefaultCellStyle.BackColor = DarkModeGridHeaderBackColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = DarkModeGridForeColor;
                    dgv.RowHeadersDefaultCellStyle.BackColor = DarkModeGridHeaderBackColor;
                    dgv.RowHeadersDefaultCellStyle.ForeColor = DarkModeGridForeColor;

                    dgv.EnableHeadersVisualStyles = false;
                }
                else
                {
                    dgv.DefaultCellStyle.BackColor = LightModeGridCellBackColor;
                    dgv.DefaultCellStyle.ForeColor = SystemColors.WindowText;
                    dgv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    dgv.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
                    dgv.RowHeadersDefaultCellStyle.BackColor = LightModeGridHeaderBackColor;
                    dgv.RowHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;

                    dgv.EnableHeadersVisualStyles = true;
                }

                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
                dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
                dgv.RowHeadersDefaultCellStyle.SelectionBackColor = dgv.RowHeadersDefaultCellStyle.BackColor;
                dgv.RowHeadersDefaultCellStyle.SelectionForeColor = dgv.RowHeadersDefaultCellStyle.ForeColor;
            }
            else if (control is MenuStrip ms)
            {
                ms.BackColor = menuBackColor;
                ms.ForeColor = menuForeColor;
                foreach (ToolStripItem item in ms.Items)
                {
                    ApplyToolStripItemTheme(item, darkMode);
                }
            }
            else if (control is ContextMenuStrip cms)
            {
                if (darkMode)
                {
                    cms.RenderMode = ToolStripRenderMode.ManagerRenderMode; // Keep for potential custom dark renderer later
                    cms.BackColor = DarkModeMenuBackColor;
                    cms.ForeColor = DarkModeMenuForeColor;
                }
                else // Light Mode
                {
                    cms.RenderMode = ToolStripRenderMode.Professional;
                    cms.BackColor = SystemColors.ControlLightLight;
                    cms.ForeColor = SystemColors.ControlText;
                }
                // Apply to items regardless of mode, ApplyToolStripItemTheme will handle specifics
                foreach (ToolStripItem item in cms.Items)
                {
                    ApplyToolStripItemTheme(item, darkMode);
                }
            }
            else if (control is StatusStrip ss)
            {
                // menuBackColor and menuForeColor are defined at the start of ApplyTheme
                // menuBackColor is DarkModeMenuBackColor or LightModeMenuBackColor (e.g. SystemColors.Control)
                // menuForeColor is DarkModeMenuForeColor or LightModeMenuForeColor (e.g. SystemColors.ControlText or MenuText)
                ss.BackColor = menuBackColor;
                ss.ForeColor = menuForeColor;
                // For StatusStrip, System RenderMode is often best for light mode OS integration
                ss.RenderMode = darkMode ? ToolStripRenderMode.ManagerRenderMode : ToolStripRenderMode.System;

                foreach (ToolStripItem item in ss.Items)
                {
                    // ApplyToolStripItemTheme will handle item-specific appearances
                    ApplyToolStripItemTheme(item, darkMode);
                }
            }
            else if (control is ToolStrip ts)
            {
                ts.BackColor = menuBackColor;
                ts.ForeColor = menuForeColor;
                foreach (ToolStripItem item in ts.Items)
                {
                    ApplyToolStripItemTheme(item, darkMode);
                }
            }
            else if (control is Label)
            {
                control.BackColor = Color.Transparent;
                control.ForeColor = darkMode ? DarkModeContainerForeColor : LightModeContainerForeColor;
            }
            else if (control is ProgressBar pb)
            {
                // Only ForeColor should be set, BackColor is usually system drawn for the track
                pb.ForeColor = darkMode ? Color.FromArgb(0, 122, 204) : SystemColors.Highlight;
            }
            // For other controls, apply general container styling if no specific styling is applied
            else if (control.HasChildren && !(control is Form || control is gForm || control is GroupBox || control is Panel || control is TabControl || control is gGroupBox || control is gTableLayoutPanel))
            {
                control.BackColor = containerBackColor;
                control.ForeColor = containerForeColor;
            }


            foreach (Control childControl in control.Controls)
            {
                ApplyTheme(childControl, darkMode);
            }
        }

        public static void ApplyToolStripItemTheme(ToolStripItem item, bool darkMode)
        {
            if (darkMode)
            {
                item.BackColor = DarkModeMenuBackColor;
                item.ForeColor = DarkModeMenuForeColor;
                // For ToolStripStatusLabel in dark mode, it should also get the dark menu color.
                // No special handling needed here unless it looked wrong.
            }
            else // Light Mode
            {
                if (item is ToolStripStatusLabel statusLabel)
                {
                    statusLabel.BackColor = Color.Transparent; // Make ToolStripStatusLabel transparent
                    statusLabel.ForeColor = SystemColors.ControlText; // Standard text color for status bars
                }
                else // For other items like ToolStripMenuItem in ContextMenus
                {
                    item.BackColor = SystemColors.ControlLightLight; // Keep for "beautiful" context menus
                    item.ForeColor = SystemColors.ControlText;
                }
            }

            // Handle dropdowns for ToolStripMenuItems (dropdowns are usually on ContextMenus, not StatusStrips)
            if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
            {
                if (darkMode)
                {
                    menuItem.DropDown.BackColor = DarkModeMenuBackColor;
                }
                else // Light Mode for dropdowns of ToolStripMenuItems
                {
                    // Dropdowns of context menu items should also match the ControlLightLight theme
                    menuItem.DropDown.BackColor = SystemColors.ControlLightLight;
                }
                foreach (ToolStripItem dropDownItem in menuItem.DropDownItems)
                {
                    ApplyToolStripItemTheme(dropDownItem, darkMode); // Recursive call
                }
            }
            // No specific DropDown handling needed for ToolStripStatusLabel as it doesn't have dropdowns.
            // ToolStripDropDownItem is for general dropdowns in ToolStrips, less common in StatusStrip.
            // If general ToolStripDropDownButtons or ToolStripSplitButtons are on the StatusStrip,
            // their dropdowns might need explicit theming if they don't inherit correctly.
            // For now, this focuses on ToolStripStatusLabel and ToolStripMenuItem.
        }

        // New helper method within ThemeManager class
        private static void ApplyToolStripItemThemeForComboBox(ToolStripItem item, bool darkMode)
        {
            if (darkMode)
            {
                item.BackColor = DarkModeButtonBackColor; // From user snippet mapping
                item.ForeColor = DarkModeFormForeColor;   // From user snippet mapping
            }
            else // Light Mode
            {
                item.BackColor = SystemColors.ControlLightLight; // From user snippet
                item.ForeColor = SystemColors.ControlText;       // From user snippet
            }

            if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems) // Corrected to HasDropDownItems
            {
                // For dropdowns of menu items, apply recursively
                // The DropDown is a ToolStripDropDownMenu which is a kind of ContextMenuStrip
                if (darkMode)
                {
                    menuItem.DropDown.BackColor = DarkModeButtonBackColor; // Match item color
                }
                else
                {
                    menuItem.DropDown.BackColor = SystemColors.ControlLightLight; // Match item color
                }
                foreach (ToolStripItem dropDownItem in menuItem.DropDownItems)
                {
                    ApplyToolStripItemThemeForComboBox(dropDownItem, darkMode); // Recursive call
                }
            }
        }
    }
}
