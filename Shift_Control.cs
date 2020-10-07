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
    }
}
