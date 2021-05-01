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
    public partial class ÇokBoyutluDiziler : Form
    {
        public ÇokBoyutluDiziler()
        {
            InitializeComponent();
        }
        Random random = new Random();

        int[,] matrisdizi=new int[4, 4];
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            

            matrisdizi[0, 0] = random.Next(1, 1);
            matrisdizi[0, 1] = random.Next(1, 1);
            matrisdizi[0, 2] = random.Next(1, 1);

            matrisdizi[1, 0] = random.Next(1, 1);
            matrisdizi[1, 1] = random.Next(1, 1);
            matrisdizi[1, 2] = random.Next(1, 1);
            matrisdizi[1, 3] = random.Next(1, 1);


            matrisdizi[2, 0] = random.Next(1, 1);
            matrisdizi[2, 1] = random.Next(1, 1);
            matrisdizi[2, 2] = random.Next(1, 1);
            matrisdizi[2, 3] = random.Next(1, 1);

            matrisdizi[3, 0] = random.Next(1, 1);
            matrisdizi[3, 1] = random.Next(1, 1);
            matrisdizi[3, 2] = random.Next(1, 1);
            matrisdizi[3, 3] = random.Next(1, 1);

            listBox1.Items.Add(matrisdizi[0, 0] + "\t" + matrisdizi[0, 1] + "\t" + matrisdizi[0, 2] + "\t" + matrisdizi[0, 3]);
            listBox1.Items.Add(matrisdizi[1, 0] + "\t" + matrisdizi[1, 1] + "\t" + matrisdizi[1, 2] + "\t" + matrisdizi[1, 3]);
            listBox1.Items.Add(matrisdizi[2, 0] + "\t" + matrisdizi[2, 1] + "\t" + matrisdizi[2, 2] + "\t" + matrisdizi[2, 3]);

            listBox1.Items.Add(matrisdizi[3, 0] + "\t" + matrisdizi[3, 1] + "\t" + matrisdizi[3, 2] + "\t" + matrisdizi[3, 3]);

            listBox2.Items.Add("Matris[0,0]=" + matrisdizi[0, 0]);
            listBox2.Items.Add("Matris[1,1]=" + matrisdizi[1, 1]);
            listBox2.Items.Add("Matris[2,2]=" + matrisdizi[2, 2]);
            listBox2.Items.Add("Matris[3,3]=" + matrisdizi[3, 3]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int toplam;
            toplam = (matrisdizi[0, 0] + matrisdizi[1, 1] + matrisdizi[2, 2] + matrisdizi[3, 3]);
            textBox1.Text = toplam.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
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
