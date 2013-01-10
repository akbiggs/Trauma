using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Engine;
using Trauma.Rooms;

namespace Trauma.Objects
{
    /// <summary>
    /// The exit portal to any room.
    /// Allows the player if they're the same color as the portal,
    /// otherwise rejects.
    /// </summary>
    public class Portal : GameObject
    {
        #region Constants

        private const string SPIN = "Spin";

        // had to split up the spin animation into several textures due to size
        private const int NUM_SPIN_TEXTURES = 4;

        private const int FRAME_DURATION = 3;

        #endregion

        #region Members

        private static readonly List<int> spinFrames = new List<int> {8, 8, 8, 6};        
        private int curSpinIndex = 1;
        #endregion

        public Portal(Vector2 position, Vector2 size, Color color) :
            base(position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, 
            color, true, size, GetSpinAnimations(), "Spin1", 0)
        {
        }

        private static List<AnimationSet> GetSpinAnimations()
        {
            List<AnimationSet> spinAnimations = new List<AnimationSet>();
            for (int i = 1; i <= NUM_SPIN_TEXTURES; i++)
            {
                String animName = SPIN + i.ToString();
                spinAnimations.Add(new AnimationSet(animName, ResourceManager.GetTexture("Portal_" + animName), 
                    spinFrames[i-1], 150, FRAME_DURATION, false));
            }
            return spinAnimations;
        }

        public override void Update(Room room, GameTime gameTime)
        {
            base.Update(room, gameTime);

            Move(room, Vector2.Zero);

            // when we're done with the current spin frameset, switch to the next one.
            if (curAnimation.IsDonePlaying())
            {
                curSpinIndex = (curSpinIndex == NUM_SPIN_TEXTURES ? 1 : curSpinIndex + 1);
                ChangeAnimation(SPIN + curSpinIndex.ToString());
            }
        }
    }
}
