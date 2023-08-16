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
using TwinCAT.Ads;  //Step1引用ADS    在References下新增TwinCAT.Ads(如有安裝TwinCAT，DLL位置在C:\TwinCAT\AdsApi\.NET\v4.0.30319\TwinCAT.Ads.dll)

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
        private int hDigitalInputs;
        private int hDigitalOutputs;

        //Step4 C#端IO實際變數宣告
        [MarshalAs(UnmanagedType.I1)]
        private bool DigitalInput1;
        private bool DigitalInput2;
        private bool DigitalInput3;
        private bool DigitalInput4;
        private bool DigitalInput5;
        private bool DigitalInput6;
        private bool DigitalInput7;
        private bool DigitalInput8;
        private bool[,] DigitalOutputs = new bool[doNum, 16];


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Step5 填入要連結的ADS位置
                adsClient.Connect(853);
            }
            catch (Exception err)
            {
                MessageBox.Show("853");
                MessageBox.Show(err.Message);
            }

            try
            {
                //Step6 取得IO Handle數值
                // hDigitalInputs = adsClient.CreateVariableHandle("GVL.DigitalInputs");
                hDigitalOutputs = adsClient.CreateVariableHandle("GVL.DigitalOutputs");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        //Read
        private void btn_read_Click(object sender, EventArgs e)
        {
            //using (TcAdsClient client = new TcAdsClient())
            //{
            //    client.Connect(853); // Connect to local port 851 (PLC)              

            //    try
            //    {
            //        DigitalInputs = (bool[,])adsClient.ReadAny(hDigitalInputs, typeof(bool[,]), new int[] { diNum, 16 });
            //        for (int i = 0; i < 16; i++)
            //        {
            //            MessageBox.Show(DigitalInputs[0, i].ToString());
            //        }
            //    }
            //    catch (Exception err)
            //    {
            //        MessageBox.Show(err.Message);
            //    }
            //}
        }

        //Write
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //Step8 IO資料寫入
                for (int i = 0; i < 16; i++)
                {
                    DigitalOutputs[0, i] = true;
                }

                // 寫入陣列                
                adsClient.WriteAny(hDigitalOutputs, DigitalOutputs);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //Step5 填入要連結的ADS位置
                adsClient.Connect(853);
            }
            catch (Exception err)
            {
                MessageBox.Show("853");
                MessageBox.Show(err.Message);
            }

            try
            {
                //Step6 取得IO Handle數值
                // hDigitalInputs = adsClient.CreateVariableHandle("GVL.DigitalInputs");
                hDigitalOutputs = adsClient.CreateVariableHandle("GVL.DigitalOutputs");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }







            try
            {
                //   adsClient.Connect(853);

                dataStream = new AdsStream(31);
                //Encoding is set to ASCII, to read strings
                binRead = new BinaryReader(dataStream);
                hConnect = new int[8];

                //增加ADS主動回報
                hConnect[0] = adsClient.AddDeviceNotification("GVL.DigitalInput1", dataStream, 0, 1, AdsTransMode.OnChange, 100, 0, DigitalInput1);
                hConnect[1] = adsClient.AddDeviceNotification("GVL.DigitalInput2", dataStream, 1, 2, AdsTransMode.OnChange, 100, 0, DigitalInput2);
                hConnect[2] = adsClient.AddDeviceNotification("GVL.DigitalInput3", dataStream, 2, 3, AdsTransMode.OnChange, 100, 0, DigitalInput3);
                hConnect[3] = adsClient.AddDeviceNotification("GVL.DigitalInput4", dataStream, 3, 4, AdsTransMode.OnChange, 100, 0, DigitalInput4);
                hConnect[4] = adsClient.AddDeviceNotification("GVL.DigitalInput5", dataStream, 4, 5, AdsTransMode.OnChange, 100, 0, DigitalInput5);
                hConnect[5] = adsClient.AddDeviceNotification("GVL.DigitalInput6", dataStream, 5, 6, AdsTransMode.OnChange, 100, 0, DigitalInput6);
                hConnect[6] = adsClient.AddDeviceNotification("GVL.DigitalInput7", dataStream, 6, 7, AdsTransMode.OnChange, 100, 0, DigitalInput7);
                hConnect[7] = adsClient.AddDeviceNotification("GVL.DigitalInput8", dataStream, 7, 8, AdsTransMode.OnChange, 100, 0, DigitalInput8);
                adsClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);
            }
            catch (Exception err)
            {
                MessageBox.Show("增加ADS主動回報");
                // MessageBox.Show(err.Message);
            }
        }

        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {
            try
            {
                if (e.NotificationHandle == hConnect[0])
                {
                    if (binRead.ReadBoolean())
                    {
                        MessageBox.Show("input1 is true");
                    }
                    else
                    {
                        MessageBox.Show("input1 is false");
                    }
                }
                else if (e.NotificationHandle == hConnect[1])
                {
                    if (binRead.ReadBoolean())
                    {
                        MessageBox.Show("input2 is true");
                    }
                    else
                    {
                        MessageBox.Show("input2 is false");
                    }
                }
                else if (e.NotificationHandle == hConnect[2])
                {
                    if (binRead.ReadBoolean())
                    {
                        MessageBox.Show("input3 is true");
                    }
                    else
                    {
                        MessageBox.Show("input3 is false");
                    }
                }
                else if (e.NotificationHandle == hConnect[3])
                {
                    if (binRead.ReadBoolean())
                    {
                        MessageBox.Show("input4 is true");
                    }
                    else
                    {
                        MessageBox.Show("input4 is false");
                    }
                }
                else if (e.NotificationHandle == hConnect[4])
                {
                    if (binRead.ReadBoolean())
                    {
                        MessageBox.Show("input5 is true");
                    }
                    else
                    {
                        MessageBox.Show("input5 is false");
                    }
                }
                else if (e.NotificationHandle == hConnect[5])
                {
                    if (binRead.ReadBoolean())
                    {
                        MessageBox.Show("input6 is true");
                    }
                    else
                    {
                        MessageBox.Show("input6 is false");
                    }
                }
                else if (e.NotificationHandle == hConnect[6])
                {
                    if (binRead.ReadBoolean())
                    {
                        MessageBox.Show("input7 is true");
                    }
                    else
                    {
                        MessageBox.Show("input7 is false");
                    }
                }
                else if (e.NotificationHandle == hConnect[7])
                {
                    if (binRead.ReadBoolean())
                    {
                        MessageBox.Show("input8 is true");
                    }
                    else
                    {
                        MessageBox.Show("input8 is false");
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    adsClient.DeleteDeviceNotification(hConnect[i]);
                }
            }
            catch (Exception err)
            {
                // MessageBox.Show(err.Message);
            }
            adsClient.Dispose();
        }

        private void btn_clamp_head_Click(object sender, EventArgs e)
        {

        }
    }
}