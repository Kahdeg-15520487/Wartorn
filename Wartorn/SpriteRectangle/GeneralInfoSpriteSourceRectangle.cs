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
    static class GeneralInfoBorderSpriteSourceRectangle
    {
        private static Dictionary<GameData.Owner, Rectangle> GeneralInfoBorderSprite;

        public static void LoadSprite()
        {
            GeneralInfoBorderSprite = new Dictionary<GameData.Owner, Rectangle>();

            GameData.Owner c = GameData.Owner.None;

            for (int x = 0; x < 5; x++)
            {
                GeneralInfoBorderSprite.Add(c, new Rectangle(x * 144, 0, 144, 116));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(GameData.Owner t)
        {
            return GeneralInfoBorderSprite[t];
        }
    }

    static class GeneralInfoCapturePointSpriteSourceRectangle
    {
        private static Dictionary<GameData.Owner, Rectangle> GeneralInfoCapturePointSprite;

        public static void LoadSprite()
        {
            GeneralInfoCapturePointSprite = new Dictionary<GameData.Owner, Rectangle>();

            GameData.Owner c = GameData.Owner.Red;

            for (int x = 0; x < 4; x++)
            {
                GeneralInfoCapturePointSprite.Add(c, new Rectangle(x * 49, 0, 49, 20));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(GameData.Owner t)
        {
            return GeneralInfoCapturePointSprite[t];
        }
    }

    static class GeneralInfoDefenseStarSpriteSourceRectangle
    {
        private static Dictionary<int, Rectangle> GeneralInfoDefenseStarSprite;

        public static void LoadSprite()
        {
            GeneralInfoDefenseStarSprite = new Dictionary<int, Rectangle>();

            GeneralInfoDefenseStarSprite.Add(0, new Rectangle(0, 0, 0, 0));
            GeneralInfoDefenseStarSprite.Add(1, new Rectangle(0, 0, 9, 9));
            GeneralInfoDefenseStarSprite.Add(2, new Rectangle(0, 0, 21, 9));
            GeneralInfoDefenseStarSprite.Add(3, new Rectangle(0, 0, 33, 9));
            GeneralInfoDefenseStarSprite.Add(4, new Rectangle(0, 0, 45, 9));

        }

        public static Rectangle GetSpriteRectangle(int t)
        {
            return GeneralInfoDefenseStarSprite[t];
        }
    }

    static class GeneralInfoLoadedUnitSpriteSourceRectangle
    {
        private static Dictionary<GameData.Owner, Rectangle> GeneralInfoLoadedUnitSprite;

        public static void LoadSprite()
        {
            GeneralInfoLoadedUnitSprite = new Dictionary<GameData.Owner, Rectangle>();

            GameData.Owner c = GameData.Owner.Red;

            for (int x = 0; x < 4; x++)
            {
                GeneralInfoLoadedUnitSprite.Add(c, new Rectangle(x * 39, 0, 39, 21));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(GameData.Owner t)
        {
            return GeneralInfoLoadedUnitSprite[t];
        }
    }

    static class GeneralInfoUnitInfoSpriteSourceRectangle
    {
        private static Dictionary<GameData.Owner, Rectangle> GeneralInfoUnitInfoSprite;

        public static void LoadSprite()
        {
            GeneralInfoUnitInfoSprite = new Dictionary<GameData.Owner, Rectangle>();

            GameData.Owner c = GameData.Owner.Red;

            for (int x = 0; x < 4; x++)
            {
                GeneralInfoUnitInfoSprite.Add(c, new Rectangle(x * 132, 0, 132, 25));
                c = c.Next();
            }
        }

        public static Rectangle GetSpriteRectangle(GameData.Owner t)
        {
            return GeneralInfoUnitInfoSprite[t];
        }
    }
}