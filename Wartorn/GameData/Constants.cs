using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wartorn.GameData
{
    public enum TerrainType
    {
        //terrain
        Reef,
        Sea,
        River,
        Coast,
        Cliff,
        Road,
        Bridge,
        Plain,
        Tree,
        Mountain,

        //neutral
        MissileSilo,
        MissileSiloLaunched,

        //building
        City,
        Factory,
        AirPort,
        Harbor,
        Radar,
        SupplyBase,
        Headquarter
    }



    public enum UnitType
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

    public enum Weather
    {
        Sunny,
        Rain,
        Snow
    }

    public enum Theme
    {
        Normal,
        Tropical,
        Desert
    }

    public enum Owner
    {
        None,
        Red,
        Blue,
        Green,
        Yellow
    }

    public enum GameMode
    {
        campaign
    }    
}
