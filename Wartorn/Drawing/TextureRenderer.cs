using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.CustomJsonConverter;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Wartorn.Drawing
{
    public static class TextureRenderer
    {
        public static Texture2D Render(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Vector2 size, float scale = 1f,params Tuple<Vector2,Texture2D>[] textureList)
        {
            RenderTarget2D result = new RenderTarget2D(graphicsDevice, (int)size.X, (int)size.Y);
            graphicsDevice.SetRenderTarget(result);
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            foreach (var sprite in textureList)
            {
                spriteBatch.Draw(sprite.Item2, sprite.Item1, Color.White);
            }

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
            return result;
        }
    }
}