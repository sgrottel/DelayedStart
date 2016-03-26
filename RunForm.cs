using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SG.Utilities;

namespace SG.DelayedStart {

    /// <summary>
    /// The run form waiting and then starting the process
    /// </summary>
    public partial class RunForm : Form {

        /// <summary>
        /// The Start info object
        /// </summary>
        private StartInfo info = null;

        /// <summary>
        /// Gets or sets the start info object
        /// </summary>
        public StartInfo StartInfo {
            get { return info; }
            set {
                if (info != value) {
                    if (info != null) {

                    }
                    info = value;
                    if (info != null) {
                        if (string.IsNullOrEmpty(info.Application)) {
                            Text = Application.ProductName;
                        } else {
                            Text = Application.ProductName + " - " + System.IO.Path.GetFileNameWithoutExtension(info.Application);
                        }
                        label1.Text = info.Application;
                        label2.Text = info.Delay.ToHumanReadableString();
                        try {
                            pictureBox1.Image = IconUtility.LoadIcon(info.Application, pictureBox1.Size, false).ToBitmap();
                            pictureBox1.Visible = true;

                        } catch {
                            pictureBox1.Visible = false;
                        }

                    } else {
                        Text = Application.ProductName;
                        label1.Text = "";
                        label2.Text = "";
                        pictureBox1.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the start info file name
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The progress report
        /// </summary>
        private ProgressReport progress = new ProgressReport();

        /// <summary>
        /// Connects the progress report to the progressBar
        /// </summary>
        private SG.Utilities.Forms.ProgressBarConnector pbcon = new Utilities.Forms.ProgressBarConnector();

        /// <summary>
        /// Connects the progress report to the taskbaricon
        /// </summary>
        private SG.Utilities.OS.TaskBarIconProgressBar tbpb = new Utilities.OS.TaskBarIconProgressBar();

        /// <summary>
        /// Ctor
        /// </summary>
        public RunForm() {
            InitializeComponent();
            Icon = Properties.Resources.ArrorRight;
            FormClosed += Program.LastFormClosed;

            pbcon.Report = progress;
            pbcon.ProgressBar = progressBar1;

            tbpb.Report = progress;
            tbpb.NativeWindowHandle = Handle;

            if (info == null) {
                info = new StartInfo();
                StartInfo = null;
            }
        }

        /// <summary>
        /// Lazy init when form is shown for the first time
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void RunForm_Shown(object sender, EventArgs e) {
            if (info == null) this.Close(); // ack

            progress.Start();

            startTime = DateTime.Now;
            delayTimer.Enabled = true;
        }

        /// <summary>
        /// The start time
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// The delay timer update tick
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void delayTimer_Tick(object sender, EventArgs e) {
            DateTime n = DateTime.Now;
            TimeSpan s = n - startTime;

            double p = s.TotalSeconds / info.Delay.TotalSeconds;
            if (p < 0.0) p = 0.0;
            if (p > 1.0) {
                delayTimer.Enabled = false;
                progress.Progress = 1.0;
                progress.Stop(true);
                progressBar1.Value = progressBar1.Maximum;
                button1.PerformClick();

            } else {
                TimeSpan r = info.Delay - s;
                label2.Text = r.ToHumanReadableString();
                progress.Progress = p;
            }

        }

        /// <summary>
        /// Starts the process now!
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void button1_Click(object sender, EventArgs e) {
            delayTimer.Enabled = false;
            label2.Text = "Starting...";
            button2.Enabled = false;
            try {
                Process proc = info.Start(true);
                if (proc == null) {
                    throw new Exception("No process object returned");
                }

                // Delayed process is now running. We can close.
                this.Close();

            } catch(Exception ex) {
                label2.Text = "Starting failed: " + ex.ToString();
                MessageBox.Show("Failed to start " + System.IO.Path.GetFileNameWithoutExtension(info.Application) + ": " + ex.ToString(),
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd, uint Msg,uint wParam, uint lParam);

        /// <summary>
        /// Button to abort the delayed start
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void button2_Click(object sender, EventArgs e) {
            delayTimer.Enabled = false;
            label2.Text = "Aborted";
            button2.Enabled = false;
            SendMessage(progressBar1.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0002, //PBST_ERROR
                0);
            tbpb.NativeWindowHandle = IntPtr.Zero;
        }

        /// <summary>
        /// Button to switch to the EditForm to edit the StartInfo
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void button3_Click(object sender, EventArgs e) {
            delayTimer.Enabled = false;
            label2.Text = "Aborted";
            button2.Enabled = false;
            SendMessage(progressBar1.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0003, //PBST_PAUSED
                0);
            tbpb.NativeWindowHandle = IntPtr.Zero;

            FormClosed -= Program.LastFormClosed;
            EditForm edit = new EditForm();
            edit.StartInfo = this.StartInfo;
            edit.Filename = this.Filename;
            edit.Show();
            Close();
        }

    }

}
