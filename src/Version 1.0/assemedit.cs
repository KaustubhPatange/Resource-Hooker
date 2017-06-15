using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace plexdata.$safeprojectname$
{
    public partial class assemedit : Form
    {
        MainForm ths;
        public assemedit(MainForm frm)
        {
            InitializeComponent();
            ths = frm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "New Application";
            textBox2.Text = "New Product";
            textBox3.Text = "Test Application";
            textBox4.Text = "My Company, Inc.";
            textBox5.Text = "Copyright \0xA9 2014 My Company, Inc.";
            textBox6.Text = "Country, City";
            textBox7.Text = "2.5.1 Demo";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ths.Label1.Text = "Writing Values";
            File.WriteAllText("C:\\temp\\edit_version.rts", File.ReadAllText("C:\\temp\\edit_version.rts").Replace("YOURPRODUCTNAME", textBox1.Text).Replace("YOURINTERNALNAME", textBox2.Text).Replace("YOURFILEDESCRIPTION", textBox3.Text).Replace("YOURCOMPANYNAME", textBox4.Text).Replace("YOURCOPYRIGHTINFO", textBox5.Text).Replace("YOURTRADEMARKSHERE", textBox6.Text).Replace("YOURPRODUCTVERSION", textBox7.Text));
            ths.Delay(1);
            ths.check2affliate();
            ths.runstuff("C:\\temp\\edit_version.bat", "");
            ths.workdoneaffliate();
            ths.loadres();
            if (ths.closeform == true)
            {
                this.Close();
            }
        }
    }
}
