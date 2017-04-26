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
        Reef,
        Sea,
        River,
        Coast,
        Cliff,
        Road,
        Tree,
        Mountain,

        //neutral
        MissileSilo,

        //building
        City,
        Factory,
        AirPort,
        Harbor,
        Radar,
        SupplyBase,
        Headquarter
    }


    enum Unit
    {
        None,
        Soldier,
        Mech,
        Recon,
        APC,
        Tank,
        H_Tank,
        Artillery,
        Rocket,
        Anti_Air,
        Missile
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
