using System;
using System.Collections.Generic;

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
		protected Matrix world;
        // Model matrix
		protected Matrix model;
		// Right-side matrix operand
		protected Matrix operand;
		// Matrix stack
		protected Stack<Matrix> stack;

		// Viewport size
		protected Vector2 viewport;

		// Graphics device
		protected GraphicsDevice gdev;

        
        // Constructor
        public Transformation(GraphicsDevice gdev)
        {
			// Set matrices to identity matrices
			view = Matrix.Identity;
			model = Matrix.Identity;
			world = Matrix.Identity;
			operand = Matrix.Identity;

			// Create stack
			stack = new Stack<Matrix>();

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


        // Identity, world
        public void IdentityWorld()
		{
			world = Matrix.Identity;
		}


        // Translate
        public void Translate(float x, float y) 
		{
			operand = Matrix.CreateTranslation(new Vector3(x, y, 0.0f));
			model = Matrix.Multiply(operand, model);
		}


        // Translate world
        public void TranslateWorld(float x, float y)
		{
			operand = Matrix.CreateTranslation(new Vector3(x, y, 0.0f));
			world = Matrix.Multiply(operand, world);
		}

        
        // Scale
        public void Scale(float x, float y) 
		{
			operand = Matrix.CreateScale(new Vector3(x, y, 1.0f));
			model = Matrix.Multiply(operand, model);
		}


        // Scale world
		public void ScaleWorld(float x, float y)
		{
			operand = Matrix.CreateScale(new Vector3(x, y, 1.0f));
			world = Matrix.Multiply(operand, world);
		}


		// Rotate
        public void Rotate(float angle)
        {
			operand = Matrix.CreateRotationZ(angle);
			model = Matrix.Multiply(operand, model);
        }


        // Get result matrix
		public Matrix GetResultMatrix()
		{
			operand = Matrix.Multiply(world, view);
			return Matrix.Multiply(model, operand);
		}


        // Get viewport
        public Vector2 GetViewport()
		{
			return viewport;
		}


        // Get framebuffer size
		public Vector2 GetFramebufferSize()
		{
			return new Vector2((float)gdev.PresentationParameters.BackBufferWidth,
							   (float)gdev.PresentationParameters.BackBufferHeight);
		}
    
	    
        // Push model to the stack
        public void Push()
		{
			stack.Push(model);
		}


        // Pop model matrix from the stack
        public void Pop()
		{
			model = stack.Pop();
		}
	}
}
