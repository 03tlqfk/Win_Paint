using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Win_Paint
{
    public partial class Form2 : Form
    {
        public int width = 0;
        public int height = 0;
        public Form2()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            width = Convert.ToInt16(textBox1.Text); //width value
            height = Convert.ToInt16(textBox2.Text);    //height value

            if (Owner != null && Owner is Form1)    //owner is form1
            {
                ((Form1)Owner).Form1_resize(width, height);
            }
            
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
