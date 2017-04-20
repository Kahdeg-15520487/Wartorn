using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Wartorn
{
    namespace Utility
    {
        public static class HelperFunction
        {
            /// <summary>
            /// convert angle from degree to radian. use to ease process of drawing sprite
            /// </summary>
            /// <param name="angle"></param>
            /// <returns></returns>
            public static float DegreeToRadian(float angle)
            {
                return (float)(Math.PI * angle / 180.0f);
            }

            public static bool IsKeyPress(Keys k)
            {
                return CONTENT_MANAGER.inputState.keyboardState.IsKeyUp(k) && CONTENT_MANAGER.lastInputState.keyboardState.IsKeyDown(k);
            }

            public static void Log(Exception e)
            {
                File.WriteAllText("crashlog.txt",e.Message+Environment.NewLine+e.StackTrace+Environment.NewLine+e.TargetSite);
            }
        }

        public static class ExtensionMethod
        {
            public static T ToEnum<T>(this string value)
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            public static int Clamp(this float value,int max,int min)
            {
                int v = (int)value;
                return v >= max ? max : v <= min ? min : v;
            }

            public static int Clamp(this int value, int max, int min)
            {
                return value >= max ? max : value <= min ? min : value;
            }

            public static bool Between(this int value, int max, int min)
            {
                return value < max && value > min;
            }

            public static int CompareWith(this SpriteSheetTerrain t, SpriteSheetTerrain other)
            {
                return ((int)t).CompareTo((int)other);
            }

            public static SpriteSheetTerrain Next(this SpriteSheetTerrain t,int count = 1)
            {
                return (SpriteSheetTerrain)((int)t + count);
            }

            public static SpriteSheetTerrain Previous(this SpriteSheetTerrain t,int count = 1)
            {
                return (SpriteSheetTerrain)((int)t - count);
            }

            public static Point GetNearbyPoint(this Point p, Direction d)
            {
                Point output = new Point(p.X, p.Y);
                switch (d)
                {
                    case Direction.NorthWest:
                        output = new Point(p.X - 1, p.Y - 1);
                        break;
                    case Direction.North:
                        output = new Point(p.X, p.Y - 1);
                        break;
                    case Direction.NorthEast:
                        output = new Point(p.X + 1, p.Y - 1);
                        break;
                    case Direction.West:
                        output = new Point(p.X - 1, p.Y);
                        break;
                    case Direction.East:
                        output = new Point(p.X + 1, p.Y);
                        break;
                    case Direction.SouthWest:
                        output = new Point(p.X - 1, p.Y + 1);
                        break;
                    case Direction.South:
                        output = new Point(p.X, p.Y + 1);
                        break;
                    case Direction.SouthEast:
                        output = new Point(p.X + 1, p.Y + 1);
                        break;
                    default:
                        break;
                }
                return output;
            }
        }
    }
}