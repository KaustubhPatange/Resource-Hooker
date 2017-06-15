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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace plexdata.$safeprojectname$
{
    public static class ImageSplitter
    {
        public static int GetIconBitDepth(Icon icon)
        {
            int result = 0;

            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                // Get a stream from given icon!
                icon.Save(stream);
                data = stream.ToArray();

                // Try finding the "right" bit-count value.
                ushort count = BitConverter.ToUInt16(data, 4); // NEWHEADER.ResCount
                if (count != 1)
                {
                    result = BitConverter.ToUInt16(data, 12); // RESDIR.BitCount
                }
                else
                {
                    result = BitConverter.ToUInt16(data, 36); // BITMAPINFOHEADER.biBitCount
                }
            }
            return result;
        }

        public static int GetCursorBitDepth(Icon cursor)
        {
            if (cursor == null)
            {
                throw new ArgumentNullException("cursor");
            }

            return ImageSplitter.GetIconBitDepth(cursor);
        }

        public static Point GetCursorHotSpot(Icon cursor)
        {
            int xHotSpot = 0; // X-coordinate, in pixels, of cursor's hot spot. 
            int yHotSpot = 0; // Y-coordinate, in pixels, of cursor's hot spot. 

            if (cursor == null)
            {
                throw new ArgumentNullException("cursor");
            }

            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                // Get a stream from given cursor (icon)!
                cursor.Save(stream);
                data = stream.ToArray();

                // Assume that given icon is a cursor.
                xHotSpot = BitConverter.ToUInt16(data, 10); // RESDIR.Planes
                yHotSpot = BitConverter.ToUInt16(data, 12); // RESDIR.BitCount
            }
            return new Point(xHotSpot, yHotSpot);
        }

        /// <summary>
        /// Splits given stream formatted as an icon file into its 
        /// single icons.
        /// </summary>
        /// <param name="input">A memory stream containing an icon in 
        /// icon file format.</param>
        /// <returns>A list of sub-icons in this icon file format.</returns>
        public static Icon[] SplitIcon(MemoryStream input)
        {
            // This function is based on code written by Tsuda Kageyu. 
            // Many thanks to him! For more information see code sample
            // under http://www.codeproject.com/KB/cs/IconExtractor.aspx.

            const int NEWHEADER = 6;    // sizeof(NEWHEADER) 
            const int ICONRESDIR = 16;  // sizeof(ICONRESDIR)

            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.Length == 0)
            {
                throw new ArgumentException("Stream with zero length not allowed!", "input");
            }

            byte[] buffer = input.ToArray();
            List<Icon> icons = new List<Icon>();
            int count = BitConverter.ToInt16(buffer, 4);
            for (int index = 0; index < count; index++)
            {
                using (MemoryStream output = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(output))
                {
                    writer.Write(buffer, 0, NEWHEADER - 2);
                    writer.Write((short)1);

                    writer.Write(buffer, NEWHEADER + ICONRESDIR * index, ICONRESDIR - 4);
                    writer.Write(NEWHEADER + ICONRESDIR);

                    int size = BitConverter.ToInt32(buffer, NEWHEADER + ICONRESDIR * index + 8);
                    int offset = BitConverter.ToInt32(buffer, NEWHEADER + ICONRESDIR * index + 12);
                    writer.Write(buffer, offset, size);

                    output.Seek(0, SeekOrigin.Begin);
                    icons.Add(new Icon(output));
                }
            }
            return icons.ToArray();
        }

        /// <summary>
        /// Splits given stream formatted as a cursor file but with a type 
        /// set to icon into its single icon representations. Ohterwise, 
        /// displaying would not be possible!
        /// </summary>
        /// <param name="input">A memory stream containing an cursor in 
        /// cursor file format but with type of an icon.</param>
        /// <returns>A list of sub-icons in this cursor file format.</returns>
        public static Icon[] SplitCursor(MemoryStream input)
        {
            const int NEWHEADER = 6;    // sizeof(NEWHEADER) 
            const int ICONRESDIR = 16;  // sizeof(ICONRESDIR)

            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.Length == 0)
            {
                throw new ArgumentException("Stream with zero length not allowed!", "input");
            }

            List<Icon> cursors = new List<Icon>();
            byte[] source = input.ToArray();

            using (BinaryReader reader = new BinaryReader(input))
            {
                // Read NEWHEADER data for a later use (remove them from stream).
                ushort idReserved = reader.ReadUInt16();
                ushort idType = reader.ReadUInt16();
                ushort idCount = reader.ReadUInt16();

                if (idType != 2) // Check cursor type!
                {
                    throw new ArgumentException("Given stream does not contain the data of a cursor file!", "input");
                }

                for (int index = 0; index < idCount; index++)
                {
                    using (MemoryStream output = new MemoryStream())
                    using (BinaryWriter writer = new BinaryWriter(output))
                    {
                        // Read CURSORDIR data as ICONRESDIR data (remove them from stream).
                        byte wWidth = reader.ReadByte();
                        byte wHeight = reader.ReadByte();
                        byte bColorCount = reader.ReadByte();
                        byte bReserved = reader.ReadByte();
                        ushort wPlanes = reader.ReadUInt16();       // Cursor's wXHotSpot!
                        ushort wBitCount = reader.ReadUInt16();     // Cursor's wYHotSpot!
                        uint dwBytesInRes = reader.ReadUInt32();    // Number of image data bytes.
                        ushort wImageOffset = reader.ReadUInt16();  // Offset within source buffer.
                        ushort wPadding = reader.ReadUInt16();

                        // Write NEWHEADER data for new icon!
                        writer.Write(idReserved);
                        // HACK: idType: Set type of this cursor to type of an icon!
                        writer.Write((ushort)1);
                        // idCount: This new icon contains only one image!
                        writer.Write((ushort)1);

                        // Write ICONRESDIR data for new icon!
                        writer.Write((byte)wWidth);
                        writer.Write((byte)wHeight);
                        writer.Write((byte)bColorCount);
                        writer.Write((byte)bReserved);
                        writer.Write((ushort)wPlanes);
                        writer.Write((ushort)wBitCount);
                        writer.Write((uint)dwBytesInRes);
                        writer.Write((ushort)(NEWHEADER + ICONRESDIR)); // New data offset!
                        writer.Write((ushort)wPadding);

                        // Now copy the image data from given source buffer and 
                        // put them immediatly behind the new icon's buffer! 
                        writer.Write(source, (int)wImageOffset, (int)dwBytesInRes);

                        // Now create current sub-icon.
                        output.Seek(0, SeekOrigin.Begin);
                        cursors.Add(new Icon(output));
                    }
                }
            }
            return cursors.ToArray();
        }

        public static Icon CursorToIcon(MemoryStream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.Length == 0)
            {
                throw new ArgumentException("Stream with zero length not allowed!", "input");
            }

            Icon icon = null;
            byte[] source = input.ToArray();

            using (BinaryReader reader = new BinaryReader(input))
            {
                // Read NEWHEADER data for a later use (remove them from stream).
                ushort idReserved = reader.ReadUInt16();
                ushort idType = reader.ReadUInt16();
                ushort idCount = reader.ReadUInt16();

                if (idType != 2) // Check cursor type!
                {
                    throw new ArgumentException("Given stream does not contain the data of a cursor file!", "input");
                }

                using (MemoryStream output = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(output))
                {
                    // HACK: idType: Set type of this cursor to type of an icon!
                    idType = 1;

                    // Write NEWHEADER data for new icon!
                    writer.Write(idReserved);
                    writer.Write(idType);

                    // Now copy the rest of available data from given 
                    // source buffer into the result buffer! 
                    writer.Write(source, 4, source.Length - 4);

                    // Now create current sub-icon.
                    output.Seek(0, SeekOrigin.Begin);
                    icon = new Icon(output);
                }

            }
            return icon;
        }
    }
}

// EOF