using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility_Project
{
    public static class Logger
    {
        public static void Log(Exception e)
        {
            File.WriteAllText("crashlog.txt", DateTime.Now.ToString(@"dd\/MM\/yyyy HH:mm") + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.TargetSite);
        }

        public static void Log(string msg)
        {
            File.WriteAllText("crashlog.txt", DateTime.Now.ToString(@"dd\/MM\/yyyy HH:mm") + Environment.NewLine + msg);
        }
    }
}
