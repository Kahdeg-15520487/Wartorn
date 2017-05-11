using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wartorn.GameData;
using Wartorn.Utility;

namespace Wartorn.PathFinding
{
    //actually we need the whole graph after the algorthm calculate the path.

    namespace Dijkstras
    {
        class Graph
        {
            //vertex list aka 
            //the dictionary of distance for a Point to another Point with the distance betwene those 2 Points
            //this thing use string to label the Points
            public Dictionary<string, Dictionary<string, int>> Vertices = new Dictionary<string, Dictionary<string, int>>();
            public Dictionary<string, string> Pathlist = new Dictionary<string, string>();
            public string Source;

            //add a Point with its associated distance to nearby Points
            public void add_vertex(string name, Dictionary<string, int> edges)
            {
                Vertices[name] = edges;
            }

            //TODO make a range calculate

            public List<string> FindShortestPath(string destination)
            {
                List<string> path = null;

                if (Pathlist.ContainsKey(destination))
                {
                    path = new List<string>();
                    string currentNode = destination;
                    while (currentNode.CompareTo(Source) != 0)
                    {
                        path.Add(currentNode);
                        currentNode = Pathlist[currentNode];
                    }
                    path.Add(Source);
                    path.Reverse();
                }

                return path;
            }

            /// <summary>
            /// find the shortest path between 2 Point
            /// </summary>
            /// <param name="source"> the label of the starting Point</param>
            /// <param name="finish">the label of the destination Point</param>
            /// <returns>the List of the Points that is on the ways</returns>
            public void Dijkstra(string source)
            {
                Source = source;
                var previous = new Dictionary<string, string>();
                var distances = new Dictionary<string, int>();
                var nodes = new List<string>();

                //initialize dijktra table
                //set the distances between of the starting Point to 0 cause we"re already here
                //set every distances between every point to horizontal 8
                foreach (var vertex in Vertices)
                {
                    if (vertex.Key == source)
                    {
                        distances[vertex.Key] = 0;
                    }
                    else
                    {
                        distances[vertex.Key] = int.MaxValue;
                    }

                    nodes.Add(vertex.Key);
                }

                StringBuilder steplog = new StringBuilder();
                while (nodes.Count != 0)
                {
                    previous.ToList().ForEach(kvp =>
                    {
                       steplog.AppendFormat("{0} <- {1}|", kvp.Key, kvp.Value);
                    });
                    steplog.AppendLine();
                    distances.ToList().ForEach(kvp =>
                    {
                        steplog.AppendFormat("{0} = {1}|", kvp.Key, kvp.Value);
                    });
                    steplog.AppendLine();
                    nodes.ForEach(node =>
                    {
                        steplog.AppendLine(node);
                    });
                    steplog.AppendLine();

                    //sort the Point based on the distances of each Point to the previous Point
                    nodes.Sort((x, y) => distances[x] - distances[y]);

                    //then the smallest node is added to the potential Path
                    var smallest = nodes[0];
                    nodes.Remove(smallest);

                    //if the distances of such smallest point is still horizontal 8
                    //then sadly there is not a viable Path
                    if (distances[smallest] == int.MaxValue)
                    {
                        break;
                    }

                    //loop through the surronding Point of the smallest
                    //to see if there is a Potential Shortest distance
                    foreach (var neighbor in Vertices[smallest])
                    {
                        var alt = distances[smallest] + neighbor.Value;
                        if (alt < distances[neighbor.Key])
                        {
                            distances[neighbor.Key] = alt;
                            previous[neighbor.Key] = smallest;
                        }
                    }
                }

                //StringBuilder dijkstraGraphFromSource = new StringBuilder();
                foreach (var kvp in previous)
                {
                    //dijkstraGraphFromSource.AppendFormat("{0} <- {1}", kvp.Key, kvp.Value);
                    //dijkstraGraphFromSource.AppendLine();
                    Pathlist.Add(kvp.Key, kvp.Value);
                }
                //File.WriteAllText("dijktra.txt", dijkstraGraphFromSource.ToString());
                //File.WriteAllText("dijktra.txt", steplog.ToString());
            }
        }

        static class DijkstraTest
        {
            public static void test()
            {
                Graph g = new Graph();

                Dictionary<MovementType, Dictionary<TerrainType, int>> TravelCost = new Dictionary<MovementType, Dictionary<TerrainType, int>>();
                TravelCost.Add(MovementType.Foot, new Dictionary<TerrainType, int>());
                TravelCost[MovementType.Foot].Add(TerrainType.Plain, 1);
                TravelCost[MovementType.Foot].Add(TerrainType.Road, 1);
                TravelCost[MovementType.Foot].Add(TerrainType.Tree, 1);
                TravelCost[MovementType.Foot].Add(TerrainType.Mountain, 2);
                TravelCost[MovementType.Foot].Add(TerrainType.Sea, int.MaxValue);


                var map = new Map(5, 5);
                map.Fill(TerrainType.Plain);

                Point start = new Point(2, 2);
                Unit unit = new Unit(UnitType.Soldier, null, Owner.Red);
                MovementType movementtype = unit.UnitType.GetMovementType();
                int maxcost = 0;

                Owner local = Owner.Red;

                map[0, 0].terrain = TerrainType.Mountain;
                map[1, 0].terrain = TerrainType.Mountain;
                map[2, 0].terrain = TerrainType.Mountain;

                map[3, 1].terrain = TerrainType.Tree;
                map[3, 2].terrain = TerrainType.Tree;

                map[1, 2].unit = new Unit(UnitType.Soldier, null, Owner.Blue);

                map[1, 3].terrain = TerrainType.Sea;
                map[2, 3].terrain = TerrainType.Sea;

                //process this map to a Graph

                switch (unit.UnitType)
                {
                    case UnitType.Soldier:
                        maxcost = 3;
                        break;
                    default:
                        break;
                }

                //PointJsonConverter converter = new PointJsonConverter();

                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        Point currentPoint = new Point(x, y);
                        Point east = currentPoint.GetNearbyPoint(Direction.East)
                            , west = currentPoint.GetNearbyPoint(Direction.West)
                            , south = currentPoint.GetNearbyPoint(Direction.South)
                            , north = currentPoint.GetNearbyPoint(Direction.North);

                        Dictionary<string, int> temp = new Dictionary<string, int>();
                        if (map[east] != null)
                        {
                            temp.Add(east.toString(), TravelCost[movementtype][map[east].terrain]);
                        }
                        if (map[west] != null)
                        {
                            temp.Add(west.toString(), TravelCost[movementtype][map[west].terrain]);
                        }
                        if (map[north] != null)
                        {
                            temp.Add(north.toString(), TravelCost[movementtype][map[north].terrain]);
                        }
                        if (map[south] != null)
                        {
                            temp.Add(south.toString(), TravelCost[movementtype][map[south].terrain]);
                        }
                        var t = currentPoint.toString();//, converter);
                        g.add_vertex(t, temp);
                    }
                }


                //g.add_vertex("A", new Dictionary<string, int>() { { "B", 7 }, { "C", 8 } });
                //g.add_vertex("B", new Dictionary<string, int>() { { "A", 7 }, { "F", 2 } });
                //g.add_vertex("C", new Dictionary<string, int>() { { "A", 8 }, { "F", 6 }, { "G", 4 } });
                //g.add_vertex("D", new Dictionary<string, int>() { { "F", 8 } });
                //g.add_vertex("E", new Dictionary<string, int>() { { "H", 1 } });
                //g.add_vertex("F", new Dictionary<string, int>() { { "B", 2 }, { "C", 6 }, { "D", 8 }, { "G", 9 }, { "H", 3 } });
                //g.add_vertex("G", new Dictionary<string, int>() { { "C", 4 }, { "F", 9 } });
                //g.add_vertex("H", new Dictionary<string, int>() { { "E", 1 }, { "F", 3 } });
                //g.add_vertex("I", new Dictionary<string, int>() { { "J", 2 } });
                //g.add_vertex("J", new Dictionary<string, int>() { { "I", 2 } });

                StringBuilder result = new StringBuilder();

                g.Dijkstra(start.toString());

                g.Vertices.ToList().ForEach(kvp =>
                {
                    kvp.Value.ToList().ForEach(kvp2 =>
                    {
                        result.AppendFormat("{0} -> {1} : {2}", kvp.Key, kvp2.Key, kvp2.Value);
                        result.AppendLine();
                    });
                    result.AppendLine();
                });
                File.WriteAllText("graph.txt", result.ToString());

                var path = g.FindShortestPath((new Point(0, 0)).toString());
                File.WriteAllLines("dijktra.txt", path);
            }
        }
    }
}
