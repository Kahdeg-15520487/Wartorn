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
    enum SpriteSheetDirectionArrow
    {
        Horizontal,Vertical,DownRight,DownLeft,UpRight,UpLeft,Left,Right,Down,Up
    }

    static class DirectionArrowSpriteSourceRectangle
    {
        private static Dictionary<SpriteSheetDirectionArrow, Rectangle> DirArrowSprite;

        public static void LoadSprite()
        {
            DirArrowSprite = new Dictionary<SpriteSheetDirectionArrow, Rectangle>();
            
            for (int i = 0; i < 10; i++)
            {
                DirArrowSprite.Add((SpriteSheetDirectionArrow)i, new Rectangle(i * 48, 0, 48, 48));
            }
        }

        public static Rectangle GetSpriteRectangle(SpriteSheetDirectionArrow t)
        {
            return DirArrowSprite[t];
        }

        public static Rectangle GetSpriteRectangle(Direction dir,bool isVertical = true)
        {
            Rectangle result = Rectangle.Empty;
            switch (dir)
            {
                case Direction.North:
                    result = DirArrowSprite[SpriteSheetDirectionArrow.Up];
                    break;
                case Direction.South:
                    result = DirArrowSprite[SpriteSheetDirectionArrow.Down];
                    break;

                case Direction.West:
                    result = DirArrowSprite[SpriteSheetDirectionArrow.Left];
                    break;
                case Direction.East:
                    result = DirArrowSprite[SpriteSheetDirectionArrow.Right];
                    break;

                case Direction.NorthWest:
                    result = DirArrowSprite[SpriteSheetDirectionArrow.DownLeft];
                    break;

                case Direction.NorthEast:
                    result = DirArrowSprite[SpriteSheetDirectionArrow.DownRight];
                    break;

                case Direction.SouthWest:
                    result = DirArrowSprite[SpriteSheetDirectionArrow.UpLeft];
                    break;
                
                case Direction.SouthEast:
                    result = DirArrowSprite[SpriteSheetDirectionArrow.UpRight];
                    break;

                case Direction.Center:
                    if (isVertical)
                    {
                        result = DirArrowSprite[SpriteSheetDirectionArrow.Vertical];
                    }
                    else
                    {
                        result = DirArrowSprite[SpriteSheetDirectionArrow.Horizontal];
                    }
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
