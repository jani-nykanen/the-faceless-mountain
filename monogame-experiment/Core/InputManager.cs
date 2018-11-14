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
		// Max gamepad button count
		const int GAMEPAD_BUTTON_COUNT = 10;

		// Key states
		private State[] keyStates;
		// Old key states
		private State[] oldKeyStates;

		// Mouse button states
		private State[] buttonStates;
		// Old button states
		private State[] oldButtonStates;

		// Gamepad button states
		private State[] gamePadStates;
		// Old gamepad button states
		private State[] oldPadStates;

 		// Mouse position
		private Vector2 mousePos;

		// Key configuration
		private KeyConfig keyConf;

		// Virtual gamepad stick
		private Vector2 stick;
        // Stick delta
        private Vector2 stickDelta;

		// Reference to graphics
		private Graphics graph;


        // Check special button array
		private void CheckSpecialButtons(bool[] pressed, State[] oldArr, State[] newArr)
		{
			for (int i = 0; i < pressed.Length; ++i)
            {
                if (pressed[i])
                {
                    // Button down
					if (oldArr[i] == State.Up)
                    {
						eventDown(newArr, i);
                    }
					oldArr[i] = State.Down;
                }
                else
                {
                    // Button up
					if (oldArr[i] == State.Down)
                    {
						eventUp(newArr, i);
                    }
					oldArr[i] = State.Up;
                }
            }
		}

        
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


        // Update stick
		private void UpdateStick(GamePadState state)
		{
			const float DELTA = 0.1f;

            // Store old stick
            Vector2 oldStick = stick;

			stick = Vector2.Zero;

            // Check arrow keys
			if(keyConf != null)
			{
				if (GetKey("left") == State.Down)
					stick.X = -1.0f;
				else if (GetKey("right") == State.Down)
                    stick.X = 1.0f;

				if (GetKey("up") == State.Down)
                    stick.Y = -1.0f;
                else if (GetKey("down") == State.Down)
                    stick.Y = 1.0f;
			}

			// Check gamepad stick
			Vector2 lstick = state.ThumbSticks.Left;
			if((float)Math.Sqrt(lstick.X*lstick.X + lstick.Y*lstick.Y) > DELTA)
			{
				lstick.Y *= -1;
				stick = lstick;
			}

			// Check dpad
			GamePadDPad dpad = state.DPad;
			if (dpad.Left == ButtonState.Pressed)
                stick.X = -1.0f;
			else if (dpad.Right == ButtonState.Pressed)
                stick.X = 1.0f;

			if (dpad.Up == ButtonState.Pressed)
                stick.Y = -1.0f;
			else if (dpad.Down == ButtonState.Pressed)
                stick.Y = 1.0f;

            // Calculate delta
            stickDelta = stick - oldStick;

        }


		// Constructor
		public InputManager()
		{
			// Create state arrays
			keyStates = new State[LAST_KEY];
			oldKeyStates = new State[LAST_KEY];
			buttonStates = new State[MOUSE_BUTTON_COUNT];
			oldButtonStates = new State[MOUSE_BUTTON_COUNT];
			gamePadStates = new State[GAMEPAD_BUTTON_COUNT];
			oldPadStates = new State[GAMEPAD_BUTTON_COUNT];

            // Set to defaults
			keyStates.Initialize();
			oldKeyStates.Initialize();
			buttonStates.Initialize();
			oldButtonStates.Initialize();
			gamePadStates.Initialize();
			oldPadStates.Initialize();

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

            const float TRIGGER_DELTA = 0.25f;

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
                     
			// Do the same with mouse buttons
			MouseState state = Mouse.GetState();
			bool[] mousePressed = {
				state.LeftButton == ButtonState.Pressed,
				state.MiddleButton == ButtonState.Pressed,
				state.RightButton == ButtonState.Pressed,
			};
			CheckSpecialButtons(mousePressed, oldButtonStates, buttonStates);

            // Update gamepad state
			GamePadState padState = GamePad.GetState(PlayerIndex.One);
			// Update stick
			UpdateStick(padState);
            
			// Update gamepad button states
			GamePadButtons buttons = padState.Buttons;
			bool[] padPressed = {

				buttons.A == ButtonState.Pressed,
				buttons.X == ButtonState.Pressed,
				buttons.B == ButtonState.Pressed,
				buttons.Y == ButtonState.Pressed,

				buttons.LeftShoulder == ButtonState.Pressed,
				buttons.RightShoulder == ButtonState.Pressed,

                padState.Triggers.Left >= TRIGGER_DELTA,
                padState.Triggers.Right >= TRIGGER_DELTA,


                buttons.Back == ButtonState.Pressed,
				buttons.Start == ButtonState.Pressed,


            };
			CheckSpecialButtons(padPressed, oldPadStates, gamePadStates);
		}


		// "Post" update
		public void PostUpdate()
		{
			// Update state arrays
			updateStateArray(keyStates);
			updateStateArray(buttonStates);
			updateStateArray(gamePadStates);
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


        // Like GetKey but also checks the gamepad button
        // (yeah, a misleading name, I'd better rename
		// these methods)
        public State GetButton(String name)
		{
			if (keyConf == null) return State.Up;

			int[] ids = keyConf.GetKeyAndButtonIndex(name);

			State ret = GetKey(ids[0]);
			if (ret == State.Up)
				ret = GetGamepadButton(ids[1]);

			return ret;
		}


        // Get mouse button
		public State GetMouseButton(int id)
		{
			if (id < 0 || id >= buttonStates.Length)
				return State.Up;

			return buttonStates[id];
		}


        // Get gamepad button
        public State GetGamepadButton(int id)
		{
			if (id < 0 || id >= gamePadStates.Length)
				return State.Up;

			return gamePadStates[id];
		}


        // Get mouse position in the view coordinates
		public Vector2 GetCursorPos()
		{
			// Get mouse position
            mousePos.X = Mouse.GetState().X;
            mousePos.Y = Mouse.GetState().Y;

			Vector2 ret = new Vector2();
			Vector2 view = graph.GetViewport();
			Vector2 frame = graph.GetFramebufferSize();
                        
			ret.X = mousePos.X / frame.X * view.X;
			ret.Y = mousePos.Y / frame.Y * view.Y;

			return ret;
		}


        // Get stick
		public Vector2 GetStick()
		{
			return stick;
		}


        // Get stick delta
        public Vector2 GetStickDelta()
        {
            return stickDelta;
        }
	}
}
