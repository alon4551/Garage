using Garage.sql_helper;
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
    public partial class UserWizzard : Form
    {

        public UserWizzard()
        {
            InitializeComponent();
        }
        private void Refresh()
        {
            UserProfile.GetUserInfo();
            load_Profile();
            load_Cars();
            load_workers();
            load_Manufactors();
            load_Orders();
        }
        private void load_workers()
        {
            worker.Items.Clear();
            foreach(Row row in UserProfile.workers)
            {
                worker.Items.Add(row.GetColValue("fname"));
            }
        }
        private void load_Orders()
        {
            orderBox.Items.Clear();
            foreach(Row row in UserProfile.filter_Orders)
            {
                orderBox.Items.Add(row.GetColValue("id"));
            }
        }
        private void checkbox_Click(object sender, EventArgs args)
        {
            CheckBox box = (CheckBox)sender;
            
            switch (box.Name)
            {
                case "worker_id":
                    {
                        worker.Enabled = box.Checked;
                        break;
                    }
                case "id_recipt":
                    {
                        recipt.Enabled = box.Checked;
                        break;
                    }
                case "date_recipt":
                    {
                        purchase.Enabled = box.Checked;
                        break;
                    }
            }
        }
        private void UserWizzard_Load(object sender, EventArgs e)
        {
            UserProfile.manufactors = Assets.manufactors;
            UserProfile.models = Assets.models;
            UserProfile.filter_Orders = UserProfile.myOrders;
            Refresh();
        }
        private void load_Manufactors()
        {
            manufacturs.Items.Clear();
            foreach(Row row in UserProfile.manufactors)
            {
                manufacturs.Items.Add(row.GetColValue("manufactor"));
            }
        }
        private void filter_models(object sender, EventArgs args)
        {
            UserProfile.manufactor = UserProfile.manufactors[manufacturs.SelectedIndex].GetColValue("id").ToString();
            UserProfile.Filter_By_Manufactor();
            foreach(Row row in UserProfile.filter_Models)
            {
                models.Items.Add(row.GetColValue("model"));
            }
        }
        private void load_Cars()
        {
            Clear_Car();
            carCombo.Items.Clear();
            carList.Items.Clear();
            foreach(Row row in UserProfile.cars)
            {
                carList.Items.Add(row.GetColValue("car_number"));
                carCombo.Items.Add(row.GetColValue("car_number"));
            }
        }
        private void load_Profile()
        {
            id.Text = UserProfile.user.GetColValue("id").ToString();
            firstname.Text = UserProfile.user.GetColValue("fname").ToString();
            lastname.Text = UserProfile.user.GetColValue("lname").ToString();
            birthday.Text = UserProfile.user.GetColValue("birthday").ToString();
            gender.Text = UserProfile.user.GetColValue("gender").ToString();
            phone.Text = UserProfile.user.GetColValue("phone").ToString();
            email.Text = UserProfile.user.GetColValue("email").ToString();

        }
        private void PreviewCar(int index)
        {
            Row car=UserProfile.cars[index];
            car_number.Text = car.GetColValue(0).ToString();
            kilometer.Text = car.GetColValue("kilomer").ToString();
            foreach(Row row in UserProfile.models)
            {
                if(row.GetColValue("id").ToString()==car.GetColValue("model").ToString())
                {
                    UserProfile.manufactor = row.GetColValue("manufactor").ToString();
                    models.Text = row.GetColValue("model").ToString();
                    UserProfile.Filter_By_Manufactor();
                    break;
                }
            }        
            foreach(Row row in UserProfile.manufactors)
            {
                if(row.GetColValue("id").ToString()==UserProfile.manufactor)
                {
                    manufacturs.Text = row.GetColValue("manufactor").ToString();
                    break;
                }
            }
        }
        private void carCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            carList.SelectedIndex = -1;
            if(carCombo.SelectedIndex!=-1)
            PreviewCar(carCombo.SelectedIndex);
        }

        private void carList_SelectedIndexChanged(object sender, EventArgs e)
        {
            carCombo.SelectedIndex = -1;
            if (carList.SelectedIndex != -1)
                PreviewCar(carList.SelectedIndex);
        }
        private bool UserInputValidator()
        {
            bool result = UserProfile.input_check(id);
            result &= UserProfile.input_check(email);
            result&=UserProfile.input_check(phone);
            return result;
        }
        private bool CarInputValidator()
        {
            bool result = UserProfile.input_check(car_number);
            result &= UserProfile.comboboxValidator(manufacturs);
            result &= UserProfile.comboboxValidator(models);
            return result;
        }
        private void UpdateUser_Click(object sender, EventArgs e)
        {
            if (UserInputValidator())
            {
            List<Col> update = new List<Col>();
            update.Add(new Col("id", int.Parse(id.Text)));
            update.Add(new Col("fname", firstname.Text));
            update.Add(new Col("lname", lastname.Text));
            update.Add(new Col("birthday", birthday.Text));
            update.Add(new Col("gender", gender.Text));
            update.Add(new Col("email", email.Text));
            update.Add(new Col("phone", phone.Text));
                if (UserProfile.UpdateUser(update))
                {
                    MessageBox.Show("user updated");
                    Refresh();
                }
                else
                    MessageBox.Show(Access.ExplaindError());
            }
        }
        private void Clear_Car()
        {
            manufacturs.Text = "";
            models.Text = "";
            models.Items.Clear();
            car_number.Text = "";
            kilometer.Text = "";
        }
        private void add_car_Click(object sender, EventArgs e)
        {
            if (CarInputValidator())
            {
                List<object> values = new List<object>()
                {
                    int.Parse(car_number.Text),
                    UserProfile.user_id,
                    UserProfile.filter_Models[models.SelectedIndex].GetColValue("id"),
                    kilometer.Text
                };
                if (UserProfile.InsertCar(values))
                {
                    Refresh();
                }
            }
        }

        private void update_car_Click(object sender, EventArgs e)
        {
            if (CarInputValidator())
            {
                List<Col> values = new List<Col>()
                {new Col("car_number",int.Parse(car_number.Text)),
                new Col("model",int.Parse(UserProfile.filter_Models[models.SelectedIndex].GetColValue("id").ToString())),
                new Col("kilomer",int.Parse(kilometer.Text))
                };
                int number;
                if (carCombo.SelectedIndex != -1)
                    number = int.Parse(carCombo.Text);
                else
                    number = int.Parse(carList.Text);
                if (UserProfile.UpdateCar(values, number))
                {
                    MessageBox.Show("car updated");
                    Refresh();
                }
                else
                    MessageBox.Show(Access.ExplaindError());
            }
        }

        private void delete_car_Click(object sender, EventArgs e)
        {

            if (UserProfile.DeleteCar(int.Parse(car_number.Text)))
            {
                MessageBox.Show("car deleted");
                Refresh();
            }
            else
                MessageBox.Show(Access.ExplaindError());
        }
        private object getFilterInfo(CheckBox box)
        {
            
            switch (box.Name)
            {
                case "worker_id":
                    {if (worker.SelectedIndex != -1)
                            return int.Parse(UserProfile.workers[worker.SelectedIndex].GetColValue("id").ToString());
                        else
                            return "";
                    }
                case "id_recipt":
                    {
                        return int.Parse(recipt.Text);
                    }
                case "date_recipt":
                    {
                        return purchase.Value.ToShortDateString();
                    }
                default:
                    return "";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {   List<object> values = new List<object>();
            List<CheckBox> boxes = new List<CheckBox>();
            foreach(CheckBox box in filter_table.Controls.OfType<CheckBox>())
            {
                if (box.Checked)
                {
                    if(getFilterInfo(box)!="")
                    {
                        boxes.Add(box);
                        values.Add(getFilterInfo(box));
                    }

                }
            }
                UserProfile.FilterOrders(boxes, values);
                Refresh();
        }
        private void Preview_Order()
        {
            Row order = UserProfile.getOrder();
            recipt_box.Text = order.GetColValue("id").ToString();
            sun.Text = order.GetColValue("summery").ToString();
            car_combo.Items.Clear();
            treatments.Items.Clear();
            worker_name.Text = UserProfile.getWorkerFullName(order.GetColValue("worker").ToString());
            foreach (string name in UserProfile.getOrderCars())
                car_combo.Items.Add(name);
            foreach (string name in UserProfile.getOrderTreatments())
                treatments.Items.Add(name);

        }
        private void orderBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserProfile.GetOrderInfo(int.Parse(orderBox.SelectedItem.ToString()));
            Preview_Order();
        }

        private void treatments_SelectedIndexChanged(object sender, EventArgs e)
        {
            quantity.Text = UserProfile.quantityTreatment(int.Parse(UserProfile.getTreatments()[treatments.SelectedIndex].GetColValue("id").ToString())).ToString();
        }
    }
}
