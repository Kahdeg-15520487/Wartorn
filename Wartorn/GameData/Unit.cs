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

namespace Wartorn.GameData
{
    public class Unit
    {
        #region static stuff
        static Dictionary<UnitPair, int> _DammageTable;
        static Dictionary<UnitType, int> _MovementRange;
        static Dictionary<UnitType, int> _VisionRange;
        Dictionary<MovementType, Dictionary<TerrainType, int>> _TravelCost;

        public static void Init()
        {
            List<UnitType> unittypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().ToList();
            unittypes.Remove(UnitType.None);
            

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

            File.WriteAllText("dmgtable.txt", JsonConvert.SerializeObject(_DammageTable.ToArray(), Formatting.Indented));
            File.WriteAllText("movementrangetable.txt", JsonConvert.SerializeObject(_MovementRange.ToArray(), Formatting.Indented));
            File.WriteAllText("visionrangetable.txt", JsonConvert.SerializeObject(_VisionRange.ToArray(), Formatting.Indented));
        }

        public static void Load()
        {
            string dmgtable = File.ReadAllText("dmgtable.txt");
            _DammageTable = JsonConvert.DeserializeObject<KeyValuePair<UnitPair, int>[]>(dmgtable).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            string movrangetable = File.ReadAllText("movementrangetable.txt");
            _MovementRange = JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(movrangetable).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            string visionrangetable = File.ReadAllText("visionrangetable.txt");
            _VisionRange = JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(visionrangetable).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// 0 : normal travel,
        /// 1 : troubled travel,
        /// 2 : cant travel
        /// </summary>
        /// <param name="unitType"></param>
        /// <param name="TerrainType"></param>
        /// <returns></returns>
        public static int CanTravel(UnitType unitType,TerrainType TerrainType)
        {
            MovementType movementType = unitType.GetMovementType();
            int result = 0;
            switch (TerrainType)
            {
                case TerrainType.Reef:
                    break;
                case TerrainType.Sea:
                    break;

                case TerrainType.River:
                    if (movementType == MovementType.Foot)
                    {
                        result = 1;
                    }

                    break;

                case TerrainType.Coast:
                    result = 0;
                    break;

                case TerrainType.Cliff:
                    result = 2;
                    break;

                case TerrainType.Road:
                case TerrainType.Bridge:
                    result = 0;
                    break;

                case TerrainType.Plain:
                    if (movementType == MovementType.Foot || movementType == MovementType.Track)
                    {
                        result = 0;
                    }
                    else
                    {
                        result = 1;
                    }
                    break;

                case TerrainType.Tree:
                    if (movementType == MovementType.Foot)
                    {
                        result = 0;
                    }
                    else
                    {
                        result = 1;
                    }
                    break;

                case TerrainType.Mountain:
                    if (movementType == MovementType.Foot)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = 2;
                    }
                    break;

                case TerrainType.MissileSilo:
                case TerrainType.MissileSiloLaunched:
                case TerrainType.City:
                case TerrainType.Factory:
                case TerrainType.AirPort:
                case TerrainType.Harbor:
                case TerrainType.Radar:
                case TerrainType.SupplyBase:
                case TerrainType.HQ:
                    result = 0;
                    break;
                default:
                    break;
            }

            return result;
        }

        public static int CalculateMovementCost(UnitType unitType,TerrainType terrainType)
        {
            int result = 1;
            switch (unitType)
            {
                //move by foot
                case UnitType.Soldier:
                    break;
                case UnitType.Mech:
                    break;

                // move by tires
                case UnitType.Recon:
                    break;
                case UnitType.Rocket:
                    break;
                case UnitType.Missile:
                    break;
                
                //move by treads
                case UnitType.APC:
                    break;
                case UnitType.Tank:
                    break;
                case UnitType.HeavyTank:
                    break;
                case UnitType.Artillery:
                    break;
                case UnitType.AntiAir:
                    break;
                default:
                    break;
            }
            return result;
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

    public class UnitJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Unit);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Unit temp = (Unit)value;
            writer.WriteStartObject();
            writer.WritePropertyName("UnitType");
            serializer.Serialize(writer, temp.UnitType.ToString());
            writer.WritePropertyName("Owner");
            serializer.Serialize(writer, temp.Owner.ToString());
            writer.WritePropertyName("HP");
            serializer.Serialize(writer, temp.HitPoint);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            UnitType unittype = UnitType.None;
            Owner owner = Owner.None;
            int hp = 0;

            bool gotUnittype = false;
            bool gotOwner = false;
            bool gotHp = false;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                string propertyName = (string)reader.Value;
                if (!reader.Read())
                {
                    continue;
                }
                switch (propertyName)
                {
                    case "UnitType":
                        unittype = (serializer.Deserialize<string>(reader)).ToEnum<UnitType>();
                        gotUnittype = true;
                        break;
                    case "Owner":
                        owner = (serializer.Deserialize<string>(reader)).ToEnum<Owner>();
                        gotOwner = true;
                        break;
                    case "HP":
                        hp = serializer.Deserialize<int>(reader);
                        gotHp = true;
                        break;
                    default:
                        break;
                }
            }

            if (!(gotUnittype && gotHp && gotOwner))
            {
                //throw new InvalidDataException("Not enought data");
                return null;
            }

            return UnitCreationHelper.Create(unittype, owner, hp);
        }
    }

    public class UnitTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnitType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            UnitType temp = (UnitType)value;
            writer.WriteStartObject();
            writer.WritePropertyName("value");
            serializer.Serialize(writer, temp.ToString());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            UnitType result = UnitType.None;
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                string propertyName = (string)reader.Value;
                if (!reader.Read())
                {
                    continue;
                }

                if (propertyName == "")
                {
                    result = (serializer.Deserialize<string>(reader)).ToEnum<UnitType>();
                }
            }

            return result;
        }
    }

    public class UnitPairJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnitPair);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            UnitPair temp = (UnitPair)value;

            writer.WriteStartObject();
            writer.WritePropertyName("Attacker");
            serializer.Serialize(writer, temp.Attacker.ToString());
            writer.WritePropertyName("Defender");
            serializer.Serialize(writer, temp.Defender.ToString());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            UnitType attacker = default(UnitType);
            UnitType defender = default(UnitType);

            bool gotAttacker = false;
            bool gotDefender = false;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                string propertyName = (string)reader.Value;
                if (!reader.Read())
                {
                    continue;
                }

                if (propertyName == "Attacker")
                {
                    attacker = (serializer.Deserialize<string>(reader)).ToEnum<UnitType>();
                    gotAttacker = true;
                }

                if (propertyName == "Defender")
                {
                    defender = (serializer.Deserialize<string>(reader)).ToEnum<UnitType>();
                    gotDefender = true;
                }
            }

            if (!(gotAttacker && gotDefender))
            {
                throw new InvalidDataException("Not enought data");
            }

            return new UnitPair(attacker, defender);
        }
    }
    #endregion
}
