using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Drawing.Animation;

namespace Utilities.UI
{
    public class AnimationPlayer : UIObject
    {
        AnimatedEntity AnimatedEntity = null;

        /// <summary>
        /// Animation Player for playing AnimatedEntity in a UI context
        /// </summary>
        /// <param name="animatedEntity">can be null to indicate the animation is not loaded yet</param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        public AnimationPlayer(AnimatedEntity animatedEntity,Point position,float scale = 1f)
        {
            AnimatedEntity = animatedEntity;
            AnimatedEntity.Position = position.ToVector2();
            Position = position;
            Scale = scale;
        }

        public void AddAnimation(Animation animation)
        {
            AnimatedEntity?.AddAnimation(animation);
        }

        public bool ContainAnimation(string animationName)
        {
            return AnimatedEntity == null ? false : AnimatedEntity.ContainAnimation(animationName);
        }

        public void RemoveAnimation(string animationName)
        {
            AnimatedEntity?.RemoveAnimation(animationName);
        }

        public void PlayAnimation(string animationName)
        {
            AnimatedEntity?.PlayAnimation(animationName);
        }

        public void StopAnimation()
        {
            AnimatedEntity?.StopAnimation();
        }

        public void ContinueAnimation()
        {
            AnimatedEntity?.ContinueAnimation();
        }

        public void Flip(SpriteEffects flip)
        {
            if (AnimatedEntity != null)
            {
                AnimatedEntity.FlipEffect = flip;
            }
        }

        public void LoadContent(Texture2D spritesheet)
        {
            AnimatedEntity.LoadContent(spritesheet);
        }

        public string CurrentAnimationName { get { return AnimatedEntity?.CurntAnimationName; } }
        public bool IsPlaying { get { return AnimatedEntity != null ? AnimatedEntity.IsPlaying : false; } }

        public override void Update(GameTime gameTime, InputState currentInputState, InputState lastInputState)
        {
            base.Update(gameTime, currentInputState, lastInputState);

            AnimatedEntity?.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            AnimatedEntity?.Draw(gameTime, spriteBatch);
        }
    }
}