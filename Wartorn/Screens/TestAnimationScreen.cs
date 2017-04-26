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

namespace Wartorn.Screens
{
    class TestAnimationScreen : Screen
    {
        Dictionary<Unit,AnimatedEntity> entityList;
        Dictionary<Unit,Texture2D> animationSpriteSheet;

        Unit currentUnit = Unit.Soldier;

        public TestAnimationScreen(GraphicsDevice device) : base(device, "TestAnimationScreen")
        {
            LoadContent();
        }

        private void LoadContent()
        {
            animationSpriteSheet = new Dictionary<Unit, Texture2D>();
            var UnitTypes = new List<Unit>((IEnumerable<Unit>)Enum.GetValues(typeof(Unit)));
            UnitTypes.Remove(Unit.None);

            foreach (Unit unittype in UnitTypes)
            {
                animationSpriteSheet.Add(unittype, CONTENT_MANAGER.Content.Load<Texture2D>("sprite//Alliance_RED//" + unittype.ToString()));
            }

            entityList = new Dictionary<Unit, AnimatedEntity>();
        }

        public override bool Init()
        {
            var UnitTypes = new List<Unit>((IEnumerable<Unit>)Enum.GetValues(typeof(Unit)));
            UnitTypes.Remove(Unit.None);

            foreach (Unit unittype in UnitTypes)
            {
                AnimatedEntity temp = new AnimatedEntity(new Vector2(24, 24), null);
                temp.LoadContent(animationSpriteSheet[unittype]);

                Animation idle = new Animation("idle", true, 4, "right");
                for (int i = 0; i < 4; i++)
                {
                    idle.AddKeyFrame(i * 48, 0, 48, 48);
                }

                Animation right = new Animation("right", true, 4, "up");
                for (int i = 0; i < 4; i++)
                {
                    right.AddKeyFrame(i * 48, 48, 48, 48);
                }

                Animation up = new Animation("up", true, 4, "down");
                for (int i = 0; i < 4; i++)
                {
                    up.AddKeyFrame(i * 48, 96, 48, 48);
                }

                Animation down = new Animation("down", true, 4, "done");
                for (int i = 0; i < 4; i++)
                {
                    down.AddKeyFrame(i * 48, 144, 48, 48);
                }

                Animation done = new Animation("done", true, 1, "idle");
                done.AddKeyFrame(0, 192, 48, 48);

                temp.AddAnimation(idle);
                temp.AddAnimation(right);
                temp.AddAnimation(up);
                temp.AddAnimation(down);
                temp.AddAnimation(done);
                temp.PlayAnimation("idle");

                entityList.Add(unittype, temp);
            }

            return base.Init();
        }

        public override void Update(GameTime gameTime)
        {
            if (HelperFunction.IsKeyPress(Keys.Up))
            {
                if (currentUnit == Unit.Missile)
                {
                    currentUnit = Unit.Soldier;
                }
                else
                {
                    currentUnit = currentUnit.Next();
                }
            }

            entityList[currentUnit].Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            entityList[currentUnit].Draw(gameTime, CONTENT_MANAGER.spriteBatch);
            base.Draw(gameTime);
        }
    }
}
