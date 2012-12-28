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
        private const float MIN_ZOOM = 0.1f;
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
            Zoom = DEFAULT_ZOOM;
            Rotation = 0.0f;
            Target = target;
        }

        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                if (zoom < MIN_ZOOM) zoom = MIN_ZOOM;
            }
        }

        public void Update(Room room, GameTime gameTime)
        {
            if (Target != null)
                position = Target.Position;

            // if we are still in the middle of zooming to a spot
            if (InZoomTransition())
            {
                Zoom = nextZoom > Zoom ? Math.Min(Zoom + ZOOM_SPEED, nextZoom) : Math.Max(Zoom - ZOOM_SPEED, nextZoom);

                // done zooming?
                if (Zoom == nextZoom) nextZoom = 0;
            }

            // TODO: Replace this with sections defining zoom levels.
            if (Input.MouseLeftButtonPressed)
                ZoomTo(1f);
            if (Input.MouseRightButtonDown)
                ZoomTo(2f);
        }

        private bool InZoomTransition()
        {
            return nextZoom >= MIN_ZOOM;
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