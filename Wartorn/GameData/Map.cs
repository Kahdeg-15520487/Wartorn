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
    class Map : IEnumerable
    {
        MapCell[,] map;

        private bool isProcessed = false;
        public bool IsProcessed
        {
            get
            {
                return isProcessed;
            }
            set
            {
                isProcessed = value;
            }
        }

        public int Width { get { return map.GetLength(0); } }
        public int Height { get { return map.GetLength(1); } }

        public Weather weather = Weather.Sunny;
        public Theme theme = Theme.Normal;

        public MapCell this[int x, int y]
        {
            get
            {
                try
                {
                    return map[x, y];
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            set
            {
                map[x, y] = value;
            }
        }
        public MapCell this[Point p]
        {
            get
            {
                try
                {
                    return map[p.X, p.Y];
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            set
            {
                map[p.X, p.Y] = value;
            }
        }

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

        public void Clone(Map m)
        {
            map = m.map;
        }

        public IEnumerator GetEnumerator()
        {
            return map.GetEnumerator();
        }

        public void Fill(TerrainType terrain)
        {
            foreach (var mapcell in map)
            {
                mapcell.terrain = terrain;
            }
        }
    }
}
