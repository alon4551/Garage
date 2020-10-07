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
    public partial class MainWindow : Form
    {
        public People_main people_Main = new People_main();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load_People()
        {
            Profile_List.Items.Clear();
            foreach (string name in people_Main.getPeopleNames())
                Profile_List.Items.Add(name);
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            Load_People();
        }

        private void Profile_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            people_Main.id = people_Main.People[Profile_List.SelectedIndex].GetColValue("id").ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Profile_List.SelectedIndex!=-1)
            {
                using(var window =new UserWizzard())
                {
                    UserProfile.setUserId(int.Parse(people_Main.id));
                    window.ShowDialog();

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (people_Main.remove_user())
            {
                Assets.Refresh();
                MessageBox.Show("משתמש נחמק");
            }
        }
    }
}
