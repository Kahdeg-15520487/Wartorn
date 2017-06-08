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
    static class DijkstraHelper
    {
        public static Graph CalculateGraph(Map map,Unit unit,Point position)
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
                    MapCell mapcell = map[point];
                    int cost = Unit.GetTravelCost(unit.UnitType, map[point].terrain);

                    #region todo allow pathing into transport unit
                    ////check if point is blocked by an unit
                    //if (mapcell.unit != null)
                    //{
                    //    //check if airborne unit
                    //    if (mapcell.unit.UnitType.GetMovementType() != MovementType.Air)
                    //    {
                    //        //check if enemy unit
                    //        if (mapcell.unit.Owner != unit.Owner)
                    //        {
                    //            cost = int.MaxValue;
                    //        }
                    //        else
                    //        {
                    //            //check if transport unit
                    //            if (mapcell.unit.UnitType != UnitType.APC
                    //             && mapcell.unit.UnitType != UnitType.TransportCopter
                    //             && mapcell.unit.UnitType != UnitType.Lander
                    //             && mapcell.unit.UnitType != UnitType.Cruiser)
                    //            {
                    //                cost = int.MaxValue;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    if (//check if there is a unit
                        map[point].unit != null
                        //check if that unit is an airborne unit
                        && map[point].unit.UnitType.GetMovementType() != MovementType.Air
                        //check if that unit is enemy
                        && map[point].unit.Owner != unit.Owner
                        )
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

            return graph;            
        }

        public static List<Point> FindRange(Graph graph)
        {
            var range = graph.FindReachableVertex();

            var result = new List<Point>();
            foreach (string dest in range)
            {
                result.Add(dest.Parse());
            }
            return result;
        }

        public static List<Point> FindPath(Graph graph,Point destination)
        {
            var path = graph.FindShortestPath(destination.toString());

            var result = new List<Point>();
            foreach (string node in path)
            {
                result.Add(node.Parse());
            }

            return result;
        }
    }
}
