﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wartorn.GameData;
using Newtonsoft.Json;

namespace Wartorn.Storage
{
    class MapData
    {
        /* a map file data structure
         * basically it's a json file serialize from the class Map
         */

        public static Map LoadMap(string MapName)
        {
            //TODO: actually load map


            Map output = new Map();
            return output;
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
