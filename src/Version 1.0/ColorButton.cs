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
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace plexdata.Controls
{
    public class ColorButton : Button
    {
        private Color color;
        private bool border;

        #region Static conversion function section.

        public static string ToHEXString(Color color)
        {
            return ColorButton.ToHEXString(color, false);
        }

        public static string ToHEXString(Color color, bool alpha)
        {
            if (alpha)
            {
                return String.Format("{0:x2}{1:x2}{2:x2}{3:x2}", color.A, color.R, color.G, color.B);
            }
            else
            {
                return String.Format("{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
            }
        }

        public static string ToHEXString(int color)
        {
            return ColorButton.ToHEXString(Color.FromArgb(color), false);
        }

        public static string ToHEXString(int color, bool alpha)
        {
            return ColorButton.ToHEXString(Color.FromArgb(color), alpha);
        }

        public static Color FromHEXString(string color)
        {
            if (!String.IsNullOrEmpty(color))
            {
                color = color.Trim();

                // Take care about alpha blending value!
                if (color.Length == 6)
                {
                    color = "ff" + color; // Use opaqueness as alpha blending!
                }

                if (color.Length >= 8)
                {
                    byte[] buffer = Encoding.Default.GetBytes(color);
                    int a = byte.Parse(Encoding.Default.GetString(buffer, 0, 2), NumberStyles.AllowHexSpecifier);
                    int r = byte.Parse(Encoding.Default.GetString(buffer, 2, 2), NumberStyles.AllowHexSpecifier);
                    int g = byte.Parse(Encoding.Default.GetString(buffer, 4, 2), NumberStyles.AllowHexSpecifier);
                    int b = byte.Parse(Encoding.Default.GetString(buffer, 6, 2), NumberStyles.AllowHexSpecifier);
                    return Color.FromArgb(a, r, g, b);
                }
                else
                {
                    throw new ArgumentException("Invalid color HEX string length!");
                }
            }
            else
            {
                throw new ArgumentException("Empty string parameter not allowed!");
            }
        }

        #endregion //Static conversion function section.

        public ColorButton()
            : base()
        {
            base.Padding = new Padding(5);
            this.Color = Color.White;
            this.Border = true;
        }

        public ColorButton(Color color)
            : this()
        {
            this.color = color;
        }

        public ColorButton(Color color, bool border)
            : this(color)
        {
            this.border = border;
        }

        public Color Color
        {
            get { return this.color; }
            set { this.color = value; base.Invalidate(); }
        }

        public bool Border
        {
            get { return this.border; }
            set { this.border = value; base.Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);

            if (!this.color.IsEmpty)
            {
                Rectangle rect = this.ClientRectangle;
                rect.X += this.Padding.Left;
                rect.Y += this.Padding.Top;
                rect.Width -= (this.Padding.Left + this.Padding.Right + 1);
                rect.Height -= (this.Padding.Top + this.Padding.Bottom + 1);

                using (SolidBrush brush = new SolidBrush(this.color))
                {
                    args.Graphics.FillRectangle(brush, rect);
                }

                if (this.border)
                {
                    args.Graphics.DrawRectangle(Pens.Black, rect);
                }

                if (!String.IsNullOrEmpty(base.Text))
                {
                    using (SolidBrush brush = new SolidBrush(this.Invert(this.color)))
                    {
                        StringFormat format = new StringFormat();
                        format.LineAlignment = StringAlignment.Center;
                        format.Alignment = StringAlignment.Center;
                        rect.Y += 1;
                        args.Graphics.DrawString(base.Text, base.Font, brush, rect, format);
                    }
                }
            }
        }

        private Color Invert(Color color)
        {
            return Color.FromArgb((byte)~color.R, (byte)~color.G, (byte)~color.B);
        }
    }
}
