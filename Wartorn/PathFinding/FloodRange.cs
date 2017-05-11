using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Wartorn;
using Wartorn.GameData;
using Wartorn.Utility;

namespace Wartorn.PathFinding
{
    class FloodRange
    {
        Dictionary<Point, Dictionary<Point, TerrainType>> _MovementCostGrid;

        public FloodRange(Map map)
        {
            _MovementCostGrid = new Dictionary<Point, Dictionary<Point, TerrainType>>();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Point currentPoint = new Point(x, y);
                    Point east = currentPoint.GetNearbyPoint(Direction.East)
                        , west = currentPoint.GetNearbyPoint(Direction.West)
                        , south = currentPoint.GetNearbyPoint(Direction.South)
                        , north = currentPoint.GetNearbyPoint(Direction.North);

                    Dictionary<Point, TerrainType> temp = new Dictionary<Point, TerrainType>();
                    if (map[east] != null)
                    {
                        temp.Add(east, map[east].terrain);
                    }
                    if (map[west] != null)
                    {
                        temp.Add(west, map[west].terrain);
                    }
                    if (map[north] != null)
                    {
                        temp.Add(north, map[north].terrain);
                    }
                    if (map[south] != null)
                    {
                        temp.Add(south, map[south].terrain);
                    }

                    _MovementCostGrid.Add(currentPoint, temp);
                }
            }
        }

        public List<Point> FindRange(UnitType ut)
        {
            return null;
        }

        public class Node
        {
            public Point Location { get; set; }
            public int TraverseCost { get; set; }
            public Node PreviousNode { get; set; }
            public Node NextNode { get; set; }

            public Node(Point location,int traversecost)
            {
                Location = location;
                TraverseCost = traversecost;
            }
        }
    }
}
