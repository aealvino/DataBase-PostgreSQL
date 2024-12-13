namespace DataBass
{
    partial class SalesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtSaleId = new TextBox();
            lblSaleId = new Label();
            cmbGoods = new ComboBox();
            lblGood = new Label();
            txtCount = new TextBox();
            lblCount = new Label();
            dtpDate = new DateTimePicker();
            lblDate = new Label();
            btnSave = new Button();
            btnDelete = new Button();
            cmbWarehouse = new ComboBox();
            lblWarehouse = new Label();
            SuspendLayout();

            // 
            // txtSaleId
            // 
            txtSaleId.Location = new Point(120, 30);
            txtSaleId.Name = "txtSaleId";
            txtSaleId.ReadOnly = true;
            txtSaleId.Size = new Size(200, 27);
            txtSaleId.TabIndex = 0;

            // 
            // lblSaleId
            // 
            lblSaleId.AutoSize = true;
            lblSaleId.Location = new Point(30, 30);
            lblSaleId.Name = "lblSaleId";
            lblSaleId.Size = new Size(94, 20);
            lblSaleId.TabIndex = 1;
            lblSaleId.Text = "ID продажи:";

            // 
            // cmbGoods
            // 
            cmbGoods.FormattingEnabled = true;
            cmbGoods.Location = new Point(120, 70);
            cmbGoods.Name = "cmbGoods";
            cmbGoods.Size = new Size(200, 28);
            cmbGoods.TabIndex = 2;

            // 
            // lblGood
            // 
            lblGood.AutoSize = true;
            lblGood.Location = new Point(30, 70);
            lblGood.Name = "lblGood";
            lblGood.Size = new Size(54, 20);
            lblGood.TabIndex = 3;
            lblGood.Text = "Товар:";

            // 
            // txtCount
            // 
            txtCount.Location = new Point(120, 110);
            txtCount.Name = "txtCount";
            txtCount.Size = new Size(200, 27);
            txtCount.TabIndex = 4;

            // 
            // lblCount
            // 
            lblCount.AutoSize = true;
            lblCount.Location = new Point(30, 110);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(93, 20);
            lblCount.TabIndex = 5;
            lblCount.Text = "Количество:";

            // 
            // dtpDate
            // 
            dtpDate.Location = new Point(120, 150);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(200, 27);
            dtpDate.TabIndex = 6;

            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Location = new Point(3, 155);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(111, 20);
            lblDate.TabIndex = 7;
            lblDate.Text = "Дата продажи:";

            // 
            // btnSave
            // 
            btnSave.Location = new Point(120, 250);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(90, 30);
            btnSave.TabIndex = 8;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;

            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(230, 250);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(90, 30);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "Удалить";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;

            // 
            // cmbWarehouse
            // 
            cmbWarehouse.FormattingEnabled = true;
            cmbWarehouse.Items.AddRange(new object[] { "Склад 1", "Склад 2", "Оба склада" });
            cmbWarehouse.Location = new Point(120, 190);
            cmbWarehouse.Name = "cmbWarehouse";
            cmbWarehouse.Size = new Size(200, 28);
            cmbWarehouse.TabIndex = 10;

            // 
            // lblWarehouse
            // 
            lblWarehouse.AutoSize = true;
            lblWarehouse.Location = new Point(30, 190);
            lblWarehouse.Name = "lblWarehouse";
            lblWarehouse.Size = new Size(82, 20);
            lblWarehouse.TabIndex = 11;
            lblWarehouse.Text = "Выбор склада:";

            // 
            // SalesForm
            // 
            ClientSize = new Size(400, 300);
            Controls.Add(lblWarehouse);
            Controls.Add(cmbWarehouse);
            Controls.Add(btnDelete);
            Controls.Add(btnSave);
            Controls.Add(lblDate);
            Controls.Add(dtpDate);
            Controls.Add(lblCount);
            Controls.Add(txtCount);
            Controls.Add(lblGood);
            Controls.Add(cmbGoods);
            Controls.Add(lblSaleId);
            Controls.Add(txtSaleId);
            Name = "SalesForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Управление продажами";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbWarehouse;
        private Label lblWarehouse;
        private TextBox txtSaleId;
        private Label lblSaleId;
        private ComboBox cmbGoods;
        private Label lblGood;
        private TextBox txtCount;
        private Label lblCount;
        private DateTimePicker dtpDate;
        private Label lblDate;
        private Button btnSave;
        private Button btnDelete;
    }
}
