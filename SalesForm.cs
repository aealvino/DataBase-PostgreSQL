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
            this.saleId = id;

            ConfigureForm();
            LoadGoodsData();
            cmbWarehouse.SelectedIndex = 0; // По умолчанию выбран Склад 1
        }
        private void ConfigureForm()
        {
            // Обновляем отображение элементов формы в зависимости от действия
            lblGood.Visible = true;
            lblCount.Visible = true;
            lblDate.Visible = true;
            txtCount.Visible = true;
            cmbGoods.Visible = true;
            dtpDate.Visible = true;
            txtSaleId.Visible = false;
            btnSave.Visible = true; // Кнопка "Сохранить" видима только для добавления и редактирования
            btnDelete.Visible = false;
            cmbWarehouse.Visible = true; // Скрываем выбор склада только для удаления
            lblWarehouse.Visible = true; // Скрываем надпись о складе только для удаления

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
                this.Text = "Удалить продажу";
                lblGood.Visible = false;
                lblCount.Visible = false;
                lblDate.Visible = false;
                cmbGoods.Visible = false;
                txtCount.Visible = false;
                dtpDate.Visible = false;
                txtSaleId.Visible = true; // Поле для ID заявки
                txtSaleId.ReadOnly = false; // Сделать поле ID доступным для ввода
                btnSave.Visible = false; // Скрываем кнопку "Сохранить"
                btnDelete.Visible = true; // Отображаем кнопку "Удалить"
                cmbWarehouse.Visible = false; // Скрываем выбор склада
                lblWarehouse.Visible = false; // Скрываем метку склада
                                              // Центрируем кнопку "Удалить"
                btnDelete.Location = new Point((this.ClientSize.Width - btnDelete.Width) / 2, btnDelete.Location.Y);
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
            string query = @"
        SELECT g.id, g.name
        FROM goods g
        LEFT JOIN warehouse1 w1 ON g.id = w1.good_id
        LEFT JOIN warehouse2 w2 ON g.id = w2.good_id";

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

                            // Добавляем товар в комбобокс
                            cmbGoods.Items.Add(new { Id = goodId, Name = goodName });
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
            if (warehouse == "warehouse2")
            {
                int stockWarehouse1 = GetStock(goodId, "warehouse1");
                if (stockWarehouse1 > 0)
                {
                    throw new InvalidOperationException("Списание с второго склада запрещено, пока товар есть на первом складе.");
                }
            }

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
                        MessageBox.Show($"Не удалось обновить склад {warehouse}. Возможно, товара недостаточно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (string.IsNullOrEmpty(txtCount.Text) || !int.TryParse(txtCount.Text, out int count) || count <= 0)
            {
                MessageBox.Show("Введите корректное количество товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        string selectedWarehouse = cmbWarehouse.SelectedItem.ToString();
                        int remainingCount = count;

                        // Проверка остатков на складах перед добавлением записи в таблицу sales
                        switch (selectedWarehouse)
                        {
                            case "Склад 1":
                            if (GetStock(goodId, "warehouse1") < remainingCount)
                            {
                                MessageBox.Show("Недостаточно товара на складе 1 для выполнения продажи!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                                return;
                            }
                            break;

                            case "Склад 2":
                            if (GetStock(goodId, "warehouse2") < remainingCount)
                            {
                                MessageBox.Show("Недостаточно товара на складе 2 для выполнения продажи!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                                return;
                            }
                            break;

                            case "Оба склада":
                            int stockWarehouse1 = GetStock(goodId, "warehouse1");
                            if (stockWarehouse1 >= remainingCount)
                            {
                                remainingCount = 0;
                            }
                            else
                            {
                                remainingCount -= stockWarehouse1;
                            }

                            if (remainingCount > 0)
                            {
                                int stockWarehouse2 = GetStock(goodId, "warehouse2");
                                if (stockWarehouse2 >= remainingCount)
                                {
                                    remainingCount = 0;
                                }
                                else
                                {
                                    MessageBox.Show("Недостаточно товара на складах для выполнения продажи!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    transaction.Rollback();
                                    return;
                                }
                            }
                            break;

                            default:
                            MessageBox.Show("Некорректный выбор склада!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            transaction.Rollback();
                            return;
                        }

                        // После проверки остатков выполняем вставку записи в таблицу sales
                        string insertQuery = "INSERT INTO sales (good_id, good_count, create_date) VALUES (@good_id, @good_count, @create_date)";
                        using (var command = new NpgsqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@good_id", goodId);
                            command.Parameters.AddWithValue("@good_count", count);
                            command.Parameters.AddWithValue("@create_date", dtpDate.Value);
                            command.Transaction = transaction;
                            command.ExecuteNonQuery();
                        }

                        // Обновление остатков после вставки записи
                        switch (selectedWarehouse)
                        {
                            case "Склад 1":
                            UpdateStock(goodId, count, "warehouse1");
                            break;

                            case "Склад 2":
                            UpdateStock(goodId, count, "warehouse2");
                            break;

                            case "Оба склада":
                            // Списываем товар сначала с одного склада, затем с другого
                            if (GetStock(goodId, "warehouse1") >= count)
                            {
                                UpdateStock(goodId, count, "warehouse1");
                            }
                            else
                            {
                                int stockWarehouse1 = GetStock(goodId, "warehouse1");
                                UpdateStock(goodId, stockWarehouse1, "warehouse1");
                                int remaining = count - stockWarehouse1;
                                UpdateStock(goodId, remaining, "warehouse2");
                            }
                            break;
                        }

                        transaction.Commit();
                        MessageBox.Show("Продажа успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SalesUpdated?.Invoke();
                        this.Close();
                    }
                }
                catch (PostgresException ex)
                {
                    MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private bool CheckStockBeforeUpdate(int goodId, int quantity)
        {
            int totalStock = GetTotalStock(goodId);

            if (quantity > totalStock)
            {
                MessageBox.Show("Недостаточно товара на складах!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
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

            // Проверяем наличие товара на первом складе
            int stockWarehouse1 = GetStock(goodId, "warehouse1");
            if (stockWarehouse1 > 0)
            {
                // Сначала списываем со склада 1
                UpdateStock(goodId, Math.Min(stockWarehouse1, remaining), "warehouse1");
                remaining -= Math.Min(stockWarehouse1, remaining);
            }

            if (remaining > 0)
            {
                // Если товар есть на втором складе, но на первом складе тоже оставался товар,
                // блокируем списание с второго склада
                if (stockWarehouse1 > 0)
                {
                    MessageBox.Show("Невозможно списать товар со второго склада, пока он есть на первом складе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new InvalidOperationException("Списание с второго склада запрещено, пока товар есть на первом складе.");
                }

                // Списываем остаток со склада 2
                UpdateStock(goodId, remaining, "warehouse2");
            }
        }

    }
}
