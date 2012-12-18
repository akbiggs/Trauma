using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Trauma.Engine
{
    /// <summary>
    /// Collection of frames into an animation.
    /// </summary>
    public class AnimationSet
    {
        int animationTimer = 0;
        int animationFrame = 0;

        String name;
        Texture2D texture;
        internal int frames;
        int width;
        int frameDuration;

        /// <summary>
        /// Make a new animation set.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="texture">The texture from which the animation will be extracted.</param>
        /// <param name="frames">The number of frames in the animation.</param>
        /// <param name="width">The width of each frame.</param>
        /// <param name="frameDuration">How long each frame should last for.</param>
        public AnimationSet(String name, Texture2D texture, int frames, int width, int frameDuration)
        {
            this.name = name;
            this.texture = texture;
            this.frames = frames;
            this.width = width;
            this.frameDuration = frameDuration;
        }

        /// <summary>
        /// Update the animation.
        /// </summary>
        public void Update()
        {
            animationTimer++;
            if (animationTimer > frameDuration)
            {
                animationTimer = 0;
                animationFrame++;
                if (animationFrame >= frames)
                    animationFrame = 0;
            }

            if (animationFrame > frames)
            {
                animationFrame = 0;
            }

        }

        /// <summary>
        /// Whether or not the name of this set is the
        /// given name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>True if they are the same name, false otherwise.</returns>
        public bool IsCalled(String name)
        {
            return this.name == name;
        }

        /// <summary>
        /// The texture from which the animation is extracted.
        /// </summary>
        /// <returns>The texture of the animation.</returns>
        public Texture2D GetTexture()
        {
            return texture;
        }

        /// <summary>
        /// Returns a rectangle corresponding to the current frame.
        /// </summary>
        /// <returns>A rectangle of the same size as the frame.</returns>
        public Rectangle GetFrameRect()
        {
            return new Rectangle(animationFrame * width, 0, width, texture.Height);
        }
    }
}
