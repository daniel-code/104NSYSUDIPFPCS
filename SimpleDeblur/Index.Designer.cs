namespace SimpleDeblur {
    partial class Index {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose( bool disposing ) {
            if( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Index));
            this.TSC = new System.Windows.Forms.ToolStripContainer();
            this.SS = new System.Windows.Forms.StatusStrip();
            this.SS_LBStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.SS_Spring = new System.Windows.Forms.ToolStripStatusLabel();
            this.SS_PB = new System.Windows.Forms.ToolStripProgressBar();
            this.TLP = new System.Windows.Forms.TableLayoutPanel();
            this.TLP_Setting = new System.Windows.Forms.TableLayoutPanel();
            this.PN_Lambda = new System.Windows.Forms.Panel();
            this.LB_LambdaTitle = new System.Windows.Forms.Label();
            this.LB_Lambda = new System.Windows.Forms.Label();
            this.TrB_Lambda = new System.Windows.Forms.TrackBar();
            this.PN_Length = new System.Windows.Forms.Panel();
            this.LB_LengthTitle = new System.Windows.Forms.Label();
            this.LB_Length = new System.Windows.Forms.Label();
            this.TrB_Length = new System.Windows.Forms.TrackBar();
            this.PB_Kernel = new System.Windows.Forms.PictureBox();
            this.FLP_Result = new System.Windows.Forms.FlowLayoutPanel();
            this.PB_Result = new System.Windows.Forms.PictureBox();
            this.MS = new System.Windows.Forms.MenuStrip();
            this.MS_File = new System.Windows.Forms.ToolStripMenuItem();
            this.MS_File_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.MS_File_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.SFD = new System.Windows.Forms.SaveFileDialog();
            this.TSC.BottomToolStripPanel.SuspendLayout();
            this.TSC.ContentPanel.SuspendLayout();
            this.TSC.TopToolStripPanel.SuspendLayout();
            this.TSC.SuspendLayout();
            this.SS.SuspendLayout();
            this.TLP.SuspendLayout();
            this.TLP_Setting.SuspendLayout();
            this.PN_Lambda.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrB_Lambda)).BeginInit();
            this.PN_Length.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrB_Length)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Kernel)).BeginInit();
            this.FLP_Result.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Result)).BeginInit();
            this.MS.SuspendLayout();
            this.SuspendLayout();
            // 
            // TSC
            // 
            // 
            // TSC.BottomToolStripPanel
            // 
            this.TSC.BottomToolStripPanel.Controls.Add(this.SS);
            // 
            // TSC.ContentPanel
            // 
            this.TSC.ContentPanel.Controls.Add(this.TLP);
            this.TSC.ContentPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TSC.ContentPanel.Size = new System.Drawing.Size(484, 415);
            this.TSC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TSC.Location = new System.Drawing.Point(0, 0);
            this.TSC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TSC.Name = "TSC";
            this.TSC.Size = new System.Drawing.Size(484, 461);
            this.TSC.TabIndex = 0;
            this.TSC.Text = "toolStripContainer1";
            // 
            // TSC.TopToolStripPanel
            // 
            this.TSC.TopToolStripPanel.Controls.Add(this.MS);
            // 
            // SS
            // 
            this.SS.BackColor = System.Drawing.Color.LightSkyBlue;
            this.SS.Dock = System.Windows.Forms.DockStyle.None;
            this.SS.GripMargin = new System.Windows.Forms.Padding(0);
            this.SS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SS_LBStatus,
            this.SS_Spring,
            this.SS_PB});
            this.SS.Location = new System.Drawing.Point(0, 0);
            this.SS.Name = "SS";
            this.SS.Size = new System.Drawing.Size(484, 22);
            this.SS.SizingGrip = false;
            this.SS.TabIndex = 0;
            // 
            // SS_LBStatus
            // 
            this.SS_LBStatus.Name = "SS_LBStatus";
            this.SS_LBStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // SS_Spring
            // 
            this.SS_Spring.Name = "SS_Spring";
            this.SS_Spring.Size = new System.Drawing.Size(167, 17);
            this.SS_Spring.Spring = true;
            // 
            // SS_PB
            // 
            this.SS_PB.Name = "SS_PB";
            this.SS_PB.Size = new System.Drawing.Size(300, 16);
            this.SS_PB.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.SS_PB.ToolTipText = "Test";
            // 
            // TLP
            // 
            this.TLP.ColumnCount = 1;
            this.TLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP.Controls.Add(this.TLP_Setting, 0, 0);
            this.TLP.Controls.Add(this.FLP_Result, 0, 1);
            this.TLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP.Location = new System.Drawing.Point(0, 0);
            this.TLP.Name = "TLP";
            this.TLP.RowCount = 2;
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP.Size = new System.Drawing.Size(484, 415);
            this.TLP.TabIndex = 0;
            // 
            // TLP_Setting
            // 
            this.TLP_Setting.BackColor = System.Drawing.Color.Lavender;
            this.TLP_Setting.ColumnCount = 2;
            this.TLP_Setting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Setting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.TLP_Setting.Controls.Add(this.PN_Lambda, 0, 1);
            this.TLP_Setting.Controls.Add(this.PN_Length, 0, 0);
            this.TLP_Setting.Controls.Add(this.PB_Kernel, 1, 0);
            this.TLP_Setting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Setting.Location = new System.Drawing.Point(0, 0);
            this.TLP_Setting.Margin = new System.Windows.Forms.Padding(0);
            this.TLP_Setting.Name = "TLP_Setting";
            this.TLP_Setting.RowCount = 2;
            this.TLP_Setting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TLP_Setting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TLP_Setting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TLP_Setting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TLP_Setting.Size = new System.Drawing.Size(484, 100);
            this.TLP_Setting.TabIndex = 0;
            // 
            // PN_Lambda
            // 
            this.PN_Lambda.Controls.Add(this.LB_LambdaTitle);
            this.PN_Lambda.Controls.Add(this.LB_Lambda);
            this.PN_Lambda.Controls.Add(this.TrB_Lambda);
            this.PN_Lambda.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_Lambda.Location = new System.Drawing.Point(3, 53);
            this.PN_Lambda.Name = "PN_Lambda";
            this.PN_Lambda.Size = new System.Drawing.Size(378, 44);
            this.PN_Lambda.TabIndex = 1;
            // 
            // LB_LambdaTitle
            // 
            this.LB_LambdaTitle.AutoSize = true;
            this.LB_LambdaTitle.Location = new System.Drawing.Point(3, 3);
            this.LB_LambdaTitle.Name = "LB_LambdaTitle";
            this.LB_LambdaTitle.Size = new System.Drawing.Size(55, 16);
            this.LB_LambdaTitle.TabIndex = 4;
            this.LB_LambdaTitle.Text = "Lambda";
            // 
            // LB_Lambda
            // 
            this.LB_Lambda.AutoSize = true;
            this.LB_Lambda.Location = new System.Drawing.Point(3, 22);
            this.LB_Lambda.Name = "LB_Lambda";
            this.LB_Lambda.Size = new System.Drawing.Size(22, 16);
            this.LB_Lambda.TabIndex = 3;
            this.LB_Lambda.Text = "10";
            // 
            // TrB_Lambda
            // 
            this.TrB_Lambda.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrB_Lambda.Location = new System.Drawing.Point(62, 9);
            this.TrB_Lambda.Maximum = 100;
            this.TrB_Lambda.Minimum = 1;
            this.TrB_Lambda.Name = "TrB_Lambda";
            this.TrB_Lambda.Size = new System.Drawing.Size(309, 45);
            this.TrB_Lambda.TabIndex = 1;
            this.TrB_Lambda.Value = 10;
            this.TrB_Lambda.Scroll += new System.EventHandler(this.TrB_Lambda_Scroll);
            // 
            // PN_Length
            // 
            this.PN_Length.Controls.Add(this.LB_LengthTitle);
            this.PN_Length.Controls.Add(this.LB_Length);
            this.PN_Length.Controls.Add(this.TrB_Length);
            this.PN_Length.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_Length.Location = new System.Drawing.Point(3, 3);
            this.PN_Length.Name = "PN_Length";
            this.PN_Length.Size = new System.Drawing.Size(378, 44);
            this.PN_Length.TabIndex = 0;
            // 
            // LB_LengthTitle
            // 
            this.LB_LengthTitle.AutoSize = true;
            this.LB_LengthTitle.Location = new System.Drawing.Point(3, 3);
            this.LB_LengthTitle.Name = "LB_LengthTitle";
            this.LB_LengthTitle.Size = new System.Drawing.Size(32, 16);
            this.LB_LengthTitle.TabIndex = 2;
            this.LB_LengthTitle.Text = "長度";
            // 
            // LB_Length
            // 
            this.LB_Length.AutoSize = true;
            this.LB_Length.Location = new System.Drawing.Point(3, 22);
            this.LB_Length.Name = "LB_Length";
            this.LB_Length.Size = new System.Drawing.Size(22, 16);
            this.LB_Length.TabIndex = 1;
            this.LB_Length.Text = "53";
            // 
            // TrB_Length
            // 
            this.TrB_Length.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrB_Length.LargeChange = 4;
            this.TrB_Length.Location = new System.Drawing.Point(62, 9);
            this.TrB_Length.Maximum = 101;
            this.TrB_Length.Minimum = 1;
            this.TrB_Length.Name = "TrB_Length";
            this.TrB_Length.Size = new System.Drawing.Size(309, 45);
            this.TrB_Length.SmallChange = 2;
            this.TrB_Length.TabIndex = 0;
            this.TrB_Length.Value = 53;
            this.TrB_Length.Scroll += new System.EventHandler(this.TrB_Length_Scroll);
            // 
            // PB_Kernel
            // 
            this.PB_Kernel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PB_Kernel.Location = new System.Drawing.Point(384, 0);
            this.PB_Kernel.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Kernel.Name = "PB_Kernel";
            this.TLP_Setting.SetRowSpan(this.PB_Kernel, 2);
            this.PB_Kernel.Size = new System.Drawing.Size(100, 100);
            this.PB_Kernel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PB_Kernel.TabIndex = 2;
            this.PB_Kernel.TabStop = false;
            // 
            // FLP_Result
            // 
            this.FLP_Result.AutoScroll = true;
            this.FLP_Result.BackColor = System.Drawing.Color.White;
            this.FLP_Result.Controls.Add(this.PB_Result);
            this.FLP_Result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Result.Location = new System.Drawing.Point(0, 100);
            this.FLP_Result.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Result.Name = "FLP_Result";
            this.FLP_Result.Size = new System.Drawing.Size(484, 315);
            this.FLP_Result.TabIndex = 1;
            // 
            // PB_Result
            // 
            this.PB_Result.Location = new System.Drawing.Point(0, 0);
            this.PB_Result.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Result.Name = "PB_Result";
            this.PB_Result.Size = new System.Drawing.Size(594, 269);
            this.PB_Result.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PB_Result.TabIndex = 0;
            this.PB_Result.TabStop = false;
            // 
            // MS
            // 
            this.MS.BackColor = System.Drawing.Color.LightSkyBlue;
            this.MS.Dock = System.Windows.Forms.DockStyle.None;
            this.MS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MS_File});
            this.MS.Location = new System.Drawing.Point(0, 0);
            this.MS.Name = "MS";
            this.MS.Size = new System.Drawing.Size(484, 24);
            this.MS.TabIndex = 0;
            this.MS.Text = "menuStrip1";
            // 
            // MS_File
            // 
            this.MS_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MS_File_Open,
            this.MS_File_Save});
            this.MS_File.Name = "MS_File";
            this.MS_File.Size = new System.Drawing.Size(60, 20);
            this.MS_File.Text = "檔案 (&F)";
            // 
            // MS_File_Open
            // 
            this.MS_File_Open.BackColor = System.Drawing.Color.LightSkyBlue;
            this.MS_File_Open.Name = "MS_File_Open";
            this.MS_File_Open.Padding = new System.Windows.Forms.Padding(0);
            this.MS_File_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MS_File_Open.Size = new System.Drawing.Size(167, 20);
            this.MS_File_Open.Text = "開啟圖片";
            this.MS_File_Open.Click += new System.EventHandler(this.MS_File_Open_Click);
            // 
            // MS_File_Save
            // 
            this.MS_File_Save.BackColor = System.Drawing.Color.LightSkyBlue;
            this.MS_File_Save.Name = "MS_File_Save";
            this.MS_File_Save.Padding = new System.Windows.Forms.Padding(0);
            this.MS_File_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.MS_File_Save.Size = new System.Drawing.Size(167, 20);
            this.MS_File_Save.Text = "匯出圖片";
            this.MS_File_Save.Click += new System.EventHandler(this.MS_File_Save_Click);
            // 
            // OFD
            // 
            this.OFD.Filter = "Tiff|*.tif";
            this.OFD.Title = "開啟圖片";
            // 
            // SFD
            // 
            this.SFD.Filter = "Tiff|*.tif";
            // 
            // Index
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(this.TSC);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.MS;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "Index";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "簡單的 Motion Deblur";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Index_FormClosing);
            this.Load += new System.EventHandler(this.Index_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Index_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Index_KeyUp);
            this.Resize += new System.EventHandler(this.Index_Resize);
            this.TSC.BottomToolStripPanel.ResumeLayout(false);
            this.TSC.BottomToolStripPanel.PerformLayout();
            this.TSC.ContentPanel.ResumeLayout(false);
            this.TSC.TopToolStripPanel.ResumeLayout(false);
            this.TSC.TopToolStripPanel.PerformLayout();
            this.TSC.ResumeLayout(false);
            this.TSC.PerformLayout();
            this.SS.ResumeLayout(false);
            this.SS.PerformLayout();
            this.TLP.ResumeLayout(false);
            this.TLP_Setting.ResumeLayout(false);
            this.PN_Lambda.ResumeLayout(false);
            this.PN_Lambda.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrB_Lambda)).EndInit();
            this.PN_Length.ResumeLayout(false);
            this.PN_Length.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrB_Length)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Kernel)).EndInit();
            this.FLP_Result.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Result)).EndInit();
            this.MS.ResumeLayout(false);
            this.MS.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer TSC;
        private System.Windows.Forms.StatusStrip SS;
        private System.Windows.Forms.MenuStrip MS;
        private System.Windows.Forms.ToolStripMenuItem MS_File;
        private System.Windows.Forms.ToolStripMenuItem MS_File_Open;
        private System.Windows.Forms.ToolStripMenuItem MS_File_Save;
        private System.Windows.Forms.ToolStripStatusLabel SS_LBStatus;
        private System.Windows.Forms.TableLayoutPanel TLP;
        private System.Windows.Forms.TableLayoutPanel TLP_Setting;
        private System.Windows.Forms.Panel PN_Lambda;
        private System.Windows.Forms.Panel PN_Length;
        private System.Windows.Forms.TrackBar TrB_Length;
        private System.Windows.Forms.TrackBar TrB_Lambda;
        private System.Windows.Forms.PictureBox PB_Kernel;
        private System.Windows.Forms.Label LB_LengthTitle;
        private System.Windows.Forms.Label LB_Length;
        private System.Windows.Forms.Label LB_LambdaTitle;
        private System.Windows.Forms.Label LB_Lambda;
        private System.Windows.Forms.FlowLayoutPanel FLP_Result;
        private System.Windows.Forms.PictureBox PB_Result;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.SaveFileDialog SFD;
        private System.Windows.Forms.ToolStripStatusLabel SS_Spring;
        private System.Windows.Forms.ToolStripProgressBar SS_PB;
    }
}

