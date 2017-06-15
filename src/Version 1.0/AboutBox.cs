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
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

namespace plexdata.$safeprojectname$
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            this.InitializeComponent();

            this.Text += AboutBox.Title;
            this.Icon = Program.GetFormIcon();

            this.lblProduct.Text = AboutBox.Product;
            this.lblVersion.Text = String.Format("{0} {1}", this.lblVersion.Text, AboutBox.Version);
            this.lblCopyright.Text = AboutBox.Copyright;
            this.txtDescription.Text = AboutBox.Description;

            this.lnkIconAuthor.Text = "Icons by Mark James";
            this.lnkIconAuthor.Links.Add(9, 10, "http://www.famfamfam.com/");
            this.linkLabel1.Text = "Special Thanks to kolby(Plexdata)";
            this.linkLabel1.Links.Add(18, 15, "http://www.plexdata.de");
        }

        private void Logo_Click(object sender, EventArgs args)
        {
            Process.Start("http://developerkp.capstricks.net/");
        }

        private void IconAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            args.Link.Visited = true;
            Process.Start(args.Link.LinkData.ToString());
        }

        #region Assembly Attribute Accessors

        public static string Title
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string Version
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return String.Format("{0}.{1} ({2})", version.Major, version.Minor, version.Build << 8 | (byte)version.Revision);
            }
        }

        public static string Description
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string Product
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string Copyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string Company
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion // Assembly Attribute Accessors 

        private void AboutBox_Load(object sender, EventArgs e)
        {

        }
    }
}
