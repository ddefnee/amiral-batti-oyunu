using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace AMIRAL_BATTI_OYUNU_
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer Zaman; 
        private int dakika = 0;
        private int saniye = 0;
        private bool[,] butonTiklamaDurumu; 
        private const int tahtaBoyutu = 15;
        private Button[,] butonlar;
        private int toplamYildiz = 0; 
        private int tiklananYildizlar = 0;
        private int birYildiz = 0;
        private int ikiYildiz = 0;
        private int ucYildiz = 0;
        private int enIyiSure = int.MaxValue;
        private List<YildizGrubu> yildizGruplari = new List<YildizGrubu>();

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            TahtayiHazirla();
            Zaman = new System.Windows.Forms.Timer();
            Zaman.Interval = 1000;
            Zaman.Tick += Zamanlayici;
            Zaman.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Zaman.Start();
        }
        private class YildizGrubu
        {
            public int BaslangicX { get; set; }
            public int BaslangicY { get; set; }
            public int Uzunluk { get; set; }
            public bool YatayMi { get; set; }
            public int TiklananYildizlar { get; set; }
        }

        private void Zamanlayici(object sender, EventArgs e)
        {
            saniye++;

            if (saniye == 60)
            {
                dakika++;
                saniye = 0;
            }

            label1.Text = $"{dakika:D2}:{saniye:D2}";
        }
        private void SureKontrol()
        {
            int mevcutSure = dakika * 60 + saniye;
            pictureBox9.Visible = true;
            if (mevcutSure < enIyiSure)
            {
                enIyiSure = mevcutSure; 
                label2.Text = $"Tebrikler! Yeni en iyi süre = {dakika:D2}:{saniye:D2}";
                pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox9.Image = Image.FromFile(@"C:\Users\defne\Desktop\Görsel Progarmlama C#\GörselProgramlamaProjesi\AMIRAL_BATTI_OYUNU_\GörsellerVeSesler\mutlu.png");
            }
            else
            {
                pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;
                label2.Text = $"Tebrikler! Süre = {dakika:D2}:{saniye:D2}.Fakat en iyisi degildi.";
                pictureBox9.Image = Image.FromFile(@"C:\Users\defne\Desktop\Görsel Progarmlama C#\GörselProgramlamaProjesi\AMIRAL_BATTI_OYUNU_\GörsellerVeSesler\uzgun.png");
            }
        }
        private void TahtayiHazirla()
        {
            butonlar = new Button[tahtaBoyutu, tahtaBoyutu];
            int butonBoyutu = 60; 
            int baslangicX = 543; 
            int baslangicY = 79; 

            Random rastgele = new Random(); 
            butonTiklamaDurumu = new bool[tahtaBoyutu, tahtaBoyutu];
            for (int i = 0; i < tahtaBoyutu; i++)
            {
                for (int j = 0; j < tahtaBoyutu; j++)
                {
                    butonlar[i, j] = new Button();
                    butonlar[i, j].Width = butonBoyutu;
                    butonlar[i, j].Height = butonBoyutu;
                    butonlar[i, j].Left = baslangicX + j * butonBoyutu;
                    butonlar[i, j].Top = baslangicY + i * butonBoyutu;
                    butonlar[i, j].Tag = new Point(i, j);
                    butonlar[i, j].Click += Buton_Tikla; 
                    butonlar[i, j].MouseEnter += Buton_MouseGiris; 
                    butonlar[i, j].MouseLeave += Buton_MouseCikis; 

                    butonlar[i, j].ForeColor = Color.Transparent; 

                    butonlar[i, j].FlatAppearance.BorderColor = Color.LightPink;
                    butonlar[i, j].FlatAppearance.BorderSize = 5;
                    butonTiklamaDurumu[i, j] = false;
                    Controls.Add(butonlar[i, j]);
                }
            }

            YildizGruplariniYerlestir(rastgele, 4, 1);

            YildizGruplariniYerlestir(rastgele, 3, 2);

            YildizGruplariniYerlestir(rastgele, 2, 3);

            YildizGruplariniYerlestir(rastgele, 1, 4);
        }

        private void YildizGruplariniYerlestir(Random rastgele, int uzunluk, int adet)
        {
            for (int k = 0; k < adet; k++)
            {
                int rastgeleX = rastgele.Next(0, tahtaBoyutu);
                int rastgeleY = rastgele.Next(0, tahtaBoyutu);
                bool yatayYerlesim = rastgele.Next(2) == 0;

                YildizGrubu yildizGrubu = new YildizGrubu
                {
                    BaslangicX = rastgeleX,
                    BaslangicY = rastgeleY,
                    Uzunluk = uzunluk,
                    YatayMi = yatayYerlesim,
                    TiklananYildizlar = 0
                };

                yildizGruplari.Add(yildizGrubu);
                if (yatayYerlesim && YildizGrubunuYerlestirebilirMi(rastgeleX, rastgeleY, uzunluk, true))
                {
                    for (int l = 0; l < uzunluk; l++)
                    {
                        butonlar[rastgeleX, rastgeleY + l].Text = "*";
                    }

                    toplamYildiz += uzunluk;
                }
                else if (!yatayYerlesim && YildizGrubunuYerlestirebilirMi(rastgeleX, rastgeleY, uzunluk, false))
                {
                    for (int l = 0; l < uzunluk; l++)
                    {
                        butonlar[rastgeleX + l, rastgeleY].Text = "*";
                    }

                    toplamYildiz += uzunluk; 
                }
                else
                {
                    k--;
                }
            }
        }

        private bool YildizGrubunuYerlestirebilirMi(int x, int y, int uzunluk, bool yatay)
        {
            if (yatay)
            {
                for (int l = 0; l < uzunluk; l++)
                {
                    if (y + l >= tahtaBoyutu || butonlar[x, y + l].Text == "*")
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int l = 0; l < uzunluk; l++)
                {
                    if (x + l >= tahtaBoyutu || butonlar[x + l, y].Text == "*")
                    {
                        return false;
                    }
                }
            }

            for (int i = -1; i <= uzunluk; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int currentX = yatay ? x : x + i;
                    int currentY = yatay ? y + i : y;

                    if (currentX >= 0 && currentX < tahtaBoyutu && currentY >= 0 && currentY < tahtaBoyutu && butonlar[currentX, currentY].Text == "*")
                    {
                        return false;
                    }

                    if (currentX >= 0 && currentX < tahtaBoyutu && currentY + j >= 0 && currentY + j < tahtaBoyutu && butonlar[currentX, currentY + j].Text == "*")
                    {
                        return false;
                    }

                    if (currentY >= 0 && currentY < tahtaBoyutu && currentX + j >= 0 && currentX + j < tahtaBoyutu && butonlar[currentX + j, currentY].Text == "*")
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private void CevreyiDoldur(YildizGrubu yildizGrubu)
        {
            int baslangicX = yildizGrubu.BaslangicX;
            int baslangicY = yildizGrubu.BaslangicY;
            int uzunluk = yildizGrubu.Uzunluk;
            bool yatayMi = yildizGrubu.YatayMi;

            string imagePath = @"C:\Users\defne\Desktop\Görsel Progarmlama C#\GörselProgramlamaProjesi\AmiralBattiOyunu2\GörsellerVeSesler\Karalama.jpeg";

            if (!File.Exists(imagePath))
            {
                MessageBox.Show("Alternatif görsel dosyasý bulunamadý!");
                return;
            }

            for (int i = 0; i < uzunluk; i++)
            {
                int currentX = yatayMi ? baslangicX : baslangicX + i;
                int currentY = yatayMi ? baslangicY + i : baslangicY;

                if (butonlar[currentX, currentY].Text == "*" && butonlar[currentX, currentY].Image == null)
                {
                    butonlar[currentX, currentY].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX, currentY].Size);
                }

                if (currentY - 1 >= 0 && butonlar[currentX, currentY - 1].Text != "*" && butonlar[currentX, currentY - 1].Image == null)
                {
                    butonlar[currentX, currentY - 1].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX, currentY - 1].Size);
                }

                if (currentY + 1 < tahtaBoyutu && butonlar[currentX, currentY + 1].Text != "*" && butonlar[currentX, currentY + 1].Image == null)
                {
                    butonlar[currentX, currentY + 1].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX, currentY + 1].Size);
                }

                if (currentX - 1 >= 0 && butonlar[currentX - 1, currentY].Text != "*" && butonlar[currentX - 1, currentY].Image == null)
                {
                    butonlar[currentX - 1, currentY].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX - 1, currentY].Size);
                }

                if (currentX + 1 < tahtaBoyutu && butonlar[currentX + 1, currentY].Text != "*" && butonlar[currentX + 1, currentY].Image == null)
                {
                    butonlar[currentX + 1, currentY].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX + 1, currentY].Size);
                }

                if (currentX - 1 >= 0 && currentY - 1 >= 0 && butonlar[currentX - 1, currentY - 1].Text != "*" && butonlar[currentX - 1, currentY - 1].Image == null)
                {
                    butonlar[currentX - 1, currentY - 1].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX - 1, currentY - 1].Size);
                }

                if (currentX + 1 < tahtaBoyutu && currentY + 1 < tahtaBoyutu && butonlar[currentX + 1, currentY + 1].Text != "*" && butonlar[currentX + 1, currentY + 1].Image == null)
                {
                    butonlar[currentX + 1, currentY + 1].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX + 1, currentY + 1].Size);
                }

                if (currentX + 1 < tahtaBoyutu && currentY - 1 >= 0 && butonlar[currentX + 1, currentY - 1].Text != "*" && butonlar[currentX + 1, currentY - 1].Image == null)
                {
                    butonlar[currentX + 1, currentY - 1].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX + 1, currentY - 1].Size);
                }

                if (currentX - 1 >= 0 && currentY + 1 < tahtaBoyutu && butonlar[currentX - 1, currentY + 1].Text != "*" && butonlar[currentX - 1, currentY + 1].Image == null)
                {
                    butonlar[currentX - 1, currentY + 1].Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), butonlar[currentX - 1, currentY + 1].Size);
                }
            }
        }
        private void Buton_Tikla(object sender, EventArgs e)
        {
            Button tiklananButon = (Button)sender;
            Point butonPozisyonu = (Point)tiklananButon.Tag;

            if (butonTiklamaDurumu[butonPozisyonu.X, butonPozisyonu.Y])
            {
                return;
            }

            butonTiklamaDurumu[butonPozisyonu.X, butonPozisyonu.Y] = true;

            if (tiklananButon.Text == "*")
            {
                string imagePath = @"C:\Users\defne\Desktop\Görsel Progarmlama C#\GörselProgramlamaProjesi\AmiralBattiOyunu2\GörsellerVeSesler\Gemi.jpg";

                if (File.Exists(imagePath))
                {
                    tiklananButon.Image = ResimBoyutunuAyarla(Image.FromFile(imagePath), tiklananButon.Size);

                    tiklananYildizlar++; 

                    YildizGrubu bulunanGrup = null;
                    foreach (var yildizGrubu in yildizGruplari)
                    {
                        if ((yildizGrubu.YatayMi && yildizGrubu.BaslangicX == butonPozisyonu.X && butonPozisyonu.Y >= yildizGrubu.BaslangicY && butonPozisyonu.Y < yildizGrubu.BaslangicY + yildizGrubu.Uzunluk) ||
                            (!yildizGrubu.YatayMi && yildizGrubu.BaslangicY == butonPozisyonu.Y && butonPozisyonu.X >= yildizGrubu.BaslangicX && butonPozisyonu.X < yildizGrubu.BaslangicX + yildizGrubu.Uzunluk))
                        {
                            bulunanGrup = yildizGrubu;
                            break;
                        }
                    }

                    if (bulunanGrup != null)
                    {
                        bulunanGrup.TiklananYildizlar++;

                        if (bulunanGrup.TiklananYildizlar == bulunanGrup.Uzunluk)
                        {
                            if (bulunanGrup.Uzunluk == 4)
                            {
                                pictureBox5.Visible = true;
                            }
                            else if (bulunanGrup.Uzunluk == 3)
                            {
                                ucYildiz++;
                                if (ucYildiz == 2)
                                    pictureBox4.Visible = true;
                            }
                            else if (bulunanGrup.Uzunluk == 2)
                            {
                                ikiYildiz++;
                                if (ikiYildiz == 3)
                                    pictureBox3.Visible = true;
                            }
                            else
                            {
                                birYildiz++;
                                if (birYildiz == 4)
                                    pictureBox2.Visible = true;
                            }

                            CevreyiDoldur(bulunanGrup);
                            bulunanGrup.TiklananYildizlar = 0;
                        }
                    }

                    if (tiklananYildizlar == toplamYildiz)
                    {
                        OyunTahtasiniGizle();
                    }
                }
                else
                {
                    MessageBox.Show("Görsel dosyasý bulunamadý!");
                }
            }
            else
            {
                string alternativeImagePath = @"C:\Users\defne\Desktop\Görsel Progarmlama C#\GörselProgramlamaProjesi\AmiralBattiOyunu2\GörsellerVeSesler\Karalama.jpeg";

                if (File.Exists(alternativeImagePath))
                {
                    tiklananButon.Image = ResimBoyutunuAyarla(Image.FromFile(alternativeImagePath), tiklananButon.Size);
                }
                else
                {
                    MessageBox.Show("Alternatif görsel dosyasý bulunamadý!");
                }
            }
        }


        private void OyunTahtasiniGizle()
        {
            Zaman.Stop();
            toplamYildiz = 0;
            tiklananYildizlar = 0;
            birYildiz = 0;
            ikiYildiz = 0;
            ucYildiz = 0;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            label1.Visible = false;
            for (int i = 0; i < tahtaBoyutu; i++)
            {
                for (int j = 0; j < tahtaBoyutu; j++)
                {
                    butonlar[i, j].Visible = false;
                }
            }
            SureKontrol();
            label2.Visible = true;

            button1.Visible = true;
            pictureBox7.Visible = true;
            pictureBox8.Visible = true;
        }
        private void Buton_MouseGiris(object sender, EventArgs e)
        {
            Button üzerindeDurulanDüðme = (Button)sender;
            üzerindeDurulanDüðme.BackColor = Color.White; 
        }

        private void Buton_MouseCikis(object sender, EventArgs e)
        {
            Button ayrilanDüðme = (Button)sender;
            ayrilanDüðme.BackColor = Color.FromArgb(5, Color.Pink); 
        }

        private Image ResimBoyutunuAyarla(Image resim, Size boyut)
        {
            return new Bitmap(resim, boyut);
        }

        private void OyunuSifirla()
        {
            label2.Visible = false;
            button1.Visible = false;
            Zaman.Stop();
            dakika = 0;
            saniye = 0;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
            for (int i = 0; i < tahtaBoyutu; i++)
            {
                for (int j = 0; j < tahtaBoyutu; j++)
                {
                    butonlar[i, j].Visible = false;
                    butonlar[i, j].Image = null;
                    butonTiklamaDurumu[i, j] = false;
                }
            }
            yildizGruplari.Clear();
            TahtayiHazirla();
            Zaman.Start();
            label1.Visible = true;
            pictureBox1.Visible = true;
            pictureBox6.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OyunuSifirla();
        }

    }
}