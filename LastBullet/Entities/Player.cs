using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LastBullet.Entities
{
    public class Player
    {
        public Point GridPosition;
        public Texture2D FrontTexture, BackTexture;
        public Texture2D CurrentTexture;
        public float Scale => CurrentTexture == BackTexture ? 0.3f : 0.14f;

        private Vector2 gridStart;
        private int gridCellSize;
        private bool canMove = true;

        public Player(Texture2D front, Texture2D back, Vector2 gridStart, int gridCellSize)
        {
            this.FrontTexture = front;
            this.BackTexture = back;
            this.CurrentTexture = front;
            this.gridStart = gridStart;
            this.gridCellSize = gridCellSize;
            GridPosition = new Point(0, 0);
        }

        public void Update(KeyboardState keyboard)
        {
            if (canMove)
            {
                if (keyboard.IsKeyDown(Keys.D) && GridPosition.X < 3) { GridPosition.X++; canMove = false; }
                if (keyboard.IsKeyDown(Keys.A) && GridPosition.X > 0) { GridPosition.X--; canMove = false; }
                if (keyboard.IsKeyDown(Keys.W) && GridPosition.Y > 0)
                {
                    GridPosition.Y--; CurrentTexture = BackTexture; canMove = false;
                }
                if (keyboard.IsKeyDown(Keys.S) && GridPosition.Y < 3)
                {
                    GridPosition.Y++; CurrentTexture = FrontTexture; canMove = false;
                }
            }

            if (!keyboard.IsKeyDown(Keys.W) && !keyboard.IsKeyDown(Keys.S) &&
                !keyboard.IsKeyDown(Keys.A) && !keyboard.IsKeyDown(Keys.D))
            {
                canMove = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = new Vector2(GridPosition.X * gridCellSize + gridStart.X,
                                      GridPosition.Y * gridCellSize + gridStart.Y);

            pos.X += (gridCellSize - CurrentTexture.Width * Scale) / 2;
            pos.Y += (gridCellSize - CurrentTexture.Height * Scale) / 2;

            spriteBatch.Draw(CurrentTexture, pos, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }

        public Vector2 GetCenterPosition()
        {
            return new Vector2(GridPosition.X * gridCellSize + gridStart.X + gridCellSize / 2,
                               GridPosition.Y * gridCellSize + gridStart.Y + gridCellSize / 2);
        }
    }
}
