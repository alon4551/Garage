using Garage.sql_helper;
using Garage.Treatment;
using Garage.Worker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
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
        //loading data to screens
        private void Load_People()
        {
            Load_List(Profile_List, people_Main.getPeopleNames());
        }
        private void Load_Register_Manufactors()
        {
            Load_List(Register_manufactor, Assets.getManufactorNames());
        }
        private void Load_car()
        {
            Load_List(Register_car_list, Register_utiles.getCarNumbers());
        }
        private void Load_Register()
        {
            Load_Register_Manufactors();
            Load_car();
        }
        private void Load_Register_Models()
        {
            Load_List(Register_model, Register_utiles.getModelsNames());
        }
        private void Load_List(ComboBox box, List<string> names)
        {
            box.Items.Clear();
            foreach (string name in names)
                box.Items.Add(name);
        }
        private void Load_List(ListBox box, List<string> names)
        {
            box.Items.Clear();
            foreach (string name in names)
                box.Items.Add(name);
        }
        private void Load_List(CheckedListBox box, List<string> names)
        {
            box.Items.Clear();
            foreach (string name in names)
                box.Items.Add(name);
        }
        private void Load_Treatments()
        {
            Load_List(Treatment_List, Treatment_utiles.getNames());
        }
        private void Load_Workers()
        {
            Load_List(Worker_List, Worker_utiles.getWorkerNames());
            Load_List(Worker_checkbox, Worker_utiles.getWorkerNames());

        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            Load_People();
            Load_Workers();
            Load_Register();
            Load_Treatments();
        }
        //finish loading data to screen

        //Customers
        private void Profile_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            switch (box.Name)
            {
                case "Profile_List":
                    {
                        people_Main.id = people_Main.People[box.SelectedIndex].GetColValue("id").ToString();

                        break;
                    }
                case "Worker_List":
                    {
                        people_Main.id = Worker_utiles.workers[box.SelectedIndex].GetColValue("id").ToString();
                        break;
                    }
                default:
                    {
                        people_Main.id = "";
                        break;
                    }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(people_Main.id!="")
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

                Refresh_Screen();
                MessageBox.Show("משתמש נחמק");
            }
        }
        //customers
        //register code
        private void Register_Cancel(object sender, EventArgs e)
        {
            //clear layout
        }
        private void Clear_Car_Register_Form()
        {
            Register_car_number.Text = "";
            Register_model.SelectedIndex = -1;
            Register_manufactor.SelectedIndex = -1;
            Register_kilometer.Text = "";
        }
        private void Register_Finish(object sender, EventArgs e)
        {
            List<object> values = new List<object>();
            if (Wizzard_Helper.inputValidator(Register_Profile_Page))
            {
                try
                {
                    values.Add(int.Parse(Register_id.Text));
                    values.Add(Register_firstname.Text);
                    values.Add(Register_lastname.Text);
                    values.Add(Register_birthady.Value.ToShortDateString());
                    values.Add(Register_gender.Text);
                    values.Add(Register_phone.Text);
                    values.Add(Register_email.Text);
                    if (Register_utiles.InsertPerson(values) && Register_utiles.InsertCar())
                    {
                        Refresh_Screen();
                        Clear_Car_Register_Form();
                        MessageBox.Show("Inserted");
                    }
                    else
                    {
                        MessageBox.Show("not Inserted");

                    }
                    }
                catch
                {

                }
            }
            else
                MessageBox.Show("not ok");
            //register account
        }
        private List<object> GetCarData()
        {
            List<object> car = new List<object>();
            try
            {
                car.Add(int.Parse(Register_car_number.Text));
                car.Add(Register_id.Text);
                car.Add(Register_utiles.getModelId(Register_model.SelectedIndex));
                car.Add(Register_kilometer.Text);
                return car;
            }
            catch
            {
                return null;
            }
        }
        
        private void Register_add_car_Click(object sender, EventArgs e)
        {
            if(Register_id.Text=="")
            {
                MessageBox.Show("אנא הכנס תעודת זהות בחלון הקודם");
                return;
            }
            if(Wizzard_Helper.inputValidator(Register_car_deatiles))
            {
                if (Register_utiles.ExsistCar(int.Parse(Register_car_number.Text)) == false)
                {
                    Register_utiles.AddCar(GetCarData());
                    Register_car_list.SelectedIndex = -1;
                    Load_car();
                    Clear_Car_Register_Form();
                }
                else
                    MessageBox.Show("מספר מכונית קיים במערכת");

            }   
        }

        private void Register_manufactor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Register_manufactor.SelectedIndex == -1)
                return;
            try
            {
                Register_utiles.SetManufactor(Register_manufactor.SelectedIndex);
                Register_model.Enabled = true;
                Load_Register_Models();
            }
            catch
            {
                Register_model.SelectedIndex = -1;
                Register_model.Enabled = false;
                Register_model.Items.Clear();
                
            }
        }

        private void Register_car_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Register_car_list.SelectedIndex == -1)
                return;
           List<object>car= Register_utiles.GetCar(Register_car_list.SelectedIndex);
            try
            {
                Register_car_number.Text = car[0].ToString();
                Register_manufactor.SelectedIndex = Register_utiles.getManufactorIndex((int)car[2]);
                Register_model.SelectedIndex = Register_utiles.getModelIndex((int)car[2]);
                Register_kilometer.Text = car[3].ToString();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Register_update_car_Click(object sender, EventArgs e)
        {
            if (Wizzard_Helper.inputValidator(Register_car_deatiles)&& Register_car_list.SelectedIndex != -1)
            {
                Register_utiles.updateCar(GetCarData(), Register_car_list.SelectedIndex);
                Register_car_list.SelectedIndex = -1;
                Load_car();
            }
        }

        private void Register_delete_car_Click(object sender, EventArgs e)
        {
            if (Register_car_list.SelectedIndex != -1)
            {
                Register_utiles.RemoveCar(Register_car_list.SelectedIndex);
                Register_car_list.SelectedIndex = -1;
                Load_car();
            }
        }
        private void CheckBoxEnable(object sender,EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            switch (box.Name)
            {
                case "Worker_Date_Select":
                    {
                        mouthPicker.Enabled = box.Enabled;
                        Search_Click(null, null);
                        break;
                    }
                 }
        }
      
        private void Customers_Page_Paint(object sender, PaintEventArgs e)
        {
            people_Main.id = "";

        }
        private void Load_Shifts(List<SQL_Table> workers)
        {
            Shifts.Series.Clear();
            foreach(SQL_Table worker in workers)
            {
                Shifts.Series.Add(worker.Name);
                Shifts.Series[worker.Name].XValueMember = "Month_name";
                Shifts.Series[worker.Name].YValueMembers = "Hours";
                foreach (Row shift in worker.Rows)
                    Shifts.Series[worker.Name].Points.AddXY(shift.GetColValue("day"), shift.GetColValue("hours"));
                   
            }

        }
        private void Search_Click(object sender, EventArgs e)
        {
            Load_Shifts(Worker_utiles.getShifts(mouthPicker,Worker_checkbox));
        }

        private void Worker_checkbox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Search_Click(null, null);
        }

        private void Worker_Register_Finish(object sender, EventArgs e)
        {
            if (Wizzard_Helper.inputValidator(Worker_Profile_Panel) &&
                Wizzard_Helper.inputValidator(Worker_User_panel))
            {
                try
                {
                    List<object> profile = new List<object>()
                {
                    int.Parse(Worker_Id.Text),
                    Worker_fname.Text,
                    Worker_lname.Text,
                    Worker_Bdate.Value.ToShortDateString(),
                    Worker_gender.Text,
                    Worker_phone.Text,
                    Worker_email.Text,
                },user=new List<object>()
                {
                    int.Parse(Worker_Id.Text),
                    Worker_password.Text,
                    Worker_Admin.Checked
                };
                    if( Access.Execute(SQL_Queries.Insert("people", profile))&& Access.Execute(SQL_Queries.Insert("workers", user)))
                    {
                        Refresh_Screen();
                        MessageBox.Show("inserted");
                    }
                    else
                    {
                        MessageBox.Show(Access.ExplaindError());
                        Access.Execute(SQL_Queries.Delete("people", new Condition("id", int.Parse(Worker_Id.Text))));
                    }
                    
                }
                catch
                {

                }
            }
            else
                MessageBox.Show("אנא מלא את השדות החסרים");
        }

        private void Treatment_Wizzard_Finish(object sender, EventArgs e)
        {
            int id = Assets.GetNewId("treatments");
            if (id == -1) {
                MessageBox.Show(Access.ExplaindError());
                return;
            }
            try
            {
                List<object> list = new List<object>()
            {
                id,
                Treatment_Name.Text,
                Treatment_Decription.Text,
                int.Parse(Treatment_Price.Text)
            };
                if (Access.Execute(SQL_Queries.Insert("treatments", list)))
                {
                    MessageBox.Show("טיפול חדש נכנס לטיפול");
                    Refresh_Screen();
                }
                else
                    MessageBox.Show(Access.ExplaindError());
            }
            catch
            {

            }
        }

        private void Treatment_Price_TextChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (box.Text != "")
                try { int.Parse(box.Text); }
                catch
                {
                    MessageBox.Show("רק מספרים בשדה זה");
                    box.Text = "";
                }

        }

        private void Treatment_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            Treatment_utiles.ChangeTreatment(Treatment_List.SelectedIndex);
            Load_Treatment();
        }
        private void Load_Treatment()
        {
            Treatment_Preview_Name.Text = Treatment_utiles.GetValue("treatment");
            Treatment_Preview_Decription.Text = Treatment_utiles.GetValue("summery");
            Treatment_Preview_Price.Text = Treatment_utiles.GetValue("price");
        }
        private void Refresh_Screen()
        {
            using (Loading_Screen window = new Loading_Screen())
            {
                window.Show();
                Assets.Refresh();
                MainWindow_Load(null, null);
                window.Close();
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                List<Col> values = new List<Col>()
            {
                new Col("treatment",Treatment_Preview_Name.Text),
                new Col("summery",Treatment_Preview_Decription.Text),
                new Col("price",int.Parse(Treatment_Preview_Price.Text))
            };
                if (Treatment_utiles.UpdateTreatment(values))
                {
                    Refresh_Screen();
                    MessageBox.Show("טיפול עודכן");
                }
                else
                {
                    MessageBox.Show(Access.ExplaindError());
                }
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Treatment_utiles.DeleteTreatment())
            {
                Treatment_List.SelectedIndex = -1;
                Refresh_Screen();
                MessageBox.Show("טיפול נמחק");
            }
            else
            {
                MessageBox.Show(Access.ExplaindError());
            }
        }





        //register code
    }
}
