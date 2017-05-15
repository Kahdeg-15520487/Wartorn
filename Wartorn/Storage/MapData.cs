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

            return CompressHelper.Zip(output.ToString());
            //return output.ToString();
        }
    }
}
