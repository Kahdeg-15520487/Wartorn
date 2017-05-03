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

        public static void Init()
        {
            _DammageTable = new Dictionary<UnitPair, int>();
            foreach (UnitType attacker in Enum.GetValues(typeof(UnitType)))
            {
                foreach (UnitType defender in Enum.GetValues(typeof(UnitType)))
                {
                    if (attacker != UnitType.None && defender != UnitType.None)
                    {
                        _DammageTable.Add(new UnitPair(attacker, defender), 0);
                    }
                }
            }

            //File.WriteAllText("dmgtable.txt", JsonConvert.SerializeObject(_DammageTable.ToArray(), Formatting.Indented));
        }

        public static void Load()
        {
            string dmgtable = File.ReadAllText("dmgtable.txt");
            _DammageTable = JsonConvert.DeserializeObject<KeyValuePair<UnitPair, int>[]>(dmgtable).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        #endregion

        #region private field
        AnimatedEntity animation;
        UnitType unitType;
        int hitPoint;
        #endregion

        #region public
        public AnimatedEntity Animation { get { return animation; } }
        public UnitType UnitType { get { return unitType; } }
        public int HitPoint { get { return hitPoint; } }
        public Owner Owner { get; set; }
        
        public Unit(UnitType unittype, AnimatedEntity anim,Owner owner,int hp = 100)
        {
            unitType = unittype;
            animation = anim;
            Owner = owner;
            hitPoint = hp;
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
        public static Unit Create(UnitType unittype,Owner owner,int hp = 100)
        {
            //todo load the sprite based on the owner of this unit
            return new Unit(unittype, (AnimatedEntity)CONTENT_MANAGER.animationEntities[unittype].Clone(), owner, hp);
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
