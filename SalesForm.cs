using System;
using System.Windows.Forms;
using Npgsql;

namespace DataBass
{
    public partial class SalesForm : Form
    {
        private string connectionString;
        private FormAction currentAction;
        private int? saleId; // Для редактирования и удаления записи о продаже

        public event Action SalesUpdated; // Событие для уведомления основной формы

        public SalesForm(string connectionString, FormAction action, int? id = null)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            this.currentAction = action;
            this.saleId = id; // Для редактирования и удаления

            ConfigureForm();
            LoadGoodsData(); // Загружаем товары при открытии формы
        }

        private void ConfigureForm()
        {
            // Обновим отображение элементов формы в зависимости от действия
            lblGood.Visible = true;
            lblCount.Visible = true;
            lblDate.Visible = true;
            txtCount.Visible = true;
            cmbGoods.Visible = true;
            dtpDate.Visible = true;
            txtSaleId.Visible = false;
            btnSave.Visible = true;
            btnDelete.Visible = false;

            switch (currentAction)
            {
                case FormAction.Add:
                this.Text = "Добавить продажу";
                lblGood.Text = "Товар:";
                lblCount.Text = "Количество:";
                lblDate.Text = "Дата продажи:";
                txtSaleId.Visible = false;
                lblSaleId.Visible = false;
                break;

                case FormAction.Delete:
                lblGood.Visible = false;
                lblCount.Visible = false;
                lblDate.Visible = false;
                cmbGoods.Visible = false;
                txtCount.Visible = false;
                dtpDate.Visible = false;
                btnDelete.Visible = true;
                txtSaleId.Visible = true;
                txtSaleId.ReadOnly = false; // Сделать поле ID доступным для ввода
                break;

                case FormAction.Edit:
                this.Text = "Изменить продажу";
                lblGood.Text = "Товар:";
                lblCount.Text = "Количество:";
                lblDate.Text = "Дата продажи:";
                break;
            }
        }

        private void LoadGoodsData()
        {
            // Загружаем данные о товарах, которые есть на складе (их количество > 0)
            string query = @"
                SELECT g.id, g.name, 
                    (COALESCE(w1.good_count, 0) + COALESCE(w2.good_count, 0)) AS total_stock
                FROM goods g
                LEFT JOIN warehouse1 w1 ON g.id = w1.good_id
                LEFT JOIN warehouse2 w2 ON g.id = w2.good_id
                WHERE (COALESCE(w1.good_count, 0) + COALESCE(w2.good_count, 0)) > 0";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var goodId = reader.GetInt32(reader.GetOrdinal("id"));
                            var goodName = reader.GetString(reader.GetOrdinal("name"));
                            var totalStock = reader.GetInt32(reader.GetOrdinal("total_stock"));

                            // Добавляем товар в комбобокс
                            cmbGoods.Items.Add(new { Id = goodId, Name = goodName, Stock = totalStock });
                        }
                    }
                }
            }

            cmbGoods.DisplayMember = "Name";
            cmbGoods.ValueMember = "Id";
        }

        private void ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    int rowsAffected = command.ExecuteNonQuery(); // Добавьте проверку на успешность выполнения
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("Не удалось выполнить операцию с базой данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void UpdateWarehouseStock(int goodId, int quantitySold)
        {
            string updateWarehouseQuery = @"
        -- Обновляем склад 1
        UPDATE warehouse1 
        SET good_count = good_count - @QuantitySold
        WHERE good_id = @GoodId AND good_count >= @QuantitySold;

        -- Обновляем склад 2
        UPDATE warehouse2 
        SET good_count = good_count - @QuantitySold
        WHERE good_id = @GoodId AND good_count >= @QuantitySold;
    ";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(updateWarehouseQuery, connection))
                {
                    command.Parameters.AddWithValue("@GoodId", goodId);
                    command.Parameters.AddWithValue("@QuantitySold", quantitySold);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateStock(int goodId, int countToDeduct, string warehouse)
        {
            string updateQuery = warehouse == "warehouse1"
                ? "UPDATE warehouse1 SET good_count = good_count - @count WHERE good_id = @good_id AND good_count >= @count;"
                : "UPDATE warehouse2 SET good_count = good_count - @count WHERE good_id = @good_id AND good_count >= @count;";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(updateQuery, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@count", countToDeduct));
                    command.Parameters.Add(new NpgsqlParameter("@good_id", goodId));

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Не удалось обновить склад {warehouse}. Возможно, товара недостаточно.");
                    }
                }
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                switch (currentAction)
                {
                    case FormAction.Add:
                    SaveSale();
                    break;
                    case FormAction.Edit:
                    EditSale();
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении операции:\n{ex.ToString()}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentAction == FormAction.Delete)
                    DeleteSale();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveSale()
        {
            if (cmbGoods.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для продажи!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedGood = (dynamic)cmbGoods.SelectedItem;
            int goodId = selectedGood.Id;
            string goodName = selectedGood.Name;
            int stockAvailable = selectedGood.Stock;

            // Проверка на наличие корректного количества товара
            if (string.IsNullOrEmpty(txtCount.Text) || !int.TryParse(txtCount.Text, out int count) || count <= 0)
            {
                MessageBox.Show("Введите корректное количество товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Запрос для получения данных о товаре на складах
            string query = @"
        SELECT 
            COALESCE(w1.good_count, 0) AS warehouse1_stock,
            COALESCE(w2.good_count, 0) AS warehouse2_stock,
            (COALESCE(w1.good_count, 0) + COALESCE(w2.good_count, 0)) AS total_stock
        FROM goods g
        LEFT JOIN warehouse1 w1 ON g.id = w1.good_id
        LEFT JOIN warehouse2 w2 ON g.id = w2.good_id
        WHERE g.id = @good_id";

            int warehouse1Stock = 0, warehouse2Stock = 0, totalStock = 0;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@good_id", goodId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            warehouse1Stock = reader.GetInt32(reader.GetOrdinal("warehouse1_stock"));
                            warehouse2Stock = reader.GetInt32(reader.GetOrdinal("warehouse2_stock"));
                            totalStock = reader.GetInt32(reader.GetOrdinal("total_stock"));
                        }
                    }
                }
            }

            // Логика проверки, есть ли достаточное количество товара
            if (count > totalStock)
            {
                MessageBox.Show("Недостаточно товара на складах!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Добавляем запись о продаже
            string insertQuery = "INSERT INTO sales (good_id, good_count, create_date) VALUES (@good_id, @good_count, @create_date) RETURNING id";
            int saleId;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@good_id", goodId);
                    command.Parameters.AddWithValue("@good_count", count);
                    command.Parameters.AddWithValue("@create_date", dtpDate.Value);

                    // Получаем id вставленной продажи
                    saleId = (int)command.ExecuteScalar();
                }
            }

            // После успешного добавления продажи обновляем склады
            int countToDeduct = count;

            // Сначала уменьшаем товар на складе 1, если возможно
            if (countToDeduct <= warehouse1Stock)
            {
                UpdateStock(goodId, countToDeduct, "warehouse1");
            }
            else
            {
                // Уменьшаем товар на складе 1 до его нуля, остаток забираем со склада 2
                countToDeduct -= warehouse1Stock;
                UpdateStock(goodId, warehouse1Stock, "warehouse1");

                // Теперь уменьшаем остаток товара на складе 2
                if (countToDeduct <= warehouse2Stock)
                {
                    UpdateStock(goodId, countToDeduct, "warehouse2");
                }
                else
                {
                    // Если на складе 2 недостаточно товара, откатываем изменения в базе
                    MessageBox.Show("Недостаточно товара на складе 2!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Удаляем запись о продаже, так как товар не был полностью списан
                    string deleteSaleQuery = "DELETE FROM sales WHERE id = @sale_id";
                    ExecuteNonQuery(deleteSaleQuery, new NpgsqlParameter("@sale_id", saleId));

                    return;
                }
            }

            MessageBox.Show("Продажа успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Если после добавления продажи требуется обновить основной интерфейс
            SalesUpdated?.Invoke();
            this.Close();
        }

        private void DeleteSale()
        {
            if (string.IsNullOrEmpty(txtSaleId.Text))
            {
                MessageBox.Show("Введите ID продажи для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id;
            if (!int.TryParse(txtSaleId.Text, out id))
            {
                MessageBox.Show("Некорректный ID продажи!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "DELETE FROM sales WHERE id = @sale_id";
            ExecuteNonQuery(query, new NpgsqlParameter("@sale_id", id));

            MessageBox.Show("Продажа успешно удалена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SalesUpdated?.Invoke();
            this.Close();
        }
        private void EditSale()
        {
            if (!saleId.HasValue)
            {
                MessageBox.Show("ID продажи не задан!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cmbGoods.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для продажи!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedGood = (dynamic)cmbGoods.SelectedItem;
            int goodId = selectedGood.Id;
            int newCount = int.Parse(txtCount.Text);
            DateTime newDate = dtpDate.Value;

            // Загрузка текущей информации о продаже
            string selectSaleQuery = "SELECT good_id, good_count FROM sales WHERE id = @sale_id";
            int currentGoodId = 0, currentCount = 0;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(selectSaleQuery, connection))
                {
                    command.Parameters.AddWithValue("@sale_id", saleId.Value);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentGoodId = reader.GetInt32(reader.GetOrdinal("good_id"));
                            currentCount = reader.GetInt32(reader.GetOrdinal("good_count"));
                        }
                    }
                }
            }

            if (currentGoodId == 0)
            {
                MessageBox.Show("Продажа не найдена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Возвращаем остаток текущего товара на склад
            if (currentCount > 0)
            {
                RestoreStock(currentGoodId, currentCount);
            }

            // Проверяем наличие достаточного количества нового товара
            int totalStock = GetTotalStock(goodId);

            if (newCount > totalStock)
            {
                MessageBox.Show("Недостаточно товара на складах!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Обновляем запись о продаже
            string updateSaleQuery = "UPDATE sales SET good_id = @good_id, good_count = @good_count, create_date = @create_date WHERE id = @sale_id";

            ExecuteNonQuery(updateSaleQuery,
                new NpgsqlParameter("@good_id", goodId),
                new NpgsqlParameter("@good_count", newCount),
                new NpgsqlParameter("@create_date", newDate),
                new NpgsqlParameter("@sale_id", saleId.Value));

            // Списываем товар с нового склада
            DeductStock(goodId, newCount);

            MessageBox.Show("Продажа успешно обновлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SalesUpdated?.Invoke();
            this.Close();
        }
        private void RestoreStock(int goodId, int count)
        {
            int remaining = count;

            // Сначала возвращаем на склад 1
            UpdateStock(goodId, -Math.Min(GetStock(goodId, "warehouse1"), remaining), "warehouse1");
            remaining -= Math.Min(GetStock(goodId, "warehouse1"), remaining);

            if (remaining > 0)
            {
                // Остаток возвращаем на склад 2
                UpdateStock(goodId, -remaining, "warehouse2");
            }
        }
        private int GetTotalStock(int goodId)
        {
            string query = @"
        SELECT 
            COALESCE(w1.good_count, 0) + COALESCE(w2.good_count, 0) AS total_stock
        FROM goods g
        LEFT JOIN warehouse1 w1 ON g.id = w1.good_id
        LEFT JOIN warehouse2 w2 ON g.id = w2.good_id
        WHERE g.id = @good_id";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@good_id", goodId);
                    return (int)command.ExecuteScalar();
                }
            }
        }
        private int GetStock(int goodId, string warehouse)
        {
            string query = warehouse == "warehouse1"
                ? "SELECT COALESCE(good_count, 0) FROM warehouse1 WHERE good_id = @good_id"
                : "SELECT COALESCE(good_count, 0) FROM warehouse2 WHERE good_id = @good_id";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@good_id", goodId);

                    var result = command.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private void DeductStock(int goodId, int count)
        {
            int remaining = count;

            // Сначала списываем со склада 1
            UpdateStock(goodId, Math.Min(GetStock(goodId, "warehouse1"), remaining), "warehouse1");
            remaining -= Math.Min(GetStock(goodId, "warehouse1"), remaining);

            if (remaining > 0)
            {
                // Остаток списываем со склада 2
                UpdateStock(goodId, remaining, "warehouse2");
            }
        }


    }
}
