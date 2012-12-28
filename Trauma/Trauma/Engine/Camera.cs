using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Objects;
using Trauma.Rooms;

namespace Trauma.Engine
{
    /// <summary>
    ///     A 2D camera.
    ///     Can zoom out, rotate and pan to places of interest.
    /// </summary>
    /// <remarks>
    ///     Largely adapted from David Amador's blog post on 2D cameras.
    ///     See http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/.
    ///     Thanks David!
    /// </remarks>
    public class Camera
    {
        #region Constants

        private const float DEFAULT_ZOOM = 1.25f;
        private const float ZOOM_SPEED = 0.1f;
        #endregion
        #region Members

        public float Rotation;
        public GameObject Target;
        public Matrix Transformation;
        private Vector2 position;
        private float zoom;
        private float nextZoom;

        #endregion
        /// <summary>
        /// Make a new camera focusing on the given game object.
        /// </summary>
        /// <param name="target">The target to focus on.</param>
        public Camera(GameObject target)
        {
            zoom = DEFAULT_ZOOM;
            Rotation = 0.0f;
            Target = target;
        }

        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                if (zoom < 0.1f) zoom = 0.1f;
            }
        }

        public void Update(Room room, GameTime gameTime)
        {
            if (Target != null)
                position = Target.Position;

            if (InZoomTransition())
                zoom = MathHelper.Lerp(zoom, nextZoom, ZOOM_SPEED);

            // TODO: Replace this with sections defining zoom levels.
            if (Input.MouseLeftButtonTapped)
                ZoomTo(1f);
            if (Input.MouseRightButtonTapped)
                ZoomTo(2f);
        }

        private bool InZoomTransition()
        {
            return nextZoom != 0 && Math.Abs(Zoom - nextZoom) > 0.00001;
        }

        public void Pan(Vector2 newPosition)
        {
        }

        public void ZoomTo(float newZoom)
        {
            nextZoom = newZoom;
        }

        public void Move(Vector2 amount)
        {
            position += amount;
        }

        public Matrix GetTransformation(GraphicsDevice graphicsDevice)
        {
            Transformation = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0))*
                             Matrix.CreateRotationZ(Rotation)*
                             Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))*
                             Matrix.CreateTranslation(
                                 new Vector3(graphicsDevice.Viewport.Width*0.5f,
                                             graphicsDevice.Viewport.Height*0.5f, 0));
            return Transformation;
        }
    }
}