using System;
using System.Security.Policy;
using System.Collections.Generic;
using System.Linq;
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

namespace B_TREE
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    public class Node
    {
        public int data;//结点数据
        public Node left = null;//左孩子指针
        public Node right = null;//右孩子指针
        public Node parent = null;//父结点指针
    }

    public class BinarySearchTree
    {
        public Node Root;//根节点
        public BinarySearchTree()//构造，根结点置null
        {
            Root = null;
        }
        public void insert(int num)//在树中插入一个新结点
        {
            Node newnode = new Node();
            newnode.data = num;

            if (Root == null)
                Root = newnode;
            else
            {
                Node current = Root;
                while (true)
                {
                    if (newnode.data < current.data)
                        if (current.left != null)
                            current = current.left;
                        else
                        {
                            newnode.parent = current;
                            current.left = newnode;
                            break;
                        }
                    else
                        if (current.right != null)
                        current = current.right;
                    else
                    {
                        newnode.parent = current;
                        current.right = newnode;
                        break;
                    }
                }
            }
        }
        public Node find(int i)//查找目标结点
        {
            Node current = Root;
            while (current != null)
            {
                if (i < current.data)
                    current = current.left;
                else if (i > current.data)
                    current = current.right;
                else
                    return current;
            }
            return null;
        }
        public void delete_(int num)//删除目标结点
        {
            Node delete_node = find(num);
            if (delete_node != null)
            {
                Node Parent = delete_node.parent;
                //叶子节点
                if (delete_node.left == null && delete_node.right == null)
                    if (Parent.left == delete_node)
                        Parent.left = null;
                    else
                        Parent.right = null;
                //有左子树
                else if (delete_node.left != null && delete_node.right == null)
                    if (Parent.left == delete_node)
                        Parent.left = delete_node.left;
                    else
                        Parent.right = delete_node.left;
                //有右子树
                else if (delete_node.left == null && delete_node.right != null)
                    if (Parent.left == delete_node)
                        Parent.left = delete_node.right;
                    else
                        Parent.right = delete_node.right;
                //有左右子树_左方最大(右子树为空)
                else
                {
                    Node flag = delete_node.left;
                    while (flag.right != null)
                        flag = flag.right;
                    delete_node.data = flag.data;
                    Parent = flag.parent;
                    if (Parent.left == flag)
                        Parent.left = flag.left;
                    else
                        Parent.right = flag.left;
                }
            }
        }
        public void cleartree()//删除整颗树
        {
            Root = null;
        }
    }

    public partial class MainWindow : Window//窗体显示
    {

        bool treecreate=false;
        bool treesearch=false;
        bool treeadd=false;
        bool treedelete=false;
        Node searchflag=null;
        BinarySearchTree node = new BinarySearchTree();
        TextBlock logo_create = new TextBlock();
        TextBlock logo_title = new TextBlock();
        Border inputnode_border = new Border();
        TextBox inputnode_box = new TextBox();
        Label input_tips = new Label();
       
        public MainWindow()//初始化窗体显示
        {
            InitializeComponent();
            firsttips.Content = "You can use this app to create a B-tree,search a node,add a node \rand delete a node,use the nav-bar above to start.";
        }

        public void paintnode_(Node node,double x,double y)//画一个树结点
        {
            var cir = new Ellipse
            {
                Stroke = new SolidColorBrush(Colors.DarkSeaGreen),
                StrokeThickness = 3,
                Fill = Brushes.White,
                Width = 34,
                Height = 34
            };
            cir.SetValue(Canvas.LeftProperty, x);
            cir.SetValue(Canvas.TopProperty, y);
            if (searchflag != null && node == searchflag)
            {
                cir.Fill = Brushes.RosyBrown;
                logo_create.Margin = new Thickness(280, 405, 0, 0);
                if(treesearch==true&&treeadd==false)
                    logo_create.Text = "\xe65f";
                else if (treesearch == true && treeadd == true)
                    logo_create.Text = "\xe660";
                canv.Children.Add(logo_create);

                logo_title.Margin = new Thickness(365, 400, 0, 0);
                logo_title.Text = "success";
                canv.Children.Add(logo_title);

                treesearch = false;
            }
            canv.Children.Add(cir);
 
            var fnum = new TextBlock
            {
                Text = node.data.ToString(),
                FontSize = 14,
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = Brushes.DarkSeaGreen
            };
            fnum.SetValue(Canvas.LeftProperty, x + 9);
            fnum.SetValue(Canvas.TopProperty, y + 9);
            canv.Children.Add(fnum);
        }

        public void show(Node theroot,double x,double y)//绘制整颗树
        {
            if (theroot != null)
            {
                paintnode_(theroot,x,y);
                if (theroot.left != null)
                {
                    var lline = new Line
                    {
                        Stroke = Brushes.DarkSeaGreen,
                        StrokeThickness = 3,
                        X1 = x + 12,
                        Y1 = y + 31,
                        X2 = x-45,
                        Y2 = y+77
                    };
                    canv.Children.Add(lline);
                }
                show(theroot.left,x-55,y+50);
                if (theroot.right != null)
                {
                    var rline = new Line
                    {
                        Stroke = Brushes.DarkSeaGreen,
                        StrokeThickness = 3,
                        X1 = x + 20,
                        Y1 = y + 31,
                        X2 = x + 77,
                        Y2 = y + 77
                    };
                    canv.Children.Add(rline);
                }
                show(theroot.right,x+55,y+50);
            }
        }

        private void input_node_keyup(object sender, KeyEventArgs e)//用户指令接受处理
        {
            if (e.Key == Key.Enter)
            {
                if (treecreate)
                {
                    string[] num = inputnode_box.Text.Split(' ');
                    for (int i = 0; i < num.Length; i++)
                        if (num[i] != null && num[i] != "")
                            node.insert(int.Parse(num[i]));
                    inputnode_box.Text = "";
                    canv.Children.Clear();
                    show(node.Root, 430, 120);

                    logo_create.Margin = new Thickness(250, 405, 0, 0);
                    logo_create.Text = "\xe649";
                    canv.Children.Add(logo_create);

                    logo_title.Margin = new Thickness(325, 400, 0, 0);
                    logo_title.Text = "completed";
                    canv.Children.Add(logo_title);

                    treecreate = false;
                }
                else if (treesearch == true)
                {
                    string[] num = inputnode_box.Text.Split(' ');
                    if (num[0] != null && num[0] != "")
                        searchflag = node.find(int.Parse(num[0]));
                    canv.Children.Clear();
                    show(node.Root, 430, 120);
                    if(treesearch)
                    {
                        logo_create.Margin = new Thickness(330, 405, 0, 0);
                        logo_create.Text = "\xe65f";
                        canv.Children.Add(logo_create);

                        logo_title.Margin = new Thickness(415, 400, 0, 0);
                        logo_title.Text = "fail";
                        canv.Children.Add(logo_title);
                    }
                    inputnode_box.Text = "";
                    searchflag = null;
                    treesearch = false;
                }
                else if(treeadd==true)
                {
                    string[] num = inputnode_box.Text.Split(' ');
                    if (num[0] != null && num[0] != "")
                    {
                        node.insert(int.Parse(num[0]));
                        searchflag = node.find(int.Parse(num[0]));
                    }
                    canv.Children.Clear();
                    treesearch = true;
                    show(node.Root, 430, 120);
                    if (treesearch)
                    {
                        logo_create.Margin = new Thickness(330, 405, 0, 0);
                        logo_create.Text = "\xe660";
                        canv.Children.Add(logo_create);

                        logo_title.Margin = new Thickness(415, 400, 0, 0);
                        logo_title.Text = "fail";
                        canv.Children.Add(logo_title);
                    }
                    inputnode_box.Text = "";
                    treesearch = false;
                    searchflag = null;
                    treeadd = false;
                }
                else if (treedelete == true)
                {
                    string[] num = inputnode_box.Text.Split(' ');
                    if (num[0] != null && num[0] != "")
                        node.delete_(int.Parse(num[0]));
                    canv.Children.Clear();
                    show(node.Root, 430, 120);
                    logo_create.Margin = new Thickness(250, 405, 0, 0);
                    logo_create.Text = "\xe64c";
                    canv.Children.Add(logo_create);

                    logo_title.Margin = new Thickness(325, 400, 0, 0);
                    logo_title.Text = "completed";
                    canv.Children.Add(logo_title);

                    inputnode_box.Text = "";
                    treedelete = false;
                }
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)//导航栏按钮,建树功能前端
        {
            //UI
            canv.Children.Clear();

            logo_create.Margin = new Thickness(290, 140, 0, 0);
            logo_create.SetValue(TextBlock.StyleProperty, Application.Current.Resources["iFont_tbk"]);
            logo_create.Text = "\xe649";
            canv.Children.Add(logo_create);

            logo_title.Margin = new Thickness(375,135,0,0);
            logo_title.SetValue(TextBlock.StyleProperty, Application.Current.Resources["title_tbk"]);
            logo_title.Text = "Create";
            canv.Children.Add(logo_title);

            inputnode_border.Margin = new Thickness(195, 280, 0, 0);
            inputnode_border.SetValue(TextBox.StyleProperty, Application.Current.Resources["input_border"]);
            canv.Children.Add(inputnode_border);

            inputnode_box.Margin = new Thickness(200,288,0,0);
            inputnode_box.SetValue(TextBox.StyleProperty,Application.Current.Resources["input_textbox"]);
            canv.Children.Add(inputnode_box);

            input_tips.Margin = new Thickness(170,390, 0, 0);
            input_tips.SetValue(Label.StyleProperty, Application.Current.Resources["input_tips_"]);
            input_tips.Content = "Please enter integers and use spaces as separator.Press the enter \rkey to end input.The first integer you enter will be the root of B-tree.";
            canv.Children.Add(input_tips);

            node.cleartree();
            treecreate = true;
            inputnode_box.KeyUp += new KeyEventHandler(input_node_keyup);
    }

        private void search_Click(object sender, RoutedEventArgs e)//导航功能按钮,查找结点功能前端
        {
            if (node.Root!=null)
            {
                canv.Children.Clear();
                show(node.Root, 430, 120);
                inputnode_border.Margin = new Thickness(195, 400, 0, 0);
                canv.Children.Add(inputnode_border);
                inputnode_box.Margin = new Thickness(200, 408, 0, 0);
                canv.Children.Add(inputnode_box);
                input_tips.Margin = new Thickness(170, 455, 0, 0);
                input_tips.Content = "Please input a number you want to find.Press the enter key to start.";
                canv.Children.Add(input_tips);

                treesearch = true;
                inputnode_box.KeyUp += new KeyEventHandler(input_node_keyup);
            }
            else
            {
                create_Click(null, null);
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)//查找功能按钮,增加功能结点前端
        {
            if (node.Root!=null)
            {
                canv.Children.Clear();
                show(node.Root, 430, 120);
                inputnode_border.Margin = new Thickness(195, 400, 0, 0);
                canv.Children.Add(inputnode_border);
                inputnode_box.Margin = new Thickness(200, 408, 0, 0);
                canv.Children.Add(inputnode_box);
                input_tips.Margin = new Thickness(170, 455, 0, 0);
                input_tips.Content = "Please input a number you want to add.Press the enter key to start.";
                canv.Children.Add(input_tips);

                treeadd = true;
                inputnode_box.KeyUp += new KeyEventHandler(input_node_keyup);
            }
            else
            {
                create_Click(null, null);
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)//功能按钮，删除结点功能前端
        {
            if (node.Root != null)
            {
                canv.Children.Clear();
                show(node.Root, 430, 120);
                inputnode_border.Margin = new Thickness(195, 400, 0, 0);
                canv.Children.Add(inputnode_border);
                inputnode_box.Margin = new Thickness(200, 408, 0, 0);
                canv.Children.Add(inputnode_box);
                input_tips.Margin = new Thickness(165, 455, 0, 0);
                input_tips.Content = "Please input a number you want to delete.Press the enter key to start.";
                canv.Children.Add(input_tips);

                treedelete = true;
                inputnode_box.KeyUp += new KeyEventHandler(input_node_keyup);
            }
            else
            {
                create_Click(null, null);
            }
        }
    }
}
