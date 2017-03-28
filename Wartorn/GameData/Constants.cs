using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wartorn.GameData
{
    enum Terrain
    {
        //terrain
        Plain,
        Dessert,
        Forest,
        Mountain,
        River,
        Coast,
        Road,
        Ocean,
        Reef,
        Waterfall,
        Cliff,
        Tropical_Plain,

        //neutral
        Bridge,
        Ruin,
        Barricade,
        Bunker,
        Turret,

        //capturable building
        City,
        Factory,
        AirPort,
        Supply_Base,
        Town,
        Missle_Silo,
        Navy_Fort,
        Control_Point,
        Radar_Station
    }


    enum Unit
    {
        soldier,
        tank
    }

    enum Weather
    {
        normal,
        rain,
        snow,
        sandstorm
    }

    enum GameMode
    {
        campaign
    }
}
