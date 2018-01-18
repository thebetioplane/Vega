using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Vega
{
    public class Texture2D : IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private int textureId = 0;
        public Texture2D(Stream stream)
        {
            using (var bitmap = new Bitmap(stream))
            {
                this.Width = bitmap.Width;
                this.Height = bitmap.Height;
                this.textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, this.textureId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
            }
            stream.Dispose();
        }
        
        public void Draw(float x, float y, Vector2 origin, Color4 color)
        {
            x -= origin.X;
            y -= origin.Y;
            GL.Color4(color);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.BindTexture(TextureTarget.Texture2D, this.textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(x, y);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(x + this.Width, y);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(x + this.Width, y + this.Height);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(x, y + this.Height);
            GL.End();
        }
        public void Draw(float x, float y, Vector2 origin, Color4 color, float rotation, float scale)
        {
            x -= origin.X;
            y -= origin.Y;
            GL.Color4(color);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            if (rotation != 0 || scale != 0)
            {
                Vector3 diff = new Vector3(-x - origin.X, -y - origin.Y, 0.0f);
                GL.Translate(-diff);
                GL.Rotate(MathHelper.RadiansToDegrees(rotation), 0.0f, 0.0f, 1.0f);
                GL.Scale(scale, scale, 1.0f);
                GL.Translate(diff);
            }
            GL.BindTexture(TextureTarget.Texture2D, this.textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(x, y);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(x + this.Width, y);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(x + this.Width, y + this.Height);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(x, y + this.Height);
            GL.End();
        }
        public void Draw(float x, float y, Vector2 origin, Color4 color, Rectangle source, float rotation, float scale)
        {
            x -= origin.X;
            y -= origin.Y;
            Vector2 texCoordMin = new Vector2(source.X / (float)this.Width, source.Y / (float)this.Height);
            Vector2 texCoordMax = new Vector2((source.X + source.Width) / (float)this.Width, (source.Y + source.Height) / (float)this.Height);
            GL.Color4(color);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            if (rotation != 0 || scale != 0)
            {
                Vector3 diff = new Vector3(-x - origin.X, -y - origin.Y, 0.0f);
                GL.Translate(-diff);
                GL.Rotate(MathHelper.RadiansToDegrees(rotation), 0.0f, 0.0f, 1.0f);
                GL.Scale(scale, scale, 1.0f);
                GL.Translate(diff);
            }
            GL.BindTexture(TextureTarget.Texture2D, this.textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texCoordMin.X, texCoordMin.Y);
            GL.Vertex2(x, y);
            GL.TexCoord2(texCoordMax.X, texCoordMin.Y);
            GL.Vertex2(x + source.Width, y);
            GL.TexCoord2(texCoordMax.X, texCoordMax.Y);
            GL.Vertex2(x + source.Width, y + source.Height);
            GL.TexCoord2(texCoordMin.X, texCoordMax.Y);
            GL.Vertex2(x, y + source.Height);
            GL.End();
        }
        public void Dispose()
        {
            GL.DeleteTexture(this.textureId);
        }
    }
}
