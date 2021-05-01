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
    public partial class GeriyeDegerDondurenMetod : Form
    {
        public GeriyeDegerDondurenMetod()
        {
            InitializeComponent();
        }

      private int toplam(int sayi1,int sayi2)
        {
            

            int sayi3 = sayi1 + sayi2;

            
            return sayi3;
        }
        private int kupBul(int s1)
        {
            int sonuc = s1 * s1 * s1;
            return sonuc;
        }

        private int dikdortgenalan(int a, int b)

        {

            int alansonuc = a * b;

            return alansonuc;

        }

        private int dikdortgencevre(int a, int b)

        {

            int cevresonuc = 2 * (a + b);

            return cevresonuc;

        }



        private void button1_Click(object sender, EventArgs e)
        {
            //label1.Text = toplam(3, 8).ToString();
            //label2.Text = toplam(4, 5).ToString();
            //label3.Text = toplam(15, 50).ToString();

            //int say1 = Convert.ToInt16(textBox1.Text);
            //label4.Text = kupBul(say1).ToString();

            if (radioButton1.Checked)
            {
                
                 
                label1.Text = label1.Text + toplam(8,9);
               
               
            }
            else if (radioButton2.Checked)
            {
                label2.Text = label2.Text + kupBul(9);
            }
            else if (radioButton3.Checked)
            {
                label3.Text = label3.Text + dikdortgenalan(3, 5);

            }
            else if (radioButton4.Checked)
            {
                label4.Text = label4.Text + dikdortgencevre(12, 17);
            }
        }

        private void GeriyeDegerDondurenMetod_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            this.Hide();
            mainPage.Show();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
