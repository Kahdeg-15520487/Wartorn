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
    public static class TextRenderer
    {
        public static Texture2D RenderText(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, string text,SpriteFont font, Color textColor, Color bckgrdColor, float scale = 1f)
        {
            var textSize = font.MeasureString(text)*scale;
            RenderTarget2D result = new RenderTarget2D(graphicsDevice, (int)textSize.X, (int)textSize.Y);
            graphicsDevice.SetRenderTarget(result);
            graphicsDevice.Clear(bckgrdColor);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, Vector2.Zero, textColor);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
            return result;
        }
    }
}