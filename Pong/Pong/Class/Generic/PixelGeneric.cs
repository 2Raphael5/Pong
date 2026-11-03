using Class.DebugTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Class.Generic
{
    public class PixelGeneric
    {
        public Vector2 Position;
        public Color OriginalColor;
        public Color Color;
        public int Height  = 10;
        public int Width = 10;
        private System.Drawing.Color white;

        public PixelGeneric(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
            OriginalColor = color;
        }

        public PixelGeneric(System.Numerics.Vector2 position, System.Drawing.Color white)
        {
            Position = position;
            this.white = white;
        }

        public void DrawRectangle(SpriteBatch spriteBatch, int thickness = 10)
        {
            Rectangle rect = new ((int)Position.X, (int)Position.Y, Width, Height);
            PrimitiveRenderer.DrawRectangle(spriteBatch, rect, Color, thickness);
        }
    }
}
