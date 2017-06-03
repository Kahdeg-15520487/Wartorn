using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Utility;
using Wartorn.Drawing;
using Newtonsoft.Json;
using Wartorn.Drawing.Animation;

namespace Wartorn.SpriteRectangle
{
    static class BuyMenuFactorySpriteSourceRectangle
    {
        private static Dictionary<Owner, Rectangle> BuyMenuFactorySprite;

        public static void LoadSprite()
        {
            BuyMenuFactorySprite = new Dictionary<Owner, Rectangle>();

            Owner c = Owner.Red;

            for (int x = 0; x < 4; x++)
            {
                BuyMenuFactorySprite.Add(c, new Rectangle(x * 146, 0, 146, 180));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(Owner t)
        {
            return BuyMenuFactorySprite[t];
        }
    }

    static class BuyMenuAirportHarborSpriteSourceRectangle
    {
        private static Dictionary<Owner, Rectangle> BuyMenuAirportHarborSprite;

        public static void LoadSprite()
        {
            BuyMenuAirportHarborSprite = new Dictionary<Owner, Rectangle>();

            Owner c = Owner.Red;

            for (int x = 0; x < 4; x++)
            {
                BuyMenuAirportHarborSprite.Add(c, new Rectangle(x * 146, 0, 146, 112));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(Owner t)
        {
            return BuyMenuAirportHarborSprite[t];
        }
    }
}
