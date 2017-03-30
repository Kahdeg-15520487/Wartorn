using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wartorn
{
    public class SpriteSheetSourceRectangle
    {
        public static readonly Rectangle
            AirPort     = new Rectangle(0   ,0  ,60 ,60),
            City        = new Rectangle(60  ,0  ,60 ,60),
            Factory     = new Rectangle(120 ,0  ,60 ,60),
            Plain       = new Rectangle(180 ,0  ,60 ,60);
    }
}
