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
using System.Xml;
using System.Text.RegularExpressions;

namespace 宿舍牌制作
{
    public partial class 主窗口 : Form
    {
        public 主窗口()
        {
            InitializeComponent();
        }
        //表示绘制时需要执行的操作
        List<string> ef = new List<string>();
        //所选择的图片队列
        List<Image> If = new List<Image>();
        //图片的文件名
        List<string> fnf = new List<string>();
        //模板所含有的照片数
        int TNOP = 0;
        /// <summary>
        /// 载入背景按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        /// <summary>
        /// 成员照片按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        /// <summary>
        /// 模板选择按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadTemplate(openFileDialog3.FileName);
                    IsButtonLoad = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    if (MessageBox.Show("所选背景的模板文件有问题！是否删除并重新制作模板？", "错误", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        File.Delete(openFileDialog3.FileName);
                        //启动模板生成器
                        new 模板生成器(Image.FromStream(openFileDialog1.OpenFile()), openFileDialog1.FileName).Show();
                    }
                }
            }
        }

        bool IsButtonLoad = false;
        /// <summary>
        /// 选择完背景图片后触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            //提取没有拓展名的文件名
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            //提取没有文件名的路径
            string Dir = Path.GetDirectoryName(openFileDialog1.FileName);
            //查找是否存在模板文件

            while (true)
                if (File.Exists(Dir + Path.DirectorySeparatorChar /*平台无关的路径分隔符*/+ fileNameWithoutExt + ".sspm") || IsButtonLoad)
                {
                    //若存在该文件
                    //载入模板

                    if (!IsButtonLoad)
                        try
                        {
                            LoadTemplate(Dir + Path.DirectorySeparatorChar /*平台无关的路径分隔符*/ + fileNameWithoutExt + ".sspm");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            if (MessageBox.Show("所选背景的模板文件有问题！是否删除并重新制作模板？", "错误", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                File.Delete(Dir + Path.DirectorySeparatorChar /*平台无关的路径分隔符*/+ fileNameWithoutExt + ".sspm");
                                //启动模板生成器
                                new 模板生成器(Image.FromStream(openFileDialog1.OpenFile()), openFileDialog1.FileName).Show();
                            }
                        }
                    //载入背景图
                    background = new Bitmap(Image.FromStream(openFileDialog1.OpenFile()));
                    pictureBox1.Image = RePaint();
                    break;
                }
                else
                {
                    //若不存在该文件
                    if (MessageBox.Show("未找到该图片对应模板！是否现在生成？", "无模板", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //启动模板生成器
                        new 模板生成器(Image.FromStream(openFileDialog1.OpenFile()), openFileDialog1.FileName).ShowDialog();
                    }
                    else if (MessageBox.Show("不生成模板？是否已有模板？", "疑问", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (openFileDialog3.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                LoadTemplate(openFileDialog3.FileName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                if (MessageBox.Show("所选背景的模板文件有问题！是否删除并重新制作模板？", "错误", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    File.Delete(openFileDialog3.FileName);
                                    //启动模板生成器
                                    new 模板生成器(Image.FromStream(openFileDialog1.OpenFile()), openFileDialog1.FileName).Show();
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
        }

        /// <summary>
        /// 载入成员照片后触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            int count = openFileDialog2.FileNames.Length;//文件数目
            foreach (string FileName in openFileDialog2.FileNames)
            {
                try
                {
                    if (If.Count == TNOP)
                    {
                        If.RemoveAt(If.Count-1);
                        
                        fnf.RemoveAt(fnf.Count-1);
                    }
                    If.Insert(0,Image.FromFile(FileName));
                    fnf.Insert(0,FileName);

                }
                catch (Exception en)
                {
                    Console.WriteLine(en);
                }
            }
            pictureBox1.Image = RePaint();
        }

        /// <summary>
        /// 从模板文件流读取模板
        /// </summary>
        /// <param name="fs">模板文件的路径</param>
        private void LoadTemplate(string fs)
        {
            //开始读取模板
            byte[] d = null;
            try
            {
                FileStream file = File.OpenRead(fs);
                d = new byte[file.Length];
                file.Read(d, 0, (int)file.Length);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "读取失败！");
                return;
            }
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
                            MessageBox.Show("该应用程序版本过低（" + Me.ToString() + "），而该图片模板由更高版本的该程序(" + mod.ToString() + ")创建，无法读取！", "版本过低！");
                            return;
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
                        Console.WriteLine("unknowLine:{0}", line);
                        break;
                }
            }
        }

        /// <summary>
        /// 打开模板编辑器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            new 模板生成器().Show();
        }

        private Image RePaint()
        {
            Bitmap image = new Bitmap(background.Width, background.Height);
            var g = Graphics.FromImage(image);

            // g.TranslateTransform(-pictureBox1.Left, -pictureBox1.Top);

            //若背景已加载则先把背景画上
            if (background != null)
                g.DrawImage(background, 0, 0);

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

                        //img = ReScaling(img, k_want);
                        //g.DrawImage(img, left, top, width, height);
                        double k_want = (double)height / width;
                        double k_img = (double)img.Height / img.Width;
                        if (k_img > k_want)
                        {
                            g.DrawImage(img, new Rectangle(left, top, width, height), 0, (int)(img.Height - img.Width * k_want) / 2, img.Width, (int)(img.Width * k_want), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(img, new Rectangle(left, top, width, height), (int)(img.Width - img.Height / k_want) / 2, 0, (int)(img.Height / k_want), img.Height, GraphicsUnit.Pixel);
                        }


                        break;
                }
            }
            g.Flush();
            g.Dispose();
            return image;
        }
        /// <summary>
        /// 图片适应
        /// </summary>
        /// <param name="img"></param>
        /// <param name="k_want"></param>
        /// <returns></returns>
        private Image ReScaling(Image img, double k_want)
        {
            double k_img = (double)img.Height / img.Width;
            Bitmap b_Img = k_img > k_want ? new Bitmap((int)(img.Width * k_want), img.Width) : new Bitmap(img.Height, (int)(img.Height / k_want));
            using (Graphics g = Graphics.FromImage(b_Img))
            {

                g.DrawImage(img, new Rectangle(0, 0, b_Img.Width, b_Img.Height),
                    k_img > k_want ?
                    new Rectangle(0, (int)((img.Height - b_Img.Height) / 2), img.Width, (int)(img.Width * k_want)) :
                    new Rectangle((int)((img.Width - b_Img.Width) / 2), 0, (int)(img.Height / k_want), img.Height),
                    GraphicsUnit.Pixel);
                g.Flush();
            }
            return b_Img;
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                Bitmap image = (Bitmap)pictureBox1.Image.Clone();
                var g = Graphics.FromImage(image);
                g.DrawImage(RePaint(), 0, 0);
                g.Flush();
                g.Dispose();
                string name = "SSP";
                foreach (string n in fnf)
                    name += n[0];
                saveFileDialog1.FileName = name;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    image.Save(saveFileDialog1.FileName);
                    toolStripLabel1.Text = "保存成功";
                    toolStripLabel1.ForeColor = Color.Green;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                toolStripLabel1.Text = "保存失败";
                toolStripLabel1.ForeColor = Color.Red;
            }
            finally
            {
                timer1.Enabled = true;//保证提示能自动消失
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //if (DateTime.Now > new DateTime(2017, 9, 9))
            //{
            //    MessageBox.Show("该调试版应用程序到期！如需请联系Denius\r\ncjd001113@outlook.com");
            //    Application.Exit();
            //}
        }

        Bitmap background = null;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
        }

        SE se = new SE();
        /// <summary>
        /// 表示文字处理引擎的类
        /// </summary>
        public class SE
        {
            Regex r1 = new Regex(@"\{[0-9]+,[0-9]+,no-dir}");//r1提供文件名的叹号分割获取服务
            Regex r2 = new Regex(@"\{[0-9]+,dir-name,[0-9]+}");//r2提供各级目录的获取服务
            /// <summary>
            /// 文字处理引擎入口
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public string StringEngine(string str, List<string> fnf)
            {
                //r1处理
                while (r1.IsMatch(str))
                {
                    try
                    {
                        string ts = r1.Match(str).Value;//匹配到的字符串
                        ts = ts.Substring(1, ts.Length - 2);//去掉大括号
                        string[] tss = ts.Split(',');//拆分两个数字字符
                        int num1 = Int32.Parse(tss[0]);
                        int num2 = Int32.Parse(tss[1]);
                        string a = null;
                        try
                        {
                            a = (fnf[num1 - 1].Substring(fnf[num1 - 1].LastIndexOf(Path.DirectorySeparatorChar /*平台无关的路径分隔符*/) + 1).Split('!')[num2 - 1]);
                        }
                        catch { }
                        str = r1.Replace(str, a == null ? "{XXX}" : a);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                //r2处理  (未测试)
                while (r2.IsMatch(str))
                {
                    try
                    {
                        string ts = r2.Match(str).Value;//匹配到的字符串
                        ts = ts.Substring(1, ts.Length - 2);//去掉大括号
                        string[] tss = ts.Split(',');//拆分各项
                        //取第一项和第三项
                        int num1 = Int32.Parse(tss[0]);
                        int num2 = Int32.Parse(tss[2]);
                        string a = null;
                        try
                        {
                            a = fnf[num1 - 1];
                            for (; num2 > 0; num2--)//共消去num2次
                            {
                                a = a.Substring(0, a.LastIndexOf(Path.DirectorySeparatorChar /*平台无关的路径分隔符*/));//每次消去路径字符串中最后一个节点（不保留最后的路径分隔符）
                            }
                            a = a.Substring(a.LastIndexOf(Path.DirectorySeparatorChar /*平台无关的路径分隔符*/) + 1);//最后去除需保留的路径节点名之前的所有部分（当然也不保留路径分隔符）
                        }
                        catch { }
                        str = r2.Replace(str, a == null ? "{DIR}" : a);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                return str;
            }

        }

        /// <summary>
        /// 自动恢复提示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripLabel1.ForeColor = Color.Gray;
            toolStripLabel1.Text = "荥";
            timer1.Enabled = false;
        }

        /// <summary>
        /// 批量工具按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("批量处理是高级功能！");
            //new 批量工具().Show();
        }

        /// <summary>
        /// 拖动添加成员照片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStrip1_DragEnter(object sender, DragEventArgs e)
        {
            // 验证拖拽到数据格式，此处验证拖拽数据是否为文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }
        /// <summary>
        /// 拖动添加成员照片
        /// </summary>
        private void toolStrip1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop); // files保存了拖拽进控件的文件的路径集合

                int count = files.Length;//文件数目
                foreach (string FileName in files)
                {
                    try
                    {
                        if (If.Count == TNOP)
                        {
                            If.RemoveAt(0);
                            fnf.RemoveAt(0);
                        }
                        If.Add(Image.FromFile(FileName));
                        fnf.Add(FileName);

                    }
                    catch (Exception en)
                    {
                        Console.WriteLine(en);
                    }
                }
                pictureBox1.Image = RePaint();
            }
            else { Console.WriteLine(e); }
        }
    }
}
