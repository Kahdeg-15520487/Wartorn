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
    public enum SpriteSheetTerrain
    {
        Min,
        //water related tile
        Reef, Sea, River_hor, River_ver, River_Inter3_l, River_Inter3_r, River_Inter3_u, River_Inter3_d, River_Turn_u_r, River_Turn_u_l, River_Turn_d_r, River_Turn_d_l, River_Flow_l, River_Flow_u, River_Flow_d, River_Flow_r,

        Coast_u_l, Coast_u, Coast_u_r, Coast_l, Coast_r, Coast_d_l, Coast_d, Coast_d_r, Cliff_u_l, Cliff_u, Cliff_u_r, Cliff_d_l, Cliff_d, Cliff_d_r,

        Isle_Coast_u_l, Isle_Coast_u_r, Isle_Coast_side_l_u, Isle_Coast_side_l_d, Isle_Coast_side_r_u, Isle_Coast_side_r_d, Isle_Coast_d_l, Isle_Coast_d_r, Isle_Cliff_u_r, Isle_Cliff_u_l, Isle_Cliff_d_r, Isle_Cliff_d_l, Cliff_l, Cliff_r, River_Cross,

        Lone_Coast_u_l, Lone_Coast_u_r, Lone_Coast_d_l, Lone_Coast_d_r, Lone_Coast_u, Lone_Coast_d, Lone_Coast_r, Lone_Coast_l, Invert_Coast_d_l, Invert_Coast_d_r, Invert_Coast_u_l, Invert_Coast_u_r, Invert_Coast_l_d, Invert_Coast_l_u, Invert_Coast_r_u, Invert_Coast_r_d,

        //road related tile
        Plain, Road_hor, Road_ver, Bridge_hor, Bridge_ver, Road_Inter3_l, Road_Inter3_r, Road_Inter3_u, Road_Inter3_d, Road_turn_u_r, Road_Turn_u_l, Road_Turn_d_r, Road_Turn_d_l, Road_Cross,

        //tree related tile
        Tree, Tree_4_1, Tree_4_2, Tree_4_3, Tree_4_4, Tree_9_1, Tree_9_2, Tree_9_3, Tree_9_4, Tree_9_5, Tree_9_6, Tree_9_7, Tree_9_8, Tree_9_9,

        //mountain tile
        Mountain_Low, Mountain_High_Upper, Mountain_High_Lower,

        //neutral building
        City_Upper, City_Lower, Factory, AirPort_Upper, AirPort_Lower, Harbor_Upper, Harbor_Lower, Radar_Upper, Radar_Lower, SupplyBase_Upper, SupplyBase_Lower, Silo_HaveMissile_Upper, Silo_HaveMissile_Lower, Silo_MissileLaunched,

        //Red-captured building
        Red_City_Upper, Red_City_Lower, Red_Factory, Red_AirPort_Upper, Red_AirPort_Lower, Red_Harbor_Upper, Red_Harbor_Lower, Red_Radar_Upper, Red_Radar_Lower, Red_SupplyBase_Upper, Red_SupplyBase_Lower,

        //Blue-captured building
        Blue_City_Upper, Blue_City_Lower, Blue_Factory, Blue_AirPort_Upper, Blue_AirPort_Lower, Blue_Harbor_Upper, Blue_Harbor_Lower, Blue_Radar_Upper, Blue_Radar_Lower, Blue_SupplyBase_Upper, Blue_SupplyBase_Lower,

        //HeadQuarter
        Red_HeadQuarter_Upper, Red_HeadQuarter_Lower, Blue_HeadQuarter_Upper, Blue_HeadQuarter_Lower,
        Max
    }

    static class SpriteSheetSourceRectangle
    {
        private static Dictionary<string, Rectangle> TerrainSprite;

        public static void LoadSprite()
        {
            TerrainSprite = new Dictionary<string, Rectangle>();
            for (int i = 0; i < ((int)SpriteSheetTerrain.Max - 1); i++)
            {
                TerrainSprite.Add(((SpriteSheetTerrain)i + 1).ToString(), new Rectangle(i * 48, 0, 48, 48));
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