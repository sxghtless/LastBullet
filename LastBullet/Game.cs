using System;
using System.Collections.Generic;
using System.IO;
using LastBullet.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LastBullet.Entities;

namespace LastBullet
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _arenaTexture;
        private Texture2D _playerFrontTexture;
        private Texture2D _playerBackTexture;
        private Texture2D _enemyFrontTexture;
        private Texture2D _enemyBackTexture;
        private Texture2D _bulletTexture;
        private Texture2D _trapTexture;

        private Player _player;
        private Enemy _enemy;
        private List<Bullet> _bullets = new List<Bullet>();
        private List<Trap> _traps = new List<Trap>();
        private List<Trap> _enemyTraps = new List<Trap>();

        private Random _random = new Random();

        private Vector2 _gridStart = new Vector2(255, 255);
        private int _gridCellSize = 130;

        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        
        private int _maxEnemyTraps = 1;
        private int _maxTraps = 1;
        private int _shotsFired = 0;
        private int _maxShots = 3;
        private bool _needsReload = false;

        private bool _isPlayerTurn = true;
        private bool _actionSelected = false;

        private EnemyAction _selectedEnemyAction;
        private PlayerAction _pendingPlayerAction = new PlayerAction();

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _playerFrontTexture = TextureUtils.ResizeToSquare(GraphicsDevice, _spriteBatch, Content.Load<Texture2D>("CharacterFront"), 1024);
            _playerBackTexture = TextureUtils.ResizeToSquare(GraphicsDevice, _spriteBatch, Content.Load<Texture2D>("CharacterBack"), 1024);
            _enemyFrontTexture = TextureUtils.ResizeToSquare(GraphicsDevice, _spriteBatch, Content.Load<Texture2D>("EnemyFront"), 1024);
            _enemyBackTexture = TextureUtils.ResizeToSquare(GraphicsDevice, _spriteBatch, Content.Load<Texture2D>("EnemyBack"), 1024);

            _bulletTexture = CreateBulletTexture();

            using (var stream = File.OpenRead("Content/Trap.png"))
            {
                var rawTrapTexture = Texture2D.FromStream(GraphicsDevice, stream);
                _trapTexture = TextureUtils.ResizeToSquare(GraphicsDevice, _spriteBatch, rawTrapTexture, 1024);
            }

            using (var stream = File.OpenRead("Content/Arena.png"))
                _arenaTexture = Texture2D.FromStream(GraphicsDevice, stream);

            _graphics.PreferredBackBufferWidth = _arenaTexture.Width;
            _graphics.PreferredBackBufferHeight = _arenaTexture.Height;
            _graphics.ApplyChanges();

            _player = new Player(_playerFrontTexture, _playerBackTexture, _gridStart, _gridCellSize);
            _enemy = new Enemy(_enemyFrontTexture, _enemyBackTexture, _gridStart, _gridCellSize);

            _previousMouseState = Mouse.GetState();
            _previousKeyboardState = Keyboard.GetState();
        }

        private Texture2D CreateBulletTexture()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 5, 5);
            Color[] data = new Color[25];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Red;

            texture.SetData(data);
            return texture;
        }

        private Point GetGridCoordinates(int mouseX, int mouseY)
        {
            int gridX = (int)((mouseX - _gridStart.X) / _gridCellSize);
            int gridY = (int)((mouseY - _gridStart.Y) / _gridCellSize);

            if (gridX >= 0 && gridX <= 3 && gridY >= 0 && gridY <= 3)
                return new Point(gridX, gridY);
            else
                return new Point(-1, -1);
        }

        private bool CanPlaceTrapAt(Point gridPosition)
        {
            int diffX = Math.Abs(gridPosition.X - _player.GridPosition.X);
            int diffY = Math.Abs(gridPosition.Y - _player.GridPosition.Y);

            foreach (var trap in _traps)
            {
                if (trap.GridPosition == gridPosition && trap.IsActive)
                    return false;
            }

            return diffX <= 1 && diffY <= 1 && !(diffX == 0 && diffY == 0);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboard.IsKeyDown(Keys.F11))
            {
                _graphics.IsFullScreen = !_graphics.IsFullScreen;
                _graphics.ApplyChanges();
            }

            _player.Update(gameTime);
            _enemy.Update();

            if (_isPlayerTurn && !_actionSelected)
            {
                HandlePlayerAction(gameTime, keyboard, mouse);
            }
            else if (!_isPlayerTurn && !_actionSelected)
            {
                _selectedEnemyAction = _enemy.DecideAction(_player.GridPosition);
                _actionSelected = true;

                ExecuteEnemyAction(_selectedEnemyAction);

                _isPlayerTurn = true;
                _actionSelected = false;
            }

            for (int i = _enemyTraps.Count - 1; i >= 0; i--)
            {
                if (_enemyTraps[i].CheckTrigger(_player.GridPosition))
                {
                    _player.TriggerTrapEffect();
                    Console.WriteLine("Игрок наступил на ловушку врага!");
                }
            }

            for (int i = _traps.Count - 1; i >= 0; i--)
            {
                if (_traps[i].CheckTrigger(_enemy.GridPosition))
                {
                    _enemy.TriggerTrapEffect();
                    Console.WriteLine("Враг наступил на ловушку!");
                }
            }

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update();
                if (_bullets[i].IsOffScreen(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight))
                    _bullets.RemoveAt(i);
            }

            _previousMouseState = mouse;
            _previousKeyboardState = keyboard;
            base.Update(gameTime);
        }

        private void HandlePlayerAction(GameTime gameTime, KeyboardState keyboard, MouseState mouse)
        {
            if (_pendingPlayerAction.ActionType == PlayerActionType.None)
            {
                if (keyboard.IsKeyDown(Keys.W)) _pendingPlayerAction.ActionType = PlayerActionType.MoveUp;
                else if (keyboard.IsKeyDown(Keys.S)) _pendingPlayerAction.ActionType = PlayerActionType.MoveDown;
                else if (keyboard.IsKeyDown(Keys.A)) _pendingPlayerAction.ActionType = PlayerActionType.MoveLeft;
                else if (keyboard.IsKeyDown(Keys.D)) _pendingPlayerAction.ActionType = PlayerActionType.MoveRight;
                else if (mouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                    _pendingPlayerAction.ActionType = PlayerActionType.Shoot;
                else if (mouse.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
                    _pendingPlayerAction.ActionType = PlayerActionType.PlaceTrap;
                else if (keyboard.IsKeyDown(Keys.LeftShift)) _pendingPlayerAction.ActionType = PlayerActionType.Dodge;
                else if (keyboard.IsKeyDown(Keys.R)) _pendingPlayerAction.ActionType = PlayerActionType.Reload;
            }

            if (_pendingPlayerAction.ActionType != PlayerActionType.None && keyboard.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                ExecutePlayerAction(_pendingPlayerAction);
                _pendingPlayerAction = new PlayerAction();
                _isPlayerTurn = false;
            }
        }

        private void ExecutePlayerAction(PlayerAction action)
        {
            if (_player.IsStunned())
            {
                Console.WriteLine("Игрок оглушён, действие невозможно");
                return;
            }

            switch (action.ActionType)
            {
                case PlayerActionType.MoveUp:
                case PlayerActionType.MoveDown:
                case PlayerActionType.MoveLeft:
                case PlayerActionType.MoveRight:
                    _player.ApplyAction(action);
                    break;

                case PlayerActionType.Shoot:
                    if (_needsReload)
                    {
                        Console.WriteLine("Нужно перезарядиться");
                        return;
                    }

                    _shotsFired++;
                    if (_shotsFired >= _maxShots)
                        _needsReload = true;

                    var bullet = new Bullet(_player.GetCenterPosition(), _enemy.GetCenterPosition(), _bulletTexture);
                    _bullets.Add(bullet);
                    break;

                case PlayerActionType.PlaceTrap:
                    if (_traps.Count >= _maxTraps)
                    {
                        Console.WriteLine("Максимум ловушек достигнут");
                        return;
                    }

                    Point mouseGrid = GetGridCoordinates(Mouse.GetState().X, Mouse.GetState().Y);

                    if (mouseGrid.X != -1 && CanPlaceTrapAt(mouseGrid))
                    {
                        var trap = new Trap(mouseGrid, _gridStart, _gridCellSize, _trapTexture);
                        _traps.Add(trap);
                        Console.WriteLine("Ловушка установлена");
                    }
                    else
                    {
                        Console.WriteLine("Нельзя поставить ловушку здесь");
                        return;
                    }
                    break;

                case PlayerActionType.Dodge:
                    Console.WriteLine("Уклонение!");
                    break;

                case PlayerActionType.Reload:
                    _needsReload = false;
                    _shotsFired = 0;
                    Console.WriteLine("Перезарядка");
                    break;
            }
        }

        private void ExecuteEnemyAction(EnemyAction action)
        {
            if (_enemy.IsStunned())
            {
                Console.WriteLine("Враг оглушён и не может ходить.");
                return;
            }

            switch (action)
            {
                case EnemyAction.Move:
                    _enemy.MoveTowards(_player.GridPosition);
                    break;

                case EnemyAction.Shoot:
                    var bullet = new Bullet(_enemy.GetCenterPosition(), GridToWorldPosition(_player.GridPosition), _bulletTexture);
                    _bullets.Add(bullet);
                    break;

                case EnemyAction.PlaceTrap:
                    if (_enemyTraps.Count >= _maxEnemyTraps)
                    {
                        Console.WriteLine("Враг не может поставить больше ловушек");
                        break;
                    }

                    Point trapPos = _enemy.GetTrapPlacementPosition(_player.GridPosition);
                    if (!_enemyTraps.Exists(t => t.GridPosition == trapPos))
                    {
                        var trap = new Trap(trapPos, _gridStart, _gridCellSize, _trapTexture);
                        _enemyTraps.Add(trap);
                        Console.WriteLine("Враг поставил ловушку");
                    }
                    break;

                case EnemyAction.Dodge:
                    Console.WriteLine("Враг уклоняется.");
                    break;

                case EnemyAction.Reload:
                    Console.WriteLine("Враг перезаряжается.");
                    break;
            }
        }


        private Vector2 GridToWorldPosition(Point gridPos)
        {
            return new Vector2(
                gridPos.X * _gridCellSize + _gridStart.X + _gridCellSize / 2,
                gridPos.Y * _gridCellSize + _gridStart.Y + _gridCellSize / 2
            );
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_arenaTexture, Vector2.Zero, Color.White);

            foreach (var trap in _traps)
                trap.Draw(_spriteBatch);

            foreach (var trap in _enemyTraps)
                trap.Draw(_spriteBatch);

            foreach (var bullet in _bullets)
                bullet.Draw(_spriteBatch);

            _player.Draw(_spriteBatch);
            _enemy.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
