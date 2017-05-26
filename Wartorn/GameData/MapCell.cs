using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;
using Newtonsoft.Json;

namespace Wartorn.GameData
{
    public class MapCell
    {
        public SpriteSheetTerrain terrainLower = SpriteSheetTerrain.None;
        public SpriteSheetTerrain terrainUpper = SpriteSheetTerrain.None;
        public SpriteSheetTerrain terrainbase;
        public TerrainType terrain;
        public bool isFog;
        public Unit unit;
        public Owner owner = Owner.None;

        /*
        public MapCell(SpriteSheetTerrain t)
        {
            terrainbase = t;
        }

        public MapCell(SpriteSheetTerrain t, Unit u, int unitId)
        {
            terrainbase = t;
            unit = u;
            this.unitId = unitId;
        }

        public MapCell(SpriteSheetTerrain t, SpriteSheetTerrain ot, SpriteSheetTerrain oot, Unit u, int unitId, bool isfog)
        {
            terrainbase = t;
            terrainLower = ot;
            terrainUpper = oot;
            unit = u;
            this.unitId = unitId;
            isFog = isfog;
        }
        */

        public MapCell(TerrainType t)
        {
            terrain = t;
            unit = null;
            isFog = false;
        }

        [JsonConstructor]
        public MapCell(TerrainType t, Unit u, bool isfog = false)
        {
            terrain = t;
            unit = u;
            isFog = isfog;
        }

        public void ClearRenderData()
        {
            terrainbase = SpriteSheetTerrain.None;
            terrainLower = SpriteSheetTerrain.None;
            terrainUpper = SpriteSheetTerrain.None;
        }
    }
}
