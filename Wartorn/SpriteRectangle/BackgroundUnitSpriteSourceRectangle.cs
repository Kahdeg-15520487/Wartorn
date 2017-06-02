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
    enum SpriteSheetBackgroundUnit
    {
        //Red_Unit.png
        //LINE 1:
        Red_Soldier, Red_Mech, Red_Mech_River, Red_Soldier_River, Red_Mech_Mountain, Red_Soldier_Mountain,
        //LINE 2:
        Red_Recon, Red_Tank, Red_AntiAir, Red_Artillery, Red_Rocket, Red_Missile,
        //LINE 3:
        Red_HeavyTank, Red_APC, Red_Lander, Red_Cruiser, Red_Battleship, Red_Submarine,
        //LINE 4:
        Red_SubMarine_Dive, Red_TransportCopter, Red_BattleCopter, Red_Fighter, Red_Bomber,

        //Blue_Unit.png
        //LINE 1:
        Blue_Soldier, Blue_Mech, Blue_Mech_River, Blue_Soldier_River, Blue_Mech_Mountain, Blue_Soldier_Mountain,
        //LINE 2:
        Blue_Recon, Blue_Tank, Blue_AntiAir, Blue_Artillery, Blue_Rocket, Blue_Missile,
        //LINE 3:
        Blue_HeavyTank, Blue_APC, Blue_Lander, Blue_Cruiser, Blue_Battleship, Blue_Submarine,
        //LINE 4:
        Blue_SubMarine_Dive, Blue_TransportCopter, Blue_BattleCopter, Blue_Fighter, Blue_Bomber,

        //Green_Unit.png
        //LINE 1:
        Green_Soldier, Green_Mech, Green_Mech_River, Green_Soldier_River, Green_Mech_Mountain, Green_Soldier_Mountain,
        //LINE 2:
        Green_Recon, Green_Tank, Green_AntiAir, Green_Artillery, Green_Rocket, Green_Missile,
        //LINE 3:
        Green_HeavyTank, Green_APC, Green_Lander, Green_Cruiser, Green_Battleship, Green_Submarine,
        //LINE 4:
        Green_SubMarine_Dive, Green_TransportCopter, Green_BattleCopter, Green_Fighter, Green_Bomber,

        //Yellow_Unit.png
        //LINE 1:
        Yellow_Soldier, Yellow_Mech, Yellow_Mech_River, Yellow_Soldier_River, Yellow_Mech_Mountain, Yellow_Soldier_Mountain,
        //LINE 2:
        Yellow_Recon, Yellow_Tank, Yellow_AntiAir, Yellow_Artillery, Yellow_Rocket, Yellow_Missile,
        //LINE 3:
        Yellow_HeavyTank, Yellow_APC, Yellow_Lander, Yellow_Cruiser, Yellow_Battleship, Yellow_Submarine,
        //LINE 4:
        Yellow_SubMarine_Dive, Yellow_TransportCopter, Yellow_BattleCopter, Yellow_Fighter, Yellow_Bomber
    }

    static class BackgroundUnitSpriteSourceRectangle
    {
        private static Dictionary<SpriteSheetBackgroundUnit, Rectangle> BackgroundUnitSprite;

        public static void LoadSprite()
        {
            BackgroundUnitSprite = new Dictionary<SpriteSheetBackgroundUnit, Rectangle>();

            SpriteSheetBackgroundUnit c = SpriteSheetBackgroundUnit.Red_Soldier;

            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    BackgroundUnitSprite.Add(c, new Rectangle(x * 128, y * 76, 128, 76));
                }
            }
        }

        public static Rectangle GetSpriteRectangle(SpriteSheetBackgroundUnit t)
        {
            return BackgroundUnitSprite[t];
        }

        public static Rectangle GetSpriteRectangle(UnitType ut, Owner o, TerrainType t)
        {
            StringBuilder spritename = new StringBuilder();

            if (o == Owner.None)
            {
                spritename.Append("Red");
            }
            else
            {
                spritename.Append(o.ToString());
            }

            spritename.Append("_");

            spritename.Append(ut.ToString());

            if (ut == UnitType.Soldier
             || ut == UnitType.Mech)
            {
                if (t == TerrainType.River)
                {
                    spritename.Append("_River");
                }
                if (t == TerrainType.Mountain)
                {
                    spritename.Append("_Mountain");
                }
            }

            return BackgroundUnitSprite[spritename.ToString().ToEnum<SpriteSheetBackgroundUnit>()];
        }
    }
}