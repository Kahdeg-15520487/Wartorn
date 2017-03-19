using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wartorn.GameClass
{
    class Map
    {
        List<List<MapCell>> map;
        int width, height;
        public int Width { get; }
        public int Height { get; }

        public Map(int w,int h)
        {
            map = new List<List<MapCell>>();
            for (int i = 0; i < w; i++)
            {
                map.Add(new List<MapCell>());
                for (int j = 0; j < h; j++)
                {
                    map[i].Add(new MapCell(i,j));
                }
            }
        }
    }

    class MapCell
    {
        int x, y;

        public MapCell(int x,int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return ("(" + x.ToString() + " " + y.ToString() + ")");
        }
    }
}
