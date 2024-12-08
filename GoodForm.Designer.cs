namespace DataBass
{
    partial class GoodForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtPriority;
        private System.Windows.Forms.TextBox txtId; // Новое поле для отображения ID товара
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.Label lblId; // Новая метка для ID товара

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
            txtName = new TextBox();
            txtPriority = new TextBox();
            txtId = new TextBox();
            btnSave = new Button();
            btnDelete = new Button();
            lblName = new Label();
            lblPriority = new Label();
            lblId = new Label();
            SuspendLayout();
            // 
            // txtName
            // 
            txtName.Location = new Point(120, 60);
            txtName.Name = "txtName";
            txtName.Size = new Size(200, 27);
            txtName.TabIndex = 0;
            // 
            // txtPriority
            // 
            txtPriority.Location = new Point(120, 110);
            txtPriority.Name = "txtPriority";
            txtPriority.Size = new Size(200, 27);
            txtPriority.TabIndex = 1;
            // 
            // txtId
            // 
            txtId.Location = new Point(120, 30);
            txtId.Name = "txtId";
            txtId.ReadOnly = true;
            txtId.Size = new Size(200, 27);
            txtId.TabIndex = 2;
            txtId.TextChanged += txtId_TextChanged;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(120, 160);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(90, 30);
            btnSave.TabIndex = 3;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(230, 160);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(90, 30);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Удалить";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(30, 60);
            lblName.Name = "lblName";
            lblName.Size = new Size(80, 20);
            lblName.TabIndex = 5;
            lblName.Text = "Название:";
            // 
            // lblPriority
            // 
            lblPriority.AutoSize = true;
            lblPriority.Location = new Point(30, 110);
            lblPriority.Name = "lblPriority";
            lblPriority.Size = new Size(88, 20);
            lblPriority.TabIndex = 6;
            lblPriority.Text = "Приоритет:";
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new Point(30, 30);
            lblId.Name = "lblId";
            lblId.Size = new Size(79, 20);
            lblId.TabIndex = 7;
            lblId.Text = "ID товара:";
            lblId.Click += lblId_Click;
            // 
            // GoodForm
            // 
            ClientSize = new Size(360, 200);
            Controls.Add(lblId);
            Controls.Add(txtId);
            Controls.Add(lblPriority);
            Controls.Add(lblName);
            Controls.Add(btnDelete);
            Controls.Add(btnSave);
            Controls.Add(txtPriority);
            Controls.Add(txtName);
            Name = "GoodForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Редактирование товара";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
