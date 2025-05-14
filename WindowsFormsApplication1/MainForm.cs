using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using TIS.Imaging;
using TwinCAT.Ads;
using DOSE_CAMERA.MyClass;
using System.Runtime.InteropServices;
using System.Threading;
using System.Configuration;

namespace DOSE_CAMERA
{
    public partial class MainForm : Form
    {
        log log = new log();
        //AdsLink ads_link;
        MyCapture capture_camera_1, capture_camera_2;

        private System.Timers.Timer retryTimer;
        //Step2 ADS Client宣告
        private TcAdsClient adsClient = new TcAdsClient();

        //主動回報
        private int[] hConnect;

        private AdsStream dataStream;
        private AdsStream dataStream_ch2;
        private BinaryReader binRead;
        private BinaryReader binRead_ch2;

        private int hCameraReady;   //camera ready handle
        private int hCapFinish;     //capture finished handle
        private int hCameraReady_ch2;   //camera ready handle
        private int hCapFinish_ch2;     //capture finished handle

        [MarshalAs(UnmanagedType.I1)]
        private bool bCameraReady;  //Camera Ready      
        private bool bCameraOn;   //capture
        private bool bCameraFinState; //capture finished
        private bool bCameraInquire;       //camera Inquire
        private bool bCameraReady_ch2;  //Camera Ready      
        private bool bCameraOn_ch2;   //capture
        private bool bCameraFinState_ch2; //capture finished
        private bool bCameraInquire_ch2;       //camera Inquire

        int current_try_count = 0;

        private System.Windows.Forms.Timer cameraWatchdogTimer;
        private DateTime lastFrameTime_cam1 = DateTime.Now;
        private DateTime lastFrameTime_cam2 = DateTime.Now;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 先啟動相機串流
            InitCamera(icImagingControl1, 1);
            InitCamera(icImagingControl2, 2);

            capture_camera_1 = new MyCapture(icImagingControl1);
            capture_camera_2 = new MyCapture(icImagingControl2);

            // ADS 連線與通知
            try
            {
                adsClient.AdsNotification += OnNotification;
                adsClient.Connect(851);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ADS 連線失敗：" + ex.Message);
                log.WriteLog($"ADS 連線失敗：{ex}");
            }

            // 啟動重試計時器...
            retryTimer = new System.Timers.Timer(1000);
            retryTimer.Elapsed += RetryTimer_Elapsed;
            retryTimer.Start();
        }

        public void InitCamera(ICImagingControl ctrl, int index)
        {
            string prefix = "Camera" + index.ToString() + "_";
            string name = ConfigurationManager.AppSettings[prefix + "Name"];
            string serial = ConfigurationManager.AppSettings[prefix + "Serial"];
            string format = ConfigurationManager.AppSettings[prefix + "Format"];
            string fpsText = ConfigurationManager.AppSettings[prefix + "FPS"];
            bool flipH = false, flipV = false;
            float fps = 0;

            bool.TryParse(ConfigurationManager.AppSettings[prefix + "FlipH"], out flipH);
            bool.TryParse(ConfigurationManager.AppSettings[prefix + "FlipV"], out flipV);
            float.TryParse(fpsText, out fps);

            try
            {
                // 1. 選擇裝置（Name + Serial）
                foreach (Device dev in ctrl.Devices)
                {
                    if (dev.Name == name)
                    {
                        ctrl.Device = dev;
                        ctrl.Sink = new FrameSnapSink();  // ✅ 關鍵補上這行
                        break;
                    }
                }

                // 2. 設定解析度+像素格式
                foreach (VideoFormat vf in ctrl.VideoFormats)
                {
                    if (vf.ToString().Contains(format))
                    {
                        ctrl.VideoFormat = vf;
                        break;
                    }
                }

                // 3. 設定幀率（DeviceFrameRate）
                if (fps > 0 && ctrl.DeviceFrameRateAvailable)
                {
                    ctrl.DeviceFrameRate = fps;
                }

                // 4. 設定翻轉（DeviceFlipHorizontal/Vertical）
                if (ctrl.DeviceFlipHorizontalAvailable)
                    ctrl.DeviceFlipHorizontal = flipH;
                if (ctrl.DeviceFlipVerticalAvailable)
                    ctrl.DeviceFlipVertical = flipV;

                // 5. 啟動串流
                ctrl.LiveDisplayDefault = true;
                ctrl.LiveStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Camera" + index.ToString() + " 初始化錯誤：" + ex.Message);
            }
        }


        //檢查ADS連線情況--連線顯示用
        private void RetryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (link_ADS())
            {
                retryTimer.Stop();
                this.lbl_link_status.Invoke((Action<string>)(msg => this.lbl_link_status.Text = msg), "連線成功");
                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "連線成功" + current_try_count.ToString());
            }
            else //retry
            {
                current_try_count++;
                string msg_txt = string.Format("重試次數{0}", current_try_count.ToString());
                this.lbl_link_status.Invoke((Action<string>)(msg => this.lbl_link_status.Text = msg), msg_txt);
                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "連線失敗" + current_try_count.ToString());
            }
        }

        private bool link_ADS()
        {
            try
            {
                //取得IO Handle數值
                hCameraReady = adsClient.CreateVariableHandle("GVL.bCameraReady");  // CameraReady
                hCapFinish = adsClient.CreateVariableHandle("GVL.bCameraFinState");     // capture finish   

                hCameraReady_ch2 = adsClient.CreateVariableHandle("GVL.bCameraReady_ch2");  // CameraReady
                hCapFinish_ch2 = adsClient.CreateVariableHandle("GVL.bCameraFinState_ch2");     // capture finish   

                dataStream = new AdsStream(8);
                binRead = new BinaryReader(dataStream);
                hConnect = new int[2];

                //增加ADS主動回報
                hConnect[0] = adsClient.AddDeviceNotification("GVL.bCameraOn", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, bCameraOn);
                hConnect[1] = adsClient.AddDeviceNotification("GVL.bCameraOn_ch2", dataStream, 1, 1, AdsTransMode.OnChange, 100, 0, bCameraOn_ch2);
                //hConnect[3] = adsClient.AddDeviceNotification("GVL.bCameraReady", dataStream, 2, 1, AdsTransMode.OnChange, 100, 0, bCameraOn_ch2);
                //hConnect[4] = adsClient.AddDeviceNotification("GVL.bCameraReady_ch2", dataStream, 3, 1, AdsTransMode.OnChange, 100, 0, bCameraOn_ch2);

                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {
            try
            {
                //利用hConnect[0] = adsClient.AddDeviceNotification("GVL.bCameraOn", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, bCameraOn);
                if (e.NotificationHandle == hConnect[0]) //Camera_1 capture image
                {
                    //拍照
                    if (binRead.ReadBoolean())
                    {
                        log.WriteLog("if (e.NotificationHandle == hConnect[0])");
                        try
                        {
                            //資料夾不存在, 就建立資料夾
                            if (Directory.Exists(@tbx_pictures.Text) == false)
                            {
                                Directory.CreateDirectory(@tbx_pictures.Text);
                            }

                            log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " 拍照準備中");

                            //執行拍照
                            string dateString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"); // 使用特定日期格式  
                            if (capture_camera_1.capture_image(@tbx_pictures.Text))
                            {
                                bCameraFinState = true;
                                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " 完成拍照*********");
                            }
                            else
                            {
                                MessageBox.Show("拍照失敗");
                                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " 拍照失敗-----------------");
                            }
                        }
                        catch (Exception err)
                        {
                            log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " OnNotification to Capture: " + err.ToString());
                            MessageBox.Show(err.Message);
                        }
                    }
                    else
                    {
                        //尚未拍照
                        bCameraFinState = false;
                    }
                    //ADS回寫     
                    adsClient.WriteAny(hCapFinish, bCameraFinState);
                }

                //hConnect[1] = adsClient.AddDeviceNotification("GVL.bCameraOn_ch2", dataStream, 1, 1, AdsTransMode.OnChange, 100, 0, bCameraOn_ch2);
                if (e.NotificationHandle == hConnect[1]) //caomera_2 capture image
                {
                    //拍照
                    if (binRead.ReadBoolean())
                    {
                        log.WriteLog("if (e.NotificationHandle == hConnect[1])");
                        try
                        {
                            //資料夾不存在, 就建立資料夾
                            if (Directory.Exists(@tbx_pictures.Text) == false)
                            {
                                Directory.CreateDirectory(@tbx_pictures.Text);
                            }

                            log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " 拍照準備中");

                            //執行拍照                           
                            if (capture_camera_2.capture_image(@tbx_pictures.Text))
                            {
                                bCameraFinState_ch2 = true;
                                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " 完成拍照*********");
                            }
                            else
                            {
                                MessageBox.Show("拍照失敗");
                                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " 拍照失敗-----------------");
                            }
                        }
                        catch (Exception err)
                        {
                            log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " OnNotification to Capture: " + err.ToString());
                            MessageBox.Show(err.Message);
                        }
                    }
                    else
                    {
                        //尚未拍照
                        bCameraFinState_ch2 = false;
                    }
                    //ADS回寫     
                    adsClient.WriteAny(hCapFinish_ch2, bCameraFinState_ch2);
                }

                if (capture_camera_1.camera_capture_test())
                {
                    //主緒行緒委派
                    this.Invoke(new Action(() =>
                    {
                        this.bCameraReady = true;
                        this.adsClient.WriteAny(this.hCameraReady, this.bCameraReady);
                    }));
                    log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "camera_capture_test1() 測試成功");
                }

                if (capture_camera_2.camera_capture_test())
                {
                    //主緒行緒委派
                    this.Invoke(new Action(() =>
                    {
                        this.bCameraReady_ch2 = true;
                        this.adsClient.WriteAny(this.hCameraReady_ch2, this.bCameraReady_ch2);
                    }));
                    log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "camera_capture_test2() 測試成功");
                }
                else
                {
                    log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "camera_capture_test() 測試失敗");
                }
            }
            catch (Exception err)
            {
                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " OnNotification to Capture: " + err.ToString());
            }
        }

        private void btn_capture_manual_Click(object sender, EventArgs e)
        {
            //執行拍照         
            if (!capture_camera_1.capture_image(@tbx_pictures.Text))
            {
                MessageBox.Show("Camera 1 拍照失敗");
            }
            Thread.Sleep(2000);
            if (!capture_camera_2.capture_image(@tbx_pictures.Text))
            {
                MessageBox.Show("Camera 2 拍照失敗");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                icImagingControl1.LiveStop();
                icImagingControl1.Sink = null;
                icImagingControl1.Device = null;

                icImagingControl2.LiveStop();
                icImagingControl2.Sink = null;
                icImagingControl2.Device = null;

                //主動告知Camera下線
                bCameraReady = false;
                bCameraReady_ch2 = false;
                adsClient.WriteAny(hCameraReady, bCameraReady);
                adsClient.WriteAny(hCameraReady_ch2, bCameraReady_ch2);

                for (int i = 0; i < 2; i++)
                {
                    adsClient.DeleteDeviceNotification(hConnect[i]);
                }


            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            adsClient.Dispose();
        }

        private void btn_dir_Click(object sender, EventArgs e)
        {
            //選使用者選擇照片存檔路徑
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "選擇一個資料夾";
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                tbx_pictures.Text = selectedFolder;
            }
        }
    }
}
