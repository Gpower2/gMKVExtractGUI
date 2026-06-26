using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using gMKVToolNix.Controls;
using gMKVToolNix.Forms;
using gMKVToolNix.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolNix.Unit.Tests
{
    [TestClass]
    public class ResponsiveLayout_Tests
    {
        private const float MockHighDpiScaleFactor = 2.5F;

        [TestInitialize]
        public void InitializeLocalization()
        {
            LocalizationManager.Reload("en");
        }

        [TestMethod]
        public void ApplyLocalizedButtonSize_SizeOverload_PreservesProvidedMinimumSize()
        {
            using (var button = new Button())
            using (var font = new Font("Segoe UI", 18F))
            {
                button.Font = font;
                button.Text = "OK";

                Size minimumSize = new Size(208, 75);
                button.ApplyLocalizedButtonSize(minimumSize);

                Assert.AreEqual(minimumSize.Width, button.Width);
                Assert.AreEqual(minimumSize.Height, button.Height);
            }
        }

        [TestMethod]
        public void frmLog_ApplyResponsiveLayout_AfterManualScale_KeepsActionsCompactAndNonOverlapping()
        {
            RunInSta(() =>
            {
                using (var form = new frmLog())
                {
                    PrepareScaledForm(form, new Size(1514, 1327));
                    InvokePrivateMethod(form, "ApplyResponsiveLayout");

                    GroupBox actionsGroup = FindControl<GroupBox>(form, "grpActions");
                    TableLayoutPanel mainLayout = FindControl<TableLayoutPanel>(form, "tlpMain");
                    Button clearButton = FindControl<Button>(form, "btnClear");
                    Button saveButton = FindControl<Button>(form, "btnSave");
                    Button refreshButton = FindControl<Button>(form, "btnRefresh");
                    Button copyButton = FindControl<Button>(form, "btnCopy");
                    Button closeButton = FindControl<Button>(form, "btnClose");

                    AssertControlsInsideGroup(actionsGroup,
                        clearButton,
                        saveButton,
                        refreshButton,
                        copyButton,
                        closeButton);

                    Assert.IsTrue(clearButton.Right + 2 <= saveButton.Left, "Clear and Save buttons overlapped.");
                    Assert.IsTrue(saveButton.Right + 40 <= refreshButton.Left, "The center gap between Save and Refresh collapsed.");
                    Assert.IsTrue(refreshButton.Right + 2 <= copyButton.Left, "Refresh and Copy buttons overlapped.");
                    Assert.IsTrue(copyButton.Right + 2 <= closeButton.Left, "Copy and Close buttons overlapped.");

                    int requiredRowHeight = new[] { clearButton.Bottom, saveButton.Bottom, refreshButton.Bottom, copyButton.Bottom, closeButton.Bottom }.Max()
                        + 6
                        + actionsGroup.Margin.Vertical;
                    Assert.IsTrue(
                        Math.Abs(mainLayout.RowStyles[1].Height - requiredRowHeight) <= 2F,
                        string.Format(
                            "Log actions row kept extra height. Row={0}, Required={1}, Buttons={2}x{3}/{4}x{5}/{6}x{7}/{8}x{9}/{10}x{11}",
                            mainLayout.RowStyles[1].Height,
                            requiredRowHeight,
                            clearButton.Width,
                            clearButton.Height,
                            saveButton.Width,
                            saveButton.Height,
                            refreshButton.Width,
                            refreshButton.Height,
                            copyButton.Width,
                            copyButton.Height,
                            closeButton.Width,
                            closeButton.Height));
                }
            });
        }

        [TestMethod]
        public void frmOptions_ApplyResponsiveLayout_AfterManualScale_KeepsResponsiveButtonsInsideTheirGroups()
        {
            RunInSta(() =>
            {
                using (var form = new frmOptions())
                {
                    PrepareScaledForm(form, new Size(1900, 1500));
                    InvokePrivateMethod(form, "ApplyResponsiveLayout");

                    GroupBox videoTracksGroup = FindControl<GroupBox>(form, "grpVideoTracks");
                    GroupBox actionsGroup = FindControl<GroupBox>(form, "grpActions");
                    GroupBox advancedGroup = FindControl<GroupBox>(form, "grpAdvanced");

                    Button addVideoButton = FindControl<Button>(form, "btnAddVideoTrackPlaceholder");
                    Button defaultVideoButton = FindControl<Button>(form, "btnDefaultVideoTrackPlaceholder");
                    TextBox videoPattern = FindControl<TextBox>(form, "txtVideoTracksFilename");
                    Button translationEditorButton = FindControl<Button>(form, "btnTranslationEditor");

                    AssertControlsInsideGroup(videoTracksGroup, addVideoButton, defaultVideoButton, videoPattern);
                    AssertControlsInsideGroup(actionsGroup,
                        FindControl<Button>(form, "btnDefaults"),
                        FindControl<Button>(form, "btnOK"),
                        FindControl<Button>(form, "btnCancel"));
                    AssertControlsInsideGroup(advancedGroup, translationEditorButton);

                    Assert.IsTrue(videoPattern.Right <= addVideoButton.Left - 4, "Pattern text box should leave room for the Add button.");
                    Assert.IsTrue(addVideoButton.Right <= defaultVideoButton.Left - 4, "Add and Default buttons should not overlap.");
                }
            });
        }

        [TestMethod]
        public void frmTranslationEditor_ApplyResponsiveLayout_AfterManualScale_KeepsSummaryAndActionsOnOneRow()
        {
            RunInSta(() =>
            {
                using (var form = new frmTranslationEditor("en"))
                {
                    PrepareScaledForm(form, new Size(2400, 1600));
                    InvokePrivateMethod(form, "ApplyResponsiveLayout");

                    FlowLayoutPanel actionsPanel = FindControl<FlowLayoutPanel>(form, "_actionsPanel");
                    GroupBox actionsGroup = FindControl<GroupBox>(form, "_actionsGroup");
                    Label summaryLabel = FindControl<Label>(form, "_lblSummary");
                    CheckBox untranslatedCheckBox = FindControl<CheckBox>(form, "_chkShowOnlyUntranslated");

                    AssertControlsInsideGroup(actionsGroup, actionsPanel);
                    AssertControlsInsideParent(actionsPanel,
                        FindControl<Button>(form, "_btnCreate"),
                        FindControl<Button>(form, "_btnSync"),
                        FindControl<Button>(form, "_btnSave"),
                        FindControl<Button>(form, "_btnClose"));
                    Assert.IsTrue(
                        summaryLabel.Top < untranslatedCheckBox.Bottom,
                        string.Format(
                            "Summary label wrapped onto a second row. Summary=({0},{1},{2},{3}) Checkbox=({4},{5},{6},{7}) Search=({8},{9},{10},{11}) Row=({12},{13},{14},{15}) FormWidth={16}",
                            summaryLabel.Left,
                            summaryLabel.Top,
                            summaryLabel.Width,
                            summaryLabel.Height,
                            untranslatedCheckBox.Left,
                            untranslatedCheckBox.Top,
                            untranslatedCheckBox.Width,
                            untranslatedCheckBox.Height,
                            FindControl<TextBox>(form, "_txtSearch").Left,
                            FindControl<TextBox>(form, "_txtSearch").Top,
                            FindControl<TextBox>(form, "_txtSearch").Width,
                            FindControl<TextBox>(form, "_txtSearch").Height,
                            FindControl<FlowLayoutPanel>(form, "_settingsRow2").Left,
                            FindControl<FlowLayoutPanel>(form, "_settingsRow2").Top,
                            FindControl<FlowLayoutPanel>(form, "_settingsRow2").Width,
                            FindControl<FlowLayoutPanel>(form, "_settingsRow2").Height,
                            form.ClientSize.Width));
                }
            });
        }

        private static void RunInSta(Action action)
        {
            Exception failure = null;
            var thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (failure != null)
            {
                throw failure;
            }
        }

        private static void PrepareScaledForm(Form form, Size clientSize)
        {
            Assert.IsNotNull(form);

            IntPtr unusedHandle = form.Handle;
            form.Scale(new SizeF(MockHighDpiScaleFactor, MockHighDpiScaleFactor));
            form.ClientSize = clientSize;
            form.PerformLayout();
        }

        private static void InvokePrivateMethod(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, string.Format("Could not find method '{0}'.", methodName));
            method.Invoke(target, null);
        }

        private static TControl FindControl<TControl>(Control root, string name) where TControl : Control
        {
            Control[] matches = root.Controls.Find(name, true);
            Assert.AreEqual(1, matches.Length, string.Format("Expected to find one control named '{0}'.", name));
            TControl control = matches[0] as TControl;
            Assert.IsNotNull(control, string.Format("Control '{0}' was not of type {1}.", name, typeof(TControl).Name));
            return control;
        }

        private static void AssertControlsInsideGroup(GroupBox groupBox, params Control[] controls)
        {
            Assert.IsNotNull(groupBox);
            AssertControlsInsideRectangle(groupBox.ClientRectangle, groupBox.DisplayRectangle.Top - 4, controls);
        }

        private static void AssertControlsInsideParent(Control parent, params Control[] controls)
        {
            Assert.IsNotNull(parent);
            AssertControlsInsideRectangle(parent.ClientRectangle, -2, controls);
        }

        private static void AssertControlsInsideRectangle(Rectangle bounds, int minimumTop, params Control[] controls)
        {
            foreach (Control control in controls)
            {
                Assert.IsTrue(control.Left >= -2, string.Format("{0} overflowed the left edge.", control.Name));
                Assert.IsTrue(control.Right <= bounds.Width + 2, string.Format("{0} overflowed the right edge.", control.Name));
                Assert.IsTrue(control.Top >= minimumTop, string.Format("{0} overflowed the top edge.", control.Name));
                Assert.IsTrue(control.Bottom <= bounds.Height + 2, string.Format("{0} overflowed the bottom edge.", control.Name));
            }
        }
    }
}
