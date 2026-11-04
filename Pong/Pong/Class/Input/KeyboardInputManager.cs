using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace Class.Input;
public class KeyboardInputManager
{
    private KeyboardState _previous = new KeyboardState();
    private KeyboardState _current = new KeyboardState();
    public void Update(GameTime gameTime, KeyboardState currentState)
    {
        _previous = _current;
        _current = currentState;
    }
    public bool IsJustPressed(Keys key)
    {
        return _current.IsKeyDown(key) && !_previous.IsKeyDown(key);
    }
    public bool IsDown(Keys key)
    {
        return _current.IsKeyDown(key);
    }
}