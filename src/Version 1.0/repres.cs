using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace plexdata.$safeprojectname$
{
    public partial class repres : Form
    {
        MainForm ths;
        public repres(MainForm frm)
        {
            InitializeComponent();
            ths = frm;
        }
        public string type, name, lang, resfile;
        private void repres_Load(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
                      defineconst();
            ths.unloadres();
            ths.Delay(1);
            if (textBox3.Text == "")
            {
                ths.Label1.Text = "Updating Whole Category";
                ths.Delay(1);
                ths.runstuff("C:\\temp\\res.exe", "-op:upd -src:" + "\"" + ths.filepath + "\"" + " -type:" + type + " -name:" + name + " -file:" + "\"" + resfile + "\"");
              }
          
            {
                ths.Label1.Text = "Updating Selected";
                ths.Delay(1);
                ths.runstuff("C:\\temp\\res.exe", "-op:upd -src:" + "\"" + ths.filepath + "\"" + " -type:" + type + " -name:" + name + " -lang:" + lang + " -file:" + "\"" + resfile + "\"");

            }
            ths.Delay(1);
            ths.loadres();
            if (ths.closeform == true)
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;
            }
                    }
        public void defineconst()
        {
            type = textBox1.Text;
            name = textBox2.Text;
            lang = textBox3.Text;
            resfile = "\"" + textBox1.Text + "\"";
        }
    }
}
