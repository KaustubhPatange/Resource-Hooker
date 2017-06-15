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
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

namespace plexdata.Utilities.Resources
{
    public class VersionInfoValue
    {
        private string name = null;
        private object value = null;

        public VersionInfoValue()
            : this(null)
        {
        }

        public VersionInfoValue(object value)
            : base()
        {
            // Use class name but bemove class qualifier if exist.
            string helper = base.ToString();
            int index = helper.LastIndexOfAny(new char[] { '+', '.' });
            if (index >= 0 && index + 1 < helper.Length)
            {
                helper = helper.Substring(index + 1);
            }

            this.name = helper;
            this.value = value;
        }

        public VersionInfoValue(string name, object value)
            : this(value)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = (String.IsNullOrEmpty(value)) ? "" : value; }
        }

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public virtual string ValueAsString()
        {
            if (this.value != null)
            {
                return this.value.ToString();
            }
            else
            {
                return "";
            }
        }

        public override string ToString()
        {
            return this.Name + " " + this.ValueAsString();
        }
    }

    public abstract class VersionInfoBase
    {
        protected BinaryReader reader = null;

        private ushort length = 0;
        private ushort value = 0;
        private ushort type = 0;
        private string key = "";

        public VersionInfoBase()
            : base()
        {
        }

        public ushort DataLength
        {
            get { return this.length; }
            protected set { this.length = value; }
        }

        public ushort ValueLength
        {
            get { return this.value; }
            protected set { this.value = value; }
        }

        public ushort DataType
        {
            get { return this.type; }
            protected set { this.type = value; }
        }

        public string Key
        {
            get { return this.key; }
            protected set { this.key = value; }
        }

        public bool IsBinaryResource()
        {
            // This member is 1 if the version resource contains text data 
            // and 0 if the version resource contains binary data.
            return (this.type == 0);
        }

        protected void SkipPadding(int until)
        {
            // Eat padding until it fits on 32 bit range.
            while (this.HasPadding(until))
            {
                byte dummy = reader.ReadByte();
            }
        }

        protected bool HasPadding(int count)
        {
            if (this.HasMoreData())
            {
                return ((this.reader.BaseStream.Position % count) != 0);
            }
            return false;
        }

        protected bool HasMoreData()
        {
            if (this.reader != null)
            {
                if (this.reader.BaseStream != null)
                {
                    return (this.reader.BaseStream.Position < this.reader.BaseStream.Length);
                }
            }
            return false;
        }

        protected bool HasMoreData(long length)
        {
            if (this.HasMoreData())
            {
                return ((this.reader.BaseStream.Length - this.reader.BaseStream.Position) >= length);
            }
            return false;
        }

        protected ushort ReadHeader()
        {
            ushort result = 0;
            if (this.HasMoreData(sizeof(ushort)))
            {
                result = this.reader.ReadUInt16();
            }
            return result;
        }

        protected string ReadString()
        {
            string result = "";
            while (this.HasMoreData(sizeof(char)))
            {
                // NOTE: Function reader.ReadString() does not return with expected data!
                char current = reader.ReadChar();
                if (current == '\0') // Not sure if strings are zero terminated!
                {
                    break;
                }
                result += current;
            }
            return result;
        }

        protected byte[] ReadBytes(int count)
        {
            byte[] result = new byte[0];
            if (this.HasMoreData(count))
            {
                result = this.reader.ReadBytes(count);
            }
            return result;
        }

        internal abstract bool Read(BinaryReader reader);
    }

    // See also Win32 structure VS_VERSIONINFO
    public class VersionInfo : VersionInfoBase
    {
        private FixedFileInfo fixedFileInfo = null;
        private StringFileInfo stringFileInfo = null;
        private VarFileInfo varFileInfo = null;
        private string name = "VersionInfo";

        public static BinaryReader GetReader(byte[] buffer)
        {
            if (buffer == null)
            {
                buffer = new byte[0];
            }
            return new BinaryReader(new MemoryStream(buffer), Encoding.Unicode);
        }

        public VersionInfo()
            : base()
        {
            this.fixedFileInfo = new FixedFileInfo();
            this.stringFileInfo = new StringFileInfo();
            this.varFileInfo = new VarFileInfo();
        }

        public string Name
        {
            get { return this.name; }
        }

        public FixedFileInfo FixedFileInfo
        {
            get { return this.fixedFileInfo; }
        }

        public StringFileInfo StringFileInfo
        {
            get { return this.stringFileInfo; }
        }

        public VarFileInfo VarFileInfo
        {
            get { return this.varFileInfo; }
        }

        public string[] GetResults()
        {
            List<string> result = new List<string>();

            result.Add(this.fixedFileInfo.Name);
            foreach (VersionInfoValue current in this.fixedFileInfo.Values)
            {
                result.Add(String.Format("    {0,-16}{1}", current.Name+":", current.ValueAsString()));

            }

            result.Add(this.stringFileInfo.Name);
            foreach (StringBlock current in this.stringFileInfo.Values)
            {
                result.Add("    BlockHeader: " + current.Name + " (" + current.Language + ", " + current.Codepage + ")");
                foreach (StringValue value in current.Values)
                {

                    result.Add(String.Format("        {0,-18}{1}", value.Name + ":", value.Value));
                }
            }

            result.Add(this.varFileInfo.Name);
            foreach (VarValue current in this.varFileInfo.Values)
            {
                result.Add("    " + current.Name);
                foreach (string value in current.Values)
                {
                    result.Add("        " + value);
                }
            }

            return result.ToArray();
        }

        internal override bool Read(BinaryReader reader)
        {
            bool success = true; // Assume success.

            this.reader = reader;

            try
            {
                if (this.HasMoreData())
                {
                    this.DataLength = this.ReadHeader();
                    this.ValueLength = this.ReadHeader(); // Length of VS_FIXEDFILEINFO
                    this.DataType = this.ReadHeader();
                    this.Key = this.ReadString();

                    // Eat padding (Padding1 of VS_VERSIONINFO) 
                    // until it fits on 32 bit range.
                    this.SkipPadding(sizeof(int));

                    if (this.Key == "VS_VERSION_INFO" && this.ValueLength > 0)
                    {
                        // Read VS_FIXEDFILEINFO data (fixed structure length)
                        success = success & this.fixedFileInfo.Read(this.reader);
                    }

                    // Eat padding (Padding2 of VS_VERSIONINFO) 
                    // until it fits on 32 bit range.
                    this.SkipPadding(sizeof(int));

                    // An array of zero or one StringFileInfo structures, 
                    // and zero or one VarFileInfo structures that are 
                    // children of the current VS_VERSIONINFO structure.

                    // Read StringFileInfo data.
                    success = success & this.stringFileInfo.Read(this.reader);

                    // Read VarFileInfo data.
                    success = success & this.varFileInfo.Read(this.reader);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("VersionInfo.Read(): " + exception.ToString());
                success = false;
            }

            return success;
        }
    }

    // See also Win32 structure VS_FIXEDFILEINFO
    public class FixedFileInfo : VersionInfoBase
    {
        #region VS_FIXEDFILEINFO helper classes.

        public class Signature : VersionInfoValue
        {
            public const uint VS_FFI_SIGNATURE = 0xFEEF04BD;
            public const uint VS_FFI_SIGNATURE_MAC = 0xBD04EFFE;

            public Signature()
                : base()
            {
            }

            public Signature(uint value)
                : base(value)
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    uint value = Convert.ToUInt32(base.Value);
                    result = String.Format("0x{0:X8} ", value);
                    switch (value)
                    {
                        case Signature.VS_FFI_SIGNATURE:
                            result += "(VS_FFI_SIGNATURE)";
                            break;
                        case Signature.VS_FFI_SIGNATURE_MAC:
                            result += "(VS_FFI_SIGNATURE_MAC)";
                            break;
                        default:
                            result += "(This is a problem!)";
                            break;
                    }
                }
                catch { }
                return result;
            }
        }

        public class StrucVersion : VersionInfoValue
        {
            public const uint VS_FFI_STRUCVERSION = 0x00010000;

            public StrucVersion()
                : base()
            {
            }

            public StrucVersion(uint value)
                : base(value)
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    uint value = Convert.ToUInt32(base.Value);
                    result = String.Format("0x{0:X8} ", value);
                    switch (value)
                    {
                        case StrucVersion.VS_FFI_STRUCVERSION:
                            result += "(VS_FFI_STRUCVERSION)";
                            break;
                        default:
                            result += "(This is a problem!)";
                            break;
                    }
                }
                catch { }
                return result;
            }
        }

        public class FileVersion : VersionInfoValue
        {
            public FileVersion()
                : base()
            {
            }

            public FileVersion(uint version, uint build)
                : base((ulong)((ulong)version << 32 | build))
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    ulong value = Convert.ToUInt64(base.Value);
                    result = String.Format(
                        "{0}.{1}.{2}.{3}",
                        this.HiWord(this.HiDWord(value)),
                        this.LoWord(this.HiDWord(value)),
                        this.HiWord(this.LoDWord(value)),
                        this.LoWord(this.LoDWord(value))
                    );
                }
                catch
                {
                }
                return result;
            }

            private ushort HiWord(uint value)
            {
                return (ushort)(value >> 16);
            }

            private ushort LoWord(uint value)
            {
                return (ushort)(value & ushort.MaxValue);
            }

            private uint HiDWord(ulong value)
            {
                return (uint)(value >> 32);
            }

            private uint LoDWord(ulong value)
            {
                return (uint)(value & uint.MaxValue);
            }
        }

        public class ProductVersion : VersionInfoValue
        {
            public ProductVersion()
                : base()
            {
            }

            public ProductVersion(uint version, uint build)
                : base((ulong)((ulong)version << 32 | build))
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    ulong value = Convert.ToUInt64(base.Value);
                    result = String.Format(
                        "{0}.{1}.{2}.{3}",
                        this.HiWord(this.HiDWord(value)),
                        this.LoWord(this.HiDWord(value)),
                        this.HiWord(this.LoDWord(value)),
                        this.LoWord(this.LoDWord(value))
                    );
                }
                catch
                {
                }
                return result;
            }

            private ushort HiWord(uint value)
            {
                return (ushort)(value >> 16);
            }

            private ushort LoWord(uint value)
            {
                return (ushort)(value & ushort.MaxValue);
            }

            private uint HiDWord(ulong value)
            {
                return (uint)(value >> 32);
            }

            private uint LoDWord(ulong value)
            {
                return (uint)(value & uint.MaxValue);
            }
        }

        public class FileFlagsMask : VersionInfoValue
        {
            public const uint VS_FFI_FILEFLAGSMASK = 0x0000003F;

            public FileFlagsMask()
                : base()
            {
            }

            public FileFlagsMask(uint value)
                : base(value)
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    uint value = Convert.ToUInt32(base.Value);
                    result = String.Format("0x{0:X8} ", value);
                    switch (value)
                    {
                        case FileFlagsMask.VS_FFI_FILEFLAGSMASK:
                            result += "(VS_FFI_FILEFLAGSMASK)";
                            break;
                        default:
                            // Not a problem because could vary...
                            // result += "(This is a problem!)";
                            break;
                    }
                }
                catch { }
                return result;
            }
        }

        public class FileFlags : VersionInfoValue
        {
            public const int VS_FF_DEBUG = 0x00000001;
            public const int VS_FF_PRERELEASE = 0x00000002;
            public const int VS_FF_PATCHED = 0x00000004;
            public const int VS_FF_PRIVATEBUILD = 0x00000008;
            public const int VS_FF_INFOINFERRED = 0x00000010;
            public const int VS_FF_SPECIALBUILD = 0x00000020;

            public FileFlags()
                : base()
            {
            }

            public FileFlags(uint value)
                : base(value)
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    uint value = Convert.ToUInt32(base.Value);
                    result = String.Format("0x{0:X8}", value);
                    if (value != 0)
                    {
                        string helper = "";
                        if (((value & FileFlags.VS_FF_DEBUG) == FileFlags.VS_FF_DEBUG))
                        {
                            helper += "VS_FF_DEBUG|";
                        }

                        if (((value & FileFlags.VS_FF_PRERELEASE) == FileFlags.VS_FF_PRERELEASE))
                        {
                            helper += "VS_FF_PRERELEASE|";
                        }

                        if (((value & FileFlags.VS_FF_PATCHED) == FileFlags.VS_FF_PATCHED))
                        {
                            helper += "VS_FF_PATCHED|";
                        }

                        if (((value & FileFlags.VS_FF_PRIVATEBUILD) == FileFlags.VS_FF_PRIVATEBUILD))
                        {
                            helper += "VS_FF_PRIVATEBUILD|";
                        }

                        if (((value & FileFlags.VS_FF_INFOINFERRED) == FileFlags.VS_FF_INFOINFERRED))
                        {
                            helper += "VS_FF_INFOINFERRED|";
                        }

                        if (((value & FileFlags.VS_FF_SPECIALBUILD) == FileFlags.VS_FF_SPECIALBUILD))
                        {
                            helper += "VS_FF_SPECIALBUILD|";
                        }

                        int index = helper.LastIndexOf('|');
                        if (index != -1)
                        {
                            result += " (" + helper.Substring(0, index).TrimEnd() + ")";
                        }
                    }
                }
                catch { }
                return result;
            }
        }

        public class FileOS : VersionInfoValue
        {
            public const uint VOS_UNKNOWN = 0x00000000;
            public const uint VOS_DOS = 0x00010000;
            public const uint VOS_OS216 = 0x00020000;
            public const uint VOS_OS232 = 0x00030000;
            public const uint VOS_NT = 0x00040000;
            public const uint VOS_WINCE = 0x00050000;
            public const uint VOS__BASE = 0x00000000;
            public const uint VOS__WINDOWS16 = 0x00000001;
            public const uint VOS__PM16 = 0x00000002;
            public const uint VOS__PM32 = 0x00000003;
            public const uint VOS__WINDOWS32 = 0x00000004;
            public const uint VOS_DOS_WINDOWS16 = 0x00010001;
            public const uint VOS_DOS_WINDOWS32 = 0x00010004;
            public const uint VOS_OS216_PM16 = 0x00020002;
            public const uint VOS_OS232_PM32 = 0x00030003;
            public const uint VOS_NT_WINDOWS32 = 0x00040004;

            public FileOS()
                : base()
            {
            }

            public FileOS(uint value)
                : base(value)
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    uint value = Convert.ToUInt32(base.Value);
                    result = String.Format("0x{0:X8} ", value);
                    switch (value)
                    {
                        case FileOS.VOS_UNKNOWN: // Legacy name VOS__BASE.
                            result += "(VOS_UNKNOWN)";
                            break;
                        case FileOS.VOS_DOS:
                            result += "(VOS_DOS)";
                            break;
                        case FileOS.VOS_OS216:
                            result += "(VOS_OS216)";
                            break;
                        case FileOS.VOS_OS232:
                            result += "(VOS_OS232)";
                            break;
                        case FileOS.VOS_NT:
                            result += "(VOS_NT)";
                            break;
                        case FileOS.VOS_WINCE:
                            result += "(VOS_WINCE)";
                            break;
                        case FileOS.VOS__WINDOWS16:
                            result += "(VOS__WINDOWS16)";
                            break;
                        case FileOS.VOS__PM16:
                            result += "(VOS__PM16)";
                            break;
                        case FileOS.VOS__PM32:
                            result += "(VOS__PM32)";
                            break;
                        case FileOS.VOS__WINDOWS32:
                            result += "(VOS__WINDOWS32)";
                            break;
                        case FileOS.VOS_DOS_WINDOWS16:
                            result += "(VOS_DOS_WINDOWS16)";
                            break;
                        case FileOS.VOS_DOS_WINDOWS32:
                            result += "(VOS_DOS_WINDOWS32)";
                            break;
                        case FileOS.VOS_OS216_PM16:
                            result += "(VOS_OS216_PM16)";
                            break;
                        case FileOS.VOS_OS232_PM32:
                            result += "(VOS_OS232_PM32)";
                            break;
                        case FileOS.VOS_NT_WINDOWS32:
                            result += "(VOS_NT_WINDOWS32)";
                            break;
                        default:
                            result += "(This is a problem!)";
                            break;
                    }
                }
                catch { }
                return result;
            }
        }

        public class FileType : VersionInfoValue
        {
            public const uint VFT_UNKNOWN = 0x00000000;
            public const uint VFT_APP = 0x00000001;
            public const uint VFT_DLL = 0x00000002;
            public const uint VFT_DRV = 0x00000003;
            public const uint VFT_FONT = 0x00000004;
            public const uint VFT_VXD = 0x00000005;
            public const uint VFT_STATIC_LIB = 0x00000007;

            public FileType()
                : base()
            {
            }

            public FileType(uint value)
                : base(value)
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    uint value = Convert.ToUInt32(base.Value);
                    result = String.Format("0x{0:X8} ", value);
                    switch (value)
                    {
                        case FileType.VFT_APP:
                            result += "(VFT_APP)";
                            break;
                        case FileType.VFT_DLL:
                            result += "(VFT_DLL)";
                            break;
                        case FileType.VFT_DRV:
                            result += "(VFT_DRV)";
                            break;
                        case FileType.VFT_FONT:
                            result += "(VFT_FONT)";
                            break;
                        case FileType.VFT_VXD:
                            result += "(VFT_VXD)";
                            break;
                        case FileType.VFT_STATIC_LIB:
                            result += "(VFT_STATIC_LIB)";
                            break;
                        case FileType.VFT_UNKNOWN:
                        default:
                            result += "(VFT_UNKNOWN)";
                            break;
                    }
                }
                catch { }
                return result;
            }
        }

        public class FileSubType : VersionInfoValue
        {
            public const uint VFT2_UNKNOWN = 0x00000000;
            public const uint VFT2_DRV_PRINTER = 0x00000001;
            public const uint VFT2_DRV_KEYBOARD = 0x00000002;
            public const uint VFT2_DRV_LANGUAGE = 0x00000003;
            public const uint VFT2_DRV_DISPLAY = 0x00000004;
            public const uint VFT2_DRV_MOUSE = 0x00000005;
            public const uint VFT2_DRV_NETWORK = 0x00000006;
            public const uint VFT2_DRV_SYSTEM = 0x00000007;
            public const uint VFT2_DRV_INSTALLABLE = 0x00000008;
            public const uint VFT2_DRV_SOUND = 0x00000009;
            public const uint VFT2_DRV_COMM = 0x0000000A;
            public const uint VFT2_DRV_INPUTMETHOD = 0x0000000B;
            public const uint VFT2_DRV_VERSIONED_PRINTER = 0x0000000C;
            public const uint VFT2_FONT_RASTER = 0x00000001;
            public const uint VFT2_FONT_VECTOR = 0x00000002;
            public const uint VFT2_FONT_TRUETYPE = 0x00000003;

            private uint type = 0;

            public FileSubType()
                : base()
            {
            }

            public FileSubType(uint value, FixedFileInfo.FileType type)
                : base(value)
            {
                try { this.type = Convert.ToUInt32(type.Value); }
                catch { }
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    uint value = Convert.ToUInt32(base.Value);
                    result = String.Format("0x{0:X8} ", value);

                    if (this.type == FileType.VFT_DRV) // VFT_WINDOWS_DRV
                    {
                        switch (value)
                        {
                            case FileSubType.VFT2_DRV_PRINTER:
                                result += "(VFT2_DRV_PRINTER)";
                                break;
                            case FileSubType.VFT2_DRV_KEYBOARD:
                                result += "(VFT2_DRV_KEYBOARD)";
                                break;
                            case FileSubType.VFT2_DRV_LANGUAGE:
                                result += "(VFT2_DRV_LANGUAGE)";
                                break;
                            case FileSubType.VFT2_DRV_DISPLAY:
                                result += "(VFT2_DRV_DISPLAY)";
                                break;
                            case FileSubType.VFT2_DRV_MOUSE:
                                result += "(VFT2_DRV_MOUSE)";
                                break;
                            case FileSubType.VFT2_DRV_NETWORK:
                                result += "(VFT2_DRV_NETWORK)";
                                break;
                            case FileSubType.VFT2_DRV_SYSTEM:
                                result += "(VFT2_DRV_SYSTEM)";
                                break;
                            case FileSubType.VFT2_DRV_INSTALLABLE:
                                result += "(VFT2_DRV_INSTALLABLE)";
                                break;
                            case FileSubType.VFT2_DRV_SOUND:
                                result += "(VFT2_DRV_SOUND)";
                                break;
                            case FileSubType.VFT2_DRV_COMM:
                                result += "(VFT2_DRV_COMM)";
                                break;
                            case FileSubType.VFT2_DRV_INPUTMETHOD:
                                result += "(VFT2_DRV_INPUTMETHOD)";
                                break;
                            case FileSubType.VFT2_DRV_VERSIONED_PRINTER:
                                result += "(VFT2_DRV_VERSIONED_PRINTER)";
                                break;
                            case FileSubType.VFT2_UNKNOWN:
                            default:
                                result += "(VFT2_UNKNOWN)";
                                break;
                        }
                    }
                    else if (this.type == FileType.VFT_FONT) // VFT_WINDOWS_FONT
                    {
                        switch (value)
                        {
                            case FileSubType.VFT2_FONT_RASTER:
                                result += "(VFT2_FONT_RASTER)";
                                break;
                            case FileSubType.VFT2_FONT_VECTOR:
                                result += "(VFT2_FONT_VECTOR)";
                                break;
                            case FileSubType.VFT2_FONT_TRUETYPE:
                                result += "(VFT2_FONT_TRUETYPE)";
                                break;
                            case FileSubType.VFT2_UNKNOWN:
                            default:
                                result += "(VFT2_UNKNOWN)";
                                break;
                        }
                    }
                    else
                    {
                        result += "(VFT2_UNKNOWN)";
                    }
                }
                catch { }
                return result;
            }
        }

        public class FileDate : VersionInfoValue
        {
            public FileDate()
                : base()
            {
            }

            public FileDate(uint high, uint low)
                : base((ulong)((ulong)high << 32 | low))
            {
            }

            public override string ValueAsString()
            {
                string result = "";
                try
                {
                    ulong value = Convert.ToUInt64(base.Value);
                    DateTime date = new DateTime((long)value);
                    result = date.ToString();
                }
                catch { }
                return result;
            }
        }

        #endregion // VS_FIXEDFILEINFO helper classes.

        private List<VersionInfoValue> values = null;

        public FixedFileInfo()
            : base()
        {
            this.values = new List<VersionInfoValue>();
        }

        public string Name
        {
            get { return this.Key; }
        }

        public VersionInfoValue[] Values
        {
            get { return this.values.ToArray(); }
        }

        internal override bool Read(BinaryReader reader)
        {
            bool success = false;

            this.reader = reader;

            try
            {
                this.Key = "FixedFileInfo";

                if (this.HasMoreData(13 * sizeof(uint)))
                {
                    this.values.Add(new Signature(this.reader.ReadUInt32()));
                    this.values.Add(new StrucVersion(this.reader.ReadUInt32()));
                    this.values.Add(new FileVersion(this.reader.ReadUInt32(), this.reader.ReadUInt32()));
                    this.values.Add(new ProductVersion(this.reader.ReadUInt32(), this.reader.ReadUInt32()));
                    this.values.Add(new FileFlagsMask(this.reader.ReadUInt32()));
                    this.values.Add(new FileFlags(this.reader.ReadUInt32()));
                    this.values.Add(new FileOS(this.reader.ReadUInt32()));
                    FileType fileType = new FileType(this.reader.ReadUInt32());
                    this.values.Add(fileType);
                    this.values.Add(new FileSubType(this.reader.ReadUInt32(), fileType));
                    this.values.Add(new FileDate(this.reader.ReadUInt32(), this.reader.ReadUInt32()));

                    success = true;
                }
            }
            catch { }
            return success;
        }
    }

    // See also Win32 structure "StringFileInfo"
    public class StringFileInfo : VersionInfoBase
    {
        private List<StringBlock> values = null;

        public StringFileInfo()
            :base()
        {
            this.values = new List<StringBlock>();
        }

        public string Name
        {
            get { return this.Key; }
        }

        public StringBlock[] Values
        {
            get { return this.values.ToArray(); }
        }

        internal override bool Read(BinaryReader reader)
        {
            bool success = true; // Assume success!

            this.reader = reader;

            try
            {
                // Save current position.
                long offset = this.reader.BaseStream.Position;

                this.DataLength = this.ReadHeader();    // The length, in bytes, of the StringFileInfo including all Children.
                this.ValueLength = this.ReadHeader();   // This member is always equal to zero.
                this.DataType = this.ReadHeader();
                this.Key = this.ReadString();           // The Unicode string "StringFileInfo".

                // Eat padding (Padding of StringFileInfo) 
                // until it fits on 32 bit range.
                this.SkipPadding(sizeof(int));

                // An array of zero or one StringFileInfo structures, 
                // and zero or one VarFileInfo structures that are 
                // children of the current VS_VERSIONINFO structure.

                if (this.Key != "StringFileInfo")
                {
                    // Back to start offset because VarFileInfo could follow!
                    this.reader.BaseStream.Position = offset;
                }
                else
                {
                    // Calculate remaining data length.
                    long delta = (this.DataLength - (this.reader.BaseStream.Position - offset));

                    while (delta > 0 && this.HasMoreData())
                    {
                        // Save current position.
                        offset = this.reader.BaseStream.Position;
                      
                        // Try reading next value.
                        StringBlock entry = new StringBlock();
                        if (entry.Read(this.reader))
                        {
                            this.values.Add(entry);
                        }

                        // Recalculate remaining data length.
                        delta -= (this.reader.BaseStream.Position - offset);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("StringFileInfo.Read(): " + exception.ToString());
                success = false;
            }

            return success;
        }
    }

    // See also Win32 structure "StringTable"
    public class StringBlock : VersionInfoBase
    {
        private List<StringValue> values = null;
        private string language = null;
        private string codepage = null;

        public StringBlock()
            : base()
        {
            this.values = new List<StringValue>();
            this.language = String.Empty;
            this.codepage = String.Empty;
        }

        public string Name
        {
            get { return this.Key; }
        }

        public StringValue[] Values
        {
            get { return this.values.ToArray(); }
        }

        public string Language
        {
            get
            {
                string result = "";
                try
                {
                    if (!String.IsNullOrEmpty(this.Key))
                    {
                        int value = 0;
                        if (int.TryParse(this.Key, NumberStyles.AllowHexSpecifier, null, out value))
                        {
                            result = CultureInfo.GetCultureInfo((value >> 16)).Name;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("StringBlock.Language: " + exception.ToString());
                }
                return result;
            }
        }

        public string Codepage
        {
            get
            {
                string result = "";
                try
                {
                    if (!String.IsNullOrEmpty(this.Key))
                    {
                        int value = 0;
                        if (int.TryParse(this.Key, NumberStyles.AllowHexSpecifier, null, out value))
                        {
                            result = Encoding.GetEncoding((value & 0x0000FFFF)).EncodingName;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("StringBlock.Codepage: " + exception.ToString());
                }
                return result;
            }
        }

        internal override bool Read(BinaryReader reader)
        {
            bool success = true; // Assume success!

            this.reader = reader;

            try
            {
                // Save current position.
                long offset = this.reader.BaseStream.Position;

                this.DataLength = this.ReadHeader();    // The length, in bytes, of this StringTable including all Children.
                this.ValueLength = this.ReadHeader();   // This member is always equal to zero.
                this.DataType = this.ReadHeader();
                this.Key = this.ReadString();           // Key == HiWord(LCID) + LoWord(Codepage)

                // Eat padding (Padding of StringTable) 
                // until it fits on 32 bit range.
                this.SkipPadding(sizeof(int));

                // Calculate remaining data length.
                long delta = (this.DataLength - (this.reader.BaseStream.Position - offset));

                while (delta > 0 && this.HasMoreData())
                {
                    // Save current position.
                    offset = this.reader.BaseStream.Position;

                    // Try reading next value.
                    StringValue value = new StringValue();
                    if (value.Read(this.reader))
                    {
                        this.values.Add(value);
                    }

                    // Recalculate remaining data length.
                    delta -= (this.reader.BaseStream.Position - offset);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("StringBlock.Read(): " + exception.ToString());
                success = false;
            }

            return success;
        }
    }

    // See also Win32 structure "String"
    public class StringValue : VersionInfoBase
    {
        private byte[] value = null;

        public StringValue()
            : base()
        {
            this.value = new byte[0];
        }

        public string Name
        {
            get { return this.Key; }
        }

        public string Value
        {
            get
            {
                string result = "";
                try
                {
                    if (this.IsBinaryResource())
                    {
                        // Convert to string with hex-binary values!
                        foreach (byte current in this.value)
                        {
                            result += String.Format("{0:X2} ", current);
                        }
                    }
                    else
                    {
                        // Convert current byte array into a string!
                        result = Encoding.Unicode.GetString(this.value);

                        // Remove zero termination which comes from byte array!
                        if (result.Length > 0 && result[result.Length - 1] == '\0')
                        {
                            result = result.Remove(result.Length - 1);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("VarValue.Value: " + exception.ToString());
                }
                return result;
            }
        }

        internal override bool Read(BinaryReader reader)
        {
            bool success = false;

            this.reader = reader;

            try
            {
                this.DataLength = this.ReadHeader();   // Length, in bytes, of this structure!
                this.ValueLength = this.ReadHeader();  // Size, in words, of the value member!
                this.DataType = this.ReadHeader();
                this.Key = this.ReadString();

                // Eat padding (Padding of String) 
                // until it fits on 32 bit range.
                this.SkipPadding(sizeof(int));

                // Read current value.
                this.value = this.ReadBytes(this.ValueLength * sizeof(char));

                // Another (undocumented) padding skip is needed!
                this.SkipPadding(sizeof(int));

                success = true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("StringValue.Read(): " + exception.ToString());
            }

            return success;
        }
    }

    // See also Win32 structure "VarFileInfo"
    public class VarFileInfo : VersionInfoBase
    {
        private List<VarValue> values = null;

        public VarFileInfo()
            :base()
        {
            this.values = new List<VarValue>();
        }

        public string Name
        {
            get { return this.Key; }
        }

        public VarValue[] Values
        {
            get { return this.values.ToArray(); }
        }

        internal override bool Read(BinaryReader reader)
        {
            bool success = false;

            this.reader = reader;

            try
            {
                // Save current position.
                long offset = this.reader.BaseStream.Position;

                this.DataLength = this.ReadHeader();    // The length, in bytes, of the entire VarFileInfo including all Children.
                this.ValueLength = this.ReadHeader();   // This member is always equal to zero.
                this.DataType = this.ReadHeader();
                this.Key = this.ReadString();           // The Unicode string "VarFileInfo".

                // Eat padding (Padding of VarFileInfo) 
                // until it fits on 32 bit range.
                this.SkipPadding(sizeof(int));

                // An array of zero or one StringFileInfo structures, 
                // and zero or one VarFileInfo structures that are 
                // children of the current VS_VERSIONINFO structure.

                if (this.Key != "VarFileInfo")
                {
                    // Back to start offset because VarFileInfo could follow!
                    this.reader.BaseStream.Position = offset;
                }
                else
                {
                    // Calculate remaining data length.
                    long delta = (this.DataLength - (this.reader.BaseStream.Position - offset));

                    while (delta > 0 && this.HasMoreData())
                    {
                        // Save current position.
                        offset = this.reader.BaseStream.Position;

                        // Try reading next value.
                        VarValue entry = new VarValue();
                        if (entry.Read(this.reader))
                        {
                            this.values.Add(entry);
                        }

                        // Recalculate remaining data length.
                        delta -= (this.reader.BaseStream.Position - offset);
                    }

                    success = true;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("VarFileInfo.Read(): " + exception.ToString());
            }

            return success;
        }
    }

    // See also Win32 structure "Var"
    public class VarValue : VersionInfoBase
    {
        private byte[] value = null;

        public VarValue()
            : base()
        {
            this.value = new byte[0];
        }

        public string Name
        {
            get { return this.Key; }
        }

        public string[] Values
        {
            get
            {
                string[] result = new string[] { "" };
                try
                {
                    if (this.IsTranslation())
                    {
                        // If key contains "Translation" then the value consists of 
                        // an array of one or more DWORD values that are language 
                        // and code page identifier pairs. 
                        if ((this.value.Length % sizeof(int)) == 0)
                        {
                            // The low-order word of each DWORD must contain a Microsoft 
                            // language identifier, and the high-order word must contain 
                            // the IBM code page number (MSDN). 
                            string[] helper = new string[this.value.Length / sizeof(int)];
                            for (int index = 0; index < helper.Length; index++)
                            {
                                int value = BitConverter.ToInt32(this.value, index * sizeof(int));
                                int lcid = (value & 0x0000FFFF);
                                int code = (value >> 16);

                                helper[index] = String.Format(
                                    "{0:X8} ({1}, {2})",
                                    value,
                                    this.GetLanguage(lcid),
                                    this.GetCodepage(code));
                            }
                            result = helper;
                        }
                        else
                        {
                            // TODO: Handle this error.
                        }
                    }
                    else if (this.IsBinaryResource())
                    {
                        // Convert to string with hex-binary values!
                        foreach (byte current in this.value)
                        {
                            result[0] += String.Format("{0:X2} ", current);
                        }
                    }
                    else
                    {
                        // Convert current byte array into a string!
                        string helper = Encoding.Unicode.GetString(this.value);

                        // Remove zero termination which comes from byte array!
                        if (helper.Length > 0 && helper[helper.Length - 1] == '\0')
                        {
                            helper = helper.Remove(helper.Length - 1);
                        }

                        result[0] = helper;
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("VarValue.Value: " + exception.ToString());
                }
                return result;
            }
        }

        internal override bool Read(BinaryReader reader)
        {
            bool success = false;

            this.reader = reader;

            try
            {
                this.DataLength = this.ReadHeader();    // Length, in bytes, of this structure!
                this.ValueLength = this.ReadHeader();   // Size, in bytes, of the value member!
                this.DataType = this.ReadHeader();
                this.Key = this.ReadString();           // Typically contains "Translation"

                // Eat padding (Padding of String) 
                // until it fits on 32 bit range.
                this.SkipPadding(sizeof(int));

                // Read current value.
                this.value = this.ReadBytes(this.ValueLength);

                // Another (undocumented) padding skip is needed!
                this.SkipPadding(sizeof(int));

                success = true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("VarValue.Read(): " + exception.ToString());
            }

            return success;
        }

        private bool IsTranslation()
        {
            return (this.Key == "Translation");
        }

        private string GetLanguage(int lcid)
        {
            string result = "";
            try
            {
                if (lcid == 0)
                {
                    result = "<Neutral>";
                }
                else
                {
                    result = CultureInfo.GetCultureInfo(lcid).Name;
                }

                if (String.IsNullOrEmpty(result))
                {
                    result = "<Unknown>";
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("VarValue.GetLanguage(): " + exception.ToString());
            }
            return result;
        }

        private string GetCodepage(int code)
        {
            string result = "";
            try
            {
                result = Encoding.GetEncoding(code).EncodingName;

                if (String.IsNullOrEmpty(result))
                {
                    result = "<Unknown>";
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("VarValue.GetCodepage(): " + exception.ToString());
            }
            return result;
        }
    }
}
