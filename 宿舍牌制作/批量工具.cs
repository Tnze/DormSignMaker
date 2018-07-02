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

namespace 宿舍牌制作
{
    public partial class 批量工具 : Form
    {
        public 批量工具()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 引擎所需接口
        /// </summary>
        interface IsspCreateEngine
        {
            string OutputDir { get; set; }
            string InputDir { get; set; }
            string Background { get; set; }
            string SSPM { get; set; }
            void GO();
        }

        /// <summary>
        /// 输入文件夹浏览按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                textBox1.Text = folderBrowserDialog1.SelectedPath;
        }
        /// <summary>
        /// 输出文件夹浏览按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                textBox2.Text = folderBrowserDialog1.SelectedPath;
        }
        /// <summary>
        /// 背景图片浏览按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "全部图片 | *.jpeg; *.jpg; *.png";
            openFileDialog1.Title = "选择背景图片";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;

                //提取没有拓展名的文件名
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                //提取没有文件名的路径
                string Dir = Path.GetDirectoryName(openFileDialog1.FileName);
                //查找是否存在模板文件,若找到且模板路径为空则填入Textbox4
                if (File.Exists(Dir + Path.DirectorySeparatorChar /*平台无关的路径分隔符*/+ fileNameWithoutExt + ".sspm") && textBox4.Text == "")
                    textBox4.Text = Dir + Path.DirectorySeparatorChar /*平台无关的路径分隔符*/+ fileNameWithoutExt + ".sspm";
            }
        }
        /// <summary>
        /// 模板文件选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "SSPM模板文件|*.sspm";
            openFileDialog1.Title = "选择模板文件";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;
            }
        }

        #region 帮助按钮事件
        private void button1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("点击这里就可以开始进行批量生成了!", "帮助");
        }

        private void label1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("这是一条帮助", "帮助");
        }

        private void button3_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("点击这个按钮使得你可以用一个预设对话框选择所需要的路径", "帮助");
        }

        private void label2_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("设置批量程序进行批量操作的时候读取图片时所有图片所在的总文件夹", "帮助");
        }

        private void label3_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("设置程序会把生成的宿舍牌保存到哪里", "帮助");
        }

        private void button2_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("点击这个按钮使得程序生成一张宿舍牌使得你可以预览效果", "帮助");
        }

        private void label4_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("设置批量生成所使用的背景图片", "帮助");
        }

        private void button5_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("点击该按钮以通过一个对话框选择背景图片", "帮助");
        }

        private void button6_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("设置批量生成所使用的模板", "帮助");
        }

        private void label5_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("设置批量生成所使用的模板", "帮助");
        }

        private void label7_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("程序员无聊的时候写的一段话\r\n缩小窗口以收回", "说明");
        }
        #endregion
        /// <summary>
        /// Go按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            IsspCreateEngine i;
            switch (comboBox1.Text)
            {
                case "基础引擎v1.0":
                    i = new InEngine();
                    break;
                default:
                    i = new InEngine();
                    break;
            }
            i.GO();
        }

        /// <summary>
        /// 内置基础引擎
        /// </summary>
        class InEngine : IsspCreateEngine
        {
            //表示绘制时需要执行的操作
            List<string> ef = new List<string>();
            //所选择的图片队列
            List<Image> If = new List<Image>();
            //图片的文件名
            List<string> fnf = new List<string>();
            //模板所含有的照片数
            int TNOP = 0;
            public string OutputDir { get; set; }//输出文件夹
            public string InputDir { get; set; }//输入文件夹
            public string Background { get; set; }//背景文件
            public string SSPM { get; set; }//模板文件
            主窗口.SE se = new 主窗口.SE();//文字引擎对象
            /// <summary>
            /// 开始批量生成
            /// </summary>
            public void GO()
            {
                //①载入背景文件
                //②载入模板文件
                //③打开输出文件夹
                //④循环制作文件


                Image bg = null;
                if (Background != null && File.Exists(Background))
                    bg = Image.FromFile(Background);
                else
                    throw new Exception("bgNull");

                LoadTemplate(SSPM);

                DirectoryInfo dir = new DirectoryInfo(InputDir);
                foreach(var 班级 in dir.EnumerateDirectories())
                {
                    Console.WriteLine("Making class {0}' dormitory cards !", 班级.Name);
                    foreach(var 宿舍号 in dir.EnumerateDirectories())
                    {
                        Console.WriteLine("Making dormitory card for {0} !", 宿舍号.Name);
                        List<Image> paten = new List<Image>();
                        //加载所有成员的照片
                        foreach(var 成员照片 in 宿舍号.GetFiles())
                        {
                            try
                            {
                                Image per = Image.FromFile(成员照片.FullName);
                                paten.Add(per);
                            }
                            catch {
                                Console.WriteLine("Canot read\" {0}\" as a photo !",成员照片.FullName);
                            }
                        }

                        Image bgCopy = (Image)bg.Clone();
                        using (Graphics g = Graphics.FromImage(bgCopy))
                        {
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
                                                    font = 模板生成器.StringToFont(arg.Substring(5));
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
                                        g.DrawString(se.StringEngine(text, fnf), font, new SolidBrush(color), left, top);
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
                                        if (TNOP < If.Count)
                                        {
                                            img = If[TNOP];
                                        }
                                        else
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
                            g.Flush();
                        }
                        bgCopy.Save(OutputDir + Path.DirectorySeparatorChar + 宿舍号.Name);
                    }
                }
            }
            /// <summary>
            /// 加载模板文件
            /// </summary>
            /// <param name="fs"></param>
            private void LoadTemplate(string fs)
            {
                //开始读取模板
                byte[] d = null;

                FileStream file = File.OpenRead(fs);
                d = new byte[file.Length];
                file.Read(d, 0, (int)file.Length);

                string va = Encoding.UTF8.GetString(d);
                Console.WriteLine(va);//
                string[] lines = va.Split(';');

                foreach (string line in lines)
                {
                    string[] f = line.Split(':');
                    switch (f[0])
                    {
                        //检查版本
                        case "VersionInfo":
                            Version mod = new Version(f[1]);
                            Version Me = new Version(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                            //检查版本
                            if (mod > Me)
                            {
                                throw new Exception("该应用程序版本过低（" + Me.ToString() + "），而该图片模板由更高版本的该程序(" + mod.ToString() + ")创建，无法读取！");
                            }
                            break;
                        case "TNOP":
                            TNOP = Int32.Parse(f[1]);
                            break;
                        case "PH":
                        case "LB":
                            ef.Add(line);
                            break;
                        default:
                            Console.WriteLine("unDefineLine:{0}", line);
                            break;
                    }
                }
            }

        }
    }
}
