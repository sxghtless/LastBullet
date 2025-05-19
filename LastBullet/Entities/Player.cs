using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LastBullet.Entities
{
    public enum PlayerActionType
    {
        None,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Shoot,
        PlaceTrap,
        Dodge,
        Reload
    }

    public class PlayerAction
    {
        public PlayerActionType ActionType = PlayerActionType.None;
        public Point? TargetGridPosition;
        public Vector2? ShootTarget;
    }

    public class Player
    {
        public Point GridPosition;
        public Texture2D FrontTexture, BackTexture;
        public Texture2D CurrentTexture;
        public float Scale => CurrentTexture == BackTexture ? 0.3f : 0.14f;

        private Vector2 _gridStart;
        private int _gridCellSize;

        private bool _isTrapStunned = false;
        private int _trapStunDuration = 0;
        private const int MaxTrapStun = 1;

        public Player(Texture2D front, Texture2D back, Vector2 gridStart, int gridCellSize)
        {
            FrontTexture = front;
            BackTexture = back;
            CurrentTexture = front;
            _gridStart = gridStart;
            _gridCellSize = gridCellSize;
            GridPosition = new Point(0, 0);
        }

        public void Update(GameTime gameTime)
        {
            if (_isTrapStunned)
            {
                _trapStunDuration--;
                if (_trapStunDuration <= 0)
                    _isTrapStunned = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = new Vector2(GridPosition.X * _gridCellSize + _gridStart.X,
                                      GridPosition.Y * _gridCellSize + _gridStart.Y);

            pos.X += (_gridCellSize - CurrentTexture.Width * Scale) / 2;
            pos.Y += (_gridCellSize - CurrentTexture.Height * Scale) / 2;

            Color drawColor = _isTrapStunned ? Color.Red * 0.7f : Color.White;
            spriteBatch.Draw(CurrentTexture, pos, null, drawColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }

        public Vector2 GetCenterPosition()
        {
            return new Vector2(GridPosition.X * _gridCellSize + _gridStart.X + _gridCellSize / 2,
                               GridPosition.Y * _gridCellSize + _gridStart.Y + _gridCellSize / 2);
        }

        public void TriggerTrapEffect()
        {
            _isTrapStunned = true;
            _trapStunDuration = MaxTrapStun;
        }

        public bool IsStunned() => _isTrapStunned;

        public void ApplyAction(PlayerAction action)
        {
            if (_isTrapStunned) return;

            switch (action.ActionType)
            {
                case PlayerActionType.MoveUp:
                    if (GridPosition.Y > 0)
                    {
                        GridPosition = new Point(GridPosition.X, GridPosition.Y - 1);
                        CurrentTexture = BackTexture;
                    }
                    break;

                case PlayerActionType.MoveDown:
                    if (GridPosition.Y < 3)
                    {
                        GridPosition = new Point(GridPosition.X, GridPosition.Y + 1);
                        CurrentTexture = FrontTexture;
                    }
                    break;

                case PlayerActionType.MoveLeft:
                    if (GridPosition.X > 0)
                    {
                        GridPosition = new Point(GridPosition.X - 1, GridPosition.Y);
                        CurrentTexture = FrontTexture;
                    }
                    break;

                case PlayerActionType.MoveRight:
                    if (GridPosition.X < 3)
                    {
                        GridPosition = new Point(GridPosition.X + 1, GridPosition.Y);
                        CurrentTexture = FrontTexture;
                    }
                    break;

                case PlayerActionType.Dodge:
                    break;

            }
        }
    }
}
