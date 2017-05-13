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
using Wartorn.Drawing;

namespace Wartorn.Screens
{
    class TestAnimationScreen : Screen
    {
        //Dictionary<UnitType, Unit> RedUnitList;
        //Dictionary<UnitType, Unit> BlueUnitList;
        //Dictionary<UnitType, Unit> GreenUnitList;
        //Dictionary<UnitType, Unit> YellowUnitList;

        UnitType currentUnit = UnitType.Soldier;
        AnimationName currentAnimation = AnimationName.idle;
        Owner currentColor = Owner.Red;
        Point position = new Point(4, 4);

        Map map;
        Camera camera;

        public TestAnimationScreen(GraphicsDevice device) : base(device, "TestAnimationScreen")
        {   }

        public override bool Init()
        {
            camera = new Camera(_device.Viewport);
            map = new Map(9, 9);
            map.Fill(TerrainType.Plain);

            map[position].unit = UnitCreationHelper.Create(currentUnit, currentColor);
            map[position].unit.Animation.PlayAnimation(AnimationName.idle.ToString());

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
                if (currentColor == Owner.Yellow)
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
                    currentColor = Owner.Yellow;
                }
                else
                {
                    currentColor = currentColor.Previous();
                }
            }

            Unit unit = map[position].unit;
            UnitType nextUnit = unit.UnitType;
            Owner nextOwner = unit.Owner;
            AnimationName nextAnimation = unit.Animation.CurntAnimationName.ToEnum<AnimationName>();
            bool isChanged = false;

            if (nextUnit != currentUnit)
            {
                isChanged = true;
            }

            if (nextOwner != currentColor)
            {
                isChanged = true;
            }

            if (nextAnimation != currentAnimation)
            {
                isChanged = true;
            }

            if (isChanged)
            {
                map[position].unit = UnitCreationHelper.Create(currentUnit, currentColor, animation: currentAnimation);
            }

            map[position].unit.Animation.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawMap(CONTENT_MANAGER.spriteBatch, gameTime);

            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.arcadefont, currentColor.ToString() + Environment.NewLine + currentUnit.ToString() + Environment.NewLine + currentAnimation.ToString(), new Vector2(100, 0), Color.White);
            base.Draw(gameTime);
        }

        private void DrawMap(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);

            //render the map
            MapRenderer.Render(map, spriteBatch, gameTime);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }
    }
}
