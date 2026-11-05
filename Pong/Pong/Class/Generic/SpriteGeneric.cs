using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
namespace Class.Generic
{
    public class SpriteGeneric
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; } = Vector2.Zero; 
        public float Drag { get; set; } = 0.5f;

        public bool isDrag = false;
        public int Width => SourceRectangle?.Width ?? Texture.Width;
        public int Height => SourceRectangle?.Height ?? Texture.Height;
        public Rectangle ColliderRect => new Rectangle((int)(Position.X - Width / 4f), (int)(Position.Y - Height / 4f), Width / 2, Height / 2);
        public Rectangle? SourceRectangle { get; set; }
        public Color Tint { get; set; } = Color.White;
        public SpriteGeneric(Texture2D texture)
        {
            Texture = texture;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
        }
        public void Update(float deltaTime)
        {
            if (isDrag)
            {
                Velocity += Acceleration * deltaTime;
                Velocity *= MathF.Pow(Drag, deltaTime);
            }
            Position += Velocity * deltaTime;
            Debug.WriteLine(Velocity);
        }

        public void Draw(SpriteBatch spriteBatch, float rotation = 0f)
        {
            Vector2 origin = new Vector2(Width / 10f, Height / 10f);
            spriteBatch.Draw(
            Texture,
            Position,
            SourceRectangle,
            Tint,
            MathHelper.ToRadians(rotation),
            origin,
            0.1f,
            SpriteEffects.None,
            0f
            );

        }
    }
}
