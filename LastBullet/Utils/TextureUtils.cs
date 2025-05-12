using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LastBullet.Core
{
    public static class TextureUtils
    {
        public static Texture2D ResizeToSquare(GraphicsDevice device, SpriteBatch spriteBatch, Texture2D original, int newSize)
        {
            RenderTarget2D renderTarget = new RenderTarget2D(device, newSize, newSize);
            device.SetRenderTarget(renderTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin();
            int offsetX = (newSize - original.Width) / 2;
            int offsetY = (newSize - original.Height) / 2;
            spriteBatch.Draw(original, new Vector2(offsetX, offsetY), Color.White);
            spriteBatch.End();

            device.SetRenderTarget(null);
            return renderTarget;
        }
    }
}