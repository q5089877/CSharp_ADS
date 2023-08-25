﻿using System;
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
        private int hDigitalOutputs;
        private int hAnalogOutputs;

        //Step4 C#端IO實際變數宣告
        [MarshalAs(UnmanagedType.I1)]
        private bool bCameraReady;  //Camera Ready      
        private bool bDI_capture;   //capture
        private bool bDO_CapFinish; //capture finished
        private bool bCameraInquire;       //camera Inquire
        private bool[,] DigitalOutputs = new bool[doNum, 16];
        private double[,] AnalogOutputs = new double[doNum, 16];

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
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
                retryTimer = new System.Timers.Timer();
                retryTimer.Interval = 500;
                retryTimer.Elapsed += new ElapsedEventHandler(RetryTimer_Elapsed);
                retryConnADS();
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

        private void RetryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (linkAds())
            {
                // 操作成功，停止定时器并更新UI
                retryTimer.Stop();
                
                UpdateUI("連線成功！");
            }
            else
            {
                // 操作失败，继续重试
                currentRetry++;
                UpdateUI($"第 {currentRetry} 次重试...");
            }
        }

        private bool linkAds()
        {
            try
            {
                adsClient.Connect(851);

                //Step6 取得IO Handle數值
                hCameraReady = adsClient.CreateVariableHandle("GVL.bCameraReady");  // CameraReady
                hCapFinish = adsClient.CreateVariableHandle("GVL.bDO_CapFinish");     // capture finish                
                hDigitalOutputs = adsClient.CreateVariableHandle("GVL.DigitalOutputs");
                hAnalogOutputs = adsClient.CreateVariableHandle("GVL.AnalogOutputs");

                dataStream = new AdsStream(8);
                //Encoding is set to ASCII, to read strings
                binRead = new BinaryReader(dataStream);
                hConnect = new int[2];

                //增加ADS主動回報
                hConnect[0] = adsClient.AddDeviceNotification("GVL.bDI_capture", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, bDI_capture);
                hConnect[1] = adsClient.AddDeviceNotification("GVL.bCameraInquire", dataStream, 2, 3, AdsTransMode.OnChange, 100, 0, bCameraInquire);
                adsClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        //回傳PLC CAMERA的狀態
        private void CameraStatus()
        {
            try
            {
                if (cameraTest())
                {
                    bCameraReady = true;              
                }
                else
                {
                    bCameraReady = false;                 
                }
                adsClient.WriteAny(hCameraReady, bCameraReady);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
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
                                MessageBox.Show("Created a new Directory.");
                            }
                            //執行拍照
                            TIS.Imaging.FrameSnapSink snapSink = icImagingControl1.Sink as TIS.Imaging.FrameSnapSink;
                            TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(3));

                            string dateString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"); // 使用特定日期格式  
                            frm.SaveAsJpeg(@tbx_pictures.Text + "\\" + dateString + ".jpg", 100);

                            //拍照完成
                            bDO_CapFinish = true;
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                        }
                    }
                    else
                    {
                        //尚未拍照
                        bDO_CapFinish = false;
                    }
                    //ADS回寫
                    adsClient.WriteAny(hCapFinish, bDO_CapFinish);
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
                    else
                    {
                        bCameraReady = false;
                    }
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
                    MessageBox.Show("Created a new Directory.");
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

        private void btn_home_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    //to home
            //    DigitalInput5 = true;
            //    // 寫入陣列                
            //    adsClient.WriteAny(hDigitalInput5, DigitalInput5);
            //}
            //catch (Exception err)
            //{
            //    MessageBox.Show(err.Message);
            //}
        }

        private void btn_capture_Click(object sender, EventArgs e)
        {
            try
            {
                //資料夾不存在, 就建立資料夾
                if (Directory.Exists(@tbx_pictures.Text) == false)
                {
                    Directory.CreateDirectory(@tbx_pictures.Text);
                    MessageBox.Show("Created a new Directory.");
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
    }
}