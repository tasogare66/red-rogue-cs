using flash.events;
using flash.geom;

namespace flash.display
{
    public class DisplayObject : EventDispatcher
	{
        public Transform transform { get; set; }

        public DisplayObjectContainer parent { get; internal set; }

		public bool visible { get; set; }

        public DisplayObject() {
			visible = true;
		}

		//Bridge to PSM
		public virtual void RenderToOffScreen()
		{
		}

		public virtual void Render()
		{
		}
    }
}
