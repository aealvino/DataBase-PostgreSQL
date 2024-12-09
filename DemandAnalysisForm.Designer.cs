partial class DemandAnalysisForm
{
    private System.ComponentModel.IContainer components = null;
    private ComboBox cmbGoods;
    private DateTimePicker dtpStartDate;
    private DateTimePicker dtpEndDate;
    private Button btnAnalyze;
    private DataGridView dgvResult;


    private void InitializeComponent()
    {
        cmbGoods = new ComboBox();
        dtpStartDate = new DateTimePicker();
        dtpEndDate = new DateTimePicker();
        btnAnalyze = new Button();
        dgvResult = new DataGridView();
        ((System.ComponentModel.ISupportInitialize)dgvResult).BeginInit();
        SuspendLayout();
        // 
        // cmbGoods
        // 
        cmbGoods.Location = new Point(27, 31);
        cmbGoods.Margin = new Padding(4, 5, 4, 5);
        cmbGoods.Name = "cmbGoods";
        cmbGoods.Size = new Size(265, 28);
        cmbGoods.TabIndex = 0;
        // 
        // dtpStartDate
        // 
        dtpStartDate.Location = new Point(27, 87);
        dtpStartDate.Margin = new Padding(4, 5, 4, 5);
        dtpStartDate.Name = "dtpStartDate";
        dtpStartDate.Size = new Size(265, 27);
        dtpStartDate.TabIndex = 1;
        // 
        // dtpEndDate
        // 
        dtpEndDate.Location = new Point(27, 140);
        dtpEndDate.Margin = new Padding(4, 5, 4, 5);
        dtpEndDate.Name = "dtpEndDate";
        dtpEndDate.Size = new Size(265, 27);
        dtpEndDate.TabIndex = 2;
        // 
        // btnAnalyze
        // 
        btnAnalyze.Location = new Point(27, 215);
        btnAnalyze.Margin = new Padding(4, 5, 4, 5);
        btnAnalyze.Name = "btnAnalyze";
        btnAnalyze.Size = new Size(133, 35);
        btnAnalyze.TabIndex = 3;
        btnAnalyze.Text = "Анализировать";
        btnAnalyze.UseVisualStyleBackColor = true;
        btnAnalyze.Click += btnAnalyze_Click;
        // 
        // dgvResult
        // 
        dgvResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvResult.Location = new Point(27, 277);
        dgvResult.Margin = new Padding(4, 5, 4, 5);
        dgvResult.Name = "dgvResult";
        dgvResult.RowHeadersWidth = 51;
        dgvResult.Size = new Size(720, 277);
        dgvResult.TabIndex = 4;
        // 
        // DemandAnalysisForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 615);
        Controls.Add(cmbGoods);
        Controls.Add(dtpStartDate);
        Controls.Add(dtpEndDate);
        Controls.Add(btnAnalyze);
        Controls.Add(dgvResult);
        Margin = new Padding(4, 5, 4, 5);
        Name = "DemandAnalysisForm";
        Text = "Анализ спроса";
        ((System.ComponentModel.ISupportInitialize)dgvResult).EndInit();
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

}
