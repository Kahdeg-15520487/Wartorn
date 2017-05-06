using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Wartorn
{
    namespace UIClass
    {
        class Canvas :UIObject
        {
            Dictionary<string, UIObject> UIelements;

            public UIObject this[string uiname]
            {
                get
                {
                    return UIelements[uiname];
                }
            }

            public Canvas()
            {
                UIelements = new Dictionary<string, UIObject>();
            }

            public bool AddElement(string uiName, UIObject element)
            {
                if (!UIelements.ContainsKey(uiName))
                {
                    UIelements.Add(uiName, element);
                    return true;
                }
                else
                {
                    Utility.HelperFunction.Log(new Exception(uiName + " existed"));
                    return false;
                }
            }

            public UIObject GetElement(string uiName)
            {
                if (UIelements.ContainsKey(uiName))
                {
                    return UIelements[uiName];
                }
                else
                {
                    Utility.HelperFunction.Log(new Exception(uiName + " not found"));
                    return null;
                }
            }

            public T GetElementAs<T>(string uiName)
            {
                if (UIelements.ContainsKey(uiName))
                {
                    return (T)(object)UIelements[uiName];
                }
                else
                {
                    Utility.HelperFunction.Log(new Exception(uiName + " not found"));
                    return default(T);
                }
            }

            public void LoadContent()
            {
                //UIspritesheet = content.Load<Texture2D>("sprite\\UIspritesheet");
            }

            public override void Update(InputState inputState, InputState lastInputState)
            {
                if (!this.IsVisible)
                    return;

                foreach (var element in UIelements.Values)
                {
                    element.Update(inputState, lastInputState);
                }
                lastInputState = inputState;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                if (!this.IsVisible)
                    return;

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