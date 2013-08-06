/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;

using Sce.PlayStation.Framework;
using Tutorial.Utility;

using System.Xml.Linq;
using System.IO;

using App;

namespace redroguecs
{
	public class GameFrameworkRedRogueCs : GameFramework
	{
		public ImageRect rectScreen;
		
		public Int32 appCounter=0;
		
		public Actor Root{ get; set;}
		
		bool m_pause=false;
		int forwardFrame = 0;
		
		
		public Texture2D textureUnified;
		
		public Dictionary<string, UnifiedTextureInfo> dicTextureInfo;
		
		public SpriteBuffer spriteBuffer;
		
		public const int sizeofSprite=1024;
		
		static GameFrameworkRedRogueCs m_instance;		

		public GameFrameworkRedRogueCs()
		{
			Util.Assert(m_instance == null);
			m_instance = this;
		}
			
		public static GameFrameworkRedRogueCs Instance {
			get {
				Util.Assert(m_instance != null);
				return m_instance;
			}
		}

		public override void Initialize()
		{
			base.Initialize((int)Game.WIDTH, (int)Game.HEIGHT);
			
			rectScreen = graphics.GetViewport();

			spriteBuffer=new SpriteBuffer(graphics, graphics.Screen, sizeofSprite);
			
			//@j 一体化テクスチャの処理。
			dicTextureInfo = UnifiedTexture.GetDictionaryTextureInfo("/Application/src/assets/unified_texture.xml");
			textureUnified=new Texture2D("/Application/src/assets/unified_texture.png", false);
			
			//@j アクターツリーの初期化。
			//@e Initialization of actor tree
			Root = new Actor("root");
			
			Root.addChild(new Game(this, "game"));
			
//			Root.addChild(new Map(this,"Map"));
			
//			Root.addChild(new Actor("bulletManager"));
		}
		
		
		public override void Update()
		{
			base.Update();
			
#if DEBUG
			debugString.WriteLine("Buttons="+PadData.Buttons);
			
			//スタートボタンでポーズする。
			if((this.PadData.ButtonsDown & (GamePadButtons.Start)) != 0)
			{
				m_pause = m_pause ? false : true;
			}

			//コマ送り。ポーズ中、セレクトボタンが押されたら、1フレーム進める。
			if(m_pause==true && (this.PadData.ButtonsDown & (GamePadButtons.Select)) != 0)
			{
				forwardFrame =1;	
			}
#endif			
			
			if( forwardFrame > 0 || m_pause == false)
			{
				Root.Update();

				++appCounter;
				
				if( forwardFrame > 0)
					forwardFrame--;
			}
			else
			{
				//ポーズ中。	
				debugString.WriteLine("Pause");
			}
		}
		
		
		public override void Render()
		{
			// off screen描画
			Root.RenderToOffScreen();

			// --------------------

			graphics.SetFrameBuffer(null);
			FrameBuffer currentBuffer = graphics.GetFrameBuffer() ;

			graphics.SetViewport(0, 0, currentBuffer.Width, currentBuffer.Height);
			graphics.SetClearColor(0.5f, 0.5f, 1.0f, 0.0f);
			graphics.Clear();
			
			
			//深度テストなし。
			//graphics.Disable(EnableMode.DepthTest);
			
			//不透明/深度テストあり。
			graphics.Enable(EnableMode.DepthTest);
			
			//半透明。
			//graphics.Disable(EnableMode.DepthTest);
			//graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			
			//加算半透明
			//graphics.Disable(EnableMode.DepthTest);
			//graphics.SetBlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.One ) ;
			
			graphics.SetTexture(0, this.textureUnified);	//FIXME:確認	
			
			spriteBuffer.Clear();
			Root.Render();
			
			spriteBuffer.Render();
			
			debugString.WriteLine("Sprite Cnt="+spriteBuffer.GetNumOfSprite());
			
			base.Render();
			
			Root.CheckStatus();
		}
		
		public override void Terminate ()
		{
			textureUnified.Dispose();
			
			base.Terminate ();

			m_instance = null;
		}
	}
}
