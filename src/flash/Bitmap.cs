using System;
//using System.Collections.Generic;

//using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Framework;
using Tutorial.Utility;

using redroguecs;

using flash.display;


namespace flash.display
{
	public class Bitmap : DisplayObject
	{
		public BitmapData bitmapData { get; set; }

		protected GameFrameworkRedRogueCs gs;

		public SimpleSprite sprite;

		FrameBuffer frameBuffer;
		Texture2D renderTexture;
		DepthBuffer depthBuffer;

		public double x {
			set { this.sprite.Position.X = (float)value; }
			get { return this.sprite.Position.X; }
		}
		public double y {
			set { this.sprite.Position.Y = (float)value; }
			get { return this.sprite.Position.Y; }
		}
		public double width {
			get { return this.sprite.Width; }
		}
		public double height {
			get { return this.sprite.Height; }
		}


		public SpriteBuffer spriteBuffer;
		public SpriteB spriteB0, spriteB1, spriteB2;

		public Bitmap(BitmapData bitmapData = null, String pixelSnapping = "auto", Boolean smoothing = false)
		{
			this.bitmapData = bitmapData;

			//FIXME:		
			int _width = 200;
			int _height = 12;
			// framebuffer
			frameBuffer = new FrameBuffer();
			renderTexture = new Texture2D(_width, _height, false, PixelFormat.Rgba, PixelBufferOption.Renderable);
			depthBuffer = new DepthBuffer(_width, _height, PixelFormat.Depth16);
			frameBuffer.SetColorTarget(renderTexture, 0);
			frameBuffer.SetDepthTarget(depthBuffer);

			this.gs = GameFrameworkRedRogueCs.Instance;
			sprite = new SimpleSprite(this.gs.Graphics, renderTexture);
			sprite.Scale.Y = -1.0f;	//FIXME:vflip

			//test2
			spriteBuffer= new SpriteBuffer(this.gs.Graphics, frameBuffer, 1024);
			this.spriteB0 = new SpriteB(gs.dicTextureInfo["font/4.png"]);
			this.spriteB1 = new SpriteB(gs.dicTextureInfo["font/5.png"]);
			this.spriteB1.Position.X = 10;
			this.spriteB2 = new SpriteB(gs.dicTextureInfo["font/a.png"]);
			this.spriteB2.Position.X = 20;
			spriteBuffer.Add( spriteB1 );
			spriteBuffer.Add( spriteB0 );
			spriteBuffer.Add( spriteB2 );
		}

		public Bitmap(int _width, int _height)
		{
			// framebuffer
			frameBuffer = new FrameBuffer();
			renderTexture = new Texture2D(_width, _height, false, PixelFormat.Rgba, PixelBufferOption.Renderable);
			depthBuffer = new DepthBuffer(_width, _height, PixelFormat.Depth16);
			frameBuffer.SetColorTarget(renderTexture, 0);
			frameBuffer.SetDepthTarget(depthBuffer);

			this.gs = GameFrameworkRedRogueCs.Instance;
			sprite = new SimpleSprite(this.gs.Graphics, renderTexture);
			sprite.Scale.Y = -1.0f;	//FIXME:vflip

			//test2
			spriteBuffer= new SpriteBuffer(this.gs.Graphics, frameBuffer, 1024);
			this.spriteB0 = new SpriteB(gs.dicTextureInfo["font/4.png"]);
			this.spriteB1 = new SpriteB(gs.dicTextureInfo["font/5.png"]);
			this.spriteB1.Position.X = 10;
			this.spriteB2 = new SpriteB(gs.dicTextureInfo["font/a.png"]);
			this.spriteB2.Position.X = 20;
			spriteBuffer.Add( spriteB1 );
			spriteBuffer.Add( spriteB0 );
			spriteBuffer.Add( spriteB2 );
		}

		~Bitmap()
		{
			if( frameBuffer != null ){
				frameBuffer.Dispose();
			}
			//FIXME:	
		}

	//FIXME:test	
    void RenderToOffScreen()
    {
		GraphicsContext graphics = this.gs.Graphics;

		graphics.SetFrameBuffer(frameBuffer);
		{
        FrameBuffer currentBuffer = graphics.GetFrameBuffer() ;

        float aspect = currentBuffer.AspectRatio;
//        float fovy = FMath.Radians(30.0f);
//        Matrix4 proj = Matrix4.Perspective(fovy, aspect, 1.0f, 1000000.0f);
//        Matrix4 view = Matrix4.LookAt(new Vector3(0.0f, 0.5f, 3.0f), new Vector3(0.0f, 0.5f, 0.0f), Vector3.UnitY);
//        Matrix4 world = Matrix4.RotationY(1.0f);
//        Matrix4 worldViewProj = proj * view * world;
//        vcolorShader.SetUniformValue(0, ref worldViewProj);

        graphics.SetViewport(0, 0, currentBuffer.Width, currentBuffer.Height);
        graphics.SetClearColor(0.0f, 0.0f, 1.0f, 1.0f);
        graphics.Clear();

		graphics.Enable(EnableMode.DepthTest);
//        graphics.SetShaderProgram(vcolorShader);
//        graphics.SetVertexBuffer(0, triangleVertices);
//        graphics.SetTexture(0, triangleTexture);

//        graphics.Enable(EnableMode.CullFace, false);
//        graphics.DrawArrays(DrawMode.Triangles, 0, triangleVertices.VertexCount);

		graphics.SetTexture(0, this.gs.textureUnified);
		spriteBuffer.Render();
		}
		graphics.SetFrameBuffer(graphics.Screen);
    }

		public override void Update ()
		{
			RenderToOffScreen();

			base.Update ();
		}

		public override void Render ()
		{
			if( this.Status == ActorStatus.Action && sprite != null ){
				sprite.Render();
			}

			base.Render ();
		}
	}
}
