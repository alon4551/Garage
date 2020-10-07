using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Garage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Assets.Refresh();
        }

        private void Login_Click(object sender, EventArgs e)
        {
            using (var window =new MainWindow())
            {
                window.ShowDialog();
            }
        }
    }
}
