using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 宿舍牌制作
{
    public partial class ScriptEditer : Form
    {
        public string Script { get; set; }
        public string scriptBackUp;
        public ScriptEditer()
        {
            InitializeComponent();
            Script = string.Empty;
        }

        private void ScriptEditer_Load(object sender, EventArgs e)
        {
            scriptBackUp = Script;
            string[] lines = Script.Split('\n');
            listBox1.Items.Clear();
            foreach (string line in lines)
            {
                listBox1.Items.Add(line);
            }
        }

        private void ScriptEditer_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (string s in listBox1.Items)
            {
                Script += s + "\r\n";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("TEX New line");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //从VB借一个InputBox过来^_^
            string line = Microsoft.VisualBasic.Interaction.InputBox("编辑第" + (listBox1.SelectedIndex + 1).ToString() + "条绘制命令", "编辑行", listBox1.Items[listBox1.SelectedIndex].ToString());
            if (line != "")
                listBox1.Items[listBox1.SelectedIndex] = line;
        }
    }
}
