using Microsoft.Xna.Framework;
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
    {
        RoadTurn1,  RoadTurn2,  RoadInter31,RoadInter32,    Road1,  RoadInter4,     blank1,         Bridge1,
        RoadTurn3,  RoadTurn4,  RoadInter33,RoadInter34,    Road2,  Plain,          blank2,         Bridge2,
        blank3,     blank4,     blank5,     blank6,         blank7, blank8,         blank9,         blank10,
        City,       Factory,    AirPort,    Harbor,         Silo1,   Radar,          SupplyBase,     Silo2,
        Tree1,      Tree2,      blank13,    blank14,        blank15,blank16,        blank17,        blank18,
        Forest1,    Forest2,    Forest3,    Forest4,        blank19,blank20,        blank21,        blank22,
        blank23,    blank24,    blank25,    blank26,        blank27,DenseForest1,   DenseForest2,   DenseForest3,
        HighMountain,filler1,   blank28,    blank29,        blank30,DenseForest4,   DenseForest5,   DenseForest6,
        Mountain,   filler2,    blank31,    blank32,        blank33, DenseForest7,  DenseForest8,   DenseForest9,
    }

    static class SpriteSheetSourceRectangle
    {
        private static Dictionary<string,Rectangle> TerrainSprite;

        public static void LoadSprite()
        {
            TerrainSprite = new Dictionary<string, Rectangle>();
            int c = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    c++;
                }
            }
            string log = JsonConvert.SerializeObject(TerrainSprite, Formatting.Indented);
            File.WriteAllText("log.txt", log);
        }

        public static Rectangle GetSpriteRectangle(string str)
        {
            return TerrainSprite[str];
        }

        {
            return TerrainSprite[t.ToString()];
        }
    }
}