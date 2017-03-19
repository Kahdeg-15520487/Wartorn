using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Text;

namespace Wartorn
{
    namespace UIClass
    {
        class Canvas :UIObject
        {
            Dictionary<string, UIObject> UIelements;

            public Canvas()
            {
                UIelements = new Dictionary<string, UIObject>();
            }

            public bool AddElement(string uiName, UIObject element)
            {
                if (!UIelements.ContainsKey(uiName))
                {
                    UIelements.Add(uiName, element);
                    if (element.font == null && element.fontname == "defaultfont")
                    {
                        element.font = this.font;
                    }
                    return true;
                }
                else return false;
            }

            public UIObject GetElement(string uiName)
            {
                if (UIelements.ContainsKey(uiName))
                {
                    return UIelements[uiName];
                }
                else return null;
            }

            public void LoadContent(ContentManager content)
            {
                //UIspritesheet = content.Load<Texture2D>("sprite\\UIspritesheet");
            }

            public override void Update(InputState inputState, InputState lastInputState)
            {
                foreach (var element in UIelements.Values)
                {
                    element.Update(inputState, lastInputState);
                }
                lastInputState = inputState;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                foreach (var element in UIelements.Values)
                {
                    if (element.IsVisible)
                    {
                        element.Draw(spriteBatch);
                    }
                }
            }
        }
    }
}