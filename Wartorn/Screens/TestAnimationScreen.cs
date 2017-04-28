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
        Dictionary<UnitType,AnimatedEntity> entityList;

        UnitType currentUnit = UnitType.Soldier;

        public TestAnimationScreen(GraphicsDevice device) : base(device, "TestAnimationScreen")
        {
            LoadContent();
        }

        private void LoadContent()
        {

            entityList = new Dictionary<UnitType, AnimatedEntity>();
        }

        public override bool Init()
        {
            var UnitTypes = new List<UnitType>((IEnumerable<UnitType>)Enum.GetValues(typeof(UnitType)));
            UnitTypes.Remove(UnitType.None);

            foreach (UnitType unittype in UnitTypes)
            {
                AnimatedEntity temp = new AnimatedEntity(new Vector2(24, 24), null, LayerDepth.Unit);
                temp.LoadContent(CONTENT_MANAGER.animationSheets[unittype]);

                temp.AddAnimation(CONTENT_MANAGER.animationTypes);


                temp.PlayAnimation("idle");

                entityList.Add(unittype, temp);
            }

            return base.Init();
        }

        public override void Update(GameTime gameTime)
        {
            if (HelperFunction.IsKeyPress(Keys.Up))
            {
                if (currentUnit == UnitType.Missile)
                {
                    currentUnit = UnitType.Soldier;
                }
                else
                {
                    currentUnit = currentUnit.Next();
                }
            }
            if (currentUnit == UnitType.None)
            {
                currentUnit = UnitType.Soldier;
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
