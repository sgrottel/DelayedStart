﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SG.DelayedStart {

    /// <summary>
    /// Start info for the application to delayed-start
    /// </summary>
    [XmlRoot("DelayedStartInfo")]
    public class StartInfo {

        /// <summary>
        /// File format extension string without leading period
        /// </summary>
        public const string FileFormatExt = "dsi";

        /// <summary>
        /// File format filter string used for file dialogs
        /// </summary>
        public const string FileFormatFilter = "Delayed Start Info Files|*." + FileFormatExt;

        /// <summary>
        /// Path to the application executable
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// The startup working directory, or null, if the application directory should be used
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// The command line arguments
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// The application start delay
        /// </summary>
        [XmlElement(Type = typeof(SG.Utilities.XmlTimeSpan))]
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Loads StartInfo from a file
        /// </summary>
        /// <param name="path">Path to the file to load</param>
        /// <param name="throwOnError">If true, exceptions are thrown on errors</param>
        /// <returns>The loaded StartInfo or Null</returns>
        public static StartInfo Load(string path, bool throwOnError) {
            if (!throwOnError) {
                try {
                    return Load(path, true);
                } catch { }
                return null;
            }

            using (TextReader reader = new StreamReader(path)) {
                XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                return (StartInfo)ser.Deserialize(reader);
            }
        }

        /// <summary>
        /// Saves this StartInfo
        /// </summary>
        /// <param name="path">Path to save at</param>
        /// <param name="throwOnError">If true, exceptions are thrown on errors</param>
        /// <returns>True on success, false on failure</returns>
        public bool Save(string path, bool throwOnError) {
            if (!throwOnError) {
                try {
                    return Save(path, true);
                } catch { }
                return false;
            }

            XmlWriterSettings writerSets = new XmlWriterSettings() {
                CloseOutput = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t"
            };
            using (XmlWriter writer = XmlWriter.Create(path, writerSets)) {
                XmlSerializer ser = new XmlSerializer(typeof(StartInfo));
                ser.Serialize(writer, this);
            }
            return true;
        }

        /// <summary>
        /// Starts the described process
        /// </summary>
        /// <param name="throwOnError">If true, exceptions are thrown on errors</param>
        /// <returns>Null on error, the started process object on success</returns>
        public Process Start(bool throwOnError) {
            if (!throwOnError) {
                try {
                    return Start(true);
                } catch { }
                return null;
            }

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Application;
            psi.WorkingDirectory = WorkingDirectory;
            psi.Arguments = Arguments;

            return Process.Start(psi);
        }

    }

}
