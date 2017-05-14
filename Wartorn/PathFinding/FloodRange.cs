using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Wartorn;
using Wartorn.GameData;
using Wartorn.Utility;
using Wartorn.PathFinding.Dijkstras;

namespace Wartorn.PathFinding
{
    static class FloodRange
    {
        public static List<Point> FindRange(Map map,Unit unit,Point position)
        {
            UnitInformation unitinfo = new UnitInformation(unit.UnitType);

            Graph graph = new Graph();
            graph.Source = position.toString();

            graph.Vertices = new Dictionary<string, Dictionary<string, int>>();

            foreach (string vertex in map.navigationGraph.Vertices.Keys.ToList())
            {
                graph.Vertices.Add(vertex, new Dictionary<string, int>());
                foreach (string neighbor in map.navigationGraph.Vertices[vertex].Keys.ToList())
                {
                    Point point = neighbor.Parse();
                    int cost = Unit.GetTravelCost(unit.UnitType, map[point].terrain);
                    //check if point is blocked by enemy unit
                    if (map[point].unit != null
                        && map[point].unit.UnitType.GetMovementType() != MovementType.Air
                        && map[point].unit.Owner != unit.Owner)
                    {
                        cost = int.MaxValue;
                    }
                    if (cost < int.MaxValue)
                    {
                        graph.Vertices[vertex].Add(neighbor, cost);
                    }
                }
            }

            graph.Dijkstra(position.toString(), unitinfo.Move + 1);
            var range = graph.FindReachableVertex(unitinfo.Move);

            CONTENT_MANAGER.ShowMessageBox(range.Count.ToString() + Environment.NewLine + unitinfo.Move.ToString());

            var result = new List<Point>();
            foreach (string dest in range)
            {
                result.Add(dest.Parse());
            }
            return result;
        }
    }
}
