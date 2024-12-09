using System.Windows.Forms.DataVisualization.Charting;

namespace DataBass
{
    partial class DemandGraphForm
    {
        private System.ComponentModel.IContainer components = null;
        private Chart chartSales;

        // Очистка ресурсов
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private ComboBox cmbGoods;

        private void InitializeComponent()
        {
            this.chartSales = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cmbGoods = new ComboBox();  // Инициализируем ComboBox

            // Настройка chartSales
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).BeginInit();
            this.SuspendLayout();

            // Настройка ComboBox
            this.cmbGoods.FormattingEnabled = true;
            this.cmbGoods.Location = new System.Drawing.Point(12, 460);  // Местоположение ComboBox
            this.cmbGoods.Name = "cmbGoods";
            this.cmbGoods.Size = new System.Drawing.Size(200, 21);  // Размер ComboBox
            this.cmbGoods.TabIndex = 1;
            this.cmbGoods.SelectedIndexChanged += new System.EventHandler(this.cmbGoods_SelectedIndexChanged);

            // Настройка chartSales
            this.chartSales.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSales.Location = new System.Drawing.Point(12, 12);
            this.chartSales.Name = "chartSales";
            this.chartSales.Size = new System.Drawing.Size(760, 437);  // Размеры графика
            this.chartSales.TabIndex = 0;
            this.chartSales.Text = "chartSales";

            // DemandGraphForm
            this.ClientSize = new System.Drawing.Size(784, 511);  // Увеличиваем размер для ComboBox
            this.Controls.Add(this.chartSales);
            this.Controls.Add(this.cmbGoods);  // Добавляем ComboBox на форму
            this.Name = "DemandGraphForm";
            this.Text = "График изменения спроса товара";
            this.Load += new System.EventHandler(this.DemandGraphForm_Load);

            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).EndInit();
            this.ResumeLayout(false);
        }

    }
}
