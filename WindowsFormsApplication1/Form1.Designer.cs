//namespace DOSE_CAMERA
//{
//    partial class Form1
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.components = new System.ComponentModel.Container();
//            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
//            this.icImagingControl1 = new TIS.Imaging.ICImagingControl();
//            this.btn_capture = new System.Windows.Forms.Button();
//            this.t_try_link = new System.Windows.Forms.Timer(this.components);
//            this.lbl_conn_status = new System.Windows.Forms.Label();
//            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
//            this.icImagingControl2 = new TIS.Imaging.ICImagingControl();
//            this.tbx_pictures = new System.Windows.Forms.TextBox();
//            this.label3 = new System.Windows.Forms.Label();
//            this.btn_dir = new System.Windows.Forms.Button();
//            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl1)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl2)).BeginInit();
//            this.SuspendLayout();
//            // 
//            // icImagingControl1
//            // 
//            this.icImagingControl1.BackColor = System.Drawing.Color.Black;
//            this.icImagingControl1.DeviceListChangedExecutionMode = TIS.Imaging.EventExecutionMode.Invoke;
//            this.icImagingControl1.DeviceLostExecutionMode = TIS.Imaging.EventExecutionMode.AsyncInvoke;
//            this.icImagingControl1.ImageAvailableExecutionMode = TIS.Imaging.EventExecutionMode.MultiThreaded;
//            this.icImagingControl1.LiveDisplayPosition = new System.Drawing.Point(0, 0);
//            this.icImagingControl1.Location = new System.Drawing.Point(12, 56);
//            this.icImagingControl1.Name = "icImagingControl1";
//            this.icImagingControl1.Size = new System.Drawing.Size(462, 316);
//            this.icImagingControl1.TabIndex = 7;
//            // 
//            // btn_capture
//            // 
//            this.btn_capture.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
//            this.btn_capture.Location = new System.Drawing.Point(941, 56);
//            this.btn_capture.Name = "btn_capture";
//            this.btn_capture.Size = new System.Drawing.Size(120, 55);
//            this.btn_capture.TabIndex = 10;
//            this.btn_capture.Text = "手動拍照";
//            this.btn_capture.UseVisualStyleBackColor = true;
//            this.btn_capture.Click += new System.EventHandler(this.btn_capture_Click);
//            // 
//            // t_try_link
//            // 
//            this.t_try_link.Interval = 3000;
//            this.t_try_link.Tick += new System.EventHandler(this.t_try_link_Tick);
//            // 
//            // lbl_conn_status
//            // 
//            this.lbl_conn_status.AutoSize = true;
//            this.lbl_conn_status.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
//            this.lbl_conn_status.Location = new System.Drawing.Point(9, 20);
//            this.lbl_conn_status.Name = "lbl_conn_status";
//            this.lbl_conn_status.Size = new System.Drawing.Size(68, 16);
//            this.lbl_conn_status.TabIndex = 17;
//            this.lbl_conn_status.Text = "連線中...";
//            // 
//            // icImagingControl2
//            // 
//            this.icImagingControl2.BackColor = System.Drawing.Color.Black;
//            this.icImagingControl2.DeviceListChangedExecutionMode = TIS.Imaging.EventExecutionMode.Invoke;
//            this.icImagingControl2.DeviceLostExecutionMode = TIS.Imaging.EventExecutionMode.AsyncInvoke;
//            this.icImagingControl2.ImageAvailableExecutionMode = TIS.Imaging.EventExecutionMode.MultiThreaded;
//            this.icImagingControl2.LiveDisplayPosition = new System.Drawing.Point(0, 0);
//            this.icImagingControl2.Location = new System.Drawing.Point(480, 56);
//            this.icImagingControl2.Name = "icImagingControl2";
//            this.icImagingControl2.Size = new System.Drawing.Size(455, 316);
//            this.icImagingControl2.TabIndex = 19;
//            // 
//            // tbx_pictures
//            // 
//            this.tbx_pictures.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
//            this.tbx_pictures.Location = new System.Drawing.Point(110, 610);
//            this.tbx_pictures.Name = "tbx_pictures";
//            this.tbx_pictures.Size = new System.Drawing.Size(679, 27);
//            this.tbx_pictures.TabIndex = 9;
//            this.tbx_pictures.Text = "C:\\DOSE\\Pictures";
//            // 
//            // label3
//            // 
//            this.label3.AutoSize = true;
//            this.label3.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
//            this.label3.Location = new System.Drawing.Point(12, 613);
//            this.label3.Name = "label3";
//            this.label3.Size = new System.Drawing.Size(92, 16);
//            this.label3.TabIndex = 16;
//            this.label3.Text = "Capture Path:";
//            // 
//            // btn_dir
//            // 
//            this.btn_dir.Location = new System.Drawing.Point(795, 611);
//            this.btn_dir.Name = "btn_dir";
//            this.btn_dir.Size = new System.Drawing.Size(75, 23);
//            this.btn_dir.TabIndex = 18;
//            this.btn_dir.Text = "資料夾選擇";
//            this.btn_dir.UseVisualStyleBackColor = true;
//            this.btn_dir.Click += new System.EventHandler(this.btn_dir_Click);
//            // 
//            // Form1
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.BackColor = System.Drawing.SystemColors.Control;
//            this.ClientSize = new System.Drawing.Size(1073, 664);
//            this.Controls.Add(this.icImagingControl2);
//            this.Controls.Add(this.btn_dir);
//            this.Controls.Add(this.lbl_conn_status);
//            this.Controls.Add(this.label3);
//            this.Controls.Add(this.btn_capture);
//            this.Controls.Add(this.tbx_pictures);
//            this.Controls.Add(this.icImagingControl1);
//            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
//            this.Name = "Form1";
//            this.Text = "Camera Window V0.7 2023_10_11";
//            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
//            this.Load += new System.EventHandler(this.Form1_Load);
//            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl1)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl2)).EndInit();
//            this.ResumeLayout(false);
//            this.PerformLayout();

//        }

//        #endregion
//        private TIS.Imaging.ICImagingControl icImagingControl1;
//        private System.Windows.Forms.Button btn_capture;
//        private System.Windows.Forms.Timer t_try_link;
//        private System.Windows.Forms.Label lbl_conn_status;
//        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
//        private TIS.Imaging.ICImagingControl icImagingControl2;
//        private System.Windows.Forms.TextBox tbx_pictures;
//        private System.Windows.Forms.Label label3;
//        private System.Windows.Forms.Button btn_dir;
//    }
//}

