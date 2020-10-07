using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage
{
    public class People_main
    {
        public List<Row> People =Assets.users;
        public string id;
        public People_main()
        {

        }
        public void Refresh()
        {
            People = Assets.users;
        }
        public bool remove_user()
        {
            try
            {
               return Access.Execute(SQL_Queries.Delete("people", new Condition("id", int.Parse(id))));
            }
            catch
            {
                return false;
            }
        }
        public List<string> getPeopleNames()
        {
            List<string> list = new List<string>();
            foreach (Row row in People)
                list.Add($"{row.GetColValue("lname")} {row.GetColValue("fname")}");
            return list;
        }


    }
}
