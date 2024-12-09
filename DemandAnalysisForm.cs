using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

public partial class DemandAnalysisForm : Form
{
    private string connectionString;

    // Конструктор с передачей строки подключения как параметра
    public DemandAnalysisForm(string connectionString)
    {
        InitializeComponent();
        this.connectionString = connectionString;
        LoadGoods(); // Загрузка товаров в ComboBox при инициализации
    }

    private void LoadGoods()
    {
        // Заполняем ComboBox товарами
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT id, name FROM goods ORDER BY name", connection))
                using (var reader = command.ExecuteReader())
                {
                    cmbGoods.Items.Clear();  // Очищаем старые элементы, если они есть

                    while (reader.Read())
                    {
                        // Добавляем каждый товар как объект с двумя свойствами: Text и Value
                        var good = new { Text = reader.GetString(1), Value = reader.GetInt32(0) };
                        cmbGoods.Items.Add(good);  // Добавляем анонимный объект в ComboBox
                    }

                    cmbGoods.DisplayMember = "Text";  // Указываем, что будет отображаться в ComboBox
                    cmbGoods.ValueMember = "Value";  // Указываем, что будет значением
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}");
        }
    }

    private void btnAnalyze_Click(object sender, EventArgs e)
    {
        if (cmbGoods.SelectedItem == null)
        {
            MessageBox.Show("Пожалуйста, выберите товар.");
            return;
        }

        var selectedGood = (dynamic)cmbGoods.SelectedItem;
        int goodId = selectedGood.Value; // Получаем Value, которое является id товара
        DateTime startDate = dtpStartDate.Value;
        DateTime endDate = dtpEndDate.Value;

        AnalyzeDemand(goodId, startDate, endDate);
    }

    private void AnalyzeDemand(int goodId, DateTime startDate, DateTime endDate)
    {
        string query = @"
            SELECT s.create_date, SUM(s.good_count) AS total_sales
            FROM sales s
            WHERE s.good_id = @GoodId
            AND s.create_date BETWEEN @StartDate AND @EndDate
            GROUP BY s.create_date
            ORDER BY s.create_date";

        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    // Добавляем параметры для предотвращения SQL-инъекций
                    command.Parameters.AddWithValue("@GoodId", goodId);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader); // Загружаем результат в DataTable
                        dgvResult.DataSource = dt; // Устанавливаем DataSource для DataGridView
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при анализе спроса: {ex.Message}");
        }
    }
}
