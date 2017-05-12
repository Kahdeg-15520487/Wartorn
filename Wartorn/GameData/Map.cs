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
using Wartorn.PathFinding.Dijkstras;
using Wartorn.Utility;

namespace Wartorn.GameData
{
    class Map : IEnumerable
    {
        public MapCell[,] map { get; private set; }

        public Graph navgivationGraph;

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
            weather = m.weather;
            theme = m.theme;
            isProcessed = false;
        }

        public IEnumerator GetEnumerator()
        {
            return map.GetEnumerator();
        }

        public void Fill(TerrainType terrain)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (map[x,y] == null)
                    {
                        map[x, y] = new MapCell(terrain);
                    }
                    else
                    {
                        map[x, y].terrain = terrain;
                    }
                }
            }
            isProcessed = false;
        }

        public void GenerateNavigationMap()
        {
            navgivationGraph = new Graph();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Point currentPoint = new Point(x, y);
                    Point east = currentPoint.GetNearbyPoint(Direction.East)
                        , west = currentPoint.GetNearbyPoint(Direction.West)
                        , south = currentPoint.GetNearbyPoint(Direction.South)
                        , north = currentPoint.GetNearbyPoint(Direction.North);

                    Dictionary<string, int> temp = new Dictionary<string, int>();
                    if (this[east] != null)
                    {
                        temp.Add(east.toString(), 0);
                    }
                    if (this[west] != null)
                    {
                        temp.Add(west.toString(), 0);
                    }
                    if (this[north] != null)
                    {
                        temp.Add(north.toString(), 0);
                    }
                    if (this[south] != null)
                    {
                        temp.Add(south.toString(), 0);
                    }
                    var t = currentPoint.toString();//, converter);
                    navgivationGraph.add_vertex(t, temp);
                }
            }
        }
    }
}
