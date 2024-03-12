namespace GTU_AVES
{
    partial class MainForm
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.map = new GMap.NET.WindowsForms.GMapControl();
            this.btnNewPolygon = new System.Windows.Forms.Button();
            this.cmbAreaSelect = new System.Windows.Forms.ComboBox();
            this.pnlAreaPlan = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClearPolygon = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.map);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.splitContainer1.Panel2.Controls.Add(this.btnNewPolygon);
            this.splitContainer1.Panel2.Controls.Add(this.cmbAreaSelect);
            this.splitContainer1.Panel2.Controls.Add(this.pnlAreaPlan);
            this.splitContainer1.Panel2.Controls.Add(this.btnClearPolygon);
            this.splitContainer1.Size = new System.Drawing.Size(1436, 726);
            this.splitContainer1.SplitterDistance = 882;
            this.splitContainer1.TabIndex = 0;
            // 
            // map
            // 
            this.map.Bearing = 0F;
            this.map.CanDragMap = true;
            this.map.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map.EmptyTileColor = System.Drawing.Color.Navy;
            this.map.GrayScaleMode = false;
            this.map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.map.LevelsKeepInMemory = 5;
            this.map.Location = new System.Drawing.Point(0, 0);
            this.map.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.map.MarkersEnabled = true;
            this.map.MaxZoom = 2;
            this.map.MinZoom = 2;
            this.map.MouseWheelZoomEnabled = true;
            this.map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.map.Name = "map";
            this.map.NegativeMode = false;
            this.map.PolygonsEnabled = true;
            this.map.RetryLoadTile = 0;
            this.map.RoutesEnabled = true;
            this.map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.map.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.map.ShowTileGridLines = false;
            this.map.Size = new System.Drawing.Size(882, 726);
            this.map.TabIndex = 0;
            this.map.Zoom = 0D;
            this.map.MouseClick += new System.Windows.Forms.MouseEventHandler(this.map_MouseClick);
            this.map.MouseDown += new System.Windows.Forms.MouseEventHandler(this.map_MouseDown);
            this.map.MouseMove += new System.Windows.Forms.MouseEventHandler(this.map_MouseMove);
            this.map.MouseUp += new System.Windows.Forms.MouseEventHandler(this.map_MouseUp);
            // 
            // btnNewPolygon
            // 
            this.btnNewPolygon.Location = new System.Drawing.Point(117, 665);
            this.btnNewPolygon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNewPolygon.Name = "btnNewPolygon";
            this.btnNewPolygon.Size = new System.Drawing.Size(115, 33);
            this.btnNewPolygon.TabIndex = 3;
            this.btnNewPolygon.Text = "Yeni Alan";
            this.btnNewPolygon.UseVisualStyleBackColor = true;
            this.btnNewPolygon.Click += new System.EventHandler(this.btnNewPolygon_Click);
            // 
            // cmbAreaSelect
            // 
            this.cmbAreaSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cmbAreaSelect.FormattingEnabled = true;
            this.cmbAreaSelect.Items.AddRange(new object[] {
            "1"});
            this.cmbAreaSelect.Location = new System.Drawing.Point(19, 401);
            this.cmbAreaSelect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbAreaSelect.Name = "cmbAreaSelect";
            this.cmbAreaSelect.Size = new System.Drawing.Size(53, 37);
            this.cmbAreaSelect.TabIndex = 2;
            this.cmbAreaSelect.Text = "1";
            this.cmbAreaSelect.SelectedIndexChanged += new System.EventHandler(this.cmbAreaSelect_SelectedIndexChanged);
            // 
            // pnlAreaPlan
            // 
            this.pnlAreaPlan.AutoScroll = true;
            this.pnlAreaPlan.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlAreaPlan.Location = new System.Drawing.Point(19, 444);
            this.pnlAreaPlan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlAreaPlan.Name = "pnlAreaPlan";
            this.pnlAreaPlan.Size = new System.Drawing.Size(514, 214);
            this.pnlAreaPlan.TabIndex = 1;
            // 
            // btnClearPolygon
            // 
            this.btnClearPolygon.Location = new System.Drawing.Point(20, 665);
            this.btnClearPolygon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClearPolygon.Name = "btnClearPolygon";
            this.btnClearPolygon.Size = new System.Drawing.Size(91, 33);
            this.btnClearPolygon.TabIndex = 0;
            this.btnClearPolygon.Text = "Alanı Sil";
            this.btnClearPolygon.UseVisualStyleBackColor = true;
            this.btnClearPolygon.Click += new System.EventHandler(this.btnClearPolygon_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1436, 726);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "GTU-AVES";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private GMap.NET.WindowsForms.GMapControl map;
        private System.Windows.Forms.Button btnClearPolygon;
        private System.Windows.Forms.FlowLayoutPanel pnlAreaPlan;
        private System.Windows.Forms.ComboBox cmbAreaSelect;
        private System.Windows.Forms.Button btnNewPolygon;
    }
}

