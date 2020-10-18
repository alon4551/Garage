using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Garage
{
    public static class Treatment_utiles
    {
        private static int Selected_Treatment = 0;
        public static void ChangeTreatment(int index)
        {
            Selected_Treatment = index;
        }
        public static List<string> getNames()
        {
            List<string> names = new List<string>();
            foreach (Row row in Assets.treatments)
                names.Add(row.GetColValue("treatment").ToString());
            return names;
        }
        public static string GetValue(string field)
        {
            if (Selected_Treatment == -1)
                return "";
            return Assets.treatments[Selected_Treatment].GetColValue(field).ToString();
        }
        public static bool DeleteTreatment()
        {
            if (Selected_Treatment == -1)
                return false;
            Condition condition = new Condition("id", int.Parse(GetValue("id")));
            return Access.Execute(SQL_Queries.Delete("treatments",condition));
        }
        public static bool UpdateTreatment(List<Col> values)
        {
            if (Selected_Treatment == -1)
                return false;
            Condition condition = new Condition("id", int.Parse(GetValue("id")));
            return Access.Execute(SQL_Queries.Update("treatments", values, condition));

        }
    }
}
