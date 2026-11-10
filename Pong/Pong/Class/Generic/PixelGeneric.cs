using Class.DebugTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Class.Generic
{
    public class PixelGeneric
    {
        // Position du rectangle sur l'écran.
        public Vector2 Position;

        // Couleur d'origine de l'objet, conservée pour pouvoir la réutiliser.
        public Color OriginalColor;

        // Couleur actuellement utilisée pour dessiner l'objet.
        public Color Color;

        // Hauteur du rectangle en pixels.
        public int Height = 10;

        // Largeur du rectangle en pixels.
        public int Width = 10;

        // Stocke une couleur provenant de la bibliothèque System.Drawing.
        private System.Drawing.Color white;

        // Initialise un rectangle avec sa position et sa couleur.
        public PixelGeneric(Vector2 position, Color color)
        {
            Position = position;
            Color = color;

            // Conserve la couleur initiale de l'objet.
            OriginalColor = color;
        }

        // Initialise un rectangle avec une position et une couleur
        // provenant des bibliothèques System.Numerics et System.Drawing.
        public PixelGeneric(System.Numerics.Vector2 position, System.Drawing.Color white)
        {
            Position = position;
            this.white = white;
        }

        // Dessine un rectangle à la position définie.
        // Le paramètre thickness permet de choisir l'épaisseur du contour.
        public void DrawRectangle(SpriteBatch spriteBatch, int thickness = 10)
        {
            // Crée un rectangle à partir de sa position et de ses dimensions.
            Rectangle rect = new(
                (int)Position.X,
                (int)Position.Y,
                Width,
                Height
            );

            // Dessine le rectangle avec sa couleur et l'épaisseur indiquées.
            PrimitiveRenderer.DrawRectangle(
                spriteBatch,
                rect,
                Color,
                thickness
            );
        }
    }
}