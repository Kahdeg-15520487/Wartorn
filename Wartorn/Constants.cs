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
    namespace Utility
    {
        static class Constants
        {
            public static int Width { get; set; }
            public static int Height { get; set; }
            public static SpriteFont defaultFont { get; set; }
            public static Texture2D spriteSheet { get; set; }
            public static Texture2D UIspriteSheet { get; set; }
        }
    }
}
