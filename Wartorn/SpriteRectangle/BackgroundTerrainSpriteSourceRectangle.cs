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
    enum SpriteSheetBackgroundTerrain
    {
        //BG_Normal.png
        //LINE 1:
        Red_HQ, Blue_HQ, Green_HQ, Yellow_HQ, Plain,
        //LINE 2:
        Road, Factory, AirPort, Harbor, City,
        //LINE 3:
        MissleSilo, SupplyBase, Radar, Tree, Mountain,
        //LINE 4:
        Bridge, River, Coast, Sea, Sky,

        //BG_Desert.png
        //LINE 1:
        Desert_Red_HQ, Desert_Blue_HQ, Desert_Green_HQ, Desert_Yellow_HQ, Desert_Plain,
        //LINE 2:
        Desert_Road, Desert_Factory, Desert_AirPort, Desert_Harbor, Desert_City,
        //LINE 3:
        Desert_MissleSilo, Desert_SupplyBase, Desert_Radar, Desert_Tree, Desert_Mountain,
        //LINE 4:
        Desert_Bridge, Desert_River, Desert_Coast, Desert_Sea, Desert_Sky,

        //BG_Rain.png
        //LINE 1:
        Rain_Red_HQ, Rain_Blue_HQ, Rain_Green_HQ, Rain_Yellow_HQ, Rain_Plain,
        //LINE 2:
        Rain_Road, Rain_Factory, Rain_AirPort, Rain_Harbor, Rain_City,
        //LINE 3:
        Rain_MissleSilo, Rain_SupplyBase, Rain_Radar, Rain_Tree, Rain_Mountain,
        //LINE 4:
        Rain_Bridge, Rain_River, Rain_Coast, Rain_Sea, Rain_Sky,

        //BG_Snow.png
        //LINE 1:
        Snow_Red_HQ, Snow_Blue_HQ, Snow_Green_HQ, Snow_Yellow_HQ, Snow_Plain,
        //LINE 2:
        Snow_Road, Snow_Factory, Snow_AirPort, Snow_Harbor, Snow_City,
        //LINE 3:
        Snow_MissleSilo, Snow_SupplyBase, Snow_Radar, Snow_Tree, Snow_Mountain,
        //LINE 4:
        Snow_Bridge, Snow_River, Snow_Coast, Snow_Sea, Snow_Sky,

        //BG_Tropical.png
        //LINE 1:
        Tropical_Red_HQ, Tropical_Blue_HQ, Tropical_Green_HQ, Tropical_Yellow_HQ, Tropical_Plain,
        //LINE 2:
        Tropical_Road, Tropical_Factory, Tropical_AirPort, Tropical_Harbor, Tropical_City,
        //LINE 3:
        Tropical_MissleSilo, Tropical_SupplyBase, Tropical_Radar, Tropical_Tree, Tropical_Mountain,
        //LINE 4:
        Tropical_Bridge, Tropical_River, Tropical_Coast, Tropical_Sea, Tropical_Sky
    }

    static class BackgroundTerrainSpriteSourceRectangle
    {
        private static Dictionary<SpriteSheetBackgroundTerrain, Rectangle> BackgroundTerrainSprite;

        public static void LoadSprite()
        {
            BackgroundTerrainSprite = new Dictionary<SpriteSheetBackgroundTerrain, Rectangle>();

            SpriteSheetBackgroundTerrain c = SpriteSheetBackgroundTerrain.Red_HQ;

            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    BackgroundTerrainSprite.Add(c, new Rectangle(x * 128, y * 76, 128, 76));
                    c = c.Next();
                }
            }
        }

        public static Rectangle GetSpriteRectangle(SpriteSheetBackgroundTerrain t)
        {
            return BackgroundTerrainSprite[t];
        }

        public static Rectangle GetSpriteRectangle(TerrainType t, Weather w, Theme th, UnitType ut, Owner o = Owner.None)
        {
            StringBuilder spritename = new StringBuilder();
            switch (w)
            {
                case Weather.Sunny:
                    switch (th)
                    {
                        case Theme.Tropical:
                        case Theme.Desert:
                            spritename.Append(th.ToString());
                            break;
                        default:
                            break;
                    }
                    break;
                case Weather.Rain:
                case Weather.Snow:
                    spritename.Append(w.ToString());
                    break;
                default:
                    break;
            }

            spritename.Append("_");

            if (o != Owner.None)
            {
                spritename.Append(o.ToString());
            }

            spritename.Append("_");

            if (ut == UnitType.TransportCopter
             || ut == UnitType.BattleCopter
             || ut == UnitType.Fighter
             || ut == UnitType.Bomber)
            {
                spritename.Append("Sky");
                goto end;
            }

            if (ut == UnitType.Lander
             || ut == UnitType.Cruiser
             || ut == UnitType.Submarine
             || ut == UnitType.Battleship)
            {
                spritename.Append("Sea");
                goto end;
            }

            if (t == TerrainType.MissileSiloLaunched)
            {
                spritename.Append(TerrainType.MissileSilo.ToString());
                goto end;
            }

            spritename.Append(t.ToString());

            end:
            return BackgroundTerrainSprite[spritename.ToString().ToEnum<SpriteSheetBackgroundTerrain>()];
        }
    }
}