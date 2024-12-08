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
                // ������ ������������ �� �����
                Config config = new Config("config.ini");
                connectionString = $"Host={config.Host};Port={config.Port};Database={config.Database};Username={config.Username};Password={config.Password};";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� �������� ������������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // ��������� ����������
            }

            // �������� ����� ������
            try
            {
                LoginForm loginForm = new LoginForm(connectionString);
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    string userRole = loginForm.UserRole.ToLower(); // �������� ���� ������������
                    Application.Run(new Form1(userRole)); // �������� ���� � Form1
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ������� ����������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
