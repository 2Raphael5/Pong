using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Class.DebugTools
{
    public static class PrimitiveRenderer
    {
        private static Texture2D _pixel;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            spriteBatch.Draw(_pixel,
            new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness), null,
            color,
            angle,
            Vector2.Zero,
            SpriteEffects.None,
            0);
        }
        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness = 1)
        {
            DrawLine(spriteBatch, new Vector2(rect.Left, rect.Top), new Vector2(rect.Right, rect.Top), color, thickness);
            DrawLine(spriteBatch, new Vector2(rect.Right, rect.Top), new Vector2(rect.Right, rect.Bottom), color, thickness);
            DrawLine(spriteBatch, new Vector2(rect.Right, rect.Bottom), new Vector2(rect.Left, rect.Bottom), color, thickness);
            DrawLine(spriteBatch, new Vector2(rect.Left, rect.Bottom), new Vector2(rect.Left, rect.Top), color, thickness);
        }
        public static void DrawPoint(SpriteBatch spriteBatch, Vector2 position, Color color, int size = 4)
        {
            spriteBatch.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, size, size), color);
        }

        public static void DrawTriangle(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, Vector2 p3, Color color, int thickness = 1)
        {
            DrawLine(spriteBatch, p1, p2, color, thickness);
            DrawLine(spriteBatch, p2, p3, color, thickness);
            DrawLine(spriteBatch, p3, p1, color, thickness);
        }
        public static void DrawVector(SpriteBatch spriteBatch, Vector2 start, Vector2 vector, Color color, int thickness = 2)
        {
            Vector2 end = start + vector;
            DrawLine(spriteBatch, start, end, color, thickness);
            Vector2 dir = Vector2.Normalize(vector);
            Vector2 perp = new Vector2(-dir.Y, dir.X);
            float arrowSize = 10f;
            Vector2 arrowP1 = end - dir * arrowSize + perp * arrowSize * 0.5f;
            Vector2 arrowP2 = end - dir * arrowSize - perp * arrowSize * 0.5f;
            DrawTriangle(spriteBatch, end, arrowP1, arrowP2, color, thickness);
        }

    }
}
