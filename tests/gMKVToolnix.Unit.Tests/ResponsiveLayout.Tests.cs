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
        private sealed class StubMainForm : IFormMain
        {
            public void SetTableLayoutMainStatus(bool argStatus)
            {
            }
        }

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
        public void frmLog_ApplyResponsiveLayout_NormalizesAutoscaledStartupBaselines()
        {
            RunInSta(() =>
            {
                using (var form = new frmLog())
                {
                    IntPtr unusedHandle = form.Handle;
                    form.Scale(new SizeF(MockHighDpiScaleFactor, MockHighDpiScaleFactor));

                    MethodInfo scaleFontsMethod = typeof(gForm).GetMethod("ScaleFonts", BindingFlags.Instance | BindingFlags.NonPublic);
                    Assert.IsNotNull(scaleFontsMethod, "Could not find gForm.ScaleFonts.");
                    scaleFontsMethod.Invoke(form, new object[] { MockHighDpiScaleFactor });

                    FieldInfo currentDpiField = typeof(gForm).GetField("currentDpi", BindingFlags.Instance | BindingFlags.NonPublic);
                    Assert.IsNotNull(currentDpiField, "Could not find gForm.currentDpi.");
                    currentDpiField.SetValue(form, 96F * MockHighDpiScaleFactor);

                    FieldInfo responsiveButtonSizesField = typeof(frmLog).GetField("_responsiveButtonBaseSizes", BindingFlags.Instance | BindingFlags.NonPublic);
                    FieldInfo actionsRowBaseHeightField = typeof(frmLog).GetField("_actionsRowBaseHeight", BindingFlags.Instance | BindingFlags.NonPublic);
                    MethodInfo captureBaselineMethod = typeof(frmLog).GetMethod("CaptureResponsiveLayoutBaseline", BindingFlags.Instance | BindingFlags.NonPublic);
                    MethodInfo applyResponsiveLayoutMethod = typeof(frmLog).GetMethod("ApplyResponsiveLayout", BindingFlags.Instance | BindingFlags.NonPublic);

                    Assert.IsNotNull(responsiveButtonSizesField, "Could not find frmLog responsive button baseline field.");
                    Assert.IsNotNull(actionsRowBaseHeightField, "Could not find frmLog actions row baseline field.");
                    Assert.IsNotNull(captureBaselineMethod, "Could not find frmLog.CaptureResponsiveLayoutBaseline.");
                    Assert.IsNotNull(applyResponsiveLayoutMethod, "Could not find frmLog.ApplyResponsiveLayout.");

                    var responsiveButtonSizes = (System.Collections.Generic.Dictionary<Button, Size>)responsiveButtonSizesField.GetValue(form);
                    responsiveButtonSizes.Clear();
                    actionsRowBaseHeightField.SetValue(form, 0F);

                    form.ClientSize = new Size(980, 870);
                    captureBaselineMethod.Invoke(form, null);
                    applyResponsiveLayoutMethod.Invoke(form, null);

                    GroupBox actionsGroup = FindControl<GroupBox>(form, "grpActions");
                    TableLayoutPanel mainLayout = FindControl<TableLayoutPanel>(form, "tlpMain");
                    Button clearButton = FindControl<Button>(form, "btnClear");
                    Button saveButton = FindControl<Button>(form, "btnSave");
                    Button refreshButton = FindControl<Button>(form, "btnRefresh");
                    Button copyButton = FindControl<Button>(form, "btnCopy");
                    Button closeButton = FindControl<Button>(form, "btnClose");

                    AssertControlsInsideGroup(actionsGroup, clearButton, saveButton, refreshButton, copyButton, closeButton);
                    Assert.IsTrue(saveButton.Right + 2 <= refreshButton.Left, "Refresh button overlapped Save after autoscaled baseline capture.");
                    Assert.IsTrue(refreshButton.Right + 2 <= copyButton.Left, "Refresh button overlapped Copy after autoscaled baseline capture.");
                    Assert.IsTrue(closeButton.Height < 60, string.Format("Close button stayed too tall after baseline normalization: {0}", closeButton.Height));

                    int requiredRowHeight = new[] { clearButton.Bottom, saveButton.Bottom, refreshButton.Bottom, copyButton.Bottom, closeButton.Bottom }.Max()
                        + 6
                        + actionsGroup.Margin.Vertical;
                    Assert.IsTrue(
                        Math.Abs(mainLayout.RowStyles[1].Height - requiredRowHeight) <= 2F,
                        string.Format(
                            "Log actions row kept autoscaled startup height. Row={0}, Required={1}",
                            mainLayout.RowStyles[1].Height,
                            requiredRowHeight));
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

                    AssertControlsInsideParent(actionsPanel.Parent, actionsPanel);
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

        [TestMethod]
        public void frmLog_DpiRoundTrip_RestoresOriginalActionLayout()
        {
            RunInSta(() =>
            {
                using (var form = new frmLog())
                {
                    IntPtr unusedHandle = form.Handle;
                    InvokePrivateMethod(form, "ApplyResponsiveLayout");

                    GroupBox actionsGroup = FindControl<GroupBox>(form, "grpActions");
                    TableLayoutPanel mainLayout = FindControl<TableLayoutPanel>(form, "tlpMain");
                    Button clearButton = FindControl<Button>(form, "btnClear");
                    Button saveButton = FindControl<Button>(form, "btnSave");
                    Button refreshButton = FindControl<Button>(form, "btnRefresh");
                    Button copyButton = FindControl<Button>(form, "btnCopy");
                    Button closeButton = FindControl<Button>(form, "btnClose");

                    Size initialCloseButtonSize = closeButton.Size;
                    float initialActionRowHeight = mainLayout.RowStyles[1].Height;
                    Size initialClientSize = form.ClientSize;
                    float baseDpi = GetCurrentDpi(form);

                    SimulateDpiChange(form, baseDpi * MockHighDpiScaleFactor, new Size(1514, 1327));
                    AssertControlsInsideGroup(actionsGroup, clearButton, saveButton, refreshButton, copyButton, closeButton);

                    SimulateDpiChange(form, baseDpi, initialClientSize);
                    AssertControlsInsideGroup(actionsGroup, clearButton, saveButton, refreshButton, copyButton, closeButton);
                    AssertSizeNear(initialCloseButtonSize, closeButton.Size, 4, closeButton.Name);
                    AssertFloatNear(initialActionRowHeight, mainLayout.RowStyles[1].Height, 4F, "frmLog actions row");
                }
            });
        }

        [TestMethod]
        public void frmOptions_DpiRoundTrip_RestoresOriginalButtonAndRowSizes()
        {
            RunInSta(() =>
            {
                using (var form = new frmOptions())
                {
                    IntPtr unusedHandle = form.Handle;
                    InvokePrivateMethod(form, "ApplyResponsiveLayout");

                    TableLayoutPanel mainLayout = FindControl<TableLayoutPanel>(form, "tlpMain");
                    GroupBox actionsGroup = FindControl<GroupBox>(form, "grpActions");
                    GroupBox advancedGroup = FindControl<GroupBox>(form, "grpAdvanced");
                    Button translationEditorButton = FindControl<Button>(form, "btnTranslationEditor");
                    Button defaultsButton = FindControl<Button>(form, "btnDefaults");
                    Button okButton = FindControl<Button>(form, "btnOK");
                    Button cancelButton = FindControl<Button>(form, "btnCancel");

                    Size initialTranslationEditorButtonSize = translationEditorButton.Size;
                    Size initialOkButtonSize = okButton.Size;
                    float initialAdvancedRowHeight = mainLayout.RowStyles[7].Height;
                    float initialActionsRowHeight = mainLayout.RowStyles[8].Height;
                    Size initialClientSize = form.ClientSize;
                    float baseDpi = GetCurrentDpi(form);

                    SimulateDpiChange(form, baseDpi * MockHighDpiScaleFactor, new Size(1900, 1500));
                    AssertControlsInsideGroup(advancedGroup, translationEditorButton);
                    AssertControlsInsideGroup(actionsGroup, defaultsButton, okButton, cancelButton);

                    SimulateDpiChange(form, baseDpi, initialClientSize);
                    AssertControlsInsideGroup(advancedGroup, translationEditorButton);
                    AssertControlsInsideGroup(actionsGroup, defaultsButton, okButton, cancelButton);
                    AssertSizeNear(initialTranslationEditorButtonSize, translationEditorButton.Size, 4, translationEditorButton.Name);
                    AssertSizeNear(initialOkButtonSize, okButton.Size, 4, okButton.Name);
                    AssertFloatNear(initialAdvancedRowHeight, mainLayout.RowStyles[7].Height, 4F, "frmOptions advanced row");
                    AssertFloatNear(initialActionsRowHeight, mainLayout.RowStyles[8].Height, 4F, "frmOptions actions row");
                }
            });
        }

        [TestMethod]
        public void frmTranslationEditor_DpiRoundTrip_RestoresOriginalActionSizesAndKeepsSummaryInline()
        {
            RunInSta(() =>
            {
                using (var form = new frmTranslationEditor("en"))
                {
                    IntPtr unusedHandle = form.Handle;
                    InvokePrivateMethod(form, "ApplyResponsiveLayout");

                    TableLayoutPanel mainLayout = FindControl<TableLayoutPanel>(form, "_mainLayout");
                    GroupBox actionsGroup = FindControl<GroupBox>(form, "_actionsGroup");
                    FlowLayoutPanel actionsPanel = FindControl<FlowLayoutPanel>(form, "_actionsPanel");
                    CheckBox untranslatedCheckBox = FindControl<CheckBox>(form, "_chkShowOnlyUntranslated");
                    Label summaryLabel = FindControl<Label>(form, "_lblSummary");
                    Button closeButton = FindControl<Button>(form, "_btnClose");
                    Button saveButton = FindControl<Button>(form, "_btnSave");
                    Button syncButton = FindControl<Button>(form, "_btnSync");
                    Button createButton = FindControl<Button>(form, "_btnCreate");

                    Size initialCloseButtonSize = closeButton.Size;
                    float initialSettingsRowHeight = mainLayout.RowStyles[0].Height;
                    float initialActionsRowHeight = mainLayout.RowStyles[2].Height;
                    Size initialClientSize = form.ClientSize;
                    float baseDpi = GetCurrentDpi(form);

                    SimulateDpiChange(form, baseDpi * MockHighDpiScaleFactor, new Size(2400, 1600));
                    AssertControlsInsideParent(actionsPanel.Parent, actionsPanel);
                    AssertControlsInsideParent(actionsPanel, closeButton, saveButton, syncButton, createButton);
                    Assert.IsTrue(summaryLabel.Top < untranslatedCheckBox.Bottom, "Summary label wrapped after scaling up.");

                    SimulateDpiChange(form, baseDpi, initialClientSize);
                    AssertControlsInsideParent(actionsPanel.Parent, actionsPanel);
                    AssertControlsInsideParent(actionsPanel, closeButton, saveButton, syncButton, createButton);
                    Assert.IsTrue(summaryLabel.Top < untranslatedCheckBox.Bottom, "Summary label wrapped after returning to normal DPI.");
                    AssertSizeNear(initialCloseButtonSize, closeButton.Size, 4, closeButton.Name);
                    AssertFloatNear(initialSettingsRowHeight, mainLayout.RowStyles[0].Height, 4F, "frmTranslationEditor settings row");
                    AssertFloatNear(initialActionsRowHeight, mainLayout.RowStyles[2].Height, 4F, "frmTranslationEditor actions row");
                }
            });
        }

        [TestMethod]
        public void frmJobManager_DpiRoundTrip_RestoresOriginalActionPanelLayout()
        {
            RunInSta(() =>
            {
                using (var form = new frmJobManager(new StubMainForm()))
                {
                    IntPtr unusedHandle = form.Handle;
                    InvokePrivateMethod(form, "ApplyResponsiveLayout");

                    TableLayoutPanel jobsLayout = FindControl<TableLayoutPanel>(form, "tlpJobs");
                    GroupBox actionsGroup = FindControl<GroupBox>(form, "grpActions");
                    Button removeButton = FindControl<Button>(form, "btnRemove");
                    Button runAllButton = FindControl<Button>(form, "btnRunAll");
                    Button loadJobsButton = FindControl<Button>(form, "btnLoadJobs");
                    Button saveJobsButton = FindControl<Button>(form, "btnSaveJobs");
                    Button abortButton = FindControl<Button>(form, "btnAbort");
                    Button abortAllButton = FindControl<Button>(form, "btnAbortAll");
                    CheckBox showPopupCheckBox = FindControl<CheckBox>(form, "chkShowPopup");

                    Size initialRemoveButtonSize = removeButton.Size;
                    float initialActionColumnWidth = jobsLayout.ColumnStyles[1].Width;
                    Size initialClientSize = form.ClientSize;
                    float baseDpi = GetCurrentDpi(form);

                    SimulateDpiChange(form, baseDpi * MockHighDpiScaleFactor, new Size(1564, 1143));
                    AssertControlsInsideGroup(actionsGroup, showPopupCheckBox, removeButton, runAllButton, loadJobsButton, saveJobsButton, abortButton, abortAllButton);

                    SimulateDpiChange(form, baseDpi, initialClientSize);
                    AssertControlsInsideGroup(actionsGroup, showPopupCheckBox, removeButton, runAllButton, loadJobsButton, saveJobsButton, abortButton, abortAllButton);
                    AssertSizeNear(initialRemoveButtonSize, removeButton.Size, 4, removeButton.Name);
                    AssertFloatNear(initialActionColumnWidth, jobsLayout.ColumnStyles[1].Width, 6F, "frmJobManager actions column");
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

        private static void SimulateDpiChange(Form form, float newDpi, Size clientSize)
        {
            Assert.IsInstanceOfType(form, typeof(gForm));

            Type baseFormType = typeof(gForm);
            FieldInfo oldDpiField = baseFormType.GetField("oldDpi", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo currentDpiField = baseFormType.GetField("currentDpi", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo pendingDpiBoundsField = baseFormType.GetField("_pendingDpiBounds", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo applyPendingDpiChangeMethod = baseFormType.GetMethod("ApplyPendingDpiChange", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(oldDpiField, "Could not find gForm.oldDpi.");
            Assert.IsNotNull(currentDpiField, "Could not find gForm.currentDpi.");
            Assert.IsNotNull(pendingDpiBoundsField, "Could not find gForm pending DPI bounds field.");
            Assert.IsNotNull(applyPendingDpiChangeMethod, "Could not find gForm.ApplyPendingDpiChange.");

            float currentDpi = (float)currentDpiField.GetValue(form);
            if (currentDpi <= 0F)
            {
                currentDpi = 96F;
            }

            oldDpiField.SetValue(form, currentDpi);
            currentDpiField.SetValue(form, newDpi);

            int nonClientWidth = Math.Max(0, form.Width - form.ClientSize.Width);
            int nonClientHeight = Math.Max(0, form.Height - form.ClientSize.Height);
            Rectangle suggestedBounds = new Rectangle(
                form.Left,
                form.Top,
                clientSize.Width + nonClientWidth,
                clientSize.Height + nonClientHeight);
            pendingDpiBoundsField.SetValue(form, (Rectangle?)suggestedBounds);

            applyPendingDpiChangeMethod.Invoke(form, null);
            form.ClientSize = clientSize;
            form.PerformLayout();
        }

        private static float GetCurrentDpi(Form form)
        {
            FieldInfo currentDpiField = typeof(gForm).GetField("currentDpi", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(currentDpiField, "Could not find gForm.currentDpi.");

            float currentDpi = (float)currentDpiField.GetValue(form);
            return currentDpi > 0F ? currentDpi : 96F;
        }

        private static void InvokePrivateMethod(object target, string methodName)
        {
            Type targetType = target.GetType();
            MethodInfo method = null;
            while (targetType != null && method == null)
            {
                method = targetType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
                targetType = targetType.BaseType;
            }

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

        private static void AssertSizeNear(Size expected, Size actual, int tolerance, string label)
        {
            Assert.IsTrue(
                Math.Abs(expected.Width - actual.Width) <= tolerance,
                string.Format("{0} width drifted. Expected {1}, actual {2}.", label, expected.Width, actual.Width));
            Assert.IsTrue(
                Math.Abs(expected.Height - actual.Height) <= tolerance,
                string.Format("{0} height drifted. Expected {1}, actual {2}.", label, expected.Height, actual.Height));
        }

        private static void AssertFloatNear(float expected, float actual, float tolerance, string label)
        {
            Assert.IsTrue(
                Math.Abs(expected - actual) <= tolerance,
                string.Format("{0} drifted. Expected {1}, actual {2}.", label, expected, actual));
        }
    }
}
