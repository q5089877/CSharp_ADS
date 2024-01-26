//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Timers;
//using System.Windows.Forms;
//using TwinCAT.Ads;
//using TIS.Imaging;

//namespace DOSE_CAMERA.MyClass
//{
//    class AdsLink
//    {
//        log log = new log();
//        private System.Timers.Timer retryTimer;
//        //Step2 ADS Client宣告
//        private TcAdsClient adsClient = new TcAdsClient();

//        //主動回報
//        private int[] hConnect = new int[2];
//        private AdsStream dataStream;
//        private BinaryReader binRead;

//        ////Step3 IO Handl宣告
//        ////  private int hDigitalInput3; //rotation

//        //private int hCameraReady, hCameraReady_ch2;   //camera ready handle
//        //private int hCapFinish, hCapFinish_ch2;     //capture finished handle

//        ////Step4 C#端IO實際變數宣告
//        //[MarshalAs(UnmanagedType.I1)]
//        //private bool bCameraReady;      //Camera Ready      
//        //private bool bCameraOn;
//        //private bool bCameraOn_ch2;         //capture
//        //private bool bCameraFinState;   //capture finished
//        //private bool bCameraInquire;    //camera Inquire

//        private Label mainform_label;
//        MyCamera camera_device_1, camera_device_2;
//        int current_try_count = 0;

//        public AdsLink(Label status_label, MyCamera camera_device_1, MyCamera camera_device_2)
//        {
//            this.camera_device_1 = camera_device_1;
//            this.camera_device_2 = camera_device_2;
//            mainform_label = status_label;
//            adsClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);
//            adsClient.Connect(851);
//            //camera_devce_1.str_CameraReady = "GVL.bCameraReady";
//            //camera_devce_1.str_CameraReady = "GVL.bCameraFinState";
//            //camera_devce_2.str_CameraReady = "GVL.bCameraReady_ch2";
//            //camera_devce_2.str_CameraReady = "GVL.bCameraFinState_ch2";
//            //camera_devce_1.offset = 0;
//            //camera_devce_2.offset = 1;
//        }

//        public void retry_conn_ADS()
//        {
//            retryTimer = new System.Timers.Timer();            
//            retryTimer.Interval = 1000;
//            retryTimer.Elapsed += new ElapsedEventHandler(RetryTimer_Elapsed);
//            retryTimer.Start();
//        }


//        //檢查ADS連線情況--連線顯示用
//        private void RetryTimer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            if (linkAds_camera(this.camera_device_1) && linkAds_camera(camera_device_2))
//            {
//                retryTimer.Stop();
//                mainform_label.Invoke((Action<string>)(msg => mainform_label.Text = msg), "連線成功");
//                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "連線成功" + current_try_count.ToString());
//            }
//            else //retry
//            {
//                current_try_count++;
//                string msg_txt = string.Format("重試次數{0}", current_try_count.ToString());
//                mainform_label.Invoke((Action<string>)(msg => mainform_label.Text = msg), msg_txt);
//                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + "連線失敗" + current_try_count.ToString());
//            }
//        }

//        public bool linkAds_camera(MyCamera camera_device)
//        {
//            try
//            {
//                //取得IO Handle數值
//                camera_device.hCameraReady = adsClient.CreateVariableHandle(camera_device.str_CameraReady);
//                camera_device.hCapFinish = adsClient.CreateVariableHandle(camera_device.str_CapFinish);
//                dataStream = new AdsStream(8);
//                //Encoding is set to ASCII, to read strings
//                binRead = new BinaryReader(dataStream);
//                ////增加ADS主動回報
//                hConnect[camera_device.offset] = adsClient.AddDeviceNotification("GVL.bCameraOn", dataStream, camera_device.offset, 1, AdsTransMode.OnChange, 100, 0,camera_device.bCameraOn);               
//                return true;
//            }
//            catch (Exception err)
//            {
//                return false;
//            }
//        }

//        //public bool linkAds_camera(MyCamera camera_device)
//        //{
//        //    try
//        //    {
//        //        //Step6 取得IO Handle數值
//        //        hCameraReady = adsClient.CreateVariableHandle("GVL.bCameraReady");  // CameraReady
//        //        hCapFinish = adsClient.CreateVariableHandle("GVL.bCameraFinState");     // capture finish             
//        //        hCameraReady_ch2 = adsClient.CreateVariableHandle("GVL.bCameraReady_ch2");  // CameraReady
//        //        hCapFinish_ch2 = adsClient.CreateVariableHandle("GVL.bCameraFinState_ch2");     // capture finish             

//        //        dataStream = new AdsStream(8);
//        //        //Encoding is set to ASCII, to read strings
//        //        binRead = new BinaryReader(dataStream);

//        //        ////增加ADS主動回報
//        //        hConnect[0] = adsClient.AddDeviceNotification("GVL.bCameraOn", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, bCameraOn);
//        //        hConnect[1] = adsClient.AddDeviceNotification("GVL.bCameraOn_ch2", dataStream, 1, 1, AdsTransMode.OnChange, 100, 0, bCameraOn_ch2);
//        //        return true;
//        //    }
//        //    catch (Exception err)
//        //    {
//        //        return false;
//        //    }
//        //}

//        //private bool linkAds_camera_2()
//        //{
//        //    //try
//        //    //{           

//        //    //    //Step6 取得IO Handle數值
//        //    //    hCameraReady = adsClient.CreateVariableHandle("GVL.bCameraReady_ch2");  // CameraReady
//        //    //    hCapFinish = adsClient.CreateVariableHandle("GVL.bCameraFinState_ch2");     // capture finish             

//        //    //    dataStream = new AdsStream(8);
//        //    //    //Encoding is set to ASCII, to read strings
//        //    //    binRead = new BinaryReader(dataStream);

//        //    //    //增加ADS主動回報
//        //    //    hConnect[0] = adsClient.AddDeviceNotification("GVL.bCameraOn_ch2", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, bCameraOn);
//        //    //    //   hConnect[1] = adsClient.AddDeviceNotification("GVL.bCameraInquire", dataStream, 2, 3, AdsTransMode.OnChange, 100, 0, bCameraInquire);
//        //    //    return true;
//        //    //}
//        //    //catch (Exception err)
//        //    //{
//        //    //    return false;
//        //    //}
//        //}


//        private void OnNotification(object sender, AdsNotificationEventArgs e)
//        {
//            try
//            {
//                if (e.NotificationHandle == hConnect[camera_device_1.offset]) //Camera_1 capture image
//                {
//                    if (binRead.ReadBoolean())
//                    {
//                        log.WriteLog("if (e.NotificationHandle == hConnect[0])");
//                        //try
//                        //{
//                        //    //資料夾不存在, 就建立資料夾
//                        //    if (Directory.Exists(@tbx_pictures.Text) == false)
//                        //    {
//                        //        Directory.CreateDirectory(@tbx_pictures.Text);
//                        //    }

//                        //    log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " 拍照準備中");

//                        //    //執行拍照
//                        //    TIS.Imaging.FrameSnapSink snapSink = icImagingControl1.Sink as TIS.Imaging.FrameSnapSink;
//                        //    TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(5));

//                        //    string dateString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"); // 使用特定日期格式  
//                        //    frm.SaveAsJpeg(@tbx_pictures.Text + "\\" + dateString + ".jpg", 100);

//                        //    //拍照完成
//                        //    bCameraFinState = true;
//                        //    log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " 完成拍照*********");
//                        //}
//                        //catch (Exception err)
//                        //{
//                        //    log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " OnNotification to Capture: " + err.ToString());
//                        //    MessageBox.Show(err.Message);
//                        //}
//                    }
//                    else
//                    {
//                        //尚未拍照
//                        camera_device_1.bCameraFinState = false;
//                        //  MessageBox.Show("尚未拍照");
//                    }

//                    //ADS回寫     
//                    adsClient.WriteAny(camera_device_1.hCapFinish, camera_device_1.bCameraFinState);
//                }
//                else if (e.NotificationHandle == hConnect[camera_device_2.offset]) //caomera_2 capture image
//                {
//                    log.WriteLog("if (e.NotificationHandle == hConnect[1])");
//                    //ADS回寫
//                    adsClient.WriteAny(camera_device_2.hCapFinish, camera_device_2.bCameraFinState);
//                }
//            }
//            catch (Exception err)
//            {
//                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " OnNotification to Capture: " + err.ToString());
//                MessageBox.Show(err.Message);
//            }
//        }

//    }
//}
