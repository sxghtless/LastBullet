using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LastBullet.Entities;
using LastBullet.Core;

namespace LastBullet
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D arenaTexture;
        private Texture2D playerFrontTexture;
        private Texture2D playerBackTexture;
        private Texture2D bulletTexture;

        private Player player;
        private List<Bullet> bullets = new List<Bullet>();

        private Vector2 gridStart = new Vector2(255, 255);
        private int gridCellSize = 130;

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerFrontTexture = TextureUtils.ResizeToSquare(GraphicsDevice, _spriteBatch, Content.Load<Texture2D>("CharacterFront"), 1024);
            playerBackTexture = TextureUtils.ResizeToSquare(GraphicsDevice, _spriteBatch, Content.Load<Texture2D>("CharacterBack"), 1024);
            bulletTexture = CreateBulletTexture();

            using (var stream = File.OpenRead("Content/Arena.png"))
                arenaTexture = Texture2D.FromStream(GraphicsDevice, stream);

            _graphics.PreferredBackBufferWidth = arenaTexture.Width;
            _graphics.PreferredBackBufferHeight = arenaTexture.Height;
            _graphics.ApplyChanges();

            player = new Player(playerFrontTexture, playerBackTexture, gridStart, gridCellSize);
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

            player.Update(keyboard);

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                bullets.Add(new Bullet(player.GetCenterPosition(), new Vector2(mouse.X, mouse.Y), bulletTexture));
            }

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();
                if (bullets[i].IsOffScreen(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight))
                    bullets.RemoveAt(i);
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            Rectangle destination = new Rectangle(0, 0, arenaTexture.Width, arenaTexture.Height);
            _spriteBatch.Draw(arenaTexture, destination, Color.White);

            player.Draw(_spriteBatch);

            foreach (var bullet in bullets)
                bullet.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
