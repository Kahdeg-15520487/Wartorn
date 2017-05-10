using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Wartorn.ScreenManager;
using Wartorn.Drawing.Animation;
using Wartorn.Utility;
using Wartorn.GameData;
using Wartorn;

namespace Wartorn.Screens
{
    class TestAnimationScreen : Screen
    {
        Dictionary<UnitType, Unit> RedUnitList;
        Dictionary<UnitType, Unit> BlueUnitList;
        Dictionary<UnitType, Unit> GreenUnitList;

        UnitType currentUnit = UnitType.Soldier;
        AnimationName currentAnimation = AnimationName.idle;
        Owner currentColor = Owner.Red;

        public TestAnimationScreen(GraphicsDevice device) : base(device, "TestAnimationScreen")
        {
            LoadContent();
        }

        private void LoadContent()
        {
            RedUnitList = new Dictionary<UnitType, Unit>();
            BlueUnitList = new Dictionary<UnitType, Unit>();
            GreenUnitList = new Dictionary<UnitType, Unit>();
        }

        public override bool Init()
        {
            var UnitTypes = new List<UnitType>((IEnumerable<UnitType>)Enum.GetValues(typeof(UnitType)));
            UnitTypes.Remove(UnitType.None);

            foreach (UnitType unittype in UnitTypes)
            {
                Unit temp = UnitCreationHelper.Create(unittype, Owner.Red);
                RedUnitList.Add(unittype, temp);
                temp = UnitCreationHelper.Create(unittype, Owner.Blue);
                BlueUnitList.Add(unittype, temp);
                temp = UnitCreationHelper.Create(unittype, Owner.Green);
                GreenUnitList.Add(unittype, temp);
            }

            return base.Init();
        }

        public override void Update(GameTime gameTime)
        {
            //cylce through unit
            if (HelperFunction.IsKeyPress(Keys.Left))
            {
                if (currentUnit == UnitType.Soldier)
                {
                    currentUnit = UnitType.Battleship;
                }
                else
                {
                    currentUnit = currentUnit.Previous();
                }
            }

            if (HelperFunction.IsKeyPress(Keys.Right))
            {
                if (currentUnit == UnitType.Battleship)
                {
                    currentUnit = UnitType.Soldier;
                }
                else
                {
                    currentUnit = currentUnit.Next();
                }
            }

            //cycle through animation
            if (HelperFunction.IsKeyPress(Keys.Up))
            {
                if (currentAnimation == AnimationName.idle)
                {
                    currentAnimation = AnimationName.done;
                }
                else
                {
                    currentAnimation = currentAnimation.Previous();
                }
            }
            if (HelperFunction.IsKeyPress(Keys.Down))
            {
                if (currentAnimation == AnimationName.done)
                {
                    currentAnimation = AnimationName.idle;
                }
                else
                {
                    currentAnimation = currentAnimation.Next();
                }
            }

            //cycle through color
            if (HelperFunction.IsKeyPress(Keys.E))
            {
                if (currentColor == Owner.Green)
                {
                    currentColor = Owner.Red;
                }
                else
                {
                    currentColor = currentColor.Next();
                }
            }
            if (HelperFunction.IsKeyPress(Keys.Q))
            {
                if (currentColor == Owner.Red)
                {
                    currentColor = Owner.Green;
                }
                else
                {
                    currentColor = currentColor.Previous();
                }
            }

            switch (currentColor)
            {
                case Owner.Red:
                    if (RedUnitList[currentUnit].Animation.CurntAnimationName.CompareTo(currentAnimation.ToString()) != 0)
                    {
                        RedUnitList[currentUnit].Animation.PlayAnimation(currentAnimation.ToString());
                    }
                    RedUnitList[currentUnit].Animation.Update(gameTime);
                    break;
                case Owner.Blue:
                    if (BlueUnitList[currentUnit].Animation.CurntAnimationName.CompareTo(currentAnimation.ToString()) != 0)
                    {
                        BlueUnitList[currentUnit].Animation.PlayAnimation(currentAnimation.ToString());
                    }
                    BlueUnitList[currentUnit].Animation.Update(gameTime);
                    break;
                case Owner.Green:
                    if (GreenUnitList[currentUnit].Animation.CurntAnimationName.CompareTo(currentAnimation.ToString()) != 0)
                    {
                        GreenUnitList[currentUnit].Animation.PlayAnimation(currentAnimation.ToString());
                    }
                    GreenUnitList[currentUnit].Animation.Update(gameTime);
                    break;
                case Owner.Yellow:
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            switch (currentColor)
            {
                case Owner.Red:
                    RedUnitList[currentUnit].Animation.Draw(gameTime, CONTENT_MANAGER.spriteBatch);
                    break;
                case Owner.Blue:
                    BlueUnitList[currentUnit].Animation.Draw(gameTime, CONTENT_MANAGER.spriteBatch);
                    break;
                case Owner.Green:
                    GreenUnitList[currentUnit].Animation.Draw(gameTime, CONTENT_MANAGER.spriteBatch);
                    break;
                case Owner.Yellow:
                    break;
                default:
                    break;
            }

            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.arcadefont, currentColor.ToString() + Environment.NewLine + currentUnit.ToString() + Environment.NewLine + currentAnimation.ToString(), new Vector2(100, 0), Color.White);
            base.Draw(gameTime);
        }
    }
}
