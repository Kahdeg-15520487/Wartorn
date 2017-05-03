using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wartorn.GameData;
using Wartorn.Drawing.Animation;
using Wartorn.Utility;

namespace Wartorn.Drawing
{
    static class MapRenderer
    {
        //TODO do the maprenderer class
        public static void Render(Map map,SpriteBatch spriteBatch,GameTime gameTime)
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
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainbase), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainBase);
                    }
                    if (tempmapcell.terrainLower != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainLower), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainLower);
                    }
                    if (tempmapcell.terrainUpper != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainUpper), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainUpper);
                    }

                    if (tempmapcell.unit!=null)
                    {
                        var tempunit = tempmapcell.unit;
                        tempunit.Animation.Position = curpos;
                        tempunit.Animation.Draw(gameTime,spriteBatch);
                    }
                }
            }
        }

        public static void Process(Map map)
        {
            Point pos = new Point(0, 0);

            bool isUp = false
                , isDown = false
                , isLeft = false
                , isRight = false;
            Point up = pos.GetNearbyPoint(Direction.North)
                   , down = pos.GetNearbyPoint(Direction.South)
                   , left = pos.GetNearbyPoint(Direction.West)
                   , right = pos.GetNearbyPoint(Direction.East);
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
                            map[pos].terrainbase = SpriteSheetTerrain.Reef.Next(nextwater);
                            break;

                        case TerrainType.Sea:
                            map[pos].terrainbase = SpriteSheetTerrain.Sea.Next(nextwater);
                            break;

                        case TerrainType.River:
                            up = pos.GetNearbyPoint(Direction.North);
                            down = pos.GetNearbyPoint(Direction.South);
                            left = pos.GetNearbyPoint(Direction.West);
                            right = pos.GetNearbyPoint(Direction.East);

                            isUp = map[up]?.terrain == TerrainType.Road;
                            isDown = map[down]?.terrain == TerrainType.Road;
                            isLeft = map[left]?.terrain == TerrainType.Road;
                            isRight = map[right]?.terrain == TerrainType.Road;

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

                        case TerrainType.Coast:
                            break;

                        case TerrainType.Cliff:
                            break;


                        case TerrainType.Road:

                            up = pos.GetNearbyPoint(Direction.North);
                            down = pos.GetNearbyPoint(Direction.South);
                            left = pos.GetNearbyPoint(Direction.West);
                            right = pos.GetNearbyPoint(Direction.East);

                            isUp = map[up]?.terrain == TerrainType.Road;
                            isDown = map[down]?.terrain == TerrainType.Road;
                            isLeft = map[left]?.terrain == TerrainType.Road;
                            isRight = map[right]?.terrain == TerrainType.Road;

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



                        case TerrainType.Plain:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            break;

                        case TerrainType.Tree:
                            break;

                        case TerrainType.Mountain:
                            switch (map.weather)
                            {
                                case Weather.Sunny:
                                    switch (map.theme)
                                    {
                                        case Theme.Normal:
                                            map[pos].terrainbase = SpriteSheetTerrain.Mountain_High_Lower;
                                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Mountain_High_Upper;
                                            break;
                                        case Theme.Tropical:
                                            map[pos].terrainbase = SpriteSheetTerrain.Tropical_Mountain_High_Lower;
                                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Tropical_Mountain_High_Upper;
                                            break;
                                        case Theme.Desert:
                                            map[pos].terrainbase = SpriteSheetTerrain.Desert_Mountain_High_Lower;
                                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Desert_Mountain_High_Upper;
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case Weather.Rain:
                                    map[pos].terrainbase = SpriteSheetTerrain.Rain_Mountain_High_Lower;
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Rain_Mountain_High_Upper;
                                    break;
                                case Weather.Snow:
                                    map[pos].terrainbase = SpriteSheetTerrain.Snow_Mountain_High_Lower;
                                    map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Snow_Mountain_High_Upper;
                                    break;
                                default:
                                    break;
                            }
                            break;

                        case TerrainType.MissileSilo:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.Missile_Silo_Lower;
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Missile_Silo_Upper;
                            break;

                        case TerrainType.MissileSiloLaunched:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.Missile_Silo_Launched;
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.None;
                            break;

                        case TerrainType.City:                            
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.City_Lower.Next(nextowner);
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.City_Upper.Next(nextowner);
                            break;
                        case TerrainType.Factory:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.Factory.Next(nextowner);
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.None;
                            break;
                        case TerrainType.AirPort:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.AirPort_Lower.Next(nextowner);
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.AirPort_Upper.Next(nextowner);
                            break;
                        case TerrainType.Harbor:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.Harbor_Lower.Next(nextowner);
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Harbor_Upper.Next(nextowner);
                            break;
                        case TerrainType.Radar:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.Radar_Lower.Next(nextowner);
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Radar_Upper.Next(nextowner);
                            break;
                        case TerrainType.SupplyBase:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.SupplyBase_Lower.Next(nextowner);
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.SupplyBase_Upper.Next(nextowner);
                            break;
                        case TerrainType.Headquarter:
                            map[pos].terrainbase = SpriteSheetTerrain.Plain.Next(nextroadtreemnt);
                            map[pos].terrainLower = SpriteSheetTerrain.Red_Headquarter_Lower.Next(nextowner - (int)SpriteSheetTerrain.Red_Headquarter_Lower);
                            map[pos.GetNearbyPoint(Direction.North)].terrainUpper = SpriteSheetTerrain.Red_Headquarter_Upper.Next(nextowner - (int)SpriteSheetTerrain.Red_Headquarter_Lower);
                            break;
                        default:
                            break;
                    }
                }
            }
            map.IsProcessed = true;
        }
    }
}
