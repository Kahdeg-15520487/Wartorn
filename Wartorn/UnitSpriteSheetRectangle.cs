using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wartorn.GameData;
using Wartorn.Utility;

namespace Wartorn
{
    public enum SpriteSheetUnit
    {
        Red_Soldier, Red_Mech, Red_Recon, Red_APC, Red_Tank, Red_HeavyTank, Red_Artillery, Red_Rocket, Red_AntiAir, Red_Missile, Red_TransportCopter, Red_BattleCopter, Red_Fighter, Red_Bomber, Red_Lander, Red_Cruise, Red_Submarine, Red_Battleship,
        Blue_Soldier, Blue_Mech, Blue_Recon, Blue_APC, Blue_Tank, Blue_HeavyTank, Blue_Artillery, Blue_Rocket, Blue_AntiAir, Blue_Missile, Blue_TransportCopter, Blue_BattleCopter, Blue_Fighter, Blue_Bomber, Blue_Lander, Blue_Cruise, Blue_Submarine, Blue_Battleship,
        Green_Soldier, Green_Mech, Green_Recon, Green_APC, Green_Tank, Green_HeavyTank, Green_Artillery, Green_Rocket, Green_AntiAir, Green_Missile, Green_TransportCopter, Green_BattleCopter, Green_Fighter, Green_Bomber, Green_Lander, Green_Cruise, Green_Submarine, Green_Battleship,
        Yellow_Soldier, Yellow_Mech, Yellow_Recon, Yellow_APC, Yellow_Tank, Yellow_HeavyTank, Yellow_Artillery, Yellow_Rocket, Yellow_AntiAir, Yellow_Missile, Yellow_TransportCopter, Yellow_BattleCopter, Yellow_Fighter, Yellow_Bomber, Yellow_Lander, Yellow_Cruise, Yellow_Submarine, Yellow_Battleship
    }

    static class UnitSpriteSheetRectangle
    {
        private static Dictionary<SpriteSheetUnit, Rectangle> UnitSprite;

        public static void LoadSprite()
        {
            UnitSprite = new Dictionary<SpriteSheetUnit, Rectangle>();
            SpriteSheetUnit counter = SpriteSheetUnit.Red_Soldier;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 18; j++)
                {
                    UnitSprite.Add(counter, new Rectangle(j * 48, i * 48, 48, 48));
                    counter = counter.Next();
                }
            }
        }

        public static Rectangle GetSprite(SpriteSheetUnit t)
        {
            return UnitSprite[t];
        }

        public static Rectangle GetSprite(UnitType unittype, Owner owner)
        {
            int x = ((int)unittype - 1) * 48;
            int y = ((int)owner - 1) * 48;

            return new Rectangle(x, y, 48, 48);
        }

        public static UnitType GetUnitType(Rectangle r)
        {
            return (UnitType)(r.X / 48 + 1);
        }
    }
}
