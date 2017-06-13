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
    static class SelectedMapCellBorderSpriteSourceRectangle
    {
        private static Dictionary<GameData.Owner, Rectangle> SelectedMapCellBorderSprite;

        public static void LoadSprite()
        {
            SelectedMapCellBorderSprite = new Dictionary<GameData.Owner, Rectangle>();

            GameData.Owner c = GameData.Owner.None;

            for (int x = 0; x < 5; x++)
            {
                SelectedMapCellBorderSprite.Add(c, new Rectangle(x * 144, 0, 144, 116));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(GameData.Owner t)
        {
            return SelectedMapCellBorderSprite[t];
        }
    }

    static class SelectedMapCellCapturePointSpriteSourceRectangle
    {
        private static Dictionary<GameData.Owner, Rectangle> SelectedMapCellCapturePointSprite;

        public static void LoadSprite()
        {
            SelectedMapCellCapturePointSprite = new Dictionary<GameData.Owner, Rectangle>();

            GameData.Owner c = GameData.Owner.Red;

            for (int x = 0; x < 4; x++)
            {
                SelectedMapCellCapturePointSprite.Add(c, new Rectangle(x * 49, 0, 49, 20));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(GameData.Owner t)
        {
            return SelectedMapCellCapturePointSprite[t];
        }
    }

    static class SelectedMapCellDefenseStarSpriteSourceRectangle
    {
        private static Dictionary<int, Rectangle> SelectedMapCellDefenseStarSprite;

        public static void LoadSprite()
        {
            SelectedMapCellDefenseStarSprite = new Dictionary<int, Rectangle>();

            SelectedMapCellDefenseStarSprite.Add(0, new Rectangle(0, 0, 0, 0));
            SelectedMapCellDefenseStarSprite.Add(1, new Rectangle(0, 0, 9, 9));
            SelectedMapCellDefenseStarSprite.Add(2, new Rectangle(0, 0, 21, 9));
            SelectedMapCellDefenseStarSprite.Add(3, new Rectangle(0, 0, 33, 9));
            SelectedMapCellDefenseStarSprite.Add(4, new Rectangle(0, 0, 45, 9));

        }

        public static Rectangle GetSpriteRectangle(int t)
        {
            return SelectedMapCellDefenseStarSprite[t];
        }
    }

    static class SelectedMapCellLoadedUnitSpriteSourceRectangle
    {
        private static Dictionary<GameData.Owner, Rectangle> SelectedMapCellLoadedUnitSprite;

        public static void LoadSprite()
        {
            SelectedMapCellLoadedUnitSprite = new Dictionary<GameData.Owner, Rectangle>();

            GameData.Owner c = GameData.Owner.Red;

            for (int x = 0; x < 4; x++)
            {
                SelectedMapCellLoadedUnitSprite.Add(c, new Rectangle(x * 39, 0, 39, 21));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(GameData.Owner t)
        {
            return SelectedMapCellLoadedUnitSprite[t];
        }
    }

    static class SelectedMapCellUnitInfoSpriteSourceRectangle
    {
        private static Dictionary<GameData.Owner, Rectangle> SelectedMapCellUnitInfoSprite;

        public static void LoadSprite()
        {
            SelectedMapCellUnitInfoSprite = new Dictionary<GameData.Owner, Rectangle>();

            GameData.Owner c = GameData.Owner.Red;

            for (int x = 0; x < 4; x++)
            {
                SelectedMapCellUnitInfoSprite.Add(c, new Rectangle(x * 132, 0, 132, 25));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(GameData.Owner t)
        {
            return SelectedMapCellUnitInfoSprite[t];
        }
    }
}