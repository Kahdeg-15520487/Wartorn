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
            //the dictionary of distance from a Point to another Point
            //this thing use string to label the Points
            public Dictionary<string, Dictionary<string, int>> Vertices = new Dictionary<string, Dictionary<string, int>>();

            //the shortest path from one point to another point.
            public Dictionary<string, string> Pathlist = new Dictionary<string, string>();

            //the source, the starting point, the origin of the graph
            //usually the location of a Unit
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

            public int CalculateShortestPathCost(string destination)
            {
                int cost = int.MaxValue;

                if (Pathlist.ContainsKey(destination))
                {
                    cost = 0;
                    string currentNode = destination;
                    while (currentNode.CompareTo(Source) != 0)
                    {
                        cost += Vertices[currentNode][Pathlist[currentNode]];
                        currentNode = Pathlist[currentNode];
                    }
                }

                return cost;
            }


            public List<string> FindReachableVertex(int maxCost)
            {
                List<string> reachableVertex = new List<string>();

                foreach (string dest in Pathlist.Keys)
                {
                    int cost = CalculateShortestPathCost(dest);
                    if (cost<=maxCost)
                    {
                        reachableVertex.Add(dest);
                    }
                }

                return reachableVertex;
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
                
                while (nodes.Count != 0)
                {
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

                    //loop through the surrounding Point of the smallest
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

                foreach (var kvp in previous)
                {
                    Pathlist.Add(kvp.Key, kvp.Value);
                }
            }
        }

        static class DijkstraTest
        {
            public static void test()
            {
                Graph g = new Graph();

                //this is a constants lookup table
                //to see how each movementtype move on each terrain
                Dictionary<MovementType, Dictionary<TerrainType, int>> TravelCost = new Dictionary<MovementType, Dictionary<TerrainType, int>>();
                TravelCost.Add(MovementType.Foot, new Dictionary<TerrainType, int>());
                TravelCost[MovementType.Foot].Add(TerrainType.Plain, 1);
                TravelCost[MovementType.Foot].Add(TerrainType.Road, 1);
                TravelCost[MovementType.Foot].Add(TerrainType.Tree, 1);
                TravelCost[MovementType.Foot].Add(TerrainType.Mountain, 2);
                TravelCost[MovementType.Foot].Add(TerrainType.Sea, int.MaxValue);

                TravelCost.Add(MovementType.Track, new Dictionary<TerrainType, int>());
                TravelCost[MovementType.Track].Add(TerrainType.Plain, 1);
                TravelCost[MovementType.Track].Add(TerrainType.Road, 1);
                TravelCost[MovementType.Track].Add(TerrainType.Tree, 2);
                TravelCost[MovementType.Track].Add(TerrainType.Mountain, int.MaxValue);
                TravelCost[MovementType.Track].Add(TerrainType.Sea, int.MaxValue);


                //the demo map
                var map = new Map(6, 6);
                map.Fill(TerrainType.Plain);

                //the demo unit
                Point start = new Point(2, 2);
                //Unit unit = new Unit(UnitType.Soldier, null, Owner.Red);
                Unit unit = new Unit(UnitType.Tank, null, Owner.Red);
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

                
                //max movement cost of the unit
                switch (unit.UnitType)
                {
                    case UnitType.Soldier:
                        maxcost = 3;
                        break;
                    case UnitType.Tank:
                        maxcost = 6;
                        break;
                    default:
                        break;
                }

                
                //init graph
                //just check if a point is connect to nearby point
                //this will be calculated with the map
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
                        if (map[east]!=null)
                        {
                            temp.Add(east.toString(), 0);
                        }
                        if (map[west]!=null)
                        {
                            temp.Add(west.toString(), 0);
                        }
                        if (map[north]!=null)
                        {
                            temp.Add(north.toString(), 0);
                        }
                        if (map[south]!=null)
                        {
                            temp.Add(south.toString(), 0);
                        }
                        var t = currentPoint.toString();//, converter);
                        g.add_vertex(t, temp);
                    }
                }

                //serialize navgraph
                string navgraph = JsonConvert.SerializeObject(g.Vertices.ToArray(),Formatting.Indented);
                File.WriteAllText("navgraph.txt", navgraph);

                //deserialize navgraph
                Dictionary<string, Dictionary<string, int>> navgg = new Dictionary<string, Dictionary<string, int>>();
                JsonConvert.DeserializeObject<KeyValuePair<string, Dictionary<string, int>>[]>(navgraph).ToList().ForEach(kvp => { navgg.Add(kvp.Key, kvp.Value); });


                //now we assign aproriate travel cost
                //based on terrain and movementtype
                List<string> vertices = g.Vertices.Keys.ToList();
                foreach (string vertex in vertices)
                {
                    List<string> neighbors = g.Vertices[vertex].Keys.ToList();
                    foreach (string neighbor in neighbors)
                    {
                        Point point = neighbor.Parse();
                        int cost = TravelCost[movementtype][map[point].terrain];
                        if (cost<int.MaxValue)
                        {
                            g.Vertices[vertex][neighbor] = cost;
                        }
                        else
                        {
                            g.Vertices[vertex].Remove(neighbor);
                        }
                    }
                }

                //start the dijkstra algorithm
                g.Dijkstra(start.toString());

                //find reachable vertex with the unit's max movement cost
                var range = g.FindReachableVertex(maxcost);
            }
        }
    }
}
