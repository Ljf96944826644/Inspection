namespace HaspDemo.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            grpDevices = new GroupBox();
            grpCamera = new GroupBox();
            btnConnectCamera = new Button();
            btnDisconnectCamera = new Button();
            lblCameraStatus = new Label();
            grpPlc = new GroupBox();
            btnConnectPLC = new Button();
            btnDisconnectPLC = new Button();
            btnStart = new Button();
            btnStop = new Button();
            lblPLCStatus = new Label();
            picPreview = new PictureBox();
            grpResult = new GroupBox();
            lbResult = new Label();
            lbTime = new Label();
            txtLog = new TextBox();
            grpDetail = new GroupBox();
            dgvRecord = new DataGridView();
            colItem = new DataGridViewTextBoxColumn();
            colValue = new DataGridViewTextBoxColumn();
            colJudge = new DataGridViewTextBoxColumn();
            grpDevices.SuspendLayout();
            grpCamera.SuspendLayout();
            grpPlc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            grpResult.SuspendLayout();
            grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRecord).BeginInit();
            SuspendLayout();
            // 
            // grpDevices
            // 
            grpDevices.Controls.Add(grpCamera);
            grpDevices.Controls.Add(grpPlc);
            grpDevices.Location = new Point(12, 12);
            grpDevices.Name = "grpDevices";
            grpDevices.Size = new Size(200, 360);
            grpDevices.TabIndex = 0;
            grpDevices.TabStop = false;
            grpDevices.Text = "设备控制";
            // 
            // grpCamera
            // 
            grpCamera.Controls.Add(btnConnectCamera);
            grpCamera.Controls.Add(btnDisconnectCamera);
            grpCamera.Controls.Add(lblCameraStatus);
            grpCamera.Location = new Point(10, 25);
            grpCamera.Name = "grpCamera";
            grpCamera.Size = new Size(180, 110);
            grpCamera.TabIndex = 0;
            grpCamera.TabStop = false;
            grpCamera.Text = "相机";
            // 
            // btnConnectCamera
            // 
            btnConnectCamera.Location = new Point(15, 25);
            btnConnectCamera.Name = "btnConnectCamera";
            btnConnectCamera.Size = new Size(75, 30);
            btnConnectCamera.TabIndex = 0;
            btnConnectCamera.Text = "连接相机";
            btnConnectCamera.UseVisualStyleBackColor = true;
            btnConnectCamera.Click += btnConnectCamera_Click;
            // 
            // btnDisconnectCamera
            // 
            btnDisconnectCamera.Enabled = false;
            btnDisconnectCamera.Location = new Point(95, 25);
            btnDisconnectCamera.Name = "btnDisconnectCamera";
            btnDisconnectCamera.Size = new Size(75, 30);
            btnDisconnectCamera.TabIndex = 1;
            btnDisconnectCamera.Text = "断开相机";
            btnDisconnectCamera.UseVisualStyleBackColor = true;
            btnDisconnectCamera.Click += btnDisconnectCamera_Click;
            // 
            // lblCameraStatus
            // 
            lblCameraStatus.ForeColor = Color.Red;
            lblCameraStatus.Location = new Point(15, 70);
            lblCameraStatus.Name = "lblCameraStatus";
            lblCameraStatus.Size = new Size(155, 23);
            lblCameraStatus.TabIndex = 2;
            lblCameraStatus.Text = "相机：未连接";
            lblCameraStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpPlc
            // 
            grpPlc.Controls.Add(btnConnectPLC);
            grpPlc.Controls.Add(btnDisconnectPLC);
            grpPlc.Controls.Add(btnStart);
            grpPlc.Controls.Add(btnStop);
            grpPlc.Controls.Add(lblPLCStatus);
            grpPlc.Location = new Point(10, 150);
            grpPlc.Name = "grpPlc";
            grpPlc.Size = new Size(180, 195);
            grpPlc.TabIndex = 1;
            grpPlc.TabStop = false;
            grpPlc.Text = "PLC";
            // 
            // btnConnectPLC
            // 
            btnConnectPLC.Location = new Point(15, 25);
            btnConnectPLC.Name = "btnConnectPLC";
            btnConnectPLC.Size = new Size(75, 30);
            btnConnectPLC.TabIndex = 0;
            btnConnectPLC.Text = "连接PLC";
            btnConnectPLC.UseVisualStyleBackColor = true;
            btnConnectPLC.Click += btnConnectPLC_Click;
            // 
            // btnDisconnectPLC
            // 
            btnDisconnectPLC.Enabled = false;
            btnDisconnectPLC.Location = new Point(95, 25);
            btnDisconnectPLC.Name = "btnDisconnectPLC";
            btnDisconnectPLC.Size = new Size(75, 30);
            btnDisconnectPLC.TabIndex = 1;
            btnDisconnectPLC.Text = "断开PLC";
            btnDisconnectPLC.UseVisualStyleBackColor = true;
            btnDisconnectPLC.Click += btnDisconnectPLC_Click;
            // 
            // btnStart
            // 
            btnStart.Enabled = false;
            btnStart.Location = new Point(15, 65);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 30);
            btnStart.TabIndex = 2;
            btnStart.Text = "启动";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(95, 65);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 30);
            btnStop.TabIndex = 3;
            btnStop.Text = "停止";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lblPLCStatus
            // 
            lblPLCStatus.ForeColor = Color.Red;
            lblPLCStatus.Location = new Point(15, 110);
            lblPLCStatus.Name = "lblPLCStatus";
            lblPLCStatus.Size = new Size(155, 23);
            lblPLCStatus.TabIndex = 4;
            lblPLCStatus.Text = "PLC：未连接";
            lblPLCStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // picPreview
            // 
            picPreview.BackColor = Color.FromArgb(30, 30, 30);
            picPreview.BorderStyle = BorderStyle.FixedSingle;
            picPreview.Location = new Point(224, 12);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(640, 480);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 1;
            picPreview.TabStop = false;
            // 
            // grpResult
            // 
            grpResult.Controls.Add(lbResult);
            grpResult.Controls.Add(lbTime);
            grpResult.Location = new Point(224, 498);
            grpResult.Name = "grpResult";
            grpResult.Size = new Size(640, 80);
            grpResult.TabIndex = 2;
            grpResult.TabStop = false;
            grpResult.Text = "检测结果";
            // 
            // lbResult
            // 
            lbResult.Font = new Font("微软雅黑", 22F, FontStyle.Bold);
            lbResult.ForeColor = Color.Gray;
            lbResult.Location = new Point(20, 22);
            lbResult.Name = "lbResult";
            lbResult.Size = new Size(200, 48);
            lbResult.TabIndex = 0;
            lbResult.Text = "--";
            lbResult.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbTime
            // 
            lbTime.Font = new Font("微软雅黑", 10F);
            lbTime.Location = new Point(240, 35);
            lbTime.Name = "lbTime";
            lbTime.Size = new Size(380, 30);
            lbTime.TabIndex = 1;
            lbTime.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.FromArgb(20, 20, 20);
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Location = new Point(876, 12);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(400, 480);
            txtLog.TabIndex = 3;
            // 
            // grpDetail
            // 
            grpDetail.Controls.Add(dgvRecord);
            grpDetail.Location = new Point(12, 584);
            grpDetail.Name = "grpDetail";
            grpDetail.Size = new Size(852, 200);
            grpDetail.TabIndex = 4;
            grpDetail.TabStop = false;
            grpDetail.Text = "检测明细";
            // 
            // dgvRecord
            // 
            dgvRecord.AllowUserToAddRows = false;
            dgvRecord.AllowUserToDeleteRows = false;
            dgvRecord.AllowUserToResizeRows = false;
            dgvRecord.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRecord.Columns.AddRange(new DataGridViewColumn[] { colItem, colValue, colJudge });
            dgvRecord.Location = new Point(10, 22);
            dgvRecord.MultiSelect = false;
            dgvRecord.Name = "dgvRecord";
            dgvRecord.ReadOnly = true;
            dgvRecord.RowHeadersVisible = false;
            dgvRecord.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecord.Size = new Size(721, 168);
            dgvRecord.TabIndex = 0;
            // 
            // colItem
            // 
            colItem.HeaderText = "项目";
            colItem.Name = "colItem";
            colItem.ReadOnly = true;
            colItem.Width = 150;
            // 
            // colValue
            // 
            colValue.HeaderText = "测量值";
            colValue.Name = "colValue";
            colValue.ReadOnly = true;
            colValue.Width = 450;
            // 
            // colJudge
            // 
            colJudge.HeaderText = "判定";
            colJudge.Name = "colJudge";
            colJudge.ReadOnly = true;
            colJudge.Width = 120;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1288, 801);
            Controls.Add(grpDevices);
            Controls.Add(picPreview);
            Controls.Add(grpResult);
            Controls.Add(txtLog);
            Controls.Add(grpDetail);
            MinimumSize = new Size(1300, 840);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "垫圈视觉检测系统";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            grpDevices.ResumeLayout(false);
            grpCamera.ResumeLayout(false);
            grpPlc.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            grpResult.ResumeLayout(false);
            grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRecord).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpDevices;
        private System.Windows.Forms.GroupBox grpCamera;
        private System.Windows.Forms.Button btnConnectCamera;
        private System.Windows.Forms.Button btnDisconnectCamera;
        private System.Windows.Forms.Label lblCameraStatus;
        private System.Windows.Forms.GroupBox grpPlc;
        private System.Windows.Forms.Button btnConnectPLC;
        private System.Windows.Forms.Button btnDisconnectPLC;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblPLCStatus;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.GroupBox grpResult;
        private System.Windows.Forms.Label lbResult;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox grpDetail;
        private System.Windows.Forms.DataGridView dgvRecord;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colJudge;
    }
}
