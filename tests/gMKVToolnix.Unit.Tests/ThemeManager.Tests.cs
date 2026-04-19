using System.Drawing;
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
    }
}
