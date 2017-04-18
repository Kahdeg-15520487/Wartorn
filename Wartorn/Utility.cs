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

            public static T ToEnum<T>(this string value)
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            public static void Log(Exception e)
            {
                File.WriteAllText("crashlog.txt",e.Message+Environment.NewLine+e.StackTrace+Environment.NewLine+e.TargetSite);
            }

            public static int Clamp(int value,int max,int min)
            {
                return value >= max ? max : value <= min ? min : value;
            }

            public static bool Between(this int value,int max,int min)
            {
                return value <= max && value >= min;
            }
        }
    }
}