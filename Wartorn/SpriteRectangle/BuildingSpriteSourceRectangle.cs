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

namespace Wartorn
{
    enum SpriteSheetBuilding
    {
        City, Factory, Airport, Harbor, Radar, Supplybase, Headquarter, MissileSilo,
        Red_City, Red_Factory, Red_Airport, Red_Harbor, Red_Radar, Red_Supplybase, Red_Headquarter, Red_MissileSilo,
        Blue_City, Blue_Factory, Blue_Airport, Blue_Harbor, Blue_Radar, Blue_Supplybase, Blue_Headquarter, Blue_MissileSilo,
        Green_City, Green_Factory, Green_Airport, Green_Harbor, Green_Radar, Green_Supplybase, Green_Headquarter, Green_MissileSilo,
        Yellow_City, Yellow_Factory, Yellow_Airport, Yellow_Harbor, Yellow_Radar, Yellow_Supplybase, Yellow_Headquarter, Yellow_MissileSilo
    }
    enum BuildingType
    {
        City,
        Factory,
        Airport,
        Harbor,
        Radar,
        Supplybase,
        Headquarter,
        MissileSilo
    }
    static class BuildingSpriteSourceRectangle
    {
        private static Dictionary<SpriteSheetBuilding, Rectangle> BuildingSprite;

        public static void LoadSprite()
        {
            BuildingSprite = new Dictionary<SpriteSheetBuilding, Rectangle>();

            SpriteSheetBuilding c = SpriteSheetBuilding.City;

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    BuildingSprite.Add(c, new Rectangle(x * 48, y * 96, 48, 96));
                    c = c.Next();
                }
            }

            //File.WriteAllText("buildingspriterectangle.txt", JsonConvert.SerializeObject(BuildingSprite.ToArray(), Formatting.Indented));

        }

        public static Rectangle GetSpriteRectangle(SpriteSheetBuilding t)
        {
            return BuildingSprite[t];
        }

        public static Rectangle GetSpriteRectangle(BuildingType bt,Owner owner = Owner.None)
        {
            StringBuilder result = new StringBuilder();
            switch (owner)
            {
                case Owner.None:
                    break;
                case Owner.Red:
                case Owner.Blue:
                case Owner.Green:
                case Owner.Yellow:
                    result.Append(owner.ToString());
                    result.Append("_");
                    break;
                default:
                    break;
            }
            result.Append(bt.ToString());

            return BuildingSprite[result.ToString().ToEnum<SpriteSheetBuilding>()];
        }

        public static BuildingType GetBuldingType(Rectangle r)
        {
            return (BuildingType)(r.X / 48);
        }

        public static Owner GetOwner(Rectangle r)
        {
            return (Owner)(r.Y / 96);
        }
    }
}
