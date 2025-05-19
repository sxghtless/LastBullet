using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LastBullet.Entities
{
    public class Trap
    {
        public Point GridPosition { get; private set; }
        private Vector2 gridStart;
        private int gridCellSize;
        private Texture2D texture;
        private float scale = 0.10f;

        public bool IsActive { get; private set; } = true;

        public Trap(Point gridPosition, Vector2 gridStart, int gridCellSize, Texture2D texture)
        {
            GridPosition = gridPosition;
            this.gridStart = gridStart;
            this.gridCellSize = gridCellSize;
            this.texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive)
                return;

            Vector2 pos = new Vector2(GridPosition.X * gridCellSize + gridStart.X,
                GridPosition.Y * gridCellSize + gridStart.Y);

            pos.X += (gridCellSize - texture.Width * scale) / 2;
            pos.Y += (gridCellSize - texture.Height * scale) / 2;

            spriteBatch.Draw(texture, pos, null, Color.White * 0.8f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
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