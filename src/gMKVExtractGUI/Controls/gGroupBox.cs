using System.Windows.Forms;

namespace gMKVToolNix
{
    public class gGroupBox:GroupBox
    {
        public const int WM_ERASEBKGND = 0x0014;

        public gGroupBox()
            : base()
        {
            this.DoubleBuffered = true;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);            
        }
    }
}
