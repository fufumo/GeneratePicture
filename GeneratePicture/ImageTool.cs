using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneratePicture
{
    public class ImageTool
    {
        public PictureBox Pb { get; set; }
        public List<string> imgList { get; set; } = new List<string>();
        public int GridCount { get; set; } = 2;
        public List<String> titleList { get; set; } = new List<string>();
        public int Height { get; set; } = 1200;
        public int Width { get; set; } = 900;
        public int WhiteEdge { get; set; } = 2;
        public int ImageWidth { get => (this.Width - ((GridCount - 1) * WhiteEdge)) / GridCount; }
        public int ImageHeight { get => (this.Height - 3 * WhiteEdge) / 4; }
        public SolidBrush Brush { get; set; } = new SolidBrush(Color.White);
        public Bitmap Generate()
        {
            StringFormat sf = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            };
            Bitmap bmp = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.Clear(Color.White);

                g.DrawImg(imgList[0].GetImage(), 0, 0, Width, ImageHeight);
                g.DrawString(titleList[0],
                    new Font("MiSans Medium", 40),
                    Brush,
                    Width / 2,
                    ImageHeight - 40,
                    sf);

                imgList.RemoveAt(0);
                titleList.RemoveAt(0);

                for (int i = 0; i < imgList.Count; i++)
                {
                    var col = i % GridCount;
                    var row = i / GridCount;
                    var x = (ImageWidth + WhiteEdge) * col;
                    var y = (ImageHeight + WhiteEdge) * (row + 1);
                    g.DrawImg(imgList[i].GetImage(), x, y, ImageWidth, ImageHeight);

                    g.DrawString(titleList[i],
                                new Font("MiSans Medium", 40),
                                Brush,
                                x + ImageWidth / 2,
                                y + ImageHeight - 40,
                                sf);
                }
            }
            return bmp;
        }

    }
    public static class ImageExt
    {
        public static void DrawImg(this Graphics g, Image img, int x, int y, int w, int h)
        {
            g.DrawImage(img.CutImage(w, h), x, y, w, h);
        }

        public static Image CutImage(this Image img, int w, int h)
        {

            int originalWidth = img.Width;
            int originalHeight = img.Height;

            float scaleX = (float)originalWidth / w;
            float scaleY = (float)originalHeight / h;
            float scale = Math.Min(scaleX, scaleY);

            int scaledWidth = (int)(w * scale);
            int scaledHeight = (int)(h * scale);

            int cropX = (originalWidth - scaledWidth) / 2;
            int cropY = (originalHeight - scaledHeight) / 2;

            Bitmap croppedImage = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(img,
                     new Rectangle(0, 0, w, h),
                     new Rectangle(cropX, cropY, scaledWidth, scaledHeight),
                     GraphicsUnit.Pixel);
            }

            return croppedImage;
        }
    }
}
