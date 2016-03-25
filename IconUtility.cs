using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SG.DelayedStart {

    static public class IconUtility {

        [DllImport("Shell32.dll", EntryPoint = "SHDefExtractIconW")]
        static extern private int SHDefExtractIconW([MarshalAs(UnmanagedType.LPTStr)] string pszIconFile, int iIndex, uint uFlags, ref IntPtr phiconLarge, ref IntPtr phiconSmall, uint nIconSize);

        [DllImport("user32.dll", EntryPoint = "DestroyIcon")]
        static extern private bool DestroyIcon(IntPtr hIcon);

        static private uint makeSize(uint low, uint hi) {
            return (hi << 16) + (low & 0xffff);
        }

        static public Icon LoadIcon(string path, Size size, bool exact) {
            int reqSize = Math.Min(size.Width, size.Height);
            IntPtr sml = IntPtr.Zero, lrg = IntPtr.Zero;
            uint sizes = makeSize(256u, (uint)reqSize);

            int retval = SHDefExtractIconW(path, 0, 0, ref lrg, ref sml, sizes);
            if (lrg != IntPtr.Zero) DestroyIcon(lrg);
            if ((retval == 0) && (sml != IntPtr.Zero)) {
                Icon i = new Icon(Icon.FromHandle(sml), new Size(reqSize, reqSize));
                DestroyIcon(sml);
                return i;
            }

            // if all else fails
            return Icon.ExtractAssociatedIcon(path);
        }

    }

}
