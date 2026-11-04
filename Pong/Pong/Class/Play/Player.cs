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
        public Player(PixelGeneric pixelGeneric, int width, int height, Vector2 velocity)
        {
            PixelGeneric = pixelGeneric;
            PixelGeneric.Width = width;
            PixelGeneric.Height = height;
            Velocity = velocity;
        }

        public void Update(float deltaTime, InputManager _input, int playerID, Game game)
        {
            if (playerID == 0)
            {
                if (_input.IsDown(InputAction.P1Up))
                {
                    Velocity = new Vector2(0, Velocity.Y - 10);
                }
                if (_input.IsDown(InputAction.P1Down))
                {
                    Velocity = new Vector2(0, Velocity.Y + 10);
                }
            }


            if (playerID == 1)
            {
                if (_input.IsDown(InputAction.P2Up))
                {
                    Velocity = new Vector2(0, Velocity.Y - 10);
                }
                if (_input.IsDown(InputAction.P2Down))
                {
                    Velocity = new Vector2(0, Velocity.Y + 10);
                }
            }

            PixelGeneric.Position += Velocity * deltaTime;


            if(PixelGeneric.Position.Y < 0 || PixelGeneric.Position.Y > game.Window.ClientBounds.Height - PixelGeneric.Height)
            {
                Velocity *= -1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PixelGeneric.DrawRectangle(spriteBatch);
        }
    }
}
