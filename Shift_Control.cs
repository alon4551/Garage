using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Garage
{
    public static class Shift_Control
    {

        private static int getLastShift()
        {
            int number=-1;
            foreach(Row row in Assets.shifts)
                number = int.Parse(row.GetColValue("id").ToString());
            return number;
        }
        public static bool Enter(int id)
        {
            List<object> list = new List<object>()
            {   getLastShift()+1,
                id,
                DateTime.Today.ToShortDateString(),
                DateTime.Today.ToShortTimeString(),
                "not"
            };
            string query = SQL_Queries.Insert("shifts", list);
            if(Access.Execute(query))
            {
                Assets.Reload_Shifts();
                return true;
            }
            return false;
        }
        public static void Exit(int id)
        {
            string query;
            foreach(Row row in Assets.shifts)
            {
                if (row.GetColValue("end_shift").ToString() == "not")
                {
                    row.UpdateColume(new Col("end_shift", DateTime.Today.ToShortTimeString()));
                    query = SQL_Queries.Update("shifts", row.GetColumes(), new Condition("shift", row.GetColValue("shift")));
                    Access.Execute(query);
                }
            }
        }
        public static List<Row> Filter_Shifts(DateTime value,int id)
        {
            List<Row> filterShifts = new List<Row>();
            Row shift;
            DateTime time, start, end;
            foreach (Row row in Assets.shifts)
            {
                try
                {
                    if (id!=-1&&row.GetColValue("id_worker").ToString() != id.ToString())
                        continue;
                    time = DateTime.Parse(row.GetColValue("day").ToString());
                    start = DateTime.Parse(row.GetColValue("start_shift").ToString());
                    end = DateTime.Parse(row.GetColValue("end_shift").ToString());
                    if (time.Month == value.Month&&time.Year==value.Year)
                    {
                            shift = new Row();
                            shift.AddColume(new Col("id", row.GetColValue("id_worker")));
                            shift.AddColume(new Col("day", $"{time.Day}/{time.Month}"));
                            shift.AddColume(new Col("hours", end.Hour - start.Hour));
                            filterShifts.Add(shift);
                    }
                }
                catch (Exception err)
                {

                }
            }
            return Organize(filterShifts);
        }
        public static List<Row> Organize(List<Row> shifts)
        {
            List<Row> orginized = new List<Row>();
            Row temp=null;
            for (; shifts.Count!=0 ;)
            {
                temp = shifts[0];
                DateTime temp_day=DateTime.Parse(temp.GetColValue("day").ToString()),day ;
                for (int j = 1; j < shifts.Count; j++)
                {
                    day = DateTime.Parse(shifts[j].GetColValue("day").ToString());
                    if (day < temp_day)
                    {
                        temp = shifts[j];
                        temp_day = day;
                    }
                }
                orginized.Add(temp);
                shifts.Remove(temp);
            }
            return orginized;
        }
    }
}
