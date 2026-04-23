using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using gMKVToolNix.Theming;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolNix.Unit.Tests
{
    [TestClass]
    public class ThemeManager_Tests
    {
        [TestMethod]
        public void ApplyContextMenuTheme_DarkMode_ThemesNestedDropDowns()
        {
            using (var menu = new ContextMenuStrip())
            {
                var childItem = new ToolStripMenuItem("Leaf");
                var parentItem = new ToolStripMenuItem("Parent");
                parentItem.DropDownItems.Add(childItem);
                menu.Items.Add(parentItem);

                ThemeManager.ApplyContextMenuTheme(menu, true);

                var nestedMenu = parentItem.DropDown;

                Assert.AreEqual(ToolStripRenderMode.ManagerRenderMode, menu.RenderMode);
                Assert.AreEqual(ThemeManager.DarkModeMenuBackColor, menu.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, menu.ForeColor);
                Assert.AreEqual(ToolStripRenderMode.ManagerRenderMode, nestedMenu.RenderMode);
                Assert.AreEqual(ThemeManager.DarkModeMenuBackColor, nestedMenu.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, nestedMenu.ForeColor);
                Assert.IsTrue(menu.ShowImageMargin);
                Assert.IsFalse(menu.ShowCheckMargin);
                Assert.IsTrue(((ToolStripDropDownMenu)nestedMenu).ShowImageMargin);
                Assert.IsFalse(((ToolStripDropDownMenu)nestedMenu).ShowCheckMargin);
                Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, childItem.ForeColor);
            }
        }

        [TestMethod]
        public void ApplyContextMenuTheme_LightMode_ResetsNestedDropDownColors()
        {
            using (var menu = new ContextMenuStrip())
            {
                var childItem = new ToolStripMenuItem("Leaf");
                var parentItem = new ToolStripMenuItem("Parent");
                parentItem.DropDownItems.Add(childItem);
                menu.Items.Add(parentItem);

                ThemeManager.ApplyContextMenuTheme(menu, false);

                var nestedMenu = parentItem.DropDown;

                Assert.AreEqual(ToolStripRenderMode.Professional, menu.RenderMode);
                Assert.AreEqual(SystemColors.ControlLightLight, menu.BackColor);
                Assert.AreEqual(SystemColors.ControlText, menu.ForeColor);
                Assert.AreEqual(ToolStripRenderMode.Professional, nestedMenu.RenderMode);
                Assert.AreEqual(SystemColors.ControlLightLight, nestedMenu.BackColor);
                Assert.AreEqual(SystemColors.ControlText, nestedMenu.ForeColor);
                Assert.IsTrue(menu.ShowImageMargin);
                Assert.IsFalse(menu.ShowCheckMargin);
                Assert.IsTrue(((ToolStripDropDownMenu)nestedMenu).ShowImageMargin);
                Assert.IsFalse(((ToolStripDropDownMenu)nestedMenu).ShowCheckMargin);
                Assert.AreEqual(SystemColors.ControlText, childItem.ForeColor);
            }
        }

        [TestMethod]
        public void ApplyTheme_DarkMode_ContextMenuStrip_UsesBuiltInMenuRendering()
        {
            using (var menu = new ContextMenuStrip())
            {
                var item = new ToolStripMenuItem("Leaf");
                menu.Items.Add(item);

                ThemeManager.ApplyTheme(menu, true);

                Assert.AreEqual(ToolStripRenderMode.ManagerRenderMode, menu.RenderMode);
                Assert.AreEqual(ThemeManager.DarkModeMenuBackColor, menu.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, menu.ForeColor);
                Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, item.ForeColor);
            }
        }

        [TestMethod]
        public void ApplyTheme_DarkMode_StatusStrip_UsesCustomRendererOnLinux()
        {
            FieldInfo isOnLinuxField = typeof(PlatformExtensions).GetField("_isOnLinux", BindingFlags.NonPublic | BindingFlags.Static);
            bool? originalValue = (bool?)isOnLinuxField.GetValue(null);

            try
            {
                isOnLinuxField.SetValue(null, true);

                using (var statusStrip = new StatusStrip())
                {
                    var item = new ToolStripStatusLabel("Ready");
                    statusStrip.Items.Add(item);

                    ThemeManager.ApplyTheme(statusStrip, true);

                    Assert.AreEqual(ThemeManager.DarkModeMenuBackColor, statusStrip.BackColor);
                    Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, statusStrip.ForeColor);
                    Assert.AreEqual(ToolStripRenderMode.Custom, statusStrip.RenderMode);
                    Assert.IsInstanceOfType(statusStrip.Renderer, typeof(ToolStripProfessionalRenderer));
                    Assert.AreEqual(ThemeManager.DarkModeMenuBackColor, item.BackColor);
                    Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, item.ForeColor);
                }
            }
            finally
            {
                isOnLinuxField.SetValue(null, originalValue);
            }
        }

        [TestMethod]
        public void ApplyTheme_Button_LightMode_ResetsDarkButtonBackground()
        {
            using (var button = new Button())
            {
                ThemeManager.ApplyTheme(button, true);

                Assert.AreEqual(FlatStyle.Flat, button.FlatStyle);
                Assert.AreEqual(ThemeManager.DarkModeButtonBackColor, button.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeButtonForeColor, button.ForeColor);

                ThemeManager.ApplyTheme(button, false);

                Assert.AreEqual(FlatStyle.Standard, button.FlatStyle);
                Assert.AreEqual(ThemeManager.LightModeButtonBackColor, button.BackColor);
                Assert.AreEqual(ThemeManager.LightModeButtonForeColor, button.ForeColor);
                Assert.IsTrue(button.UseVisualStyleBackColor);
            }
        }
    }
}
