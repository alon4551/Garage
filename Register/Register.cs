using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Garage
{
    public static class Register_utiles
    {
        static List<Row> Models;
        static List<List<object>> cars = new List<List<object>>();
        public static void SetManufactor(int index)
        {
            Models = Assets.filter_Models(index);
        }
        public static List<string> getModelsNames()
        {
            List<string> names = new List<string>();
            foreach (Row model in Models)
                names.Add(model.GetColValue("model").ToString());
            return names;
        }
        public static bool ExsistCar(int number)
        {
            foreach (List<object> car in cars)
                if (car[0].ToString() == number.ToString())
                    return true;
            foreach (Row car in Assets.cars)
                if (car.GetColValue("car_number").ToString() == number.ToString())
                    return true;
            return false;
        }
        public static void AddCar(List<object> car)
        {
            cars.Add(car);
        }
        public static List<string> getCarNumbers()
        {
            List<string> numbers = new List<string>();
            foreach (List<object> car in cars)
                numbers.Add(car[0].ToString());
            return numbers;
        }
        public static void RemoveCar(int index) {
            cars.RemoveAt(index);
        }
        public static void updateCar(List<object> list,int index)
        {
            cars.RemoveAt(index);
            cars.Add(list);
        }
        public static List<object> GetCar(int index)
        {
            
            return cars[index];
        }
        public static bool RemoveUser(int id)
        {
            return Access.Execute(SQL_Queries.Delete("people", new Condition("id",id)));
        }
        public static bool InsertPerson(List<object> values)
        {
            return Access.Execute(SQL_Queries.Insert("people", values));
        }
        public static bool InsertCar()
        {
            bool execute = true;
            foreach(List<object>car in cars)
                execute &= Access.Execute(SQL_Queries.Insert("cars", car));
            return execute;
        }
        public static int getManufactorIndex(int id)
        {
            int manufactor = -1;
            foreach (Row model in Assets.models)
                if (int.Parse(model.GetColValue("id").ToString()) == id)
                {
                    manufactor = int.Parse(model.GetColValue("manufactor").ToString());
                    break;
                }
            for (int i = 0; i < Assets.manufactors.Count; i++)
                if (int.Parse(Assets.manufactors[i].GetColValue("id").ToString()) == manufactor)
                {
                    return i;
                }
            return -1;
        }
        public static int getModelId(int Index)
        {
            return int.Parse(Models[Index].GetColValue("id").ToString());
        }

        public static int getModelIndex(int value)
        {
            for (int i = 0; i < Models.Count; i++)
                if (int.Parse(Models[i].GetColValue("id").ToString()) == value)
                    return i;
            return -1;
        }
    }
}
