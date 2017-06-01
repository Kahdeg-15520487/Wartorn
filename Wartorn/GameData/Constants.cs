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
        HQ
    }
    
    public enum UnitType
    {
        None,

        //land
        Soldier,
        Mech,
        Recon,
        APC,
        Tank,
        HeavyTank,
        Artillery,
        Rocket,
        AntiAir,
        Missile,

        //air
        TransportCopter,
        BattleCopter,
        Fighter,
        Bomber,

        //sea
        Lander,
        Cruise,
        Submarine,
        Battleship
    }

    public enum MovementType
    {
        None,
        Soldier,
        Mech,
        Tires,
        Track,
        Air,
        Ship,
        Lander
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

    [Flags]
    public enum Command
    {
        None = 0,
        Wait = 1,
        Attack = 2,
        Capture = 4,
        Load = 8,
        Drop = 16,
        Rise = 32,
        Dive = 64,
        Supply = 128,
        Move = 256
    }
}
