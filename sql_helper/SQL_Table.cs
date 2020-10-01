using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.sql_helper
{
    public class SQL_Table
    {
        public List<Row> Rows;
        public string table_Name;
        public SQL_Table (string name, List<Row> rows){
            Rows = rows;
            table_Name = name;  
        }
        public SQL_Table() { 
        }
        public Row GetRow(Col col)
        {
            foreach(Row row in Rows)
            {
                object Search = row.GetColValue(col.GetField());
                if (Search != null && Search == col.GetValue())
                    return row;

            }
            return null;
        }
        public Row GetRow(string field,object value)
        {
            foreach (Row row in Rows)
            {
                object Search = row.GetColValue(field);
                if (Search != null && Search == value)
                    return row;

            }
            return null;
        }
    }
}
