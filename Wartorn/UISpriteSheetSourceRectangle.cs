﻿using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wartorn.GameData;

namespace Wartorn
{
    //only water
    enum SpriteSheetUI
    {
        Select,
        Undo,
        Redo,
        New,
        Save,
        Open
    }

    static class UISpriteSheetSourceRectangle
    {
        private static Dictionary<string,Rectangle> UISprite;

        public static void LoadSprite()
        {
            UISprite = new Dictionary<string, Rectangle>();
            int c = 0;
            for (int i = 0; i < 6; i++)
            {
                UISprite.Add(((SpriteSheetUI)c).ToString(), new Rectangle(i * 48, 0, 48, 48));
                c++;
            }
            //string log = JsonConvert.SerializeObject(UISprite, Formatting.Indented);
            //File.WriteAllText("log.txt", log);
        }

        public static Rectangle GetSpriteRectangle(string str)
        {
            return UISprite[str];
        }

        public static Rectangle GetSpriteRectangle(SpriteSheetUI t)
        {
            return UISprite[t.ToString()];
        }
    }
}