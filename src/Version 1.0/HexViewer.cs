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
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Permissions;

namespace plexdata.Controls
{
    /// <summary>
    /// Multi-line hexadecimal view box.
    /// </summary>
    /// <remarks>
    /// The class <i>HexViewer</i>, derived from class <see cref="Control"/>, 
    /// represents a multi-line view box that is intended to be used to view 
    /// binary data in hexadecimal format.
    /// <para>
    /// This control supports various keys to accomplish the work. See lists 
    /// below which keys and shortcuts are available.
    /// </para>
    /// <para>
    /// <list type="table">
    /// <listheader>
    ///     <term>
    ///         Keys
    ///     </term>
    ///     <description>
    ///         Description
    ///     </description>
    /// </listheader>
    /// <item>
    ///     <term>
    ///         HOME
    ///     </term>
    ///     <description>
    ///         Scrolls the visible content to the very first line. If the 
    ///         <b>CTRL</b> key is also pressed then the first line is adjusted 
    ///         to the very first character.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         END
    ///     </term>
    ///     <description>
    ///         Scrolls the visible content to the very last line. If the 
    ///         <b>CTRL</b> key is also pressed then the last line is adjusted 
    ///         to the very last character.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         UP
    ///     </term>
    ///     <description>
    ///         Scrolls the visible content one line up.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         DOWN
    ///     </term>
    ///     <description>
    ///         Scrolls the visible content one line down.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         LEFT
    ///     </term>
    ///     <description>
    ///         Scrolls the visible content one character left.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         RIGHT
    ///     </term>
    ///     <description>
    ///         Scrolls the visible content one character right.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         PAGE UP
    ///     </term>
    ///     <description>
    ///         Scrolls the visible content one page up.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         PAGE DOWN
    ///     </term>
    ///     <description>
    ///         Scrolls the visible content one page down.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         TAB
    ///     </term>
    ///     <description>
    ///         Puts the keyboard input focus either onto the next control 
    ///         or onto the previous control if the <b>SHIFT</b> is pressed.
    ///     </description>
    /// </item>
    /// <item>
    ///     <term>
    ///         APPS
    ///     </term>
    ///     <description>
    ///         Shows the control's context menu.
    ///     </description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class HexViewer : Control
    {
        #region Private member variable declaration.

        /// <summary>
        /// The number of characters used to draw a single byte.
        /// </summary>
        private const int CHARS_PER_BYTE = 2;

        /// <summary>
        /// The font depending width of one single character.
        /// </summary>
        /// <seealso cref="CalculateCharacterSize"/>
        private int cxChar = 0;

        /// <summary>
        /// The font depending height of one single character. Is equivalent 
        /// to the height of one single line.
        /// </summary>
        /// <seealso cref="CalculateCharacterSize"/>
        private int cyChar = 0;

        /// <summary>
        /// The internally used format to calculate and to draw strings.
        /// </summary>
        /// <seealso cref="CalculateCharacterSize"/>
        private StringFormat stringFormat = null;

        /// <summary>
        /// Indicates whether the control has the mouse focus.
        /// </summary>
        /// <remarks>
        /// This member variable is needed to handle mouse leave on task 
        /// switch! Note, using property <i>Capture</i> instead is not a 
        /// good idea.
        /// </remarks>
        private bool mouseCaptured = false;

        /// <summary>
        /// The last known position of the hot-track bar.
        /// </summary>
        private int lastHotTrack = -1;

        /// <summary>
        /// The instance of the default context menu.
        /// </summary>
        private ContextMenu defaultMenu = null;

        #endregion // Private member variable declaration.

        #region Public constructor implementations.

        /// <summary>
        /// Initializes a new instance of the <i>HexViewer</i> class with 
        /// default settings.
        /// </summary>
        public HexViewer()
            : base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.SetupScrollbars();

            // Set other that gray as default color!
            this.BackColor = SystemColors.Window;
            this.ForeColor = SystemColors.WindowText;

            // Change default font.
            this.Font = new Font(FontFamily.GenericMonospace, 10F);

#if DEBUG
            // ATTENTION: This code initializes the buffer in debug mode.
            byte[] buffer = new byte[300];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(i & 0xFF);
            }
            this.buffer = buffer;
#endif // DEBUG
        }

        #endregion // Public constructor implementations.

        #region Property AddressWidth related implementations.

        /// <summary>
        /// The number of characters to draw the address. Default address 
        /// width is 8.
        /// </summary>
        /// <seealso cref="AddressWidth"/>
        private int addressWidth = 8;

        /// <summary>
        /// Gets or sets the number of characters used for the address field. 
        /// Allowed is  2, 4 or 8!
        /// </summary>
        /// <seealso cref="addressWidth"/>
        /// <seealso cref="AddressWidthChanged"/>
        /// <seealso cref="OnAddressWidthChanged"/>
        [DefaultValue(8)]
        [Category("Appearance")]
        [Description("Gets or sets the number of characters used for the address field. Allowed is  2, 4 or 8!")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int AddressWidth
        {
            get { return this.addressWidth; }
            set
            {
                if (this.addressWidth != value)
                {
                    if (value == 2 || value == 4 || value == 8)
                    {
                        // Change current value.
                        this.addressWidth = value;

                        // Raise change event.
                        this.OnAddressWidthChanged(EventArgs.Empty);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(
                            "Address width must be either 2 or 4 or 8!",
                            (Exception)null);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>AddressWidth</i> changed.
        /// </summary>
        /// <seealso cref="AddressWidth"/>
        /// <seealso cref="OnAddressWidthChanged"/>
        [Description("Occurs when the value of property AddressWidth changed.")]
        public event EventHandler<EventArgs> AddressWidthChanged;

        /// <summary>
        /// Raises the <i>AddressWidthChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="AddressWidth"/>
        /// <seealso cref="AddressWidthChanged"/>
        protected virtual void OnAddressWidthChanged(EventArgs args)
        {
            // Reset to visual bounds.
            this.ScrollIntoView();

            EventHandler<EventArgs> handler = AddressWidthChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property AddressWidth related implementations.

        #region Property BytesPerLine related implementations.

        /// <summary>
        /// The number of bytes to draw a single line. Default number of 
        /// bytes per line are 16.
        /// </summary>
        /// <seealso cref="BytesPerLine"/>
        private int bytesPerLine = 16;

        /// <summary>
        /// Gets or sets the number of bytes per line.
        /// </summary>
        /// <remarks>
        /// There are no limitations except the current block width.
        /// </remarks>
        /// <seealso cref="BlockWidth"/>
        /// <seealso cref="bytesPerLine"/>
        /// <seealso cref="BytesPerLineChanged"/>
        /// <seealso cref="OnBytesPerLineChanged"/>
        [DefaultValue(16)]
        [Category("Appearance")]
        [Description("Gets or sets the number of bytes per line.")]
        [RefreshProperties(RefreshProperties.All)]
        public int BytesPerLine
        {
            get { return this.bytesPerLine; }
            set
            {
                if (value > 0 && this.bytesPerLine != value)
                {
                    // Change current value.
                    this.bytesPerLine = value;

                    if ((this.bytesPerLine * CHARS_PER_BYTE) % this.blockWidth != 0)
                    {
                        // Simply reset to default value...
                        this.BlockWidth = 2;
                    }

                    // Raise change event.
                    this.OnBytesPerLineChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>BytesPerLine</i> changed.
        /// </summary>
        /// <seealso cref="BytesPerLine"/>
        /// <seealso cref="OnBytesPerLineChanged"/>
        [Description("Occurs when the value of property BytesPerLine changed.")]
        public event EventHandler<EventArgs> BytesPerLineChanged;

        /// <summary>
        /// Raises the <i>BytesPerLineChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="BytesPerLine"/>
        /// <seealso cref="BytesPerLineChanged"/>
        protected virtual void OnBytesPerLineChanged(EventArgs args)
        {
            // Reset to visual bounds.
            this.ScrollIntoView();

            EventHandler<EventArgs> handler = BytesPerLineChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property BytesPerLine related implementations.

        #region Property BlockWidth related implementations.

        /// <summary>
        /// The number of nibbles per byte block. Default number of 
        /// nibbles is 2.
        /// </summary>
        /// <seealso cref="BlockWidth"/>
        private int blockWidth = 2;

        /// <summary>
        /// Gets or sets the number of nibbles per byte block. Allowed 
        /// is 2, 4 or 8!
        /// </summary>
        /// <seealso cref="BytesPerLine"/>
        /// <seealso cref="blockWidth"/>
        /// <seealso cref="BlockWidthChanged"/>
        /// <seealso cref="OnBlockWidthChanged"/>
        [DefaultValue(2)]
        [Category("Appearance")]
        [Description("Gets or sets the number of nibbles per byte block. Allowed is 2, 4 or 8!")]
        [RefreshProperties(RefreshProperties.All)]
        public int BlockWidth
        {
            get { return this.blockWidth; }
            set
            {
                if (this.blockWidth != value)
                {
                    if (value == 2 || value == 4 || value == 8)
                    {
                        // Change current value.
                        this.blockWidth = value;

                        if ((this.bytesPerLine * CHARS_PER_BYTE) % this.blockWidth != 0)
                        {
                            // Simply reset to default value...
                            this.BytesPerLine = this.blockWidth * CHARS_PER_BYTE;
                        }

                        // Raise change event.
                        this.OnBlockWidthChanged(EventArgs.Empty);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(
                            "Byte block width must be either 2 or 4 or 8!",
                            (Exception)null);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>BlockWidth</i> changed.
        /// </summary>
        /// <seealso cref="BlockWidth"/>
        /// <seealso cref="OnBlockWidthChanged"/>
        [Description("Occurs when the value of property BlockWidth changed.")]
        public event EventHandler<EventArgs> BlockWidthChanged;

        /// <summary>
        /// Raises the <i>BlockWidthChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="BlockWidth"/>
        /// <seealso cref="BlockWidthChanged"/>
        protected virtual void OnBlockWidthChanged(EventArgs args)
        {
            // Reset to visual bounds.
            this.ScrollIntoView();

            EventHandler<EventArgs> handler = BlockWidthChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property BlockWidth related implementations.

        #region Property SeparatorWidth related implementations.

        /// <summary>
        /// The number of characters used to separate each byte block. 
        /// Default number of separators is 1.
        /// </summary>
        /// <seealso cref="SeparatorWidth"/>
        private int separatorWidth = 1;

        /// <summary>
        /// Gets or sets the amount of separators between each byte block. 
        /// Allowed is 0..5!
        /// </summary>
        /// <seealso cref="separatorWidth"/>
        /// <seealso cref="SeparatorWidthChanged"/>
        /// <seealso cref="OnSeparatorWidthChanged"/>
        [DefaultValue(1)]
        [Description("Gets or sets the amount of separators between each byte block. Allowed is 0..5!")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int SeparatorWidth
        {
            get { return this.separatorWidth; }
            set
            {
                if (this.separatorWidth != value)
                {
                    if (0 <= value && value <= 5)
                    {
                        // Change current value.
                        this.separatorWidth = value;

                        // Raise change event.
                        this.OnSeparatorWidthChanged(EventArgs.Empty);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(
                            "Separator width must be in range of  0..5!",
                            (Exception)null);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>SeparatorWidth</i> changed.
        /// </summary>
        /// <seealso cref="SeparatorWidth"/>
        /// <seealso cref="OnSeparatorWidthChanged"/>
        [Description("Occurs when the value of property SeparatorWidth changed.")]
        public event EventHandler<EventArgs> SeparatorWidthChanged;

        /// <summary>
        /// Raises the <i>SeparatorWidthChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="SeparatorWidth"/>
        /// <seealso cref="SeparatorWidthChanged"/>
        protected virtual void OnSeparatorWidthChanged(EventArgs args)
        {
            // Reset to visual bounds.
            this.ScrollIntoView();

            EventHandler<EventArgs> handler = SeparatorWidthChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property SeparatorWidth related implementations.

        #region Property GapWidth related implementations.

        /// <summary>
        /// The number of characters used as gap between address, byte block 
        /// and text block. Default gap width is 2.
        /// </summary>
        /// <seealso cref="GapWidth"/>
        private int gapWidth = 2;

        /// <summary>
        /// Gets or sets the width of the gap to the left and to the right 
        /// of the byte block. Allowed is 1..5!
        /// </summary>
        /// <seealso cref="gapWidth"/>
        /// <seealso cref="GapWidthChanged"/>
        /// <seealso cref="OnGapWidthChanged"/>
        [DefaultValue(2)]
        [Category("Appearance")]
        [Description("Gets or sets the width of the gap to the left and to the right of the byte block. Allowed is 1..5!")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int GapWidth
        {
            get { return this.gapWidth; }
            set
            {
                if (this.gapWidth != value)
                {
                    if (1 <= value && value <= 5)
                    {
                        // Change current value.
                        this.gapWidth = value;

                        // Raise change event.
                        this.OnGapWidthChanged(EventArgs.Empty);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(
                            "Gap width must be in range of  1..5!",
                            (Exception)null);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>GapWidth</i> changed.
        /// </summary>
        /// <seealso cref="GapWidth"/>
        /// <seealso cref="OnGapWidthChanged"/>
        [Description("Occurs when the value of property GapWidth changed.")]
        public event EventHandler<EventArgs> GapWidthChanged;

        /// <summary>
        /// Raises the <i>GapWidthChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="GapWidth"/>
        /// <seealso cref="GapWidthChanged"/>
        protected virtual void OnGapWidthChanged(EventArgs args)
        {
            // Reset to visual bounds.
            this.ScrollIntoView();

            EventHandler<EventArgs> handler = GapWidthChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property GapWidth related implementations.

        #region Property UpperCase related implementations.

        /// <summary>
        /// The value indicating the usage of capital letters. Capital 
        /// letters are used by default.
        /// </summary>
        /// <seealso cref="UpperCase"/>
        private bool upperCase = true;

        /// <summary>
        /// Gets or sets the value determining whether displayed text uses 
        /// capital letters.
        /// </summary>
        /// <seealso cref="upperCase"/>
        /// <seealso cref="UpperCaseChanged"/>
        /// <seealso cref="OnUpperCaseChanged"/>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Determines whether displayed text uses capital letters.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool UpperCase
        {
            get { return this.upperCase; }
            set
            {
                if (this.upperCase != value)
                {
                    // Change current value.
                    this.upperCase = value;

                    // Raise change event.
                    this.OnUpperCaseChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>UpperCase</i> changed.
        /// </summary>
        /// <seealso cref="UpperCase"/>
        /// <seealso cref="OnUpperCaseChanged"/>
        [Description("Occurs when the value of property UpperCase changed.")]
        public event EventHandler<EventArgs> UpperCaseChanged;

        /// <summary>
        /// Raises the <i>UpperCaseChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="UpperCase"/>
        /// <seealso cref="UpperCaseChanged"/>
        protected virtual void OnUpperCaseChanged(EventArgs args)
        {
            // Just redraw current content!
            this.Invalidate();

            EventHandler<EventArgs> handler = UpperCaseChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property UpperCase related implementations.

        #region Property ShowAddress related implementations.

        /// <summary>
        /// The value indicating the usage of addresses. Addresses are 
        /// shown by default.
        /// </summary>
        /// <seealso cref="ShowAddress"/>
        private bool showAddress = true;

        /// <summary>
        /// Gets or sets the value determining whether the address column 
        /// is shown or not.
        /// </summary>
        /// <seealso cref="showAddress"/>
        /// <seealso cref="ShowAddressChanged"/>
        /// <seealso cref="OnShowAddressChanged"/>
        /// <seealso cref="ShowEmptyAddress"/>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Determines whether the address column is shown or not.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool ShowAddress
        {
            get { return this.showAddress; }
            set
            {
                if (this.showAddress != value)
                {
                    // Change current value.
                    this.showAddress = value;

                    // Raise change event.
                    this.OnShowAddressChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>ShowAddress</i> changed.
        /// </summary>
        /// <seealso cref="ShowAddress"/>
        /// <seealso cref="OnShowAddressChanged"/>
        [Description("Occurs when the value of property ShowAddress changed.")]
        public event EventHandler<EventArgs> ShowAddressChanged;

        /// <summary>
        /// Raises the <i>ShowAddressChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ShowAddress"/>
        /// <seealso cref="ShowAddressChanged"/>
        protected virtual void OnShowAddressChanged(EventArgs args)
        {
            // Reset to visual bounds.
            this.ScrollIntoView();

            EventHandler<EventArgs> handler = ShowAddressChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property ShowAddress related implementations.

        #region Property ShowEmptyAddress related implementations.

        /// <summary>
        /// The value indicating the usage of addresses even if buffer 
        /// is empty. Addresses are shown by default.
        /// </summary>
        /// <seealso cref="ShowEmptyAddress"/>
        private bool showEmptyAddress = true;

        /// <summary>
        /// Gets or sets the value determining whether the address column 
        /// is shown or not even if buffer is empty.
        /// </summary>
        /// <remarks>
        /// Usually, it is not necessary to show an address value if the 
        /// internal buffer is currently empty. But some users may prefer a 
        /// control with content. Therefore, enable this property to ensure 
        /// the visibility of first possible address <c>0x00000000</c> if 
        /// current buffer is empty. If nothing should be shown in case of 
        /// current buffer is empty then just disable this property. But note 
        /// this property also depends on property <see cref="ShowAddress"/>! 
        /// This means if property <see cref="ShowAddress"/> is disabled then 
        /// this property will not have any kind of effect.
        /// </remarks>
        /// <seealso cref="showEmptyAddress"/>
        /// <seealso cref="ShowEmptyAddressChanged"/>
        /// <seealso cref="OnShowEmptyAddressChanged"/>
        /// <seealso cref="ShowAddress"/>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Determines whether the address column is shown or not even if buffer is empty.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool ShowEmptyAddress
        {
            get { return this.showEmptyAddress; }
            set
            {
                if (this.showEmptyAddress != value)
                {
                    // Change current value.
                    this.showEmptyAddress = value;

                    // Raise change event.
                    this.OnShowEmptyAddressChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>ShowEmptyAddress</i> changed.
        /// </summary>
        /// <seealso cref="ShowEmptyAddress"/>
        /// <seealso cref="OnShowEmptyAddressChanged"/>
        [Description("Occurs when the value of property ShowEmptyAddress changed.")]
        public event EventHandler<EventArgs> ShowEmptyAddressChanged;

        /// <summary>
        /// Raises the <i>ShowEmptyAddressChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ShowEmptyAddress"/>
        /// <seealso cref="ShowEmptyAddressChanged"/>
        protected virtual void OnShowEmptyAddressChanged(EventArgs args)
        {
            // Just redraw current content!
            this.Invalidate();

            EventHandler<EventArgs> handler = ShowEmptyAddressChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property ShowEmptyAddress related implementations.

        #region Property ShowTextBlock related implementations.

        /// <summary>
        /// The value indicating the usage of the text block. Text block 
        /// is shown by default.
        /// </summary>
        /// <seealso cref="ShowTextBlock"/>
        private bool showTextBlock = true;

        /// <summary>
        /// Gets or sets the value determining whether the text block is 
        /// shown or not.
        /// </summary>
        /// <seealso cref="showTextBlock"/>
        /// <seealso cref="ShowTextBlockChanged"/>
        /// <seealso cref="OnShowTextBlockChanged"/>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Determines whether the text block is shown or not.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool ShowTextBlock
        {
            get { return this.showTextBlock; }
            set
            {
                if (this.showTextBlock != value)
                {
                    // Change current value.
                    this.showTextBlock = value;

                    // Raise change event.
                    this.OnShowTextBlockChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>ShowTextBlock</i> changed.
        /// </summary>
        /// <seealso cref="ShowTextBlock"/>
        /// <seealso cref="OnShowTextBlockChanged"/>
        [Description("Occurs when the value of property ShowTextBlock changed.")]
        public event EventHandler<EventArgs> ShowTextBlockChanged;

        /// <summary>
        /// Raises the <i>ShowTextBlockChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ShowTextBlock"/>
        /// <seealso cref="ShowTextBlockChanged"/>
        protected virtual void OnShowTextBlockChanged(EventArgs args)
        {
            // Reset to visual bounds.
            this.ScrollIntoView();

            EventHandler<EventArgs> handler = ShowTextBlockChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property ShowTextBlock related implementations.

        #region Property ShowPadding related implementations.

        /// <summary>
        /// The value indicating the usage of byte paddings. Byte padding 
        /// is shown by default.
        /// </summary>
        /// <seealso cref="ShowPadding"/>
        private bool showPadding = true;

        /// <summary>
        /// Gets or sets the value determining whether the hexadecimal block 
        /// shows a byte padding or not.
        /// </summary>
        /// <seealso cref="showPadding"/>
        /// <seealso cref="ShowPaddingChanged"/>
        /// <seealso cref="OnShowPaddingChanged"/>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Determines whether the hexadecimal block shows a byte padding or not.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool ShowPadding
        {
            get { return this.showPadding; }
            set
            {
                if (this.showPadding != value)
                {
                    // Change current value.
                    this.showPadding = value;

                    // Raise change event.
                    this.OnShowPaddingChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>ShowPadding</i> changed.
        /// </summary>
        /// <seealso cref="ShowPadding"/>
        /// <seealso cref="OnShowPaddingChanged"/>
        [Description("Occurs when the value of property ShowPadding changed.")]
        public event EventHandler<EventArgs> ShowPaddingChanged;

        /// <summary>
        /// Raises the <i>ShowPaddingChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ShowPadding"/>
        /// <seealso cref="ShowPaddingChanged"/>
        protected virtual void OnShowPaddingChanged(EventArgs args)
        {
            // Just redraw current content!
            this.Invalidate();

            EventHandler<EventArgs> handler = ShowPaddingChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property ShowTextBlock related implementations.

        #region Property HotTrack related implementations.

        /// <summary>
        /// The value indicating the usage of the hot-track. Hot-track is 
        /// shown by default.
        /// </summary>
        /// <seealso cref="HotTrack"/>
        private bool hotTrack = true;

        /// <summary>
        /// Gets or sets the value determining whether the text under the 
        /// mouse pointer is highlighted or not.
        /// </summary>
        /// <seealso cref="hotTrack"/>
        /// <seealso cref="HotTrackChanged"/>
        /// <seealso cref="OnHotTrackChanged"/>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Determines whether the text under the mouse pointer is highlighted or not.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool HotTrack
        {
            get { return this.hotTrack; }
            set
            {
                if (this.hotTrack != value)
                {
                    // Change current value.
                    this.hotTrack = value;

                    // Raise change event.
                    this.OnHotTrackChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>HotTrack</i> changed.
        /// </summary>
        /// <seealso cref="HotTrack"/>
        /// <seealso cref="OnHotTrackChanged"/>
        [Description("Occurs when the value of property HotTrack changed.")]
        public event EventHandler<EventArgs> HotTrackChanged;

        /// <summary>
        /// Raises the <i>HotTrackChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="HotTrack"/>
        /// <seealso cref="HotTrackChanged"/>
        protected virtual void OnHotTrackChanged(EventArgs args)
        {
            // Just redraw current content!
            this.Invalidate();

            EventHandler<EventArgs> handler = HotTrackChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property HotTrack related implementations.

        #region Property HotTrackColor related implementations.

        /// <summary>
        /// The value of currently used hot-track color. Default hot-track 
        /// color is set to RGB(220, 234, 248).
        /// </summary>
        /// <seealso cref="HotTrackColor"/>
        private Color hotTrackColor = Color.FromArgb(220, 234, 248);

        /// <summary>
        /// Gets or sets the color that is used to highlight the text under 
        /// the mouse pointer.
        /// </summary>
        /// <remarks>
        /// Don't use a transparent color for two reasons:
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///         Filling a bitmap with a transparent color and drawing the 
        ///         text afterwards looks creepy.
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///         The hot-track should not be merged with current background.
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <seealso cref="hotTrackColor"/>
        /// <seealso cref="HotTrackColorChanged"/>
        /// <seealso cref="OnHotTrackColorChanged"/>
        [DefaultValue(typeof(Color), "220, 234, 248")]
        [Category("Appearance")]
        [Description("The color that is used to highlight the text under the mouse pointer.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color HotTrackColor
        {
            // REMARK: Don't use a transparent color for two reasons...
            // 1) Filling a bitmap with a transparent color and 
            //    drawing the text afterwards looks creepy.
            // 2) The hot-track should not be merged with current 
            //    background.
            get { return this.hotTrackColor; }
            set
            {
                if (this.hotTrackColor != value)
                {
                    // Reset default color if empty.
                    if (value.IsEmpty) { value = Color.FromArgb(220, 234, 248); }

                    // Change current value.
                    this.hotTrackColor = value;

                    // Raise change event.
                    this.OnHotTrackColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>HotTrackColor</i> changed.
        /// </summary>
        /// <seealso cref="HotTrackColor"/>
        /// <seealso cref="OnHotTrackColorChanged"/>
        [Description("Occurs when the value of property HotTrackColor changed.")]
        public event EventHandler<EventArgs> HotTrackColorChanged;

        /// <summary>
        /// Raises the <i>HotTrackColorChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="HotTrackColor"/>
        /// <seealso cref="HotTrackColorChanged"/>
        protected virtual void OnHotTrackColorChanged(EventArgs args)
        {
            // Just redraw current content!
            this.Invalidate();

            EventHandler<EventArgs> handler = HotTrackColorChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property HotTrackColor related implementations.

        #region Property BorderStyle related implementations.

        /// <summary>
        /// The value of currently used border style. Default border 
        /// style is Flat.
        /// </summary>
        /// <seealso cref="BorderStyle"/>
        private Border3DStyle borderStyle = Border3DStyle.Flat;

        /// <summary>
        /// Gets or sets the value indicating the border style of this 
        /// control.
        /// </summary>
        /// <seealso cref="borderStyle"/>
        /// <seealso cref="BorderStyleChanged"/>
        /// <seealso cref="OnBorderStyleChanged"/>
        [DefaultValue(Border3DStyle.Flat)]
        [Category("Appearance")]
        [Description("Indicates the border style of this control.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Border3DStyle BorderStyle
        {
            get { return this.borderStyle; }
            set
            {
                if (this.borderStyle != value)
                {
                    // Change current value.
                    this.borderStyle = value;

                    // Raise change event.
                    this.OnBorderStyleChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>BorderStyle</i> changed.
        /// </summary>
        /// <seealso cref="BorderStyle"/>
        /// <seealso cref="OnBorderStyleChanged"/>
        [Description("Occurs when the value of property BorderStyle changed.")]
        public event EventHandler<EventArgs> BorderStyleChanged;

        /// <summary>
        /// Raises the <i>BorderStyleChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="BorderStyle"/>
        /// <seealso cref="BorderStyleChanged"/>
        protected virtual void OnBorderStyleChanged(EventArgs args)
        {
            // Update scrollbars in this case!
            this.UpdateScrollbars();

            // And redraw current content!
            this.Invalidate();

            EventHandler<EventArgs> handler = BorderStyleChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property BorderStyle related implementations.

        #region Property ContextMenuEnabled related implementations.

        /// <summary>
        /// The value indicating the usage of a context menu. Context 
        /// menu is enabled by default.
        /// </summary>
        /// <seealso cref="ContextMenuEnabled"/>
        private bool contextMenuEnabled = true;

        /// <summary>
        /// Gets or sets the value determining whether a context menu 
        /// is supported or not.
        /// </summary>
        /// <seealso cref="contextMenuEnabled"/>
        /// <seealso cref="ContextMenuEnabledChanged"/>
        /// <seealso cref="OnContextMenuEnabledChanged"/>
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Determines whether a context menu is supported or not.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool ContextMenuEnabled
        {
            get { return this.contextMenuEnabled; }
            set
            {
                if (this.contextMenuEnabled != value)
                {
                    // Change current value.
                    this.contextMenuEnabled = value;

                    // Raise change event.
                    this.OnContextMenuEnabledChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>ContextMenuEnabled</i> 
        /// changed.
        /// </summary>
        /// <seealso cref="ContextMenuEnabled"/>
        /// <seealso cref="OnContextMenuEnabledChanged"/>
        [Description("Occurs when the value of property ContextMenuEnabled changed.")]
        public event EventHandler<EventArgs> ContextMenuEnabledChanged;

        /// <summary>
        /// Raises the <i>ContextMenuEnabledChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ContextMenuEnabled"/>
        /// <seealso cref="ContextMenuEnabledChanged"/>
        protected virtual void OnContextMenuEnabledChanged(EventArgs args)
        {
            EventHandler<EventArgs> handler = ContextMenuEnabledChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property ContextMenuEnabled related implementations.

        #region Property Buffer related implementations.

        /// <summary>
        /// The reference of currently used buffer. The buffer is empty 
        /// (but not null) by default.
        /// </summary>
        /// <seealso cref="Buffer"/>
        private byte[] buffer = new byte[0];

        /// <summary>
        /// Gets or sets the content of currently used buffer.
        /// </summary>
        /// <seealso cref="buffer"/>
        /// <seealso cref="BufferChanged"/>
        /// <seealso cref="OnBufferChanged"/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] Buffer
        {
            get { return this.buffer; }
            set
            {
                if (value != null && value.Length > 0)
                {
                    this.buffer = (byte[])value.Clone();
                }
                else
                {
                    this.buffer = new byte[0];
                }

                // Raise change event.
                this.OnBufferChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the value of property <i>Buffer</i> changed.
        /// </summary>
        /// <seealso cref="Buffer"/>
        /// <seealso cref="OnBufferChanged"/>
        [Description("Occurs when the value of property Buffer changed.")]
        public event EventHandler<EventArgs> BufferChanged;

        /// <summary>
        /// Raises the <i>BufferChanged</i> event. 
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="Buffer"/>
        /// <seealso cref="BufferChanged"/>
        protected virtual void OnBufferChanged(EventArgs args)
        {
            // Reset current scrollbar values.
            this.vertScrollbar.Value = this.vertScrollbar.Minimum;
            this.horzScrollbar.Value = this.horzScrollbar.Minimum;

            // Just update scrollbars.
            this.UpdateScrollbars();

            // And redraw current content.
            this.Invalidate();

            EventHandler<EventArgs> handler = BufferChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion // Property Buffer related implementations.

        #region Inherited default property re-implementations.

        /// <summary>
        /// Gets the control's default size which is 300 by 200 pixels. 
        /// </summary>
        protected override Size DefaultSize
        {
            get { return new Size(300, 200); }
        }

        /// <summary>
        /// Gets the control's default minimum size which is 150 by 50 
        /// pixels. 
        /// </summary>
        protected override Size DefaultMinimumSize
        {
            get { return new Size(150, 50); }
        }

        /// <summary>
        /// Gets the control's additional default padding which is 2 
        /// for all sides.
        /// </summary>
        protected override Padding DefaultPadding
        {
            get { return new Padding(2); }
        }

        #endregion // Inherited default property re-implementations.

        #region Internal property implementations.

        /// <summary>
        /// Gets the width of the address block (right gap included) in 
        /// character counts. 
        /// </summary>
        /// <seealso cref="ShowAddress"/>
        /// <seealso cref="AddressWidth"/>
        /// <seealso cref="GapWidth"/>
        /// <seealso cref="LineLength"/>
        protected int AddressLength
        {
            get { return (this.showAddress ? (this.addressWidth + this.gapWidth) : 0); }
        }

        /// <summary>
        /// Gets the width of the binary block in character counts.
        /// </summary>
        /// <seealso cref="CHARS_PER_BYTE"/>
        /// <seealso cref="BlockWidth"/>
        /// <seealso cref="BytesPerLine"/>
        /// <seealso cref="SeparatorWidth"/>
        /// <seealso cref="LineLength"/>
        protected int HexBlockLength
        {
            get
            {
                return
                    CHARS_PER_BYTE * this.bytesPerLine *
                    (this.blockWidth + this.separatorWidth) / this.blockWidth -
                    this.separatorWidth;
            }
        }

        /// <summary>
        /// Gets the width of the text block (left gap included) in character 
        /// counts.
        /// </summary>
        /// <seealso cref="ShowTextBlock"/>
        /// <seealso cref="BytesPerLine"/>
        /// <seealso cref="GapWidth"/>
        /// <seealso cref="LineLength"/>
        protected int TextBlockLength
        {
            get { return (this.showTextBlock ? (this.gapWidth + this.bytesPerLine) : 0); }
        }

        /// <summary>
        /// Gets the width of a complete line in character counts.
        /// </summary>
        /// <seealso cref="AddressLength"/>
        /// <seealso cref="HexBlockLength"/>
        /// <seealso cref="TextBlockLength"/>
        /// <seealso cref="LineCount"/>
        protected int LineLength
        {
            get
            {
                if (this.buffer.Length > 0)
                {
                    return this.AddressLength + this.HexBlockLength + this.TextBlockLength;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the total number of available lines in character counts 
        /// depending on current buffer size and bytes per line.
        /// </summary>
        /// <seealso cref="BytesPerLine"/>
        /// <seealso cref="LineLength"/>
        protected int LineCount
        {
            get
            {
                int length = this.buffer.Length;

                if (length % this.bytesPerLine == 0)
                {
                    return length / this.bytesPerLine;
                }
                else
                {
                    return length / this.bytesPerLine + 1;
                }
            }
        }

        /// <summary>
        /// Gets the first visible line in character counts.
        /// </summary>
        /// <seealso cref="VisibleLines"/>
        protected int FirstLine
        {
            get
            {
                if (this.vertScrollbar.Visible)
                {
                    return this.vertScrollbar.Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the number of currently visible lines in character counts.
        /// </summary>
        /// <seealso cref="FirstLine"/>
        protected int VisibleLines
        {
            get
            {
                return Math.Min(
                    this.DrawingBounds.Height / this.cyChar,
                    Math.Max(this.LineCount - this.FirstLine, 0));
            }
        }

        /// <summary>
        /// Gets the first visible character in character counts.
        /// </summary>
        /// <seealso cref="VisibleChars"/>
        protected int FirstChar
        {
            get
            {
                if (this.horzScrollbar.Visible)
                {
                    return this.horzScrollbar.Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the number of currently visible characters in character 
        /// counts.
        /// </summary>
        /// <seealso cref="FirstChar"/>
        protected int VisibleChars
        {
            get
            {
                return Math.Min(
                    this.DrawingBounds.Width / this.cxChar,
                    Math.Max(this.LineLength - this.FirstChar, 0));
            }
        }

        /// <summary>
        /// Gets the visible bounding rectangle of the address block 
        /// excluding the gap.
        /// </summary>
        /// <remarks>
        /// This member function is Reserved for Future Use (RFU).
        /// </remarks>
        /// <seealso cref="PointInAddressBlock"/>
        protected Rectangle AddressBounds
        {
            get
            {
                Rectangle bounds = this.DrawingBounds;
                int offset = 0;
                int left = bounds.Left + (offset - this.FirstChar) * this.cxChar;
                int top = bounds.Top;
                int width = (this.AddressLength - this.gapWidth) * this.cxChar;
                int height = this.VisibleLines * this.cyChar;

                // Save currently calculated width.
                int temp = width;

                // Adjust to left sided bounds if needed.
                if (left < bounds.Left)
                {
                    width -= bounds.Left - left;
                    left = bounds.Left;
                }

                // Adjust width again if needed.
                if (left + temp > bounds.Right)
                {
                    width = Math.Min(bounds.Right - left, width);
                }

                // Block is currently invisible.
                if (width <= 0)
                {
                    return Rectangle.Empty;
                }
                else
                {
                    return new Rectangle(left, top, width, height);
                }
            }
        }

        /// <summary>
        /// Gets the visible bounding rectangle of the binary block 
        /// excluding the gap.
        /// </summary>
        /// <remarks>
        /// This member function is Reserved for Future Use (RFU).
        /// </remarks>
        /// <seealso cref="PointInHexBlock"/>
        protected Rectangle HexBlockBounds
        {
            get
            {
                Rectangle bounds = this.DrawingBounds;
                int offset = this.AddressLength;
                int left = bounds.Left + (offset - this.FirstChar) * this.cxChar;
                int top = bounds.Top;
                int width = this.HexBlockLength * this.cxChar;
                int height = this.VisibleLines * this.cyChar;

                // Save currently calculated width.
                int temp = width;

                // Adjust to left sided bounds if needed.
                if (left < bounds.Left)
                {
                    width -= bounds.Left - left;
                    left = bounds.Left;
                }

                // Adjust width again if needed.
                if (left + temp > bounds.Right)
                {
                    width = Math.Min(bounds.Right - left, width);
                }

                // Block is currently invisible.
                if (width <= 0)
                {
                    return Rectangle.Empty;
                }
                else
                {
                    return new Rectangle(left, top, width, height);
                }
            }
        }

        /// <summary>
        /// Gets the visible bounding rectangle of the text block 
        /// excluding the gap.
        /// </summary>
        /// <remarks>
        /// This member function is Reserved for Future Use (RFU).
        /// </remarks>
        /// <seealso cref="PointInTextBlock"/>
        protected Rectangle TextBlockBounds
        {
            get
            {
                Rectangle bounds = this.DrawingBounds;
                int offset = this.AddressLength + this.HexBlockLength + this.gapWidth;
                int left = bounds.Left + (offset - this.FirstChar) * this.cxChar;
                int top = bounds.Top;
                int width = (this.TextBlockLength - this.gapWidth) * this.cxChar;
                int height = this.VisibleLines * this.cyChar;

                // Save currently calculated width.
                int temp = width;

                // Adjust to left sided bounds if needed.
                if (left < bounds.Left)
                {
                    width -= bounds.Left - left;
                    left = bounds.Left;
                }

                // Adjust width again if needed.
                if (left + temp > bounds.Right)
                {
                    width = Math.Min(bounds.Right - left, width);
                }

                // Block is currently invisible.
                if (width <= 0)
                {
                    return Rectangle.Empty;
                }
                else
                {
                    return new Rectangle(left, top, width, height);
                }
            }
        }

        /// <summary>
        /// Gets the size of the drawing area depending on current border 
        /// as well as on scrollbar visibility.
        /// </summary>
        /// <seealso cref="BorderBounds"/>
        protected Rectangle DrawingBounds
        {
            get
            {
                Padding border = this.BorderBounds;
                Rectangle client = this.ClientRectangle;
                client.X += border.Left;
                client.Y += border.Top;
                client.Width -= border.Horizontal;
                client.Height -= border.Vertical;

                if (this.vertScrollbar.Visible)
                {
                    // Drawing bounds need to be corrected!
                    client.Width -= this.vertScrollbar.Width;
                }

                if (this.horzScrollbar.Visible)
                {
                    // Drawing bounds need to be corrected!
                    client.Height -= this.horzScrollbar.Height;
                }

                return client;
            }
        }

        /// <summary>
        /// Gets the size of currently set border including currently 
        /// used border padding.
        /// </summary>
        /// <seealso cref="DrawingBounds"/>
        protected Padding BorderBounds
        {
            get
            {
                return new Padding(
                    this.Padding.Left + SystemInformation.Border3DSize.Width,
                    this.Padding.Top + SystemInformation.Border3DSize.Height,
                    this.Padding.Right + SystemInformation.Border3DSize.Width,
                    this.Padding.Bottom + SystemInformation.Border3DSize.Height);
            }
        }

        /// <summary>
        /// Gets the background color depending on current control state.
        /// </summary>
        /// <remarks>
        /// Read-only background color doesn't make sense because it's a 
        /// viewer.
        /// </remarks>
        /// <seealso cref="Foreground"/>
        protected virtual Color Background
        {
            get
            {
                if (this.Enabled)
                {
                    return this.BackColor;
                }
                else
                {
                    return SystemColors.Control;
                }
            }
        }

        /// <summary>
        /// Gets the foreground color depending on current control state.
        /// </summary>
        /// <remarks>
        /// Read-only foreground color doesn't make sense because it's a 
        /// viewer.
        /// </remarks>
        /// <seealso cref="Background"/>
        protected virtual Color Foreground
        {
            get
            {
                if (this.Enabled)
                {
                    return this.ForeColor;
                }
                else
                {
                    return SystemColors.ControlDarkDark;
                }
            }
        }

        #endregion // Internal property implementations.

        #region Scrollbar related implementations.

        /// <summary>
        /// The instance of the vertical scrollbar.
        /// </summary>
        private VScrollBar vertScrollbar = null;

        /// <summary>
        /// The instance of the horizontal scrollbar.
        /// </summary>
        private HScrollBar horzScrollbar = null;

        /// <summary>
        /// Initialize the instances of the vertical and horizontal 
        /// scrollbars.
        /// </summary>
        private void SetupScrollbars()
        {
            this.SuspendLayout();

            if (this.vertScrollbar == null)
            {
                this.vertScrollbar = new VScrollBar();
                this.vertScrollbar.Scroll += new ScrollEventHandler(this.HandleVerticalScroll);
                this.vertScrollbar.Dock = DockStyle.Right;
                this.vertScrollbar.Visible = false;
                this.Controls.Add(this.vertScrollbar);
            }

            if (this.horzScrollbar == null)
            {
                this.horzScrollbar = new HScrollBar();
                this.horzScrollbar.Scroll += new ScrollEventHandler(this.HandleHorizontalScroll);
                this.horzScrollbar.Dock = DockStyle.Bottom;
                this.horzScrollbar.Visible = false;
                this.Controls.Add(this.horzScrollbar);
            }

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Updates the settings of the vertical and horizontal 
        /// scrollbars.
        /// </summary>
        private void UpdateScrollbars()
        {
            // Save current visibility states.
            bool vertOldVisible = this.vertScrollbar.Visible;
            bool horzOldVisible = this.horzScrollbar.Visible;

            // Initialize vertical scrollbar values.
            this.vertScrollbar.Minimum = 0;
            this.vertScrollbar.Maximum = this.LineCount;
            this.vertScrollbar.SmallChange = 1;
            this.vertScrollbar.LargeChange =
                Math.Max(this.DrawingBounds.Height / this.cyChar, 0); // Maximum visible lines!
            this.vertScrollbar.Visible =
                this.vertScrollbar.LargeChange < this.vertScrollbar.Maximum;

            // Initialize horizontal scrollbar values.
            this.horzScrollbar.Minimum = 0;
            this.horzScrollbar.Maximum = this.LineLength;
            this.horzScrollbar.SmallChange = 1;
            this.horzScrollbar.LargeChange =
                Math.Max(this.DrawingBounds.Width / this.cxChar, 0); // Maximum visible characters!
            this.horzScrollbar.Visible =
                this.horzScrollbar.LargeChange < this.horzScrollbar.Maximum;

            // Horizontal scrollbar size correction!
            if (this.vertScrollbar.Visible)
            {
                this.horzScrollbar.MaximumSize = new Size(
                    this.ClientRectangle.Width - this.vertScrollbar.Width,
                    this.horzScrollbar.Height);
            }
            else if (vertOldVisible || this.horzScrollbar.Visible)
            {
                // In this case the scrollbar toggled 
                // from visible into invisible.
                this.horzScrollbar.MaximumSize = new Size(
                    this.ClientRectangle.Width, this.horzScrollbar.Height);
            }

            // Vertical scrollbar size correction!
            if (this.horzScrollbar.Visible)
            {
                this.vertScrollbar.MaximumSize = new Size(
                    this.vertScrollbar.Width,
                    this.ClientRectangle.Height - this.horzScrollbar.Height);
            }
            else if (horzOldVisible || this.vertScrollbar.Visible)
            {
                // In this case the scrollbar toggled 
                // from visible into invisible.
                this.vertScrollbar.MaximumSize = new Size(
                    this.vertScrollbar.Width, this.ClientRectangle.Height);
            }
        }

        /// <summary>
        /// Handles the vertical scrolling events.
        /// </summary>
        /// <remarks>
        /// The entire control is just redrawn.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event (not used).
        /// </param>
        /// <param name="args">
        /// The scrollbar event arguments (not used).
        /// </param>
        private void HandleVerticalScroll(object sender, ScrollEventArgs args)
        {
            // Just redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Handles the horizontal scrolling events.
        /// </summary>
        /// <remarks>
        /// The entire control is just redrawn.
        /// </remarks>
        /// <param name="sender">
        /// The sender of the event (not used).
        /// </param>
        /// <param name="args">
        /// The scrollbar event arguments (not used).
        /// </param>
        private void HandleHorizontalScroll(object sender, ScrollEventArgs args)
        {
            // Just redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Scrolls the visible content one character left.
        /// </summary>
        /// <seealso cref="ScrollLeft(int)"/>
        protected void ScrollLeft()
        {
            this.ScrollLeft(this.horzScrollbar.SmallChange);
        }

        /// <summary>
        /// Scrolls the visible content left using given <i>jumps</i>.
        /// </summary>
        /// <param name="jumps">
        /// The number of characters to scroll.
        /// </param>
        /// <seealso cref="ScrollLeft()"/>
        protected void ScrollLeft(int jumps)
        {
            int value = this.horzScrollbar.Value;
            int first = this.horzScrollbar.Minimum;

            // Fit jumps onto max jumps if needed.
            jumps = Math.Min(jumps, first + value);

            if (jumps >= 0 && value - jumps >= first)
            {
                this.horzScrollbar.Value -= jumps;
            }

            // Now redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Scrolls the visible content one character right.
        /// </summary>
        /// <seealso cref="ScrollRight(int)"/>
        protected void ScrollRight()
        {
            this.ScrollRight(this.horzScrollbar.SmallChange);
        }

        /// <summary>
        /// Scrolls the visible content right using given <i>jumps</i>.
        /// </summary>
        /// <param name="jumps">
        /// The number of characters to scroll.
        /// </param>
        /// <seealso cref="ScrollRight()"/>
        protected void ScrollRight(int jumps)
        {
            int value = this.horzScrollbar.Value;
            int last = this.horzScrollbar.Maximum - Math.Max(this.horzScrollbar.LargeChange - 1, 0);

            // Fit jumps onto max jumps if needed.
            jumps = Math.Min(jumps, last - value);

            if (jumps >= 0 && value + jumps <= last)
            {
                this.horzScrollbar.Value += jumps;
            }

            // Now redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Scrolls the visible content one line up.
        /// </summary>
        /// <seealso cref="ScrollLineUp(int)"/>
        protected void ScrollLineUp()
        {
            this.ScrollLineUp(this.vertScrollbar.SmallChange);
        }

        /// <summary>
        /// Scrolls the visible content up using given <i>lines</i>.
        /// </summary>
        /// <param name="lines">
        /// The number of lines to scroll.
        /// </param>
        /// <seealso cref="ScrollLineUp()"/>
        protected void ScrollLineUp(int lines)
        {
            int value = this.vertScrollbar.Value;
            int first = this.vertScrollbar.Minimum;

            // Fit lines onto max lines if needed.
            lines = Math.Min(lines, first + value);

            if (lines >= 0 && value - lines >= first)
            {
                this.vertScrollbar.Value -= lines;
            }

            // Now redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Scrolls the visible content one line down.
        /// </summary>
        /// <seealso cref="ScrollLineDown(int)"/>
        protected void ScrollLineDown()
        {
            this.ScrollLineDown(this.vertScrollbar.SmallChange);
        }

        /// <summary>
        /// Scrolls the visible content down using given <i>lines</i>.
        /// </summary>
        /// <param name="lines">
        /// The number of lines to scroll.
        /// </param>
        /// <seealso cref="ScrollLineDown()"/>
        protected void ScrollLineDown(int lines)
        {
            int value = this.vertScrollbar.Value;
            int last = this.vertScrollbar.Maximum - Math.Max(this.vertScrollbar.LargeChange - 1, 0);

            // Fit lines onto max lines if needed.
            lines = Math.Min(lines, last - value);

            if (lines >= 0 && value + lines <= last)
            {
                this.vertScrollbar.Value += lines;
            }

            // Now redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Scrolls the visible content one page up.
        /// </summary>
        /// <seealso cref="ScrollPageUp(int)"/>
        protected void ScrollPageUp()
        {
            this.ScrollPageUp(1);
        }

        /// <summary>
        /// Scrolls the visible content up using given <i>pages</i>.
        /// </summary>
        /// <param name="pages">
        /// The number of pages to scroll.
        /// </param>
        /// <seealso cref="ScrollPageUp()"/>
        protected void ScrollPageUp(int pages)
        {
            this.ScrollLineUp(pages * Math.Max(this.vertScrollbar.LargeChange - 1, 0));
        }

        /// <summary>
        /// Scrolls the visible content one page down.
        /// </summary>
        /// <seealso cref="ScrollPageDown(int)"/>
        protected void ScrollPageDown()
        {
            this.ScrollPageDown(1);
        }

        /// <summary>
        /// Scrolls the visible content down using given <i>pages</i>.
        /// </summary>
        /// <param name="pages">
        /// The number of pages to scroll.
        /// </param>
        /// <seealso cref="ScrollPageDown()"/>
        protected void ScrollPageDown(int pages)
        {
            this.ScrollLineDown(pages * Math.Max(this.vertScrollbar.LargeChange - 1, 0));
        }

        /// <summary>
        /// Scrolls the visible content to the very first line. 
        /// </summary>
        /// <remarks>
        /// If the <b>CTRL</b> key is also pressed then the first line 
        /// is adjusted to the very first character.
        /// </remarks>
        /// <seealso cref="ScrollEnd"/>
        protected void ScrollHome()
        {
            this.horzScrollbar.Value = this.horzScrollbar.Minimum;

            if (this.IsControl())
            {
                this.vertScrollbar.Value = this.vertScrollbar.Minimum;
            }

            // Now redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Scrolls the visible content to the very last line. 
        /// </summary>
        /// <remarks>
        /// If the <b>CTRL</b> key is also pressed then the last line 
        /// is adjusted to the very last character.
        /// </remarks>
        /// <seealso cref="ScrollHome"/>
        protected void ScrollEnd()
        {
            this.horzScrollbar.Value =
                Math.Min(
                    this.horzScrollbar.Maximum - (this.horzScrollbar.LargeChange - 1),
                    this.horzScrollbar.Maximum);

            if (this.IsControl())
            {
                this.vertScrollbar.Value =
                    Math.Min(
                        this.vertScrollbar.Maximum - (this.vertScrollbar.LargeChange - 1),
                        this.vertScrollbar.Maximum);
            }

            // Now redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Scrolls the content in a way that as much as possible 
        /// becomes visible.
        /// </summary>
        protected void ScrollIntoView()
        {
            // Update scrollbars in this case!
            this.UpdateScrollbars();

            // Adjust vertical scrollbar.
            if (this.vertScrollbar.Value + this.vertScrollbar.LargeChange >= this.vertScrollbar.Maximum)
            {
                this.vertScrollbar.Value =
                    Math.Max(this.vertScrollbar.Maximum - this.vertScrollbar.LargeChange, 0);
            }

            // Adjust horizontal scrollbar.
            if (this.horzScrollbar.Value + this.horzScrollbar.LargeChange >= this.horzScrollbar.Maximum)
            {
                this.horzScrollbar.Value =
                    Math.Max(this.horzScrollbar.Maximum - this.horzScrollbar.LargeChange, 0);
            }

            // Now redraw current content!
            this.Invalidate();
        }

        #endregion // Scrollbar related implementations.

        #region Keyboard related implementations.

        /// <summary>
        /// Indicates whether the <b>SHIFT</b> key is pressed.
        /// </summary>
        /// <returns>
        /// True, if the <b>SHIFT</b> key is pressed and otherwise false.
        /// </returns>
        private bool IsShift()
        {
            return ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
        }

        /// <summary>
        /// Indicates whether the <b>CTRL</b> key is pressed.
        /// </summary>
        /// <returns>
        /// True, if the <b>CTRL</b> key is pressed and otherwise false.
        /// </returns>
        private bool IsControl()
        {
            return ((Control.ModifierKeys & Keys.Control) == Keys.Control);
        }

        /// <summary>
        /// The value of Windows message <i>key down</i>.
        /// </summary>
        private const int WM_KEYDOWN = 0x0100;

        /// <summary>
        /// The value of Windows message <i>key up</i>.
        /// </summary>
        private const int WM_KEYUP = 0x0101;

        /// <summary>
        /// Preprocesses keyboard or input messages within the message loop 
        /// before they are dispatched.
        /// </summary>
        /// <param name="message">
        /// An instance of class Message, passed by reference, that represents 
        /// the message to process.
        /// </param>
        /// <returns>
        /// True, if the message was processed and otherwise false.
        /// </returns>
        /// <seealso cref="ProcessKeyDown"/>
        /// <seealso cref="ProcessKeyUp"/>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true),
        SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        public override bool PreProcessMessage(ref Message message)
        {
            switch (message.Msg)
            {
                case WM_KEYDOWN:
                    return this.ProcessKeyDown(ref message);
                case WM_KEYUP:
                    return this.ProcessKeyUp(ref message);
                default:
                    return base.PreProcessMessage(ref message);
            }
        }

        /// <summary>
        /// Preprocesses the <i>key down</i> keyboard message.
        /// </summary>
        /// <remarks>
        /// See list below which keyboard keys are currently processed 
        /// by this method.
        /// <para>
        /// <list type="table">
        /// <listheader>
        ///     <term>
        ///         Keys
        ///     </term>
        ///     <description>
        ///         Description
        ///     </description>
        /// </listheader>
        /// <item>
        ///     <term>
        ///         HOME
        ///     </term>
        ///     <description>
        ///         Processes the <b>HOME</b> key and all related key combinations.
        ///         See member function <see cref="ScrollHome"/> for more details.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         END
        ///     </term>
        ///     <description>
        ///         Processes the <b>END</b> key and all related key combinations.
        ///         See member function <see cref="ScrollEnd"/> for more details.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         UP
        ///     </term>
        ///     <description>
        ///         Processes the <b>UP</b> key and all related key combinations.
        ///         See member function <see cref="ScrollLineUp()"/> for more 
        ///         details.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         DOWN
        ///     </term>
        ///     <description>
        ///         Processes the <b>DOWN</b> key and all related key combinations. 
        ///         See member function <see cref="ScrollLineDown()"/> for more 
        ///         details.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         LEFT
        ///     </term>
        ///     <description>
        ///         Processes the <b>LEFT</b> key and all related key combinations.
        ///         See member function <see cref="ScrollLeft()"/> for more details.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         RIGHT
        ///     </term>
        ///     <description>
        ///         Processes the <b>RIGHT</b> key and all related key combinations. 
        ///         See member function <see cref="ScrollRight()"/> for more details.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         PAGE UP
        ///     </term>
        ///     <description>
        ///         Processes the <b>PAGE UP</b> key and all related key combinations.
        ///         See member function <see cref="ScrollPageUp()"/> for more details.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         PAGE DOWN
        ///     </term>
        ///     <description>
        ///         Processes the <b>PAGE DOWN</b> key and all related key combinations. 
        ///         See member function <see cref="ScrollPageDown()"/> for more details.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         TAB
        ///     </term>
        ///     <description>
        ///         Processes the <b>TAB</b> key and puts the keyboard input focus 
        ///         either onto the next control or onto the previous control if the 
        ///         <b>SHIFT</b> is pressed.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>
        ///         APPS
        ///     </term>
        ///     <description>
        ///         Processes the <b>APPS</b> key and all related key combinations. 
        ///         See member function <see cref="ProcessContextMenu"/> for more 
        ///         details.
        ///     </description>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="message">
        /// An instance of class Message, passed by reference, that represents 
        /// the message to process.
        /// </param>
        /// <returns>
        /// True, if the message was processed and otherwise false.
        /// </returns>
        /// <seealso cref="PreProcessMessage"/>
        /// <seealso cref="ProcessKeyUp"/>
        private bool ProcessKeyDown(ref Message message)
        {
            Keys keys = (Keys)message.WParam.ToInt32();

            if (keys == Keys.Home)
            {
                this.ScrollHome();
                return true;
            }
            else if (keys == Keys.End)
            {
                this.ScrollEnd();
                return true;
            }
            else if (keys == Keys.Up)
            {
                this.ScrollLineUp();
                return true;
            }
            else if (keys == Keys.Down)
            {
                this.ScrollLineDown();
                return true;
            }
            else if (keys == Keys.Left)
            {
                this.ScrollLeft();
                return true;
            }
            else if (keys == Keys.Right)
            {
                this.ScrollRight();
                return true;
            }
            else if (keys == Keys.PageUp)
            {
                this.ScrollPageUp();
                return true;
            }
            else if (keys == Keys.PageDown)
            {
                this.ScrollPageDown();
                return true;
            }
            else if (keys == Keys.Tab)
            {
                // Try find next selectable control and jump to.
                Control parent = this.Parent;
                while (parent != null)
                {
                    if (parent.CanSelect)
                    {
                        return parent.SelectNextControl(this, !this.IsShift(), true, true, true);
                    }
                    parent = parent.Parent;
                }
                return false;
            }
            else if (keys == Keys.Apps)
            {
                return this.ProcessContextMenu(this.DrawingBounds.Location);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Preprocesses the <i>key up</i> keyboard message.
        /// </summary>
        /// <remarks>
        /// This member function is Reserved for Future Use (RFU).
        /// </remarks>
        /// <param name="message">
        /// An instance of class Message, passed by reference, that 
        /// represents the message to process.
        /// </param>
        /// <returns>
        /// True, if the message was processed and otherwise false.
        /// </returns>
        /// <seealso cref="PreProcessMessage"/>
        /// <seealso cref="ProcessKeyDown"/>
        private bool ProcessKeyUp(ref Message message)
        {
            return false;
        }

        #endregion // Keyboard related implementations.

        #region Context menu related implementations.

        /// <summary>
        /// Shows the control's context menu.
        /// </summary>
        /// <param name="location">
        /// The location in client coordinates where the context 
        /// menu shall appear.
        /// </param>
        /// <returns>
        /// True, if the message was processed and otherwise false.
        /// </returns>
        /// <seealso cref="ShowDefaultMenu"/>
        /// <seealso cref="ContextMenuEnabled"/>
        private bool ProcessContextMenu(Point location)
        {
            if (this.contextMenuEnabled)
            {
                if (this.ContextMenu != null)
                {
                    this.ContextMenu.Show(this, location);
                }
                else if (this.ContextMenuStrip != null)
                {
                    this.ContextMenuStrip.Show(this, location);
                }
                else
                {
                    // Use default context menu if 
                    // no other menu is defined.
                    this.ShowDefaultMenu(location);
                }
            }
            return true; // Message handled.
        }

        /// <summary>
        /// Shows the control's default context menu.
        /// </summary>
        /// <remarks>
        /// All default context menu related events are handled in 
        /// one single function.
        /// </remarks>
        /// <param name="location">
        /// The location in client coordinates where the context 
        /// menu shall appear.
        /// </param>
        /// <seealso cref="ProcessContextMenu"/>
        /// <seealso cref="DefaultMenuItemClick"/>
        private void ShowDefaultMenu(Point location)
        {
            if (this.defaultMenu != null)
            {
                this.defaultMenu.Dispose();
            }

            this.defaultMenu = new ContextMenu();

            MenuItem item = new MenuItem();
            item.Click += new EventHandler(DefaultMenuItemClick);
            item.RadioCheck = true;

            if (this.showAddress)
            {
                item.Text = "16-bit Address";
                item.Checked = (this.addressWidth == 4);
                item.Tag = 0x0101;
                this.defaultMenu.MenuItems.Add(item);

                item = item.CloneMenu();
                item.Text = "32-bit Address";
                item.Checked = (this.addressWidth == 8);
                item.Tag = 0x0102;
                this.defaultMenu.MenuItems.Add(item);

                this.defaultMenu.MenuItems.Add(new MenuItem("-"));
            }

            item = item.CloneMenu();
            item.Text = "8-bit Block";
            item.Checked = (this.blockWidth == 2);
            item.Tag = 0x0201;
            this.defaultMenu.MenuItems.Add(item);

            item = item.CloneMenu();
            item.Text = "16-bit Block";
            item.Checked = (this.blockWidth == 4);
            item.Tag = 0x0202;
            this.defaultMenu.MenuItems.Add(item);

            item = item.CloneMenu();
            item.Text = "32-bit Block";
            item.Checked = (this.blockWidth == 8);
            item.Tag = 0x0203;
            this.defaultMenu.MenuItems.Add(item);

            this.defaultMenu.MenuItems.Add(new MenuItem("-"));

            item = item.CloneMenu();
            item.RadioCheck = false;
            item.Text = "Upper Cases";
            item.Checked = this.upperCase;
            item.Tag = 0x0301;
            this.defaultMenu.MenuItems.Add(item);

            // Show default context menu.
            this.defaultMenu.Show(this, location);
        }

        /// <summary>
        /// Handles all events of the default context menu.
        /// </summary>
        /// <param name="sender">
        /// The sender (menu item) of the event.
        /// </param>
        /// <param name="args">
        /// The event arguments (not used).
        /// </param>
        /// <seealso cref="ShowDefaultMenu"/>
        private void DefaultMenuItemClick(object sender, EventArgs args)
        {
            try
            {
                MenuItem item = (sender as MenuItem);
                if (item != null)
                {
                    switch ((int)item.Tag)
                    {
                        case 0x0101: // 16-bit Address 
                            this.AddressWidth = 4;
                            break;
                        case 0x0102: // 32-bit Address  
                            this.AddressWidth = 8;
                            break;
                        case 0x0201: // 8-bit Block
                            this.BlockWidth = 2;
                            break;
                        case 0x0202: // 16-bit Block
                            this.BlockWidth = 4;
                            break;
                        case 0x0203: // 32-bit Block
                            this.BlockWidth = 8;
                            break;
                        case 0x0301: // Upper Cases
                            this.UpperCase = !item.Checked;
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        #endregion // Context menu related implementations.

        #region Drawing related implementations.

        /// <summary>
        /// Paints the control's background including the border.
        /// </summary>
        /// <param name="args">
        /// The painting event arguments that contain information 
        /// about the control to paint.
        /// </param>
        /// <seealso cref="OnPaint"/>
        protected override void OnPaintBackground(PaintEventArgs args)
        {
            base.OnPaintBackground(args);

            using (Brush background = new SolidBrush(this.Background))
            {
                Rectangle client = this.ClientRectangle;

                // Deflate drawing rectangle in this case!
                if (this.vertScrollbar.Visible)
                {
                    client.Width -= this.vertScrollbar.Width;
                }

                // Deflate drawing rectangle in this case too!
                if (this.horzScrollbar.Visible)
                {
                    client.Height -= this.horzScrollbar.Height;
                }

                // Also fill this additional rectangle 
                // if both scrollbars are visible!
                if (this.vertScrollbar.Visible && this.horzScrollbar.Visible)
                {
                    args.Graphics.FillRectangle(
                        SystemBrushes.Control,
                        client.Right,
                        client.Bottom,
                        this.vertScrollbar.Width,
                        this.horzScrollbar.Height);
                }

                // Fill the entire client area.
                args.Graphics.FillRectangle(background, client);

                // 3D border drawing!
                ControlPaint.DrawBorder3D(
                    args.Graphics,
                    client,
                    this.borderStyle,
                    Border3DSide.All &      // Draw all borders
                    ~Border3DSide.Middle);  // except inner area.
            }
        }

        /// <summary>
        /// Paints the control's data.
        /// </summary>
        /// <param name="args">
        /// The painting event arguments that contain information 
        /// about the control to paint.
        /// </param>
        /// <seealso cref="OnPaintBackground"/>
        /// <seealso cref="RefreshHotTrack"/>
        /// <seealso cref="DrawDataLine"/>
        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);

            // Draw content rectangle only!
            Rectangle bounds = this.DrawingBounds;
            Region clip = new Region(this.ClientRectangle);
            clip.Exclude(bounds);
            args.Graphics.ExcludeClip(clip);

            using (Brush foreground = new SolidBrush(this.Foreground))
            {
                Font font = this.Font;
                int top = this.FirstLine;
                int lines = this.LineCount;
                int bottom = bounds.Bottom;
                int xOffset = bounds.X - (this.cxChar * this.FirstChar);
                int yOffset = bounds.Y;

                // Draw first address if buffer is empty.
                if (this.buffer.Length == 0)
                {
                    // But only if drawing is enabled.
                    if (this.showAddress && this.showEmptyAddress)
                    {
                        // Draw first possible address if buffer is empty!
                        this.DrawAddress(args.Graphics, font, foreground,
                            bounds, xOffset, yOffset, 0x00000000);
                    }
                }

                // Draw all lines inside the visible range.
                for (int line = top; line < lines && yOffset <= bottom; line++)
                {
                    if (line - top == this.lastHotTrack)
                    {
                        this.RefreshHotTrack(
                            args.Graphics,
                            this.lastHotTrack,
                            this.hotTrackColor);
                    }

                    // Draw data at current line.
                    this.DrawDataLine(
                        args.Graphics,
                        font,
                        foreground,
                        bounds,
                        xOffset,
                        yOffset,
                        line);

                    // Calculate next drawing Y position.
                    yOffset += this.cyChar;
                }
            }
        }

        /// <summary>
        /// Draws the data of given line onto given graphics context.
        /// </summary>
        /// <remarks>
        /// Drawing of "half" lines at the contol's bottom is suppressed 
        /// by debault.
        /// </remarks>
        /// <param name="graphics">
        /// The graphics context to draw on.
        /// </param>
        /// <param name="font">
        /// The font to be used to draw the text.
        /// </param>
        /// <param name="foreground">
        /// The color to be used to draw the text.
        /// </param>
        /// <param name="bounds">
        /// The bounding rectangle of the entire drawing area.
        /// </param>
        /// <param name="xOffset">
        /// The vertical drawing offset.
        /// </param>
        /// <param name="yOffset">
        /// The horizontal drawing offset.
        /// </param>
        /// <param name="line">
        /// The visible index of the line to be drawn.
        /// </param>
        /// <seealso cref="DrawAddress"/>
        /// <seealso cref="DrawByteBlock"/>
        /// <seealso cref="DrawTextBlock"/>
        private void DrawDataLine(Graphics graphics, Font font, Brush foreground, Rectangle bounds, int xOffset, int yOffset, int line)
        {
            // Don't draw a half line!
            if (bounds.Bottom < yOffset + this.cyChar) { return; }

            int count = this.buffer.Length;

            if (this.showAddress)
            {
                // Draw address of current line!
                this.DrawAddress(graphics, font, foreground, bounds, xOffset, yOffset, line);

                // Calculate next drawing X position.
                xOffset += (this.addressWidth + this.gapWidth) * this.cxChar;
            }

            // Draw bytes of current line!
            this.DrawByteBlock(graphics, font, foreground, bounds, xOffset, yOffset, line, count);

            // Calculate current drawing X position.
            xOffset += (this.HexBlockLength + this.gapWidth) * this.cxChar;

            if (this.showTextBlock)
            {
                // Draw text of current line!
                this.DrawTextBlock(graphics, font, foreground, bounds, xOffset, yOffset, line, count);
            }
        }

        /// <summary>
        /// Draws the address of given line onto given graphics context.
        /// </summary>
        /// <remarks>
        /// Drawing the address is suppressed when <i>ShowAddress</i> is 
        /// false.
        /// </remarks>
        /// <param name="graphics">
        /// The graphics context to draw on.
        /// </param>
        /// <param name="font">
        /// The font to be used to draw the text.
        /// </param>
        /// <param name="foreground">
        /// The color to be used to draw the text.
        /// </param>
        /// <param name="bounds">
        /// The bounding rectangle of the entire drawing area.
        /// </param>
        /// <param name="xOffset">
        /// The vertical drawing offset.
        /// </param>
        /// <param name="yOffset">
        /// The horizontal drawing offset.
        /// </param>
        /// <param name="line">
        /// The visible index of the line to be drawn.
        /// </param>
        /// <seealso cref="DrawDataLine"/>
        /// <seealso cref="ShowAddress"/>
        private void DrawAddress(Graphics graphics, Font font, Brush foreground, Rectangle bounds, int xOffset, int yOffset, int line)
        {
            string format = "{0:" + ((this.upperCase) ? "X" : "x") + this.addressWidth + "}";
            string output = String.Format(format, line * this.bytesPerLine);

            // Adjust address string if current address exceeds the limit.
            if (output.Length > this.addressWidth)
            {
                output = new string('?', this.addressWidth);
            }

            int left = bounds.Left;
            int length = output.Length;
            for (int index = 0; index < length; index++)
            {
                // Draw only inside the visible area!
                if (xOffset >= left)
                {
                    // Draw current character.
                    graphics.DrawString(
                        output.Substring(index, 1),
                        font,
                        foreground,
                        xOffset,
                        yOffset,
                        this.stringFormat);
                }

                // Move to next drawing position.
                xOffset += this.cxChar;
            }
        }

        /// <summary>
        /// Draws the part of the bytes for given line onto given graphics 
        /// context.
        /// </summary>
        /// <remarks>
        /// Drawing the the byte block is never suppressed.
        /// </remarks>
        /// <param name="graphics">
        /// The graphics context to draw on.
        /// </param>
        /// <param name="font">
        /// The font to be used to draw the text.
        /// </param>
        /// <param name="foreground">
        /// The color to be used to draw the text.
        /// </param>
        /// <param name="bounds">
        /// The bounding rectangle of the entire drawing area.
        /// </param>
        /// <param name="xOffset">
        /// The vertical drawing offset.
        /// </param>
        /// <param name="yOffset">
        /// The horizontal drawing offset.
        /// </param>
        /// <param name="line">
        /// The visible index of the line to be drawn.
        /// </param>
        /// <param name="count">
        /// The total number of bytes currently available in the 
        /// data buffer.
        /// </param>
        /// <seealso cref="DrawDataLine"/>
        private void DrawByteBlock(Graphics graphics, Font font, Brush foreground, Rectangle bounds, int xOffset, int yOffset, int line, int count)
        {
            string format = "{0:" + ((this.upperCase) ? "X" : "x") + CHARS_PER_BYTE + "}";

            // Number of bytes to skip.
            int skip = this.blockWidth / CHARS_PER_BYTE;

            int left = bounds.Left;
            int right = bounds.Right - this.cxChar; // Don't draw half characters!

            // Padding is needed  
            //   if padding is enadbled
            //   && current line is last line
            //   && buffer doesn't end on bytes-per-line.
            bool padding = this.showPadding && (line + 1 == this.LineCount) && ((count % this.bytesPerLine) != 0);

            // Calculate remaining paddings.
            int remaining = (this.bytesPerLine - (count % this.bytesPerLine)) / skip;

            for (int index = line * this.bytesPerLine; index < count && index < (line + 1) * this.bytesPerLine; index += skip)
            {
                string output = "";

                // Format current byte(s) string.
                for (int offset = 0; (index + offset) < count && offset < skip; offset++)
                {
                    output += String.Format(format, this.buffer[index + offset]);
                }

                // Don't use separators at the end of line.
                if (index + skip < count && (index + skip) % this.bytesPerLine != 0)
                {
                    output += new string(' ', this.separatorWidth);
                }
                else if (padding)
                {
                    // If a rest is available at all.
                    if ((count - skip) != index)
                    {
                        // Add padding for the rest of current block.
                        output += new string('_', (skip - (count - index)) * CHARS_PER_BYTE);
                    }

                    // Add remaining paddings if available.
                    while (remaining > 0)
                    {
                        // Add separators at current position.
                        output += new string(' ', this.separatorWidth);

                        // Add padding for the rest of current block.
                        output += new string('_', skip * CHARS_PER_BYTE);

                        // Decrement remaining paddings.
                        remaining--;
                    }
                }

                int length = output.Length;
                for (int current = 0; current < length; current++)
                {
                    // Draw only inside the visible area!
                    if (xOffset >= left && xOffset <= right)
                    {
                        // Draw current character.
                        graphics.DrawString(
                            output.Substring(current, 1),
                            font,
                            foreground,
                            xOffset,
                            yOffset,
                            this.stringFormat);
                    }

                    // Move to next drawing position.
                    xOffset += this.cxChar;
                }
            }
        }

        /// <summary>
        /// Draws the text block of given line onto given graphics context.
        /// </summary>
        /// <remarks>
        /// Drawing the text block is suppressed when <i>ShowTextBlock</i> is 
        /// false.
        /// </remarks>
        /// <param name="graphics">
        /// The graphics context to draw on.
        /// </param>
        /// <param name="font">
        /// The font to be used to draw the text.
        /// </param>
        /// <param name="foreground">
        /// The color to be used to draw the text.
        /// </param>
        /// <param name="bounds">
        /// The bounding rectangle of the entire drawing area.
        /// </param>
        /// <param name="xOffset">
        /// The vertical drawing offset.
        /// </param>
        /// <param name="yOffset">
        /// The horizontal drawing offset.
        /// </param>
        /// <param name="line">
        /// The visible index of the line to be drawn.
        /// </param>
        /// <param name="count">
        /// The total number of bytes currently available in the 
        /// data buffer.
        /// </param>
        /// <seealso cref="DrawDataLine"/>
        /// <seealso cref="ShowTextBlock"/>
        private void DrawTextBlock(Graphics graphics, Font font, Brush foreground, Rectangle bounds, int xOffset, int yOffset, int line, int count)
        {
            int right = bounds.Right - this.cxChar; // Don't draw half characters!

            for (int index = line * this.bytesPerLine; index < count && index < (line + 1) * this.bytesPerLine; index++)
            {
                // Draw only inside the visible area!
                if (xOffset <= right)
                {
                    char value = (char)this.buffer[index];

                    // Adjust current value if necessary.
                    if (char.IsControl(value) || char.IsWhiteSpace(value))
                    {
                        value = '.';
                    }

                    // Draw current character.
                    graphics.DrawString(
                        value.ToString(),
                        font,
                        foreground,
                        xOffset,
                        yOffset,
                        this.stringFormat);
                }
                else
                {
                    return; // Done!
                }

                // Move to next drawing position.
                xOffset += this.cxChar;
            }
        }

        /// <summary>
        /// Redraws the hot-track at its last known position whenever 
        /// the entire control is updated.
        /// </summary>
        /// <param name="graphics">
        /// The graphics context to draw on.
        /// </param>
        /// <param name="line">
        /// The physical index of the hot-track line.
        /// </param>
        /// <param name="color">
        /// The color to be used to draw the hot-track.
        /// </param>
        /// <seealso cref="OnPaint"/>
        /// <seealso cref="HotTrack"/>
        /// <seealso cref="HotTrackColor"/>
        private void RefreshHotTrack(Graphics graphics, int line, Color color)
        {
            if (this.hotTrack && this.mouseCaptured)
            {
                using (Brush brush = new SolidBrush(color))
                {
                    Rectangle bounds = this.GetHotTrackBounds(line);
                    // Safety check, I always use belt and suspenders.
                    if (!bounds.IsEmpty)
                    {
                        graphics.FillRectangle(brush, bounds);
                    }
                }
            }
        }

        /// <summary>
        /// Redraws the given line using given color as background.
        /// </summary>
        /// <remarks>
        /// This method is called two times; once to draw the new hot-track 
        /// line and once to reset the previous hot-track line to its original 
        /// background color.
        /// <para>
        /// Furthermore, to speed-up the drawing a special trick is used. This trick 
        /// is commonly known as "off-screen painting" and takes place beside the 
        /// normal painting algorithm. This means that the drawing is done directly 
        /// and not inside member function <see cref="OnPaint">OnPaint()</see>. 
        /// </para>
        /// </remarks>
        /// <param name="line">
        /// The physical index of the hot-track line.
        /// </param>
        /// <param name="color">
        /// The color to be used to draw the hot-track.
        /// </param>
        /// <seealso cref="HotTrack"/>
        /// <seealso cref="HotTrackColor"/>
        /// <seealso cref="OnMouseMove"/>
        private void UpdateHotTrack(int line, Color color)
        {
            int topLine = this.FirstLine;

            // Don't draw hot-track if mouse exceeds last line.
            if (line + topLine >= this.LineCount) { return; }

            Rectangle bounds = this.GetHotTrackBounds(line);

            // Safety check because otherwise bitmap creation crashes.
            if (bounds.IsEmpty) { return; }

            using (Brush foreground = new SolidBrush(this.Foreground))
            using (Bitmap offBitmap = new Bitmap(bounds.Width, bounds.Height))
            using (Graphics offGraphics = Graphics.FromImage(offBitmap))
            {
                // Fill entire background.
                offGraphics.Clear(color);

                // Calculate current drawing X/Y positions.
                int xOffset = 0 - (this.cxChar * this.FirstChar);
                int yOffset = 0;

                // Draw current line data onto off-screen bitmap.
                this.DrawDataLine(
                    offGraphics,
                    this.Font,
                    foreground,
                    new Rectangle(0, 0, offBitmap.Width, offBitmap.Height),
                    xOffset,
                    yOffset,
                    line + topLine);

                // Flush off-screen bitmap onto real graphics context.
                using (Graphics graphics = this.CreateGraphics())
                {
                    bounds = this.DrawingBounds;

                    graphics.DrawImage(
                        offBitmap,
                        bounds.Left,
                        bounds.Top + line * this.cyChar);
                }
            }
        }

        /// <summary>
        /// Calculates the bounding rectangle for the given hot-track line.
        /// </summary>
        /// <param name="line">
        /// The physical index of the hot-track line.
        /// </param>
        /// <returns>
        /// The bounding rectangle for the given line.
        /// </returns>
        /// <seealso cref="RefreshHotTrack"/>
        /// <seealso cref="UpdateHotTrack"/>
        private Rectangle GetHotTrackBounds(int line)
        {
            Rectangle result = new Rectangle();
            Rectangle bounds = this.DrawingBounds;
            int length = this.LineLength - this.FirstChar; // Number of visible characters.

            result.X = bounds.Left;                     // X-offset is left of drawing bounds!
            result.Y = bounds.Top + line * this.cyChar; // Y-offset is top of drawing bounds + current line.
            result.Height = this.cyChar;                // Height is always one single line!

            // Don't draw hot-track on a half line! 
            if ((line + 1) * this.cyChar <= bounds.Height)
            {
                // Check if less visible than the bounds width.
                if (length * this.cxChar < bounds.Width)
                {
                    result.Width = length * this.cxChar;
                }
                else
                {
                    result.Width = bounds.Width;
                }
            }

            // Empty result bounds in these cases.
            if (result.Width <= 0 || result.Height <= 0)
            {
                result.X = 0;
                result.Y = 0;
                result.Width = 0;
                result.Height = 0;
            }

            return result;
        }

        #endregion // Drawing related implementations.

        #region Mouse related implementations.

        /// <summary>
        /// Calculates the line from given mouse position.
        /// </summary>
        /// <param name="point">
        /// The mouse position in client coordinates.
        /// </param>
        /// <param name="result">
        /// The visible index of the corresponding line.
        /// </param>
        /// <returns>
        /// True, if given mouse position fits on a specific 
        /// line and otherwise false.
        /// </returns>
        protected bool LineFromMousePosition(Point point, out int result)
        {
            Rectangle bounds = this.DrawingBounds;
            if (bounds.Contains(point))
            {
                result = (point.Y - bounds.Top) / this.cyChar;
                return true;
            }
            else
            {
                result = -1;
                return false;
            }
        }

        /// <summary>
        /// Checks if given mouse position is within the 
        /// address block.
        /// </summary>
        /// <param name="point">
        /// The mouse position in client coordinates.
        /// </param>
        /// <returns>
        /// True, if position is inside the address block 
        /// and otherwise false.
        /// </returns>
        /// <seealso cref="AddressBounds"/>
        protected bool PointInAddressBlock(Point point)
        {
            return this.AddressBounds.Contains(point);
        }

        /// <summary>
        /// Checks if given mouse position is within the 
        /// binary block.
        /// </summary>
        /// <param name="point">
        /// The mouse position in client coordinates.
        /// </param>
        /// <returns>
        /// True, if position is inside the binary block 
        /// and otherwise false.
        /// </returns>
        /// <seealso cref="HexBlockBounds"/>
        protected bool PointInHexBlock(Point point)
        {
            return this.HexBlockBounds.Contains(point);
        }

        /// <summary>
        /// Checks if given mouse position is within the 
        /// text block.
        /// </summary>
        /// <param name="point">
        /// The mouse position in client coordinates.
        /// </param>
        /// <returns>
        /// True, if position is inside the text block 
        /// and otherwise false.
        /// </returns>
        /// <seealso cref="TextBlockBounds"/>
        protected bool PointInTextBlock(Point point)
        {
            return this.TextBlockBounds.Contains(point);
        }

        /// <summary>
        /// Raises the <i>MouseEnter</i> event and sets the 
        /// internal state to "mouse captured".
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        protected override void OnMouseEnter(EventArgs args)
        {
            base.OnMouseEnter(args);

            // NOTE: Mouse capture is needed to handle mouse leave on task switch!
            this.mouseCaptured = true;

            // Just redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Raises the <i>MouseMove</i> event and draws the 
        /// hot-track at current line.
        /// </summary>
        /// <remarks>
        /// The hot-track is only drawn if enabled and the 
        /// control has the input focus.
        /// </remarks>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="HotTrack"/>
        /// <seealso cref="UpdateHotTrack"/>
        /// <seealso cref="LineFromMousePosition"/>
        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);

            // Draw only if hot-track is enabled. Note, checking  
            // if mouse is actually captured is not necessary.
            if (this.hotTrack && this.Focused)
            {
                int newHotTrack = -1;

                // Get current line from given mouse position.
                if (this.LineFromMousePosition(args.Location, out newHotTrack))
                {
                    // Draw only if currently highlighted line has changes.
                    if (newHotTrack != this.lastHotTrack)
                    {
                        // REMARK: Drawing and releasing current hot-track...
                        // Drawing the hot-track onto current line firstly and releasing it 
                        // for the previous line secondly appears much smoother. This also  
                        // fits if the control is full-sized and the mouse is moved not to  
                        // fast. On the other hand, two hot-track bars could be seen if the  
                        // mouse is moved faster. This effect is much better recognizable if  
                        // the control is stretched over two screens. 
                        // The disadvantage is, if both calls are used vice versa, that the  
                        // hot-track drawing doesn't appear very smooth for medium control  
                        // sizes. Conclusion, leave it as it is!

                        // Draw new hot-track as first!
                        this.UpdateHotTrack(newHotTrack, this.hotTrackColor);

                        if (this.lastHotTrack != -1)
                        {
                            // Release old hot-track secondly.
                            this.UpdateHotTrack(this.lastHotTrack, this.Background);
                        }

                        // Save last highlighted line.
                        this.lastHotTrack = newHotTrack;
                    }
                }
                else
                {
                    // Reset hot-track because of leaving drawing bounds.
                    this.lastHotTrack = -1;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Raises the <i>MouseLeave</i> event and sets the 
        /// internal state to "mouse not captured".
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        protected override void OnMouseLeave(EventArgs args)
        {
            base.OnMouseLeave(args);

            // Reset on leave!
            this.lastHotTrack = -1;

            // NOTE: Mouse capture is needed to handle mouse leave on task switch!
            this.mouseCaptured = false;

            // Force redrawing current content!
            this.Invalidate();
        }

        /// <summary>
        /// Raises the <i>MouseDown</i> event and sets the 
        /// input focus to the control.
        /// </summary>
        /// <remarks>
        /// If the right mouse button was used then the context 
        /// menu is shown.
        /// </remarks>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ProcessContextMenu"/>
        protected override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);

            // Activate control if not yet activated.
            if (!this.Focused) { this.Focus(); }

            // Process mouse button.
            if (args.Button == MouseButtons.Right)
            {
                this.ProcessContextMenu(args.Location);
            }

            // Redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Raises the <i>MouseWheel</i> event and scrolls the 
        /// visible content.
        /// </summary>
        /// <remarks>
        /// If no additional keyboard key is pressed then the content 
        /// is scrolled two lines up or down. If the <b>CTRL</b> key 
        /// is pressed then the content is scrolled four characters 
        /// left or right.
        /// </remarks>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ScrollLeft(int)"/>
        /// <seealso cref="ScrollRight(int)"/>
        /// <seealso cref="ScrollLineUp(int)"/>
        /// <seealso cref="ScrollLineDown(int)"/>
        protected override void OnMouseWheel(MouseEventArgs args)
        {
            base.OnMouseWheel(args);

            if (args.Button == MouseButtons.None)
            {
                if (Control.ModifierKeys == Keys.None)
                {
                    // Scroll down.
                    if (args.Delta < 0)
                    {
                        this.ScrollLineDown(2);
                    }
                    // Scroll up.
                    else if (args.Delta > 0)
                    {
                        this.ScrollLineUp(2);
                    }
                }
                else if (this.IsControl())
                {
                    // Scroll right.
                    if (args.Delta < 0)
                    {
                        this.ScrollRight(4);
                    }
                    // Scroll left.
                    else if (args.Delta > 0)
                    {
                        this.ScrollLeft(4);
                    }
                }
            }
        }

        #endregion // Mouse related implementations.

        #region Overwritten event handler implementations.

        /// <summary>
        /// Raises the <i>Resize</i> event and tries to show 
        /// as much as possible content.
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ScrollIntoView"/>
        protected override void OnResize(EventArgs args)
        {
            base.OnResize(args);

            // Reset to visual bounds.
            this.ScrollIntoView();
        }

        /// <summary>
        /// Raises the <i>FontChanged</i> event, recalculates 
        /// font depending variables and tries to show as much 
        /// as possible content.
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        /// <seealso cref="ScrollIntoView"/>
        /// <seealso cref="CalculateCharacterSize"/>
        protected override void OnFontChanged(EventArgs args)
        {
            base.OnFontChanged(args);

            // Recalculate current character size.
            this.CalculateCharacterSize();

            // Reset to visual bounds.
            this.ScrollIntoView();
        }

        /// <summary>
        /// Raises the <i>GotFocus</i> event and redraws the 
        /// entire control to reflect current hot-track state.
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        protected override void OnGotFocus(EventArgs args)
        {
            base.OnGotFocus(args);

            // Adjust hot-track but only if it is enabled. 
            if (this.hotTrack && this.lastHotTrack == -1)
            {
                int hotTrack = -1;
                Point position = this.PointToClient(Control.MousePosition);
                if (this.LineFromMousePosition(position, out hotTrack))
                {
                    this.lastHotTrack = hotTrack;
                }
            }

            // Just redraw current content!
            this.Invalidate();
        }

        /// <summary>
        /// Raises the <i>LostFocus</i> event and redraws the 
        /// entire control to reflect current hot-track state.
        /// </summary>
        /// <param name="args">
        /// A value containing the event data.
        /// </param>
        protected override void OnLostFocus(EventArgs args)
        {
            base.OnLostFocus(args);

            // Reset on leave!
            this.lastHotTrack = -1;

            // Just redraw current content!
            this.Invalidate();
        }

        #endregion // Overwritten event handler implementations.

        #region Other private member function implementations.

        /// <summary>
        /// Recalculates font specific values and adjusts internal 
        /// member variables accordingly.
        /// </summary>
        /// <seealso cref="cxChar"/>
        /// <seealso cref="cyChar"/>
        /// <seealso cref="stringFormat"/>
        private void CalculateCharacterSize()
        {
            if (this.stringFormat == null)
            {
                this.stringFormat = new StringFormat(StringFormat.GenericTypographic);
                this.stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            }

            using (Graphics graphics = this.CreateGraphics())
            {
                SizeF size = new SizeF();

                // Take default character to calculate size!
                size = graphics.MeasureString(
                    "#", this.Font, 100, this.stringFormat);

                // Adjust calculated size.
                size = new SizeF(
                    (float)Math.Ceiling(size.Width),
                    (float)Math.Ceiling(size.Height));

                // Safe character width and line height for faster access.
                this.cxChar = Math.Max(size.ToSize().Width, 1);
                this.cyChar = Math.Max(size.ToSize().Height, 1);
            }
        }

        #endregion // Other private member function implementations.
    }
}

// EOF