using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogame_experiment.Desktop.Core
{
    // Transformation manager
	public class Transformation
    {
		// View matrix
		protected Matrix view;
        // Model matrix
		protected Matrix model;
		// Right-side matrix operand
		protected Matrix operand;

		// Viewport size
		protected Vector2 viewport;

		// Graphics device
		protected GraphicsDevice gdev;

        
        // Constructor
        public Transformation(GraphicsDevice gdev)
        {
			view = Matrix.Identity;
			model = Matrix.Identity;
			operand = Matrix.Identity;

			this.gdev = gdev;
        }


        // Set view
        public void SetView(float w, float h) 
		{
			viewport.X = w;
			viewport.Y = h;

			w /= gdev.PresentationParameters.BackBufferWidth;
			w *= 2;

			h /= gdev.PresentationParameters.BackBufferHeight;
			h *= 2;
			         
			view = Matrix.CreateOrthographic(w, h, -1.0f, 1.0f);
		}


        // Fit view height
        public void FitViewHeight(float h)
		{
			float ratio = 
				    (float)gdev.PresentationParameters.BackBufferWidth / 
				    (float)gdev.PresentationParameters.BackBufferHeight;
			
            float w = ratio * h;

			SetView(w, h);
		}


        // Identity
        public void Identity() 
		{
			model = Matrix.Identity;
		}


        // Translate
        public void Translate(float x, float y) 
		{
			operand = Matrix.CreateTranslation(new Vector3(x, y, 0.0f));
			model = Matrix.Multiply(model, operand);
		}

        
        // Scale
        public void Scale(float x, float y) 
		{
			operand = Matrix.CreateScale(new Vector3(x, y, 1.0f));
            model = Matrix.Multiply(model, operand);
		}


		// Rotate
        public void Rotate(float angle)
        {
			operand = Matrix.CreateRotationZ(angle);
            model = Matrix.Multiply(model, operand);
        }


        // Get result matrix
		public Matrix GetResultMatrix()
		{
			return Matrix.Multiply(view, model);
		}
    }
}
