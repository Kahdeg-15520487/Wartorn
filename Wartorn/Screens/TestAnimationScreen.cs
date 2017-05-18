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
using Wartorn.UIClass;
using Wartorn.SpriteRectangle;

namespace Wartorn.Screens
{
    public class TestAnimationScreen : Screen
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
        Point lastSelectedMapCell;

        Point selectedUnit = new Point(0, 0);
        List<Point> movementRange = null;

        Map map;
        Camera camera;

        UIClass.Console console;

        //information to animate a moving unit
        Graph dijkstarGraph;
        List<Point> movementPath;
        Point destination;
        bool isMovingUnitAnimPlaying = false;
        bool isMovePathCalculated = false;
        MovingUnitAnimation movingAnim;

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

            console = new UIClass.Console(new Point(0, 0), new Vector2(720, 200), CONTENT_MANAGER.hackfont);
            console.IsVisible = false;

            console.SetVariable("map", map);

            return base.Init();
        }

        public override void Update(GameTime gameTime)
        {
            mouseInputState = CONTENT_MANAGER.inputState.mouseState;
            lastMouseInputState = CONTENT_MANAGER.lastInputState.mouseState;
            keyboardInputState = CONTENT_MANAGER.inputState.keyboardState;
            lastKeyboardInputState = CONTENT_MANAGER.lastInputState.keyboardState;

            selectedMapCell = Utility.HelperFunction.TranslateMousePosToMapCellPos(mouseInputState.Position, camera, map.Width, map.Height);

            if (HelperFunction.IsKeyPress(Keys.OemTilde))
            {
                console.IsVisible = !console.IsVisible;
            }

            #region change unit and animation
            if (console.IsVisible) //suck all input in to the input box
            {
                console.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
            }
            else //accept input
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
                SelectUnit();
            }

            if (isMovingUnitAnimPlaying)
            {
                UpdateMovingUnit(gameTime);
            }

            //calculate movepath
            if (isMovePathCalculated)
            {
                if (movementRange.Contains(selectedMapCell) && selectedMapCell != lastSelectedMapCell)
                {
                    //update movement path
                    movementPath = DijkstraHelper.FindPath(dijkstarGraph, selectedMapCell);
                    lastSelectedMapCell = selectedMapCell;
                }
            }

            base.Update(gameTime);
        }

        private void SelectUnit()
        {
            MapCell temp = map[selectedMapCell];
            //check if there is a unit at selectedMapCell
            //and if said unit is not already selected
            //and if there is no animation going on
            if (temp.unit != null && !isMovingUnitAnimPlaying && selectedUnit != selectedMapCell)
            {
                //play sfx
                CONTENT_MANAGER.yes1.Play();

                selectedUnit = selectedMapCell;
                DisplayMovementRange(temp.unit, selectedUnit);
                isMovePathCalculated = true;
            }
            else
            {
                if (movementRange != null && movementRange.Contains(selectedMapCell))
                {
                    if (!isMovingUnitAnimPlaying)
                    {
                        //play sfx
                        CONTENT_MANAGER.moving_out.Play();

                        //we gonna move unit by moving a clone of it then teleport it to the destination
                        destination = selectedMapCell;
                        isMovingUnitAnimPlaying = true;

                        //create a new animation object
                        movingAnim = new MovingUnitAnimation(map[selectedUnit].unit,movementPath, new Point(selectedUnit.X * Constants.MapCellWidth, selectedUnit.Y * Constants.MapCellHeight));
                        
                        //ngung vẽ path
                        isMovePathCalculated = false;

                        //ngưng update animation cho unit gốc                        
                        map[selectedUnit].unit.Animation.StopAnimation();
                    }
                }
                else
                {
                    //bỏ lựa chọn unit sau khi đã chọn unit
                    DeselectUnit();
                }
            }
        }

        private void DeselectUnit()
        {
            movementRange = null;
            movementPath = null;
            isMovePathCalculated = false;
            isMovingUnitAnimPlaying = false;
            selectedUnit = default(Point);
            destination = default(Point);
        }

        private void UpdateMovingUnit(GameTime gameTime)
        {
            if (movingAnim.IsArrived)
            {
                //this line is only necessary for this screen
                position = destination;

                //normal stuff
                map[destination].unit = map[selectedUnit].unit;
                map[selectedUnit].unit = null;
                map[destination].unit.Animation.ContinueAnimation();
                DeselectUnit();
                return;
            }

            movingAnim.Update(gameTime);
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

            //draw moving anim
            //if (movingAnim != null && !movingAnim.IsArrived)
            //{
            //    CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, movingUnitPosition.toString(), new Vector2(500, 0), Color.White);

            //    movingUnit.Animation.Position = movingUnitPosition.ToVector2();
            //    movingUnit.Animation.Draw(gameTime, CONTENT_MANAGER.spriteBatch);
            //}

            if (console.IsVisible)
            {
                console.Draw(CONTENT_MANAGER.spriteBatch);
            }

            base.Draw(gameTime);
        }

        private void DrawMap(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);

            //render the map
            MapRenderer.Render(map, spriteBatch, gameTime);

            //draw cursor
            spriteBatch.Draw(CONTENT_MANAGER.selectCursor, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), null, Color.White, 0f, new Vector2(6, 6), 1f, SpriteEffects.None, LayerDepth.GuiUpper);

            //draw moving animation
            if (isMovingUnitAnimPlaying)
            {
                movingAnim.Draw(spriteBatch, gameTime);
            }

            //draw unit movement range
            DrawSelectedUnitMovementRange(spriteBatch);

            if (movementPath!=null && isMovePathCalculated)
            {
                DrawPathDirectionArrow(spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }

        private void DrawSelectedUnitMovementRange(SpriteBatch spriteBatch)
        {
            if (!isMovingUnitAnimPlaying && movementRange!=null)
            {
                foreach (Point dest in movementRange)
                {
                    spriteBatch.Draw(CONTENT_MANAGER.moveOverlay, new Vector2(dest.X * Constants.MapCellWidth, dest.Y * Constants.MapCellHeight), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
                }
            }
        }



        private void DrawPathDirectionArrow(SpriteBatch spriteBatch)
        {
            if (movementPath.Count<=2)
            {
                return;
            }

            List<Rectangle> rects = new List<Rectangle>();

            for (int i = 1; i < movementPath.Count-1; i++)
            {
                bool isVertical = true;
                Direction result = HelperFunction.GetIntersectionDir(movementPath[i - 1], movementPath[i], movementPath[i + 1]);

                switch (result)
                {
                    case Direction.South:
                        rects.Add(DirectionArrowSpriteSourceRectangle.GetSpriteRectangle( Direction.Center,true));
                        break;
                    case Direction.East:
                        rects.Add(DirectionArrowSpriteSourceRectangle.GetSpriteRectangle(Direction.Center, false));
                        break;
                    default:
                        rects.Add(DirectionArrowSpriteSourceRectangle.GetSpriteRectangle(result, false));
                        break;
                }                
            }
            rects.Add(DirectionArrowSpriteSourceRectangle.GetSpriteRectangle(movementPath[movementPath.Count - 2].GetDirectionFromPointAtoPointB(movementPath[movementPath.Count - 1])));

            //CONTENT_MANAGER.ShowMessageBox(lala.ToString());
            
            for(int i=1;i<movementPath.Count;i++)
            {
                spriteBatch.Draw(CONTENT_MANAGER.directionarrow, new Vector2(movementPath[i].X * Constants.MapCellWidth, movementPath[i].Y * Constants.MapCellHeight), rects[i - 1], Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiLower);
            }
        }
    }
}
