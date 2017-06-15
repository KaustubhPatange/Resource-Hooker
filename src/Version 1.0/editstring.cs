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
    public partial class editstring : Form
    {
        MainForm ths;
        public editstring(MainForm frm)
                    {
            
            InitializeComponent();
            ths = frm;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox1.Text + "=" + textBox2.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
        public void saveitemstofile()
        {
            if (System.IO.File.Exists("C:\\temp\\SRC\\strings.txt"))
            {
                // System.IO.File.WriteAllText("C:\\temp\\test.txt", listBox1.Text);
                System.IO.File.Delete("C:\\temp\\SRC\\strings.txt");
            }
            System.IO.StreamWriter SaveFile = new System.IO.StreamWriter("C:\\temp\\SRC\\strings.txt");
            foreach (var item in listBox1.Items)
            {
                SaveFile.WriteLine(item.ToString());
            }
            SaveFile.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            saveitemstofile();
            ths.Delay(1);
            ths.check2affliate();
            ths.runstuff("C:\\temp\\edit_strings.bat", "");
            ths.workdoneaffliate();
            ths.loadres();
            if (ths.closeform == true)
            {
                this.Close();
            }
        }
    }
}
