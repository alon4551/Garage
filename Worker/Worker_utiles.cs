using Garage.sql_helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Garage.Worker
{
    public static class Worker_utiles
    {
        public static List<Row> workers = Assets.getWorkers();
        public static List<Row> shifts = Assets.shifts;
        public static List<string> getWorkerNames()
        {
            List<string> names = new List<string>();
            foreach (Row row in workers)
                names.Add($"{row.GetColValue("lname")} {row.GetColValue("fname")}");
            return names;
        } 
       public static List<SQL_Table> getShifts(DateTimePicker mounth,CheckedListBox Worker_checkbox) 
        {
            List<Row> worker_shift;
            List<SQL_Table> workers_shifts = new List<SQL_Table>();
            worker_shift = Shift_Control.Filter_Shifts(mounth.Value, -1);
            for (int j = 0; j < Worker_checkbox.CheckedItems.Count; j++)
            {

                for (int i = 0; i < Worker_checkbox.Items.Count; i++)
                {
                    if (Worker_checkbox.CheckedItems[j].ToString() == Worker_checkbox.Items[i].ToString())
                    {
                        List<Row> shifts = new List<Row>();
                        foreach (Row shift in worker_shift)
                            if (shift.GetColValue("id").ToString() == Assets.workers[i].GetColValue("id").ToString())
                                shifts.Add(shift);
                        workers_shifts.Add(new SQL_Table(Worker_checkbox.Items[i].ToString(), shifts));
                    }
                }
            }
            return workers_shifts;
        }

    }
}
