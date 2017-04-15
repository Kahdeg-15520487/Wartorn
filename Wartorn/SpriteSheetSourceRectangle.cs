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
    //only water
    enum SpriteSheetTerrain
    {
        Min,
        Reef,Sea,River_hor,River_ver,River_Inter3_l,River_Inter3_r,River_Inter3_u,River_Inter3_d,River_Turn_u_r,River_Turn_u_l,River_Turn_d_r,River_Turn_d_l,River_Flow_l,River_Flow_u,River_Flow_d,
        Coast_u_l,Coast_u,Coast_u_r,Coast_l,Coast_r,Coast_d_l,Coast_d,Coast_d_r,Cliff_u_l,Cliff_u,Cliff_u_r,Cliff_d_l,Cliff_d,Cliff_d_r,River_Flow_r,
        Isle_Coast_u_l,Isle_Coast_u_r,Isle_Coast_side_l_u,Isle_Coast_side_l_d,Isle_Coast_side_r_u,Isle_Coast_side_r_d,Isle_Coast_d_l,Isle_Coast_d_r,Isle_Cliff_u_r,Isle_Cliff_u_l,Isle_Cliff_d_r,Isle_Cliff_d_l,Cliff_l,Cliff_r,River_Cross,
        Max
    }

    static class SpriteSheetSourceRectangle
    {
        private static Dictionary<string,Rectangle> TerrainSprite;

        public static void LoadSprite()
        {
            TerrainSprite = new Dictionary<string, Rectangle>();
            int c = 1;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    TerrainSprite.Add(((SpriteSheetTerrain)c).ToString(), new Rectangle(j*48,i*48,48,48));
                    c++;
                }
            }
            //string log = JsonConvert.SerializeObject(TerrainSprite, Formatting.Indented);
            //File.WriteAllText("log.txt", log);
        }

        public static Rectangle GetSpriteRectangle(string str)
        {
            return TerrainSprite[str];
        }

        public static Rectangle GetSpriteRectangle(SpriteSheetTerrain t)
        {
            return TerrainSprite[t.ToString()];
        }
    }
}