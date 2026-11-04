using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Class.DebugTools
{
    internal class DebugRenderer
    {
        public static bool IsEnabled { get; set; }
        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness = 1)
        {
            if (!IsEnabled) return;
            PrimitiveRenderer.DrawRectangle(spriteBatch, rect, color, thickness);
        }
        public static void DrawCross(SpriteBatch spriteBatch, Vector2 center, int size, Color color, int thickness = 1)
        {
            if (!IsEnabled) return;
            PrimitiveRenderer.DrawLine(spriteBatch, center - new Vector2(size, 0), center + new Vector2(size, 0), color, thickness);
            PrimitiveRenderer.DrawLine(spriteBatch, center - new Vector2(0, size), center + new Vector2(0, size), color, thickness);
        }

    }
}
