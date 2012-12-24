using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Objects;

namespace Trauma.Engine
{
    /// <summary>
    /// A 2D camera. 
    /// Can zoom out, rotate and pan to places of interest.
    /// </summary>
    /// <remarks>
    /// Largely adapted from David Amador's blog post on 2D cameras. 
    /// See http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/.
    /// Thanks David!
    /// </remarks>
    public class Camera
    {
        private float zoom;
        public float Zoom
        {
            get { return zoom; }
            set 
            { 
                zoom = value;
                if (zoom < 0.1f) zoom = 0.1f;
            }
        }

        public float Rotation;
        public Matrix Transformation;
        private Vector2 position;
        public GameObject Target;

        public Camera()
        {
            zoom = 1.0f;
            Rotation = 0.0f;
            position = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Pan(Vector2 newPosition)
        {
            
        }

        public void Move(Vector2 )
    }
}
