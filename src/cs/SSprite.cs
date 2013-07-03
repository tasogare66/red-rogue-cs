/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Framework;
using Tutorial.Utility;


namespace redroguecs
{
	public class SSprite : Actor
	{
		protected GameFrameworkRedRogueCs gs;
		
		public SimpleSprite sprite;
		//アクション。
///		public List<ActionBase> actionList=new List<ActionBase>();
		
		public SSprite(Texture2D texture)
		{
			this.gs = GameFrameworkRedRogueCs.Instance;
			sprite = new SimpleSprite(this.gs.Graphics, texture);
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
