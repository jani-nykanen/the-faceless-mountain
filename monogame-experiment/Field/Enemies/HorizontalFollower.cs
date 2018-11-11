using System;

using Microsoft.Xna.Framework;
using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field.Enemies
{
    // Horizontal follower
    public class HorizontalFollower : Enemy
    {

        // Player direction from object's pov
        private int plDir;


        // Player event
        protected override void OnPlayerEvent(Player pl)
        {
            plDir = (pl.GetPos().X > pos.X) ? 1 : -1;
        }



        // Update AI
        protected override void UpdateAI(float tm)
        {
            const float SPEED_X = 2.5f;

            target.X = plDir * SPEED_X;
        }


        // Animate
        protected override void Animate(float tm)
        {
            const float ANIM_SPEED = 6.0f;

            spr.Animate(5, 0, 3, ANIM_SPEED, tm);
        }


        // Constructor
        public HorizontalFollower(float x, float y) : base(x, y)
        {

            const float ACC_X = 0.1f;

            acc.X = ACC_X;
            height /= 2;

            plDir = 0;
        }


        // On wall collision
        protected override void OnWallCollision(float x, float y, int dir)
        {
            pos.X = x;
            speed.X /= -1.0f;
        }
    }
}
