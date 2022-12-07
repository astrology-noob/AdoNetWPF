using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Common;
using System.Data;
using System.Collections;

namespace AdoNetWPF
{
    public partial class MainWindow : Window
    {
        public DBReader dbContext = new DBReader();
        private List<Employee> CheckedRows = new List<Employee>();


        public MainWindow()
        {
            InitializeComponent();
            InsertScalarValues();
        }

        public async void InsertScalarValues()
        {
            empCount.Text = (await dbContext.GetCount()).ToString();
            minWage.Text = (await dbContext.GetMinWage()).ToString();
            maxWage.Text = (await dbContext.GetMaxWage()).ToString();
            avgWage.Text = (await dbContext.GetAverageWage()).ToString();
            
        }

        // Загружение данных в datagrid приложения
        private async void dataGrid_Initialized(object sender, EventArgs e)
        {
            dataGrid.ItemsSource = await dbContext.SelectDataFromDB();
            dataGrid.Columns[1].IsReadOnly = true;
        }

        // Получение объекта Employee из тектбоксов
        private Employee GetEmployeeFromForm()
        {
            int id;
            int.TryParse(IDTB.Text, out id);
            int wage;
            int.TryParse(WageTB.Text, out wage);

            return new Employee(
                id,
                FirstNameTB.Text,
                SecondNameTB.Text,
                MiddleNameTB.Text,
                PhoneTB.Text,
                EmailTB.Text,
                wage
            );
        }


        // Вставка строки по кнопке
        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            Employee newEmployee = GetEmployeeFromForm();

            try
            {
                await dbContext.InsertData(newEmployee);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            List<Employee> employees = (List<Employee>)dataGrid.ItemsSource;
            employees.Add(newEmployee);
            dataGrid.Items.Refresh();
            InsertScalarValues();
        }

        // Удаление выбранных строк
        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Employee employeeDelete in CheckedRows)
            {
                try
                {
                    await dbContext.DeleteData(employeeDelete);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                List<Employee> employees = (List<Employee>)dataGrid.ItemsSource;
                employees.Remove(employeeDelete);
            }

            dataGrid.Items.Refresh();
            CheckedRows.Clear();
            InsertScalarValues();
        }


        // Изменение строки при изменение ячейки
        private async void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == 0) { HandleCheckBoxes((Employee)e.Row.Item); return; }

            DataGridCellInfo cell = dataGrid.CurrentCell;

            int newColumn = cell.Column.DisplayIndex - 1;

            string newValue = (e.EditingElement as TextBox).Text;

            Employee emp = (Employee)cell.Item;

            try
            {
                await dbContext.UpdateData(emp, newColumn, newValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            InsertScalarValues();
        }


        // Хранение выбранных строк
        private void HandleCheckBoxes(Employee emp)
        {
            if (CheckedRows.Contains(emp))
                CheckedRows.Remove(emp);
            else
                CheckedRows.Add(emp);
        }
    }
}
