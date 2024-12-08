using System;
using System.Windows.Forms;
using System.Xml.Linq;
using Npgsql;

namespace DataBass
{
    public enum FormAction
    {
        Add,
        Delete,
        Edit
    }

    public partial class GoodForm : Form
    {
        private string connectionString;
        private FormAction currentAction;
        private int? goodId; // Для редактирования и удаления товара

        public event Action GoodUpdated; // Событие для уведомления основной формы

        public GoodForm(string connectionString, FormAction action, int? id = null)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            this.currentAction = action;
            this.goodId = id; // Для редактирования и удаления

            ConfigureForm();
            if (goodId.HasValue && currentAction == FormAction.Edit)
            {
                LoadGoodData(goodId.Value); // Загружаем данные товара для редактирования
            }
        }

        private void ConfigureForm()
        {
            // Обновим отображение элементов формы в зависимости от действия
            lblName.Visible = true;
            lblPriority.Visible = true;
            txtName.Visible = true;
            txtPriority.Visible = true;
            txtId.Visible = false;
            btnSave.Visible = true;
            btnDelete.Visible = false;

            switch (currentAction)
            {
                case FormAction.Add:
                this.Text = "Добавить товар";
                lblName.Text = "Название:";
                lblPriority.Text = "Приоритет:";
                txtId.Visible = false;
                lblId.Visible = false;
                break;

                case FormAction.Delete:
                lblPriority.Visible = false;
                txtPriority.Visible = false;
                txtId.Visible = true;
                btnDelete.Visible = true;
                lblName.Visible = false;
                txtName.Visible = false;
                btnSave.Visible = false;
                txtId.ReadOnly = false; // Сделать поле ID доступным для ввода
                break;

                case FormAction.Edit:
                this.Text = "Изменить товар";
                lblName.Text = "Название:";
                txtId.Visible = true;
                break;
            }
        }

        private void DeleteGood()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Введите ID товара для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id;
            if (!int.TryParse(txtId.Text, out id)) // Проверка на правильность введенного ID
            {
                MessageBox.Show("Введите корректный ID товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "DELETE FROM goods WHERE id = @id";
            ExecuteNonQuery(query, new NpgsqlParameter("@id", id));
            MessageBox.Show("Товар успешно удалён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoodUpdated?.Invoke();
            this.Close();
        }


        private void LoadGoodData(int goodId)
        {
            string query = "SELECT id, name, priority FROM goods WHERE id = @id";
            ExecuteQuery(query, new NpgsqlParameter("@id", goodId), reader =>
            {
                txtId.Text = reader["id"].ToString();
                txtName.Text = reader["name"].ToString();
                txtPriority.Text = reader["priority"].ToString();
            });
        }

        private void ExecuteQuery(string query, NpgsqlParameter parameter, Action<NpgsqlDataReader> handleReader)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    if (parameter != null) command.Parameters.Add(parameter);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            handleReader(reader);
                        }
                        else
                        {
                            MessageBox.Show("Товар не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.Close();
                        }
                    }
                }
            }
        }

        private void SaveGood()
        {
            string query = "INSERT INTO goods (name, priority) VALUES (@name, @priority)";
            ExecuteNonQuery(query, new NpgsqlParameter("@name", txtName.Text), new NpgsqlParameter("@priority", Convert.ToDouble(txtPriority.Text)));
            MessageBox.Show("Товар добавлен успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoodUpdated?.Invoke();
            this.Close();
        }



        private void EditGood()
        {
            if (string.IsNullOrEmpty(txtId.Text) || string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPriority.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены для изменения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "UPDATE goods SET name = @name, priority = @priority WHERE id = @id";
            ExecuteNonQuery(query, new NpgsqlParameter("@id", Convert.ToInt32(txtId.Text)),
                                     new NpgsqlParameter("@name", txtName.Text),
                                     new NpgsqlParameter("@priority", Convert.ToDouble(txtPriority.Text)));

            MessageBox.Show("Товар успешно изменён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoodUpdated?.Invoke();
            this.Close();
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
                    command.ExecuteNonQuery();
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
                    SaveGood();
                    break;
                    case FormAction.Edit:
                    EditGood();
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении операции: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentAction == FormAction.Delete)
                    DeleteGood();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblId_Click(object sender, EventArgs e)
        {

        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
