using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Class.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation_etre_vivant.Class.Input
{
    public class MouseInputManager
    {
        MouseState _previous;
        MouseState _current;

        Vector2 Position;  
        public void Update(GameTime gameTime, MouseState currentState)
        {
            _previous = _current;
            _current = currentState;
            Position.X = _current.X;
            Position.Y = _current.Y;
        }
        public bool IsOn(PixelGeneric _pixelgeneric)
        {
            Rectangle rect = new ((int)_pixelgeneric.Position.X, (int)_pixelgeneric.Position.Y, _pixelgeneric.Width, _pixelgeneric.Height);
            if (rect.Intersects(new Rectangle((int)Position.X, (int)Position.Y, 1, 1)))
            {
                return true;
            }
            return false;
        }

        public bool LeftMouseUp()
        {
            if (_current.LeftButton == ButtonState.Released && _previous != _current)
            {
                return true;
            }
            return false;
        }

        public bool LeftMouseDown()
        {
            if (_current.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }

        public Vector2 GetMousePosition()
        {
            return new Vector2(Position.X, Position.Y);
        }
    }
}
