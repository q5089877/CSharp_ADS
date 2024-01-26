namespace DOSE_CAMERA
{
    partial class MainForm
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
            this.icImagingControl1 = new TIS.Imaging.ICImagingControl();
            this.icImagingControl2 = new TIS.Imaging.ICImagingControl();
            this.lbl_link_status = new System.Windows.Forms.Label();
            this.btn_dir = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbx_pictures = new System.Windows.Forms.TextBox();
            this.btn_capture_manual = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl2)).BeginInit();
            this.SuspendLayout();
            // 
            // icImagingControl1
            // 
            this.icImagingControl1.BackColor = System.Drawing.Color.Black;
            this.icImagingControl1.DeviceListChangedExecutionMode = TIS.Imaging.EventExecutionMode.Invoke;
            this.icImagingControl1.DeviceLostExecutionMode = TIS.Imaging.EventExecutionMode.AsyncInvoke;
            this.icImagingControl1.ImageAvailableExecutionMode = TIS.Imaging.EventExecutionMode.MultiThreaded;
            this.icImagingControl1.LiveDisplayPosition = new System.Drawing.Point(0, 0);
            this.icImagingControl1.Location = new System.Drawing.Point(12, 12);
            this.icImagingControl1.Name = "icImagingControl1";
            this.icImagingControl1.Size = new System.Drawing.Size(600, 400);
            this.icImagingControl1.TabIndex = 8;
            // 
            // icImagingControl2
            // 
            this.icImagingControl2.BackColor = System.Drawing.Color.Black;
            this.icImagingControl2.DeviceListChangedExecutionMode = TIS.Imaging.EventExecutionMode.Invoke;
            this.icImagingControl2.DeviceLostExecutionMode = TIS.Imaging.EventExecutionMode.AsyncInvoke;
            this.icImagingControl2.ImageAvailableExecutionMode = TIS.Imaging.EventExecutionMode.MultiThreaded;
            this.icImagingControl2.LiveDisplayPosition = new System.Drawing.Point(0, 0);
            this.icImagingControl2.Location = new System.Drawing.Point(12, 418);
            this.icImagingControl2.Name = "icImagingControl2";
            this.icImagingControl2.Size = new System.Drawing.Size(600, 400);
            this.icImagingControl2.TabIndex = 9;
            // 
            // lbl_link_status
            // 
            this.lbl_link_status.AutoSize = true;
            this.lbl_link_status.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbl_link_status.Location = new System.Drawing.Point(642, 26);
            this.lbl_link_status.Name = "lbl_link_status";
            this.lbl_link_status.Size = new System.Drawing.Size(68, 16);
            this.lbl_link_status.TabIndex = 18;
            this.lbl_link_status.Text = "連線中...";
            // 
            // btn_dir
            // 
            this.btn_dir.Location = new System.Drawing.Point(784, 841);
            this.btn_dir.Name = "btn_dir";
            this.btn_dir.Size = new System.Drawing.Size(75, 23);
            this.btn_dir.TabIndex = 21;
            this.btn_dir.Text = "資料夾選擇";
            this.btn_dir.UseVisualStyleBackColor = true;
            this.btn_dir.Click += new System.EventHandler(this.btn_dir_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(14, 843);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 16);
            this.label3.TabIndex = 20;
            this.label3.Text = "Capture Path:";
            // 
            // tbx_pictures
            // 
            this.tbx_pictures.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tbx_pictures.Location = new System.Drawing.Point(112, 840);
            this.tbx_pictures.Name = "tbx_pictures";
            this.tbx_pictures.Size = new System.Drawing.Size(666, 27);
            this.tbx_pictures.TabIndex = 19;
            this.tbx_pictures.Text = "C:\\DOSE\\Pictures";
            // 
            // btn_capture_manual
            // 
            this.btn_capture_manual.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_capture_manual.Location = new System.Drawing.Point(645, 48);
            this.btn_capture_manual.Name = "btn_capture_manual";
            this.btn_capture_manual.Size = new System.Drawing.Size(205, 120);
            this.btn_capture_manual.TabIndex = 22;
            this.btn_capture_manual.Text = "手動拍照";
            this.btn_capture_manual.UseVisualStyleBackColor = true;
            this.btn_capture_manual.Click += new System.EventHandler(this.btn_capture_manual_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 883);
            this.Controls.Add(this.btn_capture_manual);
            this.Controls.Add(this.btn_dir);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbx_pictures);
            this.Controls.Add(this.lbl_link_status);
            this.Controls.Add(this.icImagingControl2);
            this.Controls.Add(this.icImagingControl1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TIS.Imaging.ICImagingControl icImagingControl1;
        private TIS.Imaging.ICImagingControl icImagingControl2;
        private System.Windows.Forms.Label lbl_link_status;
        private System.Windows.Forms.Button btn_dir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbx_pictures;
        private System.Windows.Forms.Button btn_capture_manual;
    }
}