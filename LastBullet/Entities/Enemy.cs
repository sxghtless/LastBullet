using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LastBullet.Entities
{
    public enum EnemyAction
    {
        Move,
        Shoot,
        Dodge,
        PlaceTrap,
        Reload
    }
    
    public class Enemy
    {
        public Point GridPosition;
        public Texture2D FrontTexture, BackTexture;
        public Texture2D CurrentTexture;
        public float Scale => 0.14f;
        
        private Vector2 _gridStart;
        private int _gridCellSize;
        private Random _random = new Random();
        private int _shotsFired = 0;
        private int _maxShots = 3;
        private bool _needsReload = false;
        private bool _isTrapStunned = false;
        private int _trapStunDuration = 0;
        private const int MaxTrapStun = 60; 
        
        public Enemy(Texture2D front, Texture2D back, Vector2 gridStart, int gridCellSize)
        {
            this.FrontTexture = front;
            this.BackTexture = back;
            this.CurrentTexture = front;
            this._gridStart = gridStart;
            this._gridCellSize = gridCellSize;
            GridPosition = new Point(3, 3);
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
        
        public void Update()
        {
            if (_isTrapStunned)
            {
                _trapStunDuration--;
                if (_trapStunDuration <= 0)
                {
                    _isTrapStunned = false;
                }
            }
        }
        
        public EnemyAction DecideAction(Point playerPosition)
        {
            if (_needsReload)
            {
                _needsReload = false;
                _shotsFired = 0;
                return EnemyAction.Reload;
            }
            
            if (_isTrapStunned)
                return EnemyAction.Dodge;
                
            int distance = Math.Abs(GridPosition.X - playerPosition.X) + Math.Abs(GridPosition.Y - playerPosition.Y);
            
            int choice = _random.Next(100);
            
            if (distance <= 1)
            {
                if (choice < 60 && _shotsFired < _maxShots)
                {
                    _shotsFired++;
                    if (_shotsFired >= _maxShots)
                        _needsReload = true;
                    return EnemyAction.Shoot;
                }
                else if (choice < 85)
                {
                    return EnemyAction.Dodge;
                }
                else
                {
                    return EnemyAction.Move;
                }
            }
            else if (distance <= 2)
            {
                if (choice < 40 && _shotsFired < _maxShots)
                {
                    _shotsFired++;
                    if (_shotsFired >= _maxShots)
                        _needsReload = true;
                    return EnemyAction.Shoot;
                }
                else if (choice < 70)
                {
                    return EnemyAction.Move;
                }
                else if (choice < 90)
                {
                    return EnemyAction.PlaceTrap;
                }
                else
                {
                    return EnemyAction.Dodge;
                }
            }
            else
            {
                if (choice < 60)
                {
                    return EnemyAction.Move;
                }
                else if (choice < 80)
                {
                    return EnemyAction.PlaceTrap;
                }
                else if (choice < 90 && _shotsFired < _maxShots)
                {
                    _shotsFired++;
                    if (_shotsFired >= _maxShots)
                        _needsReload = true;
                    return EnemyAction.Shoot;
                }
                else
                {
                    return EnemyAction.Dodge;
                }
            }
        }
        
        public void MoveTowards(Point playerPosition, bool awayFromPlayer = false)
        {
            if (_isTrapStunned)
                return;
                
            int dx = playerPosition.X - GridPosition.X;
            int dy = playerPosition.Y - GridPosition.Y;
            
            if (awayFromPlayer)
            {
                dx = -dx;
                dy = -dy;
            }
            
            if (Math.Abs(dx) > Math.Abs(dy) || (Math.Abs(dx) == Math.Abs(dy) && _random.Next(2) == 0))
            {
                if (dx > 0 && GridPosition.X < 3)
                {
                    GridPosition.X++;
                    CurrentTexture = FrontTexture;
                }
                else if (dx < 0 && GridPosition.X > 0)
                {
                    GridPosition.X--;
                    CurrentTexture = FrontTexture;
                }
            }
            else
            {
                if (dy > 0 && GridPosition.Y < 3)
                {
                    GridPosition.Y++;
                    CurrentTexture = FrontTexture;
                }
                else if (dy < 0 && GridPosition.Y > 0)
                {
                    GridPosition.Y--;
                    CurrentTexture = BackTexture;
                }
            }
        }
        
        public bool IsStunned()
        {
            return _isTrapStunned;
        }
        
        public Point GetTrapPlacementPosition(Point playerPosition)
        {
            List<Point> possiblePositions = new List<Point>();
            
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                        
                    int newX = GridPosition.X + dx;
                    int newY = GridPosition.Y + dy;
                    
                    if (newX >= 0 && newX <= 3 && newY >= 0 && newY <= 3)
                    {
                        int distanceToPlayer = Math.Abs(newX - playerPosition.X) + Math.Abs(newY - playerPosition.Y);
                        if (distanceToPlayer <= 2)
                        {
                            possiblePositions.Add(new Point(newX, newY));
                        }
                    }
                }
            }
            
            if (possiblePositions.Count == 0)
                return GridPosition;
                
            return possiblePositions[_random.Next(possiblePositions.Count)];
        }
    }
}