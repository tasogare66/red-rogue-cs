using System;

using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Framework;
using Tutorial.Utility;
using redroguecs;

using flash.geom;


namespace flash.display
{
    public class BitmapData
	{
		public int height { get; private set; }
        public Rectangle rect { get; private set; }
		public Boolean transparent { get; private set; }
		public int width { get; private set; }

		uint fillColor;

		public BitmapData(int width, int height, Boolean transparent = true, uint fillColor = 0xFFFFFFFF){
			this.width = width;
			this.height = height;
			this.transparent= transparent;
			this.fillColor = fillColor;
			this.rect = new Rectangle(0, 0, width, height);

			this.initRenderToOffScreen( width, height );
		}

		public BitmapData clone() {
			//FIXME:	
			return new BitmapData(width, height, transparent, fillColor);
		}

        public void colorTransform(Rectangle rect, ColorTransform colorTransform) {
			//FIXME:	
        }

        public void fillRect(Rectangle rect, uint color) {
			//FIXME:	
        }

        public void copyPixels(BitmapData sourceBitmapData, Rectangle sourceRect, Point destPoint, BitmapData alphaBitmapData = null, Point alphaPoint = null, Boolean mergeAlpha = false) {
			//FIXME:	
        }
        public void copyPixels(SpriteB sourceBitmapData, Rectangle sourceRect, Point destPoint, BitmapData alphaBitmapData = null, Point alphaPoint = null, Boolean mergeAlpha = false) {
			SpriteB tmp = sourceBitmapData.clone();
			tmp.Position.X = (float)destPoint.x;
			tmp.Position.Y = (float)destPoint.y;
			spriteBuffer.Add( tmp );
        }

		public void dispose() {
			if( this.renderTexture != null ){
				this.renderTexture.Dispose();
				this.renderTexture = null;
			}
			if( this.frameBuffer != null ){
				this.frameBuffer.Dispose();
				this.frameBuffer = null;
			}
		}

		FrameBuffer frameBuffer;
		Texture2D renderTexture;
		DepthBuffer depthBuffer;
		SimpleSprite sSprite;
		GameFrameworkRedRogueCs gs;

		SpriteBuffer spriteBuffer;

		public double x {
			set { this.sSprite.Position.X = (float)value; }
			get { return this.sSprite.Position.X; }
		}
		public double y {
			set { this.sSprite.Position.Y = (float)value; }
			get { return this.sSprite.Position.Y; }
		}

		public void initRenderToOffScreen(int in_w, int in_h)
		{
			this.frameBuffer = new FrameBuffer();
			this.renderTexture = new Texture2D(in_w, in_h, false, PixelFormat.Rgba, PixelBufferOption.Renderable);
			this.depthBuffer = new DepthBuffer(in_w, in_h, PixelFormat.Depth16);
			frameBuffer.SetColorTarget( this.renderTexture, 0 );
			frameBuffer.SetDepthTarget( this.depthBuffer );

			this.gs = GameFrameworkRedRogueCs.Instance;
			sSprite = new SimpleSprite(this.gs.Graphics, renderTexture);
			sSprite.SetTextureVFlip();

			spriteBuffer = new SpriteBuffer(this.gs.Graphics, frameBuffer, 512);
		}

		public void RenderToOffScreen()
		{
			if( frameBuffer != null && spriteBuffer.GetNumOfSprite() > 0 ){
				GraphicsContext graphics = this.gs.Graphics;

				graphics.SetFrameBuffer(frameBuffer);

				graphics.SetViewport(0, 0, frameBuffer.Width, frameBuffer.Height);
				graphics.SetClearColor(0.0f, 0.0f, 1.0f, 1.0f);
				graphics.Clear();

				graphics.Enable(EnableMode.DepthTest);

				graphics.SetTexture(0, this.gs.textureUnified);
				spriteBuffer.Render();

				spriteBuffer.Clear();
			}
		}

		public void Render()
		{
			if( sSprite != null ){
				sSprite.Render();
			}
		}

		~BitmapData()
		{
		}
    }
}
