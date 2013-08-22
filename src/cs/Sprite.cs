/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Framework;
using Tutorial.Utility;

using App;

#if false
namespace redroguecs
{
	public enum Event : int
	{
		ENTER_FRAME,
	}

	public class Sprite : Actor
	{
		protected GameFrameworkRedRogueCs gs;
		
		public SpriteB spriteB;
		
		//アクション。
///		public List<ActionBase> actionList=new List<ActionBase>();
		
		// event
		public delegate void ListenerEnterFrame();
		public event ListenerEnterFrame eventEnterFrame;

		public Sprite(GameFrameworkRedRogueCs gs, string name) : base(name) 
		{
			this.gs = gs;
		}

		public Sprite()
		{
			this.gs = GameFrameworkRedRogueCs.Instance;
		}

		public Sprite(GameFrameworkRedRogueCs gs, string name, UnifiedTextureInfo textureInfo) : this(gs,name) 
		{
			spriteB = new SpriteB(textureInfo);
		}


		// event
		public void addEventListener(Event id, ListenerEnterFrame function)
		{
			switch( id ) {
			case Event.ENTER_FRAME:
				eventEnterFrame += function;
				break;
			default:
				Util.Assert( false );
				break;
			}
		}
		
		
///		public void AddAction(ActionBase action)
///		{
///			actionList.Add(action);	
///		}
		
		public override void Update ()
		{
			// アクション。
///			foreach( ActionBase action in actionList)
///			{
///				action.Update();
///			}
			
			//アクションが完了したら、Removeする。
///			actionList.RemoveAll(action=>action.Done==true);
			
			// Event.ENTER_FRAME
			if( this.eventEnterFrame != null ){
				this.eventEnterFrame();
			}

			base.Update ();
		}
		
		
		public override void Render ()
		{
			if(spriteB!=null && this.Status == ActorStatus.Action)
				gs.spriteBuffer.Add(this.spriteB);
			
			base.Render ();
		}
	}
}
#endif
