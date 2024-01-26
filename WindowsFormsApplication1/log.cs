using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOSE_CAMERA
{
    class log
    {
        public void WriteLog(string log)
        {
            FileStream file = File.Open("log.txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine(log);
            writer.Close();
            file.Close();
        }
    }
}
