using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wartorn.GameData;
using Newtonsoft.Json;
using System.IO;
using Wartorn.Utility;

namespace Wartorn.Storage
{
    static class MapData
    {
        /* a map file data structure
         * basically it's a json file serialize from the class Map
         */

        public static Map LoadMap(string data)
        {
            data = CompressHelper.UnZip(data);

            var mapdata = data.Split('|');
            string majorver = string.Empty
                 , minorver = string.Empty;

            try
            {
                majorver = JsonConvert.DeserializeObject<string>(mapdata[0]);
                minorver = JsonConvert.DeserializeObject<string>(mapdata[1]);
            }
            catch (Exception e)
            {
                Utility.HelperFunction.Log(e);
            }

            if (string.Compare(majorver,VersionNumber.MajorVersion) != 0 
             || string.Compare(minorver, VersionNumber.MinorVersion) != 0)
            {
                CONTENT_MANAGER.ShowMessageBox("Cant't load map" + Environment.NewLine + "Version not compatible" + Environment.NewLine + "Game version: " + VersionNumber.GetVersionNumber + Environment.NewLine + "Map version: " + majorver + "." + minorver);
                return null;
            }

            Map output = new Map();

            try
            {
                output = JsonConvert.DeserializeObject<Map>(mapdata[2]);
            }
            catch (Exception er)
            {
                Utility.HelperFunction.Log(er);
                Environment.Exit(0);
            }

            if (output!=null)
            {
                return output;
            }
            else
            {
                Utility.HelperFunction.Log(new Exception(output?.ToString()));
                Environment.Exit(0);
                throw new NullReferenceException();
            }
        }

        public static string SaveMap(Map map)
        {
            StringBuilder output = new StringBuilder();

            output.Append(JsonConvert.SerializeObject(VersionNumber.MajorVersion, Formatting.Indented));
            output.Append('|');
            output.Append(JsonConvert.SerializeObject(VersionNumber.MinorVersion, Formatting.Indented));
            output.Append('|');
            map.GenerateNavigationMap();
            output.Append(JsonConvert.SerializeObject(map,Formatting.Indented));


            string result = CompressHelper.Zip(output.ToString());
            File.WriteAllText("notyetzip.txt", output.ToString());
            File.WriteAllText("zipped.txt", result);
            File.WriteAllText("zipthenunziped.txt", CompressHelper.UnZip(result));
            return result;
            //return output.ToString();
        }
    }

    public class MapJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Map);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
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
            for (int x = 0; x < temp.Width; x++)
            {
                for (int y = 0; y < temp.Height; y++)
                {
                    serializer.Serialize(writer, temp[x, y]);
                }
            }
            writer.WriteEndArray();
            writer.WritePropertyName("NavGraph");
            writer.WriteStartArray();
            foreach (var kvp in temp.navivationGraph.Vertices)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Key");
                serializer.Serialize(writer, kvp.Key);
                writer.WritePropertyName("Value");
                writer.WriteStartArray();
                foreach (var kvp2 in kvp.Value)
                {
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Map result;
            int width=0, height=0;
            Theme theme = Theme.Normal;
            Weather weather = Weather.Sunny;
            List<MapCell> map = new List<MapCell>();
            Dictionary<string, Dictionary<string, int>> navgraph = new Dictionary<string, Dictionary<string, int>>();

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
                        foreach (var obj in mapdata)
                        {
                            mctemp = JsonConvert.DeserializeObject<MapCell>(obj.ToString());
                            map.Add(mctemp);
                        }
                        //File.WriteAllText("mapdata.txt", JsonConvert.SerializeObject(map, Formatting.Indented));
                        break;
                    case "NavGraph":

                        var navgg = new Dictionary<string, Dictionary<string, int>>();

                        var tempg = serializer.Deserialize(reader);
                        var firstlvl = JsonConvert.DeserializeObject<KeyValuePair<string, object>[]>(tempg.ToString());
                        firstlvl.ToList().ForEach(kvp =>
                        {
                            var navggg = new Dictionary<string, int>();
                            var secondlvl = JsonConvert.DeserializeObject<KeyValuePair<string, int>[]>(kvp.Value.ToString());
                            secondlvl.ToList().ForEach(kvp2 =>
                            {
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
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    result[x, y] = map[c];
                    if (result[x,y].terrainbase != SpriteSheetTerrain.None)
                    {
                        isProcessed = true;
                    }
                    c++;
                }
            }
            result.navivationGraph = new PathFinding.Dijkstras.Graph();
            result.navivationGraph.Vertices = new Dictionary<string, Dictionary<string, int>>(navgraph);
            result.IsProcessed = isProcessed;

            return result;
        }
    }

    public class MapCellJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MapCell);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            MapCell temp = (MapCell)value;

            writer.WriteStartObject();
            writer.WritePropertyName("terrain");
            serializer.Serialize(writer, temp.terrain.ToString());
            writer.WritePropertyName("owner");
            serializer.Serialize(writer, temp.owner.ToString());
            writer.WritePropertyName("unit");
            serializer.Serialize(writer, temp.unit);
            writer.WritePropertyName("base");
            serializer.Serialize(writer, temp.terrainbase.ToString());
            writer.WritePropertyName("lower");
            serializer.Serialize(writer, temp.terrainLower.ToString());
            writer.WritePropertyName("upper");
            serializer.Serialize(writer, temp.terrainUpper.ToString());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            TerrainType terrain = TerrainType.Plain;
            SpriteSheetTerrain terrainbase = SpriteSheetTerrain.None;
            SpriteSheetTerrain terrainLower = SpriteSheetTerrain.None;
            SpriteSheetTerrain terrainUpper = SpriteSheetTerrain.None;
            Owner owner = Owner.None;
            Unit unit = null;
            int unitid = 0;

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
                    case "terrain":
                        terrain = (serializer.Deserialize<string>(reader)).ToEnum<TerrainType>();
                        break;
                    case "owner":
                        owner = (serializer.Deserialize<string>(reader)).ToEnum<Owner>();
                        break;
                    case "unit":
                        unit = serializer.Deserialize<Unit>(reader);
                        break;
                        break;
                    case "base":
                        terrainbase = (serializer.Deserialize<string>(reader)).ToEnum<SpriteSheetTerrain>();
                        break;
                    case "lower":
                        terrainUpper = (serializer.Deserialize<string>(reader)).ToEnum<SpriteSheetTerrain>();
                        break;
                    case "upper":
                        terrainLower = (serializer.Deserialize<string>(reader)).ToEnum<SpriteSheetTerrain>();
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
}
