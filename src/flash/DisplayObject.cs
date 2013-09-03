using flash.events;
using flash.geom;

namespace flash.display
{
    public class DisplayObject : EventDispatcher, IBitmapDrawable
	{
        /// <summary>Indicates the alpha transparency value of the object specified.</summary>
        public double alpha { get; set; }

		public Stage stage { get { return redroguecs.GameFrameworkRedRogueCs.Stage; } }

        public Transform transform { get; set; }

		public DisplayObject mask { get; set; }

        public DisplayObjectContainer parent { get; internal set; }

		public bool visible { get; set; }

        public double x { get; set; }
        public double y { get; set; }
        //public double z { get; set; }

		public double width { get; set; }
		public double height { get; set; }

        public double scaleX { get; set; }
        public double scaleY { get; set; }
        //public double scaleZ { get; set; }

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
