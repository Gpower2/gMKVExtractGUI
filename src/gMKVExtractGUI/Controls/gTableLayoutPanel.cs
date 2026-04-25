using System;
using System.Windows.Forms;

namespace gMKVToolNix
{
    public class gTableLayoutPanel:TableLayoutPanel
    {
        public const Int32 WM_ERASEBKGND = 0x0014;
        
        public gTableLayoutPanel()
            : base()
        {
            this.DoubleBuffered = true;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
