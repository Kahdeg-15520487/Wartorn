using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wartorn.GameData;

namespace Wartorn
{
    class SpriteSheetSourceRectangle
    {
        public static readonly Rectangle
            AirPort = new Rectangle(0, 0, 60, 60),
            City = new Rectangle(60, 0, 60, 60),
            Factory = new Rectangle(120, 0, 60, 60),
            Plain = new Rectangle(180, 0, 60, 60);
        public static Rectangle GetSpriteRectangle(Terrain t)
        {
            Rectangle result = Rectangle.Empty;
            switch (t)
            {
                case GameData.Terrain.Plain:
                    result = Plain;
                    break;
                case GameData.Terrain.Dessert:
                    break;
                case GameData.Terrain.Forest:
                    break;
                case GameData.Terrain.Mountain:
                    break;
                case GameData.Terrain.River:
                    break;
                case GameData.Terrain.Coast:
                    break;
                case GameData.Terrain.Road:
                    break;
                case GameData.Terrain.Ocean:
                    break;
                case GameData.Terrain.Reef:
                    break;
                case GameData.Terrain.Waterfall:
                    break;
                case GameData.Terrain.Cliff:
                    break;
                case GameData.Terrain.Tropical_Plain:
                    break;
                case GameData.Terrain.Bridge:
                    break;
                case GameData.Terrain.Ruin:
                    break;
                case GameData.Terrain.Barricade:
                    break;
                case GameData.Terrain.Bunker:
                    break;
                case GameData.Terrain.Turret:
                    break;
                case GameData.Terrain.City:
                    result = City;
                    break;
                case GameData.Terrain.Factory:
                    result = Factory;
                    break;
                case GameData.Terrain.AirPort:
                    result = AirPort;
                    break;
                case GameData.Terrain.Supply_Base:
                    break;
                case GameData.Terrain.Town:
                    break;
                case GameData.Terrain.Missle_Silo:
                    break;
                case GameData.Terrain.Navy_Fort:
                    break;
                case GameData.Terrain.Control_Point:
                    break;
                case GameData.Terrain.Radar_Station:
                    break;
                default:
                    break;

            }
            return result;
        }
    }
}