using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;
using System.Data.Common;
using System.Runtime.Remoting.Contexts;
using System.Net.NetworkInformation;

namespace AdoNetWPF
{
    public class DBReader
    {
        private string tableName = "[TBL].[Employee]";

        // Метод для select запросов
        public async Task<List<Employee>> SelectDataFromDB()
        {
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString);
            
            await sqlConnection.OpenAsync();

            string requestText = string.Format("SELECT * FROM {0}", tableName);

            SqlCommand sqlCommand = new SqlCommand(requestText, sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();


            if (!reader.HasRows) { return new List<Employee>(); }

            List<Employee> data = new List<Employee>();

            while (reader.Read())
            {
                Employee tempRow = new Employee(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetInt32(6)
                    );

                data.Add(tempRow);
            }

            return data;
        }

        // Общий метод на выполнение хранимых процедур для insert, update и delete
        public async Task<int> ChangeDataInDB(string procName, List<SqlParameter> sqlParameters)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter sqlParameter in sqlParameters)
                {
                    sqlCommand.Parameters.Add(sqlParameter);
                }

                int result = await sqlCommand.ExecuteNonQueryAsync();
                MessageBox.Show(result.ToString());
                return result;
            }
        }

        // Запрос на вставку строки
        public async Task InsertData(Employee emp)
        {
            string procName = "SP.Employee_Insert";
            
            List<SqlParameter > sqlParameters = new List<SqlParameter>() {
                new SqlParameter("@first_name", emp.First_Name),
                new SqlParameter("@second_name", emp.Second_Name),
                new SqlParameter("@middle_name", emp.Middle_Name),
                new SqlParameter("@employee_phone", emp.Employee_Phone),
                new SqlParameter("@employee_e_mail", emp.Employee_E_Mail),
                new SqlParameter("@employee_wage", emp.Employee_Wage)
            };

            await ChangeDataInDB(procName, sqlParameters);
        }

        // Запрос на удаление строки
        public async Task DeleteData(Employee emp)
        {
            string procName = "SP.Employee_Delete";

            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@id_employee", emp.ID_Employee)
            };

            await ChangeDataInDB(procName, sqlParameters);
        }
        
        // Запрос на изменение строки
        public async Task UpdateData(Employee emp, int newColumn, string newValue)
        {
            string procName = "SP.Employee_Update";

            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                new SqlParameter("@id_employee", emp.ID_Employee),
                new SqlParameter("@first_name", emp.First_Name),
                new SqlParameter("@second_name", emp.Second_Name),
                new SqlParameter("@middle_name", emp.Middle_Name),
                new SqlParameter("@employee_phone", emp.Employee_Phone),
                new SqlParameter("@employee_e_mail", emp.Employee_E_Mail),
                new SqlParameter("@employee_wage", emp.Employee_Wage)
            };

            sqlParameters[newColumn].Value = newValue;

            await ChangeDataInDB(procName, sqlParameters);
        }

        // Метод для отправления запросов на получение скалярных значений
        public async Task<int> GetScalarValue(string requestText, SqlParameter sqlParameter)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand(requestText, sqlConnection);

                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.Add(sqlParameter);

                await sqlCommand.ExecuteScalarAsync();
                return (int)sqlCommand.Parameters[sqlParameter.ParameterName].Value;
            }
        }

        // Запрос на мин. значение
        public async Task<int> GetMinWage()
        {
            string procName = "SP.Select_Min_Wage";

            SqlParameter outMin = new SqlParameter("@min", SqlDbType.Int);
            outMin.Direction = ParameterDirection.Output;

            return await GetScalarValue(procName, outMin);
        }

        // запрос на макс. значение
        public async Task<int> GetMaxWage()
        {
            string procName = "SP.Select_Max_Wage";

            SqlParameter outMax = new SqlParameter("@max", SqlDbType.Int);
            outMax.Direction = ParameterDirection.Output;

            return await GetScalarValue(procName, outMax);
        }

        // запрос на получение числа строк
        public async Task<int> GetCount()
        {
            string procName = "SP.Select_Count";

            SqlParameter outCount = new SqlParameter("@count", SqlDbType.Int);
            outCount.Direction = ParameterDirection.Output;

            return await GetScalarValue(procName, outCount);
        }

        // запрос на получение среднего значения
        public async Task<int> GetAverageWage()    
        {
            string procName = "SP.Select_Avg_Wage";

            SqlParameter outAvg = new SqlParameter("@avg", SqlDbType.Int);
            outAvg.Direction = ParameterDirection.Output;

            return await GetScalarValue(procName, outAvg);
        }
    }
}
