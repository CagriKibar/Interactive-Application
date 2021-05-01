using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class GeriyeDegerDondurmeyenMetod : Form
    {
        public GeriyeDegerDondurmeyenMetod()
        {
            InitializeComponent();
        }

        MainPage mainPage = new MainPage();
        //void metod1()
        //{

        //    label1.Text = "20.04.2021";
        //    label2.Text = "Ramazan Sabahı";
        //    label3.Text = "İftar Menüsü: Pilav,Çorba,Kuru Fasulye";
        //    label3.Text = "Çankırı/Merkez";
        //}
        //void metod2()
        //{
        //    label1.BackColor = Color.OrangeRed;
        //    label2.BackColor = Color.Blue;
        //    label3.BackColor = Color.LightBlue;
        //    label4.BackColor = Color.PaleVioletRed;
        //}
        //void metod3()
        //{
        //    label1.ForeColor = Color.White;
        //    this.Text = "Vize Ödevim";
        //    MessageBox.Show("Vize Ödevim Bitti");
        //}

        int sayac;
        double dogru = 0, yanlıs = 0, net = 0;
        string dogruCevap = "";

        void sorular()
        {
            if (sayac==1)
            {
                label2.Text = "Hangisi Avrupa Ülkesi Değildir?";
                button1.Text = "İngiltere";
                button2.Text = "Almanyca";
                button3.Text = "Avustralya";
                button4.Text = "İsviçre";
                dogruCevap = button2.Text;


            }

            if (sayac==2)
            {
                label2.Text = "Hangi ülkenin Akdeniz'e kıyısı yoktur ?";
                button1.Text = "Mısır";
                button2.Text = "Cezayir";
                button3.Text = "Fransa";
                button4.Text = "Sırbistan";
                dogruCevap = button4.Text;
            }
            

            if (sayac==3)
            {
                label2.Text = "Varşova Paktı Hangi Tarihte Dağıtılmıştır ?";
                button1.Text = "1990";
                button2.Text = "1991";
                button3.Text = "1992";
                button4.Text = "1993";
                dogruCevap = button1.Text;
            }
            if (sayac==4)
            {
                panel1.Visible = false;
                panel2.Visible = true;
                
                label2.Text = "Aspirinin Hammaddesi Nedir?";
                button1.Text = "Söğüt";
                button2.Text = "Kavak";
                button3.Text = "Meşe";
                button4.Text = "Köknar";
                dogruCevap = button1.Text;

                btnSoruGetir.Visible = false;
                MessageBox.Show("Bilgi Yarışması Sona Erdi....");
                this.Hide();
                mainPage.Show();
                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnSoruGetir.Text = "Sonraki Soru";
            sayac++;
            label1.Text = "Soru" + sayac + ". ";
            foreach (Control item in panel1.Controls)
            {
                if (item is Button)
                {
                    item.Enabled = true;
                }
            }
            sorular();

            //metod1();
            //metod2();
            //metod3();
        }

        private void DortButton(object sender, EventArgs e)
        {
            foreach (Control item in panel1.Controls)
            {
                if (item is Button)
                {
                    item.Enabled = false;
                }
            }
            if ((sender as Button).Text==dogruCevap)
            {
                dogru++;
            }
            else
            {
                yanlıs++;
            }
            net = dogru - (yanlıs / 3);
            lblDogru.Text = "Doğru  Sayisi=" + dogru;
            lblYanlis.Text = "Yanlış Sayısı=" + yanlıs;
            lblNet.Text = "Net Sayisi=" + net;
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            this.Hide();
            mainPage.Show();

        }

        private void GeriyeDegerDondurmeyenMetod_Load(object sender, EventArgs e)
        {
            panel2.Visible = true;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
