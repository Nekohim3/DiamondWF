using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DiamondWF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int PSize = 17;
        int SSize = 2;
        int PxW = 0;
        int PxH = 0;
        int FPxW = 0;
        int FPxH = 0;
        int DPI = 197;
        int clustertype = 0;
        Bitmap sbmp;
        Bitmap szbmp;
        Bitmap template;
        Bitmap ztemplate;
        Bitmap tempInfo;
        RGBT[,] RGBTs;
        List<zna4> zna4s = new List<zna4>();
        Colors colors = new Colors();
        List<RGBT> cols = new List<RGBT>();
        List<RGBT> check = new List<RGBT>();
        List<RGBT> check1 = new List<RGBT>();
        List<RGBTP> UC = new List<RGBTP>();
        List<AFormat> AFList = new List<AFormat>();
        List<FFormat> FFList = new List<FFormat>();
        List<Cluster> Clust = new List<Cluster>();
        List<Cluster> Clust1 = new List<Cluster>();
        List<RGBTC> rgbtc = new List<RGBTC>();
        void AFormatsInit()
        {
            AFList.Clear();
            //AFList.Add(new AFormat("A0", (int)Math.Round((DPI / 25.4) * 1188,0), (int)Math.Round((DPI / 25.4) * 840,0)));
            AFList.Add(new AFormat("A1", (int)Math.Round((DPI / 25.4) * 840,0), (int)Math.Round((DPI / 25.4) * 594,0)));
            AFList.Add(new AFormat("A2", (int)Math.Round((DPI / 25.4) * 594,0), (int)Math.Round((DPI / 25.4) * 420,0)));
            AFList.Add(new AFormat("A3", (int)Math.Round((DPI / 25.4) * 420,0), (int)Math.Round((DPI / 25.4) * 297,0)));
            AFList.Add(new AFormat("A4", (int)Math.Round((DPI / 25.4) * 297,0), (int)Math.Round((DPI / 25.4) * 210,0)));
        }
        class AFormat
        {
            public string name { get; set; }
            public int pxw { get; set; }
            public int pxh { get; set; }
            public AFormat(string Name, int w, int h)
            {
                name = Name;
                pxw = w;
                pxh = h;
            }
        }
        void FFormatInit()
        {
            FFList.Clear();
            FFList.Add(new FFormat("none", (int)Math.Round((DPI / 25.4) * 0, 0), (int)Math.Round((DPI / 25.4) * 0, 0)));
            FFList.Add(new FFormat("40x30", (int)Math.Round((DPI / 25.4) * 400, 0), (int)Math.Round((DPI / 25.4) * 300, 0)));
            FFList.Add(new FFormat("25x20", (int)Math.Round((DPI / 25.4) * 250, 0), (int)Math.Round((DPI / 25.4) * 200, 0)));
        }
        class FFormat
        {
            public string name { get; set; }
            public int pxw { get; set; }
            public int pxh { get; set; }
            public FFormat(string Name, int w, int h)
            {
                name = Name;
                pxw = w;
                pxh = h;
            }
        }
        void TemplateCreate()
        {
            CB_Formats.Invoke(new Action(() => 
            {
                template = new Bitmap(AFList[CB_Formats.SelectedIndex].pxw, AFList[CB_Formats.SelectedIndex].pxh);
                ztemplate = new Bitmap(AFList[CB_Formats.SelectedIndex].pxw, AFList[CB_Formats.SelectedIndex].pxh);
                using (Graphics grp = Graphics.FromImage(template))
                {
                    grp.FillRectangle(Brushes.White, 0, 0, AFList[CB_Formats.SelectedIndex].pxw, AFList[CB_Formats.SelectedIndex].pxh);
                }
                using (Graphics grp = Graphics.FromImage(ztemplate))
                {
                    grp.FillRectangle(Brushes.White, 0, 0, AFList[CB_Formats.SelectedIndex].pxw, AFList[CB_Formats.SelectedIndex].pxh);
                }
            }));
            PxW = template.Width / (PSize + SSize) - 2;
            PxH = template.Height / (PSize + SSize) - 2;
            FPxW = Convert.ToInt32((double)FFList[CB_FS.SelectedIndex].pxw / (double)(PSize + SSize));
            FPxH = Convert.ToInt32((double)FFList[CB_FS.SelectedIndex].pxh / (double)(PSize + SSize));
        }
        int Resize1()
        {
            int done = 0;
            if (CB_FS.SelectedIndex == 0)
            {
                int nhsh = Convert.ToInt32(PxH * ((double)trackBar1.Value / 100)) * PSize;
                int nhsw = (sbmp.Width * nhsh) / sbmp.Height;
                int nwsw = Convert.ToInt32(PxW * ((double)trackBar1.Value / 100)) * PSize;
                int nwsh = (sbmp.Height * nwsw) / sbmp.Width;
                try
                {
                    if (nhsw <= PxW * PSize)
                    {
                        szbmp = new Bitmap(sbmp, nhsw - (nhsw % PSize), nhsh);
                        done = 0;
                    }
                    if (nwsh <= PxH * PSize && done == 0)
                    {
                        szbmp = new Bitmap(sbmp, nwsw, nwsh - (nwsh % PSize));
                        done = 0;
                    }
                }
                catch
                {
                    done = 1;
                }
            }
            else
            {
                int nhsh = FPxH * PSize;
                int nhsw = (sbmp.Width * nhsh) / sbmp.Height;
                int nwsw = FPxW * PSize;
                int nwsh = (sbmp.Height * nwsw) / sbmp.Width;
                if (nhsw > PxW * PSize || nwsh > PxH * PSize)
                {
                    MessageBox.Show("Не влезает");
                    return 2;
                }
                if (nhsw <= PxW * PSize)
                {
                    szbmp = new Bitmap(sbmp, nhsw - (nhsw % PSize), nhsh);
                    done = 0;
                }
                if (nwsh <= PxH * PSize && done != 0)
                {
                    szbmp = new Bitmap(sbmp, nwsw, nwsh - (nwsh % PSize));
                    done = 0;
                }
                if (done != 0)
                    MessageBox.Show("Не влезает");
            }
            return done;
        }
        void Pixelize()
        {
            Invoke(new Action(() =>
            {
                this.Text = "Пикселизация изображения";
            }));
            int posx = 0;
            int posy = 0;
            RGBTs = new RGBT[szbmp.Width / PSize, szbmp.Height / PSize];
            LockBitmap lb = new LockBitmap(szbmp);
            lb.LockBits();
            for (int i = 0; i < szbmp.Width; i += PSize)
            {
                for (int j = 0; j < szbmp.Height; j += PSize)
                {
                    int AR = 0;
                    int AG = 0;
                    int AB = 0;
                    for (int h = 0; h < PSize; h++)
                    {
                        for (int w = 0; w < PSize; w++)
                        {
                            AR += lb.GetPixel(i + h, j + w).R;
                            AG += lb.GetPixel(i + h, j + w).G;
                            AB += lb.GetPixel(i + h, j + w).B;
                        }
                    }
                    AR /= PSize * PSize;
                    AG /= PSize * PSize;
                    AB /= PSize * PSize;
                    RGBTs[posx, posy] = new RGBT(AR, AG, AB);
                    posy++;
                }
                posx++;
                posy = 0;
            }
            lb.UnlockBits();
        }
        void CollectColors()
        {
            cols.Clear();
            for (int i = 0; i < RGBTs.GetUpperBound(0); i++)
            {
                for (int j = 0; j < RGBTs.GetUpperBound(1); j++)
                {
                    cols.Add(RGBTs[i, j]);
                }
            } 
            LB_UC.Invoke(new Action(() =>
            {
                LB_UC.Text = "Использовано цветов: " + cols.Count;
            }));
        }
        void Clusterize()
        {
            Invoke(new Action(() =>
            {
                this.Text = "Кластеризация палитры";
            }));
            int th = 10;
            trackBar2.Invoke(new Action(() =>
            {
                th = trackBar2.Value;
            }));
            Clust.Clear();
            Clust1.Clear();
            while (cols.Count != 0)
            {
                bool deleted = false;
                foreach (Cluster w in Clust)
                {
                    if(clustertype == 0)
                    if (Math.Abs(cols.First().R - w.center.R) <= th && Math.Abs(cols.First().G - w.center.G) <= th && Math.Abs(cols.First().B - w.center.B) <= th)
                    {
                        w.Add(cols.First());
                        cols.RemoveAt(0);
                        deleted = true;
                        break;
                    }
                    if (clustertype == 1)
                    if (Math.Sqrt(Math.Pow(cols.First().R - w.center.R, 2) + Math.Pow(cols.First().G - w.center.G, 2) + Math.Pow(cols.First().B - w.center.B, 2)) < th)
                    {
                        w.Add(cols.First());
                        cols.RemoveAt(0);
                        deleted = true;
                        break;
                    }
                }
                if (!deleted)
                {
                    Clust.Add(new Cluster(cols.First()));
                    cols.RemoveAt(0);
                }
            }
            if (checkBox1.Checked)
            {
                foreach (Cluster q in Clust)
                {
                    bool added = false;
                    foreach (Cluster w in Clust1)
                    {
                        if (Math.Sqrt(Math.Pow(q.center.R - w.center.R, 2) + Math.Pow(q.center.G - w.center.G, 2) + Math.Pow(q.center.B - w.center.B, 2)) < th)
                        {
                            w.AddRange(q.parts);
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                    {
                        Clust1.Add(q);
                    }
                }
            }
            else
            {
                Clust1 = Clust.ToList();
            }
            LB_UC.Invoke(new Action(() =>
            {
                LB_UC.Text = "Использовано цветов: " + Clust.Count.ToString();
            }));
        }
        bool ReplaceColors()
        {
            rgbtc.Clear();
            Invoke(new Action(() =>
            {
                this.Text = "Замена цветов";
            }));
            check.Clear();
            for (int i = 0; i < RGBTs.GetUpperBound(0); i++)
            {
                for (int j = 0; j < RGBTs.GetUpperBound(1); j++)
                {
                    foreach (Cluster q in Clust1)
                    {
                        if (q.parts.Where(x => x == RGBTs[i, j]).Count() != 0)
                        {
                            RGBTP color = colors.GetSC(q.center);
                            RGBTs[i, j] = new RGBT(color.R, color.G, color.B);
                            check.Add(new RGBT(color.R, color.G, color.B));
                        }
                    }
                }
            }
            check1.Clear();
            foreach (RGBT q in check)
            {
                if (check1.Where(x => x.R == q.R && x.G == q.G && x.B == q.B).Count() == 0)
                    check1.Add(q);
            }
            for (int i = 0; i < RGBTs.GetUpperBound(0); i++)
            {
                for (int j = 0; j < RGBTs.GetUpperBound(1); j++)
                {
                    if (rgbtc.Where(x => x.RGB.R == RGBTs[i, j].R && x.RGB.G == RGBTs[i, j].G && x.RGB.B == RGBTs[i, j].B).Count() == 0)
                        rgbtc.Add(new RGBTC() { Count = 1, RGB = new RGBTP(colors.GetSN(RGBTs[i, j].R, RGBTs[i, j].G, RGBTs[i, j].B), RGBTs[i, j].R, RGBTs[i, j].G, RGBTs[i, j].B) });
                    else
                        rgbtc.Where(x => x.RGB.R == RGBTs[i, j].R && x.RGB.G == RGBTs[i, j].G && x.RGB.B == RGBTs[i, j].B).First().Count++;
                }
            }
            LB_UC.Invoke(new Action(() =>
            {
                LB_UC.Text = "Использовано цветов: " + check1.Count.ToString();
            }));
            if (check1.Count > zna4s.Count)
            {
                MessageBox.Show("Нехватает значков. Надо увеличить порог");
                return false;
            }
            else
                return true;
        }
        void PReplaceGen()
        {
            int t = 0;
            UC.Clear();
            foreach(RGBT q in check1)
            {
                UC.Add(new RGBTP(colors.GetSN(q.R, q.G, q.B) , q.R, q.G, q.B, zna4s[t]));
                t++;
            }
        }
        void PCreate()
        {
            Invoke(new Action(() =>
            {
                this.Text = "Создание превью";
            }));
            //LockBitmap lb = new LockBitmap(template);
            //lb.LockBits();
            for (int i = 0; i < RGBTs.GetUpperBound(0); i++)
            {
                for (int j = 0; j < RGBTs.GetUpperBound(1); j++)
                {
                    for (int w = i * (PSize + SSize); w < (i * (PSize + SSize)) + (PSize + SSize * 2); w++)
                    {
                        for (int h = j * (PSize + SSize); h < (j * (PSize + SSize)) + (PSize + SSize * 2); h++)
                        {
                            if ((w > (i * (PSize + SSize)) + 1) && (w < ((i * (PSize + SSize)) + (PSize + SSize * 2)) - 2) && (h > (j * (PSize + SSize)) + 1) && (h < ((j * (PSize + SSize)) + (PSize + SSize * 2)) - 2))
                                template.SetPixel(w + (template.Width - RGBTs.GetUpperBound(0) * 19 + 2) / 2, h + (template.Height - RGBTs.GetUpperBound(1) * 19 + 2) / 2, Color.FromArgb(255, RGBTs[i, j].R, RGBTs[i, j].G, RGBTs[i, j].B));
                            else
                                template.SetPixel(w + (template.Width - RGBTs.GetUpperBound(0) * 19 + 2) / 2, h + (template.Height - RGBTs.GetUpperBound(1) * 19 + 2) / 2, Color.FromArgb(255, 0, 0, 0));
                        }
                    }
                }
            }
            //lb.UnlockBits();
            template.SetResolution(DPI, DPI);
        }
        void Create()
        {
            Invoke(new Action(() =>
            {
                this.Text = "Создание полотна";
            }));
            LockBitmap lb = new LockBitmap(ztemplate);
            lb.LockBits();
            if (radioButton1.Checked)
            {
                for (int i = 0; i < RGBTs.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < RGBTs.GetUpperBound(1); j++)
                    {
                        zna4 ww = UC.Where(x => x.R == RGBTs[i, j].R && x.G == RGBTs[i, j].G && x.B == RGBTs[i, j].B).First().Z;
                        int pixx = 0;
                        int pixy = 0;
                        for (int w = i * (PSize + SSize); w < (i * (PSize + SSize)) + (PSize + SSize * 2); w++)
                        {
                            for (int h = j * (PSize + SSize); h < (j * (PSize + SSize)) + (PSize + SSize * 2); h++)
                            {
                                if ((w > (i * (PSize + SSize)) + 1) &&
                                    (w < ((i * (PSize + SSize)) + (PSize + SSize * 2)) - 2) &&
                                    (h > (j * (PSize + SSize)) + 1) &&
                                    (h < ((j * (PSize + SSize)) + (PSize + SSize * 2)) - 2))
                                {
                                    int pix = ww.arr[pixx, pixy];
                                    lb.SetPixel(w + (ztemplate.Width - RGBTs.GetUpperBound(0) * 19 + 2) / 2, h + (ztemplate.Height - RGBTs.GetUpperBound(1) * 19 + 2) / 2, Color.FromArgb(255, pix, pix, pix));
                                    pixy++;
                                }
                                else
                                    lb.SetPixel(w + (ztemplate.Width - RGBTs.GetUpperBound(0) * 19 + 2) / 2, h + (ztemplate.Height - RGBTs.GetUpperBound(1) * 19 + 2) / 2, Color.FromArgb(255, 0, 0, 0));
                            }
                            if (w > (i * (PSize + SSize)) + 1)
                                pixx++;
                            pixy = 0;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < RGBTs.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < RGBTs.GetUpperBound(1); j++)
                    {
                        zna4 ww = UC.Where(x => x.R == RGBTs[i, j].R && x.G == RGBTs[i, j].G && x.B == RGBTs[i, j].B).First().Z;
                        int pixx = 0;
                        int pixy = 16;
                        for (int w = i * (PSize + SSize); w < (i * (PSize + SSize)) + (PSize + SSize * 2); w++)
                        {
                            for (int h = j * (PSize + SSize); h < (j * (PSize + SSize)) + (PSize + SSize * 2); h++)
                            {
                                if ((w > (i * (PSize + SSize)) + 1) &&
                                    (w < ((i * (PSize + SSize)) + (PSize + SSize * 2)) - 2) &&
                                    (h > (j * (PSize + SSize)) + 1) &&
                                    (h < ((j * (PSize + SSize)) + (PSize + SSize * 2)) - 2))
                                {
                                    int pix = ww.arr[pixy, pixx];
                                    lb.SetPixel(w + (ztemplate.Width - RGBTs.GetUpperBound(0) * 19 + 2) / 2, h + (ztemplate.Height - RGBTs.GetUpperBound(1) * 19 + 2) / 2, Color.FromArgb(255, pix, pix, pix));
                                    pixy--;
                                }
                                else
                                    lb.SetPixel(w + (ztemplate.Width - RGBTs.GetUpperBound(0) * 19 + 2) / 2, h + (ztemplate.Height - RGBTs.GetUpperBound(1) * 19 + 2) / 2, Color.FromArgb(255, 0, 0, 0));
                            }
                            if (w > (i * (PSize + SSize)) + 1)
                                pixx++;
                            pixy = 16;
                        }
                    }
                }
            }
            lb.UnlockBits();
            ztemplate.SetResolution(DPI, DPI);
            Invoke(new Action(() =>
            {
                this.Text = "Готово! Использовано цветов: " + check1.Count.ToString();
            }));
        }
        void CreateInfo()
        {
            tempInfo = new Bitmap(1654, 2339);
            using (Graphics g = Graphics.FromImage(tempInfo))
            {
                g.FillRectangle(Brushes.White, 0, 0, 1654, 2339);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            }
            int x = 50;
            int y = 50;
            int index = 0;
            UC = UC.OrderBy(c => c.SN).ToList();
            int counter = 0;
            foreach(RGBTP q in UC)
            {
                if (rgbtc.Where(c => c.RGB.SN == q.SN).First().Count >= 10)
                    DrawInfoString(x, y, ++index, q.Z.arr, q.SN, rgbtc.Where(c => c.RGB.SN == q.SN).First().Count);
                else
                {
                    DrawInfoStringl(x, y, ++index, q.Z.arr, q.SN, rgbtc.Where(c => c.RGB.SN == q.SN).First().Count);
                    counter++;
                }
                y += 25;
                if(y > 2339 - 50)
                {
                    x += 300;
                    y = 50;
                }
            }
            Invoke(new Action(() =>
            {
                this.Text += " (" + counter.ToString() + ")";
            }));
            //for(int i = 0;i<UC.Count;i++)
            //{
            //    Rectangle r = new Rectangle(100, i * 25 + 100, 500, 20);
            //    g.DrawString(UC[i].SN.ToString(), new Font("microsoft sans serif", 15), Brushes.Black, r);
            //    int posx = 0;
            //    int posy = 0;
            //    for (int w = 70; w < 87; w++, posx++)
            //    {
            //        for (int h = i * 25; h < i * 25 + 17; h++, posy++)
            //        {
            //            tempInfo.SetPixel(w, h + 103, Color.FromArgb(255, UC[i].Z.arr[posx, posy], UC[i].Z.arr[posx, posy], UC[i].Z.arr[posx, posy]));
            //        }
            //        posy = 0;
            //    }
            //}
        }
        void DrawInfoString(int x, int y, int index, int[,] pic, int sn, int count)
        {
            using (Graphics g = Graphics.FromImage(tempInfo))
            {

                g.DrawString(index.ToString() + ":", new Font("microsoft sans serif", 15), Brushes.Black, new Rectangle(x, y, 50, 20));
                int posx = 0;
                int posy = 0;
                for (int w = x + 50; w < x + 67; w++, posx++)
                {
                    for (int h = y + 4; h < y + 21; h++, posy++)
                    {
                        tempInfo.SetPixel(w, h, Color.FromArgb(255, pic[posx, posy], pic[posx, posy], pic[posx, posy]));
                    }
                    posy = 0;
                }
                for (int w = x + 49; w < x + 68; w++)
                {
                    for (int h = y + 3; h < y + 22; h++)
                    {
                        if (w == x + 49 || w == x + 67 || h == y + 3 || h == y + 21)
                        {
                            tempInfo.SetPixel(w, h, Color.FromArgb(255, 0, 0, 0));
                        }
                    }
                }
                g.DrawString(sn.ToString() + " - " + count.ToString(), new Font("microsoft sans serif", 15), Brushes.Black, new Rectangle(x + 100, y, 150, 20));
            }
        }
        void DrawInfoStringl(int x, int y, int index, int[,] pic, int sn, int count)
        {
            using (Graphics g = Graphics.FromImage(tempInfo))
            {

                g.DrawString(index.ToString() + "::", new Font("microsoft sans serif", 15), Brushes.Black, new Rectangle(x, y, 50, 20));
                int posx = 0;
                int posy = 0;
                for (int w = x + 50; w < x + 67; w++, posx++)
                {
                    for (int h = y + 4; h < y + 21; h++, posy++)
                    {
                        tempInfo.SetPixel(w, h, Color.FromArgb(255, pic[posx, posy], pic[posx, posy], pic[posx, posy]));
                    }
                    posy = 0;
                }
                for (int w = x + 49; w < x + 68; w++)
                {
                    for (int h = y + 3; h < y + 22; h++)
                    {
                        if (w == x + 49 || w == x + 67 || h == y + 3 || h == y + 21)
                        {
                            tempInfo.SetPixel(w, h, Color.FromArgb(255, 0, 0, 0));
                        }
                    }
                }
                g.DrawString(sn.ToString() + " - " + count.ToString(), new Font("microsoft sans serif", 15), Brushes.Black, new Rectangle(x + 100, y, 150, 20));
            }
        }
        void LoadZna4()
        {

            zna4s.Add(new zna4(new Bitmap("icons/0.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/1.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/2.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/3.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/4.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/5.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/6.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/7.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/8.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/9.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/14.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/15.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/16.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/17.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/18.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/19.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/65.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/66.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/67.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/68.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/69.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/70.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/71.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/72.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/73.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/74.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/75.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/76.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/77.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/78.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/79.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/80.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/81.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/82.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/83.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/84.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/85.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/86.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/87.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/88.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/89.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/90.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/91.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/92.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/93.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/94.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/95.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/96.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/128.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/129.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/138.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/139.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/142.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/144.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/154.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/156.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/158.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/159.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/165.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/166.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/167.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/168.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/169.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/170.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/171.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/172.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/173.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/190.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/10.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/11.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/12.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/13.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/20.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/21.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/22.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/23.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/24.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/25.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/26.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/27.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/28.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/29.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/30.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/31.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/32.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/33.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/34.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/35.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/36.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/37.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/38.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/39.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/40.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/41.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/42.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/43.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/44.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/45.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/46.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/47.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/48.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/49.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/50.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/51.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/52.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/53.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/54.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/55.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/56.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/57.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/58.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/59.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/60.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/61.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/62.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/63.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/64.bmp")));
            
            zna4s.Add(new zna4(new Bitmap("icons/97.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/98.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/99.bmp")));

            zna4s.Add(new zna4(new Bitmap("icons/100.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/101.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/102.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/103.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/104.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/105.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/106.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/107.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/108.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/109.bmp")));

            zna4s.Add(new zna4(new Bitmap("icons/110.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/111.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/112.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/113.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/114.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/115.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/116.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/117.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/118.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/119.bmp")));

            zna4s.Add(new zna4(new Bitmap("icons/120.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/121.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/122.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/123.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/124.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/125.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/126.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/127.bmp")));

            zna4s.Add(new zna4(new Bitmap("icons/130.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/131.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/132.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/133.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/134.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/135.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/136.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/137.bmp")));

            zna4s.Add(new zna4(new Bitmap("icons/140.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/141.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/143.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/145.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/146.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/147.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/148.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/149.bmp")));

            zna4s.Add(new zna4(new Bitmap("icons/150.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/151.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/152.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/153.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/155.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/157.bmp")));

            zna4s.Add(new zna4(new Bitmap("icons/160.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/161.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/162.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/163.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/164.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/174.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/175.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/176.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/177.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/178.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/179.bmp")));

            zna4s.Add(new zna4(new Bitmap("icons/180.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/181.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/182.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/183.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/184.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/185.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/186.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/187.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/188.bmp")));
            zna4s.Add(new zna4(new Bitmap("icons/189.bmp")));


            //List<string> files = Directory.GetFiles("icons/").ToList();
            //foreach (string q in files)
            //{
            //    if (q.IndexOf("bmp") == -1) continue;
            //    zna4s.Add(new zna4(new Bitmap(q)));
            //}
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (ofd.FileName == "") return;
            sbmp = new Bitmap(ofd.FileName);
            PBMain.Image = sbmp;

            szbmp = null;
            template = null;
            CB_Formats.SelectedIndex = -1;
            LB_UC.Text = "";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TB_DPI.Text = DPI.ToString();
            //colors.qwe();
            LoadZna4();
            AFormatsInit();
            foreach (AFormat q in AFList)
                CB_Formats.Items.Add(q.name);
            FFormatInit();
            foreach (FFormat q in FFList)
                CB_FS.Items.Add(q.name);
            CB_FS.SelectedIndex = 0;
            CB_ClusterType.SelectedIndex = 0;
        }
        private void CB_Formats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CB_Formats.SelectedIndex == -1) return;
            panel1.Enabled = false;
            TemplateCreate();
            panel1.Enabled = true;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            (sbmp as Image).RotateFlip(RotateFlipType.Rotate270FlipNone);
            PBMain.Image = sbmp;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            (sbmp as Image).RotateFlip(RotateFlipType.Rotate90FlipNone);
            PBMain.Image = sbmp;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if(sbmp == null)
            {
                MessageBox.Show("Загрузите изображение");
                return;
            }
            if(template == null)
            {
                MessageBox.Show("Выберите размер бумаги при печати");
                return;
            }
            panel1.Enabled = false;
            TemplateCreate();
            int err = Resize1();
            if (err == 2)
            {
                panel1.Enabled = true;
                return;
            }
            Thread th = new Thread(() =>
            {
                Pixelize();
                CollectColors();
                Clusterize();
                if (ReplaceColors())
                {
                    PReplaceGen();
                    PCreate();
                    Create();
                    CreateInfo();
                }
                panel1.Invoke(new Action(() =>
                {
                    panel1.Enabled = true;
                }));
                B_Save.Invoke(new Action(() =>
                {
                    B_Save.PerformClick();
                }));
            });
            th.Start();
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            LabelSize.Text = "Масштаб: " + trackBar1.Value.ToString() + "%";
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            THLabel.Text = "Порог: " + trackBar2.Value.ToString() + "%";
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            if(template == null || ztemplate == null || tempInfo == null)
            {
                MessageBox.Show("Ничего еще не создано");
                return;
            }
            Fout f = new Fout(template, ztemplate, tempInfo);
            f.Show();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            DPI = Convert.ToInt32(TB_DPI.Text);
            AFormatsInit();
            CB_Formats.SelectedIndex = -1;
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void CB_ClusterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            clustertype = CB_ClusterType.SelectedIndex;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            template = new Bitmap(AFList[CB_Formats.SelectedIndex].pxw, AFList[CB_Formats.SelectedIndex].pxh);
            using (Graphics grp = Graphics.FromImage(template))
            {
                grp.FillRectangle(Brushes.White, 0, 0, AFList[CB_Formats.SelectedIndex].pxw, AFList[CB_Formats.SelectedIndex].pxh);
            }
            int wi = 50;
            int he = 50;
            LockBitmap l = new LockBitmap(template);
            l.LockBits();
            for (int i = 0; i < zna4s.Count; i++)
            {
                for (int w = wi, px = 0; w < wi + 21; w++, px++)
                {
                    for (int h = he, py = 0; h < he + 21; h++, py++)
                    {
                        if ((px > 1 && py > 1) && (px < 19 && py < 19))
                            l.SetPixel(w, h, Color.FromArgb(zna4s[i].arr[px - 2, py - 2], zna4s[i].arr[px - 2, py - 2], zna4s[i].arr[px - 2, py - 2]));
                        else
                            l.SetPixel(w, h, Color.FromArgb(0, 0, 0));
                    }
                }
                wi += 25;
                if(wi > 25 * 20 + 25)
                {
                    wi = 50;
                    he += 25;
                }
            }
            l.UnlockBits();
            template.Save("C:/template/temp.bmp");
        }
    }
    public class Cluster
    {
        public List<RGBT> parts = new List<RGBT>();
        public RGBT center { get; set; }
        void CenterCalc()
        {
            int AR = 0;
            int AG = 0;
            int AB = 0;
            foreach (RGBT q in parts)
            {
                AR += q.R;
                AG += q.G;
                AB += q.B;
            }
            AR /= parts.Count;
            AG /= parts.Count;
            AB /= parts.Count;
            center = new RGBT(AR, AG, AB);
        }
        public Cluster(RGBT part)
        {
            parts.Add(part);
            CenterCalc();
        }
        public void Add(RGBT part)
        {
            parts.Add(part);
            CenterCalc();
        }
        public void AddRange(List<RGBT> part)
        {
            parts.AddRange(part);
            CenterCalc();
        }
    }
    public class Benchmark
    {
        private static DateTime startDate = DateTime.MinValue;
        private static DateTime endDate = DateTime.MinValue;

        public static TimeSpan Span { get { return endDate.Subtract(startDate); } }

        public static void Start() { startDate = DateTime.Now; }

        public static void End() { endDate = DateTime.Now; }

        public static double GetSeconds()
        {
            if (endDate == DateTime.MinValue) return 0.0;
            else return Span.TotalSeconds;
        }
    }
    public class LockBitmap
    {
        Bitmap source = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this.source = source;
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            try
            {
                // Get width and height of bitmap
                Width = source.Width;
                Height = source.Height;

                // get total locked pixels count
                int PixelCount = Width * Height;

                // Create rectangle to lock
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                // get source bitmap pixel format size
                Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

                // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                if (Depth != 8 && Depth != 24 && Depth != 32)
                {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                }

                // Lock bitmap and return bitmap data
                bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                             source.PixelFormat);

                // create byte array to copy pixel values
                int step = Depth / 8;
                Pixels = new byte[PixelCount * step];
                Iptr = bitmapData.Scan0;

                // Copy data from pointer to array
                Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Unlock bitmap data
        /// </summary>
        public void UnlockBits()
        {
            try
            {
                // Copy data from byte array to pointer
                Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                // Unlock bitmap data
                source.UnlockBits(bitmapData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }
    }
    public class Colors
    {
        public List<RGBTP> CList = new List<RGBTP>();
        public Colors()
        {
            CList.Add(new RGBTP(150, 172, 2, 73));
            CList.Add(new RGBTP(151, 243, 200, 210));
            CList.Add(new RGBTP(152, 230, 159, 149));
            CList.Add(new RGBTP(153, 231, 205, 218));
            CList.Add(new RGBTP(154, 88, 35, 52));
            CList.Add(new RGBTP(155, 152, 144, 183));
            CList.Add(new RGBTP(156, 170, 181, 213));
            CList.Add(new RGBTP(157, 189, 200, 230));
            CList.Add(new RGBTP(158, 66, 62, 112));
            CList.Add(new RGBTP(159, 199, 205, 221));
            CList.Add(new RGBTP(160, 135, 150, 181));
            CList.Add(new RGBTP(161, 93, 104, 150));
            CList.Add(new RGBTP(162, 206, 228, 241));
            CList.Add(new RGBTP(163, 88, 157, 113));
            CList.Add(new RGBTP(164, 187, 214, 169));
            CList.Add(new RGBTP(165, 240, 245, 165));
            CList.Add(new RGBTP(166, 210, 214, 42));
            CList.Add(new RGBTP(167, 176, 145, 63));
            CList.Add(new RGBTP(168, 201, 209, 211));
            CList.Add(new RGBTP(169, 130, 139, 138));
            CList.Add(new RGBTP(208, 131, 91, 141));
            CList.Add(new RGBTP(209, 164, 123, 167));
            CList.Add(new RGBTP(210, 195, 159, 195));
            CList.Add(new RGBTP(211, 227, 203, 227));
            CList.Add(new RGBTP(221, 157, 62, 68));
            CList.Add(new RGBTP(223, 216, 138, 134));
            CList.Add(new RGBTP(224, 234, 184, 175));
            CList.Add(new RGBTP(225, 254, 222, 217));
            CList.Add(new RGBTP(300, 111, 47, 1));
            CList.Add(new RGBTP(301, 177, 95, 43));
            CList.Add(new RGBTP(304, 183, 32, 51));
            CList.Add(new RGBTP(307, 254, 236, 68));
            CList.Add(new RGBTP(309, 203, 55, 79));
            CList.Add(new RGBTP(310, 0, 0, 0));
            CList.Add(new RGBTP(311, 7, 65, 87));
            CList.Add(new RGBTP(312, 48, 91, 126));
            CList.Add(new RGBTP(315, 147, 63, 79));
            CList.Add(new RGBTP(316, 183, 115, 128));
            CList.Add(new RGBTP(317, 90, 95, 99));
            CList.Add(new RGBTP(318, 159, 159, 167));
            CList.Add(new RGBTP(319, 36, 107, 51));
            CList.Add(new RGBTP(320, 119, 189, 117));
            CList.Add(new RGBTP(321, 199, 42, 59));
            CList.Add(new RGBTP(322, 72, 115, 150));
            CList.Add(new RGBTP(326, 180, 59, 76));
            CList.Add(new RGBTP(327, 88, 19, 73));
            CList.Add(new RGBTP(333, 98, 81, 136));
            CList.Add(new RGBTP(334, 99, 143, 170));
            CList.Add(new RGBTP(335, 216, 79, 99));
            CList.Add(new RGBTP(336, 15, 51, 83));
            CList.Add(new RGBTP(340, 168, 169, 200));
            CList.Add(new RGBTP(341, 190, 195, 224));
            CList.Add(new RGBTP(347, 196, 39, 68));
            CList.Add(new RGBTP(349, 203, 47, 51));
            CList.Add(new RGBTP(350, 215, 76, 73));
            CList.Add(new RGBTP(351, 223, 104, 100));
            CList.Add(new RGBTP(352, 236, 135, 127));
            CList.Add(new RGBTP(353, 242, 175, 159));
            CList.Add(new RGBTP(355, 169, 73, 61));
            CList.Add(new RGBTP(356, 212, 123, 105));
            CList.Add(new RGBTP(367, 77, 149, 76));
            CList.Add(new RGBTP(368, 175, 218, 164));
            CList.Add(new RGBTP(369, 214, 238, 204));
            CList.Add(new RGBTP(370, 176, 157, 88));
            CList.Add(new RGBTP(371, 184, 169, 104));
            CList.Add(new RGBTP(372, 195, 189, 129));
            CList.Add(new RGBTP(400, 143, 67, 17));
            CList.Add(new RGBTP(402, 248, 168, 119));
            CList.Add(new RGBTP(407, 188, 140, 117));
            CList.Add(new RGBTP(413, 63, 67, 70));
            CList.Add(new RGBTP(414, 127, 127, 135));
            CList.Add(new RGBTP(415, 196, 195, 200));
            CList.Add(new RGBTP(420, 158, 126, 53));
            CList.Add(new RGBTP(422, 214, 180, 108));
            CList.Add(new RGBTP(433, 123, 82, 26));
            CList.Add(new RGBTP(434, 142, 99, 44));
            CList.Add(new RGBTP(435, 162, 119, 64));
            CList.Add(new RGBTP(436, 183, 142, 86));
            CList.Add(new RGBTP(437, 203, 164, 108));
            CList.Add(new RGBTP(444, 255, 207, 0));
            CList.Add(new RGBTP(445, 255, 252, 139));
            CList.Add(new RGBTP(451, 128, 108, 110));
            CList.Add(new RGBTP(452, 178, 158, 167));
            CList.Add(new RGBTP(453, 212, 200, 212));
            CList.Add(new RGBTP(469, 132, 151, 69));
            CList.Add(new RGBTP(470, 160, 175, 92));
            CList.Add(new RGBTP(471, 188, 199, 120));
            CList.Add(new RGBTP(472, 215, 227, 151));
            CList.Add(new RGBTP(498, 167, 19, 43));
            CList.Add(new RGBTP(500, 6, 86, 57));
            CList.Add(new RGBTP(501, 63, 120, 87));
            CList.Add(new RGBTP(502, 90, 143, 113));
            CList.Add(new RGBTP(503, 142, 184, 164));
            CList.Add(new RGBTP(504, 183, 215, 194));
            CList.Add(new RGBTP(505, 50, 131, 98));
            CList.Add(new RGBTP(517, 47, 114, 140));
            CList.Add(new RGBTP(518, 79, 147, 168));
            CList.Add(new RGBTP(519, 116, 180, 192));
            CList.Add(new RGBTP(520, 68, 98, 44));
            CList.Add(new RGBTP(522, 132, 150, 108));
            CList.Add(new RGBTP(523, 181, 193, 153));
            CList.Add(new RGBTP(524, 196, 206, 172));
            CList.Add(new RGBTP(535, 88, 87, 69));
            CList.Add(new RGBTP(543, 237, 227, 217));
            CList.Add(new RGBTP(550, 76, 19, 62));
            CList.Add(new RGBTP(552, 122, 55, 101));
            CList.Add(new RGBTP(553, 164, 99, 141));
            CList.Add(new RGBTP(554, 219, 179, 203));
            CList.Add(new RGBTP(561, 44, 106, 69));
            CList.Add(new RGBTP(562, 83, 152, 107));
            CList.Add(new RGBTP(563, 144, 212, 171));
            CList.Add(new RGBTP(564, 187, 227, 193));
            CList.Add(new RGBTP(580, 103, 124, 19));
            CList.Add(new RGBTP(581, 141, 163, 56));
            CList.Add(new RGBTP(597, 90, 163, 178));
            CList.Add(new RGBTP(598, 135, 195, 206));
            CList.Add(new RGBTP(600, 215, 11, 100));
            CList.Add(new RGBTP(601, 224, 39, 115));
            CList.Add(new RGBTP(602, 231, 71, 131));
            CList.Add(new RGBTP(603, 239, 103, 151));
            CList.Add(new RGBTP(604, 247, 139, 175));
            CList.Add(new RGBTP(605, 255, 180, 203));
            CList.Add(new RGBTP(606, 254, 58, 18));
            CList.Add(new RGBTP(608, 252, 93, 53));
            CList.Add(new RGBTP(610, 113, 105, 66));
            CList.Add(new RGBTP(611, 135, 125, 90));
            CList.Add(new RGBTP(612, 165, 153, 115));
            CList.Add(new RGBTP(613, 216, 202, 173));
            CList.Add(new RGBTP(632, 151, 100, 73));
            CList.Add(new RGBTP(640, 149, 134, 101));
            CList.Add(new RGBTP(642, 186, 178, 150));
            CList.Add(new RGBTP(644, 223, 215, 194));
            CList.Add(new RGBTP(645, 84, 87, 66));
            CList.Add(new RGBTP(646, 114, 123, 102));
            CList.Add(new RGBTP(647, 151, 154, 133));
            CList.Add(new RGBTP(648, 191, 194, 173));
            CList.Add(new RGBTP(666, 222, 0, 41));
            CList.Add(new RGBTP(676, 240, 204, 118));
            CList.Add(new RGBTP(677, 246, 233, 189));
            CList.Add(new RGBTP(680, 205, 165, 41));
            CList.Add(new RGBTP(699, 0, 91, 36));
            CList.Add(new RGBTP(700, 8, 115, 26));
            CList.Add(new RGBTP(701, 28, 138, 23));
            CList.Add(new RGBTP(702, 71, 166, 46));
            CList.Add(new RGBTP(703, 123, 181, 71));
            CList.Add(new RGBTP(704, 176, 219, 103));
            CList.Add(new RGBTP(712, 255, 251, 239));
            CList.Add(new RGBTP(718, 187, 52, 119));
            CList.Add(new RGBTP(720, 239, 112, 71));
            CList.Add(new RGBTP(721, 244, 131, 87));
            CList.Add(new RGBTP(722, 248, 150, 111));
            CList.Add(new RGBTP(725, 247, 207, 73));
            CList.Add(new RGBTP(726, 245, 225, 109));
            CList.Add(new RGBTP(727, 247, 235, 151));
            CList.Add(new RGBTP(728, 239, 194, 31));
            CList.Add(new RGBTP(729, 227, 184, 72));
            CList.Add(new RGBTP(730, 114, 111, 34));
            CList.Add(new RGBTP(731, 155, 150, 58));
            CList.Add(new RGBTP(732, 155, 150, 58));
            CList.Add(new RGBTP(733, 195, 189, 101));
            CList.Add(new RGBTP(734, 215, 209, 121));
            CList.Add(new RGBTP(738, 224, 187, 143));
            CList.Add(new RGBTP(739, 244, 214, 176));
            CList.Add(new RGBTP(740, 255, 139, 0));
            CList.Add(new RGBTP(741, 255, 163, 44));
            CList.Add(new RGBTP(742, 255, 192, 87));
            CList.Add(new RGBTP(743, 255, 224, 108));
            CList.Add(new RGBTP(744, 255, 231, 147));
            CList.Add(new RGBTP(745, 254, 242, 187));
            CList.Add(new RGBTP(746, 255, 255, 235));
            CList.Add(new RGBTP(747, 220, 255, 251));
            CList.Add(new RGBTP(754, 247, 203, 192));
            CList.Add(new RGBTP(758, 238, 171, 155));
            CList.Add(new RGBTP(760, 232, 139, 149));
            CList.Add(new RGBTP(761, 244, 180, 181));
            CList.Add(new RGBTP(762, 232, 231, 236));
            CList.Add(new RGBTP(772, 231, 254, 184));
            CList.Add(new RGBTP(775, 221, 238, 245));
            CList.Add(new RGBTP(776, 245, 155, 165));
            CList.Add(new RGBTP(777, 132, 12, 40));
            CList.Add(new RGBTP(778, 222, 179, 188));
            CList.Add(new RGBTP(779, 99, 76, 70));
            CList.Add(new RGBTP(780, 180, 97, 1));
            CList.Add(new RGBTP(781, 202, 132, 0));
            CList.Add(new RGBTP(782, 202, 132, 0));
            CList.Add(new RGBTP(783, 222, 155, 4));
            CList.Add(new RGBTP(791, 56, 54, 101));
            CList.Add(new RGBTP(792, 77, 74, 127));
            CList.Add(new RGBTP(793, 122, 135, 177));
            CList.Add(new RGBTP(794, 147, 161, 200));
            CList.Add(new RGBTP(796, 16, 32, 119));
            CList.Add(new RGBTP(797, 36, 63, 140));
            CList.Add(new RGBTP(798, 63, 96, 163));
            CList.Add(new RGBTP(799, 96, 132, 184));
            CList.Add(new RGBTP(800, 179, 207, 228));
            CList.Add(new RGBTP(801, 104, 64, 15));
            CList.Add(new RGBTP(803, 32, 72, 108));
            CList.Add(new RGBTP(806, 62, 133, 161));
            CList.Add(new RGBTP(807, 99, 159, 167));
            CList.Add(new RGBTP(809, 135, 171, 203));
            CList.Add(new RGBTP(813, 131, 175, 202));
            CList.Add(new RGBTP(814, 124, 0, 28));
            CList.Add(new RGBTP(815, 135, 7, 30));
            CList.Add(new RGBTP(816, 150, 10, 36));
            CList.Add(new RGBTP(817, 195, 28, 36));
            CList.Add(new RGBTP(818, 255, 215, 215));
            CList.Add(new RGBTP(819, 255, 235, 236));
            CList.Add(new RGBTP(820, 1, 8, 99));
            CList.Add(new RGBTP(822, 246, 240, 228));
            CList.Add(new RGBTP(823, 7, 31, 59));
            CList.Add(new RGBTP(824, 35, 67, 116));
            CList.Add(new RGBTP(825, 58, 99, 143));
            CList.Add(new RGBTP(826, 92, 136, 175));
            CList.Add(new RGBTP(827, 180, 215, 235));
            CList.Add(new RGBTP(828, 192, 226, 236));
            CList.Add(new RGBTP(829, 106, 81, 0));
            CList.Add(new RGBTP(830, 121, 102, 0));
            CList.Add(new RGBTP(831, 149, 131, 49));
            CList.Add(new RGBTP(832, 170, 150, 53));
            CList.Add(new RGBTP(833, 206, 188, 102));
            CList.Add(new RGBTP(834, 215, 203, 131));
            CList.Add(new RGBTP(838, 85, 54, 23));
            CList.Add(new RGBTP(839, 107, 73, 38));
            CList.Add(new RGBTP(840, 153, 123, 85));
            CList.Add(new RGBTP(841, 181, 157, 129));
            CList.Add(new RGBTP(842, 219, 199, 174));
            CList.Add(new RGBTP(844, 49, 57, 46));
            CList.Add(new RGBTP(869, 118, 97, 40));
            CList.Add(new RGBTP(890, 20, 76, 37));
            CList.Add(new RGBTP(891, 243, 50, 67));
            CList.Add(new RGBTP(892, 247, 88, 120));
            CList.Add(new RGBTP(893, 252, 123, 151));
            CList.Add(new RGBTP(894, 255, 163, 184));
            CList.Add(new RGBTP(895, 28, 83, 0));
            CList.Add(new RGBTP(898, 82, 47, 7));
            CList.Add(new RGBTP(899, 228, 111, 129));
            CList.Add(new RGBTP(900, 223, 58, 0));
            CList.Add(new RGBTP(902, 111, 27, 43));
            CList.Add(new RGBTP(904, 47, 106, 0));
            CList.Add(new RGBTP(905, 86, 146, 48));
            CList.Add(new RGBTP(906, 138, 199, 62));
            CList.Add(new RGBTP(907, 199, 230, 101));
            CList.Add(new RGBTP(909, 12, 127, 60));
            CList.Add(new RGBTP(910, 31, 147, 72));
            CList.Add(new RGBTP(911, 55, 167, 91));
            CList.Add(new RGBTP(912, 88, 191, 110));
            CList.Add(new RGBTP(913, 124, 211, 140));
            CList.Add(new RGBTP(915, 147, 0, 80));
            CList.Add(new RGBTP(917, 168, 22, 93));
            CList.Add(new RGBTP(918, 163, 39, 1));
            CList.Add(new RGBTP(919, 192, 59, 14));
            CList.Add(new RGBTP(920, 210, 83, 32));
            CList.Add(new RGBTP(921, 228, 103, 55));
            CList.Add(new RGBTP(922, 237, 150, 105));
            CList.Add(new RGBTP(924, 23, 98, 90));
            CList.Add(new RGBTP(926, 124, 164, 156));
            CList.Add(new RGBTP(927, 166, 191, 193));
            CList.Add(new RGBTP(928, 200, 216, 216));
            CList.Add(new RGBTP(930, 51, 99, 119));
            CList.Add(new RGBTP(931, 72, 116, 143));
            CList.Add(new RGBTP(932, 133, 165, 190));
            CList.Add(new RGBTP(934, 51, 71, 20));
            CList.Add(new RGBTP(935, 60, 79, 15));
            CList.Add(new RGBTP(936, 84, 102, 26));
            CList.Add(new RGBTP(937, 107, 127, 45));
            CList.Add(new RGBTP(938, 64, 35, 1));
            CList.Add(new RGBTP(939, 1, 19, 39));
            CList.Add(new RGBTP(943, 56, 173, 123));
            CList.Add(new RGBTP(945, 255, 207, 184));
            CList.Add(new RGBTP(946, 227, 75, 0));
            CList.Add(new RGBTP(947, 235, 100, 8));
            CList.Add(new RGBTP(948, 253, 226, 205));
            CList.Add(new RGBTP(950, 238, 205, 186));
            CList.Add(new RGBTP(951, 255, 227, 206));
            CList.Add(new RGBTP(954, 160, 231, 173));
            CList.Add(new RGBTP(955, 205, 253, 208));
            CList.Add(new RGBTP(956, 252, 98, 134));
            CList.Add(new RGBTP(957, 255, 163, 190));
            CList.Add(new RGBTP(958, 96, 198, 161));
            CList.Add(new RGBTP(959, 139, 215, 185));
            CList.Add(new RGBTP(961, 220, 104, 127));
            CList.Add(new RGBTP(962, 223, 135, 147));
            CList.Add(new RGBTP(963, 255, 215, 223));
            CList.Add(new RGBTP(964, 187, 244, 229));
            CList.Add(new RGBTP(966, 167, 214, 178));
            CList.Add(new RGBTP(967, 255, 208, 190));
            CList.Add(new RGBTP(970, 247, 138, 19));
            CList.Add(new RGBTP(971, 255, 139, 0));
            CList.Add(new RGBTP(972, 246, 194, 23));
            CList.Add(new RGBTP(973, 255, 228, 1));
            CList.Add(new RGBTP(975, 145, 79, 18));
            CList.Add(new RGBTP(976, 204, 133, 55));
            CList.Add(new RGBTP(977, 220, 155, 87));
            CList.Add(new RGBTP(986, 44, 105, 33));
            CList.Add(new RGBTP(987, 66, 132, 42));
            CList.Add(new RGBTP(988, 107, 164, 70));
            CList.Add(new RGBTP(989, 148, 193, 108));
            CList.Add(new RGBTP(991, 12, 111, 80));
            CList.Add(new RGBTP(992, 75, 175, 139));
            CList.Add(new RGBTP(993, 131, 207, 179));
            CList.Add(new RGBTP(995, 2, 111, 168));
            CList.Add(new RGBTP(996, 19, 191, 227));
            CList.Add(new RGBTP(3011, 112, 122, 59));
            CList.Add(new RGBTP(3012, 150, 155, 89));
            CList.Add(new RGBTP(3013, 186, 185, 130));
            CList.Add(new RGBTP(3021, 70, 70, 44));
            CList.Add(new RGBTP(3022, 136, 134, 95));
            CList.Add(new RGBTP(3023, 178, 175, 142));
            CList.Add(new RGBTP(3024, 219, 218, 200));
            CList.Add(new RGBTP(3031, 73, 57, 41));
            CList.Add(new RGBTP(3032, 176, 160, 124));
            CList.Add(new RGBTP(3033, 227, 215, 199));
            CList.Add(new RGBTP(3041, 152, 119, 132));
            CList.Add(new RGBTP(3042, 180, 157, 163));
            CList.Add(new RGBTP(3045, 199, 176, 106));
            CList.Add(new RGBTP(3046, 220, 208, 168));
            CList.Add(new RGBTP(3047, 233, 229, 191));
            CList.Add(new RGBTP(3051, 83, 99, 52));
            CList.Add(new RGBTP(3052, 124, 138, 85));
            CList.Add(new RGBTP(3053, 167, 183, 136));
            CList.Add(new RGBTP(3064, 205, 150, 120));
            CList.Add(new RGBTP(3072, 231, 234, 223));
            CList.Add(new RGBTP(3078, 250, 248, 191));
            CList.Add(new RGBTP(3325, 171, 199, 220));
            CList.Add(new RGBTP(3326, 245, 155, 165));
            CList.Add(new RGBTP(3328, 207, 68, 87));
            CList.Add(new RGBTP(3340, 249, 136, 92));
            CList.Add(new RGBTP(3341, 255, 157, 125));
            CList.Add(new RGBTP(3345, 47, 107, 19));
            CList.Add(new RGBTP(3346, 77, 139, 56));
            CList.Add(new RGBTP(3347, 119, 167, 83));
            CList.Add(new RGBTP(3348, 208, 236, 151));
            CList.Add(new RGBTP(3350, 188, 67, 102));
            CList.Add(new RGBTP(3354, 239, 168, 184));
            CList.Add(new RGBTP(3362, 75, 102, 51));
            CList.Add(new RGBTP(3363, 113, 143, 89));
            CList.Add(new RGBTP(3364, 159, 180, 121));
            CList.Add(new RGBTP(3371, 60, 41, 37));
            CList.Add(new RGBTP(3607, 211, 87, 147));
            CList.Add(new RGBTP(3608, 230, 131, 175));
            CList.Add(new RGBTP(3609, 254, 179, 212));
            CList.Add(new RGBTP(3685, 135, 20, 49));
            CList.Add(new RGBTP(3688, 233, 150, 176));
            CList.Add(new RGBTP(3687, 194, 89, 109));
            CList.Add(new RGBTP(3689, 253, 194, 212));
            CList.Add(new RGBTP(3705, 248, 87, 92));
            CList.Add(new RGBTP(3706, 255, 135, 147));
            CList.Add(new RGBTP(3708, 255, 171, 184));
            CList.Add(new RGBTP(3712, 219, 103, 114));
            CList.Add(new RGBTP(3713, 254, 222, 223));
            CList.Add(new RGBTP(3716, 247, 175, 187));
            CList.Add(new RGBTP(3721, 175, 75, 83));
            CList.Add(new RGBTP(3722, 194, 103, 102));
            CList.Add(new RGBTP(3726, 167, 87, 98));
            CList.Add(new RGBTP(3727, 202, 144, 156));
            CList.Add(new RGBTP(3731, 218, 103, 132));
            CList.Add(new RGBTP(3733, 232, 135, 155));
            CList.Add(new RGBTP(3740, 120, 87, 98));
            CList.Add(new RGBTP(3743, 215, 204, 212));
            CList.Add(new RGBTP(3746, 118, 107, 151));
            CList.Add(new RGBTP(3747, 212, 214, 235));
            CList.Add(new RGBTP(3750, 29, 63, 73));
            CList.Add(new RGBTP(3752, 158, 182, 206));
            CList.Add(new RGBTP(3753, 211, 223, 235));
            CList.Add(new RGBTP(3755, 136, 167, 195));
            CList.Add(new RGBTP(3756, 239, 251, 251));
            CList.Add(new RGBTP(3760, 62, 133, 161));
            CList.Add(new RGBTP(3761, 164, 214, 223));
            CList.Add(new RGBTP(3765, 23, 99, 112));
            CList.Add(new RGBTP(3766, 159, 207, 207));
            CList.Add(new RGBTP(3768, 84, 138, 126));
            CList.Add(new RGBTP(3770, 255, 247, 236));
            CList.Add(new RGBTP(3771, 244, 187, 170));
            CList.Add(new RGBTP(3772, 172, 126, 93));
            CList.Add(new RGBTP(3773, 182, 151, 145));
            CList.Add(new RGBTP(3774, 243, 225, 215));
            CList.Add(new RGBTP(3776, 206, 121, 57));
            CList.Add(new RGBTP(3777, 143, 52, 49));
            CList.Add(new RGBTP(3778, 221, 148, 133));
            CList.Add(new RGBTP(3779, 241, 196, 193));
            CList.Add(new RGBTP(3781, 89, 79, 44));
            CList.Add(new RGBTP(3782, 206, 191, 168));
            CList.Add(new RGBTP(3787, 103, 98, 66));
            CList.Add(new RGBTP(3790, 121, 107, 73));
            CList.Add(new RGBTP(3799, 44, 45, 49));
            CList.Add(new RGBTP(3801, 240, 60, 72));
            CList.Add(new RGBTP(3802, 126, 42, 65));
            CList.Add(new RGBTP(3803, 171, 51, 86));
            CList.Add(new RGBTP(3804, 232, 58, 127));
            CList.Add(new RGBTP(3805, 243, 71, 139));
            CList.Add(new RGBTP(3806, 253, 125, 174));
            CList.Add(new RGBTP(3807, 91, 102, 148));
            CList.Add(new RGBTP(3808, 11, 83, 95));
            CList.Add(new RGBTP(3809, 31, 107, 123));
            CList.Add(new RGBTP(3810, 59, 135, 151));
            CList.Add(new RGBTP(3811, 187, 227, 235));
            CList.Add(new RGBTP(3812, 28, 149, 98));
            CList.Add(new RGBTP(3813, 183, 215, 194));
            CList.Add(new RGBTP(3814, 34, 142, 106));
            CList.Add(new RGBTP(3815, 57, 124, 91));
            CList.Add(new RGBTP(3816, 125, 181, 142));
            CList.Add(new RGBTP(3817, 175, 220, 191));
            CList.Add(new RGBTP(3818, 0, 107, 47));
            CList.Add(new RGBTP(3819, 223, 232, 103));
            CList.Add(new RGBTP(3820, 227, 186, 58));
            CList.Add(new RGBTP(3821, 242, 207, 79));
            CList.Add(new RGBTP(3822, 243, 224, 119));
            CList.Add(new RGBTP(3823, 255, 253, 215));
            CList.Add(new RGBTP(3824, 254, 187, 158));
            CList.Add(new RGBTP(3825, 253, 190, 149));
            CList.Add(new RGBTP(3826, 186, 113, 44));
            CList.Add(new RGBTP(3827, 247, 186, 119));
            CList.Add(new RGBTP(3828, 183, 149, 60));
            CList.Add(new RGBTP(3829, 169, 130, 3));
            CList.Add(new RGBTP(3830, 197, 95, 81));
            CList.Add(new RGBTP(3831, 183, 44, 85));
            CList.Add(new RGBTP(3832, 226, 92, 129));
            CList.Add(new RGBTP(3833, 237, 115, 136));
            CList.Add(new RGBTP(3834, 113, 55, 93));
            CList.Add(new RGBTP(3835, 147, 95, 131));
            CList.Add(new RGBTP(3836, 185, 145, 169));
            CList.Add(new RGBTP(3837, 108, 58, 111));
            CList.Add(new RGBTP(3838, 95, 113, 159));
            CList.Add(new RGBTP(3839, 126, 146, 181));
            CList.Add(new RGBTP(3840, 171, 182, 200));
            CList.Add(new RGBTP(3841, 198, 222, 234));
            CList.Add(new RGBTP(3842, 22, 86, 111));
            CList.Add(new RGBTP(3843, 6, 147, 194));
            CList.Add(new RGBTP(3844, 9, 160, 141));
            CList.Add(new RGBTP(3845, 4, 190, 179));
            CList.Add(new RGBTP(3846, 74, 223, 203));
            CList.Add(new RGBTP(3847, 44, 113, 84));
            CList.Add(new RGBTP(3848, 62, 142, 113));
            CList.Add(new RGBTP(3849, 121, 190, 162));
            CList.Add(new RGBTP(3850, 15, 149, 99));
            CList.Add(new RGBTP(3851, 108, 194, 160));
            CList.Add(new RGBTP(3852, 214, 165, 36));
            CList.Add(new RGBTP(3853, 231, 139, 54));
            CList.Add(new RGBTP(3854, 250, 185, 103));
            CList.Add(new RGBTP(3855, 249, 221, 148));
            CList.Add(new RGBTP(3856, 247, 201, 178));
            CList.Add(new RGBTP(3857, 119, 43, 30));
            CList.Add(new RGBTP(3858, 151, 74, 64));
            CList.Add(new RGBTP(3859, 186, 139, 123));
            CList.Add(new RGBTP(3860, 146, 112, 103));
            CList.Add(new RGBTP(3861, 167, 137, 129));
            CList.Add(new RGBTP(3862, 144, 114, 81));
            CList.Add(new RGBTP(3863, 170, 140, 104));
            CList.Add(new RGBTP(3864, 207, 187, 162));
            CList.Add(new RGBTP(3865, 252, 251, 247));
            CList.Add(new RGBTP(3866, 251, 246, 242));
            CList.Add(new RGBTP(5200, 255, 255, 255));
            for (int j = 0; j < CList.Count - 1; j++)
            {
                for (int i = 0; i < CList.Count - 1; i++)
                {
                    if (Math.Sqrt(Math.Pow((double)CList[i].R, 2) + Math.Pow((double)CList[i].G, 2) + Math.Pow((double)CList[i].B, 2)) > Math.Sqrt(Math.Pow((double)CList[i + 1].R, 2) + Math.Pow((double)CList[i + 1].G, 2) + Math.Pow((double)CList[i + 1].B, 2)))
                    {
                        RGBTP temp = CList[i];
                        CList[i] = CList[i + 1];
                        CList[i + 1] = temp;
                    }
                }
            }
        }
        public RGBTP GetSC(int R, int G, int B)
        {
            RGBTP NC = new RGBTP(0, 0, 0, 0);
            double dist = int.MaxValue;
            foreach (RGBTP q in CList)
            {
                double dr = Math.Pow((double)q.R - (double)R, 2);
                double dg = Math.Pow((double)q.G - (double)G, 2);
                double db = Math.Pow((double)q.B - (double)B, 2);
                double temp = Math.Sqrt(dr + dg + db);
                if (dist > temp)
                {
                    dist = temp;
                    NC = q;
                }
            }
            return NC;
        }
        public RGBTP GetSC(RGBT rgbt)
        {
            RGBTP NC = new RGBTP(0, 0, 0, 0);
            double dist = int.MaxValue;
            foreach (RGBTP q in CList)
            {
                double dr = Math.Pow((double)q.R - (double)rgbt.R, 2);
                double dg = Math.Pow((double)q.G - (double)rgbt.G, 2);
                double db = Math.Pow((double)q.B - (double)rgbt.B, 2);
                double temp = Math.Sqrt(dr + dg + db);
                if (dist > temp)
                {
                    dist = temp;
                    NC = q;
                }
            }
            return NC;
        }
        public void qwe()
        {
            List<RGBTP> asd = CList.ToList();
            for(int i = 0;i<asd.Count - 1;i++)
            {
                for (int j = 0; j < asd.Count - 1; j++)
                {
                    if(zxc(asd[j].GetColor(), asd[j + 1].GetColor()))
                    {
                        RGBTP temp = asd[j];
                        asd[j] = asd[j + 1];
                        asd[j + 1] = temp;
                    }
                }
            }
            Bitmap b = new Bitmap(asd.Count, 1);
            for (int i = 0; i < asd.Count; i++)
            {
                b.SetPixel(i, 0, Color.FromArgb(255, asd[i].R, asd[i].G, asd[i].B));
            }
            b.Save("qwe11.bmp", ImageFormat.Bmp);
        }
        bool zxc(Color q, Color w)
        {
            if (q.GetHue() > w.GetHue())
                return true;
            else if (q.GetHue() < w.GetHue())
                return false;
            else
            {
                if (q.GetSaturation() > w.GetSaturation())
                    return true;
                else if (q.GetSaturation() < w.GetSaturation())
                    return false;
                else
                {
                    if (q.GetBrightness() > w.GetBrightness())
                        return true;
                    else
                        return false;
                }
            }
        }
        //public List<RGBTP> GetSN(int R, int G, int B)
        //{
        //    return CList.Where(x => x.R == R && x.G == G && x.B == B).ToList();
        //}
        public int GetSN(int R, int G, int B)
        {
            return CList.Where(x => x.R == R && x.G == G && x.B == B).First().SN;
        }
        public List<RGBTP> GetRGB(int SN)
        {
            return CList.Where(x => x.SN == SN).ToList();
        }
    }
    public class RGBTP
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int SN { get; set; }
        public zna4 Z { get; set; }
        public RGBTP(int sn, int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
            SN = sn;
        }
        public RGBTP(int sn, int r, int g, int b, zna4 z)
        {
            R = r;
            G = g;
            B = b;
            SN = sn;
            Z = z;
        }
        public Color GetColor()
        {
            return Color.FromArgb(255, R, G, B);
        }
    }
    public class RGBT
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
            public RGBT(int r, int g, int b)
            {
                R = r;
                G = g;
                B = b;
            }
            public Color GetColor()
            {
                return Color.FromArgb(255, R, G, B);
            }
        }
    public class zna4
    {
        public int[,] arr = new int[17, 17];
        public zna4(Bitmap bmp)
        {
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    arr[i, j] = bmp.GetPixel(i, j).R;
                }
            }
        }
    }
    public class RGBTC
    {
        public RGBTP RGB { get; set; }
        public int Count { get; set; }
    }
}