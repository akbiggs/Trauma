using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;
using Trauma.Rooms;
using Trauma.Helpers;

namespace Trauma.Objects
{
    /// <summary>
    ///     Any object that should abide by the game's laws and physics.
    /// </summary>
    public class GameObject
    {
        #region Members

        private readonly List<AnimationSet> animations;
        protected Color color;
        protected Vector2 position;
        private float rotation;
        
        private Vector2 acceleration;
        private BBox bbox;

        private bool colorable;
        private AnimationSet curAnimation;
        private Vector2 deceleration;

        private Vector2 maxPosition;
        private Vector2 maxSpeed;
        private Vector2 minPosition;
        protected Vector2 size;
        protected Vector2 Velocity;

        private bool hasMoved;
        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameObject" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="initialVelocity">The initial velocity.</param>
        /// <param name="maxSpeed">The max speed.</param>
        /// <param name="acceleration">The acceleration.</param>
        /// <param name="deceleration">The deceleration.</param>
        /// <param name="color">The color.</param>
        /// <param name="colorable">
        ///     if set to <c>true</c> [colorable].
        /// </param>
        /// <param name="size">The size.</param>
        /// <param name="animations">The animations.</param>
        /// <param name="startAnimationName">Start name of the animation.</param>
        /// <param name="rotation">The rotation.</param>
        public GameObject(Vector2 position, Vector2 initialVelocity, Vector2 maxSpeed, Vector2 acceleration,
                          Vector2 deceleration, Color color, bool colorable, Vector2 size, List<AnimationSet> animations,
                          String startAnimationName, float rotation)
        {
            this.position = position;
            Velocity = initialVelocity;
            this.maxSpeed = maxSpeed;
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            this.color = color;
            this.colorable = colorable;
            this.size = size;

            this.animations = animations;
            curAnimation = GetAnimationByName(startAnimationName);
            Debug.Assert(curAnimation != null, "Couldn't find the starting animation.");

            this.rotation = rotation;

            bbox = new BBox(new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y));
        }

        /// <summary>
        ///     Initializes a new unanimated instance of the <see cref="GameObject" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="initialVelocity">The initial velocity.</param>
        /// <param name="maxSpeed">The max speed.</param>
        /// <param name="acceleration">The acceleration.</param>
        /// <param name="deceleration">The deceleration.</param>
        /// <param name="color">The color.</param>
        /// <param name="colorable">
        ///     if set to <c>true</c> [object's color can be changed].
        /// </param>
        /// <param name="size">The size.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="rotation">The rotation.</param>
        public GameObject(Vector2 position, Vector2 initialVelocity, Vector2 maxSpeed, Vector2 acceleration,
                          Vector2 deceleration, Color color, bool colorable, Vector2 size, Texture2D texture,
                          float rotation)
            : this(position, initialVelocity, maxSpeed, acceleration, deceleration, color, colorable, size,
                   new List<AnimationSet>
                       {
                           new AnimationSet("texture", texture, 1, texture.Width, 1)
                       }, "texture", rotation)
        {
        }

        /// <summary>
        ///     Update the object.
        /// </summary>
        /// <param name="gameTime">The current time of the game.</param>
        public virtual void Update(Room room, GameTime gameTime)
        {
            curAnimation.Update(gameTime);
        }

        /// <summary>
        /// Moves in the specified direction.
        /// </summary>
        /// <param name="room">The room that this object is moving around in.</param>
        /// <param name="direction">The direction to move in.</param>
        internal virtual void Move(Room room, Vector2 direction)
        {
            if (direction != Vector2.Zero)
                direction.Normalize();

            ApplyGravity(room);
            Accelerate(direction);

            // TODO: Figure out a way to resolve collisions and do only one collide with wall call.

            // resolve collisions on x-axis first
            UpdateBounds(room);
            ChangeXPosition(room);

            // then resolve any remaining on the y-axis
            UpdateBounds(room);
            ChangeYPosition(room);
        }

        /// <summary>
        /// Collides with a wall.
        /// </summary>
        /// <param name="room">The room containing the wall.</param>
        public virtual void CollideWithWall(Room room)
        {
            
        }

        /// <summary>
        /// Collides with the ground. Should be called alongside CollideWithWall.
        /// </summary>
        /// <param name="room">The room containing the ground.</param>
        public virtual void CollideWithGround(Room room)
        {
            
        }

        /// <summary>
        /// Applies gravity to the object.
        /// </summary>
        /// <param name="room">The room that the object is in.</param>
        private void ApplyGravity(Room room)
        {
            Velocity.Y += room.Gravity;
        }

        /// <summary>
        /// Changes the object's velocity towards the specified direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        private void Accelerate(Vector2 direction)
        {
            var change = new Vector2(acceleration.X*direction.X, acceleration.Y*direction.Y);
            if (Math.Abs(change.X - 0) < float.Epsilon)
                Decellerate();
            Velocity = Vector2.Clamp(Velocity += change, -maxSpeed, maxSpeed);
        }

        private void Decellerate()
        {
            Velocity = Velocity.PushBack(deceleration);
        }

        private void UpdateBounds(Room room)
        {
            minPosition = room.GetMinPosition(position, size);
            maxPosition = room.GetMaxPosition(position, size);
        }

        private void ChangeXPosition(Room room)
        {
            // check for colliding against a wall
            if (position.X + Velocity.X < minPosition.X || position.X + Velocity.X > maxPosition.X)
                CollideWithWall(room);
            position.X = MathHelper.Clamp(position.X + Velocity.X, minPosition.X, maxPosition.X);
        }

        private void ChangeYPosition(Room room)
        {
            // check for colliding against a wall
            if (position.Y + Velocity.Y < minPosition.Y)
                CollideWithWall(room);
            else if (position.Y + Velocity.Y > maxPosition.Y)
            {
                CollideWithWall(room);
                CollideWithGround(room);
            }
                
            position.Y = MathHelper.Clamp(position.Y + Velocity.Y, minPosition.Y, maxPosition.Y);
        }

        /// <summary>
        ///     Draw the object.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch of the game.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D frame = curAnimation.GetTexture();
            spriteBatch.Draw(frame, new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y), null,
                             color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        ///     Changes the animation being played.
        /// </summary>
        /// <param name="name">The name of the new animation.</param>
        /// <exception cref="System.InvalidOperationException">Specified animation doesn't exist.</exception>
        private void ChangeAnimation(string name)
        {
            AnimationSet newAnimation = GetAnimationByName(name);
            if (newAnimation == null)
                throw new InvalidOperationException("Specified animation doesn't exist.");
            curAnimation = newAnimation;
        }

        private AnimationSet GetAnimationByName(string name)
        {
            return animations.Find(animset => animset.IsCalled(name));
        }

    }
}