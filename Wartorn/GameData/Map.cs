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
    public class Map : IEnumerable
    {
        #region list of mapcell that have unit
        public List<Point> mapcellthathaveunit { get; private set; }

        /// <summary>
        /// add the unit to the map and register it.
        /// </summary>
        /// <param name="p">the position of the unit</param>
        /// <param name="unit">the unit to add</param>
        public void RegisterUnit(Point p,Unit unit)
        {
            if (this[p].unit == null && !mapcellthathaveunit.Contains(p))
            {
                this[p].unit = unit;
                mapcellthathaveunit.Add(p);
            }
        }

        /// <summary>
        /// remove the unit from the map.
        /// </summary>
        /// <param name="p">the position of the unit</param>
        public void RemoveUnit(Point p)
        {
            if (this[p].unit != null && mapcellthathaveunit.Contains(p))
            {
                this[p].unit = null;
                mapcellthathaveunit.Remove(p);
            }
        }

        /// <summary>
        /// find a unit's position in the map with it id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Point FindUnit(Guid guid)
        {
            Point result = Point.Zero;
            foreach (Point p in mapcellthathaveunit)
            {
                if (this[p].unit.guid == guid)
                {
                    result = p;
                }
            }
            return result;
        }

        /// <summary>
        /// move an unit from origin to destination
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        public void TeleportUnit(Point origin,Point destination)
        {
            if (this[origin].unit!=null && this[destination].unit==null)
            {
                var temp = this[origin].unit;
                RegisterUnit(destination, temp);
                RemoveUnit(origin);
            }
        }
        #endregion


        public MapCell[,] map { get; private set; }
        public Graph navigationGraph;

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
                isProcessed = false;
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
                isProcessed = false;
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

        #region building list

        private List<Point> mapcellthatbuilding;

        public IEnumerable<Point> GetOwnedBuilding(Owner owner)
        {
            foreach (var p in mapcellthatbuilding)
            {
                if (this[p].owner == owner)
                {
                    yield return p;
                }
            }
        }

        public void ChangeBuildingOwner(Point buildingposition, Owner owner)
        {
            this[mapcellthatbuilding.Find(p => { return p == buildingposition; })].owner = owner;
        }

        #endregion
        public void Clone(Map m)
        {
            map = m.map;
            navigationGraph = new Graph();

            if (m.navigationGraph != null)
            {
                navigationGraph.Vertices = new Dictionary<string, Dictionary<string, int>>(m.navigationGraph.Vertices);
            }
            else
            {
                this.GenerateNavigationMap();
            }
            weather = m.weather;
            theme = m.theme;
            isProcessed = false;

            mapcellthathaveunit = new List<Point>();
            mapcellthatbuilding = new List<Point>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Point p = new Point(x, y);
                    
                    
                    if (this[p].terrain.isBuilding())
                    {
                        mapcellthatbuilding.Add(p);
                    }
                    if (this[p].unit != null)
                    {
                        mapcellthathaveunit.Add(p); 
                    }
                   
                }
            }


            //    mapcellthathaveunit = new List<Point>();
            //    for (int x = 0; x < Width; x++)
            //    {
            //        for (int y = 0; y < Height; y++)
            //        {
            //            if (map[x,y].unit!=null)
            //            {
            //                mapcellthathaveunit.Add(new Point(x, y));
            //            }
            //        }
            //    }
            //
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
            navigationGraph = new Graph();

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
                    navigationGraph.add_vertex(t, temp);
                }
            }
        }
    }
}
