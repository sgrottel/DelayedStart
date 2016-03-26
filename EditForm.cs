using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SG.DelayedStart {

    /// <summary>
    /// The form for editing StartInfo objects
    /// </summary>
    public partial class EditForm : Form {

        #region Assembly Attribute Accessors

        public string AssemblyTitle {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0) {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "") {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion {
            get {
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                int c = 2;
                if (v.Build > 0) c = 3;
                if (v.Revision > 0) c = 4;
                return v.ToString(c);
            }
        }

        public string AssemblyCopyright {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the start info object
        /// </summary>
        public StartInfo StartInfo {
            get {
                StartInfo si = new StartInfo();
                si.Application = textBox1.Text;
                si.WorkingDirectory = textBox2.Text;
                if (radioButton1.Checked) si.WorkingDirectory = "";
                si.Arguments = textBox3.Text;
                int h, m, s;
                if (!int.TryParse(textBox4.Text, out h)) h = 0;
                if (!int.TryParse(textBox5.Text, out m)) m = 0;
                if (!int.TryParse(textBox6.Text, out s)) s = 0;
                si.Delay = new TimeSpan(h, m, s);
                return si;
            }
            set {
                if (value == null) {
                    toolStripButton1_Click(null, null);
                    return;
                }
                textBox1.Text = value.Application;
                textBox2.Text = value.WorkingDirectory;
                radioButton1.Checked = string.IsNullOrWhiteSpace(value.WorkingDirectory);
                textBox3.Text = value.Arguments;
                int ts = (int)value.Delay.TotalSeconds;
                if (ts < 0) ts = 0;
                if (ts >= (60 * 60)) {
                    int h = ts / (60 * 60);
                    ts -= h * (60 * 60);
                    textBox4.Text = h.ToString();
                } else {
                    textBox4.Text = "";
                }
                if (ts >= 60) {
                    int m = ts / 60;
                    ts -= m * 60;
                    textBox5.Text = m.ToString();
                } else {
                    textBox5.Text = "";
                }
                textBox6.Text = ts.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the start info file name
        /// </summary>
        public string Filename {
            get { return openFileDialog1.FileName; }
            set { openFileDialog1.FileName = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public EditForm() {
            InitializeComponent();
            Icon = Properties.Resources.ArrorRight;
            FormClosed += Program.LastFormClosed;

            openFileDialog1.DefaultExt = StartInfo.FileFormatExt;
            openFileDialog1.Filter = StartInfo.FileFormatFilter + "|All Files|*.*";
            saveFileDialog1.DefaultExt = StartInfo.FileFormatExt;
            saveFileDialog1.Filter = StartInfo.FileFormatFilter + "|All Files|*.*";

            string pext = Environment.GetEnvironmentVariable("PATHEXT");
            if (!string.IsNullOrWhiteSpace(pext)) {
                openFileDialog2.Filter = "Executable Files|*" + pext.Replace(";.", ";*.") + "|All Files|*.*";
            }

            label9.Text = AssemblyTitle + " (Ver. " + AssemblyVersion + ") " + AssemblyCopyright;
            label9.Left = linkLabel1.Left - label9.Width;

            if ((SG.Utilities.Forms.Elevation.IsElevationRequired) && (!SG.Utilities.Forms.Elevation.IsElevated)) {
                // Icons
                Icon i = null;
                string path = System.IO.Path.Combine(Environment.SystemDirectory, "UserAccountControlSettings.exe");
                if (System.IO.File.Exists(path)) {
                    i = IconUtility.LoadIcon(path, toolStrip1.ImageScalingSize, true);
                }
                if (i == null) {
                    path = System.IO.Path.Combine(Environment.SystemDirectory, "SmartScreenSettings.exe");
                    if (System.IO.File.Exists(path)) {
                        i = IconUtility.LoadIcon(path, toolStrip1.ImageScalingSize, true);
                    }
                }
                if (i == null) {
                    path = System.IO.Path.Combine(Environment.SystemDirectory, "UserAccountControlSettings.exe");
                    if (System.IO.File.Exists(path)) {
                        i = IconUtility.LoadIcon(path, toolStrip1.ImageScalingSize, false);
                    }
                }
                if (i == null) {
                    path = System.IO.Path.Combine(Environment.SystemDirectory, "SmartScreenSettings.exe");
                    if (System.IO.File.Exists(path)) {
                        i = IconUtility.LoadIcon(path, toolStrip1.ImageScalingSize, false);
                    }
                }
                if (i != null) {
                    if (i.Size != toolStrip1.ImageScalingSize) {
                        i = new Icon(i, toolStrip1.ImageScalingSize);
                    }
                    Bitmap b = i.ToBitmap();
                    registerFileTypesToolStripMenuItem.Image = b;
                    unregisterFileTypesToolStripMenuItem.Image = b;
                }
            }

            toolStripButton1_Click(null, null);
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            radioButton1.Checked = true;
            Filename = ""; // < Not sure about that, but seem legit
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            SG.Utilities.OS.UrlUtility.OpenUrl("http://" + linkLabel1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {
            radioButton2.Checked = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            string ofn = Filename;
            try {
                if (!string.IsNullOrWhiteSpace(openFileDialog1.FileName)) {
                    openFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                }
            } catch { }
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                try {
                    StartInfo = StartInfo.Load(openFileDialog1.FileName, true);
                } catch (Exception ex) {
                    MessageBox.Show("Failed to load " + openFileDialog1.FileName + ": " + ex.ToString(),
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Filename = ofn;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(Filename)) {
                saveAsToolStripMenuItem_Click(sender, e);
                return;
            }
            try {
                StartInfo.Save(Filename, true);
            } catch (Exception ex) {
                MessageBox.Show("Failed to save " + Filename + ": " + ex.ToString(),
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            saveFileDialog1.FileName = Filename;
            try {
                if (!string.IsNullOrWhiteSpace(saveFileDialog1.FileName)) {
                    saveFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(saveFileDialog1.FileName);
                }
            } catch { }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                try {
                    StartInfo.Save(saveFileDialog1.FileName, true);
                    Filename = saveFileDialog1.FileName;
                } catch (Exception ex) {
                    MessageBox.Show("Failed to save " + Filename + ": " + ex.ToString(),
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void startDelayedToolStripMenuItem_Click(object sender, EventArgs e) {
            FormClosed -= Program.LastFormClosed;
            RunForm run = new RunForm();
            run.StartInfo = this.StartInfo;
            run.Filename = this.Filename;
            run.Show();
            Close();
        }

        private void startNowToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                var proc = StartInfo.Start(true);
                if (proc == null) {
                    throw new Exception("No process object returned");
                }
            } catch (Exception ex) {
                MessageBox.Show("Failed to start " + textBox1.Text + ": " + ex.ToString(),
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            openFileDialog2.FileName = textBox1.Text;
            try {
                if (!string.IsNullOrWhiteSpace(openFileDialog2.FileName)) {
                    openFileDialog2.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog2.FileName);
                }
            } catch { }
            if (openFileDialog2.ShowDialog() == DialogResult.OK) {
                textBox1.Text = openFileDialog2.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            folderBrowserDialog1.SelectedPath = radioButton1.Checked ? textBox1.Text : textBox2.Text;
            try {
                if (!string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath)) {
                    if (System.IO.File.Exists(folderBrowserDialog1.SelectedPath)) {
                        if (!System.IO.Directory.Exists(folderBrowserDialog1.SelectedPath)) {
                            folderBrowserDialog1.SelectedPath = System.IO.Path.GetDirectoryName(folderBrowserDialog1.SelectedPath);
                        }
                    }
                }
            } catch { }
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                radioButton2.Checked = true;
            }
        }

        private bool updateLock = false;

        private void textBox4_TextChanged(object sender, EventArgs e) {
            if (updateLock) return;
            updateLock = true;

            int h, m, s;
            if (!int.TryParse(textBox4.Text, out h)) h = -1;
            if (!int.TryParse(textBox5.Text, out m)) m = -1;
            if (!int.TryParse(textBox6.Text, out s)) s = -1;

            int ts = (int)(new TimeSpan(Math.Max(0, h), Math.Max(0, m), Math.Max(0, s)).TotalSeconds);

            if (ts < 0) ts = 0;
            int nh = ts / (60 * 60);
            ts -= nh * (60 * 60);
            int nm = ts / 60;
            ts -= nm * 60;
            int ns = ts;

            if ((sender != textBox4) && ((h < 0) || (h != nh))) {
                textBox4.Text = (nh == 0) ? "" : nh.ToString();
            }
            if ((sender != textBox5) && ((m < 0) || (m != nm))) {
                textBox5.Text = (nm == 0) ? "" : nm.ToString();
            }
            if ((sender != textBox6) && ((s < 0) || (s != ns))) {
                textBox6.Text = ns.ToString();
            }

            updateLock = false;
        }

        private void textBox4_Leave(object sender, EventArgs e) {
            textBox4_TextChanged(null, e);
        }

        private void registerFileTypesToolStripMenuItem_Click(object sender, EventArgs e) {
            if ((SG.Utilities.Forms.Elevation.IsElevationRequired) && (!SG.Utilities.Forms.Elevation.IsElevated)) {
                SG.Utilities.Forms.Elevation.RestartElevated("-RegFileTypes ownerHWnd=" + this.Handle.ToString());
            } else {
                try {
                    Program.registerFileTypes();
                    MessageBox.Show("File type *." + StartInfo.FileFormatExt + " registered.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                } catch (Exception ex) {
                    MessageBox.Show("Failed to register *." + StartInfo.FileFormatExt + " file type:\n" + ex.ToString(),
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void unregisterFileTypesToolStripMenuItem_Click(object sender, EventArgs e) {
            if ((SG.Utilities.Forms.Elevation.IsElevationRequired) && (!SG.Utilities.Forms.Elevation.IsElevated)) {
                SG.Utilities.Forms.Elevation.RestartElevated("-UnregFileTypes ownerHWnd=" + this.Handle.ToString());
            } else {
                try {
                    Program.unregisterFileTypes();
                    MessageBox.Show("File type *." + StartInfo.FileFormatExt + " unregistered.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                } catch (Exception ex) {
                    MessageBox.Show("Failed to unregister *." + StartInfo.FileFormatExt + " file type:\n" + ex.ToString(),
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

}
