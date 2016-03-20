using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SG.DelayedStart {

    /// <summary>
    /// Application main class
    /// </summary>
    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //StartInfo si = new StartInfo();
            //si.Application = "Notepad.exe";
            //si.WorkingDirectory = "Here";
            //si.Arguments = "Test";
            //si.Delay = TimeSpan.FromSeconds(10);

            //si.Save("D:\\tmp\\test.dssi", true);

            //Application.Run(new Form1());
        }

    }

}
