using System;
namespace monogame_experiment.Desktop.Field.Enemies
{
    // A fly that does not move
    public class StaticFly : Enemy
    {

        // Constructor
        public StaticFly(float x, float y) : base(x, y)
        {
            // ...
        }


        // Animate
        protected override void Animate(float tm)
        {
            spr.Animate(3, 0, 0, 0, tm);
        }
    }
}
