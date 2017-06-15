/*
 * Copyright (C)  2011  Axel Kesseler
 * 
 * This software is free and you can use it for any purpose. Furthermore, 
 * you are free to copy, to modify and/or to redistribute this software.
 * 
 * In addition, this software is distributed in the hope that it will be 
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 */

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using plexdata.Utilities.Resources;

namespace plexdata.$safeprojectname$
{
    public class ResourceTreeView : TreeView
    {
        public event EventHandler<ResourceTreeViewEventArgs> Selected;
        public event EventHandler<ResourceTreeViewEventArgs> Expanded;
        public event EventHandler<EventArgs> TreeEmpty;

        public const string ROOT_NODE = "RootNode";
        public const string TYPE_NODE = "TypeNode";
        public const string BINARY_NODE = "BinaryNode";
        public const string BITMAP_NODE = "BitmapNode";
        public const string CURSOR_NODE = "CursorNode";
        public const string CURSOR_ITEM = "CursorItem";
        public const string DIALOG_NODE = "DialogNode";
        public const string HTML_NODE = "HtmlNode";
        public const string ICON_NODE = "IconNode";
        public const string ICON_ITEM = "IconItem";
        public const string MENU_NODE = "MenuNode";
        public const string STRING_NODE = "StringNode";
        public const string TEXT_NODE = "TextNode";
        public const string VERSION_NODE = "VersionNode";

        private string lastFile = String.Empty;

        public ResourceTreeView()
            : base()
        {
            ImageList images = new ImageList();
            images.ColorDepth = ColorDepth.Depth32Bit;
            images.ImageSize = new Size(20, 20);

            images.Images.Add(ResourceTreeView.ROOT_NODE, global::plexdata.$safeprojectname$.Properties.Resources.RootNode);
            images.Images.Add(ResourceTreeView.TYPE_NODE, global::plexdata.$safeprojectname$.Properties.Resources.TypeNode);
            images.Images.Add(ResourceTreeView.BINARY_NODE, global::plexdata.$safeprojectname$.Properties.Resources.BinaryNode);
            images.Images.Add(ResourceTreeView.BITMAP_NODE, global::plexdata.$safeprojectname$.Properties.Resources.BitmapNode);
            images.Images.Add(ResourceTreeView.CURSOR_NODE, global::plexdata.$safeprojectname$.Properties.Resources.CursorNode);
            images.Images.Add(ResourceTreeView.DIALOG_NODE, global::plexdata.$safeprojectname$.Properties.Resources.DialogNode);
            images.Images.Add(ResourceTreeView.HTML_NODE, global::plexdata.$safeprojectname$.Properties.Resources.HtmlNode);
            images.Images.Add(ResourceTreeView.ICON_NODE, global::plexdata.$safeprojectname$.Properties.Resources.IconNode);
            images.Images.Add(ResourceTreeView.MENU_NODE, global::plexdata.$safeprojectname$.Properties.Resources.MenuNode);
            images.Images.Add(ResourceTreeView.STRING_NODE, global::plexdata.$safeprojectname$.Properties.Resources.StringNode);
            images.Images.Add(ResourceTreeView.TEXT_NODE, global::plexdata.$safeprojectname$.Properties.Resources.TextNode);
            images.Images.Add(ResourceTreeView.VERSION_NODE, global::plexdata.$safeprojectname$.Properties.Resources.VersionNode);

            this.ImageList = images;

            // REMARK: Getting a tree view flicker-free is unfortunately not simply 
            //         done by setting the "DoubleBuffered" property. But if you ever 
            //         need a non-flickering tree view then you will find a sample 
            //         and HowTo under following link!
            //
            // http://www.codeproject.com/KB/list/double-buffered-tree-view.aspx
            //
        }

        #region Public property implementation.

        public string LastLoadedFile
        {
            get { return this.lastFile; }
            private set { this.lastFile = value; }
        }

        public string LastLoadedPath
        {
            get
            {
                try 
                {
                    return Path.GetDirectoryName(this.LastLoadedFile);
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public string[] LoadedFiles
        {
            get
            {
                // Iterate all root nodes.
                List<string> result = new List<string>();
                foreach (TreeNode node in this.Nodes)
                {
                    RootNode root = (node as RootNode);
                    if (root != null)
                    {
                        result.Add(root.GetFilename());
                    }
                }
                return result.ToArray();
            }
        }

        #endregion // Public property implementation.

        #region Public function implementation.

        public void OpenRecource(string filename)
        {
            this.OpenRecource(filename, true);
        }

        public void OpenRecource(string filename, bool expand)
        {
            if (!String.IsNullOrEmpty(filename))
            {
                RootNode rootNode = this.CreateRootNode(this, filename);
                ResourceType[] types = rootNode.ResourcesLocator.GetTypes();
                foreach (ResourceType type in types)
                {
                    // Don't add these types to the list!
                    if (type.Type == ResourceTypes.RT_ICON || type.Type == ResourceTypes.RT_CURSOR)
                    {
                        continue;
                    }

                    TypeNode typeNode = this.CreateTypeNode(rootNode, type);
                    ResourceValue[] values = rootNode.ResourcesLocator.GetValues(type);
                    foreach (ResourceValue value in values)
                    {
                        ValueNode valueNode = this.CreateValueNode(typeNode, value);
                        valueNode.Adjust(rootNode.ResourcesLocator, type);
                    }
                }

                if (expand) { rootNode.Expand(); }
                this.SelectedNode = rootNode;
                this.LastLoadedFile = filename;
            }
        }

        public void CloseRecource()
        {
            BaseNode node = (this.SelectedNode as BaseNode);
            if (node != null)
            {
                RootNode root = node.GetRootNode();
                if (root != null)
                {
                    // Clean up...
                    root.ResourcesLocator.Dispose();
                    
                    // Finally, remove node.
                    this.Nodes.Remove(root);
                    if (this.Nodes.Count == 0)
                    {
                        this.OnTreeEmpty();
                    }
                }
            }
        }

        public BaseNode GetCurrentNode()
        {
            return (this.SelectedNode as BaseNode);
        }

        public ResourcesLocator GetCurrentResourcesLocator()
        {
            BaseNode node = (this.SelectedNode as BaseNode);
            if (node != null)
            {
                return node.GetResourcesLocator();
            }
            else
            {
                return null;
            }
        }

        public ResourceType GetCurrentResourceType()
        {
            BaseNode node = (this.SelectedNode as BaseNode);
            if (node != null)
            {
                // If selected node is a root node then 
                // return with type of fist sub-item.
                if (node.IsRootNode())
                {
                    if (node.Nodes.Count > 0)
                    {
                        return (node.Nodes[0] as TypeNode).ResourceType;
                    }
                    else
                    {
                        return null; // Give up...
                    }
                }
                // If selected node is a type node already 
                // then return with this type.
                else if (node.IsTypeNode())
                {
                    return (node as TypeNode).ResourceType;
                }
                // If selected node is a value node then 
                // return with its parent type.
                else if (node.IsValueNode())
                {
                    return (node as ValueNode).ResourceType;
                }
                else
                {
                    return null; // Give up...
                }
            }
            else
            {
                return null; // Nothing selected...
            }
        }

        public ResourceValue GetCurrentResourceValue()
        {
            BaseNode node = (this.SelectedNode as BaseNode);
            if (node != null)
            {
                // If selected node is a root node then 
                // return with value of fist sub-item's 
                // fist sub-item.
                if (node.IsRootNode())
                {
                    if (node.Nodes.Count > 0 && node.Nodes[0].Nodes.Count > 0)
                    {
                        return ((node.Nodes[0] as TypeNode).Nodes[0] as ValueNode).ResourceValue;
                    }
                    else
                    {
                        return null; // Give up...
                    }
                }
                // If selected node is a type node then 
                // return with value of fist sub-item.
                else if (node.IsTypeNode())
                {
                    if (node.Nodes.Count > 0)
                    {
                        return (node.Nodes[0] as ValueNode).ResourceValue;
                    }
                    else
                    {
                        return null; // Give up...
                    }
                }
                // If selected node is a value node already 
                // then return with this value.
                else if (node.IsValueNode())
                {
                    return (node as ValueNode).ResourceValue;
                }
                else
                {
                    return null; // Give up...
                }
            }
            else
            {
                return null; // Nothing selected...
            }
        }

        public ResourceValue[] GetCurrentResourceValues()
        {
            BaseNode node = (this.SelectedNode as BaseNode);
            if (node != null)
            {
                // If selected node is a root node then 
                // return with values of fist sub-item's 
                // fist sub-item.
                if (node.IsRootNode())
                {
                    if (node.Nodes.Count > 0 && node.Nodes[0].Nodes.Count > 0)
                    {
                        return (node.Nodes[0] as TypeNode).ResourceValues;
                    }
                    else
                    {
                        return null; // Give up...
                    }
                }
                // If selected node is a type node already
                // then return with values of fist sub-item.
                else if (node.IsTypeNode())
                {
                    return (node as TypeNode).ResourceValues;
                }
                // If selected node is a value node then 
                // return with parent's values.
                else if (node.IsValueNode() && node.Parent is TypeNode)
                {
                    return ((node as ValueNode).Parent as TypeNode).ResourceValues;
                }
                else
                {
                    return null; // Give up...
                }
            }
            else
            {
                return null; // Nothing selected...
            }
        }

        #endregion // Public function implementation.

        #region Resource Tree View event handler implementation.

        protected override void OnAfterExpand(TreeViewEventArgs args)
        {
            base.OnAfterExpand(args);
            if (args.Node is BaseNode)
            {
                EventHandler<ResourceTreeViewEventArgs> handler = Expanded;
                if (handler != null)
                {
                    handler(this, (args.Node as BaseNode).GetEventArgs());
                }
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs args)
        {
            base.OnAfterSelect(args);
            if (args.Node is BaseNode)
            {
                EventHandler<ResourceTreeViewEventArgs> handler = Selected;
                if (handler != null)
                {
                    handler(this, (args.Node as BaseNode).GetEventArgs());
                }
            }
        }

        protected void OnTreeEmpty()
        {
            EventHandler<EventArgs> handler = TreeEmpty;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion // Resource Tree View event handler implementation.

        #region Private node helper functions.

        private RootNode CreateRootNode(TreeView parent, string filename)
        {
            RootNode result = new RootNode(filename);

            result.ImageIndex = this.ImageList.Images.IndexOfKey(ResourceTreeView.ROOT_NODE);
            result.SelectedImageIndex = result.ImageIndex;

            parent.Nodes.Add(result);

            return result;
        }

        private TypeNode CreateTypeNode(TreeNode parent, ResourceType type)
        {
            TypeNode result = new TypeNode(type);

            result.ImageIndex = this.ImageList.Images.IndexOfKey(ResourceTreeView.TYPE_NODE);
            result.SelectedImageIndex = result.ImageIndex;

            parent.Nodes.Add(result);

            return result;
        }

        private ValueNode CreateValueNode(TreeNode parent, ResourceValue value)
        {
            ValueNode result = new ValueNode(value);

            result.ImageIndex = this.ImageList.Images.IndexOfKey(ResourceTreeView.BINARY_NODE);
            result.SelectedImageIndex = result.ImageIndex;

            parent.Nodes.Add(result);

            return result;
        }

        #endregion // Private node helper functions.

        #region Public tree node helper classes.

        public abstract class BaseNode : TreeNode
        {
            public abstract ResourceTreeViewEventArgs GetEventArgs();

            public bool IsRootNode()
            {
                return (this is RootNode);
            }

            public bool IsTypeNode()
            {
                return (this is TypeNode);
            }

            public bool IsValueNode()
            {
                return (this is ValueNode);
            }

            public RootNode GetRootNode()
            {
                if (this.IsRootNode())
                {
                    return (this as RootNode);
                }
                else
                {
                    BaseNode parent = (this.Parent as BaseNode);
                    if (parent != null)
                    {
                        return parent.GetRootNode();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public ResourcesLocator GetResourcesLocator()
            {
                RootNode root = this.GetRootNode();
                if (root != null)
                {
                    return root.ResourcesLocator;
                }
                else
                {
                    return null;
                }
            }

            public string GetFilename()
            {
                RootNode root = this.GetRootNode();
                if (root != null)
                {
                    return root.Filename;
                }
                else
                {
                    return String.Empty; 
                }
            }

            protected static bool IsBinaryData(MemoryStream stream)
            {
                char[] array = Encoding.UTF8.GetString(stream.ToArray()).ToCharArray();
                foreach (char current in array)
                {
                    // If at least one character is less than the 
                    // 'space' (except line-feed etc.) then lets 
                    // assume binary data!
                    if (current < ' ' && current != '\r' && current != '\n' && current != '\t')
                    {
                        return true;
                    }
                }
                // In this case the stream seems to contain characters only...
                return false;
            }
        }

        public class RootNode : BaseNode
        {
            private string filename = null;
            private ResourcesLocator resources = null;

            public RootNode()
                : base()
            {
                this.Text = "<empty>";
                this.filename = String.Empty;
                this.resources = new ResourcesLocator();
            }

            public RootNode(string filename)
                : this()
            {
                this.Text = Path.GetFileName(filename);
                this.filename = filename;
                this.resources = new ResourcesLocator(this.filename);
            }

            public string Filename
            {
                get { return this.filename; }
            }

            public ResourcesLocator ResourcesLocator
            {
                get { return this.resources; }
            }

            public override ResourceTreeViewEventArgs GetEventArgs()
            {
                return new ResourceTreeViewEventArgs(this);
            }
        }

        public class TypeNode : BaseNode
        {
            private ResourceType type = null;

            public TypeNode()
                : base()
            {
                this.Text = "<empty>";
                this.type = new ResourceType();
            }

            public TypeNode(ResourceType type)
                : this()
            {
                this.Text = type.ToString();
                this.type = type;
            }

            public ResourceType ResourceType
            {
                get { return this.type; }
            }

            public ResourceValue[] ResourceValues
            {
                get
                {
                    List<ResourceValue> result = new List<ResourceValue>();
                    foreach (TreeNode current in this.Nodes)
                    {
                        if (current is ValueNode)
                        {
                            result.Add((current as ValueNode).ResourceValue);
                        }
                    }
                    return result.ToArray();
                }
            }

            public override ResourceTreeViewEventArgs GetEventArgs()
            {
                return new ResourceTreeViewEventArgs(this);
            }
        }

        public class ValueNode : BaseNode
        {
            private ResourceValue value = null;
            private bool binary = true;

            public ValueNode()
                : base()
            {
                this.Text = "<empty>";
                this.value = new ResourceValue();
            }

            public ValueNode(ResourceValue value)
                : this()
            {
                this.Text = value.ToString();
                this.value = value;
            }

            public ResourceValue ResourceValue
            {
                get { return this.value; }
            }

            public ResourceType ResourceType
            {
                get
                {
                    if (this.Parent is TypeNode)
                    {
                        return (this.Parent as TypeNode).ResourceType;
                    }
                    else
                    {
                        return new ResourceType(); // RT_UNKNOWN!
                    }
                }
            }

            public bool IsBinary
            {
                get { return this.binary; }
            }
            
            public void Adjust(ResourcesLocator locator, ResourceType type)
            {
                using (MemoryStream stream = locator.GetResource(type, value))
                {
                    int image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.BINARY_NODE);
                    switch (type.Type)
                    {
                        case ResourceTypes.RT_BITMAP:
                            image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.BITMAP_NODE);
                            break;
                        case ResourceTypes.RT_CURSOR:
                        case ResourceTypes.RT_GROUP_CURSOR:
                            image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.CURSOR_NODE);
                            break;
                        case ResourceTypes.RT_DIALOG:
                            image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.DIALOG_NODE);
                            break;
                        case ResourceTypes.RT_HTML:
                            image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.HTML_NODE);
                            break;
                        case ResourceTypes.RT_ICON:
                        case ResourceTypes.RT_GROUP_ICON:
                            image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.ICON_NODE);
                            break;
                        case ResourceTypes.RT_MENU:
                            image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.MENU_NODE);
                            break;
                        case ResourceTypes.RT_STRING:
                            image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.STRING_NODE);
                            break;
                        case ResourceTypes.RT_VERSION:
                            image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.VERSION_NODE);
                            break;
                        default:
                            if (!BaseNode.IsBinaryData(stream))
                            {
                                this.binary = false;
                                image = this.TreeView.ImageList.Images.IndexOfKey(ResourceTreeView.TEXT_NODE);
                            }
                            break;
                    }
                    this.ImageIndex = image;
                    this.SelectedImageIndex = this.ImageIndex;
                }
            }

            public override ResourceTreeViewEventArgs GetEventArgs()
            {
                return new ResourceTreeViewEventArgs(this);
            }
        }

        #endregion // Public tree node helper classes.
    }

    public class ResourceTreeViewEventArgs : EventArgs
    {
        private ResourceTreeView.BaseNode node = null;

        public ResourceTreeViewEventArgs(ResourceTreeView.BaseNode node)
            : base()
        {
            this.node = node;
        }

        public ResourceTreeView.BaseNode Node
        {
            get { return this.node; }
        }

        public bool IsRootNode()
        {
            return (this.node is ResourceTreeView.RootNode);
        }

        public bool IsTypeNode()
        {
            return (this.node is ResourceTreeView.TypeNode);
        }

        public bool IsValueNode()
        {
            return (this.node is ResourceTreeView.ValueNode);
        }

        public ResourcesLocator GetResourcesLocator()
        {
            if (this.IsRootNode())
            {
                return (this.node as ResourceTreeView.RootNode).ResourcesLocator;
            }
            else
            {
                ResourceTreeView.BaseNode parent = (this.node.Parent as ResourceTreeView.BaseNode);
                if (parent != null)
                {
                    return parent.GetResourcesLocator();
                }
                else
                {
                    return null;
                }
            }
        }

        public string GetFilename()
        {
            if (this.IsRootNode())
            {
                return (this.node as ResourceTreeView.RootNode).Filename;
            }
            else
            {
                ResourceTreeView.BaseNode parent = (this.node.Parent as ResourceTreeView.BaseNode);
                if (parent != null)
                {
                    return parent.GetFilename();
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}
