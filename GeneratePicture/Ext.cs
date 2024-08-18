using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneratePicture
{
    static class Ext
    {
        public static void Show(this string str) => MessageBox.Show(str);
        public static Image GetImage(this string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                int len = (int)fs.Length;
                byte[] data = new byte[len];
                fs.Read(data, 0, len);
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(data, 0, len);

                var img = Image.FromStream(memoryStream);
                return img;
            }
        }

       
    }
}
