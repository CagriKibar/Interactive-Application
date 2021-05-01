using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MainPage : Form
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void MainPage_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            TekBoyutluDizi tkb = new TekBoyutluDizi();
            this.Hide();
            tkb.Show();
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ÇokBoyutluDiziler çokBoyutluDiziler = new ÇokBoyutluDiziler();
            this.Hide();
            çokBoyutluDiziler.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GeriyeDegerDondurenMetod geriyeDegerDondurenMetod = new GeriyeDegerDondurenMetod();
            this.Hide();
            geriyeDegerDondurenMetod.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            GeriyeDegerDondurmeyenMetod geriyeDegerDondurmeyenMetod = new GeriyeDegerDondurmeyenMetod();
            this.Hide();
            geriyeDegerDondurmeyenMetod.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            AsiriYukleme asiriYukleme= new AsiriYukleme();
            this.Hide();
            asiriYukleme.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RecursiveOrnek recursiveOrnek = new RecursiveOrnek();
            this.Hide();
            recursiveOrnek.Show();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo adres = new ProcessStartInfo("https://github.com/CagriKibar");
            Process.Start(adres);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcessStartInfo adres = new ProcessStartInfo("https://www.linkedin.com/in/%C3%A7a%C4%9Fr%C4%B1-kibar-27a407188/");
            Process.Start(adres);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            this.Close();
            form1.Show();

            
        }
    }
}
