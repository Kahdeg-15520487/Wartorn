﻿using System;
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

namespace Wartorn.GameData
{
    public class Unit
    {
        #region static stuff
        static Dictionary<UnitPair, int> _DammageTable;
        static Dictionary<UnitType, int> _MovementRange;
        static Dictionary<UnitType, int> _VisionRange;
        static Dictionary<MovementType, Dictionary<TerrainType, int>> _TravelCost;

        public static void Init()
        {
            List<UnitType> unittypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().ToList();
            unittypes.Remove(UnitType.None);

            List<MovementType> movementtypes = Enum.GetValues(typeof(MovementType)).Cast<MovementType>().ToList();
            movementtypes.Remove(MovementType.None);

            List<TerrainType> terraintypes = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToList();

            _DammageTable = new Dictionary<UnitPair, int>();
            foreach (UnitType attacker in unittypes)
            {
                foreach (UnitType defender in unittypes)
                {
                    _DammageTable.Add(new UnitPair(attacker, defender), 0);
                }
            }

            _MovementRange = new Dictionary<UnitType, int>();
            foreach (UnitType unittype in unittypes)
            {
                _MovementRange.Add(unittype, 0);

            }

            _VisionRange = new Dictionary<UnitType, int>();
            foreach (UnitType unittype in unittypes)
            {
                _VisionRange.Add(unittype, 0);
            }

            _TravelCost = new Dictionary<MovementType, Dictionary<TerrainType, int>>();
            foreach (MovementType movetype in movementtypes)
            {
                _TravelCost.Add(movetype, new Dictionary<TerrainType, int>());
                foreach (TerrainType terraintype in terraintypes)
                {
                    _TravelCost[movetype].Add(terraintype, int.MaxValue);
                }
            }

            Directory.CreateDirectory(@"data/");
            File.WriteAllText(@"data/dmgtable.txt", JsonConvert.SerializeObject(_DammageTable.ToArray(), Formatting.Indented));
            File.WriteAllText(@"data/movementrangetable.txt", JsonConvert.SerializeObject(_MovementRange.ToArray(), Formatting.Indented));
            File.WriteAllText(@"data/visionrangetable.txt", JsonConvert.SerializeObject(_VisionRange.ToArray(), Formatting.Indented));

            File.WriteAllText(@"data/traversecosttable.txt", JsonConvert.SerializeObject(_TravelCost, Formatting.Indented));
        }

        public static void Load()
        {
            _DammageTable = new Dictionary<UnitPair, int>();
            _MovementRange = new Dictionary<UnitType, int>();
            _VisionRange = new Dictionary<UnitType, int>();
            _TravelCost = new Dictionary<MovementType, Dictionary<TerrainType, int>>();
            
            string dmgtable = File.ReadAllText(@"data/dmgtable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<UnitPair, int>[]>(dmgtable).ToList().ForEach(kvp =>
            {
                _DammageTable.Add(kvp.Key, kvp.Value);
            });

            string movrangetable = File.ReadAllText(@"data/movementrangetable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(movrangetable).ToList().ForEach(kvp =>
            {
                _MovementRange.Add(kvp.Key, kvp.Value);
            });

            string visionrangetable = File.ReadAllText(@"data/visionrangetable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(visionrangetable).ToList().ForEach(kvp =>
            {
                _VisionRange.Add(kvp.Key, kvp.Value);
            });

            string traversecosttable = File.ReadAllText(@"data/traversecosttable.txt");
            _TravelCost = JsonConvert.DeserializeObject<Dictionary<MovementType, Dictionary<TerrainType, int>>>(traversecosttable);
        }

        /// <summary>
        /// 0 : normal travel,
        /// 1 : troubled travel,
        /// horizontal 8 : can not travel
        /// </summary>
        /// <param name="unitType"></param>
        /// <param name="TerrainType"></param>
        /// <returns></returns>
        public static int GetTravelCost(UnitType unitType,TerrainType terrainType)
        {
            MovementType movementType = unitType.GetMovementType();
            return _TravelCost[movementType][terrainType];
        }
        #endregion

        #region private field
        AnimatedEntity animation;
        UnitType unitType;
        int hitPoint;
        int fuel;
        #endregion

        #region public
        public AnimatedEntity Animation { get { return animation; } }
        public UnitType UnitType { get { return unitType; } }
        public int HitPoint { get { return hitPoint; } }
        public int Fuel { get { return fuel; } }
        public Owner Owner { get; set; }
        public int UnitID { get; set; }
        
        public Unit(UnitType unittype, AnimatedEntity anim,Owner owner,int hp = 100)
        {
            unitType = unittype;
            animation = anim;
            Owner = owner;
            hitPoint = hp;
            fuel = 100;
        }

        public int ReceiveDammage(UnitType other)
        {
            return hitPoint = _DammageTable[new UnitPair(other, unitType)];
        }
        #endregion
    }

    
    public class UnitPair
    {
        public UnitType Attacker { get; set; }
        public UnitType Defender { get; set; }
        public UnitPair(UnitType att, UnitType def)
        {
            Attacker = att;
            Defender = def;
        }
    }

    public static class UnitCreationHelper
    {
        public static Unit Create(UnitType unittype,Owner owner,int hp = 100,AnimationName animation = AnimationName.idle)
        {
            var result = new Unit(unittype, (AnimatedEntity)CONTENT_MANAGER.animationEntities[unittype.GetSpriteSheetUnit(owner)].Clone(), owner, hp);
            result.Animation.PlayAnimation(animation.ToString());
            return result;
        }
    }

    #region JsonConverter class

    
    #endregion
}
