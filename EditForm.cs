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
                return null;
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
                if (ts >= 60 * 60) {
                    int h = ts / 60 * 60;
                    ts -= h * 60 * 60;
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
        public string Filename { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public EditForm() {
            InitializeComponent();
            Icon = Properties.Resources.ArrorRight;
            FormClosed += Program.LastFormClosed;

            label9.Text = AssemblyTitle + " (Ver. " + AssemblyVersion + ") " + AssemblyCopyright;
            label9.Left = linkLabel1.Left - label9.Width;

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
    }

}
