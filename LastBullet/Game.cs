using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace LastBullet
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private Texture2D arenaTexture;
        private Texture2D playerTexture;
        
        int cellSize = 130;
        Point playerPosition = new Point(0, 0);

        int gridSize = 4;

        bool canMove = true;

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("CharacterFront");
            using (var stream = File.OpenRead("Content/Arena.png"))
            {
                arenaTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }
            _graphics.PreferredBackBufferWidth = arenaTexture.Width;
            _graphics.PreferredBackBufferHeight = arenaTexture.Height;
            _graphics.ApplyChanges();
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.F11))
            {
                _graphics.IsFullScreen = !_graphics.IsFullScreen;
                _graphics.ApplyChanges();
            }

            if (canMove)
            {
                if (keyboard.IsKeyDown(Keys.D) && playerPosition.X < gridSize - 1)
                {
                    playerPosition.X++;
                    canMove = false;
                }
                if (keyboard.IsKeyDown(Keys.A) && playerPosition.X > 0)
                {
                    playerPosition.X--;
                    canMove = false;
                }
                if (keyboard.IsKeyDown(Keys.W) && playerPosition.Y > 0)
                {
                    playerPosition.Y--;
                    canMove = false;
                }
                if (keyboard.IsKeyDown(Keys.S) && playerPosition.Y < gridSize - 1)
                {
                    playerPosition.Y++;
                    canMove = false;
                }
            }

            if (!keyboard.IsKeyDown(Keys.D) && !keyboard.IsKeyDown(Keys.A) &&
                !keyboard.IsKeyDown(Keys.W) && !keyboard.IsKeyDown(Keys.S))
            {
                canMove = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Размеры экрана
            int screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // Размеры текстуры (для изображения, которое на фоне)
            int texWidth = arenaTexture.Width;
            int texHeight = arenaTexture.Height;

            // Масштабирование текстуры фона
            float scale = Math.Min((float)screenWidth / texWidth, (float)screenHeight / texHeight);

            int drawWidth = (int)(texWidth * scale);
            int drawHeight = (int)(texHeight * scale);

            int offsetX = (screenWidth - drawWidth) / 2;
            int offsetY = (screenHeight - drawHeight) / 2;

            Rectangle destination = new Rectangle(offsetX, offsetY, drawWidth, drawHeight);
            _spriteBatch.Draw(arenaTexture, destination, Color.White);

            Vector2 gridStart = new Vector2(255, 255);
            int gridCellSize = 130;

            Vector2 playerScreenPos = new Vector2(playerPosition.X * gridCellSize + gridStart.X, playerPosition.Y * gridCellSize + gridStart.Y);

            playerScreenPos.X += (gridCellSize - playerTexture.Width * 0.1f) / 2;
            playerScreenPos.Y += (gridCellSize - playerTexture.Height * 0.1f) / 2;

            float playerScale = 0.1f;
            _spriteBatch.Draw(playerTexture, playerScreenPos, null, Color.White, 0f, Vector2.Zero, playerScale, SpriteEffects.None, 0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }   
    }
}
