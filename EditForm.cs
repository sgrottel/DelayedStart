using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SG.DelayedStart {

    /// <summary>
    /// The form for editing StartInfo objects
    /// </summary>
    public partial class EditForm : Form {

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
        }

    }

}
