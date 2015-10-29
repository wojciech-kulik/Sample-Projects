using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmbeddedLibraries
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnMethod1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(new Library1.Lib1().Method());
        }

        private void btnMethod2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(new Library2.Lib2().Method());
        }
    }
}
