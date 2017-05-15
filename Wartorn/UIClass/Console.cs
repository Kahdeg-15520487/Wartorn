using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn.UIClass
{
    class Console : UIObject
    {
        public Label outputbox;
        public InputBox inputbox;
        private Vector2 relativePositionToMouse = Vector2.Zero;
        private List<string> log = new List<string>();

        public string Text
        {
            get
            {
                return inputbox.Text;
            }
            set
            {
                inputbox.Text = value;
            }
        }

        private int maxLogLine = 10;

        public Console(Point position, Vector2 size, SpriteFont font)
        {
            Position = position;
            Size = size;
            this.font = font;

            MouseDown += drag;

            var inputboxPosition = new Point(position.X, position.Y + (int)size.Y - 50);
            var inputboxSize = new Vector2(size.X, 50);
            var outputboxPosition = new Point(position.X,position.Y);
            var outputboxSize = new Vector2(size.X, size.Y - 50);

            inputbox = new InputBox("",inputboxPosition,inputboxSize,font, Color.White, Color.DarkGray);
            inputbox.caretColor = Color.LightGray;

            outputbox = new Label("", outputboxPosition, outputboxSize, font);
            outputbox.foregroundColor = Color.White;
            outputbox.backgroundColor = Color.LightGray;
            outputbox.Origin = outputboxPosition.ToVector2() + new Vector2(5, 5);
            maxLogLine = findMaxLogLine();

            inputbox.KeyPress += (sender, e) =>
            {
                if (e.keyboardState.IsKeyDown(Keys.Enter) && e.lastKeyboardState.IsKeyUp(Keys.Enter))
                {
                    OnCommandSubmitted(this, e);
                    log.Add(inputbox.Text);
                    inputbox.Clear();
                    outputbox.Text = log.Skip(Math.Max(0, log.Count - maxLogLine)).Aggregate((current, next) => current + "\n" + next);
                    return;
                }
            };

        }

        private int findMaxLogLine()
        {
            string teststr = "\n";
            while (font.MeasureString(teststr).Y < outputbox.rect.Height)
            {
                teststr += '\n';
            }
            return teststr.Length;
        }

        public override void Update(InputState inputState, InputState lastInputState)
        {
            inputbox.Update(inputState, lastInputState);
            outputbox.Update(inputState, lastInputState);
            base.Update(inputState, lastInputState);
        }

        public void drag(object sender,UIEventArgs e)
        {
            Position = (Position.ToVector2() + (e.mouseState.Position.ToVector2() - new Vector2(Position.X + Size.X/2,Position.Y + Size.Y/2))).ToPoint();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            inputbox.Draw(spriteBatch);
            outputbox.Draw(spriteBatch);
        }

        public event EventHandler<UIEventArgs> CommandSubmitted;

        protected virtual void OnCommandSubmitted(object sender,UIEventArgs e)
        {
            CommandSubmitted?.Invoke(sender, e);
        }
    }
}
