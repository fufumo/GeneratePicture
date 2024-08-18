using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneratePicture
{
    public partial class FrmScreen : Form
    {
        private Dictionary<string, Image> imageCache = new Dictionary<string, Image>();
        private PictureBox pb;
        public FrmScreen()
        {
            InitializeComponent();
            pb = new PictureBox();
        }
        private void FrmScreen_Load(object sender, EventArgs e)
        {
            lbPb.ItemHeight = 45;
            lbPb.DrawMode = DrawMode.OwnerDrawFixed;

            cbModel.Items.Clear();
            cbModel.Items.Add("Model Seven");
            cbModel.Items.Add("Model Ten");
            cbModel.SelectedIndex = 0;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "图片|*.png;*.jpg";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;
                ofd.FileNames.ToList().ForEach(f =>
                {
                    lbPb.Items.Add(f);
                });
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbPb.SelectedItems == null)
            {
                "Please select one image to remove.".Show();
                return;
            }
            var obj = lbPb.SelectedItem;
            lbPb.Items.Remove(obj);
        }


        private void btnUp_Click(object sender, EventArgs e)
        {
            var obj = lbPb.SelectedItem;
            var index = lbPb.SelectedIndex - 1;
            if (index < 0) index = 0;
            lbPb.Items.Remove(obj);
            lbPb.Items.Insert(index, obj);
            lbPb.SelectedIndex = index;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbPb.Items.Clear();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var obj = lbPb.SelectedItem;
            var index = lbPb.SelectedIndex + 1;
            if (index > lbPb.Items.Count - 1) index = lbPb.Items.Count - 1;
            lbPb.Items.Remove(obj);
            lbPb.Items.Insert(index, obj);
            lbPb.SelectedIndex = index;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            int needCount = cbModel.SelectedIndex == 0 ? 7 : 10;
            var imgList = lbPb.Items.OfType<String>().ToList();
            var titleList = txtTitle.Lines.ToList();
            if (imgList.Count < needCount)
            {
                "Please check pictures's Count.".Show();
                return;
            }
            imgList = imgList.Take(needCount).ToList();
            if (titleList.Count < needCount)
            {
                var addCount = needCount - titleList.Count;
                for (int i = 0; i < addCount; i++)
                {
                    titleList.Add("");
                }
            }
            titleList = titleList.Take(needCount).ToList();

            ImageTool tool = new ImageTool();
            tool.imgList = imgList;
            tool.titleList = titleList;
            tool.GridCount = needCount == 7 ? 2 : 3;
            tool.Pb = pb;
            pbPreview.Image = tool.Generate();
        }


        private void btnDownload_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "图片|*.png";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                pbPreview.Image.Save(sfd.FileName);
                "Save success.".Show();
                lbPb.Items.Clear();
                txtTitle.Clear();
                pbPreview.Image = null;
            }
        }

        //drop files
        private void lbPb_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var f in files)
                {
                    if (f.EndsWith(".png") || f.EndsWith(".jpg"))
                    {
                        lbPb.Items.Add(f);
                    }
                }
            }
        }

        private void lbPb_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void lbPb_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void lbPb_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string imagePath = lbPb.Items[e.Index].ToString();
            Font font = lbPb.Font;

            var image = GetCachedImage(imagePath);

            e.DrawBackground();
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            e.Graphics.DrawImage(
                image,
                e.Bounds.Left,
                e.Bounds.Top,
                65,
                45
                );

            e.Graphics.DrawString(
                imagePath, 
                font, 
                Brushes.Black, 
                e.Bounds.Left + e.Bounds.Height + 20, 
                e.Bounds.Top + (e.Bounds.Height - font.Height) / 2
                );


            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.DrawRectangle(
                    Pens.Blue, 
                    e.Bounds.Left, 
                    e.Bounds.Top, 
                    e.Bounds.Width - 1, 
                    e.Bounds.Height - 1
                    );
            }

            e.DrawFocusRectangle();
        }

        private Image GetCachedImage(string path)
        {
            if (!imageCache.ContainsKey(path))
            {
                try
                {
                    Image img = path.GetImage();
                    imageCache[path] = img.GetThumbnailImage(65, 45, () => false, IntPtr.Zero);
                }
                catch
                {
                    imageCache[path] = null;
                }
            }
            return imageCache[path];
        }
    }
}
