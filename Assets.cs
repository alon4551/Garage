using Garage.sql_helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage
{
    public static class Assets
    {
        public static List<Row> users,workers,cars,treatments,models,manufactors,shifts,order_treatments,orders ;
        public static void Refresh()
        {
            Reload_Cars();
            Reload_Manufactors();
            Reload_Models();
            Reload_Order_Treatments();
            Reload_Orders();
            Reload_People();
            Reload_Shifts();
            Reload_Workers();
        }
        public static void Reload_People()
        {
            users = Access.getObjects(SQL_Queries.Select("people"));

        }
        public static void Reload_Workers()
        {
            workers = Access.getObjects(SQL_Queries.Select("workers"));

        }
        public static void Reload_Cars()
        {
            cars = Access.getObjects(SQL_Queries.Select("cars"));
        }
        public static void Reload_Treatments()
        {
            treatments = Access.getObjects(SQL_Queries.Select("treatments"));
        }
        public static void Reload_Models()
        {
            models = Access.getObjects(SQL_Queries.Select("models"));

        }
        public static void Reload_Manufactors()
        {
            manufactors = Access.getObjects(SQL_Queries.Select("manufactors"));
        }
        public static void Reload_Shifts()
        {
            shifts = Access.getObjects(SQL_Queries.Select("shifts"));

        }
        public static void Reload_Order_Treatments()
        {
            order_treatments = Access.getObjects(SQL_Queries.Select("order_treatments"));

        }
        public static void Reload_Orders()
        {
            orders = Access.getObjects(SQL_Queries.Select("orders"));

        }
        private static SQL_Table setTable(string name,List<Row> rows)
        {
            return new SQL_Table(name, rows);
        }
        public static List<SQL_Table> getCarInfo(string number)
        {
            List<SQL_Table> info = new List<SQL_Table>();
            return info;
        }
        public static List<SQL_Table> getOrderInfo(string orderId)
        {
            List<SQL_Table> info = new List<SQL_Table>();
            List<Row> Treatments = new List<Row>(), quantity=new List<Row>();
            foreach(Row row in order_treatments)
            {
                if (row.GetColValue("orders").ToString() == orderId)
                    quantity.Add(row);
            }
            info.Add(setTable("quantity", quantity));
            foreach (Row row in Treatments)
            {
                foreach (Row quan in quantity)
                    if (quan.GetColValue("treatment").ToString() == row.GetColValue("id").ToString())
                    {
                        treatments.Add(row);
                        break;
                    }
            }
            info.Add(setTable("treatments", treatments));
            foreach(Row row in orders)
            {
                if(row.GetColValue("id").ToString()==orderId)
                {
                    info.Add(setTable("order", new List<Row>() { row });
                    break;
                }
            }
            return info;
        }
        public static List<SQL_Table> getPersonInfo(string id)
        {
            List<SQL_Table> info = new List<SQL_Table>();
            List<Row> myCars = new List<Row>(), orders=new List<Row>(),treatment=new List<Row>();
            foreach(Row row in users)
                   if(id==row.GetColValue("id").ToString())
                {
                    info.Add(setTable("users", new List<Row>() { row }));
                    break;
                }
            foreach(Row row in orders)
            {
                if (row.GetColValue("customer").ToString() == id)
                {
                    orders.Add(row);
                }
            }
            info.Add(setTable("myOrders", orders));
            foreach(Row row in cars)
                if (row.GetColValue("owner").ToString() == id)
                {
                    myCars.Add(row);
                }
            info.Add(setTable("cars", myCars));
            return info;
        }
        public static List<SQL_Table> getWorkerInfo(string id)
        {
            List<SQL_Table> info = getPersonInfo(id);
            List<Row> shifts = new List<Row>(), orders = new List<Row>();
            foreach (Row row in users)
                if (id == row.GetColValue("id").ToString())
                {
                    info.Add(setTable("users", new List<Row>() { row }));
                    break;
                }
            foreach (Row row in orders)
                if (id == row.GetColValue("worker").ToString())
                {
                    orders.Add(row);
                    break;
                }
            info.Add(setTable("workerOrders", orders));
            return info;
        }
    }
}
