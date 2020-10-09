using Garage.sql_helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Garage
{
    public static class UserProfile
    {
        public static List<SQL_Table> Profile,Order;
        public static Row user;
        public static List<Row> cars, workers, myOrders, filter_Orders = new List<Row>(),filter_Models=new List<Row>(),models,manufactors;
        public static int user_id;
        public static string manufactor;

        public static void setUserId(int id)
        {
            user_id = id;
            GetUserInfo();
        }
        public static void Refresh()
        {
            Assets.Refresh();
            GetUserInfo();
        }
        public static Row getOrder()
        {
            Row row = new Row();
            foreach(SQL_Table table in Order)
            {
                if (table.Name == "order")
                    row = table.Rows[0];
            }
            return row;
        }
        public static List<Row> getTreatments()
        {
            List<Row> treatments = new List<Row>();
            foreach (SQL_Table table in Order)
            {
                if (table.Name == "treatments")
                {
                    foreach (Row row in table.Rows)
                        treatments.Add(row);
                    break;
                }
            }
            return treatments;

        }
        public static List<string> getOrderTreatments()
        {
            List<string> treatments = new List<string>();
            foreach(SQL_Table table in Order)
            {
                if(table.Name== "treatments")
                {
                    foreach (Row row in table.Rows)
                        treatments.Add(row.GetColValue("treatment").ToString());
                    break;
                }
            }
            return treatments;
        }
        public static List<string> getOrderCars()
        {
            List<string> cars_id = new List<string>();
            foreach(SQL_Table table in Order)
            {
                if (table.Name == "cars")
                {
                    foreach (Row row in table.Rows)
                        cars_id.Add(row.GetColValue("car_number").ToString());
                    break;
                }
            }
            return cars_id;
        }
        public static int quantityTreatment(int treatment)
        {
            int count = 0;
            foreach(SQL_Table table in Order)
            {
                if (table.Name == "quantity")
                {
                    foreach (Row row in table.Rows)
                        if (row.GetColValue("treatment").ToString() == treatment.ToString())
                            count++;
                }
            }
            return count;
        }
        public static void GetOrderInfo(int id)
        {
            Order = Assets.getOrderInfo(id.ToString());
        }
        public static void GetUserInfo()
        {
            Profile = Assets.getPersonInfo(user_id.ToString());
            workers = Assets.getWorkers();

            foreach(SQL_Table table in Profile)
            {
                switch (table.Name)
                {
                    case "users":
                        {
                            user = table.Rows[0];
                            break;
                        }
                    case "myOrders":
                        {
                            myOrders = table.Rows;
                            break;
                        }
                    case "cars":
                        {
                            cars = table.Rows;
                            break;
                        }
                }
            }
        }
        private static Condition Create_Condition(CheckBox box,object value)
        {
            switch (box.Name)
            {
                case "worker_id":
                    {
                        return new Condition("worker",value);
                    }
                case "id_recipt":
                    {
                        return new Condition("id", value);
                    }
                case "date_recipt":
                    {
                        return new Condition("purchase", value);
                    }
                default:
                    return null;
            }
        }
        public static void FilterOrders(List<CheckBox> boxes, List<object> values)
        {
            List<Condition> conditions = new List<Condition>();
            List<Row> table = new List<Row>();
            filter_Orders.Clear();
            conditions.Add(new Condition("customer", user_id));
            bool num = false;
            string value = "";
            for (int i = 0; i < boxes.Count; i++)
                if (boxes[i].Name != "recipt")
                    conditions.Add(Create_Condition(boxes[i], values[i]));
                else
                {
                    num = true;
                    value = values[i].ToString();
                }
            if (num)
            {
                table = Access.getObjects(SQL_Queries.Select("orders", conditions, "and"));
                foreach(Row row in table)
                {
                    if (row.GetColValue("id").ToString().Contains(value))
                        filter_Orders.Add(row);
                }
            }
            else
            {
                filter_Orders= Access.getObjects(SQL_Queries.Select("orders", conditions, "and"));
            }
        }
        public static void Filter_By_Manufactor()
        {
            filter_Models.Clear();
            foreach (Row row in models)
            {
                if (row.GetColValue("manufactor").ToString() == manufactor)
                    filter_Models.Add(row);
            }
        }
        private static int digits(int digit)
        {
            if (digit < 10)
                return digit;
            else
                return digit % 10 + digit / 10;
        }
        private static bool ID_Check(string id)
        {
            if (id.Length != 9)
                return false;
            char [] array = id.ToCharArray();
            int sum=0;
            for(int i =0 ; i <8; i++)
                sum += i % 2 == 0 ? (array[i] - '0') : digits((array[i] - '0')*2);
            if (sum % 10 == 0 && array[8] == '0'|| sum % 10 == 10 - (array[8] - '0'))
                return true;
            return false;
        }
        public static string getWorkerFullName(string id)
        {
            foreach(Row row in Assets.users)
            {
                if (row.GetColValue("id").ToString() == id)
                {
                    return $"{row.GetColValue("lname")} {row.GetColValue("fname")}";
                }
            }
            return "";
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static bool isValidPhone(string phone)
        {
            if (phone.Length != 10 && phone.Length != 12)
                return false;
            int space = 0;
            foreach(char digit in phone.ToCharArray())
            {
                if ((digit < '0' || digit > '9') && digit != '-')
                    return false;
                space += digit == '-' ? 1 : 0;
            }
            if (space != 2 && space!=0)
                return false;
            return true;
        }
        public static bool input_check(TextBox input)
        {
            switch (input.Name)
            {
                case "id":
                    {
                        if (ID_Check(input.Text))
                        {
                            input.BackColor = Color.White;
                            return true;
                        }
                        else
                        {
                            input.BackColor = Color.Red;
                            return false;

                        }
                    }
                case "email": 
                    {
                        if(IsValidEmail(input.Text))
                        {
                            input.BackColor = Color.White;
                            return true;
                        }
                        else
                        {
                            input.BackColor = Color.Red;
                            return false;
                        }    
                    }
                case "phone":
                    {
                        if (isValidPhone(input.Text))
                        {
                            input.BackColor = Color.White;
                            return true;
                        }
                        else
                        {
                            input.BackColor = Color.Red;
                            return false;
                        }
                    }
                case "car_number":
                    {
                        if (input.Text.Length == 7)
                        {
                            return true;
                        }
                        else
                        {

                            return false;
                        }
                    }
                default:
                    return false;
            }
        }
        public static bool comboboxValidator(ComboBox box )
        {
            bool result= box.SelectedIndex != -1;
            if (result)
            {
                box.BackColor = Color.White;
                
            }
            else
            {
                box.BackColor = Color.Red;
            }
            return result;
        }
        public static bool UpdateUser(List<Col> update)
        {

            bool execute= Access.Execute(SQL_Queries.Update("people", update, new Condition("id", user_id)));
            if (execute)
            {
                foreach(Col col in update)
                {
                    if (col.GetField() == "id")
                    {
                        user_id = (int)(col.GetValue());
                        break;
                    }
                }
                user.UpdateColume(update);
                GetUserInfo();
            }
            return execute;
        }
        public static bool DeleteUser()
        {
            return Access.Execute(SQL_Queries.Delete("people", new Condition("id", user_id)));
        }
        public static bool InsertCar(List<object> values)
        {
            bool execute= Access.Execute(SQL_Queries.Insert("cars", values));
            if (execute)
                Refresh();
            return execute;
        }
        public static bool UpdateCar(List<Col> update,int number)
        {
            MessageBox.Show(SQL_Queries.Update("cars", update, new Condition("number", number)));
            bool execute= Access.Execute(SQL_Queries.Update("cars", update, new Condition("car_number", number)));
            if (execute)
            {
                foreach(Row row in cars)
                    if(row.GetColValue("car_number").ToString()==number.ToString())
                    {
                        row.UpdateColume(update);
                        break;
                    }
                Refresh();
            }

            return execute;
        }
        public static bool DeleteCar( int number)
        {
            bool execute= Access.Execute(SQL_Queries.Delete("cars",new Condition("car_number", number)));
            if (execute)
                Refresh();
            return execute;
        }
    }
}
