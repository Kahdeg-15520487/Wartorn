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
using Wartorn.Utility;

using Wartorn.CustomJsonConverter;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wartorn.UIClass
{
    enum ButtonContentType
    {
        Text,
        SpriteFromSheet,
        Texture2D
    }

    public class Button : UIObject
    {
        //usedonly for debug purpose, will draw the internal rect 
        //that is used to determine event on screen
        public bool isDrawRect = false;


        private bool isPressed = false;
        private Rectangle internalRect;
        private ButtonContentType contentType;
        private Rectangle? _SpriteSourceRectangle = null;

        //Add region to draw text
        private Vector2 stringRect;
        //
        public bool AutoOffset { get; set; }

        public Rectangle SpriteSourceRectangle
        {
            get
            {
                return _SpriteSourceRectangle.GetValueOrDefault();
            }
            set
            {
                _SpriteSourceRectangle = value;
                rect.Size = (value.X > 0 && value.Y > 0) ? value.Size : rect.Size;
            }
        }

        Texture2D sprite;
        bool isFromUISpriteSheet;
        public Texture2D Sprite { set { sprite = value; contentType = ButtonContentType.Texture2D; } get { return sprite; } }

        protected string text;
        public virtual string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = string.IsNullOrEmpty(value) ? text : value;
            }
        }

        public override Point Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                internalRect.Location = new Point(rect.Location.X + 1, rect.Location.Y + 1);
                internalRect.Size = new Point(rect.Size.X - 1, rect.Size.Y - 1);
            }
        }
        public override Vector2 Size
        {
            get
            {
                return rect.Size.ToVector2();
            }
            set
            {
                rect.Size = (value.X > 0 && value.Y > 0) ? value.ToPoint() : sprite.Bounds.Size;
                internalRect.Size = new Point(rect.Size.X - 1, rect.Size.Y - 1);
            }
        }

        public float Depth { get; set; } = LayerDepth.GuiUpper;

        Color buttonColorPressed = Color.LightSlateGray;
        Color buttonColorReleased = Color.White;
        public Color ButtonColorPressed
        {
            get
            {
                return buttonColorPressed;
            }
            set
            {
                buttonColorPressed = value;
            }
        }
        public Color ButtonColorReleased
        {
            get
            {
                return buttonColorReleased;
            }
            set
            {
                buttonColorReleased = value;
            }
        }

        /// <summary>
        /// Region to draw text
        /// </summary>
        public Vector2 StringRect
        {
            get
            {
                return stringRect;
            }

            set
            {
                stringRect = value;
            }
        }

        public Vector2 TextOffset { get; set; }

        public Button()
        {
            Init();
        }

        /// <summary>
        /// Text button
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="position">Position of the top left corner</param>
        /// <param name="size">Size of the button</param>
        /// <param name="font">Font to use</param>
        ///
        public Button(string text, Point position, Vector2 size, SpriteFont font, Vector2? offset = null, bool autoOffset = false)
        {
            contentType = ButtonContentType.Text;
            Text = text;
            Position = position;
            Size = size;
            this.Font = font;
            TextOffset = offset == null ? position.ToVector2() + size / 4 : offset.Value;
            AutoOffset = autoOffset;
            if (autoOffset)
            {
                CalculateSize(font.MeasureString(text));
            }

            Init();
        }

        private void CalculateSize(Vector2 size)
        {
            //Region to draw text
            StringRect = new Vector2(Position.X + size.X / 4, Position.Y + size.Y / 4);
        }

        public Button(Texture2D sprite, Rectangle? spriterect, Point position, float scale = 1)
        {
            contentType = ButtonContentType.Texture2D;
            _SpriteSourceRectangle = spriterect;
            this.sprite = sprite;
            Position = position;
            Size = _SpriteSourceRectangle.GetValueOrDefault().Size.ToVector2();
            Scale = scale;
            Init();
        }

        private void Init()
        {
            MouseClick += (sender, e) =>
            {

            };
            MouseDown += (sender, e) =>
            {
                isPressed = true;
            };
            MouseUp += (sender, e) =>
            {
                isPressed = false;
            };
            MouseLeave += (sender, e) =>
            {
                isPressed = false;
            };
        }

        public override void Update(GameTime gameTime, InputState inputState, InputState lastInputState)
        {
            base.Update(gameTime, inputState, lastInputState);
            if (isPressed)
            {
                OnButtonPressed(this, new UIEventArgs(inputState, lastInputState));
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (isDrawRect)
            {
                DrawingHelper.DrawRectangle(rect, Color.Gold, false);
            }

            switch (contentType)
            {
                case ButtonContentType.Text:
                    spriteBatch.DrawString(Font ?? CONTENT_MANAGER.Fonts["defaultFont"], (string.IsNullOrEmpty(text)) ? "" : text, Position.ToVector2() + Size / 4, ForegroundColor, Rotation, origin, scale, SpriteEffects.None, Depth);

                    DrawingHelper.DrawRectangle(internalRect, isPressed ? buttonColorPressed : buttonColorReleased, true);
                    DrawingHelper.DrawRectangle(rect, BorderColor, false);
                    break;
                case ButtonContentType.SpriteFromSheet:
                    //spriteBatch.Draw(isFromUISpriteSheet ? CONTENT_MANAGER.Sprites["UIspriteSheet"] : CONTENT_MANAGER.Sprites["spriteSheet"], Position.ToVector2(), SpriteSourceRectangle, isPressed ? buttonColorPressed : buttonColorReleased, Rotation, Vector2.Zero, Scale, SpriteEffects.None, Depth);
                    break;
                case ButtonContentType.Texture2D:
                    spriteBatch.Draw(sprite, Position.ToVector2(), _SpriteSourceRectangle, isPressed ? buttonColorPressed : buttonColorReleased, Rotation, Vector2.Zero, Scale, SpriteEffects.None, Depth);
                    break;
                default:
                    break;
            }
        }

        public event EventHandler<UIEventArgs> ButtonPressed;
        protected virtual void OnButtonPressed(object sender, UIEventArgs e)
        {
            ButtonPressed?.Invoke(sender, e);
        }
    }
}