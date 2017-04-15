using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn.GameData
{
    class MapCell
    {
        public SpriteSheetTerrain terrain;
        public bool isFog;
        public Unit unit;
        public int unitId;

        public MapCell(SpriteSheetTerrain t)
        {
            terrain = t;
        }

        public MapCell(SpriteSheetTerrain t, Unit u, int unitId)
        {
            terrain = t;
            unit = u;
            this.unitId = unitId;
        }
    }
}
