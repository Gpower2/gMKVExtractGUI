using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using gMKVToolNix.Controls;
using gMKVToolNix.Forms;
using gMKVToolNix.Jobs;
using gMKVToolNix.Localization;
using gMKVToolNix.Log;
using gMKVToolNix.MkvExtract;
using gMKVToolNix.Theming;
using gMKVToolNix.WinAPI;

namespace gMKVToolNix
{
    public partial class frmJobManager : gForm
    {
        private const int ActionButtonMinWidth = 90;
        private const int ActionPanelMinWidth = 120;
        private const int ProgressValueLabelWidth = 52;
        private readonly StringBuilder _ExceptionBuilder = new StringBuilder();
        private readonly IFormMain _MainForm = null;
        private int _CurrentJob = 0;
        private int _TotalJobs = 0;
        private gMKVExtract _gMkvExtract = null;
        private bool _ExtractRunning = false;
        private readonly gSettings _Settings = null;
        private readonly bool _FromConstructor = false;
        private bool _isCurrentlyDarkMode = false;
        private readonly Dictionary<Button, Size> _responsiveButtonBaseSizes = new Dictionary<Button, Size>();
        private float _actionPanelBaseWidth;
        private int _actionButtonBaseLeft;
        private int _actionButtonBaseRightMargin;
        private int _showPopupBaseLeft;
        private int _abortButtonsBaseSpacing;
        private int _abortAllBaseBottomMargin;
        private int _progressLabelBaseLeft;
        private int _progressContentBaseLeft;
        private int _currentTrackRightMarginBase;
        private int _progressBarRightMarginBase;

        private BindingList<gMKVJobInfo> _JobList = new BindingList<gMKVJobInfo>();

         private bool _AbortAll = false;

         public frmJobManager(IFormMain argMainForm)
        {
            try
            {
                InitializeComponent();
                CaptureResponsiveLayoutBaselines();

                _MainForm = argMainForm;

                Icon = Icon.ExtractAssociatedIcon(GetExecutingAssemblyLocation());
                Text = string.Format("gMKVExtractGUI v{0} -- Job Manager", GetCurrentVersion());

                _FromConstructor = true;

                // Load settings
                _Settings = new gSettings(this.GetCurrentDirectory());
                _Settings.Reload();

                _isCurrentlyDarkMode = _Settings.DarkMode; // Initialize field

                chkShowPopup.Checked = _Settings.ShowPopupInJobManager;

                // Apply Theme
                ThemeManager.ApplyTheme(this, _isCurrentlyDarkMode); // Use field
                if (this.Handle != IntPtr.Zero) // Or IsHandleCreated if preferred
                {
                    NativeMethods.SetWindowThemeManaged(this.Handle, _isCurrentlyDarkMode);
                    NativeMethods.TrySetImmersiveDarkMode(this.Handle, _isCurrentlyDarkMode);
                }
                else
                {
                    this.Shown += (s, ev) =>
                    {
                        NativeMethods.SetWindowThemeManaged(this.Handle, _isCurrentlyDarkMode);
                        NativeMethods.TrySetImmersiveDarkMode(this.Handle, _isCurrentlyDarkMode);
                    };
                }

                // Apply theme to context menu
                if (contextMenuStrip != null)
                {
                    ApplyContextMenuTheme();
                }

                grdJobs.DataSource = _JobList;

                _FromConstructor = false;

                SetAbortStatus(false);

                // Initialize the DPI aware scaling
                InitDPI();
                CaptureResponsiveLayoutBaselines();

                // Initialize localization
                //InitializeLocalization();
                ApplyLocalization();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                _FromConstructor = false;
                ShowErrorMessage(ex.Message);
            }
        }

        private void SetJobsList(BindingList<gMKVJobInfo> argJobList)
        {
            try
            {
                _JobList = argJobList;
                grdJobs.DataSource = _JobList;
                grdJobs.Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        public void AddJob(gMKVJobInfo argJobInfo)
        {
            // Check if the same job already exists
            foreach (var jobInfo in _JobList)
            {
                gMKVJob j = jobInfo.Job;
                gMKVJob argJ = argJobInfo.Job;
                if (j.ExtractionMode == argJ.ExtractionMode
                    && j.MKVToolnixPath == argJ.MKVToolnixPath
                    && j.ParametersList.Equals(argJ.ParametersList))
                {
                    return;
                }
            }

            _JobList.Add(argJobInfo);
        }

        private void SetAbortStatus(bool argStatus)
        {
            btnAbort.Enabled = argStatus;
            btnAbortAll.Enabled = argStatus;
        }

        private void SetActionStatus(bool argStatus)
        {
            btnRemove.Enabled = argStatus;
            btnRunAll.Enabled = argStatus;
            btnLoadJobs.Enabled = argStatus;
            btnSaveJobs.Enabled = argStatus;
        }

        void gMkvExtract_MkvExtractTrackUpdated(string filename, string trackName)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke((MethodInvoker)delegate
            {
                UpdateTrackLabel(filename, trackName);
            });
            Debug.WriteLine(trackName);
        }

        void gMkvExtract_MkvExtractProgressUpdated(int progress)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke((MethodInvoker)delegate
            {
                UpdateCurrentProgress(progress);
            });
        }

        private void frmJobManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            // To avoid getting disposed
            e.Cancel = true;
            if (_ExtractRunning)
            {
                ShowLocalizedErrorMessage("UI.Common.Errors.ExtractionRunningBeforeClose");
            }
            else
            {
                this.Hide();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdJobs.Rows.Count > 0)
                {
                    if (grdJobs.SelectedRows.Count > 0)
                    {
                        grdJobs.SelectedRows.Cast<DataGridViewRow>().ToList().ForEach(r => grdJobs.Rows.Remove(r));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        public void UpdateCurrentProgress(object val)
        {
            int progressValue = Convert.ToInt32(val);

            prgBrCurrent.Value = progressValue;
            prgBrTotal.Value = (_CurrentJob - 1) * 100 + progressValue;
            lblCurrentProgressValue.Text = string.Format("{0}%", progressValue);
            lblTotalProgressValue.Text = string.Format("{0}%", prgBrTotal.Value / _TotalJobs);
            gTaskbarProgress.SetValue(this, Convert.ToUInt64(progressValue), (UInt64)100);
        }

        public void UpdateTrackLabel(object filename, object val)
        {
            txtCurrentTrack.Text = string.Format("{0} from {1}", val, Path.GetFileName((string)filename));
        }

        private void btnRunAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetNumberOfJobs(JobState.Ready) == 0)
                {
                    throw CreateLocalizedException("UI.JobManager.Errors.NoJobsAvailable");
                }
                List<gMKVJobInfo> jobList = new List<gMKVJobInfo>();
                foreach (DataGridViewRow item in grdJobs.Rows)
                {
                    gMKVJobInfo jobInfo = (gMKVJobInfo)item.DataBoundItem;
                    if (jobInfo.State == JobState.Ready)
                    {
                        jobInfo.State = JobState.Pending;
                        jobList.Add(jobInfo);
                    }
                }
                grdJobs.Refresh();
                PrepareForRunJobs(jobList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void PrepareForRunJobs(List<gMKVJobInfo> argJobInfoList)
        {
            try
            {
                SetActionStatus(false);
                SetAbortStatus(true);
                _ExtractRunning = true;
                _MainForm.SetTableLayoutMainStatus(false);
                _TotalJobs = argJobInfoList.Count;
                _CurrentJob = 0;
                prgBrTotal.Maximum = _TotalJobs * 100;
                _ExceptionBuilder.Length = 0;
                RunNextJob(argJobInfoList, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
                FinishJobRunUi(true);
            }
        }

        private void RunNextJob(List<gMKVJobInfo> argJobInfoList, int jobIndex)
        {
            if (jobIndex >= argJobInfoList.Count || _AbortAll)
            {
                CompleteJobRun(argJobInfoList);
                return;
            }

            gMKVJobInfo jobInfo = argJobInfoList[jobIndex];

            try
            {
                gMKVJob job = jobInfo.Job;
                _gMkvExtract = new gMKVExtract(job.MKVToolnixPath);
                _gMkvExtract.MkvExtractProgressUpdated += gMkvExtract_MkvExtractProgressUpdated;
                _gMkvExtract.MkvExtractTrackUpdated += gMkvExtract_MkvExtractTrackUpdated;
                _CurrentJob++;
                jobInfo.StartTime = DateTime.Now;
                jobInfo.State = JobState.Running;
                grdJobs.Refresh();

                btnAbort.Enabled = true;
                btnAbortAll.Enabled = true;
                gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.Normal);

                job.StartAsync(_gMkvExtract)
                    .ContinueWith(
                        task => HandleJobCompleted(argJobInfoList, jobIndex, jobInfo, task),
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                if (_gMkvExtract != null)
                {
                    _gMkvExtract.MkvExtractProgressUpdated -= gMkvExtract_MkvExtractProgressUpdated;
                    _gMkvExtract.MkvExtractTrackUpdated -= gMkvExtract_MkvExtractTrackUpdated;
                }

                _ExceptionBuilder.AppendFormat(LocalizationManager.GetString("UI.JobManager.Errors.ExceptionForJob"), jobInfo.ToString(), ex.Message, Environment.NewLine);
                RunNextJob(argJobInfoList, jobIndex + 1);
            }
        }

        private void HandleJobCompleted(List<gMKVJobInfo> argJobInfoList, int jobIndex, gMKVJobInfo jobInfo, Task task)
        {
            try
            {
                jobInfo.EndTime = DateTime.Now;

                Exception extractException = GetJobException(task);
                if (extractException != null)
                {
                    jobInfo.State = JobState.Failed;
                    grdJobs.Refresh();
                    _ExceptionBuilder.AppendFormat(LocalizationManager.GetString("UI.JobManager.Errors.ExceptionForJob"), jobInfo.ToString(), extractException.Message, Environment.NewLine);
                }
                else
                {
                    jobInfo.State = JobState.Completed;
                    UpdateCurrentProgress(100);
                    grdJobs.Refresh();
                }
            }
            finally
            {
                if (_gMkvExtract != null)
                {
                    _gMkvExtract.MkvExtractProgressUpdated -= gMkvExtract_MkvExtractProgressUpdated;
                    _gMkvExtract.MkvExtractTrackUpdated -= gMkvExtract_MkvExtractTrackUpdated;
                }
            }

            RunNextJob(argJobInfoList, jobIndex + 1);
        }

        private Exception GetJobException(Task task)
        {
            if (_gMkvExtract != null && _gMkvExtract.ThreadedException != null)
            {
                return _gMkvExtract.ThreadedException;
            }

            return task.Exception == null ? null : task.Exception.Flatten().InnerException;
        }

        private void CompleteJobRun(List<gMKVJobInfo> argJobInfoList)
        {
            if (_ExceptionBuilder.Length > 0)
            {
                foreach (gMKVJobInfo jobInfo in argJobInfoList)
                {
                    if (jobInfo.State == JobState.Pending)
                    {
                        jobInfo.State = JobState.Ready;
                    }
                }

                Exception ex = new Exception(_ExceptionBuilder.ToString());
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
                FinishJobRunUi(true);
                return;
            }

            if (_TotalJobs > 0)
            {
                UpdateCurrentProgress(100);
            }

            SetAbortStatus(false);
            if (chkShowPopup.Checked)
            {
                ShowLocalizedSuccessMessage("UI.JobManager.Success.JobsCompleted", true);
            }
            else
            {
                SystemSounds.Asterisk.Play();
            }

            FinishJobRunUi(false);
        }

        private void FinishJobRunUi(bool exceptionOccured)
        {
            if (chkShowPopup.Checked || exceptionOccured)
            {
                if (_TotalJobs > 0)
                {
                    UpdateCurrentProgress(0);
                    prgBrTotal.Value = 0;
                }
                else
                {
                    prgBrCurrent.Value = 0;
                    prgBrTotal.Value = 0;
                }

                lblCurrentProgressValue.Text = "";
                lblTotalProgressValue.Text = "";
            }
            else
            {
                lblCurrentProgressValue.Text = "";
                lblTotalProgressValue.Text = "";
                txtCurrentTrack.Text = LocalizationManager.GetString("UI.Common.Status.ExtractionCompleted");
            }

            gTaskbarProgress.SetState(this, gTaskbarProgress.TaskbarStates.NoProgress);
            gTaskbarProgress.SetOverlayIcon(this, null, null);
            _ExtractRunning = false;
            _AbortAll = false;
            grdJobs.Refresh();
            SetActionStatus(true);
            SetAbortStatus(false);
            _MainForm.SetTableLayoutMainStatus(true);
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            try
            {
                if (_gMkvExtract != null)
                {
                    _gMkvExtract.Abort = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnAbortAll_Click(object sender, EventArgs e)
        {
            try
            {
                _AbortAll = true;
                if (_gMkvExtract != null)
                {
                    _gMkvExtract.Abort = true;
                    _gMkvExtract.AbortAll = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private int GetNumberOfJobs(JobState argState)
        {
            int counter = 0;
            foreach (DataGridViewRow drJobInfo in grdJobs.Rows)
            {
                gMKVJobInfo jobInfo = (gMKVJobInfo)drJobInfo.DataBoundItem;
                if (jobInfo.State == argState)
                {
                    counter++;
                }
            }
            return counter;
        }

        private void grdJobs_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Check if the row clicked is selected
                if (grdJobs.Rows[e.RowIndex].Selected)
                {
                    gMKVJobInfo jobInfo = (gMKVJobInfo)grdJobs.Rows[e.RowIndex].DataBoundItem;
                    if (jobInfo.State == JobState.Failed || jobInfo.State == JobState.Completed)
                    {
                        jobInfo.Reset();
                        grdJobs.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void btnSaveJobs_Click(object sender, EventArgs e)
        {
            try
            {
                // ask for path
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Title = LocalizationManager.GetString("UI.JobManager.Dialogs.SelectJobFileTitle"),
                    InitialDirectory = GetCurrentDirectory(),
                    Filter = "*.xml|*.xml"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(List<gMKVJobInfo>));

                        List<gMKVJobInfo> jobList = new List<gMKVJobInfo>();
                        foreach (DataGridViewRow item in grdJobs.Rows)
                        {
                            jobList.Add((gMKVJobInfo)item.DataBoundItem);
                        }

                        x.Serialize(sw, jobList);
                    }
                    ShowLocalizedSuccessMessage("UI.JobManager.Success.JobsSaved");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnLoadJobs_Click(object sender, EventArgs e)
        {
            try
            {
                // Ask for path
                OpenFileDialog ofd = new OpenFileDialog
                {
                    InitialDirectory = GetCurrentDirectory(),
                    Title = LocalizationManager.GetString("UI.JobManager.Dialogs.SelectJobsFileTitle"),
                    Filter = "*.xml|*.xml"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    List<gMKVJobInfo> jobList = null;
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(List<gMKVJobInfo>));

                        jobList = (List<gMKVJobInfo>)x.Deserialize(sr);
                    }
                    SetJobsList(new BindingList<gMKVJobInfo>(jobList));
                    ShowLocalizedSuccessMessage("UI.JobManager.Success.JobsLoaded");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            SetContextMenuText();
            ApplyContextMenuTheme();
        }

        private void SetContextMenuText()
        {
            if (_ExtractRunning)
            {
                changeToReadyStatusToolStripMenuItem.Enabled = false;
            }
            else if (grdJobs.SelectedRows.Count == 0)
            {
                changeToReadyStatusToolStripMenuItem.Enabled = false;
            }
            else
            {
                foreach (DataGridViewRow row in grdJobs.SelectedRows)
                {
                    if (((gMKVJobInfo)row.DataBoundItem).State != JobState.Ready)
                    {
                        changeToReadyStatusToolStripMenuItem.Enabled = true;
                        break;
                    }
                }
            }
        }

        private void changeToReadyStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grdJobs.SelectedRows)
            {
                if (((gMKVJobInfo)row.DataBoundItem).State != JobState.Ready)
                {
                    ((gMKVJobInfo)row.DataBoundItem).Reset();
                }
            }
            grdJobs.Refresh();
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grdJobs.ClearSelection();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grdJobs.SelectAll();
        }

        private void chkShowPopup_CheckedChanged(object sender, EventArgs e)
        {
            if (!_FromConstructor)
            {
                _Settings.ShowPopupInJobManager = chkShowPopup.Checked;
                _Settings.Save();
            }
        }

        private void grdJobs_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                UpdateJobsGroupTitle();
                ApplyJobsGridLocalization();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                gMKVLogger.Log(ex.ToString());
                ShowErrorMessage(ex.Message);
            }
        }

        private void UpdateJobsGroupTitle()
        {
            grpJobs.Text = LocalizationManager.GetString("UI.JobManager.Jobs.GroupWithCount", grdJobs.Rows.Count);
        }

        private void ApplyJobsGridLocalization()
        {
            if (grdJobs.Columns.Contains("Job"))
                grdJobs.Columns["Job"].HeaderText = LocalizationManager.GetString("UI.JobManager.Columns.Job");
            if (grdJobs.Columns.Contains("StartTime"))
                grdJobs.Columns["StartTime"].HeaderText = LocalizationManager.GetString("UI.JobManager.Columns.StartTime");
            if (grdJobs.Columns.Contains("EndTime"))
                grdJobs.Columns["EndTime"].HeaderText = LocalizationManager.GetString("UI.JobManager.Columns.EndTime");
            if (grdJobs.Columns.Contains("State"))
                grdJobs.Columns["State"].HeaderText = LocalizationManager.GetString("UI.JobManager.Columns.State");
            if (grdJobs.Columns.Contains("Duration"))
                grdJobs.Columns["Duration"].HeaderText = LocalizationManager.GetString("UI.JobManager.Columns.Duration");
        }

        public void UpdateTheme(bool darkMode)
        {
            _isCurrentlyDarkMode = darkMode; // Store the updated theme state
            ThemeManager.ApplyTheme(this, _isCurrentlyDarkMode); // Use field
            if (this.IsHandleCreated)
            {
                NativeMethods.SetWindowThemeManaged(this.Handle, _isCurrentlyDarkMode);
                NativeMethods.TrySetImmersiveDarkMode(this.Handle, _isCurrentlyDarkMode);
            }
            else
            {
                this.HandleCreated += (s, e) => {
                    NativeMethods.SetWindowThemeManaged(this.Handle, _isCurrentlyDarkMode);
                    NativeMethods.TrySetImmersiveDarkMode(this.Handle, _isCurrentlyDarkMode);
                };
            }

            if (contextMenuStrip != null)
            {
                ApplyContextMenuTheme();
            }
        }

        private void ApplyContextMenuTheme()
        {
            ThemeManager.ApplyContextMenuTheme(contextMenuStrip, _isCurrentlyDarkMode);

            if (grdJobs.ContextMenuStrip == this.contextMenuStrip)
            {
                grdJobs.ContextMenuStrip = null;
                grdJobs.ContextMenuStrip = this.contextMenuStrip;
            }

            this.contextMenuStrip.Invalidate();
            grdJobs.Invalidate();
        }

        public void ApplyLocalization()
        {
            try
            {
                this.Text = string.Format("gMKVExtractGUI v{0} -- {1}", GetCurrentVersion(), LocalizationManager.GetString("UI.JobManager.Title"));
                grpProgress.Text = LocalizationManager.GetString("UI.JobManager.Progress.Group");
                lblCurrentTrack.Text = LocalizationManager.GetString("UI.JobManager.Progress.CurrentTrack");
                lblTotalProgress.Text = LocalizationManager.GetString("UI.JobManager.Progress.TotalProgress");
                lblCurrentProgress.Text = LocalizationManager.GetString("UI.JobManager.Progress.CurrentProgress");
                UpdateJobsGroupTitle();
                changeToReadyStatusToolStripMenuItem.Text = LocalizationManager.GetString("UI.JobManager.Jobs.ChangeToReadyStatus");
                selectAllToolStripMenuItem.Text = LocalizationManager.GetString("UI.JobManager.Jobs.SelectAll");
                deselectAllToolStripMenuItem.Text = LocalizationManager.GetString("UI.JobManager.Jobs.DeselectAll");
                grpActions.Text = LocalizationManager.GetString("UI.JobManager.Actions.Group");
                chkShowPopup.Text = LocalizationManager.GetString("UI.JobManager.Actions.Popup");
                btnSaveJobs.Text = LocalizationManager.GetString("UI.JobManager.Actions.SaveJobs");
                btnLoadJobs.Text = LocalizationManager.GetString("UI.JobManager.Actions.LoadJobs");
                btnAbortAll.Text = LocalizationManager.GetString("UI.JobManager.Actions.AbortAll");
                btnAbort.Text = LocalizationManager.GetString("UI.JobManager.Actions.Abort");
                btnRunAll.Text = LocalizationManager.GetString("UI.JobManager.Actions.RunJobs");
                btnRemove.Text = LocalizationManager.GetString("UI.JobManager.Actions.Remove");
                ApplyJobsGridLocalization();
                ApplyResponsiveLayout();
                ApplyContextMenuTheme();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying localization: {ex.Message}");
                gMKVLogger.Log($"Error applying localization: {ex.Message}");
            }
        }

        private void ApplyResponsiveLayout()
        {
            ResetResponsiveLayoutBaselines();
            ApplyActionPanelLayout();
            ApplyProgressLayout();
        }

        private void CaptureResponsiveLayoutBaselines()
        {
            foreach (Button actionButton in GetActionButtons())
            {
                CaptureResponsiveButtonBaseSize(actionButton);
            }

            if (tlpJobs.ColumnStyles.Count > 1 && tlpJobs.ColumnStyles[1].Width > 0F)
            {
                _actionPanelBaseWidth = Math.Max(_actionPanelBaseWidth, tlpJobs.ColumnStyles[1].Width);
            }

            if (btnRemove != null)
            {
                if (btnRemove.Left > 0)
                {
                    _actionButtonBaseLeft = Math.Max(_actionButtonBaseLeft, btnRemove.Left);
                }

                if (grpActions != null && grpActions.ClientSize.Width > 0)
                {
                    _actionButtonBaseRightMargin = Math.Max(
                        _actionButtonBaseRightMargin,
                        grpActions.ClientSize.Width - btnRemove.Right);
                }
            }

            if (chkShowPopup != null)
            {
                _showPopupBaseLeft = Math.Max(_showPopupBaseLeft, chkShowPopup.Left);
            }

            if (btnAbort != null && btnAbortAll != null)
            {
                _abortButtonsBaseSpacing = Math.Max(_abortButtonsBaseSpacing, btnAbortAll.Top - btnAbort.Bottom);
            }

            if (btnAbortAll != null && grpActions != null && grpActions.ClientSize.Height > 0)
            {
                _abortAllBaseBottomMargin = Math.Max(_abortAllBaseBottomMargin, grpActions.ClientSize.Height - btnAbortAll.Bottom);
            }

            if (lblCurrentTrack != null)
            {
                _progressLabelBaseLeft = Math.Max(_progressLabelBaseLeft, lblCurrentTrack.Left);
            }

            if (txtCurrentTrack != null)
            {
                _progressContentBaseLeft = Math.Max(_progressContentBaseLeft, txtCurrentTrack.Left);
                if (grpProgress != null && grpProgress.ClientSize.Width > 0)
                {
                    _currentTrackRightMarginBase = Math.Max(_currentTrackRightMarginBase, grpProgress.ClientSize.Width - txtCurrentTrack.Right);
                }
            }

            if (prgBrCurrent != null && grpProgress != null && grpProgress.ClientSize.Width > 0)
            {
                _progressBarRightMarginBase = Math.Max(_progressBarRightMarginBase, grpProgress.ClientSize.Width - prgBrCurrent.Right);
            }
        }

        private void CaptureResponsiveButtonBaseSize(Button button)
        {
            if (button == null)
            {
                return;
            }

            Size currentSize = button.Size;
            if (!_responsiveButtonBaseSizes.TryGetValue(button, out Size baseSize)
                || currentSize.Width > baseSize.Width
                || currentSize.Height > baseSize.Height)
            {
                _responsiveButtonBaseSizes[button] = currentSize;
            }
        }

        private Size GetResponsiveButtonBaseSize(Button button, int fallbackWidth, int fallbackHeight = 30)
        {
            return _responsiveButtonBaseSizes.TryGetValue(button, out Size baseSize)
                ? baseSize
                : new Size(fallbackWidth, fallbackHeight);
        }

        private Button[] GetActionButtons()
        {
            return new[]
            {
                btnRemove,
                btnRunAll,
                btnLoadJobs,
                btnSaveJobs,
                btnAbort,
                btnAbortAll
            }.Where(button => button != null).ToArray();
        }

        private void ResetResponsiveLayoutBaselines()
        {
            if (tlpJobs.ColumnStyles.Count > 1)
            {
                tlpJobs.ColumnStyles[1].SizeType = SizeType.Absolute;
                tlpJobs.ColumnStyles[1].Width = _actionPanelBaseWidth > 0F
                    ? _actionPanelBaseWidth
                    : ActionPanelMinWidth;
            }
        }

        private void ApplyActionPanelLayout()
        {
            Button[] actionButtons = GetActionButtons();
            if (actionButtons.Length == 0)
            {
                return;
            }

            foreach (Button actionButton in actionButtons)
            {
                actionButton.ApplyLocalizedButtonSize(GetResponsiveButtonBaseSize(actionButton, ActionButtonMinWidth));
            }

            Size uniformButtonSize = new Size(
                actionButtons.Max(button => button.Width),
                actionButtons.Max(button => button.Height));
            int requiredButtonWidth = uniformButtonSize.Width;

            int actionButtonBaseLeft = _actionButtonBaseLeft > 0 ? _actionButtonBaseLeft : 7;
            int contentRightPadding = _actionButtonBaseRightMargin > 0 ? _actionButtonBaseRightMargin : actionButtonBaseLeft;
            int showPopupBaseLeft = _showPopupBaseLeft > 0 ? _showPopupBaseLeft : actionButtonBaseLeft;
            Size showPopupPreferredSize = chkShowPopup.GetPreferredSize(Size.Empty);
            int actionPanelHorizontalMargin = grpActions != null ? grpActions.Margin.Horizontal : 0;
            int requiredGroupWidth = Math.Max(
                requiredButtonWidth + actionButtonBaseLeft + contentRightPadding,
                showPopupPreferredSize.Width + showPopupBaseLeft + contentRightPadding);
            int requiredPanelWidth = (int)Math.Ceiling(Math.Max(
                _actionPanelBaseWidth > 0F ? _actionPanelBaseWidth : ActionPanelMinWidth,
                requiredGroupWidth + actionPanelHorizontalMargin));

            if (tlpJobs.ColumnStyles.Count > 1)
            {
                tlpJobs.ColumnStyles[1].SizeType = SizeType.Absolute;
                tlpJobs.ColumnStyles[1].Width = requiredPanelWidth;
                tlpJobs.PerformLayout();
            }

            foreach (Button actionButton in actionButtons)
            {
                actionButton.Size = uniformButtonSize;
                actionButton.Left = actionButtonBaseLeft;
            }

            chkShowPopup.Left = showPopupBaseLeft;

            int abortButtonsBaseSpacing = _abortButtonsBaseSpacing > 0 ? _abortButtonsBaseSpacing : 6;
            int abortAllBaseBottomMargin = _abortAllBaseBottomMargin > 0 ? _abortAllBaseBottomMargin : 3;
            int desiredBottomPadding = Math.Max(contentRightPadding, abortAllBaseBottomMargin);

            btnAbortAll.Top = grpActions.ClientSize.Height - desiredBottomPadding - btnAbortAll.Height;
            btnAbort.Top = btnAbortAll.Top - abortButtonsBaseSpacing - btnAbort.Height;
        }

        private void ApplyProgressLayout()
        {
            int labelColumnWidth = new[]
            {
                lblCurrentTrack.GetPreferredWidth(),
                lblCurrentProgress.GetPreferredWidth(),
                lblTotalProgress.GetPreferredWidth()
            }.Max();

            int progressLabelBaseLeft = _progressLabelBaseLeft > 0 ? _progressLabelBaseLeft : 8;
            int contentBaseLeft = _progressContentBaseLeft > 0 ? _progressContentBaseLeft : 107;
            int currentTrackRightMargin = _currentTrackRightMarginBase > 0 ? _currentTrackRightMarginBase : 51;
            int progressBarRightMargin = _progressBarRightMarginBase > 0 ? _progressBarRightMarginBase : 51;
            int contentLeft = Math.Max(contentBaseLeft, progressLabelBaseLeft + labelColumnWidth + 14);
            int trackWidth = Math.Max(120, grpProgress.ClientSize.Width - contentLeft - currentTrackRightMargin);
            int progressWidth = Math.Max(120, grpProgress.ClientSize.Width - contentLeft - progressBarRightMargin);

            txtCurrentTrack.Left = contentLeft;
            txtCurrentTrack.Width = trackWidth;

            prgBrCurrent.Left = contentLeft;
            prgBrCurrent.Width = progressWidth;

            prgBrTotal.Left = contentLeft;
            prgBrTotal.Width = progressWidth;
        }
    }
}
