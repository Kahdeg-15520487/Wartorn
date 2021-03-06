﻿using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wartorn.GameData;
using Wartorn.Utility;

namespace Wartorn.SpriteRectangle
{
    public enum SpriteSheetTerrain
    {
        Min,
        //Normal Water
        Reef, Sea, River_ver, River_hor, River_Inter3_right, River_Inter3_left, River_Inter3_up, River_Inter3_down, River_Cross, River_turn_up_right, River_turn_up_left, River_turn_down_right, River_turn_down_left, River_Flow_left, River_Flow_up, River_Flow_down, River_Flow_right, Coast_up_left, Coast_up, Coast_up_right, Coast_left, Coast_right, Coast_down_left, Coast_down, Coast_down_right, Cliff_up_left, Cliff_up, Cliff_up_right, Cliff_down_left, Cliff_down, Cliff_down_right, Isle_Coast_up_left, Isle_Coast_up_right, Isle_Coast_side_right_up, Isle_Coast_side_right_down, Isle_Coast_side_left_up, Isle_Coast_side_left_down, Isle_Coast_down_left, Isle_Coast_down_right, Isle_Cliff_down_left, Isle_Cliff_down_right, Isle_Cliff_up_left, Isle_Cliff_up_right, Cliff_left, Cliff_right, Lone_Coast_up_left, Lone_Coast_up_right, Lone_Coast_down_left, Lone_Coast_down_right, Lone_Coast_up, Lone_Coast_down, Lone_Coast_right, Lone_Coast_left, Invert_Coast_down_left, Invert_Coast_down_right, Invert_Coast_up_left, Invert_Coast_up_right, Invert_Coast_left_down, Invert_Coast_left_up, Invert_Coast_right_up, Invert_Coast_right_down,

        //Rain Water
        Rain_Reef, Rain_Sea, Rain_River_ver, Rain_River_hor, Rain_River_Inter3_right, Rain_River_Inter3_left, Rain_River_Inter3_up, Rain_River_Inter3_down, Rain_River_Cross, Rain_River_turn_up_right, Rain_River_turn_up_left, Rain_River_turn_down_right, Rain_River_turn_down_left, Rain_River_Flow_left, Rain_River_Flow_up, Rain_River_Flow_down, Rain_River_Flow_right, Rain_Coast_up_left, Rain_Coast_up, Rain_Coast_up_right, Rain_Coast_left, Rain_Coast_right, Rain_Coast_down_left, Rain_Coast_down, Rain_Coast_down_right, Rain_Cliff_up_left, Rain_Cliff_up, Rain_Cliff_up_right, Rain_Cliff_down_left, Rain_Cliff_down, Rain_Cliff_down_right, Rain_Isle_Coast_up_left, Rain_Isle_Coast_up_right, Rain_Isle_Coast_side_right_up, Rain_Isle_Coast_side_right_down, Rain_Isle_Coast_side_left_up, Rain_Isle_Coast_side_left_down, Rain_Isle_Coast_down_left, Rain_Isle_Coast_down_right, Rain_Isle_Cliff_down_left, Rain_Isle_Cliff_down_right, Rain_Isle_Cliff_up_left, Rain_Isle_Cliff_up_right, Rain_Cliff_left, Rain_Cliff_right, Rain_Lone_Coast_up_left, Rain_Lone_Coast_up_right, Rain_Lone_Coast_down_left, Rain_Lone_Coast_down_right, Rain_Lone_Coast_up, Rain_Lone_Coast_down, Rain_Lone_Coast_right, Rain_Lone_Coast_left, Rain_Invert_Coast_down_left, Rain_Invert_Coast_down_right, Rain_Invert_Coast_up_left, Rain_Invert_Coast_up_right, Rain_Invert_Coast_left_down, Rain_Invert_Coast_left_up, Rain_Invert_Coast_right_up, Rain_Invert_Coast_right_down,

        //Snow Water
        Snow_Reef, Snow_Sea, Snow_River_ver, Snow_River_hor, Snow_River_Inter3_right, Snow_River_Inter3_left, Snow_River_Inter3_up, Snow_River_Inter3_down, Snow_River_Cross, Snow_River_turn_up_right, Snow_River_turn_up_left, Snow_River_turn_down_right, Snow_River_turn_down_left, Snow_River_Flow_left, Snow_River_Flow_up, Snow_River_Flow_down, Snow_River_Flow_right, Snow_Coast_up_left, Snow_Coast_up, Snow_Coast_up_right, Snow_Coast_left, Snow_Coast_right, Snow_Coast_down_left, Snow_Coast_down, Snow_Coast_down_right, Snow_Cliff_up_left, Snow_Cliff_up, Snow_Cliff_up_right, Snow_Cliff_down_left, Snow_Cliff_down, Snow_Cliff_down_right, Snow_Isle_Coast_up_left, Snow_Isle_Coast_up_right, Snow_Isle_Coast_side_right_up, Snow_Isle_Coast_side_right_down, Snow_Isle_Coast_side_left_up, Snow_Isle_Coast_side_left_down, Snow_Isle_Coast_down_left, Snow_Isle_Coast_down_right, Snow_Isle_Cliff_down_left, Snow_Isle_Cliff_down_right, Snow_Isle_Cliff_up_left, Snow_Isle_Cliff_up_right, Snow_Cliff_left, Snow_Cliff_right, Snow_Lone_Coast_up_left, Snow_Lone_Coast_up_right, Snow_Lone_Coast_down_left, Snow_Lone_Coast_down_right, Snow_Lone_Coast_up, Snow_Lone_Coast_down, Snow_Lone_Coast_right, Snow_Lone_Coast_left, Snow_Invert_Coast_down_left, Snow_Invert_Coast_down_right, Snow_Invert_Coast_up_left, Snow_Invert_Coast_up_right, Snow_Invert_Coast_left_down, Snow_Invert_Coast_left_up, Snow_Invert_Coast_right_up, Snow_Invert_Coast_right_down,

        //Desert Water
        Desert_Reef, Desert_Sea, Desert_River_ver, Desert_River_hor, Desert_River_Inter3_right, Desert_River_Inter3_left, Desert_River_Inter3_up, Desert_River_Inter3_down, Desert_River_Cross, Desert_River_turn_up_right, Desert_River_turn_up_left, Desert_River_turn_down_right, Desert_River_turn_down_left, Desert_River_Flow_left, Desert_River_Flow_up, Desert_River_Flow_down, Desert_River_Flow_right, Desert_Coast_up_left, Desert_Coast_up, Desert_Coast_up_right, Desert_Coast_left, Desert_Coast_right, Desert_Coast_down_left, Desert_Coast_down, Desert_Coast_down_right, Desert_Cliff_up_left, Desert_Cliff_up, Desert_Cliff_up_right, Desert_Cliff_down_left, Desert_Cliff_down, Desert_Cliff_down_right, Desert_Isle_Coast_up_left, Desert_Isle_Coast_up_right, Desert_Isle_Coast_side_right_up, Desert_Isle_Coast_side_right_down, Desert_Isle_Coast_side_left_up, Desert_Isle_Coast_side_left_down, Desert_Isle_Coast_down_left, Desert_Isle_Coast_down_right, Desert_Isle_Cliff_down_left, Desert_Isle_Cliff_down_right, Desert_Isle_Cliff_up_left, Desert_Isle_Cliff_up_right, Desert_Cliff_left, Desert_Cliff_right, Desert_Lone_Coast_up_left, Desert_Lone_Coast_up_right, Desert_Lone_Coast_down_left, Desert_Lone_Coast_down_right, Desert_Lone_Coast_up, Desert_Lone_Coast_down, Desert_Lone_Coast_right, Desert_Lone_Coast_left, Desert_Invert_Coast_down_left, Desert_Invert_Coast_down_right, Desert_Invert_Coast_up_left, Desert_Invert_Coast_up_right, Desert_Invert_Coast_left_down, Desert_Invert_Coast_left_up, Desert_Invert_Coast_right_up, Desert_Invert_Coast_right_down,

        //Normal Road, Forest and Mountain
        Road_turn_down_right, Road_turn_down_left, Road_Inter3_right, Road_Inter3_down, Road_hor, Road_Cross, Bridge_hor, Road_turn_up_right, Road_turn_up_left, Road_Inter3_up, Road_Inter3_left, Road_ver, Plain, Bridge_ver, Forest, Forest_top_left, Forest_top_right, Forest_bottom_left, Forest_bottom_right, Forest_up_left, Forest_up_middle, Forest_up_right, Forest_middle_left, Forest_middle_middle, Forest_middle_right, Forest_down_left, Forest_down_middle, Forest_down_right, Mountain_High_Upper, Mountain_High_Lower, Mountain_Low,

        //Tropical Road, Forest and Mountain
        Tropical_Road_turn_down_right, Tropical_Road_turn_down_left, Tropical_Road_Inter3_right, Tropical_Road_Inter3_down, Tropical_Road_hor, Tropical_Road_Cross, Tropical_Bridge_hor, Tropical_Road_turn_up_right, Tropical_Road_turn_up_left, Tropical_Road_Inter3_up, Tropical_Road_Inter3_left, Tropical_Road_ver, Tropical_Plain, Tropical_Bridge_ver, Tropical_Forest, Tropical_Forest_top_left, Tropical_Forest_top_right, Tropical_Forest_bottom_left, Tropical_Forest_bottom_right, Tropical_Forest_up_left, Tropical_Forest_up_middle, Tropical_Forest_up_right, Tropical_Forest_middle_left, Tropical_Forest_middle_middle, Tropical_Forest_middle_right, Tropical_Forest_down_left, Tropical_Forest_down_middle, Tropical_Forest_down_right, Tropical_Mountain_High_Upper, Tropical_Mountain_High_Lower, Tropical_Mountain_Low,

        //Rain Road, Forest and Mountain
        Rain_Road_turn_down_right, Rain_Road_turn_down_left, Rain_Road_Inter3_right, Rain_Road_Inter3_down, Rain_Road_hor, Rain_Road_Cross, Rain_Bridge_hor, Rain_Road_turn_up_right, Rain_Road_turn_up_left, Rain_Road_Inter3_up, Rain_Road_Inter3_left, Rain_Road_ver, Rain_Plain, Rain_Bridge_ver, Rain_Forest, Rain_Forest_top_left, Rain_Forest_top_right, Rain_Forest_bottom_left, Rain_Forest_bottom_right, Rain_Forest_up_left, Rain_Forest_up_middle, Rain_Forest_up_right, Rain_Forest_middle_left, Rain_Forest_middle_middle, Rain_Forest_middle_right, Rain_Forest_down_left, Rain_Forest_down_middle, Rain_Forest_down_right, Rain_Mountain_High_Upper, Rain_Mountain_High_Lower, Rain_Mountain_Low,

        //Snow Road, Forest and Mountain
        Snow_Road_turn_down_right, Snow_Road_turn_down_left, Snow_Road_Inter3_right, Snow_Road_Inter3_down, Snow_Road_hor, Snow_Road_Cross, Snow_Bridge_hor, Snow_Road_turn_up_right, Snow_Road_turn_up_left, Snow_Road_Inter3_up, Snow_Road_Inter3_left, Snow_Road_ver, Snow_Plain, Snow_Bridge_ver, Snow_Forest, Snow_Forest_top_left, Snow_Forest_top_right, Snow_Forest_bottom_left, Snow_Forest_bottom_right, Snow_Forest_up_left, Snow_Forest_up_middle, Snow_Forest_up_right, Snow_Forest_middle_left, Snow_Forest_middle_middle, Snow_Forest_middle_right, Snow_Forest_down_left, Snow_Forest_down_middle, Snow_Forest_down_right, Snow_Mountain_High_Upper, Snow_Mountain_High_Lower, Snow_Mountain_Low,

        //Desert Road, Forest and Mountain
        Desert_Road_turn_down_right, Desert_Road_turn_down_left, Desert_Road_Inter3_right, Desert_Road_Inter3_down, Desert_Road_hor, Desert_Road_Cross, Desert_Bridge_hor, Desert_Road_turn_up_right, Desert_Road_turn_up_left, Desert_Road_Inter3_up, Desert_Road_Inter3_left, Desert_Road_ver, Desert_Plain, Desert_Bridge_ver, Desert_Forest, Desert_Mountain_High_Upper, Desert_Mountain_High_Lower,

        //Neutral Building
        City_Upper, City_Lower, Factory, AirPort_Upper, AirPort_Lower, Harbor_Upper, Harbor_Lower, Radar_Upper, Radar_Lower, SupplyBase_Upper, SupplyBase_Lower, Missile_Silo_Upper, Missile_Silo_Lower, Missile_Silo_Launched,

        //Red Building
        Red_City_Upper, Red_City_Lower, Red_Factory, Red_AirPort_Upper, Red_AirPort_Lower, Red_Harbor_Upper, Red_Harbor_Lower, Red_Radar_Upper, Red_Radar_Lower, Red_SupplyBase_Upper, Red_SupplyBase_Lower, Red_Headquarter_Upper, Red_Headquarter_Lower,

        //Blue Building
        Blue_City_Upper, Blue_City_Lower, Blue_Factory, Blue_AirPort_Upper, Blue_AirPort_Lower, Blue_Harbor_Upper, Blue_Harbor_Lower, Blue_Radar_Upper, Blue_Radar_Lower, Blue_SupplyBase_Upper, Blue_SupplyBase_Lower, Blue_Headquarter_Upper, Blue_Headquarter_Lower,

        //Green Building
        Green_City_Upper, Green_City_Lower, Green_Factory, Green_AirPort_Upper, Green_AirPort_Lower, Green_Harbor_Upper, Green_Harbor_Lower, Green_Radar_Upper, Green_Radar_Lower, Green_SupplyBase_Upper, Green_SupplyBase_Lower, Green_Headquarter_Upper, Green_Headquarter_Lower,

        //Yellow Building
        Yellow_City_Upper, Yellow_City_Lower, Yellow_Factory, Yellow_AirPort_Upper, Yellow_AirPort_Lower, Yellow_Harbor_Upper, Yellow_Harbor_Lower, Yellow_Radar_Upper, Yellow_Radar_Lower, Yellow_SupplyBase_Upper, Yellow_SupplyBase_Lower, Yellow_Headquarter_Upper, Yellow_Headquarter_Lower,

        Max,
        None
    }

    static class TerrainSpriteSourceRectangle
    {
        private static Dictionary<SpriteSheetTerrain, Rectangle> TerrainSprite;
        private static Dictionary<Rectangle, SpriteSheetTerrain> _TerrainSprite;

        public static void LoadSprite()
        {
            TerrainSprite = new Dictionary<SpriteSheetTerrain, Rectangle>();
            _TerrainSprite = new Dictionary<Rectangle, SpriteSheetTerrain>();
            int row = 0;
            int count = 1;

            //normal water
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 61; j++)
                {
                    TerrainSprite.Add((SpriteSheetTerrain)count, new Rectangle(j * 48, row * 48, 48, 48));
                    _TerrainSprite.Add(new Rectangle(j * 48, row * 48, 48, 48), (SpriteSheetTerrain)count);
                    count++;
                }
                row++;
            }

            //road,Forest,mountain
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 31; j++)
                {
                    TerrainSprite.Add((SpriteSheetTerrain)count, new Rectangle(j * 48, row * 48, 48, 48));
                    _TerrainSprite.Add(new Rectangle(j * 48, row * 48, 48, 48), (SpriteSheetTerrain)count);
                    count++;
                }
                row++;
            }

            //desert road,Forest,mountain
            for (int i = 0; i < 17; i++)
            {
                TerrainSprite.Add((SpriteSheetTerrain)count, new Rectangle(i * 48, row * 48, 48, 48));
                _TerrainSprite.Add(new Rectangle(i * 48, row * 48, 48, 48), (SpriteSheetTerrain)count);
                count++;
            }
            row++;

            //neutral building
            for (int j = 0; j < 14; j++)
            {
                TerrainSprite.Add((SpriteSheetTerrain)count, new Rectangle(j * 48, row * 48, 48, 48));
                _TerrainSprite.Add(new Rectangle(j * 48, row * 48, 48, 48), (SpriteSheetTerrain)count);
                count++;
            }
            row++;

            //owned building
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    TerrainSprite.Add((SpriteSheetTerrain)count, new Rectangle(j * 48, row * 48, 48, 48));
                    _TerrainSprite.Add(new Rectangle(j * 48, row * 48, 48, 48), (SpriteSheetTerrain)count);
                    count++;
                }
                row++;
            }

            //string log = JsonConvert.SerializeObject(TerrainSprite, Formatting.Indented);
            //File.WriteAllText("RectangleLog.txt", log);
        }

        public static Rectangle GetSpriteRectangle(string str)
        {
            var temp = str.ToEnum<SpriteSheetTerrain>();
            if (temp == SpriteSheetTerrain.None)
            {
                return Rectangle.Empty;
            }
            return TerrainSprite[temp];
        }

        public static Rectangle GetSpriteRectangle(SpriteSheetTerrain t)
        {
            if (t == SpriteSheetTerrain.None || t== SpriteSheetTerrain.Min || t== SpriteSheetTerrain.Max)
            {
                return Rectangle.Empty;
            }
            return TerrainSprite[t];
        }

        public static Rectangle GetSpriteRectangle(TerrainType t)
        {
            Rectangle temp = Rectangle.Empty;
            switch (t)
            {
                case TerrainType.Reef:
                    temp = TerrainSprite[SpriteSheetTerrain.Reef];
                    break;
                case TerrainType.Sea:
                    temp = TerrainSprite[SpriteSheetTerrain.Sea];
                    break;
                case TerrainType.River:
                    temp = TerrainSprite[SpriteSheetTerrain.River_hor];
                    break;
                case TerrainType.Coast:
                    temp = TerrainSprite[SpriteSheetTerrain.Coast_up];
                    break;
                case TerrainType.Cliff:
                    temp = TerrainSprite[SpriteSheetTerrain.Cliff_up];
                    break;
                case TerrainType.Bridge:
                    temp = TerrainSprite[SpriteSheetTerrain.Bridge_hor];
                    break;
                case TerrainType.Road:
                    temp = TerrainSprite[SpriteSheetTerrain.Road_hor];
                    break;
                case TerrainType.Forest:
                    temp = TerrainSprite[SpriteSheetTerrain.Forest];
                    break;
                case TerrainType.Mountain:
                    temp = TerrainSprite[SpriteSheetTerrain.Mountain_High_Lower];
                    break;
                case TerrainType.MissileSilo:
                    temp = TerrainSprite[SpriteSheetTerrain.Missile_Silo_Lower];
                    break;
                case TerrainType.City:
                    temp = TerrainSprite[SpriteSheetTerrain.City_Lower];
                    break;
                case TerrainType.Factory:
                    temp = TerrainSprite[SpriteSheetTerrain.Factory];
                    break;
                case TerrainType.AirPort:
                    temp = TerrainSprite[SpriteSheetTerrain.AirPort_Lower];
                    break;
                case TerrainType.Harbor:
                    temp = TerrainSprite[SpriteSheetTerrain.Harbor_Lower];
                    break;
                case TerrainType.Radar:
                    temp = TerrainSprite[SpriteSheetTerrain.Radar_Lower];
                    break;
                case TerrainType.SupplyBase:
                    temp = TerrainSprite[SpriteSheetTerrain.SupplyBase_Lower];
                    break;
                case TerrainType.HQ:
                    temp = TerrainSprite[SpriteSheetTerrain.Blue_Headquarter_Lower];
                    break;
                default:
                    break;
            }
            return temp;
        }

        public static SpriteSheetTerrain GetTerrain(Rectangle r)
        {
            return _TerrainSprite[r];

            //if (TerrainSprite.ContainsValue(r))
            //{
            //    foreach (var pair in TerrainSprite)
            //    {
            //        if (pair.Value == r)
            //        {
            //            return pair.Key;
            //        }
            //    }
            //}
            //return SpriteSheetTerrain.None;
        }
    }
}