using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetWPF
{
    public class Employee
    {
        public int ID_Employee { get; set; }
        public string First_Name { get; set; }
        public string Second_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Employee_Phone { get; set; }
        public string Employee_E_Mail { get; set; }
        public int Employee_Wage { get; set; }

        public Employee (int id, string first_Name, string second_Name, string middle_Name, string employee_Phone, string employee_E_Mail, int employee_Wage)
        {
            ID_Employee = id;
            First_Name = first_Name;
            Second_Name = second_Name;
            Middle_Name = middle_Name;
            Employee_Phone = employee_Phone;
            Employee_E_Mail = employee_E_Mail;
            Employee_Wage = employee_Wage;
        }
    }
}
