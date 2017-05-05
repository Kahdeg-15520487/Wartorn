using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wartorn.GameData;
using Microsoft.Xna.Framework.Content;

namespace Wartorn.Drawing
{
    class MiniMapGenerator
    {
        ContentManager content;
        GraphicsDevice graphics;
        SpriteBatch spriteBatch;
        Texture2D tile;

        public MiniMapGenerator(GraphicsDevice gd, ContentManager cm, SpriteBatch sb)
        {
            spriteBatch = sb;
            graphics = gd;
            content = new ContentManager(cm.ServiceProvider, cm.RootDirectory);
            tile = content.Load<Texture2D>(@"sprite\blank8x8");
        }


        /// <summary>
        /// 
        /// mountain is yellow-orange
        /// plain is light-green
        /// tree is dark-green
        /// water is light-blue
        /// road is gray
        /// free building is white
        /// captured building will have color of the owner toned down a bit
        /// unit have color of the owner -> red,blue,green,yellow
        /// 
        /// </summary>
        /// <returns></returns>

        public Texture2D GenerateMapTexture(Map map)
        {
            RenderTarget2D minimap = new RenderTarget2D(graphics, 8 * map.Width, 8 * map.Height);
            graphics.SetRenderTarget(minimap);
            graphics.Clear(Color.CornflowerBlue);

            Color color = Color.White;

            spriteBatch.Begin();
            for (int x = 0; x < map.Width; x++)
                for (int y = 0; y < map.Height; y++)
                {
                    if (map[x, y].unit != null)
                    {
                        switch (map[x, y].unit.Owner)
                        {
                            case Owner.Red:
                                color = Color.Red;
                                break;
                            case Owner.Blue:
                                color = Color.Blue;
                                break;
                            case Owner.Green:
                                color = Color.Green;
                                break;
                            case Owner.Yellow:
                                color = Color.Yellow;
                                break;
                            default:
                                break;
                        }
                        goto draw;
                    }

                    switch (map[x, y].terrain)
                    {
                        case TerrainType.Reef:
                        case TerrainType.Sea:
                        case TerrainType.River:
                        case TerrainType.Coast:
                        case TerrainType.Cliff:
                            color = Color.LightBlue;
                            break;

                        case TerrainType.Road:
                        case TerrainType.Bridge:
                            color = Color.LightGray;
                            break;

                        case TerrainType.Plain:
                            color = Color.LightGreen;
                            break;

                        case TerrainType.Tree:
                            color = Color.DarkGreen;
                            break;

                        case TerrainType.Mountain:
                            color = Color.Yellow;
                            break;

                        case TerrainType.MissileSilo:
                        case TerrainType.MissileSiloLaunched:
                            color = Color.White;
                            break;

                        case TerrainType.City:
                        case TerrainType.Factory:
                        case TerrainType.AirPort:
                        case TerrainType.Harbor:
                        case TerrainType.Radar:
                        case TerrainType.SupplyBase:
                        case TerrainType.HQ:
                            switch (map[x, y].owner)
                            {
                                case Owner.None:
                                    color = Color.White;
                                    break;
                                case Owner.Red:
                                    color = Color.IndianRed;
                                    break;
                                case Owner.Blue:
                                    color = Color.CadetBlue;
                                    break;
                                case Owner.Green:
                                    color = Color.LawnGreen;
                                    break;
                                case Owner.Yellow:
                                    color = Color.YellowGreen;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                    draw:
                    spriteBatch.Draw(tile, new Vector2(x * 8, y * 8), color);
                }

            spriteBatch.End();
            graphics.SetRenderTarget(null);
            return minimap;
        }
    }
}
