using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wartorn.GameData {
	public enum TerrainType {
		//terrain
		Reef,
		Sea,
		River,
		Coast,
		Cliff,
		Road,
		Bridge,
		Plain,
		Forest,
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

	public enum UnitType {
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
		Cruiser,
		Submarine,
		Battleship
	}

	public enum MovementType {
		None,
		Soldier,
		Mech,
		Tires,
		Track,
		Air,
		Ship,
		Lander
	}

	public enum Weather {
		Sunny,
		Rain,
		Snow
	}

	public enum Theme {
		Normal,
		Tropical,
		Desert
	}

	public enum Owner {
		None,
		Red,
		Blue,
		Green,
		Yellow
	}

	public enum GameMode {
		campaign
	}

	[Flags]
	public enum Command {
		None = 0,
		Wait = 1 << 0,
		Attack = 1 << 1,
		Capture = 1 << 2,
		Load = 1 << 3,
		Drop = 1 << 4,
		Rise = 1 << 5,
		Dive = 1 << 6,
		Supply = 1 << 7,
		Move = 1 << 8,
		Operate = 1 << 9
	}
}
