using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwinCAT.Ads;
using TIS.Imaging;
using System.Timers;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //IO數量宣告     
        private const int doNum = 2;

        //Step2 ADS Client宣告
        private TcAdsClient adsClient = new TcAdsClient();

        //主動回報
        private int[] hConnect;
        private AdsStream dataStream;
        private BinaryReader binRead;

        //Step3 IO Handl宣告
        //  private int hDigitalInput3; //rotation

        private int hCameraReady;   //camera ready handle
        private int hCapFinish;     //capture finished handle
        //private int hDigitalOutputs;
        //private int hAnalogOutputs;

        //Step4 C#端IO實際變數宣告
        [MarshalAs(UnmanagedType.I1)]
        private bool bCameraReady;  //Camera Ready      
        private bool bCameraOn;   //capture
        private bool bCameraFinState; //capture finished
        private bool bCameraInquire;       //camera Inquire
        //private bool[,] DigitalOutputs = new bool[doNum, 16];
        //private double[,] AnalogOutputs = new double[doNum, 16];

        private System.Timers.Timer retryTimer;
        int currentRetry = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    //啟用camera
                    MessageBox.Show("請選擇 RGB24(720*540)");
                    icImagingControl1.Sink = new TIS.Imaging.FrameSnapSink();
                    icImagingControl1.ShowDeviceSettingsDialog();
                    icImagingControl1.LiveStart();

                    //clear log file
                    FileStream file = File.Open("log.txt", FileMode.Create);
                    StreamWriter writer = new StreamWriter(file);
                    writer.WriteLine("-------Log Start---------");
                    writer.Close();
                    file.Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
                retryTimer = new System.Timers.Timer();
                retryTimer.Interval = 1000;
                retryTimer.Elapsed += new ElapsedEventHandler(RetryTimer_Elapsed);

                retryConnADS();
                adsClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void retryConnADS()
        {
            currentRetry = 0;
            retryTimer.Start();
        }

        //檢查ADS連線情況--連線顯示用
        private void RetryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (linkAds())
            {
                // 操作成功，停止定时器并更新UI
                retryTimer.Stop();

                //主緒行緒委派
                this.Invoke(new Action(() =>
                {
                    this.t_try_link.Enabled = true; // check link status
                    this.bCameraReady = true;
                    this.adsClient.WriteAny(this.hCameraReady, this.bCameraReady);
                }));

                UpdateUI("連線成功！");
                WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "連線成功" + currentRetry.ToString());
            }
            else
            {
                // 操作失败，继续重试
                currentRetry++;
                UpdateUI($"第 {currentRetry} 次重試...");
                WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "連線失敗" + currentRetry.ToString());
            }
        }




        private void WriteLog(string log)
        {
            FileStream file = File.Open("log.txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine(log);
            writer.Close();
            file.Close();
        }


        private bool linkAds()
        {
            try
            {
                adsClient.Connect(851);

                //Step6 取得IO Handle數值
                hCameraReady = adsClient.CreateVariableHandle("GVL.bCameraReady");  // CameraReady
                hCapFinish = adsClient.CreateVariableHandle("GVL.bCameraFinState");     // capture finish                
                //hDigitalOutputs = adsClient.CreateVariableHandle("GVL.DigitalOutputs");
                //hAnalogOutputs = adsClient.CreateVariableHandle("GVL.AnalogOutputs");

                dataStream = new AdsStream(8);
                //Encoding is set to ASCII, to read strings
                binRead = new BinaryReader(dataStream);
                hConnect = new int[2];

                //增加ADS主動回報
                hConnect[0] = adsClient.AddDeviceNotification("GVL.bCameraOn", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, bCameraOn);
           //     hConnect[1] = adsClient.AddDeviceNotification("GVL.bCameraInquire", dataStream, 2, 3, AdsTransMode.OnChange, 100, 0, bCameraInquire);
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }


        private void UpdateUI(string message)
        {
            // 使用 Invoke 来在 UI 线程上更新 UI 元素
            if (lbl_conn_status.InvokeRequired)
            {
                lbl_conn_status.Invoke(new Action<string>(UpdateUI), new object[] { message });
            }
            else
            {
                lbl_conn_status.Text = message;
            }
        }

        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {
            try
            {
                if (e.NotificationHandle == hConnect[0]) //capture
                {
                    if (binRead.ReadBoolean())
                    {
                        try
                        {
                            //資料夾不存在, 就建立資料夾
                            if (Directory.Exists(@tbx_pictures.Text) == false)
                            {
                                Directory.CreateDirectory(@tbx_pictures.Text);                              
                            }
                            //執行拍照
                            TIS.Imaging.FrameSnapSink snapSink = icImagingControl1.Sink as TIS.Imaging.FrameSnapSink;
                            TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(3));

                            string dateString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"); // 使用特定日期格式  
                            frm.SaveAsJpeg(@tbx_pictures.Text + "\\" + dateString + ".jpg", 100);

                            //拍照完成
                            bCameraFinState = true;
                            //    MessageBox.Show("拍照完成");
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                        }
                    }
                    else
                    {
                        //尚未拍照
                        bCameraFinState = false;
                        //  MessageBox.Show("尚未拍照");
                    }
                    //ADS回寫
                    adsClient.WriteAny(hCapFinish, bCameraFinState);
                }
                else if (e.NotificationHandle == hConnect[1]) //camera ready
                {
                    if (binRead.ReadBoolean())
                    {
                        if (cameraTest())
                        {
                            bCameraReady = true;
                        }
                        else
                        {
                            bCameraReady = false;
                        }
                    }
                    //else
                    //{
                    //    bCameraReady = false;
                    //}

                    //ADS回寫
                    adsClient.WriteAny(hCameraReady, bCameraReady);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        bool cameraTest()
        {
            try
            {
                //資料夾不存在, 就建立資料夾
                if (Directory.Exists(@"c:\temp\") == false)
                {
                    Directory.CreateDirectory(@"c:\temp\");                  
                }
                //執行拍照
                TIS.Imaging.FrameSnapSink snapSink = icImagingControl1.Sink as TIS.Imaging.FrameSnapSink;
                TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(3));
                frm.SaveAsJpeg(@"C:\temp\temp" + ".jpg", 100);
                File.Delete(@"C:\temp\temp" + ".jpg");
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //主動告知Camera下線
                bCameraReady = false;
                adsClient.WriteAny(hCameraReady, bCameraReady);

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

        private void btn_capture_Click(object sender, EventArgs e)
        {
            try
            {
                //資料夾不存在, 就建立資料夾
                if (Directory.Exists(@tbx_pictures.Text) == false)
                {
                    Directory.CreateDirectory(@tbx_pictures.Text);                
                }
                //執行拍照
                TIS.Imaging.FrameSnapSink snapSink = icImagingControl1.Sink as TIS.Imaging.FrameSnapSink;
                TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(5));

                string dateString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"); // 使用特定日期格式  
                frm.SaveAsJpeg(@tbx_pictures.Text + "\\" + dateString + ".jpg", 100);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void t_try_link_Tick(object sender, EventArgs e)
        {
            try
            {
                //adsClient.Connect(851);
                //hCameraReady = adsClient.CreateVariableHandle("GVL.bCameraReady");  // CameraReady
                //WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " try adsClient.Connect(851)");
            }
            catch
            {
                t_try_link.Enabled = false;
                this.lbl_conn_status.Text = "連線中斷，請啟動Twincat後，再重啟軟體";
                MessageBox.Show("連線中斷，請啟動Twincat後，再重啟軟體");
            }
        }
    }
}