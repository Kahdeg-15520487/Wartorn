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
        
        public Unit(UnitType unittype, AnimatedEntity anim)
        {
            unitType = unittype;
            animation = anim;
            hitPoint = 20;
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
        public static Unit Create(UnitType unittype)
        {
            return new Unit(unittype, (AnimatedEntity)CONTENT_MANAGER.animationEntities[unittype].Clone());
        }
    }

    #region JsonConverter class
    public class UnitPairJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnitPair);
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
    }
    #endregion
}
