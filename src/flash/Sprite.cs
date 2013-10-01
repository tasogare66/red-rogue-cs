namespace flash.display
{
    public class Sprite : DisplayObjectContainer {
        public Graphics graphics { get ; private set; }

        public Sprite() {
            graphics = new Graphics();
        }
    }
}
