using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LastBullet.Entities
{
    public class Trap
    {
        public Point GridPosition { get; private set; }
        private Vector2 _gridStart;
        private int _gridCellSize;
        private Texture2D _texture;
        private float _scale = 0.10f;

        public bool IsActive { get; private set; } = true;

        public Trap(Point gridPosition, Vector2 gridStart, int gridCellSize, Texture2D texture)
        {
            GridPosition = gridPosition;
            this._gridStart = gridStart;
            this._gridCellSize = gridCellSize;
            this._texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive)
                return;

            Vector2 pos = new Vector2(GridPosition.X * _gridCellSize + _gridStart.X,
                GridPosition.Y * _gridCellSize + _gridStart.Y);

            pos.X += (_gridCellSize - _texture.Width * _scale) / 2;
            pos.Y += (_gridCellSize - _texture.Height * _scale) / 2;

            spriteBatch.Draw(_texture, pos, null, Color.White * 0.8f, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }

        public bool CheckTrigger(Point position)
        {
            if (!IsActive)
                return false;

            if (position.X == GridPosition.X && position.Y == GridPosition.Y)
            {
                IsActive = false;
                return true;
            }
            return false;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}