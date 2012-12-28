using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Microsoft.Xna.Framework;
using Trauma.Engine;
using Trauma.Rooms;

namespace Trauma.Objects
{
    /// <summary>
    ///     Generates ink particles with passion.
    /// </summary>
    public class InkGenerator : GameObject
    {
        #region Constants

        private const float SIZE_X = 10;
        private const float SIZE_Y = 10;

        private const float INTERVAL = 10;

        private const float DEFAULT_VELOCITY = 5;
        private const float VELOCITY_VARIANCE = 2;

        private const float DECELERATION_X = 0.5f;

        #endregion

        #region Members

        private Vector2 direction;
        private Timer timer;
        private bool canGenerate = true;

        #endregion

        public InkGenerator(Vector2 position, Room room, float interval=INTERVAL)
            : base(
                position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Color.Transparent, true, 
                new Vector2(SIZE_X, SIZE_Y), ResourceManager.GetTexture("Misc_Pixel"), 0)
        {
            timer = new Timer();
            timer.Interval = interval;
            timer.Elapsed += (sender, args) => canGenerate = true;
            timer.Start();
        }

        public override void Update(Room room, GameTime gameTime)
        {
            if (CanGenerate(room))
                Generate(room);

        }

        /// <summary>
        /// Generate a new blob.
        /// </summary>
        /// <param name="room">The room to add the blob to.</param>
        private void Generate(Room room)
        {
        }

        private bool CanGenerate(Room room)
        {
            Debug.Assert(room.CanHaveMoreBlobs(), "Uh oh, we're getting too many blobs. (is garbage collection working?)");
            return canGenerate;
        }
    }
}