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
    class DirectionArrowRenderer
    {
        public List<Point> movementPath;
        public List<Rectangle> rects;
        
        public void UpdatePath(List<Point> movepath)
        {
            if (movepath.Count <= 1)
            {
                return;
            }
            movementPath = movepath;
            RenderPath();
        }

        private void RenderPath()
        {
            rects = new List<Rectangle>();

            for (int i = 1; i < movementPath.Count - 1; i++)
            {
                Direction result = HelperFunction.GetIntersectionDir(movementPath[i - 1], movementPath[i], movementPath[i + 1]);

                switch (result)
                {
                    case Direction.South:
                        rects.Add(DirectionArrowSpriteSourceRectangle.GetSpriteRectangle(Direction.Center, true));
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

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (movementPath == null || rects == null)
            {
                return;
            }

            for (int i = 1; i < movementPath.Count; i++)
            {
                spriteBatch.Draw(CONTENT_MANAGER.directionarrow, new Vector2(movementPath[i].X * Constants.MapCellWidth, movementPath[i].Y * Constants.MapCellHeight), rects[i - 1], Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiLower);
            }
        }
    }
}
