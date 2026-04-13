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

                Assert.AreEqual(ThemeManager.DarkModeMenuBackColor, menu.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, menu.ForeColor);
                Assert.AreEqual(ThemeManager.DarkModeMenuBackColor, nestedMenu.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeMenuForeColor, nestedMenu.ForeColor);
                Assert.AreEqual("DarkModeContextMenuRenderer", menu.Renderer.GetType().Name);
                Assert.AreEqual("DarkModeContextMenuRenderer", nestedMenu.Renderer.GetType().Name);
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

                Assert.AreEqual(SystemColors.ControlLightLight, menu.BackColor);
                Assert.AreEqual(SystemColors.ControlText, menu.ForeColor);
                Assert.AreEqual(SystemColors.ControlLightLight, nestedMenu.BackColor);
                Assert.AreEqual(SystemColors.ControlText, nestedMenu.ForeColor);
                Assert.AreEqual(typeof(ToolStripProfessionalRenderer), menu.Renderer.GetType());
                Assert.AreEqual(typeof(ToolStripProfessionalRenderer), nestedMenu.Renderer.GetType());
                Assert.IsTrue(menu.ShowImageMargin);
                Assert.IsFalse(menu.ShowCheckMargin);
                Assert.IsTrue(((ToolStripDropDownMenu)nestedMenu).ShowImageMargin);
                Assert.IsFalse(((ToolStripDropDownMenu)nestedMenu).ShowCheckMargin);
                Assert.AreEqual(SystemColors.ControlText, childItem.ForeColor);
            }
        }
    }
}
