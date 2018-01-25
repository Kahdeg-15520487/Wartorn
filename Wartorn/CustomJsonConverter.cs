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
using Wartorn.Utility.Drawing;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wartorn {
	namespace CustomJsonConverter {
		#region map
		public class MapJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(Map);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				Map temp = (Map)value;

				writer.WriteStartObject();
				writer.WritePropertyName("Width");
				serializer.Serialize(writer, temp.Width);
				writer.WritePropertyName("Height");
				serializer.Serialize(writer, temp.Height);
				writer.WritePropertyName("Theme");
				serializer.Serialize(writer, temp.theme.ToString());
				writer.WritePropertyName("Weather");
				serializer.Serialize(writer, temp.weather.ToString());
				writer.WritePropertyName("Data");
				writer.WriteStartArray();
				for (int x = 0; x < temp.Width; x++) {
					for (int y = 0; y < temp.Height; y++) {
						serializer.Serialize(writer, temp[x, y]);
					}
				}
				writer.WriteEndArray();
				writer.WritePropertyName("NavGraph");
				writer.WriteStartArray();
				foreach (var kvp in temp.navigationGraph.Vertices) {
					writer.WriteStartObject();
					writer.WritePropertyName("Key");
					serializer.Serialize(writer, kvp.Key);
					writer.WritePropertyName("Value");
					writer.WriteStartArray();
					foreach (var kvp2 in kvp.Value) {
						writer.WriteStartObject();
						writer.WritePropertyName("Key");
						serializer.Serialize(writer, kvp2.Key);
						writer.WritePropertyName("Value");
						serializer.Serialize(writer, kvp2.Value);
						writer.WriteEndObject();
					}
					writer.WriteEndArray();
					writer.WriteEndObject();
				}
				writer.WriteEndArray();
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				Map result;
				int width = 0, height = 0;
				Theme theme = Theme.Normal;
				Weather weather = Weather.Sunny;
				List<MapCell> map = new List<MapCell>();
				Dictionary<string, Dictionary<string, int>> navgraph = new Dictionary<string, Dictionary<string, int>>();

				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					switch (propertyName) {
						case "Width":
							width = serializer.Deserialize<int>(reader);
							break;
						case "Height":
							height = serializer.Deserialize<int>(reader);
							break;
						case "Theme":
							theme = (serializer.Deserialize<string>(reader)).ToEnum<Theme>();
							break;
						case "Weather":
							weather = (serializer.Deserialize<string>(reader)).ToEnum<Weather>();
							break;
						case "Data":
							//map = serializer.Deserialize<MapCell[,]>(reader);
							var temp = serializer.Deserialize(reader);
							List<object> mapdata = JsonConvert.DeserializeObject<List<object>>(temp.ToString());
							MapCell mctemp;
							foreach (var obj in mapdata) {
								mctemp = JsonConvert.DeserializeObject<MapCell>(obj.ToString());
								map.Add(mctemp);
							}
							//File.WriteAllText("mapdata.txt", JsonConvert.SerializeObject(map, Formatting.Indented));
							break;
						case "NavGraph":

							var navgg = new Dictionary<string, Dictionary<string, int>>();

							var tempg = serializer.Deserialize(reader);
							var firstlvl = JsonConvert.DeserializeObject<KeyValuePair<string, object>[]>(tempg.ToString());
							firstlvl.ToList().ForEach(kvp => {
								var navggg = new Dictionary<string, int>();
								var secondlvl = JsonConvert.DeserializeObject<KeyValuePair<string, int>[]>(kvp.Value.ToString());
								secondlvl.ToList().ForEach(kvp2 => {
									navggg.Add(kvp2.Key, kvp2.Value);
								});
								navgg.Add(kvp.Key, navggg);
							});
							navgraph = new Dictionary<string, Dictionary<string, int>>(navgg);
							break;
						default:
							break;
					}
				}

				result = new Map(width, height);
				result.theme = theme;
				result.weather = weather;
				int c = 0;
				bool isProcessed = false;
				for (int x = 0; x < result.Width; x++) {
					for (int y = 0; y < result.Height; y++) {
						result[x, y] = map[c];
						if (result[x, y].terrainbase != SpriteSheetTerrain.None) {
							isProcessed = true;
						}
						c++;
					}
				}
				result.navigationGraph = new PathFinding.Dijkstras.Graph();
				result.navigationGraph.Vertices = new Dictionary<string, Dictionary<string, int>>(navgraph);
				result.IsProcessed = isProcessed;

				return result;
			}
		}
		#endregion

		#region mapcell
		public class MapCellJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(MapCell);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				MapCell temp = (MapCell)value;

				writer.WriteStartObject();
				writer.WritePropertyName("base");
				serializer.Serialize(writer, (int)temp.terrainbase);
				writer.WritePropertyName("lower");
				serializer.Serialize(writer, (int)temp.terrainLower);
				writer.WritePropertyName("upper");
				serializer.Serialize(writer, (int)temp.terrainUpper);
				writer.WritePropertyName("terrain");
				serializer.Serialize(writer, (int)temp.terrain);
				writer.WritePropertyName("owner");
				serializer.Serialize(writer, (int)temp.owner);
				writer.WritePropertyName("unit");
				serializer.Serialize(writer, temp.unit);
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				TerrainType terrain = TerrainType.Plain;
				SpriteSheetTerrain terrainbase = SpriteSheetTerrain.None;
				SpriteSheetTerrain terrainLower = SpriteSheetTerrain.None;
				SpriteSheetTerrain terrainUpper = SpriteSheetTerrain.None;
				Owner owner = Owner.None;
				Unit unit = null;

				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					switch (propertyName) {
						case "terrain":
							//terrain = (TerrainType)(serializer.Deserialize<int>(reader));
							terrain = serializer.Deserialize<string>(reader).ToEnum<TerrainType>();
							break;
						case "owner":
							//owner = (Owner)(serializer.Deserialize<int>(reader));
							owner = serializer.Deserialize<string>(reader).ToEnum<Owner>();
							break;
						case "unit":
							unit = serializer.Deserialize<Unit>(reader);
							break;
						case "base":
							//terrainbase = (SpriteSheetTerrain)(serializer.Deserialize<int>(reader));
							terrainbase = serializer.Deserialize<string>(reader).ToEnum<SpriteSheetTerrain>();
							break;
						case "lower":
							//terrainUpper = (SpriteSheetTerrain)(serializer.Deserialize<int>(reader));
							terrainLower= serializer.Deserialize<string>(reader).ToEnum<SpriteSheetTerrain>();
							break;
						case "upper":
							//terrainLower = (SpriteSheetTerrain)(serializer.Deserialize<int>(reader));
							terrainUpper = serializer.Deserialize<string>(reader).ToEnum<SpriteSheetTerrain>();
							break;
						default:
							break;
					}
				}


				MapCell result = new MapCell(terrain, unit);
				result.owner = owner;
				result.terrainbase = terrainbase;
				result.terrainLower = terrainLower;
				result.terrainUpper = terrainUpper;

				return result;
			}
		}
		#endregion

		#region unit
		public class UnitJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(Unit);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				Unit temp = (Unit)value;
				writer.WriteStartObject();
				writer.WritePropertyName("UnitType");
				serializer.Serialize(writer, temp.UnitType);
				writer.WritePropertyName("Owner");
				serializer.Serialize(writer, (int)temp.Owner);
				writer.WritePropertyName("HP");
				serializer.Serialize(writer, temp.HitPoint);
				writer.WritePropertyName("guid");
				serializer.Serialize(writer, temp.guid);
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				UnitType unittype = UnitType.None;
				Owner owner = Owner.None;
				int hp = 0;

				bool gotUnittype = false;
				bool gotOwner = false;
				bool gotHp = false;

				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}
					switch (propertyName) {
						case "UnitType":
							unittype = serializer.Deserialize<UnitType>(reader);
							gotUnittype = true;
							break;
						case "Owner":
							//owner = (Owner)(serializer.Deserialize<int>(reader));
							owner = serializer.Deserialize<string>(reader).ToEnum<Owner>();
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

				if (!(gotUnittype && gotHp && gotOwner)) {
					//throw new InvalidDataException("Not enought data");
					return null;
				}

				return UnitCreationHelper.Create(unittype, owner, hp);
			}
		}
		#endregion

		#region unittype
		public class UnitTypeJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(UnitType);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				UnitType temp = (UnitType)value;
				writer.WriteStartObject();
				writer.WritePropertyName("value");
				serializer.Serialize(writer, (int)temp);
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				UnitType result = UnitType.None;
				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					if (propertyName == "value") {
						//result = (UnitType)(serializer.Deserialize<int>(reader));
						result = serializer.Deserialize<string>(reader).ToEnum<UnitType>();
					}
				}

				return result;
			}
		}
		#endregion

		#region unitpair
		public class UnitPairJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(UnitPair);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				UnitPair temp = (UnitPair)value;

				writer.WriteStartObject();
				writer.WritePropertyName("Attacker");
				serializer.Serialize(writer, (int)temp.Attacker);
				writer.WritePropertyName("Defender");
				serializer.Serialize(writer, (int)temp.Defender);
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				UnitType attacker = default(UnitType);
				UnitType defender = default(UnitType);

				bool gotAttacker = false;
				bool gotDefender = false;

				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					if (propertyName == "Attacker") {
						attacker = (UnitType)(serializer.Deserialize<int>(reader));
						gotAttacker = true;
					}

					if (propertyName == "Defender") {
						defender = (UnitType)(serializer.Deserialize<int>(reader));
						gotDefender = true;
					}
				}

				if (!(gotAttacker && gotDefender)) {
					throw new InvalidDataException("Not enought data");
				}

				return new UnitPair(attacker, defender);
			}
		}
		#endregion

		#region movementtype
		public class MovementTypeJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(MovementType);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				MovementType temp = (MovementType)value;
				writer.WriteStartObject();
				writer.WritePropertyName("value");
				serializer.Serialize(writer, (int)temp);
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				MovementType result = MovementType.None;
				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					if (propertyName == "value") {
						//result = (MovementType)(serializer.Deserialize<int>(reader));
						result = serializer.Deserialize<string>(reader).ToEnum<MovementType>();
					}
				}

				return result;
			}
		}
		#endregion

		#region terraintype
		public class TerrainTypeJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(TerrainType);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				TerrainType temp = (TerrainType)value;
				writer.WriteStartObject();
				writer.WritePropertyName("value");
				serializer.Serialize(writer, (int)temp);
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				TerrainType result = TerrainType.Plain;
				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					if (propertyName == "value") {
						//result = (TerrainType)serializer.Deserialize<int>(reader);
						result = (serializer.Deserialize<string>(reader)).ToEnum<TerrainType>();
					}
				}

				return result;
			}
		}
		#endregion

		#region Dictionary_MovementType_Dictionary_TerrainType_int
		public class Dictionary_MovementType_Dictionary_TerrainType_int_JsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(Dictionary<MovementType, Dictionary<TerrainType, int>>);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				Dictionary<MovementType, Dictionary<TerrainType, int>> temp = (Dictionary<MovementType, Dictionary<TerrainType, int>>)value;
				writer.WriteStartObject();
				writer.WritePropertyName("data");
				writer.WriteStartArray();

				foreach (var kvp in temp) {
					writer.WriteStartObject();
					writer.WritePropertyName("Key");
					serializer.Serialize(writer, kvp.Key);
					writer.WritePropertyName("Value");
					writer.WriteStartArray();
					foreach (var kvp2 in kvp.Value) {
						writer.WriteStartObject();
						writer.WritePropertyName("Key");
						serializer.Serialize(writer, kvp2.Key);
						writer.WritePropertyName("Value");
						serializer.Serialize(writer, kvp2.Value);
						writer.WriteEndObject();
					}
					writer.WriteEndArray();
					writer.WriteEndObject();
				}

				writer.WriteEndArray();
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				Dictionary<MovementType, Dictionary<TerrainType, int>> result = new Dictionary<MovementType, Dictionary<TerrainType, int>>();

				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					if (propertyName == "data") {
						result = new Dictionary<MovementType, Dictionary<TerrainType, int>>();

						var tempg = serializer.Deserialize(reader);
						var firstlvl = JsonConvert.DeserializeObject<KeyValuePair<MovementType, object>[]>(tempg.ToString());
						firstlvl.ToList().ForEach(kvp => {
							result.Add(kvp.Key, new Dictionary<TerrainType, int>());
							var secondlvl = JsonConvert.DeserializeObject<KeyValuePair<TerrainType, int>[]>(kvp.Value.ToString());
							secondlvl.ToList().ForEach(kvp2 => {
								result[kvp.Key].Add(kvp2.Key, kvp2.Value);
							});
						});
					}
				}

				return new Dictionary<MovementType, Dictionary<TerrainType, int>>(result);
			}
		}
		#endregion

		#region range
		public class RangeJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(Range);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				Range temp = (Range)value;

				writer.WriteStartObject();
				writer.WritePropertyName("Max");
				serializer.Serialize(writer, temp.Max);
				writer.WritePropertyName("Min");
				serializer.Serialize(writer, temp.Min);
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				int max = 0;
				int min = 0;

				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					switch (propertyName) {
						case "Max":
							max = serializer.Deserialize<int>(reader);
							break;
						case "Min":
							min = serializer.Deserialize<int>(reader);
							break;
						default:
							break;
					}
				}

				return new Range(max, min);
			}
		}
		#endregion

		#region Dictionary_UnitType_Dictionary_UnitType_int
		public class Dictionary_UnitType_Dictionary_UnitType_int_JsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(Dictionary<UnitType, Dictionary<UnitType, int>>);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				Dictionary<UnitType, Dictionary<UnitType, int>> temp = (Dictionary<UnitType, Dictionary<UnitType, int>>)value;
				writer.WriteStartObject();
				writer.WritePropertyName("data");
				writer.WriteStartArray();

				foreach (var kvp in temp) {
					writer.WriteStartObject();
					writer.WritePropertyName("Key");
					serializer.Serialize(writer, kvp.Key);
					writer.WritePropertyName("Value");
					writer.WriteStartArray();
					foreach (var kvp2 in kvp.Value) {
						writer.WriteStartObject();
						writer.WritePropertyName("Key");
						serializer.Serialize(writer, kvp2.Key);
						writer.WritePropertyName("Value");
						serializer.Serialize(writer, kvp2.Value);
						writer.WriteEndObject();
					}
					writer.WriteEndArray();
					writer.WriteEndObject();
				}

				writer.WriteEndArray();
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				Dictionary<UnitType, Dictionary<UnitType, int>> result = new Dictionary<UnitType, Dictionary<UnitType, int>>();

				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					if (propertyName == "data") {
						result = new Dictionary<UnitType, Dictionary<UnitType, int>>();

						var tempg = serializer.Deserialize(reader);
						var firstlvl = JsonConvert.DeserializeObject<KeyValuePair<UnitType, object>[]>(tempg.ToString());
						firstlvl.ToList().ForEach(kvp => {
							result.Add(kvp.Key, new Dictionary<UnitType, int>());
							var secondlvl = JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(kvp.Value.ToString());
							secondlvl.ToList().ForEach(kvp2 => {
								result[kvp.Key].Add(kvp2.Key, kvp2.Value);
							});
						});
					}
				}

				return new Dictionary<UnitType, Dictionary<UnitType, int>>(result);
			}
		}
		#endregion

		#region UnitStat
		public class UnitStatJsonConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return objectType == typeof(UnitStat);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				UnitStat temp = (UnitStat)value;
				writer.WriteStartObject();
				writer.WritePropertyName("Cost");
				serializer.Serialize(writer, temp.Cost);
				writer.WritePropertyName("Gas");
				serializer.Serialize(writer, temp.Gas);
				writer.WritePropertyName("Ammo");
				serializer.Serialize(writer, temp.Ammo);
				writer.WritePropertyName("AP");
				serializer.Serialize(writer, temp.ActionPoint);
				writer.WritePropertyName("MvmRange");
				serializer.Serialize(writer, temp.MovementRange);
				writer.WritePropertyName("MvmType");
				serializer.Serialize(writer, temp.MovementType);
				writer.WritePropertyName("AtkRange");
				serializer.Serialize(writer, temp.AttackRange);
				writer.WritePropertyName("VisRange");
				serializer.Serialize(writer, temp.VisionRange);
				writer.WriteEndObject();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				int Cost = 0;
				int Gas = 0;
				int Ammo = 0;
				int ActionPoint = 0;
				int MovementRange = 0;
				MovementType MovementType = default(MovementType);
				Range AttackRange = default(Range);
				int VisionRange = 0;

				int propcount = 0;

				while (reader.Read()) {
					if (reader.TokenType != JsonToken.PropertyName) {
						break;
					}

					string propertyName = (string)reader.Value;
					if (!reader.Read()) {
						continue;
					}

					switch (propertyName) {
						case "Cost":
							Cost = serializer.Deserialize<int>(reader);
							propcount++;
							break;
						case "Gas":
							Gas = serializer.Deserialize<int>(reader);
							propcount++;
							break;
						case "Ammo":
							Ammo = serializer.Deserialize<int>(reader);
							propcount++;
							break;
						case "AP":
							ActionPoint = serializer.Deserialize<int>(reader);
							propcount++;
							break;
						case "MvmRange":
							MovementRange = serializer.Deserialize<int>(reader);
							propcount++;
							break;
						case "MvmType":
							MovementType = serializer.Deserialize<MovementType>(reader);
							propcount++;
							break;
						case "AtkRange":
							AttackRange = serializer.Deserialize<Range>(reader);
							propcount++;
							break;
						case "VisRange":
							VisionRange = serializer.Deserialize<int>(reader);
							propcount++;
							break;
					}
				}

				return new UnitStat() {
					Cost = Cost,
					Gas = Gas,
					Ammo = Ammo,
					ActionPoint = ActionPoint,
					MovementRange = MovementRange,
					MovementType = MovementType,
					AttackRange = AttackRange,
					VisionRange = VisionRange
				};
			}
			#endregion
		}
	}
}