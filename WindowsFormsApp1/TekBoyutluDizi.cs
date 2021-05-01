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
    public partial class TekBoyutluDizi : Form
    {
        public TekBoyutluDizi()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int liste, girilen;
            liste = listBox1.Items.Count;
            int[] dizi = new int[liste];
            girilen = Convert.ToInt32(textBox1.Text);
            for (int i = 0; i < liste; i++)
            {
                dizi[i] = Convert.ToInt32(listBox1.Items[i]);
            }
            label1.Text = "Eleman Sayısı=" + dizi.Length;
            label2.Text = "Boyut Sayısı=" + dizi.Rank;
            label3.Text = "Boyuttaki Eleman Sayısı=" + dizi.GetLength(0);
            label4.Text = "Kaçıncı Sırada=" + Array.IndexOf(dizi, girilen);
        }
        

        private void TekBoyutluDizi_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            this.Hide();
            mainPage.Show();
            
        }
    }
}
