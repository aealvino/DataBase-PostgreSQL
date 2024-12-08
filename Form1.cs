using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace DataBass
{
    public partial class Form1 : Form
    {
        private Config config;
        private string connectionString;
        private string UserRole;

        public Form1(string role)
        {
            InitializeComponent();
            UserRole = role; // Устанавливаем роль пользователя
            lblUserRole.Text = $"Role: {UserRole}"; // Отображаем роль

            // Подключаем обработчик события Resize
            this.Resize += Form1_Resize;

            // Настраиваем соединение
            Config config = new Config("config.ini");
            connectionString = $"Host={config.Host};Port={config.Port};Database={config.Database};Username={config.Username};Password={config.Password};";

            // Добавляем кнопки на вкладки
            AddCommonButtons(tabManagerLog);
            AddCommonButtons(tabGoods);
            AddCommonButtons(tabSales);

            // Загрузка данных
            LoadData();
        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            AdjustLayout(); // Перераспределение элементов при изменении размера
        }

        private void AdjustLayout()
        {
            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            // Настраиваем размеры TabControl
            tabControl.Size = new Size(formWidth, formHeight);

            // Настраиваем элементы внутри каждой вкладки
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                int buttonAreaWidth = (int)(formWidth * 0.35); // Отводим 35% ширины под кнопки
                int dataGridWidth = formWidth - buttonAreaWidth - 30;

                foreach (Control control in tabPage.Controls)
                {
                    if (control is DataGridView dgv)
                    {
                        dgv.Size = new Size(dataGridWidth, formHeight - 20);
                        dgv.Location = new Point(10, 10);
                    }
                    else if (control is Button button)
                    {
                        int buttonWidth = buttonAreaWidth - 20; // Уменьшаем ширину кнопки, чтобы оставить отступы
                        int buttonHeight = 60; // Высота кнопки
                        int buttonSpacing = 10; // Расстояние между кнопками

                        // Расчет позиций кнопок
                        int buttonIndex = tabPage.Controls.IndexOf(button) - 1; // Индекс кнопки
                        button.Size = new Size(buttonWidth, buttonHeight);

                        // Расположение кнопок с учетом их порядка
                        int buttonX = formWidth - buttonAreaWidth + 10;
                        int buttonY = 10 + buttonIndex * (buttonHeight + buttonSpacing);

                        button.Location = new Point(buttonX, buttonY);
                    }
                }
            }
        }


        private void LoadData()
        {
            try
            {
                // Загружаем данные для каждой вкладки
                LoadManagerLogData();
                LoadGoodsData();
                LoadSalesData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadManagerLogData()
        {
            string query = @"
                SELECT 
                    g.id AS ТоварID, 
                    g.name AS НазваниеТовара, 
                    COALESCE(w1.good_count, 0) AS Склад1, 
                    COALESCE(w2.good_count, 0) AS Склад2, 
                    COALESCE(w1.good_count, 0) + COALESCE(w2.good_count, 0) AS ОбщееКоличество
                FROM goods g
                LEFT JOIN warehouse1 w1 ON g.id = w1.good_id
                LEFT JOIN warehouse2 w2 ON g.id = w2.good_id;";

            dgvManagerLog.DataSource = GetDataTable(query);
        }

        public void LoadGoodsData()
        {
            string query = "SELECT id AS ID, name AS Название, priority AS Приоритет FROM goods;";
            dataGridView1.DataSource = GetDataTable(query);
        }

        private void LoadSalesData()
        {
            string query = @"
                SELECT 
                    s.id AS ПродажаID, 
                    g.name AS НазваниеТовара, 
                    s.good_count AS Количество, 
                    s.create_date AS ДатаПродажи
                FROM sales s
                JOIN goods g ON s.good_id = g.id;";
            dataGridView2.DataSource = GetDataTable(query);
        }

        private DataTable GetDataTable(string query)
        {
            DataTable dataTable = new DataTable();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabManagerLog)
            {
                // Открыть форму ManagerForm для добавления товара на склад
                ManagerForm managerForm = new ManagerForm(connectionString);
                managerForm.ShowDialog();
                LoadManagerLogData();
            }
            else if (tabControl.SelectedTab == tabGoods)
            {
                // Логика для добавления товара
                GoodForm addGoodForm = new GoodForm(connectionString, FormAction.Add);
                addGoodForm.GoodUpdated += () => LoadGoodsData();
                addGoodForm.ShowDialog();
            }
            else if (tabControl.SelectedTab == tabSales)
            {
                // Логика для добавления продажи
                SalesForm addSalesForm = new SalesForm(connectionString, FormAction.Add);
                addSalesForm.SalesUpdated += () => LoadSalesData();
                addSalesForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Добавление в этой вкладке не поддерживается.");
            }
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabManagerLog)
            {
                // Проверяем, что выбрана строка для удаления
                if (dgvManagerLog.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите товар для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем ID товара из выбранной строки
                int selectedProductId = Convert.ToInt32(dgvManagerLog.SelectedRows[0].Cells["ТоварID"].Value);

                // Запрос для удаления товара из базы данных
                string deleteQuery = @"
            DELETE FROM goods
            WHERE id = @ProductId;
        ";

                try
                {
                    // Выполнение запроса на удаление
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        using (NpgsqlCommand command = new NpgsqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ProductId", selectedProductId);
                            command.ExecuteNonQuery();
                        }
                    }

                    // Обновляем данные в таблице после удаления
                    LoadManagerLogData();
                    MessageBox.Show("Товар успешно удален.", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (tabControl.SelectedTab == tabGoods)
            {
                // Логика для удаления товара
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите товар для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                GoodForm deleteGoodForm = new GoodForm(connectionString, FormAction.Delete, selectedId);
                deleteGoodForm.GoodUpdated += () => LoadGoodsData();
                deleteGoodForm.ShowDialog();
            }
            else if (tabControl.SelectedTab == tabSales)
            {
                // Логика для удаления продажи
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите продажу для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int selectedSaleId = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["ПродажаID"].Value);
                SalesForm deleteSalesForm = new SalesForm(connectionString, FormAction.Delete, selectedSaleId);
                deleteSalesForm.SalesUpdated += () => LoadSalesData();
                deleteSalesForm.ShowDialog();
            }
            else
            {
                // Логика для других вкладок
                MessageBox.Show("Удаление в этой вкладке не поддерживается.");
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabManagerLog)
            {
                // Проверяем, что выбрана строка для редактирования
                if (dgvManagerLog.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите товар для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем ID товара и склад из выбранной строки
                int goodId = Convert.ToInt32(dgvManagerLog.SelectedRows[0].Cells["ТоварID"].Value);
                string warehouse1Count = dgvManagerLog.SelectedRows[0].Cells["Склад1"].Value.ToString();
                string warehouse2Count = dgvManagerLog.SelectedRows[0].Cells["Склад2"].Value.ToString();

                // Преобразуем строки в целые числа
                int warehouse1CountInt;
                int warehouse2CountInt;

                if (int.TryParse(warehouse1Count, out warehouse1CountInt) && int.TryParse(warehouse2Count, out warehouse2CountInt))
                {
                    // Создаем форму для редактирования товара на складе
                    ManagerForm editManagerForm = new ManagerForm(connectionString);
                    editManagerForm.Text = "Редактировать товар на складе"; // Устанавливаем название формы

                    // Передаем данные в форму
                    editManagerForm.SetInitialValues(goodId, warehouse1CountInt, warehouse2CountInt);

                    // Обработчик для обновления данных после закрытия формы
                    editManagerForm.FormClosed += (s, args) =>
                    {
                        // Обновляем данные в таблице после редактирования
                        LoadManagerLogData();
                    };

                    // Показываем форму в модальном режиме
                    editManagerForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Ошибка при преобразовании данных в целое число.");
                }
            }
            else if (tabControl.SelectedTab == tabGoods)
            {
                // Логика для редактирования товара
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите товар для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int goodId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                GoodForm editGoodForm = new GoodForm(connectionString, FormAction.Edit, goodId);
                editGoodForm.GoodUpdated += () => LoadGoodsData();
                editGoodForm.ShowDialog();
            }
            else if (tabControl.SelectedTab == tabSales)
            {
                // Логика для редактирования продажи
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите продажу для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int saleId = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["ПродажаID"].Value);
                SalesForm editSalesForm = new SalesForm(connectionString, FormAction.Edit, saleId);
                editSalesForm.SalesUpdated += () => LoadSalesData();
                editSalesForm.ShowDialog();
            }
            else
            {
                // Логика для других вкладок
                MessageBox.Show("Редактирование в этой вкладке не поддерживается.");
            }
        }







        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}