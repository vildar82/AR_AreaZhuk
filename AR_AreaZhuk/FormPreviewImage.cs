using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AR_AreaZhuk
{
    public partial class FormPreviewImage : Form
    {
        public Image image = null;
        static bool isPress = false;
        static Point startPst;
        public FormPreviewImage(Image image)
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(this_MouseWheel);
            isPress = false;
            this.image = image;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            //{
            //    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            //    pictureBox.Image = image;
            //}
            //else
            //{
                isPress = true;
                startPst = e.Location;
           // }
          
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPress)
            {
                Control control = (Control)sender;
                control.Top += e.Y - startPst.Y;
                control.Left += e.X - startPst.X;
            }
        }

        private void this_MouseWheel(System.Object sender, MouseEventArgs e)
        {
            if (pictureBox.Image != null)
            {
                if (e.Delta > 0)
                {
                    if ((pictureBox.Width < (15 * this.Width)) && (pictureBox.Height < (15 * this.Height)))
                    {
                        pictureBox.Width = (int)(pictureBox.Width * 1.25);
                        pictureBox.Height = (int)(pictureBox.Height * 1.25);
                        pictureBox.Top = (int)(e.Y - 1.25 * (e.Y - pictureBox.Top));
                        pictureBox.Left = (int)(e.X - 1.25 * (e.X - pictureBox.Left));
                    }
                }
                else
                {
                    if ((pictureBox.Width > (this.Width / 15)) && (pictureBox.Height > (this.Height / 15)))
                    {
                        pictureBox.Width = (int)(pictureBox.Width / 1.25);
                        pictureBox.Height = (int)(pictureBox.Height / 1.25);
                        if (pictureBox.Width != 750)
                        {
                            pictureBox.Top = (int)(e.Y - 0.80 * (e.Y - pictureBox.Top));
                            pictureBox.Left = (int)(e.X - 0.80 * (e.X - pictureBox.Left));
                        }
                    }
                }
            }
            //   Control control = (Control)sender;
            //if (pictureBox.Top + e.Y - startPst.Y < 23)
            //    pictureBox.Top = 23;
            //else
            //    pictureBox.Top += e.Y - startPst.Y;


        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;//проверка что нажата левая кнопка
            isPress = false;
        }

        private void FormPreviewImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) this.Close();
        }

        private void FormPreviewImage_Load(object sender, EventArgs e)
        {
            pictureBox.Image = image;
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox.Image = image;
            }
        }

        private void FormPreviewImage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)0x1B) this.Close();
        }



    }
}
