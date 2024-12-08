using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace DataBass
{
    public partial class ManagerForm : Form
    {
        private string connectionString;

        public ManagerForm(string connectionString)
        {
            InitializeComponent();

            // Устанавливаем максимальное значение для NumericUpDown
            nudQuantity.Maximum = decimal.MaxValue;

            this.connectionString = connectionString;
            LoadData(); // Загрузка данных о товарах и складах
        }

        // Метод для загрузки данных о складах и товарах
        private void LoadData()
        {
            // Загрузка данных о складах (сделаем их фиксированными для складов 1 и 2)
            cmbWarehouse.Items.Add(new { Id = 1, Name = "Склад 1" });
            cmbWarehouse.Items.Add(new { Id = 2, Name = "Склад 2" });

            // Загрузка данных о товарах
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var query = "SELECT id, name FROM goods";
                var cmd = new NpgsqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cmbProduct.Items.Add(new { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                }
                reader.Close();
            }
        }

        // Обработчик кнопки "Сохранить"
        private void btnSave_Click(object sender, EventArgs e)
        {
            var selectedWarehouse = cmbWarehouse.SelectedItem as dynamic;
            var selectedProduct = cmbProduct.SelectedItem as dynamic;

            if (selectedWarehouse != null && selectedProduct != null)
            {
                int warehouseId = selectedWarehouse.Id;
                int productId = selectedProduct.Id;
                nudQuantity.Maximum = decimal.MaxValue; // Устанавливаем максимальное значение
                int quantity = (int)nudQuantity.Value;  // Заменяем на Value, а не Maximum

                // Проверяем, что количество не отрицательное
                if (quantity <= 0)
                {
                    MessageBox.Show("Количество товара должно быть больше 0.");
                    return;
                }

                // Определяем, к какому складу относится запись (warehouse1 или warehouse2)
                string warehouseTable = warehouseId == 1 ? "warehouse1" : "warehouse2";

                // Добавление товара на выбранный склад
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    var query = $@"
                        INSERT INTO {warehouseTable} (good_id, good_count) 
                        VALUES (@productId, @quantity) 
                        ON CONFLICT (good_id) 
                        DO UPDATE SET good_count = {warehouseTable}.good_count + @quantity;
                    ";

                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Товар успешно добавлен на склад.");
                this.Close();  // Закрываем форму после успешного добавления

                // Обновляем таблицы в главной форме
                if (Owner is Form1 mainForm)
                {
                    mainForm.LoadManagerLogData();
                    mainForm.LoadGoodsData();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите склад и продукт.");
            }
        }

        // Обработчик кнопки "Удалить"
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedWarehouse = cmbWarehouse.SelectedItem as dynamic;
            var selectedProduct = cmbProduct.SelectedItem as dynamic;

            if (selectedWarehouse != null && selectedProduct != null)
            {
                int warehouseId = selectedWarehouse.Id;
                int productId = selectedProduct.Id;

                // Определяем, к какому складу относится запись (warehouse1 или warehouse2)
                string warehouseTable = warehouseId == 1 ? "warehouse1" : "warehouse2";

                // Запрос на удаление товара с выбранного склада
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    var query = $@"
                        DELETE FROM {warehouseTable} 
                        WHERE good_id = @productId;
                    ";

                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Товар успешно удален с выбранного склада.");
                    }
                    else
                    {
                        MessageBox.Show("Товар не найден на выбранном складе.");
                    }
                }

                // Закрываем форму после удаления
                this.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите склад и продукт.");
            }
        }

        // Обработчик кнопки "Изменить" (для обновления информации о товаре и количестве)
        // Обработчик кнопки "Изменить" (для обновления информации о товаре и количестве)
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var selectedWarehouse = cmbWarehouse.SelectedItem as dynamic;
            var selectedProduct = cmbProduct.SelectedItem as dynamic;

            if (selectedWarehouse != null && selectedProduct != null)
            {
                int warehouseId = selectedWarehouse.Id;
                int productId = selectedProduct.Id;
                int quantity = (int)nudQuantity.Value;  // Получаем количество товара из NumericUpDown

                // Проверяем, что количество не отрицательное
                if (quantity <= 0)
                {
                    MessageBox.Show("Количество товара должно быть больше 0.");
                    return;
                }

                string warehouseTable = warehouseId == 1 ? "warehouse1" : "warehouse2";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // Обновление количества товара на складе
                    var query = $@"
                UPDATE {warehouseTable} 
                SET good_count = @quantity 
                WHERE good_id = @productId;
            ";

                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);  // Обновление количества товара
                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Информация о товаре на складе успешно обновлена.");
                    }
                    else
                    {
                        MessageBox.Show("Товар не найден на складе.");
                    }
                }

                // Закрываем форму после обновления
                this.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите склад и продукт.");
            }
        }

        public void SetInitialValues(int goodId, int warehouse1Count, int warehouse2Count)
        {
            // Устанавливаем начальное значение для товара
            cmbProduct.SelectedItem = cmbProduct.Items.Cast<dynamic>().FirstOrDefault(item => item.Id == goodId);

            // Устанавливаем количество товара на складе в зависимости от выбранного склада
            // Пример: если на первом складе больше, показываем склад 1
            if (warehouse1Count > 0)
            {
                cmbWarehouse.SelectedItem = cmbWarehouse.Items.Cast<dynamic>().FirstOrDefault(item => item.Id == 1);
                nudQuantity.Value = warehouse1Count;
            }
            else if (warehouse2Count > 0)
            {
                cmbWarehouse.SelectedItem = cmbWarehouse.Items.Cast<dynamic>().FirstOrDefault(item => item.Id == 2);
                nudQuantity.Value = warehouse2Count;
            }
        }

        // Обработчик кнопки "Отмена"
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
