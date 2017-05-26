using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Wartorn.GameData;
using Wartorn.Drawing;
using Wartorn.SpriteRectangle;

namespace Wartorn
{
    namespace Utility
    {
        public static class HelperFunction
        {
            /// <summary>
            /// convert angle from degree to radian. use to ease process of drawing sprite
            /// </summary>
            /// <param name="angle"></param>
            /// <returns></returns>
            public static float DegreeToRadian(float angle)
            {
                return (float)(Math.PI * angle / 180.0f);
            }

            public static bool IsKeyPress(Keys k)
            {
                return CONTENT_MANAGER.inputState.keyboardState.IsKeyUp(k) && CONTENT_MANAGER.lastInputState.keyboardState.IsKeyDown(k);
            }

            public static bool IsLeftMousePressed()
            {
                return CONTENT_MANAGER.inputState.mouseState.LeftButton == ButtonState.Pressed;
            }

            public static bool IsRightMousePressed()
            {
                return CONTENT_MANAGER.inputState.mouseState.RightButton == ButtonState.Pressed;
            }

            public static void Log(Exception e)
            {
                File.WriteAllText("crashlog.txt", DateTime.Now.ToString(@"dd\/MM\/yyyy HH:mm") + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.TargetSite);
            }

            public static void Log(string msg)
            {
                File.WriteAllText("crashlog.txt", DateTime.Now.ToString(@"dd\/MM\/yyyy HH:mm") + Environment.NewLine + msg);
            }


            public static Point TranslateMousePosToMapCellPos(Point mousepos, Camera camera, int width, int height)
            {
                //calculate currently selected mapcell
                Vector2 temp = camera.TranslateFromScreenToWorld(mousepos.ToVector2());
                temp.X = (int)(temp.X / Constants.MapCellWidth);       //mapcell size
                temp.Y = (int)(temp.Y / Constants.MapCellHeight);

                if (temp.X >= 0 && temp.X < width && temp.Y >= 0 && temp.Y < height)
                    return temp.ToPoint();
                return Point.Zero;
            }

            /// <summary>
            /// A -> B -> C
            /// </summary>
            /// <param name="pA"></param>
            /// <param name="pB"></param>
            /// <param name="pC"></param>
            /// <returns></returns>
            public static Direction GetIntersectionDir(Point pA, Point pB, Point pC)
            {
                var indir = pB.GetDirectionFromPointAtoPointB(pA);
                var outdir = pB.GetDirectionFromPointAtoPointB(pC);

                Direction result = Direction.Void;

                switch (indir)
                {
                    case Direction.North:
                        switch (outdir)
                        {
                            case Direction.West:
                                result = Direction.NorthWest;
                                break;
                            case Direction.East:
                                result = Direction.NorthEast;
                                break;
                            case Direction.South:
                                result = Direction.South;
                                break;
                            default:
                                break;
                        }
                        break;

                    case Direction.South:
                        switch (outdir)
                        {
                            case Direction.West:
                                result = Direction.SouthWest;
                                break;
                            case Direction.East:
                                result = Direction.SouthEast;
                                break;
                            case Direction.North:
                                result = Direction.South;
                                break;
                            default:
                                break;
                        }
                        break;

                    case Direction.West:
                        switch (outdir)
                        {
                            case Direction.North:
                                result = Direction.NorthWest;
                                break;
                            case Direction.South:
                                result = Direction.SouthWest;
                                break;
                            case Direction.East:
                                result = Direction.East;
                                break;
                            default:
                                break;
                        }
                        break;

                    case Direction.East:
                        switch (outdir)
                        {
                            case Direction.North:
                                result = Direction.NorthEast;
                                break;
                            case Direction.South:
                                result = Direction.SouthEast;
                                break;
                            case Direction.West:
                                result = Direction.East;
                                break;
                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }

                return result;
            }
        }

        public static class ExtensionMethod
        {
            public static T ToEnum<T>(this string value)
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            public static int Clamp(this float value,int max,int min)
            {
                int v = (int)value;
                return v >= max ? max : v <= min ? min : v;
            }

            public static int Clamp(this int value, int max, int min)
            {
                return value >= max ? max : value <= min ? min : value;
            }

            public static bool Between(this int value, int max, int min)
            {
                return value < max && value > min;
            }

            public static int CompareWith(this SpriteSheetTerrain t, SpriteSheetTerrain other)
            {
                return ((int)t).CompareTo((int)other);
            }

            public static bool Is2Tile(this TerrainType t)
            {
                switch (t)
                {
                    case TerrainType.Reef:
                    case TerrainType.Sea:
                    case TerrainType.River:
                    case TerrainType.Coast:
                    case TerrainType.Cliff:
                    case TerrainType.Road:
                    case TerrainType.Plain:
                    case TerrainType.Tree:
                    case TerrainType.MissileSiloLaunched:
                    case TerrainType.Factory:
                        return false;

                    case TerrainType.Mountain:
                    case TerrainType.MissileSilo:
                    case TerrainType.City:
                    case TerrainType.AirPort:
                    case TerrainType.Harbor:
                    case TerrainType.Radar:
                    case TerrainType.SupplyBase:
                    case TerrainType.HQ:
                        return true;
                    default:
                        break;
                }
                return false;
            }

            public static MovementType GetMovementType(this UnitType ut)
            {
                switch (ut)
                {
                    case UnitType.Soldier:
                        return MovementType.Soldier;
                    case UnitType.Mech:
                        return MovementType.Mech;

                    case UnitType.Recon:
                    case UnitType.Rocket:
                    case UnitType.Missile:
                        return MovementType.Tires;

                    case UnitType.APC:
                    case UnitType.Tank:
                    case UnitType.HeavyTank:
                    case UnitType.Artillery:
                    case UnitType.AntiAir:
                        return MovementType.Track;

                    case UnitType.TransportCopter:
                    case UnitType.BattleCopter:
                    case UnitType.Fighter:
                    case UnitType.Bomber:
                        return MovementType.Air;

                    case UnitType.Lander:
                        return MovementType.Lander;

                    case UnitType.Cruise:
                    case UnitType.Submarine:
                    case UnitType.Battleship:
                        return MovementType.Ship;
                    default:
                        break;
                }
                return MovementType.None;
            }

            public static SpriteSheetUnit GetSpriteSheetUnit(this UnitType ut,Owner owner)
            {
                StringBuilder result = new StringBuilder();
                result.Append(owner.ToString());
                result.Append("_");
                result.Append(ut.ToString());
                return result.ToString().ToEnum<SpriteSheetUnit>();
            }

            public static TerrainType ToTerrainType(this SpriteSheetTerrain t)
            {
                TerrainType result = TerrainType.Plain;
                switch (t)
                {
                    case SpriteSheetTerrain.Reef:
                    case SpriteSheetTerrain.Rain_Reef:
                    case SpriteSheetTerrain.Snow_Reef:
                    case SpriteSheetTerrain.Desert_Reef:
                        result = TerrainType.Reef;
                        break;





                    case SpriteSheetTerrain.Sea:
                    case SpriteSheetTerrain.Rain_Sea:
                    case SpriteSheetTerrain.Snow_Sea:
                    case SpriteSheetTerrain.Desert_Sea:
                        result = TerrainType.Sea;
                        break;





                    case SpriteSheetTerrain.River_ver:
                    case SpriteSheetTerrain.River_hor:
                    case SpriteSheetTerrain.River_Inter3_right:
                    case SpriteSheetTerrain.River_Inter3_left:
                    case SpriteSheetTerrain.River_Inter3_up:
                    case SpriteSheetTerrain.River_Inter3_down:
                    case SpriteSheetTerrain.River_Cross:
                    case SpriteSheetTerrain.River_turn_up_right:
                    case SpriteSheetTerrain.River_turn_up_left:
                    case SpriteSheetTerrain.River_turn_down_right:
                    case SpriteSheetTerrain.River_turn_down_left:
                    case SpriteSheetTerrain.River_Flow_left:
                    case SpriteSheetTerrain.River_Flow_up:
                    case SpriteSheetTerrain.River_Flow_down:
                    case SpriteSheetTerrain.River_Flow_right:

                    case SpriteSheetTerrain.Rain_River_ver:
                    case SpriteSheetTerrain.Rain_River_hor:
                    case SpriteSheetTerrain.Rain_River_Inter3_right:
                    case SpriteSheetTerrain.Rain_River_Inter3_left:
                    case SpriteSheetTerrain.Rain_River_Inter3_up:
                    case SpriteSheetTerrain.Rain_River_Inter3_down:
                    case SpriteSheetTerrain.Rain_River_Cross:
                    case SpriteSheetTerrain.Rain_River_turn_up_right:
                    case SpriteSheetTerrain.Rain_River_turn_up_left:
                    case SpriteSheetTerrain.Rain_River_turn_down_right:
                    case SpriteSheetTerrain.Rain_River_turn_down_left:
                    case SpriteSheetTerrain.Rain_River_Flow_left:
                    case SpriteSheetTerrain.Rain_River_Flow_up:
                    case SpriteSheetTerrain.Rain_River_Flow_down:
                    case SpriteSheetTerrain.Rain_River_Flow_right:

                    case SpriteSheetTerrain.Snow_River_ver:
                    case SpriteSheetTerrain.Snow_River_hor:
                    case SpriteSheetTerrain.Snow_River_Inter3_right:
                    case SpriteSheetTerrain.Snow_River_Inter3_left:
                    case SpriteSheetTerrain.Snow_River_Inter3_up:
                    case SpriteSheetTerrain.Snow_River_Inter3_down:
                    case SpriteSheetTerrain.Snow_River_Cross:
                    case SpriteSheetTerrain.Snow_River_turn_up_right:
                    case SpriteSheetTerrain.Snow_River_turn_up_left:
                    case SpriteSheetTerrain.Snow_River_turn_down_right:
                    case SpriteSheetTerrain.Snow_River_turn_down_left:
                    case SpriteSheetTerrain.Snow_River_Flow_left:
                    case SpriteSheetTerrain.Snow_River_Flow_up:
                    case SpriteSheetTerrain.Snow_River_Flow_down:
                    case SpriteSheetTerrain.Snow_River_Flow_right:

                    case SpriteSheetTerrain.Desert_River_ver:
                    case SpriteSheetTerrain.Desert_River_hor:
                    case SpriteSheetTerrain.Desert_River_Inter3_right:
                    case SpriteSheetTerrain.Desert_River_Inter3_left:
                    case SpriteSheetTerrain.Desert_River_Inter3_up:
                    case SpriteSheetTerrain.Desert_River_Inter3_down:
                    case SpriteSheetTerrain.Desert_River_Cross:
                    case SpriteSheetTerrain.Desert_River_turn_up_right:
                    case SpriteSheetTerrain.Desert_River_turn_up_left:
                    case SpriteSheetTerrain.Desert_River_turn_down_right:
                    case SpriteSheetTerrain.Desert_River_turn_down_left:
                    case SpriteSheetTerrain.Desert_River_Flow_left:
                    case SpriteSheetTerrain.Desert_River_Flow_up:
                    case SpriteSheetTerrain.Desert_River_Flow_down:
                    case SpriteSheetTerrain.Desert_River_Flow_right:
                        result = TerrainType.River;
                        break;





                    case SpriteSheetTerrain.Coast_up_left:
                    case SpriteSheetTerrain.Coast_up:
                    case SpriteSheetTerrain.Coast_up_right:
                    case SpriteSheetTerrain.Coast_left:
                    case SpriteSheetTerrain.Coast_right:
                    case SpriteSheetTerrain.Coast_down_left:
                    case SpriteSheetTerrain.Coast_down:
                    case SpriteSheetTerrain.Coast_down_right:
                    case SpriteSheetTerrain.Lone_Coast_up_left:
                    case SpriteSheetTerrain.Lone_Coast_up_right:
                    case SpriteSheetTerrain.Lone_Coast_down_left:
                    case SpriteSheetTerrain.Lone_Coast_down_right:
                    case SpriteSheetTerrain.Lone_Coast_up:
                    case SpriteSheetTerrain.Lone_Coast_down:
                    case SpriteSheetTerrain.Lone_Coast_right:
                    case SpriteSheetTerrain.Lone_Coast_left:
                    case SpriteSheetTerrain.Invert_Coast_down_left:
                    case SpriteSheetTerrain.Invert_Coast_down_right:
                    case SpriteSheetTerrain.Invert_Coast_up_left:
                    case SpriteSheetTerrain.Invert_Coast_up_right:
                    case SpriteSheetTerrain.Invert_Coast_left_down:
                    case SpriteSheetTerrain.Invert_Coast_left_up:
                    case SpriteSheetTerrain.Invert_Coast_right_up:
                    case SpriteSheetTerrain.Invert_Coast_right_down:
                    case SpriteSheetTerrain.Isle_Coast_up_left:
                    case SpriteSheetTerrain.Isle_Coast_up_right:
                    case SpriteSheetTerrain.Isle_Coast_side_right_up:
                    case SpriteSheetTerrain.Isle_Coast_side_right_down:
                    case SpriteSheetTerrain.Isle_Coast_side_left_up:
                    case SpriteSheetTerrain.Isle_Coast_side_left_down:
                    case SpriteSheetTerrain.Isle_Coast_down_left:
                    case SpriteSheetTerrain.Isle_Coast_down_right:

                    case SpriteSheetTerrain.Rain_Coast_up_left:
                    case SpriteSheetTerrain.Rain_Coast_up:
                    case SpriteSheetTerrain.Rain_Coast_up_right:
                    case SpriteSheetTerrain.Rain_Coast_left:
                    case SpriteSheetTerrain.Rain_Coast_right:
                    case SpriteSheetTerrain.Rain_Coast_down_left:
                    case SpriteSheetTerrain.Rain_Coast_down:
                    case SpriteSheetTerrain.Rain_Coast_down_right:
                    case SpriteSheetTerrain.Rain_Isle_Coast_up_left:
                    case SpriteSheetTerrain.Rain_Isle_Coast_up_right:
                    case SpriteSheetTerrain.Rain_Isle_Coast_side_right_up:
                    case SpriteSheetTerrain.Rain_Isle_Coast_side_right_down:
                    case SpriteSheetTerrain.Rain_Isle_Coast_side_left_up:
                    case SpriteSheetTerrain.Rain_Isle_Coast_side_left_down:
                    case SpriteSheetTerrain.Rain_Isle_Coast_down_left:
                    case SpriteSheetTerrain.Rain_Isle_Coast_down_right:
                    case SpriteSheetTerrain.Rain_Lone_Coast_up_left:
                    case SpriteSheetTerrain.Rain_Lone_Coast_up_right:
                    case SpriteSheetTerrain.Rain_Lone_Coast_down_left:
                    case SpriteSheetTerrain.Rain_Lone_Coast_down_right:
                    case SpriteSheetTerrain.Rain_Lone_Coast_up:
                    case SpriteSheetTerrain.Rain_Lone_Coast_down:
                    case SpriteSheetTerrain.Rain_Lone_Coast_right:
                    case SpriteSheetTerrain.Rain_Lone_Coast_left:
                    case SpriteSheetTerrain.Rain_Invert_Coast_down_left:
                    case SpriteSheetTerrain.Rain_Invert_Coast_down_right:
                    case SpriteSheetTerrain.Rain_Invert_Coast_up_left:
                    case SpriteSheetTerrain.Rain_Invert_Coast_up_right:
                    case SpriteSheetTerrain.Rain_Invert_Coast_left_down:
                    case SpriteSheetTerrain.Rain_Invert_Coast_left_up:
                    case SpriteSheetTerrain.Rain_Invert_Coast_right_up:
                    case SpriteSheetTerrain.Rain_Invert_Coast_right_down:

                    case SpriteSheetTerrain.Snow_Coast_up_left:
                    case SpriteSheetTerrain.Snow_Coast_up:
                    case SpriteSheetTerrain.Snow_Coast_up_right:
                    case SpriteSheetTerrain.Snow_Coast_left:
                    case SpriteSheetTerrain.Snow_Coast_right:
                    case SpriteSheetTerrain.Snow_Coast_down_left:
                    case SpriteSheetTerrain.Snow_Coast_down:
                    case SpriteSheetTerrain.Snow_Coast_down_right:
                    case SpriteSheetTerrain.Snow_Isle_Coast_up_left:
                    case SpriteSheetTerrain.Snow_Isle_Coast_up_right:
                    case SpriteSheetTerrain.Snow_Isle_Coast_side_right_up:
                    case SpriteSheetTerrain.Snow_Isle_Coast_side_right_down:
                    case SpriteSheetTerrain.Snow_Isle_Coast_side_left_up:
                    case SpriteSheetTerrain.Snow_Isle_Coast_side_left_down:
                    case SpriteSheetTerrain.Snow_Isle_Coast_down_left:
                    case SpriteSheetTerrain.Snow_Isle_Coast_down_right:
                    case SpriteSheetTerrain.Snow_Lone_Coast_up_left:
                    case SpriteSheetTerrain.Snow_Lone_Coast_up_right:
                    case SpriteSheetTerrain.Snow_Lone_Coast_down_left:
                    case SpriteSheetTerrain.Snow_Lone_Coast_down_right:
                    case SpriteSheetTerrain.Snow_Lone_Coast_up:
                    case SpriteSheetTerrain.Snow_Lone_Coast_down:
                    case SpriteSheetTerrain.Snow_Lone_Coast_right:
                    case SpriteSheetTerrain.Snow_Lone_Coast_left:
                    case SpriteSheetTerrain.Snow_Invert_Coast_down_left:
                    case SpriteSheetTerrain.Snow_Invert_Coast_down_right:
                    case SpriteSheetTerrain.Snow_Invert_Coast_up_left:
                    case SpriteSheetTerrain.Snow_Invert_Coast_up_right:
                    case SpriteSheetTerrain.Snow_Invert_Coast_left_down:
                    case SpriteSheetTerrain.Snow_Invert_Coast_left_up:
                    case SpriteSheetTerrain.Snow_Invert_Coast_right_up:
                    case SpriteSheetTerrain.Snow_Invert_Coast_right_down:

                    case SpriteSheetTerrain.Desert_Coast_up_left:
                    case SpriteSheetTerrain.Desert_Coast_up:
                    case SpriteSheetTerrain.Desert_Coast_up_right:
                    case SpriteSheetTerrain.Desert_Coast_left:
                    case SpriteSheetTerrain.Desert_Coast_right:
                    case SpriteSheetTerrain.Desert_Coast_down_left:
                    case SpriteSheetTerrain.Desert_Coast_down:
                    case SpriteSheetTerrain.Desert_Coast_down_right:
                    case SpriteSheetTerrain.Desert_Isle_Coast_up_left:
                    case SpriteSheetTerrain.Desert_Isle_Coast_up_right:
                    case SpriteSheetTerrain.Desert_Isle_Coast_side_right_up:
                    case SpriteSheetTerrain.Desert_Isle_Coast_side_right_down:
                    case SpriteSheetTerrain.Desert_Isle_Coast_side_left_up:
                    case SpriteSheetTerrain.Desert_Isle_Coast_side_left_down:
                    case SpriteSheetTerrain.Desert_Isle_Coast_down_left:
                    case SpriteSheetTerrain.Desert_Isle_Coast_down_right:
                    case SpriteSheetTerrain.Desert_Lone_Coast_up_left:
                    case SpriteSheetTerrain.Desert_Lone_Coast_up_right:
                    case SpriteSheetTerrain.Desert_Lone_Coast_down_left:
                    case SpriteSheetTerrain.Desert_Lone_Coast_down_right:
                    case SpriteSheetTerrain.Desert_Lone_Coast_up:
                    case SpriteSheetTerrain.Desert_Lone_Coast_down:
                    case SpriteSheetTerrain.Desert_Lone_Coast_right:
                    case SpriteSheetTerrain.Desert_Lone_Coast_left:
                    case SpriteSheetTerrain.Desert_Invert_Coast_down_left:
                    case SpriteSheetTerrain.Desert_Invert_Coast_down_right:
                    case SpriteSheetTerrain.Desert_Invert_Coast_up_left:
                    case SpriteSheetTerrain.Desert_Invert_Coast_up_right:
                    case SpriteSheetTerrain.Desert_Invert_Coast_left_down:
                    case SpriteSheetTerrain.Desert_Invert_Coast_left_up:
                    case SpriteSheetTerrain.Desert_Invert_Coast_right_up:
                    case SpriteSheetTerrain.Desert_Invert_Coast_right_down:
                        result = TerrainType.Coast;
                        break;



                    case SpriteSheetTerrain.Cliff_up_left:
                    case SpriteSheetTerrain.Cliff_up:
                    case SpriteSheetTerrain.Cliff_up_right:
                    case SpriteSheetTerrain.Cliff_down_left:
                    case SpriteSheetTerrain.Cliff_down:
                    case SpriteSheetTerrain.Cliff_down_right:
                    case SpriteSheetTerrain.Cliff_left:
                    case SpriteSheetTerrain.Cliff_right:
                    case SpriteSheetTerrain.Isle_Cliff_down_left:
                    case SpriteSheetTerrain.Isle_Cliff_down_right:
                    case SpriteSheetTerrain.Isle_Cliff_up_left:
                    case SpriteSheetTerrain.Isle_Cliff_up_right:

                    case SpriteSheetTerrain.Rain_Cliff_up_left:
                    case SpriteSheetTerrain.Rain_Cliff_up:
                    case SpriteSheetTerrain.Rain_Cliff_up_right:
                    case SpriteSheetTerrain.Rain_Cliff_down_left:
                    case SpriteSheetTerrain.Rain_Cliff_down:
                    case SpriteSheetTerrain.Rain_Cliff_down_right:
                    case SpriteSheetTerrain.Rain_Cliff_left:
                    case SpriteSheetTerrain.Rain_Cliff_right:
                    case SpriteSheetTerrain.Rain_Isle_Cliff_down_left:
                    case SpriteSheetTerrain.Rain_Isle_Cliff_down_right:
                    case SpriteSheetTerrain.Rain_Isle_Cliff_up_left:
                    case SpriteSheetTerrain.Rain_Isle_Cliff_up_right:

                    case SpriteSheetTerrain.Snow_Cliff_up_left:
                    case SpriteSheetTerrain.Snow_Cliff_up:
                    case SpriteSheetTerrain.Snow_Cliff_up_right:
                    case SpriteSheetTerrain.Snow_Cliff_down_left:
                    case SpriteSheetTerrain.Snow_Cliff_down:
                    case SpriteSheetTerrain.Snow_Cliff_down_right:
                    case SpriteSheetTerrain.Snow_Cliff_left:
                    case SpriteSheetTerrain.Snow_Cliff_right:
                    case SpriteSheetTerrain.Snow_Isle_Cliff_down_left:
                    case SpriteSheetTerrain.Snow_Isle_Cliff_down_right:
                    case SpriteSheetTerrain.Snow_Isle_Cliff_up_left:
                    case SpriteSheetTerrain.Snow_Isle_Cliff_up_right:

                    case SpriteSheetTerrain.Desert_Cliff_up_left:
                    case SpriteSheetTerrain.Desert_Cliff_up:
                    case SpriteSheetTerrain.Desert_Cliff_up_right:
                    case SpriteSheetTerrain.Desert_Cliff_down_left:
                    case SpriteSheetTerrain.Desert_Cliff_down:
                    case SpriteSheetTerrain.Desert_Cliff_down_right:
                    case SpriteSheetTerrain.Desert_Cliff_left:
                    case SpriteSheetTerrain.Desert_Cliff_right:
                    case SpriteSheetTerrain.Desert_Isle_Cliff_down_left:
                    case SpriteSheetTerrain.Desert_Isle_Cliff_down_right:
                    case SpriteSheetTerrain.Desert_Isle_Cliff_up_left:
                    case SpriteSheetTerrain.Desert_Isle_Cliff_up_right:
                        result = TerrainType.Cliff;
                        break;


                    case SpriteSheetTerrain.Bridge_hor:
                    case SpriteSheetTerrain.Bridge_ver:
                    case SpriteSheetTerrain.Tropical_Bridge_hor:
                    case SpriteSheetTerrain.Tropical_Bridge_ver:
                    case SpriteSheetTerrain.Rain_Bridge_hor:
                    case SpriteSheetTerrain.Rain_Bridge_ver:
                    case SpriteSheetTerrain.Snow_Bridge_hor:
                    case SpriteSheetTerrain.Snow_Bridge_ver:
                    case SpriteSheetTerrain.Desert_Bridge_hor:
                    case SpriteSheetTerrain.Desert_Bridge_ver:
                        result =  TerrainType.Bridge;
                        break;

                    case SpriteSheetTerrain.Road_turn_up_right:
                    case SpriteSheetTerrain.Road_turn_up_left:
                    case SpriteSheetTerrain.Road_Inter3_right:
                    case SpriteSheetTerrain.Road_Inter3_down:
                    case SpriteSheetTerrain.Road_hor:
                    case SpriteSheetTerrain.Road_Cross:
                    case SpriteSheetTerrain.Road_turn_down_right:
                    case SpriteSheetTerrain.Road_turn_down_left:
                    case SpriteSheetTerrain.Road_Inter3_up:
                    case SpriteSheetTerrain.Road_Inter3_left:
                    case SpriteSheetTerrain.Road_ver:

                    case SpriteSheetTerrain.Tropical_Road_turn_up_right:
                    case SpriteSheetTerrain.Tropical_Road_turn_up_left:
                    case SpriteSheetTerrain.Tropical_Road_Inter3_right:
                    case SpriteSheetTerrain.Tropical_Road_Inter3_down:
                    case SpriteSheetTerrain.Tropical_Road_hor:
                    case SpriteSheetTerrain.Tropical_Road_Cross:
                    case SpriteSheetTerrain.Tropical_Road_turn_down_right:
                    case SpriteSheetTerrain.Tropical_Road_turn_down_left:
                    case SpriteSheetTerrain.Tropical_Road_Inter3_up:
                    case SpriteSheetTerrain.Tropical_Road_Inter3_left:
                    case SpriteSheetTerrain.Tropical_Road_ver:

                    case SpriteSheetTerrain.Rain_Road_turn_up_right:
                    case SpriteSheetTerrain.Rain_Road_turn_up_left:
                    case SpriteSheetTerrain.Rain_Road_Inter3_right:
                    case SpriteSheetTerrain.Rain_Road_Inter3_down:
                    case SpriteSheetTerrain.Rain_Road_hor:
                    case SpriteSheetTerrain.Rain_Road_Cross:
                    case SpriteSheetTerrain.Rain_Road_turn_down_right:
                    case SpriteSheetTerrain.Rain_Road_turn_down_left:
                    case SpriteSheetTerrain.Rain_Road_Inter3_up:
                    case SpriteSheetTerrain.Rain_Road_Inter3_left:
                    case SpriteSheetTerrain.Rain_Road_ver:

                    case SpriteSheetTerrain.Snow_Road_turn_up_right:
                    case SpriteSheetTerrain.Snow_Road_turn_up_left:
                    case SpriteSheetTerrain.Snow_Road_Inter3_right:
                    case SpriteSheetTerrain.Snow_Road_Inter3_down:
                    case SpriteSheetTerrain.Snow_Road_hor:
                    case SpriteSheetTerrain.Snow_Road_Cross:
                    case SpriteSheetTerrain.Snow_Road_turn_down_right:
                    case SpriteSheetTerrain.Snow_Road_turn_down_left:
                    case SpriteSheetTerrain.Snow_Road_Inter3_up:
                    case SpriteSheetTerrain.Snow_Road_Inter3_left:
                    case SpriteSheetTerrain.Snow_Road_ver:

                    case SpriteSheetTerrain.Desert_Road_turn_up_right:
                    case SpriteSheetTerrain.Desert_Road_turn_up_left:
                    case SpriteSheetTerrain.Desert_Road_Inter3_right:
                    case SpriteSheetTerrain.Desert_Road_Inter3_down:
                    case SpriteSheetTerrain.Desert_Road_hor:
                    case SpriteSheetTerrain.Desert_Road_Cross:
                    case SpriteSheetTerrain.Desert_Road_turn_down_right:
                    case SpriteSheetTerrain.Desert_Road_turn_down_left:
                    case SpriteSheetTerrain.Desert_Road_Inter3_up:
                    case SpriteSheetTerrain.Desert_Road_Inter3_left:
                    case SpriteSheetTerrain.Desert_Road_ver:
                        result = TerrainType.Road;
                        break;



                    case SpriteSheetTerrain.Plain:
                    case SpriteSheetTerrain.Tropical_Plain:
                    case SpriteSheetTerrain.Rain_Plain:
                    case SpriteSheetTerrain.Snow_Plain:
                    case SpriteSheetTerrain.Desert_Plain:
                        result = TerrainType.Plain;
                        break;


                    case SpriteSheetTerrain.Tree:
                    case SpriteSheetTerrain.Tree_top_left:
                    case SpriteSheetTerrain.Tree_top_right:
                    case SpriteSheetTerrain.Tree_bottom_left:
                    case SpriteSheetTerrain.Tree_bottom_right:
                    case SpriteSheetTerrain.Tree_up_left:
                    case SpriteSheetTerrain.Tree_up_middle:
                    case SpriteSheetTerrain.Tree_up_right:
                    case SpriteSheetTerrain.Tree_middle_left:
                    case SpriteSheetTerrain.Tree_middle_middle:
                    case SpriteSheetTerrain.Tree_middle_right:
                    case SpriteSheetTerrain.Tree_down_left:
                    case SpriteSheetTerrain.Tree_down_middle:
                    case SpriteSheetTerrain.Tree_down_right:

                    case SpriteSheetTerrain.Tropical_Tree:
                    case SpriteSheetTerrain.Tropical_Tree_top_left:
                    case SpriteSheetTerrain.Tropical_Tree_top_right:
                    case SpriteSheetTerrain.Tropical_Tree_bottom_left:
                    case SpriteSheetTerrain.Tropical_Tree_bottom_right:
                    case SpriteSheetTerrain.Tropical_Tree_up_left:
                    case SpriteSheetTerrain.Tropical_Tree_up_middle:
                    case SpriteSheetTerrain.Tropical_Tree_up_right:
                    case SpriteSheetTerrain.Tropical_Tree_middle_left:
                    case SpriteSheetTerrain.Tropical_Tree_middle_middle:
                    case SpriteSheetTerrain.Tropical_Tree_middle_right:
                    case SpriteSheetTerrain.Tropical_Tree_down_left:
                    case SpriteSheetTerrain.Tropical_Tree_down_middle:
                    case SpriteSheetTerrain.Tropical_Tree_down_right:

                    case SpriteSheetTerrain.Rain_Tree:
                    case SpriteSheetTerrain.Rain_Tree_top_left:
                    case SpriteSheetTerrain.Rain_Tree_top_right:
                    case SpriteSheetTerrain.Rain_Tree_bottom_left:
                    case SpriteSheetTerrain.Rain_Tree_bottom_right:
                    case SpriteSheetTerrain.Rain_Tree_up_left:
                    case SpriteSheetTerrain.Rain_Tree_up_middle:
                    case SpriteSheetTerrain.Rain_Tree_up_right:
                    case SpriteSheetTerrain.Rain_Tree_middle_left:
                    case SpriteSheetTerrain.Rain_Tree_middle_middle:
                    case SpriteSheetTerrain.Rain_Tree_middle_right:
                    case SpriteSheetTerrain.Rain_Tree_down_left:
                    case SpriteSheetTerrain.Rain_Tree_down_middle:
                    case SpriteSheetTerrain.Rain_Tree_down_right:

                    case SpriteSheetTerrain.Snow_Tree:
                    case SpriteSheetTerrain.Snow_Tree_top_left:
                    case SpriteSheetTerrain.Snow_Tree_top_right:
                    case SpriteSheetTerrain.Snow_Tree_bottom_left:
                    case SpriteSheetTerrain.Snow_Tree_bottom_right:
                    case SpriteSheetTerrain.Snow_Tree_up_left:
                    case SpriteSheetTerrain.Snow_Tree_up_middle:
                    case SpriteSheetTerrain.Snow_Tree_up_right:
                    case SpriteSheetTerrain.Snow_Tree_middle_left:
                    case SpriteSheetTerrain.Snow_Tree_middle_middle:
                    case SpriteSheetTerrain.Snow_Tree_middle_right:
                    case SpriteSheetTerrain.Snow_Tree_down_left:
                    case SpriteSheetTerrain.Snow_Tree_down_middle:
                    case SpriteSheetTerrain.Snow_Tree_down_right:

                    case SpriteSheetTerrain.Desert_Tree:
                        result = TerrainType.Tree;
                        break;

                    case SpriteSheetTerrain.Mountain_High_Upper:
                    case SpriteSheetTerrain.Mountain_High_Lower:
                    case SpriteSheetTerrain.Mountain_Low:

                    case SpriteSheetTerrain.Tropical_Mountain_High_Upper:
                    case SpriteSheetTerrain.Tropical_Mountain_High_Lower:
                    case SpriteSheetTerrain.Tropical_Mountain_Low:

                    case SpriteSheetTerrain.Rain_Mountain_High_Upper:
                    case SpriteSheetTerrain.Rain_Mountain_High_Lower:
                    case SpriteSheetTerrain.Rain_Mountain_Low:

                    case SpriteSheetTerrain.Snow_Mountain_High_Upper:
                    case SpriteSheetTerrain.Snow_Mountain_High_Lower:
                    case SpriteSheetTerrain.Snow_Mountain_Low:

                    case SpriteSheetTerrain.Desert_Mountain_High_Upper:
                    case SpriteSheetTerrain.Desert_Mountain_High_Lower:
                        result = TerrainType.Mountain;
                        break;


                    case SpriteSheetTerrain.City_Lower:
                    case SpriteSheetTerrain.Red_City_Lower:
                    case SpriteSheetTerrain.Blue_City_Lower:
                    case SpriteSheetTerrain.Green_City_Lower:
                    case SpriteSheetTerrain.Yellow_City_Lower:
                        result = TerrainType.City;
                        break;

                    case SpriteSheetTerrain.Factory:
                    case SpriteSheetTerrain.Red_Factory:
                    case SpriteSheetTerrain.Blue_Factory:
                    case SpriteSheetTerrain.Green_Factory:
                    case SpriteSheetTerrain.Yellow_Factory:
                        result = TerrainType.Factory;
                        break;

                    case SpriteSheetTerrain.AirPort_Lower:
                    case SpriteSheetTerrain.Red_AirPort_Lower:
                    case SpriteSheetTerrain.Blue_AirPort_Lower:
                    case SpriteSheetTerrain.Green_AirPort_Lower:
                    case SpriteSheetTerrain.Yellow_AirPort_Lower:
                        result = TerrainType.AirPort;
                        break;

                    case SpriteSheetTerrain.Harbor_Lower:
                    case SpriteSheetTerrain.Red_Harbor_Lower:
                    case SpriteSheetTerrain.Blue_Harbor_Lower:
                    case SpriteSheetTerrain.Green_Harbor_Lower:
                    case SpriteSheetTerrain.Yellow_Harbor_Lower:
                        result = TerrainType.Harbor;
                        break;

                    case SpriteSheetTerrain.Radar_Lower:
                    case SpriteSheetTerrain.Red_Radar_Lower:
                    case SpriteSheetTerrain.Blue_Radar_Lower:
                    case SpriteSheetTerrain.Green_Radar_Lower:
                    case SpriteSheetTerrain.Yellow_Radar_Lower:
                        result = TerrainType.Radar;
                        break;

                    case SpriteSheetTerrain.SupplyBase_Lower:
                    case SpriteSheetTerrain.Red_SupplyBase_Lower:
                    case SpriteSheetTerrain.Blue_SupplyBase_Lower:
                    case SpriteSheetTerrain.Green_SupplyBase_Lower:
                    case SpriteSheetTerrain.Yellow_SupplyBase_Lower:
                        result = TerrainType.SupplyBase;
                        break;

                    case SpriteSheetTerrain.Missile_Silo_Lower:
                        result = TerrainType.MissileSilo;
                        break;
                    case SpriteSheetTerrain.Missile_Silo_Launched:
                        result = TerrainType.MissileSiloLaunched;
                        break;

                    case SpriteSheetTerrain.Red_Headquarter_Lower:
                    case SpriteSheetTerrain.Blue_Headquarter_Lower:
                    case SpriteSheetTerrain.Green_Headquarter_Lower:
                    case SpriteSheetTerrain.Yellow_Headquarter_Lower:
                        result = TerrainType.HQ;
                        break;
                }

                return result;
            }

            public static T Next<T>(this T t, int count = 1)
            {
                var type = typeof(T);
                int result;
                T box = default(T);
                count.Clamp(int.MaxValue, 0);
                switch (type.Name)
                {
                    case "SpriteSheetTerrain":
                        SpriteSheetTerrain unboxSpriteSheetTerrain = (SpriteSheetTerrain)((object)t);
                        result = (int)unboxSpriteSheetTerrain + count;
                        box = (T)((object)result);
                        return box;
                    case "UnitType":
                        UnitType unboxUnit = (UnitType)((object)t);
                        result = (int)unboxUnit + count;
                        box = (T)((object)result);
                        break;
                    case "TerrainType":
                        TerrainType unboxTerrain = (TerrainType)((object)t);
                        result = (int)unboxTerrain + count;
                        box = (T)((object)result);
                        break;
                    case "SpriteSheetUnit":
                        SpriteSheetUnit unboxSpriteSheetUnit = (SpriteSheetUnit)((object)t);
                        result = (int)unboxSpriteSheetUnit + count;
                        box = (T)((object)result);
                        break;
                    case "AnimationName":
                        AnimationName unboxAnimationName = (AnimationName)((object)t);
                        result = (int)unboxAnimationName + count;
                        box = (T)((object)result);
                        break;
                    case "Owner":
                        Owner unboxOwner = (Owner)((object)t);
                        result = (int)unboxOwner + count;
                        box = (T)((object)result);
                        break;
                    case "SpriteSheetBuilding":
                        SpriteSheetBuilding unboxSpriteSheetBuilding = (SpriteSheetBuilding)((object)t);
                        result = (int)unboxSpriteSheetBuilding + count;
                        box = (T)((object)result);
                        break;
                    case "SpriteSheetCommand":
                        SpriteSheetCommand unboxSpriteSheetCommand = (SpriteSheetCommand)((object)t);
                        result = (int)unboxSpriteSheetCommand + count;
                        box = (T)((object)result);
                        break;
                    default:
                        break;
                }
                return box;
            }

            public static T Previous<T>(this T t, int count = 1)
            {
                var type = typeof(T);
                int result;
                T box = default(T);
                count.Clamp(0, int.MinValue);
                switch (type.Name)
                {
                    case "SpriteSheetTerrain":
                        SpriteSheetTerrain unboxSpriteSheetTerrain = (SpriteSheetTerrain)((object)t);
                        result = (int)unboxSpriteSheetTerrain - count;
                        box = (T)((object)result);
                        return box;
                    case "UnitType":
                        UnitType unboxUnit = (UnitType)((object)t);
                        result = (int)unboxUnit - count;
                        box = (T)((object)result);
                        break;
                    case "TerrainType":
                        TerrainType unboxTerrain = (TerrainType)((object)t);
                        result = (int)unboxTerrain - count;
                        box = (T)((object)result);
                        break;
                    case "SpriteSheetUnit":
                        SpriteSheetUnit unboxSpriteSheetUnit = (SpriteSheetUnit)((object)t);
                        result = (int)unboxSpriteSheetUnit - count;
                        box = (T)((object)result);
                        break;
                    case "AnimationName":
                        AnimationName unboxAnimationName = (AnimationName)((object)t);
                        result = (int)unboxAnimationName - count;
                        box = (T)((object)result);
                        break;
                    case "Owner":
                        Owner unboxOwner = (Owner)((object)t);
                        result = (int)unboxOwner - count;
                        box = (T)((object)result);
                        break;
                    case "SpriteSheetBuilding":
                        SpriteSheetBuilding unboxSpriteSheetBuilding = (SpriteSheetBuilding)((object)t);
                        result = (int)unboxSpriteSheetBuilding - count;
                        box = (T)((object)result);
                        break;
                    default:
                        return t;
                }
                return box;
            }

            public static Point GetNearbyPoint(this Point p, Direction d)
            {
                Point output = new Point(p.X, p.Y);
                switch (d)
                {
                    case Direction.NorthWest:
                        output = new Point(p.X - 1, p.Y - 1);
                        break;
                    case Direction.North:
                        output = new Point(p.X, p.Y - 1);
                        break;
                    case Direction.NorthEast:
                        output = new Point(p.X + 1, p.Y - 1);
                        break;
                    case Direction.West:
                        output = new Point(p.X - 1, p.Y);
                        break;
                    case Direction.East:
                        output = new Point(p.X + 1, p.Y);
                        break;
                    case Direction.SouthWest:
                        output = new Point(p.X - 1, p.Y + 1);
                        break;
                    case Direction.South:
                        output = new Point(p.X, p.Y + 1);
                        break;
                    case Direction.SouthEast:
                        output = new Point(p.X + 1, p.Y + 1);
                        break;
                    default:
                        break;
                }
                return output;
            }

            public static string toString(this Point p)
            {
                return string.Format("{0}:{1}", p.X, p.Y);
            }

            public static Point Parse(this string str)
            {
                var data = str.Split(':');
                int x, y;

                if (int.TryParse(data[0], out x) && int.TryParse(data[1], out y))
                {
                    return new Point(x, y);
                }
                else
                {
                    return Point.Zero;
                }
            }

            public static bool TryParse(this string str,out Point p)
            {
                var data = str.Split(':');
                int x, y;
                bool result = false;

                if (int.TryParse(data[0], out x) && int.TryParse(data[1], out y))
                {
                    p = new Point(x, y);
                    return true;
                }
                else
                {
                    p = Point.Zero;
                    return result;
                }
            }

            public static Direction GetDirectionFromPointAtoPointB(this Point pA,Point pB)
            {
                int deltaX = pA.X - pB.X;
                int deltaY = pA.Y - pB.Y;

                bool isLeft = false;
                bool isRight = false;
                bool isUp = false;
                bool isDown = false;

                if (deltaX>0)
                {
                    isLeft = true;
                }
                else
                {
                    if (deltaX < 0)
                    {
                        isRight = true;
                    }
                }

                if (deltaY>0)
                {
                    isUp = true;
                }
                else
                {
                    if (deltaY < 0)
                    {
                        isDown = true;
                    }
                }

                if (isLeft && isUp)
                {
                    return Direction.NorthWest;
                }

                if (isRight && isUp)
                {
                    return Direction.NorthEast;
                }

                if (isLeft && isDown)
                {
                    return Direction.SouthWest;
                }

                if (isRight && isDown)
                {
                    return Direction.SouthEast;
                }

                if (isLeft)
                {
                    return Direction.West;
                }

                if (isRight)
                {
                    return Direction.East;
                }

                if (isUp)
                {
                    return Direction.North;
                }

                if (isDown)
                {
                    return Direction.South;
                }

                return Direction.Center;
            }

            public static bool DistanceToOtherLessThan(this Point p,Point other,float MaxDistance)
            {
                return ((p.X - other.X) * (p.X - other.X) + (p.Y - other.Y) * (p.Y - other.Y)) < MaxDistance * MaxDistance;
            }
        }
    }
}