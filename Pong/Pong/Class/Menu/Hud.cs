using Class.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;


namespace Class
{
    public class Hud
    {
        private readonly SpriteFont _font;
        public Hud(SpriteFont font)
        {
            _font = font;
        }
        public void DrawMenue(SpriteBatch spriteBatch, int scoreP1, int scoreP2, Game game)
        {


            Vector2 textSize = _font.MeasureString($"J2 : {scoreP2}");
            spriteBatch.DrawString(_font, $"J1 : {scoreP1}", new Vector2(10, 10), Color.White);

            spriteBatch.DrawString(_font, $"J2: {scoreP2}", new Vector2(game.Window.ClientBounds.Width - textSize.X - 10, 10), Color.White);
            //_mainMenu.Draw(_spriteBatch, _font, new Vector2((_graphics.PreferredBackBufferWidth - textSize.X) / 2f, (_graphics.PreferredBackBufferHeight - textSize.Y) / 2f));
        }

    }
}
