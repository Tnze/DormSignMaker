using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace 宿舍牌制作
{
    public partial class MainForm : Form
    {
        string drawScript = string.Empty;
        bool isSaved = true;
        List<Image> members = new List<Image>();
        Image background, output;

        public MainForm()
        {
            InitializeComponent();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openDSM.ShowDialog() != DialogResult.Cancel)
                drawScript = File.ReadAllText(openDSM.FileName);
            isSaved = true;
        }

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //如果还没保存
            if (!isSaved)
            {
                var result = MessageBox.Show("您还未保存！是否现在保存？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                switch (result)
                {
                    case DialogResult.Yes:
                        //这里要启动保存的程序
                        保存ToolStripMenuItem_Click(sender, e);
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            //清理窗口
            drawScript = "";
            isSaved = false;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isSaved)
                return;

            string fileDir = openDSM.FileName;
            if (fileDir == "")
            {
                if (saveDSM.FileName == "")
                {
                    var result = saveDSM.ShowDialog();
                    if (result == DialogResult.Cancel)
                        return;
                }
                fileDir = saveDSM.FileName;
            }

            File.WriteAllText(fileDir, drawScript);
            isSaved = true;
        }

        /// <summary>
        /// 根据drawScript绘制图片和文字
        /// </summary>
        /// <param name="g">画布</param>
        private void DrawPICandTEX(Graphics g)
        {
            string[] lines = drawScript.Split('\n');
            foreach (string line in lines)
            {
                string[] words = line.Split(new char[] { ' ' }, 2);
                if (words[0] == "PIC")
                {
                    string[] args = words[1].Split(' ');
                    if (args.Length < 5)
                        throw new Exception("解析脚本时出错：参数不足：" + line);
                    int index, x, y, width, height;
                    //读取参数
                    try
                    {
                        index = Convert.ToInt32(args[0]);
                        x = Convert.ToInt32(args[1]);
                        y = Convert.ToInt32(args[2]);
                        width = Convert.ToInt32(args[3]);
                        height = Convert.ToInt32(args[4]);
                    }
                    catch
                    {
                        throw new Exception("解析脚本时出错：参数错误：" + line);
                    }
                    if (index < members.Count)
                        g.DrawImage(members[index], x, y, width, height);
                }
                else if (words[0] == "TEX")
                {

                }
            }
        }

        private void 输出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outputPicture.FileName == "")
                if (outputPicture.ShowDialog() == DialogResult.Cancel)
                    return;
            output.Save(outputPicture.FileName);
        }

        private void 背景ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openPicture.ShowDialog();
            if (result == DialogResult.Cancel)
                return;
            background = Image.FromFile(openPicture.FileName);
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            output = new Bitmap(background);
            using (Graphics g = Graphics.FromImage(output))
            {
                DrawPICandTEX(g);
            }
            pictureBox1.Image = output;
        }

        private void 载入ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 脚本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ScriptEditer se = new ScriptEditer())
            {
                se.Script = drawScript;
                se.ShowDialog();
                drawScript = se.Script;
            }
            isSaved = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null) return;
            //在toolStripStatusLabel1上显示当前坐标
            var r = GetPictureBoxZoomSize(pictureBox1);
            int x = e.X - r.X;
            int y = e.Y - r.Y;
            x = x * pictureBox1.Image.Width / (r.Width);
            y = y * pictureBox1.Image.Height / (r.Height);
            toolStripStatusLabel1.Text = "(" + x.ToString() + ", " + y.ToString() + ")";
        }

        private void 输出文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputPicture.ShowDialog();
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/GeniusJunDao/DormSignMaker");
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isSaved) return;
            if (saveDSM.ShowDialog() == DialogResult.Cancel) return;
            File.WriteAllText(saveDSM.FileName, drawScript);
            isSaved = true;
        }

        /// <summary>
        /// 获取PictureBox在Zoom下显示的位置和大小
        /// https://blog.csdn.net/zgke/article/details/4372246
        /// </summary>
        /// <param name="p_PictureBox">Picture 如果没有图形或则非ZOOM模式 返回的是PictureBox的大小</param>
        /// <returns>如果p_PictureBox为null 返回 Rectangle(0, 0, 0, 0)</returns>
        private Rectangle GetPictureBoxZoomSize(PictureBox p_PictureBox)
        {
            if (p_PictureBox != null)
            {
                PropertyInfo _ImageRectanglePropert = p_PictureBox.GetType().GetProperty("ImageRectangle", BindingFlags.Instance | BindingFlags.NonPublic);
                return (Rectangle)_ImageRectanglePropert.GetValue(p_PictureBox, null);
            }
            return new Rectangle(0, 0, 0, 0);
        }
    }
}
