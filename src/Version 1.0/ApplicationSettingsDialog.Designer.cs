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

namespace plexdata.$safeprojectname$
{
    partial class ApplicationSettingsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationSettingsDialog));
            this.ttHelper = new System.Windows.Forms.ToolTip(this.components);
            this.btnDefaults = new System.Windows.Forms.Button();
            this.btnImageBorderColor = new plexdata.Controls.ColorButton();
            this.btnCursorBKColor = new plexdata.Controls.ColorButton();
            this.btnViewFGColor = new plexdata.Controls.ColorButton();
            this.btnViewBKColor = new plexdata.Controls.ColorButton();
            this.btnExportPath = new System.Windows.Forms.Button();
            this.btnClearLastFilenames = new System.Windows.Forms.Button();
            this.chkAutoLoadLastFiles = new System.Windows.Forms.CheckBox();
            this.cmbBinLineLength = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.General = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.closeform = new System.Windows.Forms.CheckBox();
            this.overwrite = new System.Windows.Forms.CheckBox();
            this.loaddetemplate = new System.Windows.Forms.CheckBox();
            this.Misc = new System.Windows.Forms.TabPage();
            this.grpColors = new System.Windows.Forms.GroupBox();
            this.lblImageBorderColor = new System.Windows.Forms.Label();
            this.lblCursorBKColor = new System.Windows.Forms.Label();
            this.lblViewFGColor = new System.Windows.Forms.Label();
            this.lblViewBKColor = new System.Windows.Forms.Label();
            this.grpGeneral = new System.Windows.Forms.GroupBox();
            this.lblExportPath = new System.Windows.Forms.Label();
            this.lblClearLastFilenames = new System.Windows.Forms.Label();
            this.lblBinLineLength = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.advanceoption = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.General.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.Misc.SuspendLayout();
            this.grpColors.SuspendLayout();
            this.grpGeneral.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ttHelper
            // 
            this.ttHelper.AutoPopDelay = 10000;
            this.ttHelper.InitialDelay = 500;
            this.ttHelper.ReshowDelay = 100;
            // 
            // btnDefaults
            // 
            this.btnDefaults.Image = ((System.Drawing.Image)(resources.GetObject("btnDefaults.Image")));
            this.btnDefaults.Location = new System.Drawing.Point(14, 329);
            this.btnDefaults.Name = "btnDefaults";
            this.btnDefaults.Size = new System.Drawing.Size(75, 23);
            this.btnDefaults.TabIndex = 2;
            this.btnDefaults.Text = "&Defaults";
            this.btnDefaults.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ttHelper.SetToolTip(this.btnDefaults, "Restore default values for each option and reset currently used settings.");
            this.btnDefaults.UseVisualStyleBackColor = true;
            this.btnDefaults.Click += new System.EventHandler(this.DefaultsButton_Click);
            // 
            // btnImageBorderColor
            // 
            this.btnImageBorderColor.Border = true;
            this.btnImageBorderColor.Color = System.Drawing.Color.White;
            this.btnImageBorderColor.Location = new System.Drawing.Point(180, 106);
            this.btnImageBorderColor.Name = "btnImageBorderColor";
            this.btnImageBorderColor.Padding = new System.Windows.Forms.Padding(5);
            this.btnImageBorderColor.Size = new System.Drawing.Size(75, 23);
            this.btnImageBorderColor.TabIndex = 7;
            this.ttHelper.SetToolTip(this.btnImageBorderColor, "Choose a color to be used as border for displayed icons and cursors.");
            this.btnImageBorderColor.UseVisualStyleBackColor = true;
            // 
            // btnCursorBKColor
            // 
            this.btnCursorBKColor.Border = true;
            this.btnCursorBKColor.Color = System.Drawing.Color.White;
            this.btnCursorBKColor.Location = new System.Drawing.Point(180, 77);
            this.btnCursorBKColor.Name = "btnCursorBKColor";
            this.btnCursorBKColor.Padding = new System.Windows.Forms.Padding(5);
            this.btnCursorBKColor.Size = new System.Drawing.Size(75, 23);
            this.btnCursorBKColor.TabIndex = 5;
            this.ttHelper.SetToolTip(this.btnCursorBKColor, "Choose a color to be used as background for statically displayed cursors.");
            this.btnCursorBKColor.UseVisualStyleBackColor = true;
            // 
            // btnViewFGColor
            // 
            this.btnViewFGColor.Border = true;
            this.btnViewFGColor.Color = System.Drawing.Color.White;
            this.btnViewFGColor.Location = new System.Drawing.Point(180, 48);
            this.btnViewFGColor.Name = "btnViewFGColor";
            this.btnViewFGColor.Padding = new System.Windows.Forms.Padding(5);
            this.btnViewFGColor.Size = new System.Drawing.Size(75, 23);
            this.btnViewFGColor.TabIndex = 3;
            this.ttHelper.SetToolTip(this.btnViewFGColor, "Choose a color to be used as foreground (e.g. text color) for each data output sc" +
        "reen.");
            this.btnViewFGColor.UseVisualStyleBackColor = true;
            // 
            // btnViewBKColor
            // 
            this.btnViewBKColor.Border = true;
            this.btnViewBKColor.Color = System.Drawing.Color.White;
            this.btnViewBKColor.Location = new System.Drawing.Point(180, 19);
            this.btnViewBKColor.Name = "btnViewBKColor";
            this.btnViewBKColor.Padding = new System.Windows.Forms.Padding(5);
            this.btnViewBKColor.Size = new System.Drawing.Size(75, 23);
            this.btnViewBKColor.TabIndex = 1;
            this.ttHelper.SetToolTip(this.btnViewBKColor, "Choose a color to be used as background for each data output screen.");
            this.btnViewBKColor.UseVisualStyleBackColor = true;
            // 
            // btnExportPath
            // 
            this.btnExportPath.Image = global::plexdata.$safeprojectname$.Properties.Resources.delete;
            this.btnExportPath.Location = new System.Drawing.Point(180, 46);
            this.btnExportPath.Name = "btnExportPath";
            this.btnExportPath.Size = new System.Drawing.Size(25, 23);
            this.btnExportPath.TabIndex = 3;
            this.ttHelper.SetToolTip(this.btnExportPath, "Click this button to clear last known file export path.");
            this.btnExportPath.UseVisualStyleBackColor = true;
            // 
            // btnClearLastFilenames
            // 
            this.btnClearLastFilenames.Image = global::plexdata.$safeprojectname$.Properties.Resources.delete;
            this.btnClearLastFilenames.Location = new System.Drawing.Point(180, 75);
            this.btnClearLastFilenames.Name = "btnClearLastFilenames";
            this.btnClearLastFilenames.Size = new System.Drawing.Size(25, 23);
            this.btnClearLastFilenames.TabIndex = 5;
            this.ttHelper.SetToolTip(this.btnClearLastFilenames, resources.GetString("btnClearLastFilenames.ToolTip"));
            this.btnClearLastFilenames.UseVisualStyleBackColor = true;
            // 
            // chkAutoLoadLastFiles
            // 
            this.chkAutoLoadLastFiles.AutoSize = true;
            this.chkAutoLoadLastFiles.Location = new System.Drawing.Point(9, 104);
            this.chkAutoLoadLastFiles.Name = "chkAutoLoadLastFiles";
            this.chkAutoLoadLastFiles.Size = new System.Drawing.Size(157, 17);
            this.chkAutoLoadLastFiles.TabIndex = 6;
            this.chkAutoLoadLastFiles.Text = "Load last files &automatically.";
            this.ttHelper.SetToolTip(this.chkAutoLoadLastFiles, "Uncheck this option to avoid loading of last loaded files.");
            this.chkAutoLoadLastFiles.UseVisualStyleBackColor = true;
            this.chkAutoLoadLastFiles.CheckedChanged += new System.EventHandler(this.chkAutoLoadLastFiles_CheckedChanged);
            // 
            // cmbBinLineLength
            // 
            this.cmbBinLineLength.FormattingEnabled = true;
            this.cmbBinLineLength.Items.AddRange(new object[] {
            "8",
            "16",
            "32"});
            this.cmbBinLineLength.Location = new System.Drawing.Point(180, 19);
            this.cmbBinLineLength.Name = "cmbBinLineLength";
            this.cmbBinLineLength.Size = new System.Drawing.Size(75, 21);
            this.cmbBinLineLength.TabIndex = 1;
            this.ttHelper.SetToolTip(this.cmbBinLineLength, "Provide a number of bytes to be used as line length of binary output screens.");
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(200, 329);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.Location = new System.Drawing.Point(119, 329);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "&OK";
            this.btnOk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Controls.Add(this.Misc);
            this.tabControl1.Location = new System.Drawing.Point(3, -1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(294, 324);
            this.tabControl1.TabIndex = 5;
            // 
            // General
            // 
            this.General.Controls.Add(this.groupBox2);
            this.General.Controls.Add(this.groupBox1);
            this.General.Location = new System.Drawing.Point(4, 22);
            this.General.Name = "General";
            this.General.Padding = new System.Windows.Forms.Padding(3);
            this.General.Size = new System.Drawing.Size(286, 298);
            this.General.TabIndex = 0;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.closeform);
            this.groupBox1.Controls.Add(this.overwrite);
            this.groupBox1.Controls.Add(this.loaddetemplate);
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 97);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // closeform
            // 
            this.closeform.AutoSize = true;
            this.closeform.Location = new System.Drawing.Point(6, 65);
            this.closeform.Name = "closeform";
            this.closeform.Size = new System.Drawing.Size(191, 17);
            this.closeform.TabIndex = 2;
            this.closeform.Text = "Close Form After Process Complete";
            this.closeform.UseVisualStyleBackColor = true;
            // 
            // overwrite
            // 
            this.overwrite.AutoSize = true;
            this.overwrite.Location = new System.Drawing.Point(6, 42);
            this.overwrite.Name = "overwrite";
            this.overwrite.Size = new System.Drawing.Size(155, 17);
            this.overwrite.TabIndex = 1;
            this.overwrite.Text = "Overwrite Existing Backups";
            this.overwrite.UseVisualStyleBackColor = true;
            // 
            // loaddetemplate
            // 
            this.loaddetemplate.AutoSize = true;
            this.loaddetemplate.Checked = true;
            this.loaddetemplate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loaddetemplate.Location = new System.Drawing.Point(6, 19);
            this.loaddetemplate.Name = "loaddetemplate";
            this.loaddetemplate.Size = new System.Drawing.Size(220, 17);
            this.loaddetemplate.TabIndex = 0;
            this.loaddetemplate.Text = "Always Load a Default Template in Editor";
            this.loaddetemplate.UseVisualStyleBackColor = true;
            this.loaddetemplate.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Misc
            // 
            this.Misc.Controls.Add(this.grpColors);
            this.Misc.Controls.Add(this.grpGeneral);
            this.Misc.Location = new System.Drawing.Point(4, 22);
            this.Misc.Name = "Misc";
            this.Misc.Padding = new System.Windows.Forms.Padding(3);
            this.Misc.Size = new System.Drawing.Size(286, 298);
            this.Misc.TabIndex = 1;
            this.Misc.Text = "Misc";
            this.Misc.UseVisualStyleBackColor = true;
            // 
            // grpColors
            // 
            this.grpColors.Controls.Add(this.lblImageBorderColor);
            this.grpColors.Controls.Add(this.btnImageBorderColor);
            this.grpColors.Controls.Add(this.lblCursorBKColor);
            this.grpColors.Controls.Add(this.lblViewFGColor);
            this.grpColors.Controls.Add(this.lblViewBKColor);
            this.grpColors.Controls.Add(this.btnCursorBKColor);
            this.grpColors.Controls.Add(this.btnViewFGColor);
            this.grpColors.Controls.Add(this.btnViewBKColor);
            this.grpColors.Location = new System.Drawing.Point(13, 139);
            this.grpColors.Name = "grpColors";
            this.grpColors.Size = new System.Drawing.Size(261, 135);
            this.grpColors.TabIndex = 3;
            this.grpColors.TabStop = false;
            // 
            // lblImageBorderColor
            // 
            this.lblImageBorderColor.AutoSize = true;
            this.lblImageBorderColor.Location = new System.Drawing.Point(6, 111);
            this.lblImageBorderColor.Name = "lblImageBorderColor";
            this.lblImageBorderColor.Size = new System.Drawing.Size(137, 13);
            this.lblImageBorderColor.TabIndex = 6;
            this.lblImageBorderColor.Text = "&General Image Border Color";
            // 
            // lblCursorBKColor
            // 
            this.lblCursorBKColor.AutoSize = true;
            this.lblCursorBKColor.Location = new System.Drawing.Point(6, 82);
            this.lblCursorBKColor.Name = "lblCursorBKColor";
            this.lblCursorBKColor.Size = new System.Drawing.Size(157, 13);
            this.lblCursorBKColor.TabIndex = 4;
            this.lblCursorBKColor.Text = "&Cursor Image Background Color";
            // 
            // lblViewFGColor
            // 
            this.lblViewFGColor.AutoSize = true;
            this.lblViewFGColor.Location = new System.Drawing.Point(6, 53);
            this.lblViewFGColor.Name = "lblViewFGColor";
            this.lblViewFGColor.Size = new System.Drawing.Size(140, 13);
            this.lblViewFGColor.TabIndex = 2;
            this.lblViewFGColor.Text = "Data View &Foreground Color";
            // 
            // lblViewBKColor
            // 
            this.lblViewBKColor.AutoSize = true;
            this.lblViewBKColor.Location = new System.Drawing.Point(6, 24);
            this.lblViewBKColor.Name = "lblViewBKColor";
            this.lblViewBKColor.Size = new System.Drawing.Size(144, 13);
            this.lblViewBKColor.TabIndex = 0;
            this.lblViewBKColor.Text = "Data View &Background Color";
            // 
            // grpGeneral
            // 
            this.grpGeneral.Controls.Add(this.btnExportPath);
            this.grpGeneral.Controls.Add(this.btnClearLastFilenames);
            this.grpGeneral.Controls.Add(this.lblExportPath);
            this.grpGeneral.Controls.Add(this.lblClearLastFilenames);
            this.grpGeneral.Controls.Add(this.chkAutoLoadLastFiles);
            this.grpGeneral.Controls.Add(this.cmbBinLineLength);
            this.grpGeneral.Controls.Add(this.lblBinLineLength);
            this.grpGeneral.Location = new System.Drawing.Point(13, 6);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(261, 127);
            this.grpGeneral.TabIndex = 2;
            this.grpGeneral.TabStop = false;
            // 
            // lblExportPath
            // 
            this.lblExportPath.AutoSize = true;
            this.lblExportPath.Location = new System.Drawing.Point(6, 51);
            this.lblExportPath.Name = "lblExportPath";
            this.lblExportPath.Size = new System.Drawing.Size(109, 13);
            this.lblExportPath.TabIndex = 2;
            this.lblExportPath.Text = "Clear last e&xport path.";
            // 
            // lblClearLastFilenames
            // 
            this.lblClearLastFilenames.AutoSize = true;
            this.lblClearLastFilenames.Location = new System.Drawing.Point(6, 80);
            this.lblClearLastFilenames.Name = "lblClearLastFilenames";
            this.lblClearLastFilenames.Size = new System.Drawing.Size(130, 13);
            this.lblClearLastFilenames.TabIndex = 4;
            this.lblClearLastFilenames.Text = "Clear list of last &used Files.";
            // 
            // lblBinLineLength
            // 
            this.lblBinLineLength.AutoSize = true;
            this.lblBinLineLength.Location = new System.Drawing.Point(6, 22);
            this.lblBinLineLength.Name = "lblBinLineLength";
            this.lblBinLineLength.Size = new System.Drawing.Size(121, 13);
            this.lblBinLineLength.TabIndex = 0;
            this.lblBinLineLength.Text = "Binary Data &Line Length";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.advanceoption);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(6, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(273, 186);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(256, 26);
            this.label3.TabIndex = 5;
            this.label3.Text = "Note: I am not here to provide pirated things, I found \r\nthis way to trick it whi" +
    "ch is legal so presenting it here.";
            // 
            // advanceoption
            // 
            this.advanceoption.AutoSize = true;
            this.advanceoption.Checked = true;
            this.advanceoption.CheckState = System.Windows.Forms.CheckState.Checked;
            this.advanceoption.Location = new System.Drawing.Point(9, 163);
            this.advanceoption.Name = "advanceoption";
            this.advanceoption.Size = new System.Drawing.Size(152, 17);
            this.advanceoption.TabIndex = 3;
            this.advanceoption.Text = "Show Options Related to it";
            this.advanceoption.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(264, 26);
            this.label2.TabIndex = 4;
            this.label2.Text = "All Infos Provided in the Help Topics, access it from F1\r\nor Help > Topics";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 88);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Open Topics";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "At First you need to configure the res console tuner,\r\nbefore using this Applicat" +
    "ion.";
            // 
            // ApplicationSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(298, 359);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnDefaults);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplicationSettingsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.ApplicationSettingsDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.General.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Misc.ResumeLayout(false);
            this.grpColors.ResumeLayout(false);
            this.grpColors.PerformLayout();
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnDefaults;
        private System.Windows.Forms.ToolTip ttHelper;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage General;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage Misc;
        private System.Windows.Forms.Label lblBinLineLength;
        private System.Windows.Forms.ComboBox cmbBinLineLength;
        private System.Windows.Forms.CheckBox chkAutoLoadLastFiles;
        private System.Windows.Forms.Label lblClearLastFilenames;
        private System.Windows.Forms.Label lblExportPath;
        private System.Windows.Forms.Button btnClearLastFilenames;
        private System.Windows.Forms.Button btnExportPath;
        private System.Windows.Forms.GroupBox grpGeneral;
        private Controls.ColorButton btnViewBKColor;
        private Controls.ColorButton btnViewFGColor;
        private Controls.ColorButton btnCursorBKColor;
        private System.Windows.Forms.Label lblViewBKColor;
        private System.Windows.Forms.Label lblViewFGColor;
        private System.Windows.Forms.Label lblCursorBKColor;
        private Controls.ColorButton btnImageBorderColor;
        private System.Windows.Forms.Label lblImageBorderColor;
        private System.Windows.Forms.GroupBox grpColors;
        public System.Windows.Forms.CheckBox loaddetemplate;
        public System.Windows.Forms.CheckBox closeform;
        public System.Windows.Forms.CheckBox overwrite;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox advanceoption;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}