using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Worker
{
    public static class Worker_utiles
    {
        public static List<Row> workers = Assets.getWorkers();
        public static List<string> getWorkerNames()
        {
            List<string> names = new List<string>();
            foreach (Row row in workers)
                names.Add($"{row.GetColValue("lname")} {row.GetColValue("fname")}");
            return names;
        } 
    }
}
