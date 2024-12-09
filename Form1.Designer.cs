namespace DataBass
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabManagerLog;
        private System.Windows.Forms.TabPage tabGoods;
        private System.Windows.Forms.TabPage tabReport; // Новая вкладка
        private System.Windows.Forms.TabPage tabSales;
        private System.Windows.Forms.DataGridView dgvManagerLog;
        private System.Windows.Forms.DataGridView dgvReport;
        Button btnTopProducts;
        private Button btnAnalyzeDemand;
        private Button btnShowGraph;  // Добавьте это поле

        private Label lblUserRole;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tabControl = new TabControl();
            tabManagerLog = new TabPage();
            dgvManagerLog = new DataGridView();
            tabGoods = new TabPage();
            dataGridView1 = new DataGridView();
            tabSales = new TabPage();
            dataGridView2 = new DataGridView();
            lblUserRole = new Label();
            tabReport = new TabPage(); // Инициализация новой вкладки
            dgvReport = new DataGridView();

            tabControl.SuspendLayout();
            tabManagerLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvManagerLog).BeginInit();
            tabGoods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tabSales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            tabReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReport).BeginInit();
            SuspendLayout();

            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabManagerLog);
            tabControl.Controls.Add(tabGoods);
            tabControl.Controls.Add(tabSales);
            tabControl.Controls.Add(tabReport);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(800, 500);
            tabControl.TabIndex = 0;

            // 
            // tabManagerLog
            // 
            tabManagerLog.Controls.Add(lblUserRole);
            tabManagerLog.Controls.Add(dgvManagerLog);
            AddCommonButtons(tabManagerLog); // Добавляем кнопки
            tabManagerLog.Location = new Point(4, 29);
            tabManagerLog.Name = "tabManagerLog";
            tabManagerLog.Padding = new Padding(3);
            tabManagerLog.Size = new Size(792, 467);
            tabManagerLog.TabIndex = 0;
            tabManagerLog.Text = "Журнал Менеджера";
            tabManagerLog.UseVisualStyleBackColor = true;


            tabReport.Controls.Add(dgvReport);
            tabReport.Location = new Point(4, 29);
            tabReport.Name = "tabReport";
            tabReport.Padding = new Padding(3);
            tabReport.Size = new Size(792, 467);
            tabReport.TabIndex = 3;
            tabReport.Text = "Отчеты";
            tabReport.UseVisualStyleBackColor = true;
            // 
            // dgvManagerLog
            // 
            dgvManagerLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvManagerLog.Location = new Point(8, 6);
            dgvManagerLog.Name = "dgvManagerLog";
            dgvManagerLog.RowHeadersWidth = 51;
            dgvManagerLog.Size = new Size(366, 433);
            dgvManagerLog.TabIndex = 0;

            // 
            // tabGoods
            // 
            tabGoods.Controls.Add(dataGridView1);
            AddCommonButtons(tabGoods); // Добавляем кнопки
            tabGoods.Location = new Point(4, 29);
            tabGoods.Name = "tabGoods";
            tabGoods.Padding = new Padding(3);
            tabGoods.Size = new Size(792, 467);
            tabGoods.TabIndex = 1;
            tabGoods.Text = "Товары";
            tabGoods.UseVisualStyleBackColor = true;

            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(8, 6);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(366, 433);
            dataGridView1.TabIndex = 1;

            // 
            // tabRequests
            // 
            tabSales.Controls.Add(dataGridView2);
            AddCommonButtons(tabSales); // Добавляем кнопки
            tabSales.Location = new Point(4, 29);
            tabSales.Name = "tabRequests";
            tabSales.Padding = new Padding(3);
            tabSales.Size = new Size(792, 467);
            tabSales.TabIndex = 2;
            tabSales.Text = "Заявки";
            tabSales.UseVisualStyleBackColor = true;

            // 
            // dataGridView2
            // 
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(8, 6);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.RowHeadersWidth = 51;
            dataGridView2.Size = new Size(366, 433);
            dataGridView2.TabIndex = 1;

            lblUserRole.Location = new Point(670, 10);
            Controls.Add(lblUserRole);

            // Уберите добавление lblUserRole из tabManagerLog
            tabManagerLog.Controls.Remove(lblUserRole);

            // Обновите lblUserRole
            lblUserRole.AutoSize = true;
            lblUserRole.Name = "lblUserRole";
            lblUserRole.Size = new Size(107, 20);
            lblUserRole.TabIndex = 0;
            lblUserRole.Text = "Role: Unknown";
            lblUserRole.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            // 
            // Form1
            // 

            dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReport.Location = new Point(8, 6);
            dgvReport.Name = "dgvReport";
            dgvReport.RowHeadersWidth = 51;
            dgvReport.Size = new Size(366, 433);
            dgvReport.TabIndex = 1;


            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 500);
            Controls.Add(tabControl);
            MinimumSize = new Size(600, 400);
            Name = "Form1";
            Text = "Database Viewer";
            tabControl.ResumeLayout(false);
            tabManagerLog.ResumeLayout(false);
            tabManagerLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvManagerLog).EndInit();
            tabGoods.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tabSales.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ResumeLayout(false);

            btnTopProducts = new Button
            {
                Text = "Пять самых популярных товаров",
                Size = new Size(300, 40),
                Location = new Point(400, 50),
                Name = "btnTopProducts"
            };
            btnTopProducts.Click += BtnTopProducts_Click;
            tabReport.Controls.Add(btnTopProducts);

            btnAnalyzeDemand = new Button
            {
                Text = "Анализ спроса",
                Size = new Size(300, 40),
                Location = new Point(400, 100), // Позиция кнопки
                Name = "btnAnalyzeDemand"
            };
            btnAnalyzeDemand.Click += BtnAnalyzeDemand_Click;
            tabReport.Controls.Add(btnAnalyzeDemand);

            btnShowGraph = new Button
            {
                Text = "Показать график изменения спроса",
                Size = new Size(300, 40),
                Location = new Point(400, 150), // Позиция кнопки
                Name = "btnShowGraph"
            };
            btnShowGraph.Click += BtnShowGraph_Click;
            tabReport.Controls.Add(btnShowGraph);

        }
        private void AddCommonButtons(TabPage tabPage)
        {
            // Очистка старых кнопок перед добавлением новых
            var existingButtons = tabPage.Controls.OfType<Button>().ToList();
            foreach (var button in existingButtons)
            {
                tabPage.Controls.Remove(button);
            }

            if (UserRole != "admin")
            {
                Console.WriteLine("Кнопки недоступны для текущего пользователя.");
                return; // Кнопки не добавляются для пользователей, не являющихся администраторами
            }

            string[] buttonNames = { "Добавить", "Удалить", "Изменить" };
            for (int i = 0; i < buttonNames.Length; i++)
            {
                Button button = new Button
                {
                    Text = buttonNames[i],
                    Size = new Size(200, 40),
                    Location = new Point(400, 50 + i * 60),
                    Name = $"btn{buttonNames[i]}" // Уникальное имя для каждой кнопки
                };

                // Привязываем обработчики событий к кнопкам
                if (buttonNames[i] == "Добавить")
                {
                    button.Click += AddButton_Click;
                }
                else if (buttonNames[i] == "Удалить")
                {
                    button.Click += DeleteButton_Click;
                }
                else if (buttonNames[i] == "Изменить")
                {
                    button.Click += EditButton_Click;
                }

                tabPage.Controls.Add(button); // Добавляем кнопку на текущую вкладку
            }
        }




        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Label Role;
    }
}
