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
            btnSave.Location = new Point(120, 200);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(90, 30);
            btnSave.TabIndex = 8;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(230, 200);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(90, 30);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "Удалить";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // SalesForm
            // 
            ClientSize = new Size(400, 300);
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

        private System.Windows.Forms.TextBox txtSaleId;
        private System.Windows.Forms.Label lblSaleId;
        private System.Windows.Forms.ComboBox cmbGoods;
        private System.Windows.Forms.Label lblGood;
        private System.Windows.Forms.TextBox txtCount;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
    }
}
