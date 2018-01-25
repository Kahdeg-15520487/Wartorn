using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Utility;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wartorn.GameData {
	public struct UnitStat {
		public int Cost { get; set; }
		public int Gas { get; set; }
		public int Ammo { get; set; }
		public int ActionPoint { get; set; }
		public int MovementRange { get; set; }
		public int VisionRange { get; set; }
		public Range AttackRange { get; set; }
		public MovementType MovementType { get; set; }
	}

	public class Unit {
		#region unit stat

		public static Dictionary<UnitType, Dictionary<UnitType, int>> _DammageTable { get; private set; }
		public static Dictionary<UnitType, UnitStat> _UnitStat { get; private set; }
		public static Dictionary<MovementType, Dictionary<TerrainType, int>> _TravelCost { get; private set; }
		public static Dictionary<TerrainType, int> _DefenseStar { get; private set; }

		public static void Init() {
			List<UnitType> unittypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().ToList();
			unittypes.Remove(UnitType.None);

			List<MovementType> movementtypes = Enum.GetValues(typeof(MovementType)).Cast<MovementType>().ToList();
			movementtypes.Remove(MovementType.None);

			List<TerrainType> terraintypes = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToList();

			_DammageTable = new Dictionary<UnitType, Dictionary<UnitType, int>>();
			foreach (UnitType attacker in unittypes) {
				_DammageTable.Add(attacker, new Dictionary<UnitType, int>());
				foreach (UnitType defender in unittypes) {
					_DammageTable[attacker].Add(defender, 0);
				}
			}

			_UnitStat = new Dictionary<UnitType, UnitStat>();
			foreach (UnitType ut in unittypes) {
				_UnitStat.Add(ut, new UnitStat());
			}

			_TravelCost = new Dictionary<MovementType, Dictionary<TerrainType, int>>();
			foreach (MovementType movetype in movementtypes) {
				_TravelCost.Add(movetype, new Dictionary<TerrainType, int>());
				foreach (TerrainType terraintype in terraintypes) {
					_TravelCost[movetype].Add(terraintype, int.MaxValue);
				}
			}

			_DefenseStar = new Dictionary<TerrainType, int>();
			foreach (TerrainType terraintype in terraintypes) {
				_DefenseStar.Add(terraintype, 0);
			}

			Directory.CreateDirectory(@"data\");
			//File.WriteAllText(@"data\unitstat.txt", JsonConvert.SerializeObject(_UnitStat));
			//File.WriteAllText(@"data\dmgtable.txt", JsonConvert.SerializeObject(_DammageTable, Formatting.Indented));
			//File.WriteAllText(@"data\costtable.txt", JsonConvert.SerializeObject(_Cost.ToArray(), Formatting.Indented));
			//File.WriteAllText(@"data\gastable.txt", JsonConvert.SerializeObject(_Gas.ToArray(), Formatting.Indented));
			//File.WriteAllText(@"data\ammotable.txt", JsonConvert.SerializeObject(_Ammo.ToArray(), Formatting.Indented));
			//File.WriteAllText(@"data\aptable.txt", JsonConvert.SerializeObject(_ActionPoint.ToArray(), Formatting.Indented));
			//File.WriteAllText(@"data\movementrangetable.txt", JsonConvert.SerializeObject(_MovementRange.ToArray(), Formatting.Indented));
			//File.WriteAllText(@"data\visionrangetable.txt", JsonConvert.SerializeObject(_VisionRange.ToArray(), Formatting.Indented));
			//File.WriteAllText(@"data\attackrangetable.txt", JsonConvert.SerializeObject(_AttackRange.ToArray(), Formatting.Indented));
			//File.WriteAllText(@"data\traversecosttable.txt", JsonConvert.SerializeObject(_TravelCost, Formatting.Indented));
			//File.WriteAllText(@"data\defensestartable.txt", JsonConvert.SerializeObject(_DefenseStar.ToArray(), Formatting.Indented));
		}

		public static void Load() {
			_DammageTable = new Dictionary<UnitType, Dictionary<UnitType, int>>();
			_UnitStat = new Dictionary<UnitType, UnitStat>();
			_TravelCost = new Dictionary<MovementType, Dictionary<TerrainType, int>>();
			_DefenseStar = new Dictionary<TerrainType, int>();

			string dmgtable = File.ReadAllText(@"data\dmgtable.txt");
			_DammageTable = JsonConvert.DeserializeObject<Dictionary<UnitType, Dictionary<UnitType, int>>>(dmgtable);

			string unitstat = File.ReadAllText(@"data\unitstat.txt");
			_UnitStat = JsonConvert.DeserializeObject<Dictionary<UnitType, UnitStat>>(unitstat);

			string traversecosttable = File.ReadAllText(@"data\traversecosttable.txt");
			_TravelCost = JsonConvert.DeserializeObject<Dictionary<MovementType, Dictionary<TerrainType, int>>>(traversecosttable);

			string defensestartable = File.ReadAllText(@"data\defensestartable.txt");
			JsonConvert.DeserializeObject<KeyValuePair<TerrainType, int>[]>(defensestartable).ToList().ForEach(kvp => {
				_DefenseStar.Add(kvp.Key, kvp.Value);
			});
		}

		/// <summary>
		/// 0 : normal travel,
		/// 1 : troubled travel,
		/// horizontal 8 : can not travel
		/// </summary>
		/// <param name="unitType"></param>
		/// <param name="TerrainType"></param>
		/// <returns></returns>
		public static int GetTravelCost(UnitType unitType, TerrainType terrainType) {
			MovementType movementType = unitType.GetMovementType();
			return _TravelCost[movementType][terrainType];
		}

		/// <summary>
		/// get the base damage that the defender will receive in ideal condition
		/// </summary>
		/// <param name="attacker">the attacking unit</param>
		/// <param name="defender">the defending unit</param>
		/// <returns></returns>
		public static int GetBaseDamage(UnitType attacker, UnitType defender) {
			return _DammageTable[attacker][defender];
		}

		public struct CalculatedDamage {
			public int attackerHP;
			public int defenderHP;
			public float damage;
			public float counterDamage;
			public CalculatedDamage(int atkhp, int defhp, float dmg, float counterdmg) {
				attackerHP = atkhp;
				defenderHP = defhp;
				damage = dmg;
				counterDamage = counterdmg;
			}

			public override string ToString() {
				return string.Format("{4} damage {0}{4} counterdamage: {1}{4} attackerHP: {2}{4} defenderHP: {3}", damage, counterDamage, attackerHP, defenderHP, Environment.NewLine);
			}
		}

		//todo: fix this shit
		/// <summary>
		/// calculate the going damage and counter damage between 2 unit on 2 mapcell
		/// and return resulting hitpoint after the damage exchange
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="defender"></param>
		/// <returns></returns>
		public static CalculatedDamage GetCalculatedDamage(MapCell attacker, MapCell defender) {
			//damage = base damage * (HP/100)*(- Terrain Bonus)
			//Damage = Current HP * Base Damage * (1 - Terrain star / 10)
			//hitpoint = hitpoint - Math.Floor(damage/10)

			int atkHP = attacker.unit.HitPoint;
			int defHP = defender.unit.HitPoint;

			float damage;
			float counterDamage;

			//calculate going damage
			damage = atkHP * GetBaseDamage(attacker.unit.UnitType, defender.unit.UnitType) / 100f * (1 - _DefenseStar[defender.terrain] / 10f);
			damage = (float)Math.Round(damage, MidpointRounding.AwayFromZero);
			defHP -= (int)damage;

			//check if the defender is already dead
			if (defHP <= 0) {
				return new CalculatedDamage(atkHP, 0, damage, 0);
			}

			if ((attacker.unit.unitType.isRangedUnit() && defender.unit.unitType.isRangedUnit())
			 || (!attacker.unit.unitType.isRangedUnit() && !defender.unit.unitType.isRangedUnit())) {

				//calculate counter damage
				counterDamage = defHP * GetBaseDamage(defender.unit.UnitType, attacker.unit.UnitType) / 100f * (1 - _DefenseStar[attacker.terrain] / 10f);
				counterDamage = (float)Math.Round(counterDamage, MidpointRounding.AwayFromZero);
				atkHP -= (int)counterDamage;

				return new CalculatedDamage(atkHP, defHP, damage, counterDamage);
			}
			else {
				return new CalculatedDamage(atkHP, defHP, damage, 0);
			}
		}
		#endregion

		#region private field
		AnimatedEntity animation;
		UnitType unitType;
		int hitPoint;
		int actionpoint;
		int gas;
		int ammo;
		#endregion

		#region property
		public AnimatedEntity Animation { get { return animation; } }
		public UnitType UnitType { get { return unitType; } }
		public int HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public int ActionPoint { get { return actionpoint; } }
		public int Gas { get { return gas; } set { gas = value; } }
		public int Ammo { get { return ammo; } set { ammo = value; } }
		public int CapturePoint { get; set; }
		public Owner Owner { get; set; }
		public readonly Guid guid;

		#endregion

		#region submarine dive
		private bool isdiving;
		public bool isDiving {
			get {
				return unitType == UnitType.Submarine && isdiving;
			}
		}
		#endregion

		public Unit(UnitType unittype, AnimatedEntity anim, Owner owner, int hp = 10) {
			unitType = unittype;
			animation = anim;
			Owner = owner;
			hitPoint = hp;
			gas = _UnitStat[unitType].Gas;
			ammo = _UnitStat[unitType].Ammo;
			actionpoint = _UnitStat[unitType].ActionPoint;
			guid = Guid.NewGuid();
		}

		public string GetUnitName() {
			return unitType.GetName();
		}


		public Range GetAttackkRange() {
			return _UnitStat[unitType].AttackRange;
		}

		public void Resupply() {
			gas = _UnitStat[unitType].Gas;
			ammo = _UnitStat[unitType].Ammo;
		}

		public void Repair() {
			hitPoint = (hitPoint + 2).Clamp(10, 0);
		}

		/// <summary>
		/// peak what the actionpoint will be after taking an action but not update the actionpoint
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public int PeakUpdateActionPoint(Command cmd) {
			int result;
			int temp = actionpoint;
			UpdateActionPoint(cmd);
			result = actionpoint;
			actionpoint = temp;
			return result;
		}

		/// <summary>
		/// update the actionpoint after taking an action
		/// </summary>
		/// <param name="cmd"></param>
		public void UpdateActionPoint(Command cmd) {
			switch (cmd) {
				case Command.None:
					actionpoint = _UnitStat[unitType].ActionPoint;
					break;

				case Command.Wait:
					actionpoint = 0;
					break;
				case Command.Move:
					if (unitType == UnitType.Artillery
					 || unitType == UnitType.Rocket
					 || unitType == UnitType.Missile
					 || unitType == UnitType.Battleship) {
						actionpoint = 0;
						break;
					}
					actionpoint -= 1;
					break;
				case Command.Attack:
					actionpoint = 0;
					break;
				case Command.Capture:
					actionpoint = 0;
					break;
				case Command.Dive:
					actionpoint = 0;
					isdiving = true;
					break;
				case Command.Rise:
					actionpoint = 0;
					isdiving = false;
					break;
				case Command.Load:
					break;
				case Command.Drop:
					break;
				case Command.Supply:
					actionpoint = 0;
					break;
				case Command.Operate:
					actionpoint = 0;
					break;
				default:
					break;
			}
		}
	}


	public struct UnitPair {
		public UnitType Attacker { get; set; }
		public UnitType Defender { get; set; }
		public UnitPair(UnitType att, UnitType def) {
			Attacker = att;
			Defender = def;
		}
	}

	public struct Range {
		public int Max { get; set; }
		public int Min { get; set; }
		public Range(int max, int min) {
			Max = max;
			Min = min;
		}
		public Range(int max) {
			Max = max;
			Min = max;
		}

		public bool IsInRange(int value) {
			return value <= Max && value >= Min;
		}

		public override string ToString() {
			return string.Format("{0}->{1}", Min, Max);
		}
	}

	public static class UnitCreationHelper {
		public static Unit Create(UnitType unittype, Owner owner, int hp = 10, AnimationName startingAnimation = AnimationName.idle) {
			AnimatedEntity animEntity = new AnimatedEntity(Vector2.Zero, Vector2.Zero, Color.White, LayerDepth.Unit);
			animEntity.LoadContent(CONTENT_MANAGER.animationSheets[unittype.GetSpriteSheetUnit(owner)]);

			#region declare animation frame
			Animation idle;
			Animation right;
			Animation up;
			Animation down;
			Animation done;
			switch (unittype) {
				case UnitType.TransportCopter:
				case UnitType.BattleCopter:
					animEntity.Origin = new Vector2(8, 16);
					idle = new Animation("idle", true, 3, string.Empty);
					for (int i = 0; i < 3; i++) {
						idle.AddKeyFrame(i * 64, 0, 64, 64);
					}

					right = new Animation("right", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						right.AddKeyFrame(i * 64, 64, 64, 64);
					}

					up = new Animation("up", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						up.AddKeyFrame(i * 64, 128, 64, 64);
					}

					down = new Animation("down", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						down.AddKeyFrame(i * 64, 192, 64, 64);
					}

					done = new Animation("done", true, 1, string.Empty);
					done.AddKeyFrame(0, 256, 64, 64);
					break;

				case UnitType.Fighter:
				case UnitType.Bomber:
					animEntity.Origin = new Vector2(8, 16);

					idle = new Animation("idle", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						idle.AddKeyFrame(i * 64, 0, 64, 64);
					}

					right = new Animation("right", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						right.AddKeyFrame(i * 64, 64, 64, 64);
					}

					up = new Animation("up", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						up.AddKeyFrame(i * 64, 128, 64, 64);
					}

					down = new Animation("down", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						down.AddKeyFrame(i * 64, 192, 64, 64);
					}

					done = new Animation("done", true, 1, string.Empty);
					done.AddKeyFrame(0, 256, 64, 64);
					break;
				default:
					idle = new Animation("idle", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						idle.AddKeyFrame(i * 48, 0, 48, 48);
					}

					right = new Animation("right", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						right.AddKeyFrame(i * 48, 48, 48, 48);
					}

					up = new Animation("up", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						up.AddKeyFrame(i * 48, 96, 48, 48);
					}

					down = new Animation("down", true, 4, string.Empty);
					for (int i = 0; i < 4; i++) {
						down.AddKeyFrame(i * 48, 144, 48, 48);
					}

					done = new Animation("done", true, 1, string.Empty);
					done.AddKeyFrame(0, 192, 48, 48);
					break;
			}
			#endregion

			animEntity.AddAnimation(idle, right, up, down, done);

			var result = new Unit(unittype, animEntity, owner, hp);
			result.Animation.PlayAnimation(startingAnimation.ToString());
			return result;
		}
	}
}
