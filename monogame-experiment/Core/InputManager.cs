using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace monogame_experiment.Desktop.Core
{
	// An input manager
	public class InputManager
	{
		// Possibly no more keys are needed
		const int LAST_KEY = 255;

		// Key states
		private State[] keyStates;
		// Old states
		private State[] oldStates;


		// Key down event (simulated)
		private void keyDown(int key)
		{
			// If the key is in range and not already down, make it be pressed
			if (key < 0 || key >= LAST_KEY || keyStates[key] == State.Down)
				return;

			keyStates[key] = State.Pressed;
		}


		// Key up event (simulated)
		private void keyUp(int key)
		{
			// If the key is in range and not already down, make it be pressed
			if (key < 0 || key >= LAST_KEY || keyStates[key] == State.Up)
				return;

			keyStates[key] = State.Released;
		}


		// Constructor
		public InputManager()
		{
			// Create an array of keys (plus the 
			// array of old states)
			keyStates = new State[LAST_KEY];
			oldStates = new State[LAST_KEY];
			keyStates.Initialize();
			oldStates.Initialize();
		}


		// Update 
		public void Update()
		{
			// Go through all the damn keys and check
			// status changes
			for (int i = 0; i < LAST_KEY; ++i)
			{
				if (Keyboard.GetState().IsKeyDown((Keys)i))
				{
					// Compare to the old state
					if (oldStates[i] == State.Up)
					{
						keyDown(i);
					}

					oldStates[i] = State.Down;
				}
				else
				{

					// Key up event
					if (oldStates[i] == State.Down)
					{

						keyUp(i);
					}
					oldStates[i] = State.Up;
				}
			}
		}


		// "Post" update
		public void PostUpdate()
		{
			// Update key states
			for (int i = 0; i < LAST_KEY; ++i)
			{
				if (keyStates[i] == State.Released)
					keyStates[i] = State.Up;

				else if (keyStates[i] == State.Pressed)
					keyStates[i] = State.Down;
			}
		}


		// Get a key
		public State GetKey(int key)
		{
			if (key < 0 || key >= LAST_KEY)
				return State.Up;

			return keyStates[key];
		}


		// Get a key (when given in Keys type)
		public State GetKey(Keys key)
        {
			return GetKey((int)key);
        }
	}
}
