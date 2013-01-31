using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Engine;
using Trauma.Rooms;
using Microsoft.Xna.Framework.Graphics;

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
        private const string SYM = "Symbol";

        // had to split up the spin animation into several textures due to size
        private const int NUM_SPIN_TEXTURES = 4;
        private const int NUM_SYM_TEXTURES = 4;
        private const int FRAME_DURATION = 3;

        #endregion

        #region Members

        private static readonly List<int> spinFrames = new List<int> {8, 8, 8, 6};
        private static readonly List<int> symFrames = new List<int> { 9, 9, 9, 3 };
        private int curSpinIndex = 1;
        private int curSymIndex = 1;

        private AnimationSet curSymAnimation;
        public bool IsCorrect;
        public bool Symbolizing = false;
        public bool Unlocked = false;
        #endregion

        public Portal(RoomType type, Vector2 position, Vector2 size, Color color, bool isCorrect=false) :
            base(position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, 
            color, true, size, GetAnimations(type), "Spin1", 0)
        {
            IsCorrect = isCorrect;
        }

        private static List<AnimationSet> GetAnimations(RoomType type)
        {
            List<AnimationSet> animations = new List<AnimationSet>();
            for (int i = 1; i <= NUM_SPIN_TEXTURES; i++)
            {
                String animName = SPIN + i.ToString();
                animations.Add(new AnimationSet(animName, ResourceManager.GetTexture("Portal_" + animName), 
                    spinFrames[i-1], 150, FRAME_DURATION, false));
            }
            for (int i = 1; i <= NUM_SYM_TEXTURES; i++)
            {
                String animName = SYM + i.ToString();
                animations.Add(new AnimationSet(animName, ResourceManager.GetTexture("Portal_" + animName + "_" + GetTypeName(type)),
                    symFrames[i - 1], 215, FRAME_DURATION, false));
            }
            return animations;
        }

        private static String GetTypeName(RoomType type)
        {
            switch (type)
            {
                case RoomType.Acceptance:
                    return "Acceptance";
                case RoomType.Anger:
                case RoomType.Denial:
                    return "Anger";
                case RoomType.Bargain:
                    return "Bargain";
                case RoomType.Depression:
                    return "Depression";
                default:
                    throw new InvalidOperationException("Invalid room type.");
            }
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

            // when we're done with the current symbol frameset, switch
            if (curSymAnimation != null) {
                curSymAnimation.Update();
                if (curSymAnimation.IsDonePlaying())
                {
                    if (curSymIndex == NUM_SYM_TEXTURES)
                        Unlocked = true;
                    else
                    {
                        curSymAnimation = animations.Find((set) => set.IsCalled(SYM + (curSymIndex + 1).ToString()));
                        curSymIndex += 1;
                    }
                }
            }
        }

        public override void CollideWithObject(GameObject obj, Room room, BBox collision)
        {
            if (obj is Player && obj.Color == Color && !Symbolizing)
            {
                if (this != room.CorrectPortal)
                {
                    Unlocked = true;
                    room.Failed = true;
                    room.Finish();
                }
                else
                {
                    curSymAnimation = animations.Find((set) => set.IsCalled(SYM + "1"));
                    Symbolizing = true;
                }
            }
            base.CollideWithObject(obj, room, collision);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curAnimation.GetTexture(),
                new Rectangle((int)Position.X - 50, (int)Position.Y - 50, (int)(size.X * 1.5), (int)(size.Y * 1.5)),
                curAnimation.GetFrameRect(), color, 0, Vector2.Zero, SpriteEffects.None, 0);

            if (Symbolizing)
            {
                spriteBatch.Draw(curSymAnimation.GetTexture(),
                    new Rectangle((int)Center.X - 60, (int)Center.Y - 40, (int)(size.X / 2), (int)(size.Y / 2)),
                    curSymAnimation.GetFrameRect(), Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0);
            }
        }
    }
}
