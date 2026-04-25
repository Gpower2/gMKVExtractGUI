using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace gMKVToolNix.Controls
{
    public class gDataGridView:DataGridView
    {
        protected int _LastClickedRowIndex = -1;

        [Browsable(false)]
        public int LastClickedRowIndex
        {
            get { return _LastClickedRowIndex; }
            set { _LastClickedRowIndex = value; }
        }

        protected int _LastClickedColumnIndex = -1;

        [Browsable(false)]
        public int LastClickedColumnIndex
        {
            get { return _LastClickedColumnIndex; }
            set { _LastClickedColumnIndex = value; }
        }

        protected int _ColumnResizedIndex = -1;
        protected bool _ColumnResizing = false;

        public gDataGridView()
            : base()
        {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.RowHeadersVisible = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            this.GridColor = Color.Gainsboro;
            this.BackgroundColor = System.Drawing.Color.GhostWhite;
        }

        protected override void OnCellMouseUp(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseUp(e);

            try
            {
                _LastClickedRowIndex = e.RowIndex;
                _LastClickedColumnIndex = e.ColumnIndex;

                if (Cursor.Current == Cursors.SizeWE)
                {
                    _ColumnResizing = false;
                }
            }
            catch (Exception ex)
            {
                ex.ShowException(this.FindForm());
            }
        }

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseDown(e);

            try
            {
                _LastClickedRowIndex = e.RowIndex;
                _LastClickedColumnIndex = e.ColumnIndex;

                _ColumnResizing = false;
                if (Cursor.Current == Cursors.SizeWE && e.RowIndex == -1 && e.ColumnIndex > -1)
                {
                    _ColumnResizedIndex = e.ColumnIndex;

                    // if we are in the first 8 pixel, we assume we resize the left column
                    if (e.X < 8)
                    {
                        _ColumnResizedIndex = _ColumnResizedIndex - 1;
                    }

                    _ColumnResizing = true;
                }
            }
            catch (Exception ex)
            {
                ex.ShowException(this.FindForm());
            }
        }

        protected override void OnCellMouseMove(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseMove(e);

            try
            {
                if (e.Button == MouseButtons.Left && _ColumnResizing)
                {
                    var c = new DataGridViewColumnEventArgs(this.Columns[_ColumnResizedIndex]);

                    if (e.ColumnIndex > _ColumnResizedIndex)
                    {
                        // If we are in the next column, then e.X is zeroed
                        // so we add the Column Width
                        c.Column.Width += e.X;
                    }
                    else
                    {
                        // If we are in the same column, then e.X equals to Width
                        c.Column.Width = e.X;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
