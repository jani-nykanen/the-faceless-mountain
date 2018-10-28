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

        
		// Constructor
		public Camera(Vector2 pos)
        {
			this.pos = pos;
			scale = Vector2.One;
        }


		// Constructor with no param
		public Camera() : this(Vector2.Zero) {}


        // Move to
        public void MoveTo(float x = 0.0f, float y = 0.0f)
		{
			pos.X = x;
			pos.Y = y;
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

			g.IdentityWorld();
			g.TranslateWorld(view.X / 2.0f, view.Y / 2.0f);
			g.TranslateWorld(pos.X, pos.Y);
			g.ScaleWorld(scale.X, scale.Y);

		}
    }
}
