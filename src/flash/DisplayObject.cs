using flash.events;
using flash.geom;

namespace flash.display
{
    public class DisplayObject : EventDispatcher
	{
        /// <summary>Indicates the alpha transparency value of the object specified.</summary>
        public double alpha { get; set; }

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
