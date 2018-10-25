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
        // Mouse button count
		const int MOUSE_BUTTON_COUNT = 3;

		// Key states
		private State[] keyStates;
		// Old key states
		private State[] oldKeyStates;

		// Mouse button states
		private State[] buttonStates;
		// Old button states
		private State[] oldButtonStates;

 		// Mouse position
		private Vector2 mousePos;

		// Key configuration
		private KeyConfig keyConf;

		// Reference to graphics
		private Graphics graph;

        
        // Update a state array
        private void updateStateArray(State[] arr)
		{
			// Update states
			for (int i = 0; i < arr.Length; ++i)
            {
                if (arr[i] == State.Released)
                    arr[i] = State.Up;

				else if (arr[i] == State.Pressed)
					arr[i] = State.Down;
            }
		}


		// Key/button down event (simulated)
		private void eventDown(State[] arr, int key)
		{
			// If the key is in range and not already down, make it be pressed
			if (key < 0 || key >= arr.Length || arr[key] == State.Down)
				return;
            
			arr[key] = State.Pressed;
		}
        

		// Key/button up event (simulated)
		private void eventUp(State[] arr, int key)
		{
			// If the key is in range and not already down, make it be pressed
			if (key < 0 || key >= arr.Length || arr[key] == State.Up)
				return;

			arr[key] = State.Released;
		}


		// Constructor
		public InputManager()
		{
			// Create state arrays
			keyStates = new State[LAST_KEY];
			oldKeyStates = new State[LAST_KEY];
			buttonStates = new State[MOUSE_BUTTON_COUNT];
			oldButtonStates = new State[MOUSE_BUTTON_COUNT];

			keyStates.Initialize();
			oldKeyStates.Initialize();
			buttonStates.Initialize();
			oldButtonStates.Initialize();

			mousePos = new Vector2();
		}


        // Store reference to graphics
        public void PassGraphics(Graphics g)
		{
			graph = g;
		}


		// Update 
		public void Update()
		{
			// Get mouse position
			mousePos.X = Mouse.GetState().X;
			mousePos.Y = Mouse.GetState().Y;

			// Go through all the damn keys and check
			// status changes
			for (int i = 0; i < LAST_KEY; ++i)
			{
				if (Keyboard.GetState().IsKeyDown((Keys)i))
				{
					// Compare to the old state
					if (oldKeyStates[i] == State.Up)
					{
						eventDown(keyStates, i);
					}
                    
					oldKeyStates[i] = State.Down;
				}
				else
				{

					// Key up event
					if (oldKeyStates[i] == State.Down)
					{
						eventUp(keyStates, i);
					}
					oldKeyStates[i] = State.Up;
				}
			}

			MouseState state = Mouse.GetState();
			// Do the same with mouse buttons, but now we
			// have to go through buttons manually (kind of)
			bool[] pressed = {
				state.LeftButton == ButtonState.Pressed,
				state.MiddleButton == ButtonState.Pressed,
				state.RightButton == ButtonState.Pressed,
			};
			for (int i = 0; i < pressed.Length; ++ i)
			{
				if(pressed[i])
				{
					// Button down
					if(oldButtonStates[i] == State.Up)
					{
						eventDown(buttonStates, i);
					}
					oldButtonStates[i] = State.Down;
				}
				else 
				{
					// Button up
					if (oldButtonStates[i] == State.Down)
                    {
						eventUp(buttonStates, i);
                    }
                    oldButtonStates[i] = State.Up;
				}
			}

		}


		// "Post" update
		public void PostUpdate()
		{
			// Update state arrays
			updateStateArray(keyStates);
			updateStateArray(buttonStates);
		}


        // Bind a key configuration
		public void BindKeyConfig(KeyConfig kconf)
		{
			this.keyConf = kconf;
		}


		// Get a key (by index)
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


		// Get a key (by name, if key config bound)
        public State GetKey(String name)
		{
			if (keyConf == null) return State.Up;

			return GetKey(keyConf.GetKeyIndex(name));
		}


        // Get mouse button
		public State GetMouseButton(int id)
		{
			if (id < 0 || id >= buttonStates.Length)
				return State.Up;

			return buttonStates[id];
		}


        // Get mouse position in the view coordinates
		public Vector2 GetCursorPos()
		{
			Vector2 ret = new Vector2();
			Vector2 view = graph.GetViewport();
			Vector2 frame = graph.GetFramebufferSize();
                        
			ret.X = mousePos.X / frame.X * view.X;
			ret.Y = mousePos.Y / frame.Y * view.Y;

			return ret;
		}
	}
}
