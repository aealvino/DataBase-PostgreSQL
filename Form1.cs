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
            UserRole = role; // ������������� ���� ������������
            lblUserRole.Text = $"Role: {UserRole}"; // ���������� ����

            // ���������� ���������� ������� Resize
            this.Resize += Form1_Resize;

            // ����������� ����������
            Config config = new Config("config.ini");
            connectionString = $"Host={config.Host};Port={config.Port};Database={config.Database};Username={config.Username};Password={config.Password};";

            // ��������� ������ �� �������
            AddCommonButtons(tabManagerLog);
            AddCommonButtons(tabGoods);
            AddCommonButtons(tabSales);

            // �������� ������
            LoadData();
        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            AdjustLayout(); // ����������������� ��������� ��� ��������� �������
        }

        private void AdjustLayout()
        {
            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            // ����������� ������� TabControl
            tabControl.Size = new Size(formWidth, formHeight);

            // ����������� �������� ������ ������ �������
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                int buttonAreaWidth = (int)(formWidth * 0.35); // ������� 35% ������ ��� ������
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
                        int buttonWidth = buttonAreaWidth - 20; // ��������� ������ ������, ����� �������� �������
                        int buttonHeight = 60; // ������ ������
                        int buttonSpacing = 10; // ���������� ����� ��������

                        // ������ ������� ������
                        int buttonIndex = tabPage.Controls.IndexOf(button) - 1; // ������ ������
                        button.Size = new Size(buttonWidth, buttonHeight);

                        // ������������ ������ � ������ �� �������
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
                // ��������� ������ ��� ������ �������
                LoadManagerLogData();
                LoadGoodsData();
                LoadSalesData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ �������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadManagerLogData()
        {
            string query = @"
                SELECT 
                    g.id AS �����ID, 
                    g.name AS ��������������, 
                    COALESCE(w1.good_count, 0) AS �����1, 
                    COALESCE(w2.good_count, 0) AS �����2, 
                    COALESCE(w1.good_count, 0) + COALESCE(w2.good_count, 0) AS ���������������
                FROM goods g
                LEFT JOIN warehouse1 w1 ON g.id = w1.good_id
                LEFT JOIN warehouse2 w2 ON g.id = w2.good_id;";

            dgvManagerLog.DataSource = GetDataTable(query);
        }

        public void LoadGoodsData()
        {
            string query = "SELECT id AS ID, name AS ��������, priority AS ��������� FROM goods;";
            dataGridView1.DataSource = GetDataTable(query);
        }

        private void LoadSalesData()
        {
            string query = @"
                SELECT 
                    s.id AS �������ID, 
                    g.name AS ��������������, 
                    s.good_count AS ����������, 
                    s.create_date AS �����������
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
                // ������� ����� ManagerForm ��� ���������� ������ �� �����
                ManagerForm managerForm = new ManagerForm(connectionString);
                managerForm.ShowDialog();
                LoadManagerLogData();
            }
            else if (tabControl.SelectedTab == tabGoods)
            {
                // ������ ��� ���������� ������
                GoodForm addGoodForm = new GoodForm(connectionString, FormAction.Add);
                addGoodForm.GoodUpdated += () => LoadGoodsData();
                addGoodForm.ShowDialog();
            }
            else if (tabControl.SelectedTab == tabSales)
            {
                // ������ ��� ���������� �������
                SalesForm addSalesForm = new SalesForm(connectionString, FormAction.Add);
                addSalesForm.SalesUpdated += () => LoadSalesData();
                addSalesForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("���������� � ���� ������� �� ��������������.");
            }
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabManagerLog)
            {
                // ���������, ��� ������� ������ ��� ��������
                if (dgvManagerLog.SelectedRows.Count == 0)
                {
                    MessageBox.Show("�������� ����� ��� ��������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // �������� ID ������ �� ��������� ������
                int selectedProductId = Convert.ToInt32(dgvManagerLog.SelectedRows[0].Cells["�����ID"].Value);

                // ������ ��� �������� ������ �� ���� ������
                string deleteQuery = @"
            DELETE FROM goods
            WHERE id = @ProductId;
        ";

                try
                {
                    // ���������� ������� �� ��������
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        using (NpgsqlCommand command = new NpgsqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ProductId", selectedProductId);
                            command.ExecuteNonQuery();
                        }
                    }

                    // ��������� ������ � ������� ����� ��������
                    LoadManagerLogData();
                    MessageBox.Show("����� ������� ������.", "��������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ��� �������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (tabControl.SelectedTab == tabGoods)
            {
                // ������ ��� �������� ������
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("�������� ����� ��� ��������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                GoodForm deleteGoodForm = new GoodForm(connectionString, FormAction.Delete, selectedId);
                deleteGoodForm.GoodUpdated += () => LoadGoodsData();
                deleteGoodForm.ShowDialog();
            }
            else if (tabControl.SelectedTab == tabSales)
            {
                // ������ ��� �������� �������
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("�������� ������� ��� ��������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int selectedSaleId = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["�������ID"].Value);
                SalesForm deleteSalesForm = new SalesForm(connectionString, FormAction.Delete, selectedSaleId);
                deleteSalesForm.SalesUpdated += () => LoadSalesData();
                deleteSalesForm.ShowDialog();
            }
            else
            {
                // ������ ��� ������ �������
                MessageBox.Show("�������� � ���� ������� �� ��������������.");
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabManagerLog)
            {
                // ���������, ��� ������� ������ ��� ��������������
                if (dgvManagerLog.SelectedRows.Count == 0)
                {
                    MessageBox.Show("����������, �������� ����� ��� ��������������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // �������� ID ������ � ����� �� ��������� ������
                int goodId = Convert.ToInt32(dgvManagerLog.SelectedRows[0].Cells["�����ID"].Value);
                string warehouse1Count = dgvManagerLog.SelectedRows[0].Cells["�����1"].Value.ToString();
                string warehouse2Count = dgvManagerLog.SelectedRows[0].Cells["�����2"].Value.ToString();

                // ����������� ������ � ����� �����
                int warehouse1CountInt;
                int warehouse2CountInt;

                if (int.TryParse(warehouse1Count, out warehouse1CountInt) && int.TryParse(warehouse2Count, out warehouse2CountInt))
                {
                    // ������� ����� ��� �������������� ������ �� ������
                    ManagerForm editManagerForm = new ManagerForm(connectionString);
                    editManagerForm.Text = "������������� ����� �� ������"; // ������������� �������� �����

                    // �������� ������ � �����
                    editManagerForm.SetInitialValues(goodId, warehouse1CountInt, warehouse2CountInt);

                    // ���������� ��� ���������� ������ ����� �������� �����
                    editManagerForm.FormClosed += (s, args) =>
                    {
                        // ��������� ������ � ������� ����� ��������������
                        LoadManagerLogData();
                    };

                    // ���������� ����� � ��������� ������
                    editManagerForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("������ ��� �������������� ������ � ����� �����.");
                }
            }
            else if (tabControl.SelectedTab == tabGoods)
            {
                // ������ ��� �������������� ������
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("����������, �������� ����� ��� ��������������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int goodId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                GoodForm editGoodForm = new GoodForm(connectionString, FormAction.Edit, goodId);
                editGoodForm.GoodUpdated += () => LoadGoodsData();
                editGoodForm.ShowDialog();
            }
            else if (tabControl.SelectedTab == tabSales)
            {
                // ������ ��� �������������� �������
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("����������, �������� ������� ��� ��������������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int saleId = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["�������ID"].Value);
                SalesForm editSalesForm = new SalesForm(connectionString, FormAction.Edit, saleId);
                editSalesForm.SalesUpdated += () => LoadSalesData();
                editSalesForm.ShowDialog();
            }
            else
            {
                // ������ ��� ������ �������
                MessageBox.Show("�������������� � ���� ������� �� ��������������.");
            }
        }







        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}