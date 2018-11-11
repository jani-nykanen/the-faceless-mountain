using System;

using Microsoft.Xna.Framework;
using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field.Enemies
{
    // Vertical follower
    public class VerticalFollower : Enemy
    {

        // Player direction from object's pov
        private int plDir;


        // Player event
        protected override void OnPlayerEvent(Player pl)
        {
            plDir = (pl.GetPos().Y > pos.Y) ? 1 : -1;
        }



        // Update AI
        protected override void UpdateAI(float tm)
        {
            const float SPEED_Y = 2.5f;

            target.Y = plDir * SPEED_Y;
        }


        // Animate
        protected override void Animate(float tm)
        {
            spr.Animate(5, 0, 0, 0, tm);
        }


        // Constructor
        public VerticalFollower(float x, float y) : base(x, y)
        {

            const float ACC_Y = 0.1f;

            acc.Y = ACC_Y;
            width /= 2;

            plDir = 0;
        }


        // On floor collision
        protected override void OnFloorCollision(float x, float y)
        {
            if (speed.Y < 0.0f) return;

            pos.Y = y;
            speed.Y *= -1;
        }


        // On ceiling collision
        protected override void OnCeilingCollision(float x, float y)
        {
            if (speed.Y > 0.0f) return;

            pos.Y = y;
            speed.Y *= -1;
        }
    }
}
