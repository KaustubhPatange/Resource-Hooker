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
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace plexdata.$safeprojectname$
{
    public class ApplicationSettings : ICloneable
    {
        private static ApplicationSettings instance = null;

        public static ApplicationSettings Instance
        {
            get
            {
                if (ApplicationSettings.instance == null)
                {
                    ApplicationSettings.instance = new ApplicationSettings();
                }
                return ApplicationSettings.instance;
            }
        }

        private ApplicationSettings()
            : base()
        {
        }

        public bool Save()
        {
            return plexdata.generic.Serializer.Save<ApplicationSettings>(
                ApplicationSettings.ConfigurationFilename,
                ApplicationSettings.instance);
        }

        public bool Load()
        {
            return plexdata.generic.Serializer.Load<ApplicationSettings>(
                ApplicationSettings.ConfigurationFilename,
                out ApplicationSettings.instance);
        }

        private MainWindow mainWindow = new MainWindow();
        public MainWindow MainWindow
        {
            get { return this.mainWindow; }
            set { this.mainWindow = value; }
        }

        private Settings settings = new Settings();
        public Settings Settings
        {
            get { return this.settings; }
            set { this.settings = value; }
        }

        private static string ConfigurationFilename
        {
            get
            {
                string path = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);

                return
                    Path.GetDirectoryName(path) +
                    Path.DirectorySeparatorChar +
                    Path.GetFileNameWithoutExtension(path) +
                    ".xml";
            }
        }

        #region ICloneable member implementation

        public object Clone()
        {
            ApplicationSettings clone = new ApplicationSettings();
            clone.mainWindow = (MainWindow)this.MainWindow.Clone();
            clone.settings = (Settings)this.Settings.Clone();
            return clone;
        }

        public void FromClone(ApplicationSettings clone)
        {
            this.mainWindow.FromClone(clone.mainWindow);
            this.settings.FromClone(clone.Settings);
        }

        #endregion // ICloneable member implementation
    }

    public class MainWindow : ICloneable
    {
        private static FormWindowState DEFAULT_STATE = FormWindowState.Normal;
        private static Point DEFAULT_LOCATION = new Point(200, 200);
        private static Size DEFAULT_SIZE = new Size(850, 500);
        private static int DEFAULT_SPLITTER_DISTANCE = 200;

        public MainWindow()
            : base()
        {
        }

        private FormWindowState state = MainWindow.DEFAULT_STATE;
        public FormWindowState State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        private Point location = MainWindow.DEFAULT_LOCATION;
        public Point Location
        {
            get { return this.EnsureScreenLocation(this.location); }
            set { this.location = this.EnsureScreenLocation(value); }
        }

        private Size size = MainWindow.DEFAULT_SIZE;
        public Size Size
        {
            get { return this.size; }
            set
            {
                if (value.Width < MainWindow.DEFAULT_SIZE.Width)
                {
                    this.size.Width = MainWindow.DEFAULT_SIZE.Width;
                }
                else
                {
                    this.size.Width = value.Width;
                }

                if (value.Height < MainWindow.DEFAULT_SIZE.Height)
                {
                    this.size.Height = MainWindow.DEFAULT_SIZE.Height;
                }
                else
                {
                    this.size.Height = value.Height;
                }
            }
        }

        private int splitterDistance = MainWindow.DEFAULT_SPLITTER_DISTANCE;
        public int SplitterDistance
        {
            get { return this.splitterDistance; }
            set { this.splitterDistance = value; }
        }

        private Point EnsureScreenLocation(Point location)
        {
            if (this.State == FormWindowState.Maximized || this.State == FormWindowState.Minimized)
            {
                // In this case let the form handle resizing.
                return location;
            }

            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(new Rectangle(location, this.Size)))
                {
                    return location; // Location is still within bounds.
                }
            }

            Rectangle bounds = Screen.PrimaryScreen.WorkingArea;

            // Adjust X location to be on the primary screen!

            int X = 0;
            if (location.X < bounds.Left)
            {
                X = bounds.Left;
            }
            else if (location.X + this.Size.Width > bounds.Left + bounds.Right)
            {
                X = bounds.Right - this.Size.Width;
            }
            else
            {
                X = location.X;
            }

            // Adjust Y location to be on the primary screen!

            int Y = 0;
            if (location.Y < bounds.Top)
            {
                Y = bounds.Top;
            }
            else if (location.Y + this.Size.Height > bounds.Top + bounds.Bottom)
            {
                Y = bounds.Bottom - this.Size.Height;
            }
            else
            {
                Y = location.Y;
            }

            return new Point(X, Y);
        }

        #region ICloneable member implementation

        public object Clone()
        {
            MainWindow clone = new MainWindow();
            clone.state = this.state;
            clone.location = new Point(this.location.X, this.location.Y);
            clone.size = new Size(this.size.Width, this.size.Height);
            return clone;
        }

        public void FromClone(MainWindow clone)
        {
            this.State = clone.state;
            this.Location = new Point(clone.location.X, clone.location.Y);
            this.Size = new Size(clone.size.Width, clone.size.Height);
        }

        #endregion // ICloneable member implementation
    }

    public class Settings : ICloneable
    {
        private static int MIN_BINARY_LINE_LENGTH = 8;
        private static int DEFAULT_BINARY_LINE_LENGTH = 16;
        private static int DEFAULT_CURSOR_BACKGROUND = Color.CadetBlue.ToArgb();
        private static int DEFAULT_VIEW_BACKGROUND = Color.White.ToArgb();
        private static int DEFAULT_VIEW_FOREGROUND = Color.Black.ToArgb();
        private static int DEFAULT_IMAGE_BORDER_COLOR = SystemColors.ControlLight.ToArgb();

        public Settings()
            : base()
        {
        }

        public void ResetDefaults()
        {
            this.exportPath = String.Empty;
            this.binaryLineLength = Settings.DEFAULT_BINARY_LINE_LENGTH;
            this.cursorBackground = Settings.DEFAULT_CURSOR_BACKGROUND;
            this.viewBackground = Settings.DEFAULT_VIEW_BACKGROUND;
            this.viewForeground = Settings.DEFAULT_VIEW_FOREGROUND;
            this.imageBorderColor = Settings.DEFAULT_IMAGE_BORDER_COLOR;
            this.autoLoadLastFiles = true;
            this.loaddetemplate = true;
            this.overwrite = false;
            this.advanceoption = true;
            this.closeform = false;

        }

        private string exportPath = String.Empty;
        public string ExportPath
        {
            get { return this.exportPath; }
            set { this.exportPath = (value == null) ? String.Empty : value; }
        }

        private List<string> lastFilenames = new List<string>();
        public string[] LastFilenames
        {
            get { return this.lastFilenames.ToArray(); }
            set
            {
                this.lastFilenames.Clear();
                if (value != null && value.Length > 0)
                {
                    this.lastFilenames.AddRange(value);
                }
            }
        }

        private int binaryLineLength = Settings.DEFAULT_BINARY_LINE_LENGTH;
        public int BinaryLineLength
        {
            get { return this.binaryLineLength; }
            set
            {
                if (value >= Settings.MIN_BINARY_LINE_LENGTH)
                {
                    this.binaryLineLength = value;
                }
                else
                {
                    this.binaryLineLength = Settings.MIN_BINARY_LINE_LENGTH;
                }
            }
        }

        private bool autoLoadLastFiles = true;
        public bool AutoLoadLastFiles
        {
            get { return this.autoLoadLastFiles; }
            set { this.autoLoadLastFiles = value; }
        }
        private bool loaddetemplate = true;
          public bool LoadTemplate
        {
            get { return this.loaddetemplate; }
            set { this.loaddetemplate = value; }
        }

        private bool overwrite = true;
        public bool Overwrite
        {
            get { return this.overwrite; }
            set { this.overwrite = value; }
        }
        private bool advanceoption = true;
        public bool ShowAdvanceOptions
        {
            get { return this.advanceoption; }
            set { this.advanceoption = value; }
        }
        private bool closeform = true;
        public bool CloseForm
        {
            get { return this.closeform; }
            set { this.closeform = value; }
        }
        private int cursorBackground = Settings.DEFAULT_CURSOR_BACKGROUND;
        public int CursorBackground
        {
            get { return this.cursorBackground; }
            set { this.cursorBackground = value; }
        }

        private int viewBackground = Settings.DEFAULT_VIEW_BACKGROUND;
        public int ViewBackground
        {
            get { return this.viewBackground; }
            set { this.viewBackground = value; }
        }

        private int viewForeground = Settings.DEFAULT_VIEW_FOREGROUND;
        public int ViewForeground
        {
            get { return this.viewForeground; }
            set { this.viewForeground = value; }
        }

        private int imageBorderColor = Settings.DEFAULT_IMAGE_BORDER_COLOR;
        public int ImageBorderColor
        {
            get { return this.imageBorderColor; }
            set { this.imageBorderColor = value; }
        }

        #region Color helper function

        public Color GetCursorBackground()
        {
            return Color.FromArgb(this.cursorBackground);
        }

        public void SetCursorBackground(Color color)
        {
            this.cursorBackground = color.ToArgb();
        }

        public Color GetViewBackground()
        {
            return Color.FromArgb(this.viewBackground);
        }

        public void SetViewBackground(Color color)
        {
            this.viewBackground = color.ToArgb();
        }

        public Color GetViewForeground()
        {
            return Color.FromArgb(this.viewForeground);
        }

        public void SetViewForeground(Color color)
        {
            this.viewForeground = color.ToArgb();
        }

        public Color GetImageBorderColor()
        {
            return Color.FromArgb(this.imageBorderColor);
        }

        public void SetImageBorderColor(Color color)
        {
            this.imageBorderColor = color.ToArgb();
        }

        #endregion // Color helper function

        #region ICloneable member implementation

        public object Clone()
        {
            Settings clone = new Settings();
            clone.lastFilenames.AddRange(this.lastFilenames.ToArray());
            clone.exportPath = this.exportPath;
            clone.binaryLineLength = this.binaryLineLength;
            clone.autoLoadLastFiles = this.autoLoadLastFiles;
            clone.loaddetemplate = this.loaddetemplate;
            clone.overwrite = this.overwrite;
            clone.advanceoption = this.advanceoption;
            clone.closeform = this.closeform;
            clone.cursorBackground = this.cursorBackground;
            clone.viewBackground = this.viewBackground;
            clone.viewForeground = this.viewForeground;
            clone.imageBorderColor = this.imageBorderColor;
            return clone;
        }

        public void FromClone(Settings clone)
        {
            this.LastFilenames = clone.lastFilenames.ToArray();
            this.ExportPath = clone.exportPath;
            this.BinaryLineLength = clone.binaryLineLength;
            this.autoLoadLastFiles = clone.autoLoadLastFiles;
            this.loaddetemplate = clone.loaddetemplate;
            this.overwrite = clone.overwrite;
            this.advanceoption = clone.advanceoption;
            this.closeform = clone.closeform;
            this.CursorBackground = clone.cursorBackground;
            this.ViewBackground = clone.viewBackground;
            this.ViewForeground = clone.viewForeground;
            this.ImageBorderColor = clone.imageBorderColor;
        }

        #endregion // ICloneable member implementation
    }
}

// EOF