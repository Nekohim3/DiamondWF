using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiamondWF
{
    public partial class Fout : Form
    {
        public Fout(Bitmap pbmp, Bitmap tbmp, Bitmap ibmp)
        {
            InitializeComponent();
            this.pbmp = pbmp;
            this.tbmp = tbmp;
            this.ibmp = ibmp;
        }
        Bitmap pbmp;
        Bitmap tbmp;
        Bitmap ibmp;
        private void Fout_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pbmp;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "bmp|*.bmp|png|*.png";
            sfd.ShowDialog();
            string str = sfd.FileName;
            if (str.IndexOf("bmp") != -1)
            {
                pbmp.Save(sfd.FileName.Replace(".bmp", "_p.bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
                tbmp.Save(sfd.FileName.Replace(".bmp", "_t.bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
                ibmp.Save(sfd.FileName.Replace(".bmp", "_i.bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
            }
            if (str.IndexOf("png") != -1)
            {
                pbmp.Save(sfd.FileName.Replace(".png", "_p.png"), System.Drawing.Imaging.ImageFormat.Png);
                tbmp.Save(sfd.FileName.Replace(".png", "_t.png"), System.Drawing.Imaging.ImageFormat.Png);
                ibmp.Save(sfd.FileName.Replace(".png", "_i.png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = pbmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = tbmp;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = ibmp;
        }
    }
}
