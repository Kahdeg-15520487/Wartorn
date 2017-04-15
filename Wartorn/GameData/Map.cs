using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn.GameData
{
    [JsonObject(MemberSerialization.OptIn)]
    class Map : IEnumerable
    {
        [JsonProperty]
        MapCell[,] map;

        public int Width { get { return map.GetLength(0); } }
        public int Height { get { return map.GetLength(1); } }

        public MapCell this[int x, int y] { get { return map[x, y]; } set { map[x, y] = value; } }
        public MapCell this[Point p] { get { return map[p.X, p.Y]; } set { map[p.X, p.Y] = value; } }

        //constructor
        public Map()
        {
            map = new MapCell[20,20];
        }
        public Map(int w,int h)
        {
            map = new MapCell[w, h];
        }

        public MapCell getMapCell(int x,int y)
        {
            return map[x, y];
        }

        public IEnumerator GetEnumerator()
        {
            return map.GetEnumerator();
        }
    }
}
