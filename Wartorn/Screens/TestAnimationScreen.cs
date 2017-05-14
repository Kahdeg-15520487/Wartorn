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
using Wartorn.PathFinding.Dijkstras;
using Wartorn.PathFinding;

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

        MouseState mouseInputState;
        MouseState lastMouseInputState;
        KeyboardState keyboardInputState;
        KeyboardState lastKeyboardInputState;
        Point selectedMapCell;

        Point selectedUnit = new Point(0, 0);
        List<Point> movementRange = null;

        Map map;
        Camera camera;

        public TestAnimationScreen(GraphicsDevice device) : base(device, "TestAnimationScreen")
        {   }

        public override bool Init()
        {
            camera = new Camera(_device.Viewport);

            map = new Map(9, 9);
            map.Fill(TerrainType.Plain);

            map[3, 3].terrain = TerrainType.Mountain;
            map[4, 3].terrain = TerrainType.Mountain;
            map[3, 4].terrain = TerrainType.Mountain;
            map[3, 5].terrain = TerrainType.Mountain;
            map[3, 6].terrain = TerrainType.Mountain;

            map[5, 3].terrain = TerrainType.Sea;
            map[6, 3].terrain = TerrainType.Sea;
            map[6, 4].terrain = TerrainType.River;
            map[6, 5].terrain = TerrainType.River;

            map[5, 6].terrain = TerrainType.Road;
            map[6, 6].terrain = TerrainType.Road;
            map[7, 6].terrain = TerrainType.Road;
            map[7, 5].terrain = TerrainType.Road;
            map[7, 4].terrain = TerrainType.Road;
            map[7, 3].terrain = TerrainType.Road;

            map.GenerateNavigationMap();

            map[position].unit = UnitCreationHelper.Create(currentUnit, currentColor);
            map[position].unit.Animation.PlayAnimation(AnimationName.idle.ToString());

            return base.Init();
        }

        public override void Update(GameTime gameTime)
        {
            mouseInputState = CONTENT_MANAGER.inputState.mouseState;
            lastMouseInputState = CONTENT_MANAGER.lastInputState.mouseState;
            keyboardInputState = CONTENT_MANAGER.inputState.keyboardState;
            lastKeyboardInputState = CONTENT_MANAGER.lastInputState.keyboardState;

            selectedMapCell = Utility.HelperFunction.TranslateMousePosToMapCellPos(mouseInputState.Position, camera, map.Width, map.Height);

            #region change unit and animation
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
            #endregion

            if (mouseInputState.LeftButton == ButtonState.Released
             && lastMouseInputState.LeftButton == ButtonState.Pressed)
            {
                UpdateUnit();
            }

            if (movingUnit!=null)
            {
                UpdateMovingUnit(gameTime);
            }

            base.Update(gameTime);
        }

        Unit movingUnit = null;
        Graph dijkstarGraph = null;
        List<Point> movementPath = null;
        Point movingUnitPosition;
        Point destination;
        int currentdest = 0;
        bool isArrived = false;

        private void UpdateUnit()
        {
            MapCell temp = map[selectedMapCell];
            if (temp.unit != null)
            {
                CONTENT_MANAGER.yes1.Play();
                selectedUnit = selectedMapCell;
                DisplayMovementRange(temp.unit, selectedUnit);
            }
            else
            {
                if ( movementRange!=null && movementRange.Contains(selectedMapCell))
                {
                    //we gonna move unit by moving a image of it then teleport it to the destination
                    CONTENT_MANAGER.moving_out.Play();
                    movementRange = null;
                    destination = selectedMapCell;
                    movingUnit = map[selectedUnit].unit;
                    movingUnit = UnitCreationHelper.Create(movingUnit.UnitType, movingUnit.Owner);
                    movementPath = DijkstraHelper.FindPath(dijkstarGraph, destination);

                    currentdest = 0;
                    movingUnitPosition = new Point(selectedUnit.X * Constants.MapCellWidth, selectedUnit.Y * Constants.MapCellHeight);
                    isArrived = false;
                    map[selectedUnit].unit.Animation.StopAnimation();
                }
                else
                {
                    movementRange = null;
                }
            }
        }

        float totalElapsedTime = 0;
        float delay = 50; //ms
        private void UpdateMovingUnit(GameTime gameTime)
        {
            if (isArrived)
            {
                movingUnit = null;
                position = destination;
                map[destination].unit = map[selectedUnit].unit;
                map[selectedUnit].unit = null;
                map[destination].unit.Animation.ContinueAnimation();
                return;
            }

            totalElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            AnimationName anim = movingUnit.Animation.CurntAnimationName.ToEnum<AnimationName>();
            bool isLeft = movingUnit.Animation.FlipEffect == SpriteEffects.FlipHorizontally ? true : false;

            if (totalElapsedTime >= delay)
            {
                if (currentdest < movementPath.Count)
                {
                    int currentdestX = movementPath[currentdest].X * Constants.MapCellWidth;
                    int currentdestY = movementPath[currentdest].Y * Constants.MapCellHeight;

                    if (movingUnitPosition.X < currentdestX)
                    {
                        //to the right
                        anim = AnimationName.right;
                        movingUnitPosition.X += 12;
                    }
                    else
                    {
                        if (movingUnitPosition.X > currentdestX)
                        {
                            //to the left
                            anim = AnimationName.right;
                            movingUnitPosition.X -= 12;
                            isLeft = true;
                        }
                    }

                    if (movingUnitPosition.Y < currentdestY)
                    {
                        //down
                        movingUnitPosition.Y += 12;
                        anim = AnimationName.down;
                    }
                    else
                    {
                        if (movingUnitPosition.Y > currentdestY)
                        {
                            //up
                            movingUnitPosition.Y -= 12;
                            anim = AnimationName.up;
                        }
                    }

                    if (movingUnitPosition.X == currentdestX
                     && movingUnitPosition.Y == currentdestY)
                    {
                        currentdest++;
                    }
                }
                else
                {
                    isArrived = true;
                    currentdest = 0;
                }

                totalElapsedTime -= totalElapsedTime;
            }


            movingUnit.Animation.PlayAnimation(anim.ToString());
            movingUnit.Animation.FlipEffect = isLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            movingUnit.Animation.Update(gameTime);
        }

        private void DisplayMovementRange(Unit unit, Point position)
        {
            dijkstarGraph = DijkstraHelper.CalculateGraph(map, unit, position);
            movementRange = DijkstraHelper.FindRange(dijkstarGraph);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawMap(CONTENT_MANAGER.spriteBatch, gameTime);

            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.arcadefont, currentColor.ToString() + Environment.NewLine + currentUnit.ToString() + Environment.NewLine + currentAnimation.ToString(), new Vector2(100, 0), Color.White);

            if (movingUnit != null && !isArrived)
            {
                CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, movingUnitPosition.toString() /* + Environment.NewLine + (new Point(movementPath[currentdest].X * Constants.MapCellWidth, movementPath[currentdest].Y * Constants.MapCellHeight)).toString()*/, new Vector2(500, 0), Color.White);

                movingUnit.Animation.Position = movingUnitPosition.ToVector2();
                movingUnit.Animation.Draw(gameTime, CONTENT_MANAGER.spriteBatch);
            }

            base.Draw(gameTime);
        }

        private void DrawMap(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);

            //render the map
            MapRenderer.Render(map, spriteBatch, gameTime);
            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), new Rectangle(0, 0, 48, 48), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            DrawSelectedUnit(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }

        private void DrawSelectedUnit(SpriteBatch spriteBatch)
        {
            if (movementRange != null)
            {
                foreach (Point dest in movementRange)
                {
                    spriteBatch.Draw(CONTENT_MANAGER.moveOverlay, new Vector2(dest.X * Constants.MapCellWidth, dest.Y * Constants.MapCellHeight), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
                }
            }
        }
    }
}
