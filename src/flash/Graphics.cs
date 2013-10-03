using flash.geom;

namespace flash.display
{
    public sealed class Graphics {
        public static Graphics Instance { get; private set; }
               
        static Graphics() {
            Instance =  new Graphics();
        }

        public void clear() {

        }

		public void beginFill(uint color, double alpha = 1.0) {
			//FIXME:	
		}

        public void beginBitmapFill(BitmapData bitmapData, object matrix = null) {
            //Cache the commands for later to replay in the drawing phase
        }
         
        public void drawRect(double x, double y, double width, double height) {
            //Cache the commands for later to replay in the drawing phase
        }
         
        public void endFill() {

        }

		public void moveTo(double x, double y) {
			//FIXME:	
		}

		public void lineTo(double x, double y) {
			//FIXME:	
		}
    }
}
