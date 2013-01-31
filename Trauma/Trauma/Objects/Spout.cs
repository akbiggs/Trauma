﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Engine;
using Trauma.Helpers;
using Trauma.Rooms;
using Microsoft.Xna.Framework.Graphics;

namespace Trauma.Objects
{
    public class Spout : GameObject
    {
        const float SPOUT_SPEED = 0f;
        const float SIZE_X = 50;
        const float SIZE_Y = 225;

        public override Vector2 BoxOffset
        {
            get
            {
                return Vector2.Zero;
            }
        }
        public static Vector2 SIZE
        {
            get { return new Vector2(SIZE_X, SIZE_Y); }
        }

        public bool ShouldDamage
        {
            get { return curAnimation.IsCalled("Main") || curAnimation.IsCalled("Appear"); }
        }

        private Vector2 direction;

        public Spout(Vector2 position, Color color, Vector2 direction, bool isWater)
            : base(position, direction * SPOUT_SPEED, SPOUT_SPEED * Vector2.One, Vector2.Zero,
            Vector2.Zero, color, true, new Vector2(SIZE_X, SIZE_Y), new List<AnimationSet>() 
            {
                new AnimationSet("Appear", GetTexture(color, isWater), 2, 50, 2, false),
                new AnimationSet("Main", GetTexture(color, isWater), 1, 50, 2, false, 2),
                new AnimationSet("Disappear", GetTexture(color, isWater), 2, 50, 2, false, 3)
            }, "Appear", direction.ToAngle())
        {
            this.direction = direction;            
        }

        private static Texture2D GetTexture(Color color, bool isWater)
        {
            if (isWater)
                return ResourceManager.GetTexture("Generator_Spout_Water");
            if (color == Color.Black)
                return ResourceManager.GetTexture("Generator_Spout_Black");

            return ResourceManager.GetTexture("Generator_Spout");
        }

        public override void Update(Room room, GameTime gameTime)
        {
            if (curAnimation.IsCalled("Appear") && curAnimation.IsDonePlaying())
                ChangeAnimation("Main");
            else if (curAnimation.IsCalled("Main") && curAnimation.IsDonePlaying())
                ChangeAnimation("Disappear");
            else if (curAnimation.IsCalled("Disappear") && curAnimation.IsDonePlaying())
                room.Remove(this);

            Move(room, Vector2.Zero);

            
            if (direction.X < 0)
                box = new BBox((int)(Position.X - SIZE_Y / 4 + SIZE_X), (int)(Position.Y - SIZE_X), (int)SIZE_Y, (int)SIZE_X);
            else if (direction.X > 0)
            {
                box = new BBox((int)(Center.X - 60), (int)Position.Y, (int)SIZE_Y, (int)SIZE_X);
            }

            base.Update(room, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            box.Draw(spriteBatch);
        }
    }
}
