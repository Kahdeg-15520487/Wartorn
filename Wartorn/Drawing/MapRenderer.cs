﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wartorn.GameData;
using Wartorn.Drawing.Animation;
using Wartorn.Utility;
using System.IO;

namespace Wartorn.Drawing
{
    static class MapRenderer
    {
        public static void Render(Map map, SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!map.IsProcessed)
            {
                Process(map);
            }

            MapCell tempmapcell;
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    Vector2 curpos = new Vector2(i * Constants.MapCellWidth, j * Constants.MapCellHeight);
                    tempmapcell = map[i, j];
                    if (tempmapcell.terrainbase != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, TerrainSpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainbase), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainBase);
                    }
                    if (tempmapcell.terrainLower != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, TerrainSpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainLower), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainLower);
                    }
                    if (tempmapcell.terrainUpper != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, TerrainSpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainUpper), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainUpper);
                    }

                    if (tempmapcell.unit != null)
                    {
                        var tempunit = tempmapcell.unit;
                        tempunit.Animation.Position = curpos;
                        tempunit.Animation.Draw(gameTime, spriteBatch);
                    }
                }
            }
        }

        public static void Render(Map map, bool[,] fogofwar, SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!map.IsProcessed)
            {
                Process(map);
            }

            MapCell tempmapcell;
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    Vector2 curpos = new Vector2(i * Constants.MapCellWidth, j * Constants.MapCellHeight);
                    tempmapcell = map[i, j];
                    if (fogofwar[i, j])
                    {
                        if (tempmapcell.terrainbase != SpriteSheetTerrain.None)
                        {
                            spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, TerrainSpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainbase), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainBase);
                        }
                        if (tempmapcell.terrainLower != SpriteSheetTerrain.None)
                        {
                            spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, TerrainSpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainLower), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainLower);
                        }
                        if (tempmapcell.terrainUpper != SpriteSheetTerrain.None)
                        {
                            spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, TerrainSpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainUpper), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainUpper);
                        }

                        if (tempmapcell.unit != null)
                        {
                            var tempunit = tempmapcell.unit;
                            tempunit.Animation.Position = curpos;
                            tempunit.Animation.Draw(gameTime, spriteBatch);
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        public static void Process(Map map)
        {
            try
            {
                Point pos = new Point(0, 0);
                Point center;

                bool isUp = false
                    , isDown = false
                    , isLeft = false
                    , isRight = false;
                Point up = pos.GetNearbyPoint(Direction.North)
                       , down = pos.GetNearbyPoint(Direction.South)
                       , left = pos.GetNearbyPoint(Direction.West)
                       , right = pos.GetNearbyPoint(Direction.East)
                       , upleft = pos.GetNearbyPoint(Direction.NorthWest)
                       , upright = pos.GetNearbyPoint(Direction.NorthEast)
                       , downleft = pos.GetNearbyPoint(Direction.SouthWest)
                       , downright = pos.GetNearbyPoint(Direction.SouthEast);
                SpriteSheetTerrain result = SpriteSheetTerrain.Road_hor;
                int nextowner = 0;
                int nextwater = 0;
                int nextroadtreemnt = 0;

                switch (map.weather)
                {
                    case Weather.Sunny:
                        switch (map.theme)
                        {
                            case Theme.Normal:
                                nextwater = 0;
                                nextroadtreemnt = 0;
                                break;
                            case Theme.Tropical:
                                nextwater = SpriteSheetTerrain.Desert_Reef - SpriteSheetTerrain.Reef;
                                nextroadtreemnt = SpriteSheetTerrain.Tropical_Road_hor - SpriteSheetTerrain.Road_hor;
                                break;
                            case Theme.Desert:
                                nextwater = SpriteSheetTerrain.Desert_Reef - SpriteSheetTerrain.Reef;
                                nextroadtreemnt = SpriteSheetTerrain.Desert_Road_hor - SpriteSheetTerrain.Road_hor;
                                break;
                            default:
                                break;
                        }
                        break;
                    case Weather.Rain:
                        nextwater = SpriteSheetTerrain.Rain_Reef - SpriteSheetTerrain.Reef;
                        nextroadtreemnt = SpriteSheetTerrain.Rain_Road_hor - SpriteSheetTerrain.Road_hor;
                        break;
                    case Weather.Snow:
                        nextwater = SpriteSheetTerrain.Snow_Reef - SpriteSheetTerrain.Reef;
                        nextroadtreemnt = SpriteSheetTerrain.Snow_Road_hor - SpriteSheetTerrain.Road_hor;
                        break;
                    default:
                        break;
                }

                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        pos.X = x;
                        pos.Y = y;

                        switch (map[pos].owner)
                        {
                            case Owner.None:
                                nextowner = 0;
                                break;
                            case Owner.Red:
                                nextowner = SpriteSheetTerrain.Red_City_Lower - SpriteSheetTerrain.City_Lower;
                                break;
                            case Owner.Blue:
                                nextowner = SpriteSheetTerrain.Blue_City_Lower - SpriteSheetTerrain.City_Lower;
                                break;
                            case Owner.Green:
                                nextowner = SpriteSheetTerrain.Green_City_Lower - SpriteSheetTerrain.City_Lower;
                                break;
                            case Owner.Yellow:
                                nextowner = SpriteSheetTerrain.Yellow_City_Lower - SpriteSheetTerrain.City_Lower;
                                break;
                            default:
                                break;
                        }

                        switch (map[x, y].terrain)
                        {
                            case TerrainType.Reef:
                                map[pos].ClearRenderData();
                                map[pos].terrainbase = SpriteSheetTerrain.Reef.Next(nextwater);
                                break;

                            case TerrainType.Sea:
                                map[pos].ClearRenderData();
                                map[pos].terrainbase = SpriteSheetTerrain.Sea.Next(nextwater);
                                break;

                            case TerrainType.River:

                                map[pos].ClearRenderData();

                                up = pos.GetNearbyPoint(Direction.North);
                                down = pos.GetNearbyPoint(Direction.South);
                                left = pos.GetNearbyPoint(Direction.West);
                                right = pos.GetNearbyPoint(Direction.East);

                                isUp = map[up]?.terrain == TerrainType.River;
                                isDown = map[down]?.terrain == TerrainType.River;
                                isLeft = map[left]?.terrain == TerrainType.River;
                                isRight = map[right]?.terrain == TerrainType.River;
                                result = SpriteSheetTerrain.River_hor;
                                if (isUp || isDown)
                                {
                                    result = SpriteSheetTerrain.River_ver;
                                }

                                //iver turn
                                {
                                    if (isUp && isLeft)
                                    {
                                        result = SpriteSheetTerrain.River_turn_down_left;
                                    }

                                    if (isDown && isLeft)
                                    {
                                        result = SpriteSheetTerrain.River_turn_up_left;
                                    }

                                    if (isUp && isRight)
                                    {
                                        result = SpriteSheetTerrain.River_turn_down_right;
                                    }

                                    if (isDown && isRight)
                                    {
                                        result = SpriteSheetTerrain.River_turn_up_right;
                                    }

                                }
                                //river intersection 3
                                {
                                    if (isUp && isDown && isLeft)
                                    {
                                        result = SpriteSheetTerrain.River_Inter3_left;
                                    }

                                    if (isUp && isDown && isRight)
                                    {
                                        result = SpriteSheetTerrain.River_Inter3_right;
                                    }

                                    if (isUp && isLeft && isRight)
                                    {
                                        result = SpriteSheetTerrain.River_Inter3_up;
                                    }

                                    if (isDown && isLeft && isRight)
                                    {
                                        result = SpriteSheetTerrain.River_Inter3_down;
                                    }

                                }
                                //river cross
                                if (isUp && isDown && isLeft && isRight)
                                {
                                    result = SpriteSheetTerrain.River_Cross;
                                }



                                map[pos].terrainLower = result.Next(nextroadtreemnt);
                                map[pos].terrainbase = SpriteSheetTerrain.Plain;
                                result = SpriteSheetTerrain.River_hor;
                                break;

                            case TerrainType.Coast:
                                map[pos].ClearRenderData();
                                map[pos].terrainbase = SpriteSheetTerrain.Coast_down.Next(nextwater);
                                break;

                            case TerrainType.Cliff:
                                map[pos].ClearRenderData();
                                map[pos].terrainbase = SpriteSheetTerrain.Cliff_down.Next(nextwater);
                                break;


                            case TerrainType.Road:

                                map[pos].ClearRenderData();

                                up = pos.GetNearbyPoint(Direction.North);
                                down = pos.GetNearbyPoint(Direction.South);
                                left = pos.GetNearbyPoint(Direction.West);
                                right = pos.GetNearbyPoint(Direction.East);

                                isUp = (map[up]?.terrain == TerrainType.Road) || (map[up]?.terrain == TerrainType.Bridge);
                                isDown = (map[down]?.terrain == TerrainType.Road) || (map[down]?.terrain == TerrainType.Bridge);
                                isLeft = (map[left]?.terrain == TerrainType.Road) || (map[left]?.terrain == TerrainType.Bridge);
                                isRight = (map[right]?.terrain == TerrainType.Road) || (map[right]?.terrain == TerrainType.Bridge);
                                result = SpriteSheetTerrain.Road_hor;
                                if (isUp || isDown)
                                {
                                    result = SpriteSheetTerrain.Road_ver;
                                }

                                //road turn
                                {
                                    if (isUp && isLeft)
                                    {
                                        result = SpriteSheetTerrain.Road_turn_up_left;
                                    }

                                    if (isDown && isLeft)
                                    {
                                        result = SpriteSheetTerrain.Road_turn_down_left;
                                    }

                                    if (isUp && isRight)
                                    {
                                        result = SpriteSheetTerrain.Road_turn_up_right;
                                    }

                                    if (isDown && isRight)
                                    {
                                        result = SpriteSheetTerrain.Road_turn_down_right;
                                    }

                                }
                                //road intersection 3
                                {
                                    if (isUp && isDown && isLeft)
                                    {
                                        result = SpriteSheetTerrain.Road_Inter3_left;
                                    }

                                    if (isUp && isDown && isRight)
                                    {
                                        result = SpriteSheetTerrain.Road_Inter3_right;
                                    }

                                    if (isUp && isLeft && isRight)
                                    {
                                        result = SpriteSheetTerrain.Road_Inter3_up;
                                    }

                                    if (isDown && isLeft && isRight)
                                    {
                                        result = SpriteSheetTerrain.Road_Inter3_down;
                                    }

                                }
                                //road cross
                                if (isUp && isDown && isLeft && isRight)
                                {
                                    result = SpriteSheetTerrain.Road_Cross;
                                }



                                map[pos].terrainLower = result.Next(nextroadtreemnt);
                                map[pos].terrainbase = SpriteSheetTerrain.Plain;
                                result = SpriteSheetTerrain.Road_hor;
                                break;

                            case TerrainType.Bridge:
                                var tempterrainbase = map[pos].terrainbase;
                                map[pos].ClearRenderData();
                                map[pos].terrainbase = tempterrainbase;
                                map[pos].terrainLower = SpriteSheetTerrain.Bridge_hor.Next(nextroadtreemnt);

                                up = pos.GetNearbyPoint(Direction.North);
                                down = pos.GetNearbyPoint(Direction.South);

                                isUp = (map[up]?.terrain == TerrainType.Road) || (map[up]?.terrain == TerrainType.Bridge);
                                isDown = (map[down]?.terrain == TerrainType.Road) || (map[down]?.terrain == TerrainType.Bridge);

                                if (isUp && isDown)
                                {
                                    map[pos].terrainLower = SpriteSheetTerrain.Bridge_ver.Next(nextroadtreemnt);
                                }
                                break;

                            case TerrainType.Plain:
                                map[pos].ClearRenderData();
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                break;

                            //todo: fix this shit
                            case TerrainType.Tree:
                                //desert ko co forest
                                if (map.theme == Theme.Desert)
                                {
                                    map[pos].ClearRenderData();
                                    map[pos].terrainbase = SpriteSheetTerrain.Desert_Plain;
                                    map[pos].terrainLower = SpriteSheetTerrain.Desert_Tree;
                                    break;
                                }

                                /*
                                 * T1 T2 T3
                                 * T4 T5 T6
                                 * T7 T8 T9
                                 * 
                                 * kiem tra xem o t5 co phai la tree khong va co phai da xu li hay k
                                 * da xu li nghia la terrainLower cua no khong phai la tree.
                                 * sau do ktra t2 va t4. neu la tree thi 
                                 * 
                                 */

                                //kiem tra xem co phai la tree 1x1 hoac none hay khong
                                //neu co thi xu li tu dau
                                //neu khong thi thoat vi da xu li roi
                                if (map[pos].terrainLower != SpriteSheetTerrain.Tree_top_left.Next(nextroadtreemnt)
                                 && map[pos].terrainLower != SpriteSheetTerrain.Tree.Next(nextroadtreemnt)
                                 && map[pos].terrainLower != SpriteSheetTerrain.None)
                                {
                                    break;
                                }

                                map[pos].ClearRenderData();

                                up = pos.GetNearbyPoint(Direction.North);
                                down = pos.GetNearbyPoint(Direction.South);
                                left = pos.GetNearbyPoint(Direction.West);
                                right = pos.GetNearbyPoint(Direction.East);
                                upleft = pos.GetNearbyPoint(Direction.NorthWest);
                                upright = pos.GetNearbyPoint(Direction.NorthEast);
                                downleft = pos.GetNearbyPoint(Direction.SouthWest);
                                downright = pos.GetNearbyPoint(Direction.SouthEast);

                                Point downright2 = downright.GetNearbyPoint(Direction.SouthEast);
                                Point right2 = right.GetNearbyPoint(Direction.East);
                                Point down2 = down.GetNearbyPoint(Direction.South);

                                bool isDownright = (map[downright]?.terrain == TerrainType.Tree);// && (map[downright]?.terrainLower == SpriteSheetTerrain.Tree.Next(nextroadtreemnt));
                                bool isDownright2 = (map[downright2]?.terrain == TerrainType.Tree);// && (map[downright2]?.terrainLower == SpriteSheetTerrain.Tree.Next(nextroadtreemnt));
                                isRight = (map[right]?.terrain == TerrainType.Tree);// && (map[right]?.terrainLower == SpriteSheetTerrain.Tree.Next(nextroadtreemnt));
                                isDown = (map[down]?.terrain == TerrainType.Tree);// && (map[down]?.terrainLower == SpriteSheetTerrain.Tree.Next(nextroadtreemnt));

                                bool is2x2 = false;
                                bool is3x3 = false;

                                //check if 2x2:
                                if (isDownright)
                                {
                                    if (isRight && isDown)
                                    {
                                        is2x2 = true;
                                    }
                                }

                                //check if 3x3:
                                if (is2x2 && isDownright2)
                                {
                                    if (map[right2]?.terrain == TerrainType.Tree
                                     && map[down2]?.terrain == TerrainType.Tree
                                     && map[downright2.GetNearbyPoint(Direction.North)]?.terrain == TerrainType.Tree
                                     && map[downright2.GetNearbyPoint(Direction.West)]?.terrain == TerrainType.Tree)
                                    {
                                        is3x3 = true;
                                    }
                                }

                                //it's render time!

                                //single tree
                                if (map[pos].terrainbase == SpriteSheetTerrain.None)
                                {
                                    map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[pos].terrainLower = SpriteSheetTerrain.Tree.Next(nextroadtreemnt);
                                }

                                if (is3x3)
                                {
                                    center = downright;

                                    //laydown the base
                                    map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.North)].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.West)].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[center].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.East)].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.South)].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);

                                    //place the tile
                                    map[center.GetNearbyPoint(Direction.NorthWest)].terrainLower = SpriteSheetTerrain.Tree_up_left.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.North)].terrainLower = SpriteSheetTerrain.Tree_up_middle.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.NorthEast)].terrainLower = SpriteSheetTerrain.Tree_up_right.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.West)].terrainLower = SpriteSheetTerrain.Tree_middle_left.Next(nextroadtreemnt);
                                    map[center].terrainLower = SpriteSheetTerrain.Tree_middle_middle.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.East)].terrainLower = SpriteSheetTerrain.Tree_middle_right.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.SouthWest)].terrainLower = SpriteSheetTerrain.Tree_down_left.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.South)].terrainLower = SpriteSheetTerrain.Tree_down_middle.Next(nextroadtreemnt);
                                    map[center.GetNearbyPoint(Direction.SouthEast)].terrainLower = SpriteSheetTerrain.Tree_down_right.Next(nextroadtreemnt);
                                    break;
                                }

                                if (is2x2)
                                {
                                    map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[right].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[down].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                    map[downright].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);

                                    map[pos].terrainLower = SpriteSheetTerrain.Tree_top_left.Next(nextroadtreemnt);
                                    map[right].terrainLower = SpriteSheetTerrain.Tree_top_right.Next(nextroadtreemnt);
                                    map[down].terrainLower = SpriteSheetTerrain.Tree_bottom_left.Next(nextroadtreemnt);
                                    map[downright].terrainLower = SpriteSheetTerrain.Tree_bottom_right.Next(nextroadtreemnt);
                                    break;
                                }
                                break;

                            case TerrainType.Mountain:
                                SpriteSheetTerrain north = SpriteSheetTerrain.None;
                                switch (map.weather)
                                {
                                    case Weather.Sunny:
                                        switch (map.theme)
                                        {
                                            case Theme.Normal:
                                                map[pos].terrainbase = SpriteSheetTerrain.Mountain_High_Lower;
                                                north = SpriteSheetTerrain.Mountain_High_Upper;
                                                break;
                                            case Theme.Tropical:
                                                map[pos].terrainbase = SpriteSheetTerrain.Tropical_Mountain_High_Lower;
                                                north = SpriteSheetTerrain.Tropical_Mountain_High_Upper;
                                                break;
                                            case Theme.Desert:
                                                map[pos].terrainbase = SpriteSheetTerrain.Desert_Mountain_High_Lower;
                                                north = SpriteSheetTerrain.Desert_Mountain_High_Upper;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case Weather.Rain:
                                        map[pos].terrainbase = SpriteSheetTerrain.Rain_Mountain_High_Lower;
                                        north = SpriteSheetTerrain.Rain_Mountain_High_Upper;
                                        break;
                                    case Weather.Snow:
                                        map[pos].terrainbase = SpriteSheetTerrain.Snow_Mountain_High_Lower;
                                        north = SpriteSheetTerrain.Snow_Mountain_High_Upper;
                                        break;
                                    default:
                                        break;
                                }
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = north;
                                }
                                catch { }
                                break;

                            case TerrainType.MissileSilo:
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.Missile_Silo_Lower;
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Missile_Silo_Upper;
                                }
                                catch { }
                                break;

                            case TerrainType.MissileSiloLaunched:
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.Missile_Silo_Launched;
                                try {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.None;
                                }
                                catch { }
                                break;

                            case TerrainType.City:
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.City_Lower.Next(nextowner);
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.City_Upper.Next(nextowner);
                                }
                                catch { }
                                break;
                            case TerrainType.Factory:
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.Factory.Next(nextowner);
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.None;
                                }
                                catch { }
                                break;
                            case TerrainType.AirPort:
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.AirPort_Lower.Next(nextowner);
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.AirPort_Upper.Next(nextowner);
                                }
                                catch { }
                                break;
                            case TerrainType.Harbor:
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.Harbor_Lower.Next(nextowner);
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Harbor_Upper.Next(nextowner);
                                }
                                catch { }
                                break;
                            case TerrainType.Radar:
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.Radar_Lower.Next(nextowner);
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Radar_Upper.Next(nextowner);
                                }
                                catch { }
                                break;
                            case TerrainType.SupplyBase:
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.SupplyBase_Lower.Next(nextowner);
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.SupplyBase_Upper.Next(nextowner);
                                }
                                catch { }
                                break;
                            case TerrainType.HQ:
                                switch (map[pos].owner)
                                {
                                    case Owner.Red:
                                        nextowner = 0;
                                        break;
                                    case Owner.Blue:
                                        nextowner = SpriteSheetTerrain.Blue_Headquarter_Lower - SpriteSheetTerrain.Red_Headquarter_Lower;
                                        break;
                                    case Owner.Green:
                                        nextowner = SpriteSheetTerrain.Green_Headquarter_Lower - SpriteSheetTerrain.Red_Headquarter_Lower;
                                        break;
                                    case Owner.Yellow:
                                        nextowner = SpriteSheetTerrain.Yellow_Headquarter_Lower - SpriteSheetTerrain.Red_Headquarter_Lower;
                                        break;
                                    default:
                                        break;
                                }
                                map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                                map[pos].terrainLower = SpriteSheetTerrain.Red_Headquarter_Lower.Next(nextowner);
                                try
                                {
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Red_Headquarter_Upper.Next(nextowner);
                                }
                                catch { }
                                break;
                            default:
                                break;
                        }
                    }
                }
                map.IsProcessed = true;
            }
            catch (Exception er)
            {
                Utility.HelperFunction.Log(er);
            }
        }

    }
}
