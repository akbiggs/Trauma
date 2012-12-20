using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;

namespace Trauma.Objects
{
    /// <summary>
    /// Any object that should abide by the game's laws and physics.
    /// </summary>
    public class GameObject
    {
        #region Members
        
        private Vector2 position;
        private Vector2 minPosition;
        private Vector2 maxPosition;

        private Vector2 velocity;
        private Vector2 maxVelocity;

        private Vector2 acceleration;
        private Vector2 decceleration;

        private Color color;
        private bool colorable;

        private Vector2 size;

        private Texture2D frame;
        private AnimationSet curAnimation;
        private List<AnimationSet> animations = new List<AnimationSet>();

        private float rotation;

        private BBox bbox;

        #endregion

        public GameObject(Vector2 position, Vector2 minPosition, Vector2 maxPosition, Vector2 velocity, Vector2 maxVelocity, Vector2 acceleration, Vector2 decceleration, Color color, bool colorable, Vector2 size, Texture2D frame, AnimationSet startAnimation, float rotation)
        {
            this.position = position;
            this.minPosition = minPosition;
            this.maxPosition = maxPosition;
            this.velocity = velocity;
            this.maxVelocity = maxVelocity;
            this.acceleration = acceleration;
            this.decceleration = decceleration;
            this.color = color;
            this.colorable = colorable;
            this.size = size;
            this.frame = frame;
            this.curAnimation = startAnimation;
            this.rotation = rotation;

            this.bbox = new BBox(new Rectangle((int)position.X, (int)position.Y, size.X, size.Y));
        }
    }
}
