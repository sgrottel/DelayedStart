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
        public StartInfo StartInfo { get; set; }

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
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            SG.Utilities.OS.UrlUtility.OpenUrl("http://" + linkLabel1.Text);
        }
    }

}
