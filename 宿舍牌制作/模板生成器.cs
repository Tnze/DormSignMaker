using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace 宿舍牌制作
{
    public partial class 模板生成器 : Form
    {

        List<string> ef = new List<string>();//用多行字符串表示操作
        Image backGround = null;
        /// <summary>
        /// 表示当前编辑器所处模式
        /// none    预览模式
        /// select   选择模式
        /// lable     点选文字框模式
        /// </summary>
        string Mode = "none";
        int TNOP = 0;//人数
        public 模板生成器(Image backGround, string Filename)
        {
            InitializeComponent();

            openImage(backGround);//打开图片

            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(Filename) + ".sspm";
        }

        public 模板生成器()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
        /// <summary>
        /// 选择图片ok事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            openImage(Image.FromStream(openFileDialog1.OpenFile()));//载入图片
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + ".sspm";
        }
        /// <summary>
        /// 载入图片、窗口适应图片大小
        /// </summary>
        /// <param name="backGround">图片</param>
        private void openImage(Image backGround)
        {
            //调整窗口大小
            int width = backGround.Width;
            int height = backGround.Height;
            this.Width = this.Width - this.ClientSize.Width + 1000;
            this.Height = this.Height - this.ClientSize.Height + menuStrip1.Height + (int)(1000 * ((double)height / (double)width));
            if (this.Height > Screen.GetWorkingArea(this).Height)
            {
                this.Height = Screen.GetWorkingArea(this).Height;
                this.Width = this.Width - this.ClientSize.Width + (int)((this.ClientSize.Height - menuStrip1.Height) * ((double)width / (double)height));
            }
            else if (this.Width > Screen.GetWorkingArea(this).Width)
            {
                this.Width = Screen.GetWorkingArea(this).Width;
                this.Height = this.Height - this.ClientSize.Height + menuStrip1.Height + (int)(this.ClientSize.Width * ((double)height / (double)width));
            }
            this.backGround = (Image)backGround.Clone();
            //显示图片
            pictureBox1.Image = this.backGround;
        }

        /// <summary>
        /// 添加照片按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 照片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = "select";//切换模式
            pictureBox1.Cursor = Cursors.Cross;//切换为十字光标
            pictureBox1.BringToFront();
            TNOP++;//人数计数器+1
        }
        /// <summary>
        /// 添加文字按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 文字ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = "text";//切换模式
            pictureBox1.Cursor = Cursors.IBeam;//切换为I型光标
            pictureBox1.BringToFront();
        }

        #region 选择模式数据
        int firstPointX = 0, firstPointY = 0;
        #endregion

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//按下的是鼠标左键
            {
                switch (Mode)
                {
                    case "none":
                        break;
                    case "select":
                    case "text":
                        firstPointX = e.X; firstPointY = e.Y;
                        if (g != null)
                            g.Dispose();
                        g = pictureBox1.CreateGraphics();
                        g.TranslateTransform(-pictureBox1.Left, -pictureBox1.Top);
                        break;

                }
            }
        }
        /// <summary>
        /// 鼠标抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        TextBox tb = null;
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//松开的是鼠标左键
            {
                switch (Mode)
                {
                    case "none":
                        break;
                    case "text":
                        #region 文字
                        Mode = "none";
                        pictureBox1.Refresh();//重绘pictureBox
                                              //新建一个文字输入框
                        tb = new TextBox();
                        //设置控件大小
                        //设置AutoSize
                        //tb.AutoSize = false;
                        int ext = pictureBox1.Top, eyt = pictureBox1.Left;
                        int x1t = firstPointX + eyt, y1t = firstPointY + ext, x2t = e.X - firstPointX, y2t = e.Y - firstPointY;
                        tb.Width = x2t > 0 ? x2t : -x2t;
                        tb.Height = y2t > 0 ? y2t : -y2t;
                        tb.Top = y2t > 0 ? y1t : y1t + y2t;
                        tb.Left = x2t > 0 ? x1t : x1t + x2t;

                        fontDialog1.ShowDialog();

                        tb.Font = fontDialog1.Font;
                        tb.ForeColor = fontDialog1.Color;
                        /*
                        if (tb.Height < fontDialog1.Font.Size)//防止高度过低
                        {
                        tb.Height = (int)Math.Ceiling(fontDialog1.Font.Size);
                        }*/
                        if (tb.Width < 2 * tb.Height)//防止宽度过窄
                            tb.Width = 2 * tb.Height;
                        Controls.Add(tb);//添加控件到窗体

                        tb.Show();//显示控件
                        tb.BringToFront();//控件置顶

                        tb.Click += new EventHandler(TextBox_Click);//点击置顶

                        tb.KeyPress += new KeyPressEventHandler(TextBox_Click);//处理当用户设置好时发生的事件

                        tb.BringToFront();//控件置顶
                        #endregion
                        break;
                    case "select":
                        #region 图片
                        Mode = "none";
                        //           pictureBox1.Refresh();//重绘pictureBox
                        //计算大小
                        int ex = pictureBox1.Top, ey = pictureBox1.Left;
                        int x1 = firstPointX + ey, y1 = firstPointY + ex, x2 = e.X - firstPointX, y2 = e.Y - firstPointY;
                        int Width = x2 > 0 ? x2 : -x2;
                        int Height = y2 > 0 ? y2 : -y2;
                        int Top = y2 > 0 ? y1 : y1 + y2;
                        int Left = x2 > 0 ? x1 : x1 + x2;

                        string ctrl = "PH:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(
                            "num=" + TNOP.ToString() +
                            ";top=" + Top.ToString() +
                            ";left=" + Left.ToString() +
                            ";width=" + Width.ToString() +
                            ";height=" + Height.ToString() + ';'));
                        ef.Add(ctrl);
                        Refresh();//重绘
                        #endregion
                        break;
                }
                pictureBox1.Cursor = Cursors.Default;//恢复指针
            }
            else if (e.Button == MouseButtons.Right)//松开的是鼠标右键
            {
                tb.BringToFront();
            }
            Refresh();//重绘
        }
        /// <summary>
        /// TextBox按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Click(object sender, KeyPressEventArgs e)
        {
            if (g != null)
                g.Dispose();
            g = pictureBox1.CreateGraphics();
            g.TranslateTransform(-pictureBox1.Left, -pictureBox1.Top);
            if (e.KeyChar == 13)//若按下了回车
            {
                TextBox tb = (TextBox)sender;
                string ctrl = "LB:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    "color=" + fontDialog1.Color.ToArgb().ToString() +
                    ";font=" + FontToString(fontDialog1.Font) +
                    ";text=" + tb.Text +
                    ";left=" + tb.Left +
                    ";top=" + tb.Top + ';'));
                ef.Add(ctrl);
                g.DrawString(tb.Text, fontDialog1.Font, new SolidBrush(fontDialog1.Color), tb.Left, tb.Top);
                Controls.Remove(tb);
                g.Flush();
                g.Dispose();
                tb.Dispose();//释放TextBox
            }
        }

        /// <summary>
        /// 用String表示Font
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        static public string FontToString(Font font)
        {
            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter iFor = new BinaryFormatter();
                iFor.Serialize(ms, font);
                buff = ms.GetBuffer();
            }
            return Convert.ToBase64String(buff);
        }
        /// <summary>
        /// 从String转回Font
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        static public Font StringToFont(string font)
        {
            byte[] buff = Convert.FromBase64String(font);
            Font re;
            using (MemoryStream ms = new MemoryStream(buff))
            {
                IFormatter iFor = new BinaryFormatter();
                re = (Font)iFor.Deserialize(ms);
            }
            return re;
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            ((TextBox)sender).BringToFront();
        }
        Graphics g;

        /// <summary>
        /// 撤销按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ef.Count != 0)
                ef.RemoveAt(ef.Count - 1);
            else
            {
                MessageBox.Show("没什么好撤销的了！");
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string value = GetFileString();

            var res = saveFileDialog1.ShowDialog();
            if (res.ToString() == "OK")
            {
                using (FileStream f = File.Open(saveFileDialog1.FileName, FileMode.Create))
                {
                    byte[] va = Encoding.UTF8.GetBytes(value);
                    f.Write(va, 0, va.Length);
                    f.Flush();
                }
            }
        }
        /// <summary>
        /// 生成需保存到模板文件里的字符串
        /// </summary>
        /// <returns></returns>
        private string GetFileString()
        {
            StringBuilder a = new StringBuilder();
            a.AppendFormat("VersionInfo:{0};", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());//写入程序版本
            a.AppendFormat("TNOP:{0};", TNOP);//the number of people 人数

            foreach (string s in ef)
            {
                switch (s.Split(':')[0])
                {
                    case "LB":
                        string args = Encoding.UTF8.GetString(Convert.FromBase64String(s.Substring(3)));
                        string[] argsItem = args.Split(';');
                        string color = "";
                        string text = "";
                        int top = 0;
                        int left = 0;
                        string font = "";
                        foreach (string arg in argsItem)
                        {
                            //处理每一个操作
                            switch (arg.Split('=')[0])
                            {
                                case "color":
                                    color = arg.Substring(6);
                                    break;
                                case "font":
                                    font = (arg.Substring(5));
                                    break;
                                case "text":
                                    text = arg.Substring(5);
                                    break;
                                case "top":
                                    top = Convert.ToInt32(arg.Substring(4));
                                    break;
                                case "left":
                                    left = Convert.ToInt32(arg.Substring(5));
                                    break;
                            }
                        }
                        string ctrl = "LB:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                "color=" + color +
                                ";font=" +FontSizeChange(font) +//这一步有更改字体的大小
                                ";text=" + text +
                                ";left=" + ((int)(left * ((double)backGround.Width / pictureBox1.ClientSize.Width)) - pictureBox1.Left) +
                               ";top=" + ((int)(top * ((double)backGround.Height / pictureBox1.ClientSize.Height)) - pictureBox1.Top) + ';'));
                        a.Append(ctrl + ';');
                        break;
                    case "PH":
                        string argss = Encoding.UTF8.GetString(Convert.FromBase64String(s.Substring(3)));
                        string[] argsItemm = argss.Split(';');
                        int num = 0;
                        top = 0;
                        left = 0;
                        int width = 0;
                        int height = 0;
                        foreach (string arg in argsItemm)
                        {
                            switch (arg.Split('=')[0])
                            {
                                case "num":
                                    num = Convert.ToInt32(arg.Substring(4));
                                    break;
                                case "top":
                                    top = Convert.ToInt32(arg.Substring(4));
                                    break;
                                case "left":
                                    left = Convert.ToInt32(arg.Substring(5));
                                    break;
                                case "width":
                                    width = Convert.ToInt32(arg.Substring(6));
                                    break;
                                case "height":
                                    height = Convert.ToInt32(arg.Substring(7));
                                    break;
                            }
                        }
                        ctrl = "PH:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(
                           "num=" + num +
                           ";top=" + ((int)(top * ((double)backGround.Height / pictureBox1.ClientSize.Height)) - pictureBox1.Top) +
                           ";left=" + ((int)(left * ((double)backGround.Width / pictureBox1.ClientSize.Width)) - pictureBox1.Left) +
                           ";width=" + (int)(width * ((double)backGround.Width / pictureBox1.ClientSize.Width)) +
                           ";height=" + (int)(height * ((double)backGround.Height / pictureBox1.ClientSize.Height)) + ';'));

                        a.Append(ctrl + ';');
                        break;
                }
            }

            return a.ToString();
        }
        private string FontSizeChange(string Font)
        {
            Font font = StringToFont(Font);
            double k = ((((double)backGround.Height / pictureBox1.ClientSize.Height)) + ((double)backGround.Width / pictureBox1.ClientSize.Width)) / 2;
            font = new Font(font.FontFamily,(int) (font.Size *k), font.Style);
            return FontToString(font);
        }
        private void 模板生成器_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (g != null)
                g.Dispose();
            g = e.Graphics;

            g.TranslateTransform(-pictureBox1.Left, -pictureBox1.Top);

            int TNOP = 0;//当前人数（决定绘制时使用哪张图片）
            foreach (string s in ef)
            {
                switch (s.Split(':')[0])
                {
                    case "LB":
                        string args = Encoding.UTF8.GetString(Convert.FromBase64String(s.Substring(3)));
                        string[] argsItem = args.Split(';');
                        Color color = Color.Black;
                        Font font = new Font("微软雅黑", 5);
                        string text = "";
                        int top = 0;
                        int left = 0;
                        foreach (string arg in argsItem)
                        {
                            //处理每一个操作
                            switch (arg.Split('=')[0])
                            {
                                case "color":
                                    color = Color.FromArgb(Convert.ToInt32(arg.Substring(6)));
                                    break;
                                case "font":
                                    font = StringToFont(arg.Substring(5));
                                    break;
                                case "text":
                                    text = arg.Substring(5);
                                    break;
                                case "top":
                                    top = Convert.ToInt32(arg.Substring(4));
                                    break;
                                case "left":
                                    left = Convert.ToInt32(arg.Substring(5));
                                    break;
                            }
                        }
                        g.DrawString(text, font, new SolidBrush(color), left, top);
                        break;
                    case "PH":
                        string argss = Encoding.UTF8.GetString(Convert.FromBase64String(s.Substring(3)));
                        string[] argsItemm = argss.Split(';');
                        int num = 0;
                        top = 0;
                        left = 0;
                        int width = 0;
                        int height = 0;
                        foreach (string arg in argsItemm)
                        {
                            switch (arg.Split('=')[0])
                            {
                                case "num":
                                    num = Convert.ToInt32(arg.Substring(4));
                                    break;
                                case "top":
                                    top = Convert.ToInt32(arg.Substring(4));
                                    break;
                                case "left":
                                    left = Convert.ToInt32(arg.Substring(5));
                                    break;
                                case "width":
                                    width = Convert.ToInt32(arg.Substring(6));
                                    break;
                                case "height":
                                    height = Convert.ToInt32(arg.Substring(7));
                                    break;
                            }
                        }
                        Image img;
                        switch ((TNOP % 4) + 1)
                        {
                            case 1:
                                img = Properties.Resources.one;
                                break;
                            case 2:
                                img = Properties.Resources.two;
                                break;
                            case 3:
                                img = Properties.Resources.three;
                                break;
                            default:
                                img = Properties.Resources.fore;
                                break;
                        }
                        TNOP++;
                        g.DrawImage(img, left, top, width, height);
                        break;
                }
            }

            // g.Flush();
            // g.Dispose();
            //如果存在textbox则把它置顶
            if (tb != null)
                tb.BringToFront();
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//移动的是鼠标左键
            {
                switch (Mode)
                {
                    case "none":
                        break;
                    case "select":
                    case "text":
                        pictureBox1.Refresh();//重绘背景图片
                        Pen pen = new Pen(Color.Black, 1);
                        pen.DashStyle = DashStyle.Custom;
                        pen.DashPattern = new float[] { 3f, 2f };

                        HatchBrush brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Cyan);

                        // int ex = pictureBox1.Top, ey = pictureBox1.Left;
                        int x1 = firstPointX, y1 = firstPointY, x2 = e.X - firstPointX, y2 = e.Y - firstPointY;
                        // int absX2 = Math.Abs(x2), absY2 = Math.Abs(y2);
                        using (Graphics g = pictureBox1.CreateGraphics())
                        {
                            g.DrawRectangle(pen, x2 > 0 ? x1 : x1 + x2, y2 > 0 ? y1 : y1 + y2, x2 > 0 ? x2 : -x2, y2 > 0 ? y2 : -y2);
                            g.Flush();
                        }
                        break;
                }
            }
        }
    }
}
