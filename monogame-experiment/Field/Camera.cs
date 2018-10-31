using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
    // Camera class
	public class Camera
    {
		// Position
		private Vector2 pos;
		// Scale
		private Vector2 scale;

        // Top-left corner
        private Vector2 topLeft;
        // Viewport size
        private Vector2 viewport;
        
		// Constructor
		public Camera(Vector2 pos)
        {
			this.pos = pos;
			scale = Vector2.One;

            viewport = new Vector2(1, 1);
            topLeft = new Vector2(1, 1);
        }


		// Constructor with no param
		public Camera() : this(Vector2.Zero) {}


        // Move to
        public void MoveTo(float x = 0.0f, float y = 0.0f)
		{
			pos.X = x;
			pos.Y = y;
		}


        // Translate
        public void Translate(float x, float y)
        {
            pos.X += x;
            pos.Y += y;
        }


        // Get position
		public Vector2 GetPos()
		{
			return pos;
		}


        // Scale
        public void Scale(float x = 1.0f, float y = 1.0f)
		{
			scale.X = x;
			scale.Y = y;
		}


        // Use camera
		public void Use(Graphics g)
		{
			Vector2 view = g.GetViewport();

            // Store viewport
            viewport = new Vector2(view.X / scale.X, view.Y / scale.Y);
            topLeft = pos - (new Vector2((viewport.X / 2.0f), (viewport.Y / 2.0f) ));

            g.IdentityWorld();
			g.TranslateWorld(view.X / 2.0f, view.Y / 2.0f);
            g.ScaleWorld(scale.X, scale.Y);
            g.TranslateWorld(-pos.X, -pos.Y);

		}


        // Get top-left corner
        public Vector2 GetTopLeftCorner()
        {
            return topLeft;
        }


        // Get viewport size (scaled to camera size)
        public Vector2 GetViewport()
        {
            return viewport;
        }
    }
}
