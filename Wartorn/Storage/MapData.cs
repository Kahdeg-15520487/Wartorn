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
            //TODO: actually load map
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
            output.Append(JsonConvert.SerializeObject(map,Formatting.Indented));

            return output.ToString();
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
            serializer.Serialize(writer, temp.map);
            writer.WriteEndObject();

            //writer.WriteStartArray();
            //for (int x = 0; x < temp.Width; x++)
            //{
            //    for (int y = 0; y < temp.Height; y++)
            //    {
            //        serializer.Serialize(writer, temp[x, y]);
            //    }
            //}
            //writer.WriteEndArray();
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Map result;
            int width=0, height=0;
            Theme theme = Theme.Normal;
            Weather weather = Weather.Sunny;
            MapCell[,] map = new MapCell[0, 0];

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
                        map = serializer.Deserialize<MapCell[,]>(reader);
                        break;
                    default:
                        break;
                }
            }

            result = new Map(width, height);
            result.theme = theme;
            result.weather = weather;
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    result[x, y] = map[x, y];
                }
            }

            return result;
        }
    }
}
