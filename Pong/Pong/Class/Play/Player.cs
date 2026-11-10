using Class;
using Class.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace Pong.Class.Play
{
    public class Player
    {
        public PixelGeneric PixelGeneric { get; set; }
        public Vector2 Velocity { get; set; }

        public float Speed {  get; set; }
        public Player(PixelGeneric pixelGeneric, int width, int height, Vector2 velocity, float speed)
        {
            PixelGeneric = pixelGeneric;
            PixelGeneric.Width = width;
            PixelGeneric.Height = height;
            Velocity = velocity;
            Speed = speed;
        }

        public void Update(float deltaTime, InputManager _input, int playerID, Game1 game)
        {
            float dir = 0;

            if (playerID == 0)
            {
                if (_input.IsDown(InputAction.P1Up)) dir -= 1;
                if (_input.IsDown(InputAction.P1Down)) dir += 1;
            }
            else if (playerID == 1)
            {
                if (_input.IsDown(InputAction.P2Up)) dir -= 1;
                if (_input.IsDown(InputAction.P2Down)) dir += 1;
            }

            Velocity = new Vector2(0, dir * Speed);
            PixelGeneric.Position += Velocity * deltaTime;

            float maxY = Game1.VirtualHeight - PixelGeneric.Height;
            float y = MathHelper.Clamp(PixelGeneric.Position.Y, 0, maxY);
            PixelGeneric.Position = new Vector2(PixelGeneric.Position.X, y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PixelGeneric.FillRectangle(spriteBatch);
        }
    }
}
