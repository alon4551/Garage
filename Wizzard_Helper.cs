using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Garage
{
    public static class Wizzard_Helper
    {
        public static bool inputValidator(TableLayoutPanel table)
        {
            Control control;
            bool validate = true;
            for (int i = 0, j; i < table.RowCount; i++)
                for (j = 0; j < table.ColumnCount; j++)
                {
                    control = table.GetControlFromPosition(j, i);
                    validate &= CheckInput(control);
                    if (!validate)
                        return validate;
                }
            return validate;
        }
        public static void ClearValidator(TableLayoutPanel table)
        {
            Control control;
            for (int i = 0, j; i < table.RowCount; i++)
                for (j = 0; j < table.ColumnCount; j++)
                {
                    control = table.GetControlFromPosition(j, i);
                    ClearInput(control);
                }
        }
        public static bool CheckInput(Control control)
        {
            if (control == null)
                return true;
            switch (control.GetType().Name)
            {
                case "TextBox":
                    {
                        if (((TextBox)control).Text == "")
                            return false;
                        break;
                    }
                case "ComboBox":
                    {
                        if (((ComboBox)control).SelectedIndex == -1)
                            return false;
                        break;
                    }
                default:
                    {
                        if (control.Text == "")
                            return false;
                        break;
                    }
            }
            return true;
        }
        public static void ClearInput(Control control)
        {
            if (control == null)
                return ;
            switch (control.GetType().Name)
            {
                case "DateTimePicker":
                    {
                        ((DateTimePicker)control).Value = DateTime.Now;
                        break;
                    }
                case "ComboBox":
                    {
                        ((ComboBox)control).SelectedIndex = -1;
                        break;
                    }
                default:
                    {
                        control.Text = "";
                        break;
                    }
            }
        }
    }
}
