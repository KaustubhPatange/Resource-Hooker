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
using System.Windows.Forms;

using plexdata.Controls;

namespace plexdata.$safeprojectname$
{
   
    public partial class ApplicationSettingsDialog : Form
    {
       
        private Settings settings = null;

        public ApplicationSettingsDialog()
        {
          
            this.InitializeComponent();
            this.Icon = Program.GetFormIcon();
            this.settings = (Settings)ApplicationSettings.Instance.Settings.Clone();
        }

        private void ApplicationSettingsDialog_Load(object sender, EventArgs args)
        {
            // General settings
            this.cmbBinLineLength.Text = this.settings.BinaryLineLength.ToString();
            this.btnExportPath.Enabled = !String.IsNullOrEmpty(this.settings.ExportPath);
            this.lblExportPath.Enabled = this.btnExportPath.Enabled;
            this.btnClearLastFilenames.Enabled = (this.settings.LastFilenames.Length > 0);
            this.lblClearLastFilenames.Enabled = this.btnClearLastFilenames.Enabled;
            this.chkAutoLoadLastFiles.Checked = this.settings.AutoLoadLastFiles;
            this.loaddetemplate.Checked = this.settings.LoadTemplate;
            this.overwrite.Checked = this.settings.Overwrite;
            this.advanceoption.Checked = this.settings.ShowAdvanceOptions;
            this.closeform.Checked = this.settings.CloseForm;
            // Color settings
            this.btnViewFGColor.Color = this.settings.GetViewForeground();
            this.btnViewBKColor.Color = this.settings.GetViewBackground();
            this.btnCursorBKColor.Color = this.settings.GetCursorBackground();
            this.btnImageBorderColor.Color = this.settings.GetImageBorderColor();

            this.Update();
        }

        private void OkButton_Click(object sender, EventArgs args)
        {
            int length = 0;
            if (int.TryParse(this.cmbBinLineLength.Text, out length))
            {
                this.settings.BinaryLineLength = length;
            }
            else
            {
                // Handle error if wanted (e.g. avoid dialg closing)...
            }

            this.settings.AutoLoadLastFiles = this.chkAutoLoadLastFiles.Checked;
            this.settings.LoadTemplate = this.loaddetemplate.Checked;
            this.settings.Overwrite = this.overwrite.Checked;
            this.settings.ShowAdvanceOptions = this.advanceoption.Checked;
            this.settings.CloseForm = this.closeform.Checked;
            this.settings.SetViewForeground(this.btnViewFGColor.Color);
            this.settings.SetViewBackground(this.btnViewBKColor.Color);
            this.settings.SetCursorBackground(this.btnCursorBKColor.Color);
            this.settings.SetImageBorderColor(this.btnImageBorderColor.Color);
          

            ApplicationSettings.Instance.Settings.FromClone(this.settings);
        }

        private void DefaultsButton_Click(object sender, EventArgs args)
        {
            this.settings.ResetDefaults();

            this.cmbBinLineLength.Text = this.settings.BinaryLineLength.ToString();
            this.chkAutoLoadLastFiles.Checked = this.settings.AutoLoadLastFiles;
            this.loaddetemplate.Checked = this.settings.LoadTemplate;
            this.overwrite.Checked = this.settings.Overwrite;
            this.advanceoption.Checked = this.settings.ShowAdvanceOptions;
            this.closeform.Checked = this.settings.CloseForm;
            this.btnViewFGColor.Color = this.settings.GetViewForeground();
            this.btnViewBKColor.Color = this.settings.GetViewBackground();
            this.btnCursorBKColor.Color = this.settings.GetCursorBackground();
            this.btnImageBorderColor.Color = this.settings.GetImageBorderColor();
        }

        private void ClearExportPathButton_Click(object sender, EventArgs args)
        {
            this.settings.ExportPath = null;
            this.btnExportPath.Enabled = !String.IsNullOrEmpty(this.settings.ExportPath);
            this.lblExportPath.Enabled = this.btnExportPath.Enabled;
        }

        private void ClearLastFilenamesButton_Click(object sender, EventArgs args)
        {
            this.settings.LastFilenames = null;
            this.btnClearLastFilenames.Enabled = (this.settings.LastFilenames.Length > 0);
            this.lblClearLastFilenames.Enabled = this.btnClearLastFilenames.Enabled;
        }

        private void ColorButton_Click(object sender, EventArgs args)
        {
            ColorButton button = (sender as ColorButton);
            if (button != null)
            {
                ColorDialog dialog = new ColorDialog();
                dialog.Color = button.Color;
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    button.Color = dialog.Color;
                }
            }
        }

        private void chkAutoLoadLastFiles_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        private void groupBox2_Enter_1(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

     
    }
}
