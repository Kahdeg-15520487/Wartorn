using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wartorn.GameData;
using Wartorn.Drawing.Animation;

namespace Wartorn.Drawing
{
    static class MapRenderer
    {
        //TODO do the maprenderer class
        public static void Render(Map map,SpriteBatch spriteBatch,GameTime gameTime)
        {
            MapCell tempmapcell;
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    Vector2 curpos = new Vector2(i * Constants.MapCellWidth, j * Constants.MapCellHeight);
                    tempmapcell = map[i, j];
                    if (tempmapcell.terrainbase != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainbase), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainBase);
                    }
                    if (tempmapcell.terrainLower != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainLower), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainLower);
                    }
                    if (tempmapcell.terrainUpper != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, curpos, SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainUpper), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainUpper);
                    }

                    if (tempmapcell.unit!=null)
                    {
                        var tempunit = tempmapcell.unit;
                        tempunit.Animation.Position = curpos;
                        tempunit.Animation.Draw(gameTime,spriteBatch);
                    }
                }
            }
        }
    }
}
