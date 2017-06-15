using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using plexdata.$safeprojectname$;
using plexdata.Controls;
using plexdata.Utilities.Resources;
using MsgBox;
namespace plexdata.$safeprojectname$
{
      public partial class Create : Form
    {
        MainForm ths;
        String path;
        public Create(MainForm frm)
        {
            InitializeComponent();
            ths = frm;
                    }

        private void Create_Load(object sender, EventArgs e)
        {
           
            timer1.Start();
            this.Text = ths.Functions1;
            if (ths.Functions4 == "Custom")
            {
                Editor1.Language = FastColoredTextBoxNS.Language.Custom;
            }
            else if (ths.Functions4 == "HTML")
            {
                toolStripButton5.Enabled = true;
                Editor1.Language = FastColoredTextBoxNS.Language.HTML;
            }
            else if (ths.Functions4 == "XML")
            {
                Editor1.Language = FastColoredTextBoxNS.Language.XML;
            }else if (ths.Functions4 == "Other")
            {
                toolStripComboBox1.Enabled = true;
                toolStripComboBox1.Visible = true;
                toolStripButton5.Enabled = true;
                Editor1.Language = FastColoredTextBoxNS.Language.HTML;
            }
            Label1.Text = "Default template Loaded";
            Editor1.Text = ths.Functions3;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (path != null)
            {
                Editor1.Text = "";

            }
            else
            {
                var result = MessageBox.Show("Your template is not Saved!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                if (result == DialogResult.OK)
                {
                    nsavefile1.FileName = "";
                    nsavefile1.Filter = ths.Functions2;
                    if (nsavefile1.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(nsavefile1.FileName, Editor1.Text);
                        path = nsavefile1.FileName;
                        Label1.Text = "File Exported";
                        Editor1.Text = "";
                    }
                  
                } 
            }
            path = "";
            toolStripButton2.Image = plexdata.$safeprojectname$.Properties.Resources.unsave;
        }
       
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (path != null) {
                System.IO.File.WriteAllText(path, Editor1.Text);
                toolStripButton2.Image = plexdata.$safeprojectname$.Properties.Resources.save;
            } else
            {
                nsavefile1.FileName = "";
                nsavefile1.Filter = ths.Functions2;
                if (nsavefile1.ShowDialog() == DialogResult.OK)
                {
                    toolStripButton2.Image = plexdata.$safeprojectname$.Properties.Resources.save;
                    System.IO.File.WriteAllText(nsavefile1.FileName, Editor1.Text);
                    path = nsavefile1.FileName;
                    Label1.Text = "File Exported";
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Label1.Text = "Ready";
        }

        private void Editor1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "All Files|*.*";
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                Editor1.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
                path = openFileDialog1.FileName;
                Label1.Text = "File Captured";
                toolStripButton2.Image = plexdata.$safeprojectname$.Properties.Resources.save;
            }
                
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            nsavefile1.FileName = "";
            nsavefile1.Filter = ths.Functions2;
            if (nsavefile1.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(nsavefile1.FileName, Editor1.Text);
                path = nsavefile1.FileName;
                Label1.Text = "File Exported";
                toolStripButton2.Image = plexdata.$safeprojectname$.Properties.Resources.save;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            String test = Editor1.Text;
            Editor1.Text = "";
            if (ths.Functions4 == "Custom")
            {

                Editor1.Language = FastColoredTextBoxNS.Language.Custom;
            }
            else if (ths.Functions4 == "HTML")
            {
                toolStripButton7.Enabled = false;
                toolStripButton5.Enabled = true;
                Editor1.Language = FastColoredTextBoxNS.Language.HTML;
            }
            else if (ths.Functions4 == "MANIFEST")
            {
                toolStripButton7.Enabled = true;
                Editor1.Language = FastColoredTextBoxNS.Language.XML;
            }
            Editor1.Text = ths.Functions3;
          
            if (toolStripComboBox1.Text == "MANIFEST Script")
            {
                toolStripButton7.Enabled = true;
                Label1.Text = "Manifest Editor";
                toolStripButton5.Enabled = false;
                Editor1.Language = FastColoredTextBoxNS.Language.XML;
            }
            else if (toolStripComboBox1.Text == "HTML Script")
            {
                toolStripButton7.Enabled = false;
                Label1.Text = "HTML Editor";
                toolStripButton5.Enabled = true;
                Editor1.Language = FastColoredTextBoxNS.Language.HTML;
            }
            else if (toolStripComboBox1.Text == "XML Script")
            {
                toolStripButton7.Enabled = false;
                Label1.Text = "XML Editor";
                toolStripButton5.Enabled = false;
                Editor1.Language = FastColoredTextBoxNS.Language.XML;
            }
            else if (toolStripComboBox1.Text == "INI Script")
            {
                toolStripButton7.Enabled = false;
                Label1.Text = "INI Editor";
                toolStripButton5.Enabled = false;
                Editor1.Language = FastColoredTextBoxNS.Language.Custom;
            }
            else if (toolStripComboBox1.Text == "Other")
            {
                toolStripButton7.Enabled = false;
                Label1.Text = "You Set it to Custom";
                toolStripButton5.Enabled = false;
                Editor1.Language = FastColoredTextBoxNS.Language.Custom;
            }
            Editor1.Text = test;
        
    }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText("C:\\temp\\myFile.htm", Editor1.Text);
            Process.Start("IExplore.exe", "C:\\temp\\myFile.htm");
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
           
        }

        private void Create_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (path != null)
            {
                Editor1.Text = "";

            }
            else
            {
                var result = MessageBox.Show("Your template is not Saved!", "" ,MessageBoxButtons.OK,MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    Editor1.Text = "";
                } else
                {
                    if (path != null)
                    {
                        System.IO.File.WriteAllText(path, Editor1.Text);
                    }
                    else
                    {
                        nsavefile1.FileName = "";
                        nsavefile1.Filter = ths.Functions2;
                        if (nsavefile1.ShowDialog() == DialogResult.OK)
                        {
                            System.IO.File.WriteAllText(nsavefile1.FileName, Editor1.Text);
                            path = nsavefile1.FileName;
                            Label1.Text = "File Exported";
                        }
                    }
                }
               
            }
        }
        public void check1affliate()
        {
            timer1.Stop();
            Label1.Text = "Getting Resources";
            ths.trvResources.CloseRecource();
            ths.Delay(1);
            if (System.IO.File.Exists("C:\\temp\\SRC\\temp.exe"))
            {
                System.IO.File.Delete("C:\\temp\\SRC\\temp.exe");
                          }
                            System.IO.File.Copy(filepath, "C:\\temp\\SRC\\temp.exe");
                             
                  }
        public void workdoneaffliate()
        {
            Label1.Text = "Finalising Stuff";
            if (System.IO.File.Exists("C:\\temp\\RELEASE\\temp-new.exe"))
            {
                System.IO.File.Delete(filepath);
                System.IO.File.Move("C:\\temp\\RELEASE\\temp-new.exe", filepath);
            }
            else
            {

            }
           
        }
       
        public void runstuff(String Path, String Arguments)
        {
            Label1.Text = "Running Stuffs";
                     Process p = new Process();
            p.StartInfo.FileName = Path;
            p.StartInfo.Arguments = Arguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
        }
        public string filepath, resname;

        private void Editor1_TextChanging(object sender, FastColoredTextBoxNS.TextChangingEventArgs e)
        {
            toolStripButton2.Image = plexdata.$safeprojectname$.Properties.Resources.unsave;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (ths.advance == false)
            {
                
            filepath = ths.Functions5;
            System.IO.File.WriteAllText("C:\\temp\\SRC\\manifest.xml", Editor1.Text);
            ths.Delay(1);
           resname = "manifest.xml";
               check1affliate();
               runstuff("C:\\temp\\edit_manifest.bat", "");
               workdoneaffliate();
            ths.filepath = filepath;
            ths.loadres();
            Label1.Text = "Compiled SuccessFully";
            timer1.Start();
            }
            else
            {
                Label1.Text = "Cannot Compile Manifest!";
            }
        }
    }
}
