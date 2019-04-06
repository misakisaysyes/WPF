using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace olymipc
{

    public struct Country
    {
        public int cid;//国家编号
        public string name;//国家名称
        public int malescore;//国家男团总分
        public int femalescore;//国家女团总分
        public int score;//国家总分
    }

    public struct Item
    {
        public string name;//项目名称
        public int getnum;//前三名还是前五名
    }
    
    public class relation//关联关系
    {
        public int countryid;//国家编号
        public int itemid;//项目编号
        public int score;//比赛成绩
        public int rank;//在此项目中的排名
        public relation next=null;//构建链表的指针
    }

    public class RelationLink//链表类
    {
        public relation head; //头结点
        public RelationLink()//初始化头结点
        {
            head = new relation();
        }

        //修改国家数组中的男团总分，女团总分，总分
        public void modifyscore(Country[] country_, Item[] item_)
        {
            int[] score5 = { 7, 5, 3, 2, 1 };
            int[] score3 = { 5, 3, 2 };
            int w = 28;

            relation current = head.next;
            while (current != null)
            {
                country_[current.countryid].score = 0;
                country_[current.countryid].malescore = 0;
                country_[current.countryid].femalescore = 0;
                current = current.next;
            }

            current = head.next;
            while (current != null)
            {
                if (current.rank <= item_[current.itemid].getnum)
                    if (item_[current.itemid].getnum == 5)
                    {
                        country_[current.countryid].score += score5[current.rank - 1];
                        if (current.itemid < w)
                            country_[current.countryid].malescore += score5[current.rank - 1];
                        else
                            country_[current.countryid].femalescore += score5[current.rank - 1];
                    }
                    else
                    {
                        country_[current.countryid].score += score3[current.rank - 1];
                        if (current.itemid < w)
                            country_[current.countryid].malescore += score3[current.rank - 1];
                        else
                            country_[current.countryid].femalescore += score3[current.rank - 1];
                    }
                current = current.next;
            }
        }

        //插入一个关系记录
        public void insert(int countryid, int itemid, int score)
        {
            relation node = new relation();
            node.countryid = countryid;
            node.itemid = itemid;
            node.score = score;
            node.rank = 1;

            if (head.next == null)
                head.next = node;
            else
            {
                relation current = head.next;
                while (current != null)
                {
                    if (current.itemid == node.itemid)
                    {
                        node.rank++;
                        if (node.score > current.score)
                        {
                            int tcid = current.countryid;
                            current.countryid = node.countryid;
                            node.countryid = tcid;

                            int ts = current.score;
                            current.score = node.score;
                            node.score = ts;
                        }
                    }
                    if (current.next == null)
                    {
                        current.next = node;
                        break;
                    }
                    current = current.next;
                }
            }
        }

        //取一个关系中的信息
        public int get_info(int countryid, int itemid, string type)
        {
            int r=-10;
            relation current = head.next;
            while (current != null)
            {
                if (current.countryid == countryid)
                {
                    r = -5;
                    if (current.itemid == itemid)
                    {
                        if (type == "itemid")
                            return current.itemid;
                        else if (type == "countryid")
                            return current.countryid;
                        else if (type == "score")
                            return current.score;
                        else if (type == "rank")
                            return current.rank;
                    }
                }
                current = current.next;
            }
            return r;
        }

        //查询某项目的前三名或者是前五名
        public string queryitemrank(int itemid,Country[] country,Item[] item)
        {
            string r = "";
            relation current = head.next;
            while (current != null)
            {
                if (current.itemid == itemid && current.rank <= item[itemid].getnum)
                {
                    string[] name = country[current.countryid].name.Split('.');
                    r += "\r\r    [rank" + current.rank.ToString() + "  " + name[1] + "]";
                }
                current = current.next;
            }
            if (r == "")
                r = "\r\r     暂无记录";
            return r;
        }
    }

        /// <summary>
        /// MainWindow.xaml 的交互逻辑
        /// </summary>
        public partial class MainWindow : Window
    {
        RelationLink link=new RelationLink();

        ComboBox clist = new ComboBox();
        ComboBox ilist = new ComboBox();
        Border inputc_border = new Border();
        TextBox inputc_box = new TextBox();
        Border inputi_border = new Border();
        TextBox inputi_box = new TextBox();
        Border inputs_border = new Border();
        TextBox inputs_box = new TextBox();
        TextBlock logoc = new TextBlock();
        TextBlock logoi = new TextBlock();
        TextBlock logos = new TextBlock();
        Border show_border = new Border();
        TextBlock tips = new TextBlock();
        Button submit = new Button();
        Button submitq = new Button();
        RadioButton rbtn1 = new RadioButton();
        RadioButton rbtn2 = new RadioButton();
        RadioButton rbtn3 = new RadioButton();
        RadioButton rbtn4 = new RadioButton();
        CheckBox cbox1 = new CheckBox();
        CheckBox cbox2 = new CheckBox();
        RichTextBox rtb = new RichTextBox();
        
        Country[] country_ = new Country[180];
        Item[] item_ = new Item[56];

        public MainWindow()
        {
            InitializeComponent();

            StreamReader sr = new StreamReader("country.txt", Encoding.Default);
            String line;
            int i = 0;
            while ((line = sr.ReadLine()) != null && i < country_.Length)
            {
                country_[i].cid = i;
                country_[i].name =i.ToString()+"."+ line.ToString();
                clist.Items.Add(country_[i].name);
                country_[i].malescore = 0;
                country_[i].femalescore = 0;
                country_[i++].score = 0;
            }

            i = 0;
            line = "";
 
            StreamReader sr_ = new StreamReader("item.txt", Encoding.Default);
            while ((line= sr_.ReadLine()) != null && i < item_.Length)
            {
                item_[i].name = i.ToString() + "." + line.ToString();
                ilist.Items.Add(item_[i].name);
                item_[i++].getnum = (i % 2 == 0) ? 3 : 5;
            }

            firsttips.Content = "1.INPUT_输入比赛结果，程序自动统计各项目的前三名或前五名\r" + "2.RANK_统计排名，按各国总分或各国男女团体总分排名显示"+"\r3.QUERY_查询某国家或某项目的比赛情况";
 
            inputi_box.LostFocus += new RoutedEventHandler(ilist_lostf);
            inputc_box.LostFocus += new RoutedEventHandler(clist_lostf);
            inputs_box.LostFocus += new RoutedEventHandler(slist_lostf);
            inputi_box.GotFocus += new RoutedEventHandler(ilist_f);
            inputc_box.GotFocus += new RoutedEventHandler(clist_f);
            inputs_box.GotFocus += new RoutedEventHandler(slist_f);
            ilist.SelectionChanged += new SelectionChangedEventHandler(ilist_selected);
            clist.SelectionChanged += new SelectionChangedEventHandler(clist_selected);

        }

        private void Clear_()
        {
            inputc_box.Text = "";
            inputi_box.Text = "";
            inputs_box.Text = "";
            rbtn1.IsChecked = true;
            rbtn2.IsChecked = false;
            rbtn3.IsChecked = false;
            rbtn4.IsChecked = false;
            cbox1.IsChecked =false;
            cbox2.IsChecked = false;
            ilist.SelectedIndex = -1;
            clist.SelectedIndex = -1;
        }

        private void ilist_lostf(object sender, RoutedEventArgs e)
        {
            string []buffer= inputi_box.Text.Split(' ');
            if (buffer[0] == null || buffer[0] == "" || buffer[0] == " " || buffer[0].Length > 2)
                inputi_box.Text = "_input_";
            else
            {
                if (int.Parse(buffer[0]) > 55)
                    inputi_box.Text = "_input_";
                else {
                    ilist.SelectedIndex = int.Parse(buffer[0]);
                    inputi_box.Text =  ilist.SelectedItem.ToString();
                }
            }
        }
        private void ilist_f(object sender, RoutedEventArgs e)
        {
            inputi_box.Text = "";
        }

        private void clist_lostf(object sender, RoutedEventArgs e)
        {
            string[] buffer = inputc_box.Text.Split(' ');
            if (buffer[0] == null || buffer[0] == "" || buffer[0] == " " || buffer[0].Length > 3)
                inputc_box.Text = "_input_";
            else
            {
                if (int.Parse(buffer[0]) > 180)
                    inputc_box.Text = "_input_";
                else {
                    clist.SelectedIndex = int.Parse(buffer[0]);
                    inputc_box.Text =  clist.SelectedItem.ToString();
                }
            }
        }
        private void clist_f(object sender, RoutedEventArgs e)
        {
            inputc_box.Text = "";
        }

        private void slist_lostf(object sender, RoutedEventArgs e)
        {
            string[] buffer = inputs_box.Text.Split(' ');
            if (buffer[0] == null || buffer[0] == "" || buffer[0] == " " || buffer[0].Length > 5)
                inputs_box.Text = "_input_";
        }
        private void slist_f(object sender, RoutedEventArgs e)
        {
            inputs_box.Text = "";
        }

        private void ilist_selected(object sender, RoutedEventArgs e)
        {
            if (ilist.SelectedIndex != -1)
                inputi_box.Text = ilist.SelectedItem.ToString();
            else
                inputi_box.Text = "";
        }

        private void clist_selected(object sender, RoutedEventArgs e)
        {
            if (clist.SelectedIndex != -1)
                inputc_box.Text =  clist.SelectedItem.ToString();
            else
                inputc_box.Text = "";
        }

        private void input_Click(object sender, RoutedEventArgs e)
        {
            Clear_();
            canv.Children.Clear();

            tips.Text = "";
            this.rtb.Document.Blocks.Clear();
            rtb.Margin = new Thickness(493, 113, 0, 0);
            rtb.SetValue(RichTextBox.StyleProperty, Application.Current.Resources["rtb"]);
            rtb.AppendText("\r  使用说明\r  项目编号范围（0-55）\r  国家编号范围（0-179）\r  分数范围（0-100）。\r  可使用下拉列表代替输入");
            canv.Children.Add(rtb);

            logoi.Margin = new Thickness(100, 136, 0, 0);
            logoi.SetValue(TextBlock.StyleProperty, Application.Current.Resources["title_tbk"]);
            logoi.Text = "项目编号:";
            canv.Children.Add(logoi);

            inputi_border.Margin = new Thickness(230, 130, 0, 0);
            inputi_border.SetValue(Border.StyleProperty, Application.Current.Resources["input_border"]);
            canv.Children.Add(inputi_border);

            inputi_box.Margin = new Thickness(235, 138, 0, 0);
            inputi_box.SetValue(TextBox.StyleProperty, Application.Current.Resources["input_textbox"]);
            canv.Children.Add(inputi_box);

            ilist.Margin = new Thickness(235, 180, 0, 0);
            canv.Children.Add(ilist);

            logoc.Margin = new Thickness(100, 248, 0, 0);
            logoc.SetValue(TextBlock.StyleProperty, Application.Current.Resources["title_tbk"]);
            logoc.Text = "国家编号:";
            canv.Children.Add(logoc);

            inputc_border.Margin = new Thickness(230, 242, 0, 0);
            inputc_border.SetValue(Border.StyleProperty, Application.Current.Resources["input_border"]);
            canv.Children.Add(inputc_border);

            inputc_box.Margin = new Thickness(235, 250, 0, 0);
            inputc_box.SetValue(TextBox.StyleProperty, Application.Current.Resources["input_textbox"]);
            canv.Children.Add(inputc_box);

            clist.Margin = new Thickness(235, 290, 0, 0);
            canv.Children.Add(clist);

            logos.Margin = new Thickness(100, 366, 0, 0);
            logos.SetValue(TextBlock.StyleProperty, Application.Current.Resources["title_tbk"]);
            logos.Text = "比赛成绩:";
            canv.Children.Add(logos);

            inputs_border.Margin = new Thickness(230, 360, 0, 0);
            inputs_border.SetValue(Border.StyleProperty, Application.Current.Resources["input_border"]);
            canv.Children.Add(inputs_border);

            inputs_box.Margin = new Thickness(235, 368, 0, 0);
            inputs_box.SetValue(TextBox.StyleProperty, Application.Current.Resources["input_textbox"]);
            canv.Children.Add(inputs_box);
 
            submit.Margin = new Thickness(120, 450, 0, 0);
            submit.SetValue(Button.StyleProperty, Application.Current.Resources["submit_btn"]);
            submit.Content = "submit";
            submit.Click += new RoutedEventHandler(submit_Click); 
            canv.Children.Add(submit);

            show_border.Margin = new Thickness(490, 110, 0, 0);
            show_border.SetValue(Border.StyleProperty, Application.Current.Resources["show_border"]);
            canv.Children.Add(show_border);
 
            canv.Children.Add(tips);

        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            this.rtb.Document.Blocks.Clear();
            tips.Text = "";
            
            String[] bi = inputi_box.Text.Split('.');
            String[] bc = inputc_box.Text.Split('.');
            String[] bs = inputs_box.Text.Split(' ');

            if (bi[0].Length<1||bi[0].Length>2|| bc[0].Length < 1 || bc[0].Length > 3 || bs[0].Length < 1 || bs[0].Length > 3)
            {
                tips.Margin = new Thickness(503, 150, 0, 0);
                tips.SetValue(TextBlock.StyleProperty, Application.Current.Resources["iFont_tbk"]);
                tips.Text = "记录添加失败";
            }
            else
            {
                link.insert(int.Parse(bc[0]),int.Parse(bi[0]),int.Parse(bs[0]));
                String[] cbuffer = country_[int.Parse(bc[0])].name.Split('.');
                String[] ibuffer = item_[int.Parse(bi[0])].name.Split('.');
                rtb.AppendText("\r    国家："+cbuffer[1]+"\r\r    比赛项目："+ibuffer[1]+"\r\r    比赛成绩："+ bs[0]+ "\r\r    目前排名："+link.get_info(int.Parse(bc[0]), int.Parse(bi[0]), "rank").ToString());
                tips.Margin = new Thickness(504, 400, 0, 0);
                tips.SetValue(TextBlock.StyleProperty, Application.Current.Resources["iFont_tbk"]);
                tips.Text = "记录添加成功";
            }
        }

        private void modifyc()
        {
            Country t;
            for (int i = 0;i< country_.Length;i++)
            {
                if(country_[i].cid!=i)
                    for (int j=i+1; j < country_.Length; j++)
                        if(country_[j].cid == i)
                        {
                            t = country_[i];
                            country_[i] = country_[j];
                            country_[j] = t;
                        }
            }
        }

        private void showrank(int f)
        {
            if (f == 2 || f == 3 || f == 4)
                for (int i = 0; i < country_.Length - 1; i++)
                    for (int j = 0; j < country_.Length - 1 - i; j++)
                    {
                        int n1 = 0, n2 = 0;
                        Country t;
                        if (f == 2)
                        {
                            n1 = country_[j].score;
                            n2 = country_[j + 1].score;
                        }
                        else if (f == 3)
                        {
                            n1 = country_[j].malescore;
                            n2 = country_[j + 1].malescore;
                        }
                        else if (f == 4)
                        {
                            n1 = country_[j].femalescore;
                            n2 = country_[j + 1].femalescore;
                        }

                        if (n1 < n2)
                        {
                            t = country_[j + 1];
                            country_[j + 1] = country_[j];
                            country_[j] = t;
                        }
                    }
            else 
                modifyc();

            string rtb_show = "国家名:[比赛项目,排名]";
            for (int ic = 0; ic < country_.Length; ic++)
            { 
                bool flag = true;
                if (link.get_info(country_[ic].cid, 0, "countryid") == -10)
                    rtb_show += "\r\r" + country_[ic].name + ":\r  [无记录,无记录]";
                else
                    for (int j = 0; j < item_.Length; j++)
                    {
                        int get_itemid = link.get_info(country_[ic].cid, j, "itemid");
                        if (get_itemid >= 0)
                        {
                            int get_rank = link.get_info(country_[ic].cid, j, "rank");
                            string[] bi = item_[j].name.Split('.');
                            if (flag == true)
                                rtb_show += "\r\r" + country_[ic].name + ":\r"+"[总:"+ country_[ic].score.ToString()+",男:"+ country_[ic].malescore.ToString()+",女:" + country_[ic].femalescore.ToString()+ "]\r  [" + bi[1] + "," + get_rank.ToString() + "]";
                            else
                                rtb_show += "\r  [" + bi[1] + "," + get_rank.ToString() + "]";
                            flag = false;
                        }
                    }
            }
            this.rtb.Document.Blocks.Clear();
            rtb.AppendText(rtb_show);
        }

        private void rbtnrank(object sender, RoutedEventArgs e)
        {
            int rankshowflag=0;

            if (rbtn1.IsChecked == true)
                rankshowflag = 1;
            else if (rbtn2.IsChecked == true)
                rankshowflag = 2;
            else if (rbtn3.IsChecked == true)
                rankshowflag = 3;
            else if (rbtn4.IsChecked == true)
                rankshowflag = 4;
            showrank(rankshowflag);
        }

        private void rank_Click(object sender, RoutedEventArgs e)
        {
            Clear_();
            canv.Children.Clear();

            link.modifyscore(country_,item_);
            this.rtb.Document.Blocks.Clear();
            tips.Text = "";
            showrank(1);

            rbtn1.Margin = new Thickness(100,150,0,0);
            rbtn1.SetValue(RadioButton.StyleProperty, Application.Current.Resources["rbtn"]);
            rbtn1.Content = "按各国编号";
            rbtn1.IsChecked = true;
            canv.Children.Add(rbtn1);

            rbtn2.Margin = new Thickness(100, 250, 0, 0);
            rbtn2.SetValue(RadioButton.StyleProperty, Application.Current.Resources["rbtn"]);
            rbtn2.Content = "按各国总分";
            canv.Children.Add(rbtn2);

            rbtn3.Margin = new Thickness(250, 150, 0, 0);
            rbtn3.SetValue(RadioButton.StyleProperty, Application.Current.Resources["rbtn"]);
            rbtn3.Content = "按各国男团总分";
            canv.Children.Add(rbtn3);

            rbtn4.Margin = new Thickness(250, 250, 0, 0);
            rbtn4.SetValue(RadioButton.StyleProperty, Application.Current.Resources["rbtn"]);
            rbtn4.Content = "按各国女团总分";
            canv.Children.Add(rbtn4);

            show_border.Margin = new Thickness(490, 110, 0, 0);
            show_border.SetValue(Border.StyleProperty, Application.Current.Resources["show_border"]);
            canv.Children.Add(show_border);

            rtb.Margin = new Thickness(495, 116, 0, 0);
            rtb.SetValue(RichTextBox.StyleProperty, Application.Current.Resources["rtb1"]);
            canv.Children.Add(rtb);

            //事件
            rbtn1.Click+= new RoutedEventHandler(rbtnrank);
            rbtn2.Click += new RoutedEventHandler(rbtnrank);
            rbtn3.Click += new RoutedEventHandler(rbtnrank);
            rbtn4.Click += new RoutedEventHandler(rbtnrank);
        }

        private void submitq_Click(object sender, RoutedEventArgs e)
        {
            modifyc();
            this.rtb.Document.Blocks.Clear();
            tips.Text = "";

            string[] cid;
            string[] iid;
            int score;
            int rank;

            iid = ilist.SelectedItem.ToString().Split('.');

            if (cbox2.IsChecked == false)
            {
                if (clist.SelectedIndex < 0 || ilist.SelectedIndex < 0)
                    tips.Text = "没有查询结果";
                else {
                    cid = clist.SelectedItem.ToString().Split('.');
                    score = link.get_info(int.Parse(cid[0]), int.Parse(iid[0]), "score");
                    if (score < 0)
                        tips.Text = "没有查询结果";
                    else
                    {
                        rank = link.get_info(int.Parse(cid[0]), int.Parse(iid[0]), "rank");
                        String[] cbuffer = country_[int.Parse(cid[0])].name.Split('.');
                        String[] ibuffer = item_[int.Parse(iid[0])].name.Split('.');
                        rtb.AppendText("\r    国家：" + cbuffer[1] + "\r\r    比赛项目：" + ibuffer[1] + "\r\r    比赛成绩：" + score.ToString() + "\r\r    目前排名：" + rank.ToString());
                    }
                }
            }
            else
            {
                if (clist.SelectedIndex < 0 || ilist.SelectedIndex < 0)
                    tips.Text = "没有查询结果";
                else {
                    rtb.AppendText("\r\r    项目：" + item_[int.Parse(iid[0])].name);
                    rtb.AppendText(link.queryitemrank(int.Parse(iid[0]), country_, item_));
                }
            }
        }

        private void query_Click(object sender, RoutedEventArgs e)
        {
            Clear_();
            canv.Children.Clear();
            this.rtb.Document.Blocks.Clear();
            tips.Text = "";

            logoi.Margin = new Thickness(100, 136, 0, 0);
            logoi.SetValue(TextBlock.StyleProperty, Application.Current.Resources["title_tbk"]);
            logoi.Text = "项目编号:";
            canv.Children.Add(logoi);

            inputi_border.Margin = new Thickness(230, 130, 0, 0);
            inputi_border.SetValue(Border.StyleProperty, Application.Current.Resources["input_border"]);
            canv.Children.Add(inputi_border);

            inputi_box.Margin = new Thickness(235, 138, 0, 0);
            inputi_box.SetValue(TextBox.StyleProperty, Application.Current.Resources["input_textbox"]);
            canv.Children.Add(inputi_box);

            ilist.Margin = new Thickness(235, 180, 0, 0);
            canv.Children.Add(ilist);

            logoc.Margin = new Thickness(100, 248, 0, 0);
            logoc.SetValue(TextBlock.StyleProperty, Application.Current.Resources["title_tbk"]);
            logoc.Text = "国家编号:";
            canv.Children.Add(logoc);

            inputc_border.Margin = new Thickness(230, 242, 0, 0);
            inputc_border.SetValue(Border.StyleProperty, Application.Current.Resources["input_border"]);
            canv.Children.Add(inputc_border);

            inputc_box.Margin = new Thickness(235, 250, 0, 0);
            inputc_box.SetValue(TextBox.StyleProperty, Application.Current.Resources["input_textbox"]);
            canv.Children.Add(inputc_box);

            clist.Margin = new Thickness(235, 290, 0, 0);
            canv.Children.Add(clist);

            show_border.Margin = new Thickness(490, 110, 0, 0);
            show_border.SetValue(Border.StyleProperty, Application.Current.Resources["show_border"]);
            canv.Children.Add(show_border);
 
            rtb.Margin = new Thickness(495, 115, 0, 0);
            rtb.SetValue(RichTextBox.StyleProperty, Application.Current.Resources["rtb"]);
            canv.Children.Add(rtb);

            cbox2.Margin = new Thickness(100, 370, 0, 0);
            cbox2.SetValue(CheckBox.StyleProperty, Application.Current.Resources["cbox"]);
            cbox2.Content = "只查询项目信息";
            cbox2.IsChecked = false;
            canv.Children.Add(cbox2);

            submitq.Margin = new Thickness(120, 450, 0, 0);
            submitq.SetValue(Button.StyleProperty, Application.Current.Resources["submit_btn"]);
            submitq.Content = "submit";
            submitq.Click += new RoutedEventHandler(submitq_Click);
            canv.Children.Add(submitq);

            this.rtb.Document.Blocks.Clear();
            tips.Margin = new Thickness(505, 150, 0, 0);
            tips.SetValue(TextBlock.StyleProperty, Application.Current.Resources["iFont_tbk"]);
            canv.Children.Add(tips);

        }
    }
}
