using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wartorn.GameData;
using Wartorn.Storage;

namespace Wartorn
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread, SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
			var handler = new Handler();
			handler.Args = args;
			Application.Run(handler);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            string message = "==========" + Environment.NewLine +
                ex.Message + Environment.NewLine +
                ex.StackTrace + Environment.NewLine +
                ex.TargetSite;
            string logfilename = "crashlog_" + DateTime.Now.ToString(@"dd_MM_yyyy_HH_mm") + ".txt";
            try
            {
                File.WriteAllText(logfilename, message);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "crashhandler.exe";
                startInfo.Arguments = logfilename;
                Process.Start(startInfo);
            }
            catch (Exception exx)
            {
                throw exx;
            }
        }
    }
}
