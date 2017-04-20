using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wartorn
{
    public static class VersionNumber
    {
        public static string MajorVersion = "0";
        public static string MinorVersion = "1";
        public static string GetVersionNumber { get { return MajorVersion + "." + MinorVersion; } }
    }

    static class LayerDepth
    {
        public static float BackGround = 0.0f;
        public static float TerrainLower = 0.1f;
        public static float TerrainUpper = 0.2f;
        public static float Unit = 0.3f;
        public static float GuiLower = 0.9f;
        public static float GuiUpper = 1.0f;
    }

    static class Constants
    {
        public static int Width { get; set; }
        public static int Height { get; set; }
    }
}
