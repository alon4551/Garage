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
        public static List<Row> users,workers,cars,treatments,models,manufactors,shifts,order_treatments,orders ,cars_order;
        public static void Refresh()
        {
            Reload_Cars();
            Reload_Manufactors();
            Reload_Models();
            Reload_Treatments();
            Reload_Order_Treatments();
            Reload_Orders();
            Reload_People();
            Reload_Shifts();
            Reload_Workers();
            Reload_Cars_Order();
        }
        public static void Reload_Cars_Order()
        {
            cars_order = Access.getObjects(SQL_Queries.Select("cars_orders"));

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
            manufactors = Access.getObjects(SQL_Queries.Select("manfuactors"));
        }
        public static void Reload_Shifts()
        {
            shifts = Access.getObjects(SQL_Queries.Select("shifts"));

        }
        public static void Reload_Order_Treatments()
        {
            order_treatments = Access.getObjects(SQL_Queries.Select("order_treatment"));

        }
        public static void Reload_Orders()
        {
            orders = Access.getObjects(SQL_Queries.Select("orders"));

        }
        public static List<string> getManufactorNames()
        {
            List<string> names = new List<string>();
            foreach (Row row in manufactors)
                names.Add(row.GetColValue("manufactor").ToString());
            return names;
        }
        public static Row getCar(string id)
        {
            foreach(Row row in cars)
            {
                if (row.GetColValue("car_number").ToString() == id) return row;
            }
            return null;
        }
        public static List<Row> GetCustomers()
        {
            bool exsist;
            List<Row> customers = new List<Row>();
            foreach (Row person in users)
            {
                exsist = false;
                foreach (Row worker in workers)
                {
                    if (person.GetColValue("id").ToString() == worker.GetColValue("id").ToString())
                    {
                        exsist = true;
                        break;
                    }
                }
                if (exsist == false)
                    customers.Add(person);
            }
            return customers;
        }
        public static List<Row> filter_Models(int manufactors_index)
        {
            Row manufactor = manufactors[manufactors_index];
            List<Row> filter = new List<Row>();
            foreach (Row model in models)
            {
                if (model.GetColValue("manufactor").ToString() == manufactor.GetColValue("id").ToString())
                    filter.Add(model);
            }
            return filter;
        }
        public static List<Row> getWorkers()
        {
            List<Row> list = new List<Row>();
            foreach(Row worker in workers)
            {
                foreach(Row user in users)
                    if (worker.GetColValue("id").ToString() == user.GetColValue("id").ToString())
                    {
                        list.Add(user);
                        break;
                    }
            }
            return list;
        }
        private static SQL_Table setTable(string name,List<Row> rows)
        {
            return new SQL_Table(name, rows);
        }
        public static List<SQL_Table> getCarInfo(string number)
        {
            List<SQL_Table> info = new List<SQL_Table>();
            List<Row> list = new List<Row>();
            foreach(Row row in cars)
            {
                if (row.GetColValue("number").ToString() == number)
                {
                    info.Add(setTable("cars", new List<Row>() { row }));
                    break;
                }
            }
            foreach(Row row in cars_order)
            {
                foreach(Row order in orders)
                {
                    if (order.GetColValue("car").ToString() == number
                        &&row.GetColValue("id").ToString()==order.GetColValue("orders").ToString())
                    {
                        list.Add(row);
                        break;
                    }
                }
            }
            info.Add(setTable("Orders", list));
            return info;
        }
        public static List<SQL_Table> getOrderInfo(string orderId)
        {
            List<SQL_Table> info = new List<SQL_Table>();
            List<Row> Treatments = new List<Row>(), quantity=new List<Row>(),ordercars=new List<Row>();
            foreach(Row row in cars_order)
            {
                if (row.GetColValue("orders").ToString() == orderId)
                    ordercars.Add(getCar(row.GetColValue("car").ToString()));
            }
            info.Add(setTable("cars", ordercars));
            foreach(Row row in order_treatments)
            {
                if (row.GetColValue("orders").ToString() == orderId)
                    quantity.Add(row);
            }
            info.Add(setTable("quantity", quantity));
            foreach (Row row in treatments)
            {
                foreach (Row quan in quantity)
                    if (quan.GetColValue("treatment").ToString() == row.GetColValue("id").ToString())
                    {
                        Treatments.Add(row);
                        break;
                    }
            }
            info.Add(setTable("treatments", Treatments));
            foreach(Row row in orders)
            {
                if(row.GetColValue("id").ToString()==orderId)
                {
                    info.Add(setTable("order", new List<Row>() { row }));
                    break;
                }
            }
            return info;
        }
        public static List<SQL_Table> getPersonInfo(string id)
        {
            List<SQL_Table> info = new List<SQL_Table>();
            List<Row> myCars = new List<Row>(), myorders=new List<Row>(),treatment=new List<Row>();
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
                    myorders.Add(row);
                }
            }
            info.Add(setTable("myOrders", myorders));
            foreach(Row row in cars)
                if (row.GetColValue("id_owner").ToString() == id)
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
