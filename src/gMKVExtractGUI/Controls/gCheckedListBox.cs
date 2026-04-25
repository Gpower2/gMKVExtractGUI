using System.Windows.Forms;

namespace gMKVToolNix
{

    public class gCheckedListBox: System.Windows.Forms.CheckedListBox
    {
        public gCheckedListBox()
            : base()
        {
            this.DoubleBuffered = true;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.Opaque, true);            
        }
    }
}
