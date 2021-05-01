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
    public partial class AsiriYukleme : Form
    {
        public AsiriYukleme()
        {
            InitializeComponent();
        }

        private string Add(string durum1, string durum2)
        {
            string ekle = durum1 + durum2;
            textBox1.Text = ekle.ToString();
            return ekle;
        }
        private int Added(int d1)
        {
            int Added = d1;
            textBox2.Text = Added.ToString();
            return Added;

        }

        private void AsiriYukleme_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked==true)
            {
                listBox1.Items.Add(textBox1.Text);
            }
            else if (radioButton2.Checked==true)
            {
                listBox1.Items.Add(textBox2.Text);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            this.Hide();
            mainPage.Show();

        }
    }
}
