using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Text;

namespace Wartorn.Drawing.Animation
{
    public class AnimatedEntity : ICloneable
    {
        #region Fields

        // Holds all animations this entity can play
        private Dictionary<string, Animation> animations;

        // The animation we are currently playing
        private Animation currentAnimation;

        // The texture that contains all of our frames
        private Texture2D spriteSheet;

        // Positon of the sprite in our world
        private Vector2 position;

        // Tells SpriteBatch where to center our texture
        private Vector2 origin;

        // The rotation of our sprite
        private float rotation;

        // The scale of our sprite
        private float scale;

        private float depth;

        // Tells SpriteBatch how to flip our texture
        private SpriteEffects flipEffect;

        // Tells SpriteBatch what color to tint our texture with
        private Color tintColor;

        #endregion

        #region Properties

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public SpriteEffects FlipEffect
        {
            get { return flipEffect; }
            set { flipEffect = value; }
        }
        public string CurntAnimationName
        {
            get { return currentAnimation.Name; }
        }
        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        #endregion

        #region Constructor

        public AnimatedEntity()
        {
            animations = new Dictionary<string, Animation>(24);
            spriteSheet = null;
            position = Vector2.Zero;
            origin = Vector2.Zero;
            rotation = 0;
            scale = 1;
            depth = LayerDepth.BackGround;
            flipEffect = SpriteEffects.None;
            tintColor = Color.White;
        }
        public AnimatedEntity(Vector2 position,Vector2 origin, Color? tintColor, float depth, float scale = 1)
        {
            //Initialize the Dictionary
            animations = new Dictionary<string, Animation>(24);
            spriteSheet = null;
            this.origin = origin;
            rotation = 0;
            this.depth = depth;
            flipEffect = SpriteEffects.None;

            this.position = position;
            this.scale = scale;
            this.tintColor = tintColor ?? Color.White;

            //If the scale is less than 0 we wont see the texture drawn
            if (scale <= 0)
                scale = 0.1f;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Loads the sprite sheet texture
        /// </summary>
        /// <param name="content">ContentManager to load from</param>
        /// <param name="textureAssetName">The asset name of our texture</param>
        public void LoadContent(ContentManager content, string textureAssetName)
        {
            //Load our sprite sheet texture
            spriteSheet = content.Load<Texture2D>(textureAssetName);
        }
        public void LoadContent(Texture2D spriteSheet)
        {
            //Set our sprite sheet texture
            this.spriteSheet = spriteSheet;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Helper method to add Animations to this entity
        /// </summary>
        /// <param name="animation">The Animation we want to add</param>
        public void AddAnimation(Animation animation)
        {
            // Is this Animation already in the Dictionary?
            if (!animations.ContainsKey(animation.Name))
            {
                // If not we can safely add it
                animations.Add(animation.Name, animation);
            }
            else
            {
                // Otherwise we tell are computer to yell at us
                Utility.HelperFunction.Log(new ApplicationException("Animation Key is already contained in the Dictionary"));
            }
        }
        public void AddAnimation(params Animation[] anims)
        {
            foreach (Animation animation in anims)
            {
                // Is this Animation already in the Dictionary?
                if (!animations.ContainsKey(animation.Name))
                {
                    // If not we can safely add it
                    animations.Add(animation.Name, animation);
                }
                else
                {
                    // Otherwise we tell are computer to yell at us
                    Utility.HelperFunction.Log(new ApplicationException("Animation Key is already contained in the Dictionary"));
                }
            }
        }
        public void AddAnimation(List<Animation> anims)
        {
            foreach (Animation animation in anims)
            {
                // Is this Animation already in the Dictionary?
                if (!animations.ContainsKey(animation.Name))
                {
                    // If not we can safely add it
                    animations.Add(animation.Name, animation);
                }
                else
                {
                    // Otherwise we tell are computer to yell at us
                    Utility.HelperFunction.Log(new ApplicationException("Animation Key is already contained in the Dictionary"));
                }
            }
        }

        /// <summary>
        /// Tells this entity to play an specific animation
        /// </summary>
        /// <param name="key">The name of the animation you want to play</param>
        public void PlayAnimation(string key)
        {
            if (string.IsNullOrEmpty(key) || !animations.ContainsKey(key))
                return;

            if (currentAnimation != null)
            {
                if (currentAnimation.Name == key)
                {
                    return;
                }
            }

            currentAnimation = animations[key];
            currentAnimation.Reset();
        }

        #endregion

        #region Update

        /// <summary>
        /// We call this method to Update our animation each frame
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public void Update(GameTime gameTime)
        {
            if (currentAnimation != null)
            {
                //2 câu lệnh sau sẽ làm cho origin của animation ở chính giữa khung hình.
                //vì nguyên cái game chạy theo origin là vector2.zero nên bỏ
                //origin.X = currentAnimation.CurntKeyFrame.Width / 2;
                //origin.Y = currentAnimation.CurntKeyFrame.Height / 2;

                currentAnimation.Update(gameTime);

                if (currentAnimation.IsComplete)
                {
                    if (!string.IsNullOrEmpty(currentAnimation.TransitionKey))
                    {
                        PlayAnimation(currentAnimation.TransitionKey);
                    }
                }
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the AnimatedEntity
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        /// <param name="spriteBatch">The SpriteBatch object we will us to draw</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (currentAnimation != null)
            {
                spriteBatch.Draw(spriteSheet, position, currentAnimation.CurntKeyFrame.Source, tintColor,
                    rotation, origin, scale, flipEffect, depth);
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (string key in animations.Keys)
            {
                result.Append(key);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }
    }


}
