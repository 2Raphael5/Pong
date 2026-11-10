using Class.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace Class
{
    public class InputManager
    {
        private readonly KeyboardInputManager _keyboard = new();
        private readonly Dictionary<InputAction, Keys> _keyMap = new()
        {
            { InputAction.P1Up, Keys.W },
            { InputAction.P1Down, Keys.S },

            { InputAction.P2Up, Keys.Up },
            { InputAction.P2Down, Keys.Down },

            { InputAction.Pause, Keys.P },
            { InputAction.Confirm, Keys.Space },
            { InputAction.Restart, Keys.R },
            { InputAction.Debug, Keys.F1 },
            { InputAction.Exit, Keys.Escape},
        };
        private readonly Dictionary<InputAction, Buttons> _buttonMap = new()
        {
            { InputAction.Pause, Buttons.Start },
            { InputAction.Confirm, Buttons.A },
            { InputAction.Restart, Buttons.Y },
        };

        private GamePadState _previousGamePad;
        private GamePadState _currentGamePad;
        private const float StickDeadzone = 0.0000025f;
        public void Update(GameTime gameTime)
        {
            _keyboard.Update(gameTime, Keyboard.GetState());
            _previousGamePad = _currentGamePad;
            _currentGamePad = GamePad.GetState(PlayerIndex.One);
            
        }
        public bool IsDown(InputAction action)
        {
            if (_keyMap.TryGetValue(action, out Keys key) && _keyboard.IsDown(key))
            {
                return true;
            }
            if (_buttonMap.TryGetValue(action, out Buttons button) && _currentGamePad.IsButtonDown(button))
            {
                return true;
            }
            return IsDirectionActive(action, _currentGamePad.ThumbSticks.Left);
        }
        public bool IsJustPressed(InputAction action)
        {
            if (_keyMap.TryGetValue(action, out Keys key) && _keyboard.IsJustPressed(key))
            {
                return true;
            }
            if (_buttonMap.TryGetValue(action, out Buttons button))
            {
                bool wasDown = _previousGamePad.IsButtonDown(button);
                bool isDown = _currentGamePad.IsButtonDown(button);
                if (isDown && !wasDown)
                {
                    return true;
                }
            }
            bool wasDirectionActive = IsDirectionActive(action, _previousGamePad.ThumbSticks.Left);
            bool isDirectionActive = IsDirectionActive(action, _currentGamePad.ThumbSticks.Left);
            return isDirectionActive && !wasDirectionActive;
        }
        private bool IsDirectionActive(InputAction action, Vector2 stick)
        {
            return action switch
            {
                InputAction.P1Up => stick.Y > StickDeadzone,
                InputAction.P1Down => stick.Y < -StickDeadzone,
                InputAction.P2Up => stick.Y > StickDeadzone,
                InputAction.P2Down => stick.Y < -StickDeadzone,
                _ => false,
            };
        }
    }
}

