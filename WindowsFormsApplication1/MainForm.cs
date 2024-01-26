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

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            camera_init();

            capture_camera_1 = new MyCapture(icImagingControl1);
            capture_camera_2 = new MyCapture(icImagingControl2);

            adsClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);
            adsClient.Connect(851);

            if (!camera_capture_test())
            {
                MessageBox.Show("Camera拍照失敗");
                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "Camera拍照失敗");
            }
            else
            {
                MessageBox.Show("Camera拍照測試完成");
                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "Camera拍照測試完成");
            }

            retryTimer = new System.Timers.Timer();
            retryTimer.Interval = 1000;
            retryTimer.Elapsed += new ElapsedEventHandler(RetryTimer_Elapsed);
            retryTimer.Start();
        }

        void camera_init()
        {
            //啟用camera
            MessageBox.Show("請選擇 RGB24(720*540)，位置不對時，請更改Device Name即可");
            icImagingControl1.Sink = new TIS.Imaging.FrameSnapSink();
            icImagingControl1.ShowDeviceSettingsDialog();
            icImagingControl1.LiveStart();

            icImagingControl2.Sink = new TIS.Imaging.FrameSnapSink();
            icImagingControl2.ShowDeviceSettingsDialog();
            icImagingControl2.LiveStart();
        }

        bool camera_capture_test()
        {
            if (capture_camera_1.camera_capture_test() && capture_camera_2.camera_capture_test())
                return true;
            else
                return false;
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
                return true;
            }
            catch (Exception err)
            {
                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " link_ADS" + err.ToString());
                return false;
            }
        }

        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {
            try
            {
                if (e.NotificationHandle == hConnect[0]) //Camera_1 capture image
                {
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
                                //ADS回寫     
                                adsClient.WriteAny(hCapFinish, bCameraFinState);
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
                }

                if (e.NotificationHandle == hConnect[1]) //caomera_2 capture image
                {
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
                                //ADS回寫     
                                adsClient.WriteAny(hCapFinish_ch2, bCameraFinState_ch2);
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
            capture_camera_1.capture_image(@tbx_pictures.Text);
            Thread.Sleep(2000);        
            capture_camera_2.capture_image(@tbx_pictures.Text);
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
