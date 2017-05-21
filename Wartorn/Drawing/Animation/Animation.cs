using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Wartorn.Drawing.Animation
{
    /// <summary>
    /// Contains the data needed to animate a series of frames defined in a texture
    /// </summary>
    public sealed class Animation : ICloneable
    {
        #region Fields

        //Used to identify specific animations
        private string name;

        //Will this animation play in a constant loop?
        private bool shouldLoop;

        //Lets us know if the animation has reached the last frame
        private bool isComplete;

        //How many frames a second we want to display
        private float framesPerSecond;

        //The amount of seconds that have to pass before we switch frames
        private float timePerFrame;

        //We add the elapsed time to this variable each frame
        private double totalElapsedTime;

        //Holds a value that points to an element in a list of frames
        private int currentFrame;

        //The frames of this animation
        private List<Frame> keyFrames;

        // Used for animations that do not loop to switch to other
        // animations when complete
        private string transitionKey;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public bool ShouldLoop
        {
            get { return shouldLoop; }
            set { shouldLoop = value; }
        }
        public float FramesPerSecond
        {
            get { return framesPerSecond; }
            set { framesPerSecond = value; }
        }
        public List<Frame> KeyFrames
        {
            get { return keyFrames; }
            set { keyFrames = value; }
        }
        public Frame CurntKeyFrame
        {
            get { return keyFrames[currentFrame]; }
        }
        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; }
        }
        public string TransitionKey
        {
            get { return transitionKey; }
        }

        #endregion

        #region Constructor

        public Animation()
        {
            name = string.Empty;
            shouldLoop = false;
            isComplete = false;
            framesPerSecond = 0;
            timePerFrame = 0;
            totalElapsedTime = 0;
            currentFrame = -1;
            keyFrames = new List<Frame>();
        }
        public Animation(string name, bool shouldLoop, float framesPerSecond, string transitionKey)
        {
            this.name = name;
            this.shouldLoop = shouldLoop;
            this.framesPerSecond = framesPerSecond;
            this.transitionKey = transitionKey;

            timePerFrame = 1.0f / framesPerSecond;
            keyFrames = new List<Frame>(60);
            isComplete = false;
            currentFrame = -1;
            totalElapsedTime = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method will make sure this animation starts fresh
        /// </summary>
        public void Reset()
        {
            currentFrame = 0;
            totalElapsedTime = 0;
            isComplete = false;
        }

        /// <summary>
        /// Adds a KeyFrame object to our list of KeyFrames
        /// </summary>
        /// <param name="x">The x coord in texture space</param>
        /// <param name="y">The y coord in texture space</param>
        /// <param name="width">The width in pixels of out source</param>
        /// <param name="height">The height in pixels of out source</param>
        public void AddKeyFrame(int x, int y, int width, int height)
        {
            Frame keyFrame = new Frame(x, y, width, height);
            keyFrames.Add(keyFrame);
        }
        public void AddKeyFrame(Rectangle source)
        {
            Frame keyFrame = new Frame(source.X, source.Y, source.Width, source.Height);
            keyFrames.Add(keyFrame);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the logic to advance through our list of frames
        /// </summary>
        /// <param name="gameTime">provides a snapshot of timing values</param>
        public void Update(GameTime gameTime)
        {
            totalElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            Frame keyFrame = keyFrames[currentFrame];

            if (totalElapsedTime >= timePerFrame)
            {
                if (currentFrame >= keyFrames.Count - 1)
                {
                    if (shouldLoop)
                    {
                        currentFrame = 0;
                        isComplete = false;
                    }
                    else
                    {
                        isComplete = true;
                    }
                }
                else
                {
                    currentFrame++;
                }

                //totalElapsedTime -= totalElapsedTime;
                totalElapsedTime = 0;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }


}
