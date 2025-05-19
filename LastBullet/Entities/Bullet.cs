using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LastBullet.Entities
{
    public class Bullet
    {
        public Vector2 Position;
        private Vector2 velocity;
        private float speed = 5f;
        private Texture2D texture;

        public Bullet(Vector2 start, Vector2 target, Texture2D texture)
        {
            Position = start;
            Vector2 direction = target - start;
            if (direction != Vector2.Zero)
                direction.Normalize();
            velocity = direction * speed;
            this.texture = texture;
        }

        public void Update()
        {
            Position += velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public bool IsOffScreen(int screenWidth, int screenHeight)
        {
            return Position.X < 0 || Position.Y < 0 || Position.X > screenWidth || Position.Y > screenHeight;
        }
    }
}