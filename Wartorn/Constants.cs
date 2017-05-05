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
        public static string MinorVersion = "3";
        public static string GetVersionNumber { get { return MajorVersion + "." + MinorVersion; } }
    }

    static class LayerDepth
    {
        public static float BackGround = 0.0f;
        public static float TerrainBase = 0.1f;
        public static float TerrainLower = 0.2f;
        public static float Unit = 0.5f;
        public static float TerrainUpper = 0.7f;
        public static float GuiBackground = 0.8f;
        public static float GuiLower = 0.9f;
        public static float GuiUpper = 1.0f;
    }

    static class Constants
    {
        public static int Width { get; set; }
        public static int Height { get; set; }
        public const int MapCellWidth = 48;
        public const int MapCellHeight = 48;
    }

    public enum Direction
    {
        NorthWest,  North   ,   NorthEast,
        West     ,  Center  ,   East,
        SouthWest,  South   ,   SouthEast
    }
}
