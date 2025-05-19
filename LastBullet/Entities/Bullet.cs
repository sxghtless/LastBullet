using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LastBullet.Entities
{
    public class Bullet
    {
        public Vector2 Position;
        private Vector2 _velocity;
        private float _speed = 5f;
        private Texture2D _texture;

        public Bullet(Vector2 start, Vector2 target, Texture2D texture)
        {
            Position = start;
            Vector2 direction = target - start;
            if (direction != Vector2.Zero)
                direction.Normalize();
            _velocity = direction * _speed;
            this._texture = texture;
        }

        public void Update()
        {
            Position += _velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public bool IsOffScreen(int screenWidth, int screenHeight)
        {
            return Position.X < 0 || Position.Y < 0 || Position.X > screenWidth || Position.Y > screenHeight;
        }
    }
}