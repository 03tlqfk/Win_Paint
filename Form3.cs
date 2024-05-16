using System;
using System.Collections;
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
    public partial class Form3 : Form
    {
        private Dictionary<string, Bitmap> dictionary;
        public event EventHandler<string> ValueSelected;
        public Form3(Dictionary<string, Bitmap> dict)
        {
            InitializeComponent();

            dictionary = dict;
            // ListBox에 Dictionary의 값들 추가
            foreach (var item in dictionary)
            {
                listBox1.Items.Add($"{item.Key}: {item.Value}");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //히스토리 선택 시 화면 바꾸게 설정
            Console.WriteLine(listBox1.SelectedItem.ToString());

            string[] separatingStrings = { ":", " " };

            string[] words = listBox1.SelectedItem.ToString().Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);


            ValueSelected?.Invoke(this, words[0]);
        }
    }
}
