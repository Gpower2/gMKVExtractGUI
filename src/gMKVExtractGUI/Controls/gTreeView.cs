using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace gMKVToolNix
{
    public class gTreeView : TreeView
    {
        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_GETITEM = TV_FIRST + 62;
        private const int TVM_SETITEM = TV_FIRST + 63;

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref TVITEM lParam);

        private bool _CheckOnClick = false;

        /// <summary>
        /// Gets or sets a value indicating whether the check box should be toggled when an item is selected.
        /// </summary>
        [Description("Indicates whether the check box should be toggled when an item is selected.")]
        public bool CheckOnClick
        {
            get { return _CheckOnClick; }
            set { _CheckOnClick = value; }
        }

        private bool _SelectOnRightClick = false;

        /// <summary>
        /// Gets or sets a value indicating whether right clicking a node changes the selection.
        /// </summary>
        [Description("Indicates whether right clicking a node changes the selection..")]
        public bool SelectOnRightClick
        {
            get { return _SelectOnRightClick; }
            set { _SelectOnRightClick = value; }
        }

        /// <summary>
        /// Returns a list with all the nodes of this TreeView
        /// </summary>
        [Description("Gets a list with all the nodes of this TreeView")]
        public List<TreeNode> AllNodes
        {
            get
            {
                List<TreeNode> nodes = new List<TreeNode>();
                GetAllNodes(this, nodes);
                return nodes;
            }
        }

        private void GetAllNodes(object rootNode, List<TreeNode> nodeList)
        {
            if (rootNode == null)
            {
                return;
            }
            if (nodeList == null)
            {
                nodeList = new List<TreeNode>();
            }
            if (rootNode is TreeView)
            {
                foreach (var node in (rootNode as TreeView).Nodes)
                {
                    GetAllNodes(node, nodeList);
                }
            }
            else if (rootNode is TreeNode)
            {
                nodeList.Add(rootNode as TreeNode);
                if ((rootNode as TreeNode).Nodes != null && (rootNode as TreeNode).Nodes.Count > 0)
                {
                    foreach (var node in (rootNode as TreeNode).Nodes)
                    {
                        GetAllNodes(node, nodeList);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list with all the checked nodes of this TreeView
        /// </summary>
        [Description("Gets a list with all checked the nodes of this TreeView")]
        public List<TreeNode> CheckedNodes
        {
            get
            {
                List<TreeNode> nodes = new List<TreeNode>();
                GetCheckedNodes(this, nodes);
                return nodes;
            }
        }

        private void GetCheckedNodes(object rootNode, List<TreeNode> nodeList)
        {
            if (nodeList == null)
            {
                nodeList = new List<TreeNode>();
            }
            if (rootNode is TreeView)
            {
                foreach (var node in (rootNode as TreeView).Nodes)
                {
                    GetCheckedNodes(node, nodeList);
                }
            }
            else if (rootNode is TreeNode)
            {
                if ((rootNode as TreeNode).Checked)
                {
                    nodeList.Add(rootNode as TreeNode);
                }
                if ((rootNode as TreeNode).Nodes != null && (rootNode as TreeNode).Nodes.Count > 0)
                {
                    foreach (var node in (rootNode as TreeNode).Nodes)
                    {
                        GetCheckedNodes(node, nodeList);
                    }
                }
            }
        }

        public gTreeView() : base()
        {
            this.DoubleBuffered = true;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// Sets a value indicating if the checkbox is visible on the tree node.
        /// </summary>
        /// <param name="node">The tree node.</param>
        /// <param name="value"><value>true</value> to make the checkbox visible on the tree node; otherwise <value>false</value>.</param>
        public void SetIsCheckBoxVisible(TreeNode node, bool value)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (node.TreeView == null) throw new InvalidOperationException("The node does not belong to a tree.");

            // If we are on Linux, we can't use P/Invoke to user32.dll
            // So this function can't do anything
            if (PlatformExtensions.IsOnLinux) return;

            var tvi = new TVITEM
            {
                hItem = node.Handle,
                mask = TVIF_STATE,
                stateMask = TVIS_STATEIMAGEMASK,
                state = (value ? node.Checked ? 2 : 1 : 0) << 12
            };
            
            var result = SendMessage(node.TreeView.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
            
            if (result == IntPtr.Zero) throw new ApplicationException("Error setting TreeNode state.");
        }

        /// <summary>
        /// Gets a value indicating if the checkbox is visible on the tree node.
        /// </summary>
        /// <param name="node">The tree node.</param>
        /// <returns><value>true</value> if the checkbox is visible on the tree node; otherwise <value>false</value>.</returns>
        public bool IsCheckBoxVisible(TreeNode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (node.TreeView == null) throw new InvalidOperationException("The node does not belong to a tree.");

            // If we are on Linux, we can't use P/Invoke to user32.dll
            // So if the node's check box visibility has the same value as the node's TreeView CheckBoxes property 
            if (PlatformExtensions.IsOnLinux) return node.TreeView.CheckBoxes;

            var tvi = new TVITEM
            {
                hItem = node.Handle,
                mask = TVIF_STATE
            };
            
            var result = SendMessage(node.TreeView.Handle, TVM_GETITEM, node.Handle, ref tvi);
            
            if (result == IntPtr.Zero) throw new ApplicationException("Error getting TreeNode state.");
            
            var imageIndex = (tvi.state & TVIS_STATEIMAGEMASK) >> 12;
            return (imageIndex != 0);
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);

            if (_CheckOnClick)
            {
                if (e.Button == MouseButtons.Left &&
                    e.Node.Bounds.Contains(new Point(e.X, e.Y))
                    && this.CheckBoxes
                    && IsCheckBoxVisible(e.Node))
                {
                    e.Node.Checked = !e.Node.Checked;
                }
            }

            if (_SelectOnRightClick && e.Button == MouseButtons.Right)
            {
                this.SelectedNode = e.Node;
            }
        }
    }
}
