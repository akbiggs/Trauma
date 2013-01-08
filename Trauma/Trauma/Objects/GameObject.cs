using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;
using Trauma.Helpers;
using Trauma.Rooms;

namespace Trauma.Objects
{
    /// <summary>
    ///     Any object that should abide by the game's laws and physics.
    /// </summary>
    public class GameObject
    {
        #region Constants
        private const float COLLISION_FORGIVENESS_FACTOR = 0.1f;
        #endregion

        #region Members

        protected readonly List<AnimationSet> animations;
        private readonly Vector2 deceleration;
        private readonly Vector2 maxSpeed;

        protected float rotation;
        protected Vector2 Velocity;

        private Vector2 acceleration;
        protected BBox box;

        private bool colorable;
        protected Color color;
        public Color Color
        {
            get { return color; }
        }

        protected virtual AnimationSet curAnimation { get; set; }
        private bool hasMoved;
        protected bool hasCollidedWithWall;

        private Vector2 maxPosition;
        private Vector2 minPosition;
        private Vector2 position;
        protected Vector2 size;

        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                box.Position = value;
            }
        }

        public Vector2 Center
        {
            get { return new Vector2(Position.X + (size.X / 2), Position.Y + (size.Y / 2)); }
        }

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
            Debug.Assert(size.X > 0 && size.Y > 0, "Invalid object size.");
            this.size = size;

            this.animations = animations;
            curAnimation = GetAnimationByName(startAnimationName);
            Debug.Assert(curAnimation != null, "Couldn't find the starting animation.");

            this.rotation = rotation;

            box = new BBox(new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y));
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
            hasCollidedWithWall = false;
            curAnimation.Update();
        }

        /// <summary>
        ///     Moves in the specified direction.
        /// </summary>
        /// <param name="room">The room that this object is moving around in.</param>
        /// <param name="direction">The direction to move in.</param>
        internal virtual void Move(Room room, Vector2 direction)
        {
            if (direction != Vector2.Zero)
                direction.Normalize();

            ApplyGravity(room);
            Accelerate(direction);

            // TODO: Figure out a way to resolve collisions and do only one function call.

            // resolve collisions on x-axis first
            UpdateBounds(room);
            ChangeXPosition(room);

            // then resolve any remaining on the y-axis
            UpdateBounds(room);
            ChangeYPosition(room);
        }

        /// <summary>
        /// Returns whether or not the given object is colliding with this object.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="collisionRegion">The region of collision. Empty if no collision occurred.</param>
        /// <returns>True if the objects are colliding, false otherwise.</returns>
        public virtual bool IsColliding(GameObject obj, out BBox collisionRegion)
        {
            collisionRegion = box.Intersect(obj.box);

            // because boxes are cruel, and not very true to the shape of the objects, only return true
            // if the area of collision is larger than we're willing to forgive.
            return collisionRegion != null && !collisionRegion.IsEmpty() && collisionRegion.Area >= box.Area * COLLISION_FORGIVENESS_FACTOR;
        }

        public virtual void CollideWithObject(GameObject obj, Room room, BBox collision)
        {
        }

        /// <summary>
        ///     Collides with a wall.
        /// </summary>
        /// <param name="room">The room containing the wall.</param>
        public virtual void CollideWithWall(Room room)
        {
            hasCollidedWithWall = true;
        }

        /// <summary>
        ///     Collides with the ground. Should be called alongside CollideWithWall.
        /// </summary>
        /// <param name="room">The room containing the ground.</param>
        public virtual void CollideWithGround(Room room)
        {
        }

        /// <summary>
        ///     Applies gravity to the object.
        /// </summary>
        /// <param name="room">The room that the object is in.</param>
        private void ApplyGravity(Room room)
        {
            Velocity.Y += room.Gravity;
        }

        /// <summary>
        ///     Changes the object's velocity towards the specified direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        private void Accelerate(Vector2 direction)
        {
            var change = new Vector2(acceleration.X*direction.X, acceleration.Y*direction.Y);
            // let gravity handle decelleration on the y-axis.
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
            minPosition = room.GetMinPosition(box.Position, box.Size);
            maxPosition = room.GetMaxPosition(box.Position, box.Size);
        }

        private void ChangeXPosition(Room room)
        {
            // check for colliding against a wall
            if (Position.X + Velocity.X < minPosition.X || Position.X + Velocity.X > maxPosition.X)
                CollideWithWall(room);
            Position = new Vector2(MathHelper.Clamp(Position.X + Velocity.X, minPosition.X, maxPosition.X), Position.Y);
        }

        private void ChangeYPosition(Room room)
        {
            bool shouldCollideWithGround = false;

            // check for colliding against a wall
            if (Position.Y + Velocity.Y < minPosition.Y)
                CollideWithWall(room);
            else if (Position.Y + Velocity.Y > maxPosition.Y)
            {
                CollideWithWall(room);
                // handle the ground collision after we've moved, to prevent weirdness when player is moving too fast
                shouldCollideWithGround = true;
            }

            Position = new Vector2(Position.X, MathHelper.Clamp(position.Y + Velocity.Y, minPosition.Y, maxPosition.Y));

            if (shouldCollideWithGround)
                CollideWithGround(room);
        }

        /// <summary>
        ///     Draw the object.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch of the game.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curAnimation.GetTexture(), new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y), curAnimation.GetFrameRect(),
                             color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        ///     Changes the animation being played. Doesn't do anything if called with the name of the currently
        ///     playing animation.
        /// </summary>
        /// <param name="name">The name of the new animation.</param>
        /// <exception cref="System.InvalidOperationException">Specified animation doesn't exist.</exception>
        protected virtual void ChangeAnimation(string name)
        {
            if (!curAnimation.IsCalled(name))
            {
                AnimationSet newAnimation = GetAnimationByName(name);
                if (newAnimation == null)
                    throw new InvalidOperationException("Specified animation doesn't exist.");
                newAnimation.Reset();
                newAnimation.Update();
                curAnimation = newAnimation;
            }
        }

        private AnimationSet GetAnimationByName(string name)
        {
            return animations.Find(animset => animset.IsCalled(name));
        }
    }
}