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

        private const float BLOB_SIZE_X = 60;
        private const float BLOB_SIZE_Y = 60;

        private const float VELOCITY_VARIANCE = 5;

        // little bit of extra push to get the blob away from the wall
        private const float EXTRA_SHOVE_LEFT = BLOB_SIZE_X*1.5f;
        private const float EXTRA_SHOVE_UP = BLOB_SIZE_Y*1.4f;
        private const float EXTRA_SHOVE_RIGHT = BLOB_SIZE_X;
        private const float EXTRA_SHOVE_DOWN = BLOB_SIZE_Y*1.25f;

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
            reloadTimer = interval;
            this.interval = interval;

            this.blobDirection = blobDirection;
            blobSpeed = speed;
            blobColor = color;
        }

        public override void Update(Room room, GameTime gameTime)
        {
            reloadTimer++;
            if (CanGenerate(room))
            {
                room.Add(Generate());
                reloadTimer = 0;
            }
            base.Update(room, gameTime);
        }

        /// <summary>
        /// Generate a new blob.
        /// </summary>
        /// <param name="room">The room to add the blob to.</param>
        protected virtual InkBlob Generate()
        {
            Random random = new Random();
            float velocityVariance = (float)random.NextDouble() * VELOCITY_VARIANCE;
            int varianceSign = random.Next(2) == 0 ? 1 : -1;

            float shoveX = 0;
            if (blobDirection.X < 0)
                shoveX = EXTRA_SHOVE_LEFT;
            if (blobDirection.X > 0)
                shoveX = EXTRA_SHOVE_RIGHT;

            float shoveY = 0;
            if (blobDirection.Y < 0)
                shoveY = EXTRA_SHOVE_UP;
            if (blobDirection.Y > 0)
                shoveY = EXTRA_SHOVE_DOWN;

            return new InkBlob(Center + blobDirection * new Vector2(shoveX, shoveY),
                blobDirection * blobSpeed + (Vector2.One * (varianceSign * velocityVariance)), blobColor, new Vector2(BLOB_SIZE_X, BLOB_SIZE_Y));
        }

        protected virtual bool CanGenerate(Room room)
        {
            Debug.Assert(room.CanHaveMoreBlobs(), "Uh oh, we're getting too many blobs. (is garbage collection working?)");
            return reloadTimer >= interval;
        }
    }
}