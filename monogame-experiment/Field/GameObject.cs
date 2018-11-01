using System;

using monogame_experiment.Desktop.Core;

namespace monogame_experiment.Desktop.Field
{
    // A game object
    public abstract class GameObject : CollisionObject
    {

        // Update
        public abstract void Update(float tm, InputManager input = null);

        // Draw
        public abstract void Draw(Graphics g);
    }
}
