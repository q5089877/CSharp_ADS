using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS.Imaging;
using TwinCAT.Ads;

namespace DOSE_CAMERA.MyClass
{
    class MyCapture
    {
        log log = new log();
        ICImagingControl camera_device;
        public MyCapture(ICImagingControl camera_device)
        {
            this.camera_device = camera_device;
        }

        public bool camera_capture_test()
        {
            
            try
            {
                //資料夾不存在, 就建立資料夾
                if (Directory.Exists(@"c:\temp\") == false)
                {
                    Directory.CreateDirectory(@"c:\temp\");
                }
                //執行拍照
                TIS.Imaging.FrameSnapSink snapSink = camera_device.Sink as TIS.Imaging.FrameSnapSink;
                TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(3));
                frm.SaveAsJpeg(@"C:\temp\temp" + ".jpg", 100);
                File.Delete(@"C:\temp\temp" + ".jpg");
                return true;
            }
            catch(Exception err)
            {
                log.WriteLog(err.ToString());
                return false;
            }
        }

        public bool capture_image(string dir_path)
        {
            try
            {
                //資料夾不存在, 就建立資料夾
                if (Directory.Exists(@dir_path) == false)
                {
                    Directory.CreateDirectory(@dir_path);
                }
                //執行拍照
                TIS.Imaging.FrameSnapSink snapSink = camera_device.Sink as TIS.Imaging.FrameSnapSink;
                TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(5));

                string dateString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"); // 使用特定日期格式  
                frm.SaveAsJpeg(@dir_path + "\\" + dateString + ".jpg", 100);
                return true;
            }
            catch (Exception err)
            {
                log.WriteLog(DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + " btn_capture_Click");
                //MessageBox.Show(err.Message);
                return false;
            }
        }     
    }
}
