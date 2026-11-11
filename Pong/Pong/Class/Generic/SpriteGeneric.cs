using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Class.Generic
{
    public class SpriteGeneric
    {
        // Image utilisée pour représenter l'objet à l'écran.
        public Texture2D Texture { get; set; }

        // Position de l'objet sur l'écran (X = horizontal, Y = vertical).
        public Vector2 Position { get; set; }

        // Vitesse de déplacement de l'objet.
        public Vector2 Velocity { get; set; }

        // Accélération appliquée à l'objet lorsqu'il est en mouvement.
        public Vector2 Acceleration { get; set; } = Vector2.Zero;

        // Facteur de ralentissement progressif de l'objet.
        public float Drag { get; set; } = 0.5f;

        // Indique si le ralentissement et l'accélération doivent être appliqués.
        public bool isDrag = false;

        // Largeur de l'objet, basée sur sa zone d'image ou sur sa texture complète.
        public int Width => SourceRectangle?.Width ?? Texture.Width;

        // Hauteur de l'objet, basée sur sa zone d'image ou sur sa texture complète.
        public int Height => SourceRectangle?.Height ?? Texture.Height;

        // Définit la zone de collision de l'objet.
        // Elle correspond ici à la moitié de sa largeur et de sa hauteur.
        public Rectangle ColliderRect => new Rectangle(
            (int)(Position.X - Width / 4f),
            (int)(Position.Y - Height / 4f),
            Width / 2,
            Height / 2
        );

        // Partie de l'image à afficher. Si aucune zone n'est définie,
        // l'image entière sera utilisée.
        public Rectangle? SourceRectangle { get; set; }

        // Couleur appliquée à l'image. Blanc conserve ses couleurs originales.
        public Color Tint { get; set; } = Color.White;

        // Initialise un nouvel objet graphique à partir d'une image.
        public SpriteGeneric(Texture2D texture)
        {
            Texture = texture;

            // Place l'objet à l'origine de l'écran et arrête son déplacement.
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
        }

        // Met à jour la position et le déplacement de l'objet.
        // deltaTime correspond au temps écoulé depuis la dernière image.
        public void Update(float deltaTime)
        {
            // Applique l'accélération et le ralentissement uniquement
            // si le système de freinage est activé.
            if (isDrag)
            {
                // Augmente ou diminue la vitesse selon l'accélération définie.
                Velocity += Acceleration * deltaTime;

                // Réduit progressivement la vitesse de déplacement.
                Velocity *= MathF.Pow(Drag, deltaTime);
            }

            // Déplace l'objet en fonction de sa vitesse actuelle.
            Position += Velocity * deltaTime;

            // Affiche la vitesse actuelle dans la console de débogage.
            Debug.WriteLine(Velocity);
        }

        // Dessine l'objet à l'écran.
        // Le paramètre rotation permet de définir son angle de rotation.
        public void Draw(SpriteBatch spriteBatch, float size, float rotation = 0f)
        {
            // Définit le point autour duquel l'image va être dessinée et tournée.
            Vector2 origin = new Vector2(Width / 10f, Height / 10f);

            // Dessine l'image avec ses paramètres actuels.
            spriteBatch.Draw(
                Texture,                         // Image à afficher.
                Position,                        // Position sur l'écran.
                SourceRectangle,                 // Partie de l'image à afficher.
                Tint,                            // Couleur de l'image.
                MathHelper.ToRadians(rotation),  // Angle de rotation en radians.
                origin,                          // Point de rotation.
                size,                            // Taille de l'image (échelle).
                SpriteEffects.None,              // Aucun effet miroir.
                0f                               // Profondeur d'affichage.
            );
        }
    }
}