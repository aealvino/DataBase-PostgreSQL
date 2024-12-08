using DataBass4;
using System;
using System.Windows.Forms;

namespace DataBass
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string connectionString = null;

            try
            {
                // Чтение конфигурации из файла
                Config config = new Config("config.ini");
                connectionString = $"Host={config.Host};Port={config.Port};Database={config.Database};Username={config.Username};Password={config.Password};";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке конфигурации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Завершаем приложение
            }

            // Открытие формы логина
            try
            {
                LoginForm loginForm = new LoginForm(connectionString);
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    string userRole = loginForm.UserRole.ToLower(); // Получаем роль пользователя
                    Application.Run(new Form1(userRole)); // Передаем роль в Form1
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
