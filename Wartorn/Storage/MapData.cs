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
            int w, h;

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

            output.Append(JsonConvert.SerializeObject(map.Width));
            output.Append('|');
            output.Append(JsonConvert.SerializeObject(map.Height));
            output.Append('|');
            output.Append(JsonConvert.SerializeObject(map));

            return output.ToString();
        }
    }
}
