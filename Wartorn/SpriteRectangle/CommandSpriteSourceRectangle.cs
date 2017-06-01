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
    public enum SpriteSheetCommand
    {
        Wait,
        Fire,
        Capt,
        Load,
        Drop,
        Rise,
        Dive,
        Supply
    }

    public enum SpriteSheetCommandSlot
    {
        oneslotblue,
        twoslotblue,
        threeslotblue,
        oneslotred,
        twoslotred,
        threeslotred
    }

    public class CommandSpriteSourceRectangle
    {
        private static Dictionary<SpriteSheetCommand, Rectangle> CommandSprite;
        private static Dictionary<SpriteSheetCommandSlot,Rectangle> CommandSlotSprite;

        public static void LoadSprite()
        {
            CommandSprite = new Dictionary<SpriteSheetCommand, Rectangle>();

            SpriteSheetCommand c = SpriteSheetCommand.Wait;

            for (int y = 0; y < 8; y++)
            {
                CommandSprite.Add(c, new Rectangle(112, y * 16, 48, 16));
                c = c.Next();
            }

            CommandSlotSprite = new Dictionary<SpriteSheetCommandSlot, Rectangle>();

            CommandSlotSprite.Add(SpriteSheetCommandSlot.oneslotblue, new Rectangle(0,0,56,32));
            CommandSlotSprite.Add(SpriteSheetCommandSlot.twoslotblue, new Rectangle(0,32,56,48));
            CommandSlotSprite.Add(SpriteSheetCommandSlot.threeslotblue, new Rectangle(0,80,56,64));
            CommandSlotSprite.Add(SpriteSheetCommandSlot.oneslotred, new Rectangle(56,0,56,32));
            CommandSlotSprite.Add(SpriteSheetCommandSlot.twoslotred, new Rectangle(56,32,56,48));
            CommandSlotSprite.Add(SpriteSheetCommandSlot.threeslotred, new Rectangle(56, 80, 56, 64));
        }

        public static Rectangle GetSprite(SpriteSheetCommand t)
        {
            return CommandSprite[t];
        }

        public static Rectangle GetSprite(Command t)
        {
            SpriteSheetCommand temp = (SpriteSheetCommand)((int)t - 1);
            return CommandSprite[temp];
        }

        public static Rectangle GetSprite(SpriteSheetCommandSlot t)
        {
            return CommandSlotSprite[t];
        }

        public static Rectangle GetSprite(int cmdcount,Owner color)
        {
            SpriteSheetCommandSlot result = SpriteSheetCommandSlot.oneslotblue;
            if (color == Owner.Red)
            {
                switch (cmdcount)
                {
                    case 1:
                        result = SpriteSheetCommandSlot.oneslotblue;
                        break;
                    case 2:
                        result = SpriteSheetCommandSlot.twoslotblue;
                        break;
                    case 3:
                        result = SpriteSheetCommandSlot.threeslotblue;
                        break;
                    default:
                        break;
                }
            }
            if (color == Owner.Blue)
            {
                switch (cmdcount)
                {
                    case 1:
                        result = SpriteSheetCommandSlot.oneslotred;
                        break;
                    case 2:
                        result = SpriteSheetCommandSlot.twoslotred;
                        break;
                    case 3:
                        result = SpriteSheetCommandSlot.threeslotred;
                        break;
                    default:
                        break;
                }
            }

            return CommandSlotSprite[result];
        }

        public static Command GetCommand(Rectangle r)
        {
            return (Command)(r.Y / 16 + 1);
        }

        public static SpriteSheetCommand GetSpriteSheetCommand(Rectangle r)
        {
            return (SpriteSheetCommand)(r.Y / 16);
        }
    }
}
