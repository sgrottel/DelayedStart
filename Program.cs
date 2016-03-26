using Microsoft.Win32;
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
        /// <param name="args">The command line arguments</param>
        [STAThread]
        static int Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool doEdit = false;
            StartInfo info = null;
            string infoFile = string.Empty;

            if ((args != null) && (args.Length > 0)) {
                bool regFileTypes = false;
                bool unregFileTypes = false;
                IWin32Window ownerWindow = null;

                foreach (string arg in args) {
                    if (arg.Equals("-RegFileTypes", StringComparison.CurrentCultureIgnoreCase)) {
                        regFileTypes = true;
                    } else if (arg.Equals("-UnregFileTypes", StringComparison.CurrentCultureIgnoreCase)) {
                        unregFileTypes = true;
                    } else if (arg.StartsWith("ownerHWnd=", StringComparison.CurrentCultureIgnoreCase)) {
                        if (regFileTypes || unregFileTypes) {
                            ulong hVal = 0;
                            if (ulong.TryParse(arg.Substring(10), out hVal)) {
                                ownerWindow = NativeWindow.FromHandle((IntPtr)hVal);
                            }
                        }
                    } else if (arg.Equals("-Edit", StringComparison.CurrentCultureIgnoreCase)) {
                        doEdit = true;
                    } else if (info == null) {
                        try {
                            info = StartInfo.Load(arg, true);
                            infoFile = arg;
                        } catch(Exception ex) {
                            MessageBox.Show("Failed to load " + arg + ": " + ex.ToString(),
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                if (regFileTypes) {
                    try {
                        registerFileTypes();
                        MessageBox.Show(ownerWindow, "File type *." + StartInfo.FileFormatExt + " registered.",
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return 0;
                    } catch (Exception ex) {
                        MessageBox.Show(ownerWindow, "Failed to register *." + StartInfo.FileFormatExt + " file type:\n" + ex.ToString(),
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return -1;
                }
                if (unregFileTypes) {
                    try {
                        unregisterFileTypes();
                        MessageBox.Show(ownerWindow, "File type *." + StartInfo.FileFormatExt + " unregistered.",
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return 0;
                    } catch (Exception ex) {
                        MessageBox.Show(ownerWindow, "Failed to unregister *." + StartInfo.FileFormatExt + " file type:\n" + ex.ToString(),
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return -1;
                }
            }

            if ((info != null) && (!doEdit)) {
                RunForm f = new RunForm();
                f.StartInfo = info;
                f.Filename = infoFile;
                f.Show();
            } else {
                EditForm f = new EditForm();
                f.StartInfo = info;
                f.Filename = infoFile;
                f.Show();
            }

            Application.Run();

            return 0;
        }

        /// <summary>
        /// Closes the application when the last form closed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        static internal void LastFormClosed(object sender, FormClosedEventArgs e) {
            Application.ExitThread();
        }

        /// <summary>
        /// Adds the file type information to the registry
        /// </summary>
        static void registerFileTypes() {
            RegistryKey ext_dsi = Registry.ClassesRoot.CreateSubKey("." + StartInfo.FileFormatExt);
            ext_dsi.SetValue(null, "DelayedStart.StartInfo");
            ext_dsi.Close();

            RegistryKey desc_dsi = Registry.ClassesRoot.CreateSubKey("DelayedStart.StartInfo");
            desc_dsi.SetValue(null, "DelayedStart Start Info File");

            RegistryKey icon_desc_dsi = desc_dsi.CreateSubKey("DefaultIcon");
            icon_desc_dsi.SetValue(null, "\"" + Application.ExecutablePath + "\",-2");
            icon_desc_dsi.Close();

            RegistryKey shell_desc_dsi = desc_dsi.CreateSubKey("shell");
            RegistryKey open_shell_desc_dsi = shell_desc_dsi.CreateSubKey("open");
            RegistryKey cmd_open_shell_desc_dsi = open_shell_desc_dsi.CreateSubKey("command");
            cmd_open_shell_desc_dsi.SetValue(null, "\"" + Application.ExecutablePath + "\" \"%1\"");
            cmd_open_shell_desc_dsi.Close();
            open_shell_desc_dsi.Close();

            RegistryKey edit_shell_desc_dsi = shell_desc_dsi.CreateSubKey("edit");
            RegistryKey cmd_edit_shell_desc_dsi = edit_shell_desc_dsi.CreateSubKey("command");
            cmd_edit_shell_desc_dsi.SetValue(null, "\"" + Application.ExecutablePath + "\" -Edit \"%1\"");
            cmd_edit_shell_desc_dsi.Close();
            edit_shell_desc_dsi.Close();

            shell_desc_dsi.Close();
            desc_dsi.Close();
        }

        /// <summary>
        /// Removes the file type information from the registry
        /// </summary>
        static void unregisterFileTypes() {
            Registry.ClassesRoot.DeleteSubKeyTree("." + StartInfo.FileFormatExt, false);
            Registry.ClassesRoot.DeleteSubKeyTree("DelayedStart.StartInfo", false);
        }

    }

}
