using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
	// Player object
    public class Player
    {
		const float SCALE = 64.0f;

		// Animated figure ("skeleton")
		private AnimatedFigure skeleton;

		// Position
		private Vector2 pos;


        // Constructor
        public Player(Vector2 pos)
        {
			skeleton = new AnimatedFigure(SCALE);

			this.pos = pos;
		}
		public Player() : this(Vector2.Zero) { }


        // Update player
        public void Update(InputManager input, float tm)
		{
			const float ANIM_SPEED = 0.05f;

			// Animate skeleton
			skeleton.Animate(ANIM_SPEED, tm);
		}


        // Draw player
        public void Draw(Graphics g)
		{
			// Draw figure
			skeleton.Draw(g);
		}
    }
}
