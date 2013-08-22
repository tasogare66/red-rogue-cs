using System;
using System.Collections.Generic;

namespace flash.display
{
    public class DisplayObjectContainer : InteractiveObject {
        readonly List<DisplayObject> _displayObjects = new List<DisplayObject>();

        public DisplayObject addChild (DisplayObject child) {
            _displayObjects.Add(child);
            child.parent = this;

            return child;
        }

        public void removeChild(DisplayObject child) {
            _displayObjects.Remove(child);
            child.parent = null;
        }

        public int numChildren { get { return _displayObjects.Count; } }

        public void removeChildAt(int index) {
            _displayObjects.RemoveAt(index);
        }


		public override void RenderToOffScreen()
		{
			base.RenderToOffScreen();

			foreach( var dispObj in _displayObjects ){
				dispObj.RenderToOffScreen();
			}
		}

		public override void Render()
		{
			base.Render();

			if( visible ){
				foreach( var dispObj in _displayObjects ){
					dispObj.Render();
				}
			}
		}
    }
}
