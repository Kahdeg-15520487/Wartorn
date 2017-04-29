using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wartorn.GameData;
using Newtonsoft.Json;
using System.IO;

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

                Utility.HelperFunction.Log(new Exception(JsonConvert.SerializeObject(output, Formatting.Indented)));
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
