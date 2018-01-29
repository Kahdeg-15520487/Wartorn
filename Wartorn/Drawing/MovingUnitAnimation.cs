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

namespace Wartorn.Drawing
{
    class MovingUnitAnimation
    {
        public bool IsArrived
        {
            get
            {
                return isArrived;
            }
        }

        Unit movingUnit = null;
        List<Point> movementPath = null;
        Point movingUnitPosition;
        int currentdest = 0;
        bool isArrived = false;
        float totalElapsedTime = 0;
        float delay = 25; //ms

        public MovingUnitAnimation(Unit unit,List<Point> movementPath,Point startingPoint)
        {
            movingUnit = UnitCreationHelper.Instantiate(unit.UnitType, unit.Owner);
            movingUnit.Animation.Depth = LayerDepth.Unit + 0.001f;
            this.movementPath = movementPath;
            movingUnitPosition = startingPoint;

        }

        public void Update(GameTime gameTime)
        {
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

        public void Draw(SpriteBatch spriteBatch,GameTime gameTime)
        {
            movingUnit.Animation.Position = movingUnitPosition.ToVector2();
            movingUnit.Animation.Draw(gameTime, CONTENT_MANAGER.spriteBatch);
        }
    }
}
