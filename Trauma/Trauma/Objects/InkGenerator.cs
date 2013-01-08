using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Microsoft.Xna.Framework;
using Trauma.Engine;
using Trauma.Rooms;
using Trauma.Helpers;

namespace Trauma.Objects
{
    /// <summary>
    ///     Generates ink particles with passion.
    /// </summary>
    public class InkGenerator : GameObject
    {
        #region Constants

        private const float SIZE_X = BLOB_SIZE_X*5;
        private const float SIZE_Y = BLOB_SIZE_Y*5;

        private const float BLOB_SIZE_X = 20;
        private const float BLOB_SIZE_Y = 20;

        private const float VELOCITY_VARIANCE = 5;

        // little bit of extra push to get the blob away from the wall
        private const float EXTRA_SHOVE = 40;

        private const string DRIP = "Drip";
        private const int DRIP_FRAME_WIDTH = 215;
        private const int DRIP_NUM_FRAMES = 6;

        private const int FRAME_DURATION = 5; 
        #endregion

        #region Members

        private Vector2 blobDirection;
        private float blobSpeed;
        private Color blobColor;

        private int reloadTimer;
        private int interval;

        #endregion

        public InkGenerator(Vector2 position, Vector2 blobDirection, Color color, int interval, float speed)
            : base(
                position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, color, true, 
                Vector2.One, new List<AnimationSet>
                    {
                        new AnimationSet(DRIP, ResourceManager.GetTexture("Generator_Drip"), DRIP_NUM_FRAMES, 
                            DRIP_FRAME_WIDTH, FRAME_DURATION)
                    },
                DRIP, blobDirection.Negate().ToAngle())
        {
            reloadTimer = 0;
            this.interval = interval;

            this.blobDirection = blobDirection;
            blobSpeed = speed;
            blobColor = color;
        }

        public override void Update(Room room, GameTime gameTime)
        {
            if (++reloadTimer == 20000) reloadTimer = 0;
            if (CanGenerate(room))
                Generate(room);
            base.Update(room, gameTime);
        }

        /// <summary>
        /// Generate a new blob.
        /// </summary>
        /// <param name="room">The room to add the blob to.</param>
        private void Generate(Room room)
        {
            Random random = new Random();
            float velocityVariance = (float)random.NextDouble() * VELOCITY_VARIANCE;
            int sign = random.Next(2) == 0 ? 1 : -1;
            room.Add(new InkBlob(Position.ShoveToSide(size, blobDirection) + blobDirection * EXTRA_SHOVE, 
                blobDirection * blobSpeed + (Vector2.One * (sign * velocityVariance)), blobColor, new Vector2(BLOB_SIZE_X, BLOB_SIZE_Y)));
        }

        private bool CanGenerate(Room room)
        {
            Debug.Assert(room.CanHaveMoreBlobs(), "Uh oh, we're getting too many blobs. (is garbage collection working?)");
            return reloadTimer%interval == 0;
        }
    }
}