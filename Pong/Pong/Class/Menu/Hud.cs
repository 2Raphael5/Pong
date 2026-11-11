using Class.DebugTools;
using Class.Generic;
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
        private readonly SpriteFont Font;
        public Hud(SpriteFont font)
        {
            Font = font;
        }
        public void DrawMenue(SpriteBatch spriteBatch, int scoreP1, int scoreP2, Game game, Color fontColor)
        {


            Vector2 textSize = Font.MeasureString($"{scoreP2}");
            spriteBatch.DrawString(Font, $"{scoreP1}", new Vector2(game.Window.ClientBounds.Width/4, game.Window.ClientBounds.Height / 10), fontColor);

            spriteBatch.DrawString(Font, $"{scoreP2}", new Vector2(game.Window.ClientBounds.Width- textSize.X - game.Window.ClientBounds.Width / 4, game.Window.ClientBounds.Height / 10), fontColor);            
        }

        public void DrawTerrain(SpriteBatch spriteBatch, Game game, Color lineColor, int terrainThickness= 1, int lineThickness = 1, int nbPoint = 10)
        {
            PrimitiveRenderer.DrawLinePoint(spriteBatch, new Vector2(game.Window.ClientBounds.Width/2, 0), new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height), nbPoint, lineColor, lineThickness);
            //PrimitiveRenderer.DrawRectangle(spriteBatch, new Microsoft.Xna.Framework.Rectangle(0,0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height), lineColor, terrainThickness);
            PrimitiveRenderer.DrawLine(spriteBatch, new Vector2(0, 0), new Vector2(game.Window.ClientBounds.Width, 0), lineColor, terrainThickness);
            PrimitiveRenderer.DrawLine(spriteBatch, new Vector2(0, game.Window.ClientBounds.Height- terrainThickness), new Vector2(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height - terrainThickness), lineColor, terrainThickness);
        }

    }
}
