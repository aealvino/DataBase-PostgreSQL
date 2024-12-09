using System;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Npgsql;

namespace DataBass
{
    public partial class DemandGraphForm : Form
    {
        private string connectionString;

        // Конструктор, который принимает строку соединения
        public DemandGraphForm(string connString)
        {
            InitializeComponent();
            connectionString = connString;
        }

        // Метод для загрузки данных о продажах и построения графика
        private void LoadDemandData(int goodId)
        {
            string query = @"
                SELECT s.create_date, SUM(s.good_count) AS total_sales
                FROM sales s
                WHERE s.good_id = @goodId
                GROUP BY s.create_date
                ORDER BY s.create_date;";

            DataTable salesData = GetSalesData(query, goodId);
            BuildChart(salesData);
        }

        // Метод для получения данных о продажах из базы данных
        private DataTable GetSalesData(string query, int goodId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@goodId", goodId);

                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        // Метод для построения графика
        private void BuildChart(DataTable salesData)
        {
            chartSales.Series.Clear();
            chartSales.ChartAreas.Clear();
            chartSales.ChartAreas.Add(new ChartArea("ChartArea1"));
            chartSales.Series.Add("Sales");

            chartSales.Series["Sales"].ChartType = SeriesChartType.Line;
            chartSales.Series["Sales"].BorderWidth = 3;

            foreach (DataRow row in salesData.Rows)
            {
                DateTime date = (DateTime)row["create_date"];
                int totalSales = Convert.ToInt32(row["total_sales"]);
                chartSales.Series["Sales"].Points.AddXY(date, totalSales);
            }

            chartSales.ChartAreas[0].AxisX.LabelStyle.Format = "dd.MM.yyyy";
            chartSales.ChartAreas[0].AxisX.Title = "Дата";
            chartSales.ChartAreas[0].AxisY.Title = "Количество продаж";
        }

        // Метод для отображения графика
        private void DisplayDemandGraph(int goodId)
        {
            LoadDemandData(goodId);
        }

        // Метод для загрузки списка товаров в ComboBox
        private void LoadGoodsList()
        {
            string query = "SELECT id, name FROM goods ORDER BY name";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbGoods.Items.Add(new KeyValuePair<int, string>(
                                reader.GetInt32(0),
                                reader.GetString(1)
                            ));
                        }
                    }
                }
                if (cmbGoods.Items.Count > 0)
                {
                    cmbGoods.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик изменения выбора товара
        private void cmbGoods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGoods.SelectedItem != null)
            {
                int goodId = ((KeyValuePair<int, string>)cmbGoods.SelectedItem).Key;
                DisplayDemandGraph(goodId);
            }
        }

        // Метод, вызываемый при загрузке формы
        private void DemandGraphForm_Load(object sender, EventArgs e)
        {
            LoadGoodsList();

            if (cmbGoods.Items.Count > 0)
            {
                int goodId = ((KeyValuePair<int, string>)cmbGoods.Items[0]).Key;
                DisplayDemandGraph(goodId);
            }
        }
    }
}
