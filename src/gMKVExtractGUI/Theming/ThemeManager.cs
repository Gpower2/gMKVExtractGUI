using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using gMKVToolNix.Controls;
using gMKVToolNix.WinAPI;

namespace gMKVToolNix.Theming
{
    public static class ThemeManager
    {
        private static readonly ToolStripRenderer LinuxDarkStatusStripRenderer = new LinuxDarkStatusStripProfessionalRenderer();

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
        public static Color DarkModeButtonBackColor { get; set; } = Color.FromArgb(60, 60, 60);
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

            if (!(control is ToolStripDropDown))
            {
                // Retheming popup menu HWNDs during Opening can corrupt native menu state.
                NativeMethods.SetWindowThemeManaged(control.Handle, darkMode);
                NativeMethods.TrySetImmersiveDarkMode(control.Handle, darkMode);
            }

            if (control is Form || control is gForm)
            {
                control.BackColor = formBackColor;
                control.ForeColor = formForeColor;
            }
            else if (control is GroupBox || control is Panel || control is TabControl || control is gGroupBox || control is gTableLayoutPanel)
            {
                control.BackColor = containerBackColor;
                control.ForeColor = containerForeColor; // This sets the default for child controls that inherit

                if (control is GroupBox)
                {
                    control.Paint -= groupBoxPaintEventHandler;
                    
                    // Only for Dark mode, since in light mode it creates an issue
                    if (darkMode)
                    {
                        control.Paint += groupBoxPaintEventHandler;
                    }
                }
            }
            else if (control is TextBox || control is RichTextBox || control is gTextBox || control is gRichTextBox)
            {
                control.BackColor = textBackColor;
                control.ForeColor = textForeColor;

                if (control is TextBox textBox)
                {
                    textBox.BorderStyle = darkMode ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
                }

                if (control is gRichTextBox gRich)
                {
                    gRich.DarkMode = darkMode; // Set the dark mode property for gRichTextBox
                }

                if (control is RichTextBox rich)
                {
                    try
                    {
                        // For RichTextBox, ensure the selection colors are set correctly
                        if (darkMode)
                        {
                            rich.BackColor = rich.Parent.BackColor;
                            rich.BorderStyle = BorderStyle.None;
                            rich.SelectionBackColor = Color.FromArgb(80, 80, 80); // Dark selection background

                            if (!PlatformExtensions.IsOnLinux)
                            {
                                rich.SelectionColor = Color.White; // White text on dark selection
                            }
                        }
                        else
                        {
                            rich.BorderStyle = BorderStyle.Fixed3D;
                            rich.SelectionBackColor = SystemColors.Highlight; // Standard highlight color
                            if (!PlatformExtensions.IsOnLinux)
                            {
                                rich.SelectionColor = SystemColors.HighlightText; // Standard highlight text color
                            }

                            // For ReadOnly, we want to have a different back color than the default
                            if (rich.ReadOnly)
                            {
                                control.BackColor = SystemColors.Control;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that might occur during RichTextBox styling
                        // Especially for Linux via Mono
                        Debug.WriteLine(ex);
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
                    btn.UseVisualStyleBackColor = true;
                }
                else
                {
                    btn.FlatStyle = FlatStyle.Standard;
                    btn.BackColor = LightModeButtonBackColor;
                    btn.ForeColor = LightModeButtonForeColor;
                    btn.UseVisualStyleBackColor = true;
                    // Explicitly clear any custom border color for light mode standard buttons
                    // or set to a system default if absolutely necessary, but usually not needed for standard.
                    btn.FlatAppearance.BorderColor = SystemColors.ControlDark; // Or remove this line
                }

                btn.Invalidate();
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
                // Apply Windows Color Mode:
                NativeMethods.SetWindowThemeForComboBoxManaged(control.Handle, darkMode);

                ComboBoxStyle originalStyle = cb.DropDownStyle;
                try
                {
                    if (darkMode)
                    {
                        cb.BackColor = DarkModeTextBackColor;
                        cb.ForeColor = DarkModeFormForeColor; // Using DarkModeFormForeColor for text
                    }
                    else // Light Mode
                    {
                        cb.BackColor = SystemColors.Window;
                        cb.ForeColor = SystemColors.ControlText;
                    }
                    cb.Invalidate();
                }
                finally
                {
                    cb.DropDownStyle = originalStyle;
                }

                if (cb is gComboBox gcmb && gcmb.ContextMenuStrip != null)
                {
                    if (darkMode)
                    {
                        gcmb.ContextMenuStrip.BackColor = DarkModeButtonBackColor;
                        gcmb.ContextMenuStrip.ForeColor = DarkModeFormForeColor;
                        gcmb.ContextMenuStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                        foreach (ToolStripItem item in gcmb.ContextMenuStrip.Items)
                        {
                            ApplyToolStripItemThemeForComboBox(item, darkMode); // Use a dedicated helper
                        }
                    }
                    else // Light Mode for ComboBox ContextMenuStrip
                    {
                        gcmb.ContextMenuStrip.BackColor = SystemColors.ControlLightLight;
                        gcmb.ContextMenuStrip.ForeColor = SystemColors.ControlText;
                        gcmb.ContextMenuStrip.RenderMode = ToolStripRenderMode.System;
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
                    tv.LineColor = SystemColors.ControlLight;
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
                ApplyContextMenuTheme(cms, darkMode);
            }
            else if (control is StatusStrip ss)
            {
                // menuBackColor and menuForeColor are defined at the start of ApplyTheme
                // menuBackColor is DarkModeMenuBackColor or LightModeMenuBackColor (e.g. SystemColors.Control)
                // menuForeColor is DarkModeMenuForeColor or LightModeMenuForeColor (e.g. SystemColors.ControlText or MenuText)
                ss.BackColor = menuBackColor;
                ss.ForeColor = menuForeColor;
                // Mono does not provide the same built-in dark strip rendering as Windows, so use
                // an explicit renderer there instead of relying on the runtime defaults.
                if (darkMode && PlatformExtensions.IsOnLinux)
                {
                    ss.Renderer = LinuxDarkStatusStripRenderer;
                }
                else
                {
                    // For StatusStrip, System RenderMode is often best for light mode OS integration.
                    ss.RenderMode = darkMode ? ToolStripRenderMode.ManagerRenderMode : ToolStripRenderMode.System;
                }

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
            else if (control.HasChildren 
                && !(control is Form 
                || control is gForm 
                || control is GroupBox 
                || control is Panel 
                || control is TabControl 
                || control is gGroupBox 
                || control is gTableLayoutPanel))
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
                ApplyContextMenuTheme(menuItem.DropDown, darkMode);
            }
            // No specific DropDown handling needed for ToolStripStatusLabel as it doesn't have dropdowns.
            // ToolStripDropDownItem is for general dropdowns in ToolStrips, less common in StatusStrip.
            // If general ToolStripDropDownButtons or ToolStripSplitButtons are on the StatusStrip,
            // their dropdowns might need explicit theming if they don't inherit correctly.
            // For now, this focuses on ToolStripStatusLabel and ToolStripMenuItem.
        }

        public static void ApplyContextMenuTheme(ToolStripDropDown menu, bool darkMode)
        {
            if (menu == null || menu.IsDisposed)
            {
                return;
            }

            if (menu is ToolStripDropDownMenu dropDownMenu)
            {
                dropDownMenu.ShowCheckMargin = false;
                dropDownMenu.ShowImageMargin = true;
            }

            if (darkMode)
            {
                menu.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                menu.BackColor = DarkModeMenuBackColor;
                menu.ForeColor = DarkModeMenuForeColor;
            }
            else
            {
                menu.RenderMode = ToolStripRenderMode.Professional;
                menu.BackColor = SystemColors.ControlLightLight;
                menu.ForeColor = SystemColors.ControlText;
            }

            foreach (ToolStripItem item in menu.Items)
            {
                ApplyToolStripItemTheme(item, darkMode);
            }

            menu.Invalidate();
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

            if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
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

        private static void groupBoxPaintEventHandler(object sender, PaintEventArgs e)
        {
            var groupBox = sender as GroupBox;
            if (groupBox.Enabled == false)
            {
                using (Brush brush = new SolidBrush(groupBox.ForeColor))
                {
                    e.Graphics.DrawString(
                        groupBox.Text,
                        groupBox.Font,
                        brush,
                        new PointF(
                            groupBox.Font.SizeInPoints,
                            -1),
                        StringFormat.GenericTypographic);
                }
            }
        }

        private sealed class LinuxDarkStatusStripProfessionalRenderer : ToolStripProfessionalRenderer
        {
            public LinuxDarkStatusStripProfessionalRenderer()
                : base(new LinuxDarkStatusStripColorTable())
            {
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                Rectangle bounds = e.ToolStrip.ClientRectangle;
                using (Brush brush = new SolidBrush(e.ToolStrip.BackColor))
                {
                    e.Graphics.FillRectangle(brush, bounds);
                }
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                int width = e.ToolStrip.Width;
                if (width <= 0)
                {
                    return;
                }

                using (Pen pen = new Pen(DarkModeFormBackColor))
                {
                    e.Graphics.DrawLine(pen, 0, 0, width - 1, 0);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.ForeColor;
                base.OnRenderItemText(e);
            }
        }

        private sealed class LinuxDarkStatusStripColorTable : ProfessionalColorTable
        {
            public override Color StatusStripGradientBegin
            {
                get { return DarkModeMenuBackColor; }
            }

            public override Color StatusStripGradientEnd
            {
                get { return DarkModeMenuBackColor; }
            }

            public override Color ToolStripGradientBegin
            {
                get { return DarkModeMenuBackColor; }
            }

            public override Color ToolStripGradientMiddle
            {
                get { return DarkModeMenuBackColor; }
            }

            public override Color ToolStripGradientEnd
            {
                get { return DarkModeMenuBackColor; }
            }

            public override Color ToolStripBorder
            {
                get { return DarkModeFormBackColor; }
            }
        }

    }
}
