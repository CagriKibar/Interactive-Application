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
    public partial class RecursiveOrnek : Form
    {
        public RecursiveOrnek()
        {
            InitializeComponent();
        }

        
        static int usAl(int taban, int us)
        {
            
            if (us==0)
            {
                return 1;

            }
            else
            {
                return taban * usAl(taban, us - 1);
            }
        }

        static int tribonacci(int sayi)
        {
            if (sayi==0)
            {
                return 0;
            }
            if (sayi==1 || sayi==2)
            {
                return 1;
            }
            else
            {
                return tribonacci(sayi - 1 + tribonacci(sayi - 2) + tribonacci(sayi - 3));
            }
        }
      

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked==true)
            {
                if (true)
                {
                    string sonucMetin = "";
                    label1.Text = "Birinci Sayiyi Giriniz";
                    int us = Convert.ToInt32(textBox1.Text);
                    label2.Text = "İkinci Sayiyi Giriniz";
                    int taban = Convert.ToInt32(textBox2.Text);

                    listBox1.Items.Add(usAl(taban, us));
                }
                
                
                
            }
           

            if (radioButton2.Checked==true)
            {
                if (true)
                {

                    label1.Text = "Tek Bir Sayi Girmeniz Yeterli";
                    int sayi = Convert.ToInt32(textBox1.Text);
                    listBox1.Items.Add(tribonacci(sayi));
                }
                else
                {
                    MessageBox.Show("Lütfen Geçerli Bir Sayi Giriniz");
                }

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            this.Hide();
            mainPage.Show();
        }
    }
}
