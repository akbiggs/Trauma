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
    public class Wave : GameObject
    {
        private const int WAVING_SPEED = 3;
        private const float WAVE_SPEED_X = 6f;

        int wavingCounter = 0;
        float maxHeight;
        float maxHeightPos;

        Vector2 lastPosition;

        public override bool IsCollidable
        {
            get { return Math.Abs(curAnimation.CurFrameNumber - 3) == 1; }
        }

        public Wave(Vector2 position, Vector2 size, Vector2 velocity, Color color, bool isWater)
            : base(position, velocity, new Vector2(WAVE_SPEED_X, 3f), Vector2.Zero,
            Vector2.Zero, color, true, size, new List<AnimationSet> 
            {
                new AnimationSet("Main", GetTexture(color, isWater), 6, 200, 6, true),
                new AnimationSet("Disappear", GetTexture(color, isWater), 3, 200, 6, false, 4)
            }, "Main", 0)
        {
            maxHeight = size.Y;
            maxHeightPos = position.Y;
        }

        private static Texture2D GetTexture(Color color, bool isWater)
        {
            if (isWater)
                return ResourceManager.GetTexture("Generator_Wave_Water");
            if (color == Color.Black)
                return ResourceManager.GetTexture("Generator_Wave_Black");

            return ResourceManager.GetTexture("Generator_Wave");
        }

        public override void Update(Room room, GameTime gameTime)
        {
            lastPosition = Position;
            base.Update(room, gameTime);
            //Position = new Vector2(Position.X, maxHeightPos + (maxHeight - size.Y));
            Move(room, Velocity);
            if ((Velocity.Y > 1 || Math.Abs(Vector2.Distance(lastPosition, Position)) < Math.Abs(Velocity.X / 2)) && !curAnimation.IsCalled("Disappear"))
                 ChangeAnimation("Disappear");
            if (curAnimation.IsCalled("Disappear") && curAnimation.IsDonePlaying())
                room.Remove(this);

        }

        public override void CollideWithWall(Room room)
        {
            base.CollideWithWall(room);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curAnimation.GetTexture(), new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y), curAnimation.GetFrameRect(),
                color, rotation, Vector2.Zero, Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
