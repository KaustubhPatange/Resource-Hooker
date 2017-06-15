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
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace plexdata.Utilities.Resources
{
  
    public class ResourcesLocator : IDisposable
    {
      
        // Effective Resource Structure
        // ----------------------------
        //
        // Each resource file starts with NEWHEADER structure immediately 
        // followed by at least one RESDIR structure. A RESDIR structure 
        // includes depending on its type either an ICONRESDIR sub-structure 
        // or a CURSORDIR sub-structure. Therefore, each file uses the 
        // following format: NEWHEADER, RESDIR[1..n], DATA[1..n]
        //
        // Furthermore, each cursor data part includes an additional header 
        // called LOCALHEADER which defines the hot spot coordinates!
        //
        // The DATA part of each type begins with another header called 
        // BITMAPINFOHEADER that defines the image depending information.
        //
        // Effective File Structure
        // ------------------------
        //
        // For Icon files the structure to be used is the same as used for 
        // resources! 
        //
        // But for Cursor files the structure has to be converted a bit 
        // before they can be saved into a file. See following list of
        // changes!
        //
        // * Width of CURSORDIR is mapped to Width ICONRESDIR.
        // * Height of CURSORDIR is divided by 2 and then mapped to Height ICONRESDIR.
        // * ColorCount and Reserved of ICONRESDIR are set to 0.
        // * Planes of RESDIR is set to xHotSpot of LOCALHEADER.
        // * BitCount of RESDIR is set to yHotSpot of LOCALHEADER.
        // * The LOCALHEADER structure must be removed from a cursor's data part!
        //
        // Structure Definitions
        // ---------------------
        //
        // See: http://msdn.microsoft.com/en-us/library/ms648023(v=vs.85).aspx
        // struct NEWHEADER { 
        //     WORD Reserved;  // Must be zero. 
        //     WORD ResType;   // Resource type: RES_ICON (=1) or RES_CURSOR(=2)
        //     WORD ResCount;  // Number of icons or cursors in this group. 
        // }
        //
        // See: MSDN Loacal help of Visual Studio!
        // See: http://msdn.microsoft.com/en-us/library/ms648026(v=vs.85).aspx
        // typedef struct RESDIR { 
        //     union { 
        //         ICONRESDIR Icon; 
        //         CURSORDIR  Cursor; 
        //     } ResInfo; 
        //     WORD  Planes;       // Color planes in icon or cursor bitmap. 
        //     WORD  BitCount;     // Bits per pixel in icon or cursor bitmap. 
        //     DWORD BytesInRes;   // Size of the resource in bytes. 
        //     WORD  IconCursorId; // Icon's or cursor's unique ordinal identifier. 
        //     WORD  Padding;      // An undocumented padding but needed to fix 32-bit alignment.
        // }
        //
        // See: http://msdn.microsoft.com/en-us/library/ms648016(v=vs.85).aspx
        // struct ICONRESDIR { 
        //     BYTE Width;         // Width of the icon in pixels (16, 32, 64, ...).
        //     BYTE Height;        // Height of the icon in pixels (16, 32, 64, ...).
        //     BYTE ColorCount;    // Number of colors in the icon (2, 8, 16, ...). 
        //     BYTE Reserved;      // Must be set to same value as for Reserved. 
        // }
        //
        // See: http://msdn.microsoft.com/en-us/library/ms648011(v=vs.85).aspx
        // struct CURSORDIR { 
        //     WORD Width;         // Width of the icon in pixels (16, 32, 64, ...).
        //     WORD Height;        // Height of the icon in pixels (16, 32, 64, ...).
        // }
        //
        // See: http://msdn.microsoft.com/en-us/library/ms648017(v=vs.85).aspx
        // typedef LOCALHEADER { 
        //     WORD xHotSpot;     // X-coordinate, in pixels, of cursor's hot spot. 
        //     WORD yHotSpot;     // Y-coordinate, in pixels, of cursor's hot spot. 
        // }
        //
        // See: http://msdn.microsoft.com/en-us/library/dd183376(v=vs.85).aspx
        // struct BITMAPINFOHEADER {
        //     DWORD biSize;             // Number of bytes required by the structure (=40).
        //     LONG  biWidth;            // The width of the bitmap, in pixels.
        //     LONG  biHeight;           // The height of the bitmap, in pixels. 
        //     WORD  biPlanes;           // Number of planes for the target device (must be 1).
        //     WORD  biBitCount;         // Number of bits-per-pixel (0, 1, 4, 8, 16, ...). 
        //     DWORD biCompression;      // Type of compression.
        //     DWORD biSizeImage;        // Image size in bytes.
        //     LONG  biXPelsPerMeter;    // Horizontal resolution in pixels-per-meter.
        //     LONG  biYPelsPerMeter;    // Vertical resolution in pixels-per-meter.
        //     DWORD biClrUsed;          // Number of color indexes in the color table.
        //     DWORD biClrImportant;     // Number of color indexes that are required.
        // }      

        private IntPtr hModule = IntPtr.Zero;
        private string filename = null;
        private Dictionary<ResourceType, List<ResourceValue>> cache = null;

        #region Class construction.

        public ResourcesLocator()
            : base()
        {
            this.cache = new Dictionary<ResourceType, List<ResourceValue>>();
        }

        public ResourcesLocator(string filename)
            : this()
        {
            if (this.Load(filename))
            {
                this.Enumerate();
            }
        }

        ~ResourcesLocator()
        {
            this.Dispose();
        }

        #endregion // Class construction.

        #region Class properties.

        public string Filename
        {
            get { return this.filename; }
        }

        #endregion // Class properties.

        #region Public implementation.

        public bool Load(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                return false;
            }

            try
            {
                this.filename = filename;

                if (this.hModule != IntPtr.Zero)
                {
                    this.Free();
                }

                this.hModule = ResourcesLocator.LoadLibraryEx(this.filename, IntPtr.Zero, ResourcesLocator.LOAD_LIBRARY_AS_DATAFILE);

                if (this.hModule != IntPtr.Zero)
                {
                    return true;
                }
                else
                {
                    Debug.WriteLine("ResourcesLocator::Load() LastError: " + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("ResourcesLocator::Load() " + exception.ToString());
                return false;
            }
        }

        public bool Enumerate()
        {
            try
            {
                if (this.hModule != IntPtr.Zero)
                {
                    this.cache.Clear();

                    if (ResourcesLocator.EnumResourcesByType(this.hModule, new EnumResourceTypesDelegate(EnumResourceTypes), this))
                    {
                        return (this.cache.Keys.Count > 0);
                    }
                    else
                    {
                        Debug.WriteLine("ResourcesLocator::Enumerate() LastError: " + Marshal.GetLastWin32Error());
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("ResourcesLocator::Enumerate() " + exception.ToString());
                return false;
            }
        }

        public void Free()
        {
            if (this.hModule != IntPtr.Zero)
            {
                ResourcesLocator.FreeLibrary(this.hModule);
                this.hModule = IntPtr.Zero;
            }
        }

        public ResourceType[] GetTypes()
        {
            ResourceType[] result = new ResourceType[this.cache.Keys.Count];
            this.cache.Keys.CopyTo(result, 0);
            return result;
        }

        public ResourceValue[] GetValues(ResourceType type)
        {
            ResourceValue[] result = new ResourceValue[0];
            if (this.cache.ContainsKey(type))
            {
                result = this.cache[type].ToArray();
            }
            return result;
        }

        public MemoryStream GetResource(ResourceType type, ResourceValue name)
        {
            MemoryStream result = null;

            if (this.hModule != IntPtr.Zero)
            {
                if (type.Type == ResourceTypes.RT_BITMAP)
                {
                    result = this.GetBitmapResource(name);
                }
                else if (type.Type == ResourceTypes.RT_GROUP_ICON || type.Type == ResourceTypes.RT_ICON)
                {
                    // Resource icons consisting of an icon group on this calling level!
                    result = this.GetIconResource(name);
                }
                else if (type.Type == ResourceTypes.RT_GROUP_CURSOR || type.Type == ResourceTypes.RT_CURSOR)
                {
                    // Resource icons consisting of an icon group on this calling level!
                    result = this.GetCursorResource(name);
                }
                else if (type.Type == ResourceTypes.RT_STRING)
                {
                    result = this.GetStringResource(name);
                }
                else if (type.Type == ResourceTypes.RT_VERSION)
                {
                    // Simply return as binary data.
                    result = new MemoryStream(this.GetBinaryResource(type, name));
                }
                else
                {
                    result = new MemoryStream(this.GetBinaryResource(type, name));
                }
            }
            return result;
        }

        #endregion // Public implementation.

        #region Private implementation.

        private MemoryStream GetBitmapResource(ResourceValue name)
        {
            // Bitmap resources are saved either as DIB or DDB bitmaps and
            // therefore it is quite ok to use Win-API function LoadBitmap().
            // See also http://support.microsoft.com/kb/67883/de.
            // TODO: Use GetBinaryResource() instead of LoadBitmap().
            //       Because the bitmap returned by function LoadBitmap() seems
            //       to be loaded with wrong colors.

            // REMARK: Using LoadImage() instead produces almost the same result!
            //         And therefore it makes no sense to use this function.
            MemoryStream result = new MemoryStream();
            IntPtr hBitmap = ResourcesLocator.LoadBitmap(this.hModule, name.Value);
            if (hBitmap != IntPtr.Zero)
            {
                Bitmap bitmap = Image.FromHbitmap(hBitmap);
                if (bitmap != null)
                {
                    bitmap.Save(result, ImageFormat.Bmp);
                }
                ResourcesLocator.DeleteObject(hBitmap);
                result.Position = 0;
            }
            return result;
        }

        private MemoryStream GetIconResource(ResourceValue name)
        {
            // This function is based on code written by Tsuda Kageyu. 
            // Many thanks to him! For more information see code sample
            // under http://www.codeproject.com/KB/cs/IconExtractor.aspx.

            const int NEWHEADER = 6;    // sizeof(NEWHEADER) 
            const int ICONRESDIR = 16;  // sizeof(ICONRESDIR)
            const int GROUPRESDIR = 14; // sizeof(GROUPRESDIR)

            ResourceType group = new ResourceType(ResourceTypes.RT_GROUP_ICON);
            ResourceType entry = new ResourceType(ResourceTypes.RT_ICON);
            MemoryStream result = new MemoryStream();

            byte[] groupBuffer = this.GetBinaryResource(group, name);
            if (groupBuffer.Length > 0)
            {
                BinaryWriter writer = new BinaryWriter(result);
                int count = BitConverter.ToUInt16(groupBuffer, 4);
                int offset = NEWHEADER + ICONRESDIR * count;

                writer.Write(groupBuffer, 0, NEWHEADER);

                for (int index = 0; index < count; index++)
                {
                    writer.BaseStream.Seek(NEWHEADER + ICONRESDIR * index, SeekOrigin.Begin);
                    writer.Write(groupBuffer, NEWHEADER + GROUPRESDIR * index, ICONRESDIR - 4);
                    writer.Write(offset);

                    IntPtr iconID = (IntPtr)BitConverter.ToUInt16(groupBuffer, NEWHEADER + GROUPRESDIR * index + 12);
                    byte[] imageBuffer = this.GetBinaryResource(entry, new ResourceValue(iconID));

                    writer.BaseStream.Seek(offset, SeekOrigin.Begin);
                    writer.Write(imageBuffer, 0, imageBuffer.Length);

                    offset += imageBuffer.Length;
                }
                result.Position = 0;
            }
            return result;
        }

        private MemoryStream GetCursorResource(ResourceValue name)
        {
            // Implementing this function made a lot pain in the ass!
            // This is because of that class Cursor does not eat streams 
            // with data comming from a resource! This class only likes 
            // such streams in the cursour file format (file type *.cur). 
            // Not a problem?!? Indeed, it is a problem, because resource 
            // format for cursors differs from cursor file format! Further 
            // but not complete information about cursors can be found 
            // under http://msdn.microsoft.com/en-us/library/ms646975.aspx
            // Conclusion: It was a very long way!

            const int NEWHEADER = 6;    // sizeof(NEWHEADER) 
            const int CURSORDIR = 16;   // sizeof(CURSORDIR)
            const int LOCALHEADER = 4;  // sizeof(LOCALHEADER)

            ResourceType group = new ResourceType(ResourceTypes.RT_GROUP_CURSOR);
            ResourceType entry = new ResourceType(ResourceTypes.RT_CURSOR);
            MemoryStream result = new MemoryStream();
            MemoryStream images = new MemoryStream(); // Temporary buffer to hold image data.

            byte[] groupBuffer = this.GetBinaryResource(group, name);
            if (groupBuffer.Length > 0)
            {
                BinaryWriter groupWriter = new BinaryWriter(result);
                BinaryReader groupReader = new BinaryReader(new MemoryStream(groupBuffer));

                // Read NEWHEADER data.
                ushort idReserved = groupReader.ReadUInt16();
                ushort idType = groupReader.ReadUInt16();
                ushort idCount = groupReader.ReadUInt16();

                // Data offset within cursor file.
                ushort offset = (ushort)(NEWHEADER + CURSORDIR * idCount);

                // Write NEWHEADER data.
                groupWriter.Write(idReserved);   // Must be 0.
                groupWriter.Write(idType);       // Must be 2 for cursors!
                groupWriter.Write(idCount);      // Number of cursor "images".

                for (int index = 0; index < idCount; index++)
                {
                    // Read CURSORDIR data.
                    ushort wWidth = groupReader.ReadUInt16();
                    ushort wHeight = groupReader.ReadUInt16();
                    ushort wPlanes = groupReader.ReadUInt16();
                    ushort wBitCount = groupReader.ReadUInt16();
                    uint dwBytesInRes = groupReader.ReadUInt32();
                    ushort wImageOffset = groupReader.ReadUInt16();

                    byte[] imageBuffer = this.GetBinaryResource(entry, new ResourceValue(new IntPtr(wImageOffset)));
                    if (imageBuffer.Length > 0)
                    {
                        BinaryReader imageReader = new BinaryReader(new MemoryStream(imageBuffer));

                        // Read LOCALHEADER data.
                        ushort wXHotSpot = imageReader.ReadUInt16();
                        ushort wYHotSpot = imageReader.ReadUInt16();

                        // Adapt resource data to fit cursor file data needs!
                        byte bWidth = (byte)wWidth;
                        byte bHeight = (byte)(wHeight / 2); // Recalc if odd!
                        byte bColorCount = 0;
                        byte bReserved = 0;
                        ushort wPadding = 0;
                        // Planes and BitCount are used as HotSpot information instead!
                        wPlanes = wXHotSpot;
                        wBitCount = wYHotSpot;

                        // HotSpot information have to be removed from image buffer!
                        dwBytesInRes = dwBytesInRes - LOCALHEADER; // 4 bytes of HotSpot!

                        // Write new CURSORDIR data.
                        groupWriter.Write(bWidth);
                        groupWriter.Write(bHeight);
                        groupWriter.Write(bColorCount);
                        groupWriter.Write(bReserved);
                        groupWriter.Write(wPlanes);
                        groupWriter.Write(wBitCount);
                        groupWriter.Write(dwBytesInRes);
                        groupWriter.Write(offset);
                        groupWriter.Write(wPadding);

                        // Recalculate next offset.
                        offset += (ushort)(dwBytesInRes);

                        // Append current image data at the temporary buffer.
                        images.Write(imageBuffer, LOCALHEADER, (int)dwBytesInRes);
                    }
                }
                // Now append all image data at once at the result buffer.
                groupWriter.Write(images.ToArray(), 0, (int)images.Length);

                // Return to the result stream's start position.
                result.Position = 0;
            }
            return result;
        }

        private MemoryStream GetStringResource(ResourceValue name)
        {
            MemoryStream result = new MemoryStream();
            ResourceType type = new ResourceType(ResourceTypes.RT_STRING);

            byte[] buffer = this.GetBinaryResource(type, name);
            if (buffer.Length > 0)
            {
                BinaryWriter writer = new BinaryWriter(result);
                char[] helper = Encoding.Unicode.GetString(buffer).ToCharArray();
                int offset = 0;
                for (int index = 0; index < 16; index++)
                {
                    if (helper[offset] != 0)
                    {

                        int length = helper[offset];  // GetWord()  // String length in characters 
                        offset++;

                        if (length > 0)
                        {
                            int stringID = (name.Value.ToInt32() - 1) * 16 + index;

                            writer.Write(stringID);
                            writer.Write(length);

                            for (int inner = offset; inner < offset + length; inner++)
                            {
                                writer.Write(helper[inner]);
                            }
                        }
                        offset += length;
                    }
                    else
                    {
                        offset++;
                    }
                }
                result.Position = 0;
            }
            return result;
        }

        private byte[] GetBinaryResource(ResourceType type, ResourceValue name)
        {
            // Writing a general function to handle resource loading based on  
            // functions FindResource(), LoadResource() etc. is not really  
            // easy because of the returned handle HGLOBAL is a pointer to  
            // resource depending structures that contain additional resource 
            // type depending information and not the resource information  
            // directly! For example see loading an icon resource under  
            // http://www.codeproject.com/Tips/62005/Extracting-A-Particular-Icon-From-an-ICO-Resource.aspx 
            // But the code below should work for custom resources like PNG, 
            // XML etc.

            byte[] result = new byte[0];
            IntPtr hInfo = ResourcesLocator.FindResource(this.hModule, name.Value, type.Value);
            if (hInfo != IntPtr.Zero)
            {
                int size = ResourcesLocator.SizeofResource(this.hModule, hInfo);
                IntPtr hResource = ResourcesLocator.LoadResource(this.hModule, hInfo);
                if (hResource != IntPtr.Zero)
                {
                    result = new byte[size];
                    IntPtr hLock = ResourcesLocator.LockResource(hResource);
                    if (hLock != IntPtr.Zero)
                    {
                        Marshal.Copy(hLock, result, 0, size);
                    }
                }
            }
            return result;
        }

        #endregion // Private implementation.

        #region Resources enumerating delegate implementation.

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
        private delegate bool EnumResourceTypesDelegate(IntPtr hModule, IntPtr type, ResourcesLocator parameter);
        private static bool EnumResourceTypes(IntPtr hModule, IntPtr type, ResourcesLocator parameter)
        {
            try
            {
                if (ResourcesLocator.EnumResourceNamesByID(parameter.hModule, type, new EnumResourceNamesDelegate(EnumResourceNames), parameter))
                {
                    return true;
                }
                else
                {
                    Debug.WriteLine("ResourcesLocator::EnumResourceTypes() LastError: " + Marshal.GetLastWin32Error());
                    return false;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("ResourcesLocator::EnumResourceTypes() " + exception.ToString());
                return false;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
        private delegate bool EnumResourceNamesDelegate(IntPtr hModule, IntPtr type, IntPtr value, ResourcesLocator parameter);
        private static bool EnumResourceNames(IntPtr hModule, IntPtr type, IntPtr value, ResourcesLocator parameter)
        {
            try
            {
                ResourceType helper = new ResourceType(type);
                if (!parameter.cache.ContainsKey(helper))
                {
                    parameter.cache.Add(helper, new List<ResourceValue>());
                }
                parameter.cache[helper].Add(new ResourceValue(value));

                // TODO: Handle language depending resources. See example "Creating a Resource List" in
                // ms-help://MS.VSCC.v90/MS.MSDNQTR.v90.en/winui/winui/windowsuserinterface/resources/introductiontoresources/usingresources.htm

                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("ResourcesLocator::EnumResourceNames() " + exception.ToString());
                return false;
            }
        }

        #endregion // Resources enumerating delegate implementation.

        #region Resources enumeration Win32 imports.

        private const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string filename, IntPtr reserved, int flags);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "EnumResourceTypes", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool EnumResourcesByType(IntPtr hModule, EnumResourceTypesDelegate callback, ResourcesLocator parameter);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "EnumResourceNames", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool EnumResourceNamesByID(IntPtr hModule, IntPtr type, EnumResourceNamesDelegate callback, ResourcesLocator parameter);

        #endregion // Resources enumeration Win32 imports.

        #region Resources loading Win32 imports.

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr FindResource(IntPtr hModule, IntPtr name, IntPtr type);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr type, IntPtr name, ushort language);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr info);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr LockResource(IntPtr hResource);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern int SizeofResource(IntPtr hModule, IntPtr info);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern IntPtr LoadBitmap(IntPtr hModule, IntPtr name);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        #endregion // Resources loading Win32 imports.

        #region IDisposable implementation.

        public void Dispose()
        {
            this.Free();
        }

        #endregion // IDisposable implementation.
    }

    public enum ResourceTypes
    {
        // See header file "winuser.h".
        RT_CURSOR = 1,
        RT_BITMAP = 2,
        RT_ICON = 3,
        RT_MENU = 4,
        RT_DIALOG = 5,
        RT_STRING = 6,
        RT_FONTDIR = 7,
        RT_FONT = 8,
        RT_ACCELERATOR = 9,
        RT_RCDATA = 10,
        RT_MESSAGETABLE = 11,
        RT_GROUP_CURSOR = ResourceTypes.RT_CURSOR + 11, // 11 == DIFFERENCE!
        RT_GROUP_ICON = ResourceTypes.RT_ICON + 11, // 11 == DIFFERENCE!
        RT_VERSION = 16,
        RT_DLGINCLUDE = 17,
        RT_PLUGPLAY = 19,
        RT_VXD = 20,
        RT_ANICURSOR = 21,
        RT_ANIICON = 22,
        RT_HTML = 23,
        RT_MANIFEST = 24,
        // Self-defines!
        RT_OTHER = ushort.MinValue,
        RT_UNKNOWN = ushort.MaxValue,
    }

    public class ResourceValue
    {
        private IntPtr buffer;
        protected IntPtr value;
        protected string name;

        #region Class construction.

        public ResourceValue()
            : base()
        {
            this.buffer = IntPtr.Zero;
            this.value = IntPtr.Zero;
            this.name = "";
        }

        public ResourceValue(IntPtr value)
            : base()
        {
            this.buffer = IntPtr.Zero;
            this.value = value;
            this.name = this.AsResourceName(this.value);
        }

        public ResourceValue(int value)
            : base()
        {
            this.buffer = IntPtr.Zero;
            this.value = new IntPtr(value);
            this.name = this.AsResourceName(this.value);
        }

        public ResourceValue(string value)
            : base()
        {
            this.buffer = IntPtr.Zero;
            this.value = this.AsResourceValue(value);
            this.name = this.AsResourceName(this.value);
        }

        ~ResourceValue()
        {
            this.FreeBuffer();
        }

        #endregion // Class construction.

        #region Class properties.

        public IntPtr Value
        {
            get
            {
                if (this.IsIntResource(this.value))
                {
                    return this.value;
                }
                else
                {
                    return this.AsResourceValue(this.name);
                }
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        #endregion // Class properties.

        #region Overridden implementation.

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other is ResourceValue)
            {
                return ((other as ResourceValue).name == this.name);
            }
            else
            {
                return base.Equals(other);
            }
        }

        public override string ToString()
        {
            return this.IsIntResource(this.value) ? this.name : "\"" + this.name + "\"";
        }

        #endregion // Overridden implementation.

        #region Protected implementation.

        protected bool IsIntResource(IntPtr value)
        {
            try
            {
                return (System.Convert.ToUInt32(value.ToInt32()) <= ushort.MaxValue);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("ResourceValue::IsIntResource() " + exception.ToString());
            }
            return false;
        }

        protected string AsResourceName(IntPtr value)
        {
            if (this.IsIntResource(value))
            {
                return value.ToString();
            }
            else
            {
                return Marshal.PtrToStringAuto(value);
            }
        }

        protected IntPtr AsResourceValue(string value)
        {
            try
            {
                this.FreeBuffer();
                this.buffer = Marshal.StringToHGlobalAuto(value);
                return this.buffer;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("ResourceValue::AsResourceValue() " + exception.ToString());
                return IntPtr.Zero;
            }
        }

        #endregion // Protected implementation.

        #region Private implementation.

        private void FreeBuffer()
        {
            if (this.buffer != IntPtr.Zero)
            {
                try { Marshal.FreeHGlobal(this.buffer); }
                catch { }
                this.buffer = IntPtr.Zero;
            }
        }

        #endregion // Private implementation.
    }

    public class ResourceType : ResourceValue
    {
        private ResourceTypes type;

        #region Class construction.

        public ResourceType()
            : base()
        {
            this.type = ResourceTypes.RT_UNKNOWN;
        }

        public ResourceType(IntPtr value)
            : base(value)
        {
            this.type = this.Convert(value);
            if (this.IsIntResource(value))
            {
                this.name = this.type.ToString();
            }
        }

        public ResourceType(ResourceTypes value)
            : this((int)value)
        {
        }

        public ResourceType(int value)
            : this(new IntPtr(value))
        {
        }

        #endregion // Class construction.

        #region Class properties.

        public ResourceTypes Type
        {
            get { return this.type; }
        }

        #endregion // Class properties.

        #region Overridden implementation.

        public override string ToString()
        {
            if (this.IsIntResource(this.value))
            {
                switch (this.type)
                {
                    case ResourceTypes.RT_CURSOR:
                        return "Cursor Entry";
                    case ResourceTypes.RT_BITMAP:
                        return "Bitmap";
                    case ResourceTypes.RT_ICON:
                        return "Icon Entry";
                    case ResourceTypes.RT_MENU:
                        return "Menu";
                    case ResourceTypes.RT_DIALOG:
                        return "Dialog";
                    case ResourceTypes.RT_STRING:
                        return "String Table";
                    case ResourceTypes.RT_FONTDIR:
                        return "Font Directory";
                    case ResourceTypes.RT_FONT:
                        return "Font";
                    case ResourceTypes.RT_ACCELERATOR:
                        return "Accelerator";
                    case ResourceTypes.RT_RCDATA:
                        return "Binary";
                    case ResourceTypes.RT_MESSAGETABLE:
                        return "Messages";
                    case ResourceTypes.RT_GROUP_CURSOR:
                        return "Cursor";
                    case ResourceTypes.RT_GROUP_ICON:
                        return "Icon";
                    case ResourceTypes.RT_VERSION:
                        return "Version";
                    case ResourceTypes.RT_DLGINCLUDE:
                        return "Dialog Include";
                    case ResourceTypes.RT_PLUGPLAY:
                        return "Plug & Play";
                    case ResourceTypes.RT_VXD:
                        return "VDX";
                    case ResourceTypes.RT_ANICURSOR:
                        return "Animated Cursor";
                    case ResourceTypes.RT_ANIICON:
                        return "Animated Icon";
                    case ResourceTypes.RT_HTML:
                        return "HTML";
                    case ResourceTypes.RT_MANIFEST:
                        return "Manifest";
                    case ResourceTypes.RT_OTHER:
                        return "Other";
                    case ResourceTypes.RT_UNKNOWN:
                    default:
                        return "Unknown";
                }
            }
            else
            {
                return "\"" + this.name + "\"";
            }
        }

        #endregion // Overridden implementation.

        #region Prvate implementation.

        private ResourceTypes Convert(IntPtr value)
        {
            try
            {
                if (this.IsIntResource(value))
                {
                    switch ((ResourceTypes)value.ToInt32())
                    {
                        case ResourceTypes.RT_CURSOR:
                        case ResourceTypes.RT_BITMAP:
                        case ResourceTypes.RT_ICON:
                        case ResourceTypes.RT_MENU:
                        case ResourceTypes.RT_DIALOG:
                        case ResourceTypes.RT_STRING:
                        case ResourceTypes.RT_FONTDIR:
                        case ResourceTypes.RT_FONT:
                        case ResourceTypes.RT_ACCELERATOR:
                        case ResourceTypes.RT_RCDATA:
                        case ResourceTypes.RT_MESSAGETABLE:
                        case ResourceTypes.RT_GROUP_CURSOR:
                        case ResourceTypes.RT_GROUP_ICON:
                        case ResourceTypes.RT_VERSION:
                        case ResourceTypes.RT_DLGINCLUDE:
                        case ResourceTypes.RT_PLUGPLAY:
                        case ResourceTypes.RT_VXD:
                        case ResourceTypes.RT_ANICURSOR:
                        case ResourceTypes.RT_ANIICON:
                        case ResourceTypes.RT_HTML:
                        case ResourceTypes.RT_MANIFEST:
                            return (ResourceTypes)value.ToInt32();
                        default:
                            return ResourceTypes.RT_UNKNOWN;
                    }
                }
                else
                {
                    return ResourceTypes.RT_OTHER;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("ResourceType::Convert() " + exception.ToString());
            }
            return ResourceTypes.RT_UNKNOWN;
        }

        #endregion // Prvate implementation.
    }
}

// EOF