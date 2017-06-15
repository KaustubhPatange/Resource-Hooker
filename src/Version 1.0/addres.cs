using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace plexdata.$safeprojectname$
{
    public partial class addres : Form
    {
        MainForm ths;
        public addres(MainForm frm)
        {
            InitializeComponent();
            ths = frm;
        }
        public string lang, type, resfile, name;
               private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                comboBox1.Enabled = false;
                textBox1.Enabled = true;
            } else
            {
                comboBox1.Enabled = true;
                textBox1.Enabled = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                numericUpDown1.Enabled = true;
                textBox3.Enabled = false;
            } 
          
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                numericUpDown1.Enabled = false;
                textBox3.Enabled = true;
            }
        }

        private void addres_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void combolist2()
        {
            switch (comboBox2.Text)
            {
                case "Accelerator (9)":
                    type = "9";
                    break;
                case "Bitmap (2)":
                    type = "2";
                    break;
                case "Cursor Animated (21)":
                    type = "21";
                                        break;
                case "Cursor Directory (12)":
                    type = "12";
                    break;
                case "Cursor Image (1)":
                    type = "1";
                    break;
                case "Dialog (5)":
                    type = "5";
                    break;
                case "DLG Included (17)":
                    type = "17";
                    break;
                case "Font (8)":
                    type = "8";
                    break;
                case "Font Directory (7)":
                    type = "7";
                    break;
                case "HTML (23)":
                    type = "23";
                    break;
                case "Icon Animated (22)":
                    type = "22";
                    break;
                case "Icon Directory (14)":
                    type = "14";
                    break;
                case "Icon Image (3)":
                    type = "3";
                    break;
                case "Manifest (14)":
                    type = "14";
                    break;
                case "Menu (4)":
                    type = "4";
                    break;
                case "Message Table (11)":
                    type = "11";
                    break;
                case "RC Data (10)":
                    type = "10";
                    break;
                case "String Table (6)":
                    type = "6";
                    break;
                case "Toolbar (24)":
                    type = "24";
                    break;
                case "Version (16)":
                    type = "16";
                    break;
                case "VXD (20)":
                    type = "20";
                    break;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        public void combolist1()
        {
            if (checkBox1.Checked != true) {
                switch (comboBox1.Text)
                {
                    case "English (United States) (1033)":
                        lang = "1033";
                        break;
                    case "Spanish (United States) (21514)":
                        lang = "21514";
                        break;
                    case "Russian (25)":
                        lang = "25";
                        break;
                    case "Neutral (0) (Preffered)":
                        lang = "0";
                        break;
                } 
            } else
            {
                lang = textBox1.Text;
            }
        }
        public void resnameid()
        {
            if (radioButton1.Checked == true)
            {
                name = Convert.ToString(numericUpDown1.Value);
            } else if (radioButton2.Checked == true)
            {
                name = textBox3.Text;
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            resfile = textBox2.Text;
            combolist1();
            combolist2();
            resnameid();
            ths.unloadres();
            ths.Delay(1);
            ths.runstuff("C:\\temp\\res.exe", "-op:add -src:" + "\"" + ths.filepath + "\""  + " -type:" + type + " -name:"+ name  +" -lang:"+ lang +  " -file:" + "\"" + resfile + "\"");
            ths.Delay(1);
            ths.loadres();
        if (ths.closeform== true)
            {
                this.Close();
            }
        }
    }
}
